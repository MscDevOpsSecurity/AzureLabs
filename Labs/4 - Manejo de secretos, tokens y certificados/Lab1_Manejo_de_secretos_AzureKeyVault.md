
| Lab |  1  |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Manejo de secretos, tokens y certificados | Uso de Azure KeyVault para el almacenamiento de secretos |

## Lab overview

## Objetivos
El objetivo de esta práctica es entender la responsabilidad que delegamos en Azure keyVault para almacenar datos sensibles, que nadie será capaz de ver o usar, a menos que nosotros lo diseñemos de esa manera.

## Duración
90 min aprox.

## Instrucciones

### Antes de comenzar
Para poder empezar con la práctica, vamos a necesitar:
- Azure KeyVault creado en un Azure ResourceGroup (RG).
- Base de datos CosmosDB creada en el mismo Azure RG.
- Un Azure App Service creado en el mismo Azure RG.
- Cliente Rest para su uso desde vuestro pc personal. Puede ser un cliente online, como por ejemplo [Advanced REST Client](https://chrome.google.com/webstore/detail/advanced-rest-client/hgmloofddffdnphfgcellkdfbfbjeloo/related) de Google Chrome.
- Visual Code instalado en vuestro pc.

Para la preparación de este lab, os vamos a proveer de todo lo necesario para que podáis levantar la infraestructura en Azure de forma automática, mediante la ejecución de ARM templates. Esto nos permitirá tener funcionando el Azure KeyVault, CosmosDB y el App Service.

Estos templates los podréis localizar en la carpeta de recursos del módulo 4, en la ruta "AzureLabs/Recursos/4 - Manejo de secretos, tokens y certificados/Lab1_AzureKeyVault_localhost/ARM_templates"

La estructura que vamos a crear, responde al siguiente diseño. Puede parecer muy complejo, pero es lo más sencillo que nos vamos a encontrar en el manejo de secretos dentro de Azure.

![Module4Lab1_general_view_localhost](images/Module4Lab1_general_view_localhost.png)

### Tarea 1 : Vamos a cargar los ARM templates en vuestra cuenta de Azure.
1 - Hacemos log in en nuestra cuenta personal de Azure (o en la cuenta de una persona del grupo, en caso de serlo).
2 - Nos dirigimos al [portal de Azure](https://portal.azure.com/#home).
3 - Creamos un ResourceGroup nuevo, dentro del cuál se irán creando todos los demás componentes. Esto facilitará al final de la práctica, la eliminación de todos los recursos de forma conjunta.
  - Desde el shell del portal de Azure, ejecutamos el siguiente comando.    
```sh
az deployment group create --name AzureLabsModulo4Lab1Deployment --resource-group AzureLabsModulo4Lab1 --template-uri "https://github.com/MscDevOpsSecurity/AzureLabs/blob/6239ab3be061c40e86c2da269feb69455c1cdeac/Recursos/4%20-%20Manejo%20de%20secretos,%20tokens%20y%20certificados/ARM%20templates/placeholder.md" --parameters storageAccountType=Standard_GRS
```
  > **Tip:** Si no tenemos el recurso creado para el shell de Azure, nos aparecerá una ventana como la siguiente, que nos pedirá que elijamos la subscripción de Azure donde poder montar el storage account para el shell. Si solo tenemos una subscripción, estará seleccionada por defecto, solo nos queda pinchar en _Create storage_.
  
  ![AzureShellWarning](images/AzureShellWarning.png)
  
  - Tardará un rato en completarse la tarea, pero lo que nos queda claro, es que cuando termine, este template que acabamos de ejecutar nos creará automáticamente los 3 recursos que necesitamos: Azure KeyVault, CosmosDb y el App Service (Web App) sin más intervención.
4 - Accedemos al Resource group que acabamos de crear, para asegurarnos de que todos los recursos previamente mencionados están ahí.

### Tarea 2: Vamos a preparar el código para ejecutar en nuestro visual studio code.

1 - Al clonar este repositorio, nos hemos descargado el código fuente necesario en la ruta "AzureLabs/Recursos/4 - Manejo de secretos, tokens y certificados/source/initial/". Solo tenemos que abrir la solución con VS Code/Visual Studio 2019 Community o el editor gráfico que queráis. 

2 - Compilamos el código para asegurarnos de que todo está correctamente preparado.

3 - Ahora vamos a fijarnos en el archivo _appsettings.json_, porque en él reside el principal problema que vamos a resolver con esta práctica. En él podéis ver, en la sección llamada **"CosmosDB"**, como la key (o contraseña) de la base de datos está en texto plano. Es cierto que está codificada en base64 y no es fácilmente legible, pero si alguien se hiciese con ella, solo tendría que copiarla y pegarla donde la necesite, para poder acceder a todos los recursos de nuestra base de datos en la nube.

![CosmosDB_VSConfiguration](images/CosmosDB_VSConfiguration.png)

4 - Antes de poder ejecutar la aplicación, vamos a necesitar la verdadera clave de nuestra base de datos Azure CosmosDB, porque la existente es la de una base de datos de ejemplo, y a nosotros nos interesa conectar con la nuestra. Para ello, nos vamos a ir al [portal de Azure](https://portal.azure.com/), y buscar nuestro resource group reciéntemente creado mediante los templates de ARM.

![AzCosmosDB_link](images/AzCosmosDB_link.png)

5 - Pinchamos en ese link, y una vez dentro de la base de datos, en la parte izquierda veremos un menú llamado **Settings**, y dentro del mismo, una opción **keys**. Si pinchamos en ella veremos todas las claves utilizadas por la propia base de datos (así como cadenas de conexión), de las cuales nos interesa la _PRIMARY KEY_. La copiamos y la pegamos en nuestro archivo de configuración de la aplicación de C# _appSettings.json_.

![AzCosmosDB_PK](images/AzCosmosDB_PK.png)

6 - Ahora sí, vamos a ejecutar nuestra aplicación, la cual será un servicio web que se quedará esperando por peticiones REST (GET, PUT, POST, DELETE..).

7 - Una vez esté la aplicación corriendo, nos dirigimos al cliente Rest que tengamos en nuestro laptop y vamos a ejecutar la siguiente request:
  - Lo primero que vamos a hacer, para facilitar la ejecución del lab, es quitar la autenticación que nos exije nuestro cliente REST (no es buena práctica hacer esto, y por ello vamos a volver a restaurarlo al finalizar la misma). Seguro que el cliente tiene algún tipo de configuración o settings, que os permiten deshabilitar la validación SSL, hacedlo.

![Remove_SSL_validation_postman](images/Remove_SSL_validation_postman.png)

  - Operación: **GET**
  - Barra de direcciones: _http://localhost:puerto/api/Items_, donde \<puerto\> será un valor numérico especificado en vuestro archivo _launchSettings.json_ dentro del código.
  - Pinchamos en "Send/Enviar"

![postman_get_items_localhost](images/postman_get_items_localhost.png)

  - La respuesta tiene que ser casi inmediata, ya que estamos trabajando en localhost. En la sección **Response** del cliente REST, debéis tener algo como lo que se muestra a continuación, que no es otra cosa que el contenido extraído de la base de datos CosmosDB en Azure. Podéis probar a introducir nuevos valores desde el propio portal, volver a ejecutar este comando y ver los resultados, para aseguraros que realmente se extraen de ahí y no hay truco.

8 - Como hemos visto, la aplicación funciona, pero con la contraseña de la base de datos totalmente expuesta al público que use la aplicación web. Para solucionar este problema, nace nuestro primer resource dentro de Azure, **Azure KeyVault**.

### Tarea 4: Crear un App Registration en Azure Active Directory (AD)

### Tarea 5: Crear certificados de seguridad e instalarlos donde se requieran

### Tarea 6: Conectar nuestra aplicación web con Azure KeyVault mediante código.

1 - Desde el propio [portal de Azure](https://portal.azure.com/), nos dirigirnos al resource group que hemos creado al principio, para poder ver las propiedades del KeyVault _Modulo4Lab1-akv_.

![AzKeyVault_Menu](images/AzKeyVault_Menu.png)

2 - Una vez dentro, veremos un menú con opciones a la izquierda. Pinchamos en la opción **Secrets**. Una vez dentro, veremos un botón en la parte superior llamado **+ Generate/Import**, y aquí le vamos a dar los siguientes valores, (dejando el resto de campos con su valor por defecto):

  - Name: Module4Lab1-CosmosDb--Key
  - Value: pegamos la contraseña que hemos copiado desde nuestro Azure CosmosDB (y pegado en la tarea anterior en el código).

  > ℹ️ Nota: es importante mantener la estructura en el nombre, porque así nos será mucho más fácil recuperarla desde C#. Si nos fijamos detenidamente, los valores corresponden   al nombre de la aplicación de C#, luego viene el nombre de la sección dentro del archivo de configuración, y finalmente el nombre de la propiedad a recuperar.

3 - Para que tenga sentido todo lo que estamos haciendo, es necesario también, que eliminemos la contraseña del código C#. En su lugar podemos poner algo así _<Set-by-Keyvault>_ como se muestra a continuación.
  
![AzKeyVault_RemoveKeyFromCode](images/AzKeyVault_RemoveKeyFromCode.png)

4 - Ahora llega el momento de introducir nuevo código para que nuestra aplicación se conecte automáticamente con Azure al arrancar. Por eso nos vamos al código en el editor, y abrimos el archivo _Program.cs_. Ahora pegamos el siguiente código donde corresponde:

```csharp
    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    var root = builder.Build();
                    var vaultName = root["KeyVault:Vault"];
                    builder.AddAzureKeyVault($"https://{vaultName}.vault.azure.net/", 
                        root["KeyVault:ClientId"],
                        GetCertificate(root["KeyVault:Thumbprint"]), new PrefixKeyVaultExample("Module4Lab1"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
```
 
 5 - Como se puede observar, hay una nueva configuración creada, aparte de la que teníamos por defecto en la aplicación REST. Esto es lo que nos permitirá conectarnos automáticamente con nuestro Azure KeyVault, y ¿cómo lo hace?, pues vamos a verlo.
 
   5.1 - Lo primero es crear una nueva [AppConfiguration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder.configureappconfiguration?view=dotnet-plat-ext-5.0), con el que construiremos la configuración de Az KeyVault.
    
   5.2 - Le indicamos cuál es el nombre del secreto:

```csharp
var vaultName = root["KeyVault:Vault"];
```

   5.3 - La siguiente línea conecta directamente con nuestro Az KeyVault, haciendo uso de varias cosas:
      - Url en Azure del KeyVault: la obtenemos desde el portal de Azure, dentro de nuestro keyvault resource.

![AzKeyVault_Uri.png](images/AzKeyVault_Uri.png)

      - El ClientId: este es el ClientId que hemos copiado al hacer el registro de la WebApp en Azure AD.
      - El certificado self-signed instalado en nuestra computadora, creado en la tarea 4.
      - Nuestro propio IKeyVaultSecretManager, en nuestro caso, _PrefixKeyVaultExample_.

> ℹ️ Para más información acerca del _Secret Manager_ podéis acceder a este [enlace](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-5.0#use-a-key-name-prefix).

6 - Ahora ya tenemos todo lo necesario para poder conectarnos desde nuestra aplicación a Azure CosmosDB, haciendo uso del secreto almacenado en nuestra cuenta de Azure KeyVault, de forma automática y segura. Nadie que tenga acceso a nuestro código fuente en C# será capaz de saber la contraseña de la base de datos.

Algunos pensaréis: la contraseña no está en texto plano en la configuración, pero ahora tenemos la URL + ClientId + thumbprint, ¿podríamos acceder nosotros también usando esos mismos datos, si encontrásemos la manera de acceder al código? La respuesta es no, porque necesitas tener el certificado instalado en tu propia máquina, y eso no lo tiene cualquiera, solo nosotros.

![CertificadoNoEncontrado.png](images/CertificadoNoEncontrado.png)


### Tarea 6: Comprobemos de nuevo la aplicación

### Tarea 7: Eliminar todos los recursos creados en Azure.

Al final de cada ejercicio es importante dejar nuestra cuenta de Azure limpia para evitar sobrecostes nos esperados por parte de Microsoft.
Para eliminar todos los recursos del ejercicio, vamos a hacer lo siguiente:

1 - En el portal de Azure, abrimos sesión de **Bash** dentro del panel de Cloud Shell.
2 - Listamos todos los resource groups creados a lo largo del lab de este módulo, ejecutando el siguiente comando:
```bash
az group list --query "[?starts_with(name,'AzureLabsModulo4Lab1')].name" --output tsv
```
3 - A continuación eliminamos todos los resource groups creados en el lab, ejecutando el siguiente comando:
```bash
az group list --query "[?starts_with(name,'AzureLabsModulo4Lab1')].[name]" --output tsv | xargs -L1 bash -c 'az group delete --name $0 --no-wait --yes'
```
