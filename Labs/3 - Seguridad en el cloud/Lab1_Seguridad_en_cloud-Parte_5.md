| Lab |  1 |
| --  | -- |

| Parte | Título | 
| --  | -- |
| 5 | Desplegar Azure Container Instances usando el contenedor de inicialización. |

### Lab overview

Los clientes nos han pedido la posibilidad de acceder a la API usando la FQDN en lugar de acceder a través de la dirección IP (ejercicio anterior), y además, que nos aseguremos de que la FQDN no cambia en caso de que los containers se destruyan y se vuelvan a crear. Podemos utilizar contenedores de inicialización para conseguir esta última parte.

A veces necesitamos ejecutar ciertas tareas antes de que arranque la propia aplicación. Estas tareas podrían ser muchas cosas, como por ejemplo configurar ciertos servicios para que acepten connectividad de entrada del contenedor, o inyectar secretos desde Azure Key Vault a un volumen. En esta parte del lab, vamos a utilizar un _contenedor de inicialización_ para actualizar los DNS, de manera que los clientes tengan acceso a la API utlizando el nombre de dominio, en lugar de la dirección IP.

El _contenedor de inicialización_ es uno de los usos prácticos del patrón sidecar que vimos en la parte anterior. Sin embargo, este contenedor se ejecutará antes que cualquier otro contenedor en el grupo de contenedores. Por tanto, podemos implementar diferentes conceptos usando esa inicialización, como por ejemplo una validación o tareas de inicialización.

La contenedores actuales de la aplicación en nuestro ACI solo arrancarán una vez que nuestro contenedor de inicialización haya terminado su ejecución de forma satisfactoria.

Los contenedores de inicialización ACI son el mismo concepto que los _Kubernetes Init Containers_.

![Imagen topologia parte 5](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part5.png)

### Objetivos

- Crear un contenedor de inicialización que nos devuelva la dirección IP actual del ACI, que a su vez actualizará las entradas DNS que la API utiliza para poder alcanzar el propio ACI.

### Conceptos

- Azure Files share: es la solución de Azure para el almacenamiento de archivos en la nube. Lee más [aquí](https://docs.microsoft.com/en-us/azure/storage/files/storage-files-introduction).
- Service Principal: es la forma que tiene Azure de asegurar el acceso a recursos que están bajo el control del Directorio Activo interno. Lee más [aquí](https://docs.microsoft.com/en-us/azure/active-directory/develop/app-objects-and-service-principals). 

### Duración
45 minutos

### Instrucciones

#### Crear el script de inicialización

1 - Lo primero que vamos a hacer es crear un _Azure Service Principal_ que utilizaremos con el contenedor de inicialización para interactuar con Azure, conseguir su dirección IP y actualizar la DNS. En este ejemplo, le daremos permisos de **Contributor** al grupo de recursos por razones de simplicidad, pero en entornos de producción tendremos que afinar un poco más estos permisos.

```bash
# Crear SP
scope=$(az group show -n $rg --query id -o tsv)
new_sp=$(az ad sp create-for-rbac --scopes $scope --role Contributor --name acilab -o json)
sp_appid=$(echo $new_sp | jq -r '.appId') && echo $sp_appid
sp_tenant=$(echo $new_sp | jq -r '.tenant') && echo $sp_tenant
sp_password=$(echo $new_sp | jq -r '.password')
```

2 - En este paso, crearemos la **Private DNS Zone** que utilizarán los clientes de la aplicación para acceder al ACI y vincularlo a la **Red Privada Virtual**. Esta zona DNS es diferente de la creada en el ejercicio anterior para el link privado, el cual era usado por el ACI para acceder a la base de datos de Azure SQL.

```bash
# Crear Azure DNS private zone y los registros
dns_zone_name=contoso.com
az network private-dns zone create -n $dns_zone_name -g $rg 
az network private-dns link vnet create -g $rg -z $dns_zone_name -n contoso --virtual-network $vnet_name --registration-enabled false
```

3 - Hay muchas formas de inyectar un script dentro del contenedor de inicialización. En esta parte del lab, utilizaremos **Azure Files share** para almacenarlo. Crearemos dicho Azure File share y lo subiremos el script de inicialización:

```bash
# Crear script para el contenedor de inicialización
storage_account_name="acilab$RANDOM"
az storage account create -n $storage_account_name -g $rg --sku Standard_LRS --kind StorageV2
storage_account_key=$(az storage account keys list --account-name $storage_account_name -g $rg --query '[0].value' -o tsv)
az storage share create --account-name $storage_account_name --account-key $storage_account_key --name initscript
init_script_filename=init.sh
init_script_path=/tmp/
cat <<EOF > ${init_script_path}${init_script_filename}
echo "Logging into Azure..."
az login --service-principal -u \$SP_APPID -p \$SP_PASSWORD --tenant \$SP_TENANT
echo "Finding out IP address..."
my_private_ip=\$(az container show -n \$ACI_NAME -g \$RG --query 'ipAddress.ip' -o tsv) && echo \$my_private_ip
echo "Creating DNS record..."
az network private-dns record-set a create -n \$HOSTNAME -z \$DNS_ZONE_NAME -g \$RG
az network private-dns record-set a add-record --record-set-name \$HOSTNAME -z \$DNS_ZONE_NAME -g \$RG -a \$my_private_ip
EOF
az storage file upload --account-name $storage_account_name --account-key $storage_account_key -s initscript --source ${init_script_path}${init_script_filename}
```

> Note: date cuenta que el script utiliza el Azure CLI para ejecutar los comandos que encontrarán la dirección IP del ACI and crearán los registros-A en la _Private DNS zone_. Se autenticará utilizando el _Service Principal application Id_ y el secreto que esperará encontrar como variable de entorno.

#### Desplegar el grupo de contenedores con el contenedor de inicialización

1 - En este punto, podemos crear el archivo YAML utilizando el contenido de los ejercicios anteriores. Ten en mente los siguientes aspectos:
  - Ahora tenemos la sección `initContainers` 
  - El `initContainer` ahora utiliza la imagen `microsoft/azure-clie:latest`, la cual ya contiene el Azure CLI instalado dentro
  - El contenedor de inicialización ejecuta un script almacenado en la carpeta `/mnt/init`, la cual se monta desde un volumen de Azure Files
  - El grupo de recursos, el nombre del ACI y las credenciales del Service Principal se pasan como variables de entorno.
  - El secreto del Service Principal se pasa como una variable de entorno segura.
  - El resto de definiciones del archivo de configuración YAML permanece inalterado si lo comparamos con los anteriores.
   
```bash
# Create YAML
aci_yaml_file=/tmp/acilab.yaml
cat <<EOF > $aci_yaml_file
apiVersion: 2019-12-01
location: westus
name: $aci_name
properties:
  networkProfile:
    id: $nw_profile_id
  initContainers:
  - name: azcli
    properties:
      image: microsoft/azure-cli:latest
      command:
      - "/bin/sh"
      - "-c"
      - "/mnt/init/$init_script_filename"
      environmentVariables:
      - name: RG
        value: $rg
      - name: SP_APPID
        value: $sp_appid
      - name: SP_PASSWORD
        secureValue: $sp_password
      - name: SP_TENANT
        value: $sp_tenant
      - name: DNS_ZONE_NAME
        value: $dns_zone_name
      - name: HOSTNAME
        value: $aci_name
      - name: ACI_NAME
        value: $aci_name
      volumeMounts:
      - name: initscript
        mountPath: /mnt/init/
  containers:
  - name: nginx
    properties:
      image: nginx
      ports:
      - port: 443
        protocol: TCP
      resources:
        requests:
          cpu: 1.0
          memoryInGB: 1.5
      volumeMounts:
      - name: nginx-config
        mountPath: /etc/nginx
  - name: sqlapi
    properties:
      image: erjosito/sqlapi:1.0
      environmentVariables:
      - name: SQL_SERVER_FQDN
        value: $sql_server_fqdn
      - name: SQL_SERVER_USERNAME
        value: $sql_username
      - name: SQL_SERVER_PASSWORD
        secureValue: $sql_password
      ports:
      - port: 8080
        protocol: TCP
      resources:
        requests:
          cpu: 1.0
          memoryInGB: 1
      volumeMounts:
  volumes:
  - secret:
      ssl.crt: "$ssl_crt"
      ssl.key: "$ssl_key"
      nginx.conf: "$nginx_conf"
    name: nginx-config
  - name: initscript
    azureFile:
      readOnly: false
      shareName: initscript
      storageAccountName: $storage_account_name
      storageAccountKey: $storage_account_key
  ipAddress:
    ports:
    - port: 443
      protocol: TCP
    type: Private
  osType: Linux
tags: null
type: Microsoft.ContainerInstance/containerGroups
EOF
```

2 - Podemos comprobar el contenido del YAML generado, sobre todo para asegurarnos que las variables seguras están a salvo.

```bash
more $aci_yaml_file
```

3 - En este punto, ya podemos crear el ACI. Esta vez llevará algo más de tiempo en crearse, ya que el contenedor de inicialización se ejecutará antes de que ambos, el contenedor de nginx y el de la API, se ejecuten.

```bash
az container create -g $rg --file $aci_yaml_file
```

4 - Ya solo nos queda probar los endpoints de la API para asegurarnos de que todo funciona correctamente. En esta ocasión utilizaremos el nombre de dominio para acceder el contenedor y no su dirección IP.

```bash
# Test
aci_fqdn=${aci_name}.${dns_zone_name} && echo $aci_fqdn
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "nslookup $aci_fqdn"
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_fqdn/api/healthcheck"
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_fqdn/api/sqlversion"
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_fqdn/api/sqlsrcip"
```

5 - Podemos incluso inspeccionar los logs de cada uno de los contenedores dentro de el ACI. Por ejemplo, los logs del contenedor de inicialización:

```bash
az container logs -n $aci_name -g $rg --container-name azcli
```

6 - Como siempre, el último paso es eliminarlo todo para no llevarnos sorpresas de costes en nuestra cuenta de Azure. Al eliminar el grupo de recursos, todo lo que esté contenido dentro del mismo se irá eliminando en cascada.

```bash
az group delete -n $rg -y --no-wait
```
