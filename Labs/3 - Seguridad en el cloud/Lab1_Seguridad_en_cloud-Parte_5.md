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

### Duración
TBD

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
´´´

2 - 

#### Desplegar el grupo de contenedores con el contenedor de inicialización

