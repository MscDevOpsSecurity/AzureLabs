
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

1 - Desplegamos la primera VM e [Instalamos net Core 5.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-5.0.15-windows-x64-installer) (_virtual_machine_win10_template_)

2 - Desde el portal de Azure, dentro de la ventana de la VM, habilitamos "System assigned" Identity.

3 - Desplegamos el Azure KeyVault (_azure_keyvault_template_)

4 - Desplegamos un servidor SQL y una base de datos de ejemplo. Después, desde la ventana de la base de datos, en **Propiedades**, guardamos los valores del _connectionString_ 
  
> Server=tcp:masterub-sqlserver.database.windows.net,1433;Initial Catalog=masterub-db;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

5 - Vamos a la ventana del Azure KeyVault y creamos los secretos correspondientes a la connectionString que acabamos de guardar (usuario, contraseña, ...)

6 - Abrimos la VM y copiamos la aplicación .NET en el escritorio (_AccessAzKeyVault_OneDatabase.zip_).

7 - Ejecuta la opción del menú que nos permite leer los secretos del Az KeyVault, (sin haber configurado las **Access policies**) y comprueba que te da un error de permisos.

![lab5_console_onedatabase](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab5_console_onedatabase.png)

8 - Define las **Access policies** correspondientes para que la VM pueda leer del Az KeyVault (Get).

9 - Ejecuta de nuevo y comprueba que realmente puedes leer los secretos.


### Tarea 2 : Jugaremos con 2 bases de datos y 2 VMs, para reproducir el problema de las "Vault Access Policy".

Vamos a intentar replicar la siguiente infraestructura, que contiene un problema de seguridad evidente.

![2_db_2_vms_issue](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab5_2_db_2_vms_issue.png)

> Dejamos las **Access Policies** de la primera VM como están.

1 - Desplegamos la segunda VM e [Instalamos net Core 5.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-5.0.15-windows-x64-installer) (_virtual_machine_win10_template_2_)
	
2	- Desde el portal de Azure, dentro de la ventana de la VM, habilitamos "System assigned" Identity.

3 - Configuramos las **Access policies** para que la segunda VM pueda acceder al Azure KeyVault.

4 - Creamos la segunda base de datos de ejemplo dentro del mismo servidor SQL. Después, desde la ventana de la base de datos, en **Propiedades**, guardamos los valores del _connectionString_ 
  
> Server=tcp:masterub-sqlserver.database.windows.net,1433;Initial Catalog=masterub-db;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

5 - Vamos a la ventana del Azure KeyVault y creamos los secretos correspondientes a la connectionString que acabamos de guardar (usuario, contraseña, ...)

6 - Accedemos a la primera VM de nuevo, y copiamos la aplicación de .NET en el escritorio (_AccessAzKeyVault_TwoDatabases.zip_).

7 - Intentamos leer los secretos relativos a la primera base de datos (aquí no hay problema de seguridad).

8 - Intentamos leer los secretos relativos a la segunda base de datos, y comprobamos que funciona y tenemos permisos (aquí si está el problema de seguridad).

![lab5_console_twodatabases](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab5_console_twodatabases.png)


### Tarea 3 : Jugaremos con 2 bases de datos y 2 VMs, arreglando el problema con RBAC.

Vamos a intentar replicar la siguiente infraestructura, que soluciona el problema de seguridad visto en el punto anterior.

![2_db_2_vms_rbac](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab5_2_db_2_vms_rbac.png)

1 - Primero necesitamos cambiar el **Permission model** de _Vault Access Policy_ a _Azure Role Base Access Control_.

> **NOTA**: recuerda que para cambiar este modo, el grupo de recursos donde se encuentre tu Azure KeyVault debe tener permisos de **Azure KeyVault Administrator** o **Owner** para el usuario con el que accedes al portal.

2 - Ahora, la parte que va a eliminar el riesgo de que otra VM acceda a nuestros secretos, es la posibilidad de asignar a cada uno de ellos, los permisos de la VM que corresponde. Podemos ir uno a uno por los secretos, o ejecutar los siguientes comandos en el **Cloud Shell**.

```bash
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM1>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/usernamedb0"
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM1>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/passworddb0"
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM1>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/sourcenamedb0"
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM1>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/initialcatalogdb0"
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM2>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/usernamedb1"
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM2>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/passworddb1"
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM2>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/sourcenamedb1"
az role assignment create --role "Key Vault Secrets User" --assignee-object-id "<principal object id from VM2>" --scope "/subscriptions/<suscriptionId>/resourceGroups/<ResourceGroupName>/providers/Microsoft.KeyVault/vaults/<keyvault_name>/secrets/initialcatalogdb1"
```
> **NOTA**: cambia las variables correspondientes por tus propios valores.

3 - Abre de nuevo la primera VM y ejecuta la segunda aplicación que hemos copiado anteriormente.

4 - Ejecuta la primera opción, que nos permite leer nuestros propios secretos. Comprueba que se leen sin problema.

5 - Ejecuta la segunda opción, que nos permite leer los secretos asociados a la otra base de datos y a la VM2. Comprueba que te lanza un error **Forbidden**.


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
