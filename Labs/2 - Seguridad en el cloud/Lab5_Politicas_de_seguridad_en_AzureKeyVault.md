
| Lab |  5  |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Manejo de secretos, tokens y certificados | Políticas de seguridad en Azure Key Vault |

## Lab overview

## Objetivos
El objetivo de esta práctica es ser capaces de entender ciertas políticas de seguridad aplicadas en Azure KeyVault cuando accedemos a secretos, y saber cómo usarlas en la práctica.

## Duración
 min aprox.

## Instrucciones

### Antes de comenzar

Para la preparación de este lab, os vamos a proveer de todo lo necesario para que podáis levantar la infraestructura en Azure de forma automática, mediante la ejecución de ARM templates. Esta ejecución puede hacerse de modo local o remoto.

> Local execution

```powershell
az deployment group create --name <deployment_name> --template-file template.json --parameters parameters.json --resource-group <resource_grousp>
```

> Remote execution

```bash
az deployment group create --resource-group <resource_group> --parameters <github_raw_uri> --template-uri <github_raw_uri>
```

> **NOTA**: si utilizas la ejecución remota, no pongas la uri de los recursos que os proveemos directamente, porque hay variables que tendréis que rellenar con vuestros datos, como usuario, contraseña o tenantId. Haced un fork del repositorio y ahí ya podréis cambiar los valores y hacer el commit.

Seguid el vídeo de la práctica, y volved aquí cuando llegue el momento de los "Hands on".

### Tarea 1 : Jugaremos con una sola base de datos y una VM, asignando permisos de "Vault Access Policy".

Vamos a intentar replicar la siguiente infraestructura.

![1_db_1_vm](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab5_1_db_1_vm.png)


### Tarea 2 : Jugaremos con 2 bases de datos y 2 VMs, para reproducir el problema de las "Vault Access Policy".

Vamos a intentar replicar la siguiente infraestructura, que contiene un problema de seguridad evidente.

![2_db_2_vms_issue](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab5_2_db_2_vms_issue.png)

### Tarea 3 : Jugaremos con 2 bases de datos y 2 VMs, arreglando el problema con RBAC.

Vamos a intentar replicar la siguiente infraestructura, que soluciona el issue visto en el punto anterior.

![2_db_2_vms_rbac](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab5_2_db_2_vms_rbac.png)


### Tarea Final : Eliminar todos los recursos creados.

Al final de cada ejercicio es importante dejar nuestra cuenta de Azure limpia para evitar sobrecostes nos esperados por parte de Microsoft.
Para eliminar todos los recursos del ejercicio, vamos a hacer lo siguiente:

1 - En el portal de Azure, abrimos sesión de **Bash** dentro del panel de Cloud Shell.
2 - Listamos todos los resource groups creados a lo largo del lab de este módulo, ejecutando el siguiente comando:
```bash
az group list --query "[?starts_with(name,'masterub-rg')].name" --output tsv
```
3 - A continuación eliminamos todos los resource groups creados en el lab, ejecutando el siguiente comando:
```bash
az group list --query "[?starts_with(name,'masterub-rg')].[name]" --output tsv | xargs -L1 bash -c 'az group delete --name $0 --no-wait --yes'
```
