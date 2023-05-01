
| Lab |  1  |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Manejo de secretos, tokens y certificados | Uso de Azure KeyVault para el almacenamiento de secretos |

## Lab overview

## Objetivos
El objetivo de esta práctica es entender la responsabilidad que delegamos en Azure keyVault para almacenar datos sensibles, que nadie será capaz de ver o usar, a menos que nosotros lo diseñemos de esa manera.

## Duración
60 min aprox.

## Instrucciones

### Antes de comenzar
Para poder empezar con la práctica, vamos a necesitar:
- Azure KeyVault creado en un Azure ResourceGroup (RG).
- Base de datos CosmosDB creada en el mismo Azure RG.
- Un Azure App Service creado en el mismo Azure RG.
- Cliente Rest para su uso desde vuestro pc personal. Puede ser un cliente online, como por ejemplo [Advanced REST Client](https://chrome.google.com/webstore/detail/advanced-rest-client/hgmloofddffdnphfgcellkdfbfbjeloo/related) de Google Chrome.
- [Visual Studio Code](https://code.visualstudio.com/download) instalado en vuestro pc.

Para la preparación de este lab, os vamos a proveer de todo lo necesario para que podáis levantar la infraestructura en Azure de forma automática, mediante la ejecución de ARM templates. Esto nos permitirá tener funcionando el Azure KeyVault, CosmosDB y el App Service.

La estructura que vamos a crear, responde al siguiente diseño. Puede parecer muy complejo, pero es lo más sencillo que nos vamos a encontrar en el manejo de secretos dentro de Azure.

![Module4Lab1_general_view_cloud](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Module4Lab1_general_view_cloud.png)

### Tarea 1 : Creación de los recursos en Azure.
Hacemos log in en nuestra cuenta personal de Azure (o en la cuenta de una persona del grupo, en caso de serlo) y nos dirigimos al [portal de Azure](https://portal.azure.com/#home).

#### Creación del Resource Group

Vamos a crear un Resource Group nuevo, dentro del cuál se irán creando todos los demás componentes. Esto facilitará al final de la práctica, la eliminación de todos los recursos de forma conjunta.

  - En el portal de Azure nos dirigimos a la [pagina de creacion de resource group](https://portal.azure.com/#create/Microsoft.ResourceGroup) para crear un nuevo *Resource Group*
  - Junto a la selecion de la Subscription y de la region, definiremos el nombre ***AzureLabsModulo4Lab2*** por el *resource group*
  - Después de haber revisado la configuración, podremos crear el *resource group*

#### Creación del Azure Key Vault

Vamos ahora a crear un key vault dentro del resource group que acabamos de definir.

  - En el portal de Azure nos dirigimos a la [pagina de creación de Azure Key Vault](https://portal.azure.com/#create/Microsoft.KeyVault)
  - Junto a la selección de la Subscription, de la Región y del Pricing Tier, definiremos el nombre **Modulo4Lab2-key-vault** por el key vault

  > **Tip:** Los Azure Key Vaults son unívocos globalmente. Además, Azure habilita de default soft deletion para ellos. Como consecuencia podría ocurrir que el nombre escogido para el Keyvault sea ya en utilizo. En este caso la UI mostrará el error *"The name 'Modulo4Lab2-key-vault' is already in use, or is still being reserved by a vault which was previously soft deleted. Please use a different name."*. Puedes solucionar el error utilizando un nombre de key vault no en uso o haciendo un *purge* del key vault en el apartado **Managed Deleted Vaults**.

  -  Después de haber revisado la configuración, podremos crear el Azure Key Vault.

#### Creación del Azure Cosmos DB

Como último paso de esta tarea crearemos el Azure Cosmos DB cuyos credenciales de acceso protegeremos en el Azure Key Vault, el objetivo de esta práctica.

  - En el portal de Azure nos dirigimos a la [pagina de creación de Azure Cosmos DB](https://portal.azure.com/#create/Microsoft.DocumentDB)
  - Seleccionamos la creación de un **Azure Cosmos DB for NoSQL**
  - Junto a la selección de la Suscription y de la Región, definiremos el nombre de account  ***modulo4lab2-cosmosdb***
  > **Tip:** También las instancias de Cosmos Db son unívocas globalmente, al igual que los Azure Key Vaults. Asegurarse entonces de utilizar un nombre que no sea ya en uso.

- Después de haber revisado la configuración, podremos crear el Azure Cosmos DB.

Finalmente accedemos al *resource group* ***AzureLabsModulo4Lab2*** para asegurarnos de que el Azure Key Vault y el Azure Cosmos DB están ahí.

#### Creación del App Service

Para simplificar la creación de la web app vamos a cargar un ARM template ya preparado en nuestra cuenta de Azure.

  - Primero necesitamos el template, que podremos encontrar en la ruta 📁 Recursos/2 - Seguridad en Cloud/lab4/2_AzureKeyVault_Cloud/ARM_templates/template.json
  - Desde el shell del portal de Azure, lo cargamo mediante el botón _Upload/Download files_ (uno cada vez).
  - Ahora tendremos el archivo cargado en nuestra raíz del shell de bash, con lo que podemos ejecutar el comando para desplegar los recursos:

```sh
az deployment group create --resource-group AzureLabsModulo4Lab2 --template-file template.json
```

  > **Tip:** Si no tenemos el recurso creado para el shell de Azure, nos aparecerá una ventana como la siguiente, que nos pedirá que elijamos la subscripción de Azure donde poder montar el storage account para el shell. Si solo tenemos una subscripción, estará seleccionada por defecto, solo nos queda pinchar en _Create storage_.

  > **Tip:** Los Azure websites son unívocos globalmente. En el caso que el nombre modulo4lab2 es ya en uso adaptar el template con un nuevo nombre.

  ![AzureShellWarning](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzureShellWarning.png)

  - Tardará un rato en completarse la tarea, pero lo que nos queda claro, es que cuando termine, este template que acabamos de ejecutar nos creará automáticamente el recurso que necesitamos: el App Service (Web App) sin más intervención.
  > **Warning:** El App Service se ha creado con el *Pricing Plan* y ***Basic B1***. Hay disponible también un plan gratis, el ***Free F1***, para ahorrar créditos de Azure, pero el plan free no permite la gestión de certificados. Por esta practica es necesario por lo meno el plan Basic B1 a partir de la Tarea 3. 

Accedemos al Resource group que acabamos de crear, para asegurarnos de que todos los recursos previamente mencionados están ahí.

### Tarea 2: Vamos a preparar el entorno.

Para esto, vamos a seguir los mismos pasos del Lab1 desde la tarea 4 hasta la tarea 7 adaptando los nombres de los recursos.

### Tarea 3: Configurar certificado para el app service.

Vamos a necesitar que el app service actue como un cliente, es decir, que nada más arrancar, instale el certificado privado que generamos previamente. De esta manera, tendrá permisos para leer del Azure Key Vault.

1 - Vamos al portal de Azure y entramos en el App Service que tenemos creado.

2 - Bajo el menú de **settings**, nos vamos a _Certificates_.

3 - Ahora pinchamos en la pestaña **Bring your own certificates (.pfx)** y subimos nuestro certificado _pfx_ que hemos generado antes. Por supuesto nos pedirá la contraseña que le dimos cuando lo creamos. Esta será la única vez que tengamos que usar las contraseñas.

4 - Ahora necesitamos que el App Service instale este certificado nada más arrancar. Para ello, en el propio menú **Settings**, nos vamos a _Configuration_.

5 - Añadimos una nueva **Application setting**, que no es otra cosa que una variable de entorno.

| Campo | Valor |
|---|---|
|Name|WEBSITE_LOAD_CERTIFICATES|
|Value| * |

6 - Hacemos clic en OK => Save => Continue.


### Tarea 4: Comprobemos de nuevo la aplicación

Para poder ejecutar la aplicación, vamos a necesitar primero hacer un publish desde nuestro entorno de desarrollo al App Service que hemos creado en Azure.

1 - Teniendo el código en Visual Studio Code listo para usarse, nos conectarnos con Azure con la extension ***Azure Account*** que sera instalada previamente en VS Code. Nos vamos ahora al menú **View/Command Palette** y se nos desplegará una barra superior para escribir comandos.

![VSCode_CommandPalette](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_CommandPalette.png)

2 - Escribimos:

```bash
Sign in to Azure Cloud
```

3 - De aquí saltaremos a la web de Azure para hacer log in, seleccionamos nuestra cuenta y metemos las credenciales que nos pida. A partir de aquí, ya estaremos logueados en Azure dentro de VSCode.

![VSCode_CommandPalette_login](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_CommandPalette_login.png)

![VSCode_CommandPalette_logged](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_CommandPalette_logged.png)

4 - Cerramos la pestaña del navegador y nos volvemos al VSCode.

5 - Abrimos la ventana del terminal y ejecutamos el comando siguiente para compilar el proyecto (si ya estaba compilado no hace falta).

```powershell
dotnet publish --configuration Release
```

6 - Gracias a la extension ***Azure App Service*** de VS Code, hacemos clic con botón derecho sobre la carpeta donde se han publicado los binarios (generalmente \bin\Release\net7.0\publish) y seleccionamos **Deploy to Web App..**.

![VSCode_DeployMenu](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_DeployMenu.png)

7 - Seleccionamos el App Service de la lista(*). 

![VSCode_Deploy_Choose](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_Deploy_Choose.png)

8 - VS Code te preguntará si quieres sobreescribir el contenido existente de la Web App. Le damos a **Deploy** para indicar una respuesta afirmativa.

![VSCode_Deploy_Warning](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_Deploy_Warning.png)
![VSCode_Deploy_Deploying](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_Deploy_Deploying.png)

9 - Al final del proceso veremos una opción como esta, donde podremos lanzar la web directamente.

![VSCode_Deploy_Browse](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/VSCode_Deploy_Browse.png)

Finalmente, cuando la web se lance, el certificado se instalará automáticamente desde Azure en la máquina que despliega, con lo que no tendremos que preocuparnos de los permisos de Azure KeyVault, ya estarán concedidos.

  > ℹ️ (*) Si no somos capaces de ver nuestro AppService en la lista desplegable al hacer el _Deploy to Web App.._ del paso 6, tendremos que reiniciar el terminal de VSCode y ya aparecerá. Suele pasar cuando hacemos log-in con nuestra cuenta de Azure, no encuentra todos los recursos de forma inmediata.

### Tarea 5: Eliminar todos los recursos creados.

Al final de cada ejercicio es importante dejar nuestra cuenta de Azure limpia para evitar sobrecostes nos esperados por parte de Microsoft.
Para eliminar todos los recursos del ejercicio, vamos a hacer lo siguiente:

1 - En el portal de Azure, abrimos sesión de **Bash** dentro del panel de Cloud Shell.

2 - Eliminamos el resource group creado en el lab, ejecutando el siguiente comando:

```bash
az group delete --name <your_resource_group_name> --no-wait --yes
```

3 - En este caso no es necesario eliminar ningún certificado porque al estar instalado dentro de la máquina que ejecuta el App Service, se eliminará automáticamente con el ResourceGroup.
