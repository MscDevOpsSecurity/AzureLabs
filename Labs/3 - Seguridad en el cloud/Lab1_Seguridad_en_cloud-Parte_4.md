| Lab |  1 |
| --  | -- |

| Parte | Título | 
| --  | -- |
| 4 | Acceder a servicios habilitados mediante el uso de private links desde Azure Container Instances. |

### Lab overview

Nuestros clientes se han dado cuenta de que la base de datos detrás de la API está expuesta con una dirección IP pública. Auqnue sabemos que esta dirección IP pública está protegida por un firewall para que solo la ACI pueda acceder a ella, se nos pide que use direcciones IP privadas entre la API y la base de datos.

Normalmente, se puede acceder a los servicios de Azure PaaS con un punto de conexión público, utilizando una dirección IP pública accesible a través de la Internet pública. Sin embargo, muchos servicios de Azure también admiten la creación de puntos de conexión privados, donde el servicio de Azure solo es accesible desde el interior de una red virtual. Crearemos un punto de conexión privado para la base de datos SQL de Azure creada en ejercicios anteriores y nos aseguraremos de que el contenedor aún pueda alcanzarlo.

Azure Private Link es una tecnología que se puede usar para asegurar la conectividad a un recurso de Azure PaaS como Azure SQL Database. Anteriormente, hemos probado la aplicación accediendo a una base de datos que está disponible usando una dirección IP pública. Esta dirección IP pública está protegida por las reglas de firewall de Azure SQL, pero la comunicación se puede restringir aún más para usar solo direcciones IP privadas.

El DNS desempeña un papel fundamental en la funcionalidad requerida, ya que el sistema que accede a la base de datos SQL (el ACI que aloja la aplicación) deberá resolver el nombre de dominio completo (FQDN) de Azure SQL a su IP privada, en lugar de a su IP pública. 

![Imagen topologia parte 4](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part4.png)

### Objetivos

El objetivo es ser capaz de acceder desde el ACI a la base de datos Azure SQL utilizando su dirección IP privada. De esta manera aseguramos aún más el acceso a la misma, que ya de por sí está protegido por el firewall propio.

### Conceptos

- FQDN (Fully Qualified Domain Name): nombre completo de dominio de un equipo específico (\<nombreVM\>.\<dominio\>.com).

### Duración

30-45 minutos

### Instrucciones

#### Crear endpoint privado

1 - Primero crearemos una nueva subnet dentro de la Red Privada Virtual, y después crearemos el endpoint privado de Azure SQL dentro de dicha subnet.

```bash
# Crear nueva subnet para el SQL private endpoint
sql_subnet_name=sql
sql_subnet_prefix=192.168.3.0/24
az network vnet subnet create -g $rg --vnet-name $vnet_name -n $sql_subnet_name --address-prefix $sql_subnet_prefix
az network vnet subnet update -n $sql_subnet_name -g $rg --vnet-name $vnet_name --disable-private-endpoint-network-policies true

# SQL Server endpoint privado
sql_endpoint_name=sqlep
sql_server_id=$(az sql server show -n $sql_server_name -g $rg -o tsv --query id)
az network private-endpoint create -n $sql_endpoint_name -g $rg \
  --vnet-name $vnet_name --subnet $sql_subnet_name \
  --private-connection-resource-id $sql_server_id --group-id sqlServer --connection-name sqlConnection
```

> Nota: como se puede apreciar en el último comando, el endpoint privado está asociado a una _Red Privada Virtual_ y a una _subnet_.

2 - Ahora podemos verificar la dirección IP asignada a endpoint privado usando el comando `az network nic`, dado que los endpoint se representan en Azure como tarjetas de **Network Interface** (NICs).

```bash
# Obtener dirección IP del endpoint privado
sql_nic_id=$(az network private-endpoint show -n $sql_endpoint_name -g $rg --query 'networkInterfaces[0].id' -o tsv)
sql_endpoint_ip=$(az network nic show --ids $sql_nic_id --query 'ipConfigurations[0].privateIpAddress' -o tsv) && echo $sql_endpoint_ip
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "nslookup ${sql_server_name}.database.windows.net"
```

![nslookup to Public IP](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part4_nslookup.png)


#### DNS resolution

1- Si observamos detenidamente el resultado del comando nslookup en la sección anterior, el nombre de dominio completo de la base de datos SQL de Azure aún se resuelve dentro de la red virtual en su dirección IP pública (última línea marcada con la flecha). 

Para forzar a los sistemas implementados en la Red Virtual a usar la dirección IP privada de Azure SQL Database, necesitamos crear una zona DNS privada. Las bases de datos de Azure SQL con vínculos privados configurados utilizan el dominio intermedio `privatelink.database.windows.net`, por lo que crearemos una zona privada para este dominio y agregaremos un registro-A para la dirección IP del punto de conexión privado de Azure SQL creado anteriormente. En lugar de agregar manualmente el registro-A, conectaremos el endpoint privado y la zona DNS privada con el comando **az network private-endpoint dns-zone-group create**, de modo que el registro-A se cree automáticamente con la dirección IP correcta:

```bash
# Creamos Azure DNS private zone y los registros-A
dns_zone_name=privatelink.database.windows.net
az network private-dns zone create -n $dns_zone_name -g $rg 
az network private-dns link vnet create -g $rg -z $dns_zone_name -n myDnsLink --virtual-network $vnet_name --registration-enabled false
az network private-endpoint dns-zone-group create --endpoint-name $sql_endpoint_name -g $rg -n zonegroup --zone-name zone1 --private-dns-zone $dns_zone_name
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "nslookup ${sql_server_name}.database.windows.net"
```

> Fíjate que la VM dentro de la Red Privada Virtual, debería resolver ya el FQDN para la base de datos Azure SQL contra la dirección IP privada del endpoint privado.

2 - Si eliminaste el ACI en el ejercicio anterior, puedes volver a recrearla utilizando el mismo archivo YAML. Date cuenta que nada ha cambiado para el ACI, dado que sigue accediendo a la base de datos usando el FQDN.

```bash
az container create -g $rg --file $aci_yaml_file
```

3 - Podemos verificar que ACI esta funcionando y listo para usarse mediante el endpoint `api/healthcheck`. También podemos verificar la resolución de nombre correcta contra la dirección IP privada con el endpoint `api/dns`. Finalmente, podemos comprobar la accesibilidad de la base de datos mediante los endpoints `api/sqlversion` y `api/sqlsrcip`.

```bash
# Test
aci_ip=$(az container show -n $aci_name -g $rg --query 'ipAddress.ip' -o tsv) && echo $aci_ip
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_ip/api/healthcheck"
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_ip/api/dns?fqdn=${sql_server_name}.database.windows.net"
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_ip/api/sqlversion"
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_ip/api/sqlsrcip"
```

> Nota: Algunos servicios de Azure automáticamente deshabilitan su endpoint público cuando existe otro privado, pero no todos. En el caso de las base de datos Azure SQL, el endpoint público seguirá funcionando incluso después de configurar el privado. Para poder deshabilitar el acceso desde intenet a la base de datos, se necesita incluir más configuración en el propio firewall de Azure SQL.

4 - A través de la salida anterior en el Azure CLI, que la Azure SQL API ahora es capaz de ver que se las llamadas desde el ACI proceden de su dirección IP privada. Dado que el firewall de la base de datos de Azure SQL solo se usa para proteger el endpoint público, es por eso que no hemos tenido que cambiar o añadir ninguna regla al mismo.

![nslookup to Public IP2](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part4_nslookup2.png)

Ya podemos eliminar el contenedor y proceder a la siguiente parte del lab.

```bash
az container delete -n $aci_name -g $rg -y
```
