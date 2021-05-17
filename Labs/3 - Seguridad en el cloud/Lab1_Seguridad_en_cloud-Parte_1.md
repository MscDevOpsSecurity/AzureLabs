| Lab |  1 |
| --  | -- |

| Parte | Título | 
| --  | -- |
| 1 | Desplegar Azure Container Instances en una red virtual |

### Lab overview

Vamos a comenzar este lab creando un ACI dentro de una red privada virtual, de manera que la API solo sea accesible por el cliente desde dentro de la propia red privada virtual. Debido a esta restricción de acceso, para testear que la API funciona correctamente, vamos a crear una máquina virtual (VM) desde la cual haremos llamadas al ACI.

La aplicación consistirá en una API que accedera a una base de datos externa, que vamos a desplegar como una base de datos Azure SQL.

Esta será la topología de nuestro lab.

![Topology lab 1](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1.png)

### Objetivos

- Desplegar un ACI que contrendrá nuestra aplicación denstro de una red privada virtual.
- Desplegar una VM dentro de la red privada virtual para testear nuestra API.
- Desplegar una base de datos Azure SQL. 

### Objetivos

- ACI: Azure Container Instance.
- VM: máquina virtual.
- SKU: pago por uso ( [Stock-keeping-Unit](https://docs.microsoft.com/en-us/azure/search/search-sku-tier))
- Fqdn (Fully Qualified Domain Name): nombre completo de dominio de un equipo específico (<nombreVM>.<dominio>.com).

### Duración
20 minutos

### Instrucciones

#### Inicializa tu entorno

1 - Lo primero es loguearte en el [portal de Azure](https://portal.azure.com/) con tu subscripción. Si no cuentas con una subscripción válida, puedes crear una cuenta gratuita en este [enlace](https://azure.microsoft.com/free/).

2 - Abrimos el Azure Cloud Shell desde el portal de Azure haciendo clic en el icono del Shell.

![ShellIcon](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_shell.png)

3 - Vamos a definir una serie de variables en el shell ejecutando las siguientes líneas. Reemplaza el variable _location_ con la que corresponda en tu caso.

```bash
# Variables
rg=acilab
location=northeurope
aci_name=learnaci
aci_dns=${aci_name}${RANDOM}
vnet_name=acivnet
vnet_prefix=192.168.0.0/16
vm_subnet_name=vm
vm_subnet_prefix=192.168.1.0/24
aci_subnet_name=aci
aci_subnet_prefix=192.168.2.0/24
```

4 - Creamos un grupo de recursos y la VM que usaremos para testear la API. En este ejemplo, dejaremos que sea Azure quien cree la red virtual al mismo tiempo que desplegamos la VM.

```bash
# Crear RG y la VM después
az group create -n $rg -l $location
az vm create -n test-vm -g $rg -l $location --image ubuntuLTS --generate-ssh-keys \
    --public-ip-address test-vm-pip --vnet-name $vnet_name \
    --vnet-address-prefix $vnet_prefix --subnet $vm_subnet_name --subnet-address-prefix $vm_subnet_prefix
vm_pip=$(az network public-ip show -n test-vm-pip -g $rg --query ipAddress -o tsv) && echo $vm_pip
```
![ResourceGroup](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_RGCreated.png)
![VMCreated](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_VMCreated.png)

Verifica ahora que puedes conectarte a la VM que acabamos de desplegar por ssh.

```bash
ssh $vm_pip
[...]
exit
```

Nos preguntará si queremos incluir el fingerprint de esta VM como válida para que la próxima vez acceda directamente, le decimos "_yes_" y enter.

![VMssh](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_VMssh.png)

5 - Creamos la base de datos a la que se conectará la aplicación. Vamos a utilizar el SKU (básico) mas reducido de Azure SQL Database para reducir costes, pero ten en cuenta que este tamaño no es recomendable para entornos de producción.

```bash
# Create database
sql_server_name=sqlserver$RANDOM
sql_db_name=mydb
sql_username=azure
sql_password=$(tr -dc A-Za-z0-9 </dev/urandom 2>/dev/null | head -c 15)
az sql server create -n $sql_server_name -g $rg -l $location --admin-user $sql_username --admin-password $sql_password
sql_server_fqdn=$(az sql server show -n $sql_server_name -g $rg -o tsv --query fullyQualifiedDomainName)
az sql db create -n $sql_db_name -s $sql_server_name -g $rg -e Basic -c 5 --no-wait
```

> Si os sentís más comodos, podéis poner en la variable _$sql_password_ lo que os guste más, para poder acordaros luego mejor, sino se generará una aleatoria.

#### Crear un Azure Container Instance dentro de una Red Virtual.

Ahora que ya tenemos todos los componentes necesarios, podemos desplegar el ACI. Vamos a utilizar el cliente de Azure para esto, donde especificaremos el **Fully-Qualified Domain Name** y las credenciales de Azure SQL Database, de manera que el ACI pueda conectarse a ella (volver a ejecutar el comando si obtenéis un error la primera vez).

```bash
# Crear ACI en una nueva subred
az network vnet subnet create -g $rg --vnet-name $vnet_name -n $aci_subnet_name --address-prefix $aci_subnet_prefix
vnet_id=$(az network vnet show -n $vnet_name -g $rg --query id -o tsv)
aci_subnet_id=$(az network vnet subnet show -n $aci_subnet_name --vnet-name $vnet_name -g $rg --query id -o tsv)
az container create -n $aci_name -g $rg -e "SQL_SERVER_USERNAME=$sql_username" \
  "SQL_SERVER_PASSWORD=$sql_password" \
  "SQL_SERVER_FQDN=${sql_server_fqdn}" \
  --image erjosito/sqlapi:1.0 \
  --ip-address private --ports 8080 --vnet $vnet_id --subnet $aci_subnet_id
```

2 - Ahora puedes conectarte para testear la VM y verificar la conectividad desde ella. Como primer paso, vamos a obtener la dirección IP del la instancia del contenedor creado con el comando _az container show_. 

Date cuenta que la IP que se obtenga sera privada, por tanto necesitarás acceder a ella desde la VM. 

La API desplegada tendrá un punto de acceso (endpoint) **/api/healthcheck** que nos devolverá _OK_ si el contenedor está levantado y listo para usarse.

```bash
# Testea accesibilidad del contenedor
aci_ip=$(az container show -n $aci_name -g $rg --query 'ipAddress.ip' -o tsv) && echo $aci_ip
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -s http://$aci_ip:8080/api/healthcheck"
```

![Test first part](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_TestACI.png)

![Test first part OK](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_TestACI_OK.png)

> Nota: las opciones sugeridad en el comando ssh anterior (-n -o BatchMode=yes -o StrictHostKeyChecking=no) no son importantes para esta parte, pero son de bastante ayuda para enviar varios comandosa una VM remota mediante ssh.

3 - Antes de que la aplicación pueda conectarse con el backend de la base de datos, tendremos que actualizar algunas reglas del Azure SQL Firewall para que la API tenga permisos de conexión. La conexión utilizará la IP pública, así que es importante saber qué dirección IP utilizará la aplicación cuando acceda a internet.

La aplicación tiene un endpoint **api/ip** que muestra algunas de las características de red, incluyendo su IP pública.

Tengamos en cuenta que la dirección IP estática de un ACI no es fácil de conseguir ni trivial, pero en este caso, el código de la aplicación (la API) tiene la forma de encontrarlo.

Utilizaremos la dirección IP estática obtenida para actualizar la base de datos Azure SQL. Después de esto, la aplicación debería ser capaz de acceder a dicha base de datos Azure SQL, lo que podremos verificar con otro endpoint de la aplicación **api/sqlversion**, que mostrará la versión de la base de datos objeto de este lab.

```bash
# Actualizar reglas de Azure SQL firewall y testear la API
aci_pip=$(ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -s http://$aci_ip:8080/api/ip" | jq -r .my_public_ip) && echo $aci_pip
az sql server firewall-rule create -g $rg -s $sql_server_name -n public_sqlapi_aci-source --start-ip-address $aci_pip --end-ip-address $aci_pip
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -s http://$aci_ip:8080/api/sqlversion"
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -s http://$aci_ip:8080/api/sqlsrcip"
```

![SQL Firewall](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_SQLFirewall.png)

|![SQL Firewall sin regla](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_SQLSinRules.png)|![SQL Firewall con regla](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part1_SQLConRules.png)|
| --  | -- |
| Azure SQL Firewall sin regla | Azure SQL Firewall con regla |

4 - Como puedes ver en la salida del último comando del bloque anterior, la base de datos de Azure SQL puede ver la aplicación a través de su IP pública. Podrás ver esto llamando al endpoint de la aplicación **api/sqlsrcip**, el cual manda una consulta SQL al backend de la base de datos preguntando por la dirección IP tal cual la ve la base de datos.

Como siguiente punto, puedes eliminar el ACI creado en esta parte del lab.

```bash
az container delete -n $aci_name -g $rg -y
```
