| Lab |  1 |
| --  | -- |

| Parte | Título | 
| --  | -- |
| 2 | Entender Azure Container Instances mediante definiciones en YAML |

### Lab overview
En esta parte, veremos como es posible replicar lo mismo que hemos hecho en la parte anterior, pero con archivos YAML. Esto nos permitirá utilizar una configuración más sofisticada dentro del ACI.

Kubernetes en un orquestador de contenedores que utiliza este tipo de archivos para describir objetos. Su popularidad ha llevado a YAML a ser el estándard de facto para definiciones declarativas de estructuras de contenedores. Si ya estas familiarizado con el YAML de Kubernetes, entonces reconoceras ciertas estructuras usadas en el YAML de ACI.

YAML es una de las muchas maneras de desplegar ACIs, otras incluyen plantillas de ARM y Terraform. No existe ninguna razón para pensar que una es mejor que la otra, pero YAML tiende a ser muy conveniente cuando trabajamos con grupos de contenedores más complejos, como el patrón sidecar que veremos más adelante.

### Objetivos

El objetivo es conseguir desplegar el mismo ACI de la parte anterior del ejercicio, pero esta vez utilizando plantillas YAML.

### Duración
30 minutos

### Instrucciones

#### Extraer el código de YAML del grupo de contenedores existente

1 - Si eliminaste el contenedor al final de la parte anterior, puedes recrearlo con el siguiente comando de Azure CLI. Si no lo borraste, puedes saltarte este paso.

```bash
# Volver a Crear Azure Container si no existe
az container create -n $aci_name -g $rg -e "SQL_SERVER_USERNAME=$sql_username" \
  "SQL_SERVER_PASSWORD=$sql_password" \
  "SQL_SERVER_FQDN=${sql_server_fqdn}" \
  --image erjosito/sqlapi:1.0 \
  --ip-address private --ports 8080 --vnet $vnet_id --subnet $aci_subnet_id
```

2 - En este punto, podemos inspeccionar ciertos valores de nuestro ACI en formato YAML usando el cliente de Azure, y salvando el código YAML en un archivo.

```bash
# Extraer a YAML
az container export -n $aci_name -g $rg -f /tmp/aci.yaml
more /tmp/aci.yaml
```

Este es un ejemplo de cómo se vería. Por ejemplo puedes usar el comando **cat /tmp/aci.yaml**.

```bash
additional_properties: {}
apiVersion: '2018-10-01'
identity: null
location: westeurope
name: learnaci
properties:
  containers:
  - name: learnaci
    properties:
      environmentVariables:
      - name: SQL_SERVER_USERNAME
        value: azure
      - name: SQL_SERVER_PASSWORD
        value: Microsoft321!
      - name: SQL_SERVER_FQDN
        value: sqlserver4341.database.windows.net
      image: erjosito/sqlapi:1.0
      ports:
      - port: 8080
        protocol: TCP
      resources:
        requests:
          cpu: 1.0
          memoryInGB: 1.5
  ipAddress:
    ip: 192.168.2.4
    ports:
    - port: 8080
      protocol: TCP
    type: Private
  networkProfile:
    id: /subscriptions/<<subscriptionid>>/resourceGroups/acilab/providers/Microsoft.Network/networkProfiles/aci-network-profile-acivnet-aci
  osType: Linux
  restartPolicy: Always
tags: {}
type: Microsoft.ContainerInstance/containerGroups
```

> Nota: Las secciones en el archivo autogenerado de YAML están ordenadas alfabéticamente.

Hay algunas características importantes de esta descripción de YAML, que merece la pena repasar.
- Ten en cuenta que YAML es muy sensible a la sangría. Si eliminas o agregas un espacio en blanco en el archivo anterior, no será sintácticamente correcto. Solo se admiten espacios para la sangría (sin tabulaciones), así que ten cuidado con el editor de texto.
- Las propiedades y los atributos se especifican jerárquicamente en pares clave-valor.
- Si estas familiarizado con Kubernetes, reconoceras muchas de las etiquetas. Por ejemplo, los _resources_ siguen la misma sintaxis. Sin embargo, no esperes que todas las propiedades sean idénticas a Kubernetes. Por ejemplo, las variables de entorno ACI se definen en la propiedad _environmentVariables_, mientras que Kubernetes usaría la palabra clave _env_.
- Si observas las variables de entorno, las veras en texto sin cifrar. Si bien esto es probablemente aceptable para la mayoría de las variables de entorno, otras no deben escribirse en texto plano, como la contraseña de SQL utilizada en este ejemplo. Una mejor forma de definir esta información confidencial sería con ACI Secure Values. En nuestro caso, no queremos que nuestro cliente pueda ver la contraseña de la base de datos, por lo que debemos enmascararla.

#### Modificar y desplegar el archivo de YAML

1 - Si bien podemos cambiar la variable de entorno para la contraseña de SQL por una variable de entorno segura mediante la CLI de Azure, usaremos YAML como preparación para los requisitos futuros. Para generar el YAML requerido, puedes editar manualmente el archivo generado automáticamente en esta unidad y volver a implementarlo para crear el ACI modificado. Utiliza tu editor de texto favorito para cambiar la línea 13 de /tmp/aci.yaml con valor: Microsoft123! por el secureValue: Microsoft123! (no cambies la sangría). En su lugar, puedes utilizar el editor de texto en línea sed para realizar el cambio:

```bash
# Modificar código auto-generado YAML
sed -i 's/        value: Microsoft123!/        secureValue: Microsoft123!/g' /tmp/aci.yaml
```

2 - Después de modificar el archivo, puedes volver a implementar el nuevo YAML. El comando **az container create** de Azure CLI toma el argumento _--file_ donde puedes indicar la descripción YAML del contenedor que se va a crear. Ten en cuenta que solo necesitas especificar el grupo de recursos donde se debe crear el nuevo ACI, ya que el resto de la información está contenida en el archivo YAML, incluido el nombre del ACI: 

```bash
# Recrear container usando el YAML actualizado 
az container delete -n $aci_name -g $rg -y
az container create -g $rg --file /tmp/aci.yaml
```

3 - Si exportas el nuevo ACI a un archivo YAML diferente _/tmp/aci2.yaml_ te darás cuenta de que los cambios que has hecho al YAML original, han sido aplicados: por ejemplo, la contraseña SQL ya no está expuesta en texto plano.

```bash
# Recrear container
az container export -n $aci_name -g $rg -f /tmp/aci2.yaml
more /tmp/aci2.yaml
```

![Secure password yaml](../..//Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part2_SecurePass.png)

4 - Como siguiente paso, podemos eliminar de nuevo el contenedor creado en esta parte, de manera que podamos movernos a la siguiente parte.

```bash
az container delete -n $aci_name -g $rg -y
```
