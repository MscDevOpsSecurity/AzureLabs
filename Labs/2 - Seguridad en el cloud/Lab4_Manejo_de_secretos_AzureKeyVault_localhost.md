
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
- Cliente Rest para su uso desde vuestro pc personal. Puede ser un cliente online, como por ejemplo [Advanced REST Client](https://chrome.google.com/webstore/detail/advanced-rest-client/hgmloofddffdnphfgcellkdfbfbjeloo/related) de Google Chrome o [PostMan](https://www.postman.com/).
- [Visual Studio Code](https://code.visualstudio.com/download) instalado en vuestro pc.

La estructura que vamos a crear, responde al siguiente diseño. Puede parecer muy complejo, pero es lo más sencillo que nos vamos a encontrar en el manejo de secretos dentro de Azure.

![Module4Lab1_general_view_localhost](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Module4Lab1_general_view_localhost.png)

### Tarea 1 : Creación de los recursos en Azure.
Hacemos log in en nuestra cuenta personal de Azure (o en la cuenta de una persona del grupo, en caso de serlo) y nos dirigimos al [portal de Azure](https://portal.azure.com/#home).

#### Creación del Resource Group

Vamos a crear un Resource Group nuevo, dentro del cuál se irán creando todos los demás componentes. Esto facilitará al final de la práctica, la eliminación de todos los recursos de forma conjunta.

  - En el portal de Azure nos dirigimos a la [pagina de creacion de resource group](https://portal.azure.com/#create/Microsoft.ResourceGroup)

  ![Module4Lab1_Resource_group_creation](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Module4Lab1_Resource_group_creation.png)

  - Junto a la selecion de la Subscription y de la region, definiremos el nombre ***AzureLabsModulo4Lab1*** por el resource group
  -  Después de haber revisado la configuración, podremos crear el resource group

#### Creación del Azure Key Vault

Vamos ahora a crear un key vault dentro del resource group que acabamos de definir.

  - En el portal de Azure nos dirigimos a la [pagina de creación de Azure Key Vault](https://portal.azure.com/#create/Microsoft.KeyVault)

  ![Module4Lab1_key_vault_creation](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Module4Lab1_key_vault_creation.png)

  - Junto a la selección de la Subscription, de la Región y del Pricing Tier, definiremos el nombre **Modulo4Lab1-key-vault** por el key vault

  > **Tip:** Los Azure Key Vaults son unívocos globalmente. Además, Azure habilita de default soft deletion para ellos. Como consecuencia podría ocurrir que el nombre escogido para el Keyvault sea ya en utilizo. En este caso la UI mostrará el error *"The name 'Modulo4Lab1-key-vault' is already in use, or is still being reserved by a vault which was previously soft deleted. Please use a different name."*. Puedes solucionar el error utilizando un nombre de key vault no en uso o haciendo un *purge* del key vault en el apartado **Managed Deleted Vaults**.

  -  Después de haber revisado la configuración, podremos crear el Azure Key Vault.

#### Creación del Azure Cosmos DB

Como último paso de esta tarea crearemos el Azure Cosmos DB cuyos credenciales de acceso protegeremos en el Azure Key Vault, el objetivo de esta práctica.

  - En el portal de Azure nos dirigimos a la [pagina de creación de Azure Cosmos DB](https://portal.azure.com/#create/Microsoft.DocumentDB)
  - Seleccionamos la creación de un **Azure Cosmos DB for NoSQL**
  - Junto a la selección de la Suscription y de la Región, definiremos el nombre de account  ***modulo4lab1-cosmosdb*** ![Module4Lab1_cosmos_db_creation](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Module4Lab1_cosmos_db_creation.png)

  > **Tip:** También las instancias de Cosmos Db son unívocas globalmente, al igual que los Azure Key Vaults. Asegurarse entonces de utilizar un nombre que no sea ya en uso.

- Después de haber revisado la configuración, podremos crear el Azure Cosmos DB.

Finalmente accedemos al resource group *AzureLabsModulo4Lab1* para asegurarnos de que el Azure Key Vault y el Azure Cosmos DB están ahí.

### Tarea 2: Acceder al Cosmos DB desde un entorno local.

A lo largo de esta tarea conseguiremos leer y actualizar la instancia del Cosmos DB desde nuestra máquina de desarrollo. En esta tarea todavía no utilizaremos una solución óptima desde el punto de vista de seguridad. Pero nos permitirá averiguar el funcionamiento de la conexión y entender los puntos débiles.

1 - Al clonar este repositorio, nos hemos descargado el código fuente necesario en la ruta "AzureLabs/Recursos/2 - Seguridad en Cloud/lab4/1_AzureKeyVault_localhost/source/initial/". Solo tenemos que abrir la solución con VS Code/Visual Studio 2019 Community o el editor gráfico que queráis. 

2 - Compilamos el código para asegurarnos de que todo está correctamente preparado.

3 - Ahora vamos a fijarnos en el archivo _appsettings.json_, porque en él reside el principal problema que vamos a resolver con esta práctica. En él podéis ver, en la sección llamada **"CosmosDB"**, como la key (o contraseña) de la base de datos está en texto plano. Es cierto que está codificada en base64 y no es fácilmente legible, pero si alguien se hiciese con ella, solo tendría que copiarla y pegarla donde la necesite, para poder acceder a todos los recursos de nuestra base de datos en la nube.

![CosmosDB_VSConfiguration](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/CosmosDB_VSConfiguration.png)

4 - Antes de poder ejecutar la aplicación, vamos a necesitar la verdadera clave de nuestra base de datos Azure CosmosDB, porque la existente es la de una base de datos de ejemplo, y a nosotros nos interesa conectar con la nuestra. Para ello, nos vamos a ir al [portal de Azure](https://portal.azure.com/), y buscar nuestro resource group reciéntemente creado mediante los templates de ARM.

![AzCosmosDB_link](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzCosmosDB_link.png)

5 - Pinchamos en ese link, y una vez dentro de la base de datos, en la parte izquierda veremos un menú llamado **Settings**, y dentro del mismo, una opción **keys**. Si pinchamos en ella veremos todas las claves utilizadas por la propia base de datos (así como cadenas de conexión), de las cuales nos interesa la _PRIMARY KEY_. La copiamos y la pegamos en nuestro archivo de configuración de la aplicación de C# _appSettings.json_.

![AzCosmosDB_PK](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzCosmosDB_PK.png)

6 - Otra cosa que deberemos cambiar es el valor de la propiedad **Account**, porque es la url que nos ha proporcionado nuestro CosmosDB, que en cada caso puede ser diferente. Simplemente comprueba que coincide con la tuya, de lo contrario reemplazala.

7 - Ahora sí, vamos a ejecutar nuestra aplicación, la cual será un servicio web que se quedará esperando por peticiones REST (GET, PUT, POST, DELETE..).

8 - Una vez esté la aplicación corriendo, nos dirigimos al cliente Rest que tengamos en nuestro laptop y vamos a ejecutar la siguiente request:
  - Lo primero que vamos a hacer, para facilitar la ejecución del lab, es quitar la autenticación que nos exije nuestro cliente REST (no es buena práctica hacer esto, y por ello vamos a volver a restaurarlo al finalizar la misma). Seguro que el cliente tiene algún tipo de configuración o settings, que os permiten deshabilitar la validación SSL, hacedlo.

![Remove_SSL_validation_postman](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Remove_SSL_validation_postman.png)

  - Operación: **GET**
  - Barra de direcciones: _http://localhost:puerto/api/Items_, donde \<puerto\> será un valor numérico especificado en vuestro archivo _launchSettings.json_ dentro del código, en el profile que no es **IIS** (normalmente será 5001).
  - Pinchamos en "Send/Enviar"

![postman_get_items_localhost](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/postman_get_items_localhost.png)

  - La respuesta tiene que ser casi inmediata, ya que estamos trabajando en localhost. En la sección **Response** del cliente REST, debéis tener algo como lo que se muestra a continuación, que no es otra cosa que el contenido extraído de la base de datos CosmosDB en Azure. Podéis probar a introducir nuevos valores desde el propio portal, volver a ejecutar este comando y ver los resultados, para aseguraros que realmente se extraen de ahí y no hay truco.

![postman_get_items_localhost_result](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/postman_get_items_localhost_result.png)

9 - Como hemos visto, la aplicación funciona, pero con la contraseña de la base de datos totalmente expuesta al público que use la aplicación web. Para solucionar este problema, nace nuestro primer resource dentro de Azure, **Azure KeyVault**.

  > ℹ️ Si por alguna razón obtienes un error al inicial la aplicación relativa a IIS, puede que sea porque tienes el puerto de inicio ocupado por otro recurso del sistema. Para buscar un puerto vacío, prueba la siguiente llamada desde un terminal:
```bash
netsh interface ipv4 show excludedportrange protocol=tcp
```

### Tarea 3: Crear certificados de seguridad en la máquina donde se ejecutará al aplicación

Necesitamos crear un certificado digital (self-signed en este caso), que más adelante utilizaremos para conectar Azure KeyVault y nuestra aplicación web.

1 - Abrimos una consola de WSL ([Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10)).

2 - Ejecutamos el siguiente comando:

```Bash
openssl req -newkey rsa:2048 -nodes -keyout privateKey.pem -x509 -days 365 -out publicKey.pem
```
3 - Ahora nos preguntará una serie de cosas, a las cuales le daremos _Enter_ para dejar los valores por defecto, excepto para **Common Name** al cual le daremos: _MasterDevSecOps_.

4 - Ahora con estas claves, vamos a generar el archivo \*.pfx, el cual es definitivamente el certificado que necesitamos. Para ellos ejecutamos en la misma consola el siguiente comando:

```Bash
openssl pkcs12 -inkey privateKey.pem -in publicKey.pem -export -out certificate.pfx
```

5 - A continuación nos pedirá una contraseña, y mucho ojo con lo que ponemos, porque no quedará registro de ella en ningún otro sitio, es decir, necesitamos guardarla o recordarla para más adelante.

![CertificatesAndKeys](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/CertificatesAndKeys.png)

6 - Comprueba que se han generado estos 3 elementos en la carpeta donde ejecutaste los comandos, y con los mismos nombres que le proporcionaste.

### Tarea 4: Crear un App Registration en Azure Active Directory (AD)

Esta tarea tratará de configurar un nuevo registro de aplicación dentro de Azure, es decir, vamos a crear una entidad dentro de Azure Active Directory, a la cual concederemos permisos de lectura sobre los secretos de Azure KeyVault, asumiendo que esta entidad posee el certificado creado en la tarea anterior.

#### Tarea 4a: Crear un App Registration en Azure Active Directory (AD) con Azure UI

1 - Desde el portal de Azure, nos vamos al buscador de arriba y buscamos **Azure Active Directory**. Al pinchar sobre el link, nos llevará al propio AD.

  > **Tip:** El administrador de la suscripción de Azure podría haber restringido el acceso a la UI de Azure Active Directory a solo los usuarios administradores. Por ejemplo, la suscrición por estudiantes suelen ser configurada con esta tipo de restricción. En este caso, es posible registrar  la App utilizando el Cloud Shell como comentado en la Tarea 4b.

2 - En el menú de la izquierda, pinchamos sobre **App registrations**.

![Azure_AD_AppRegistration](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Azure_AD_AppRegistration.png)

3 - En el menú superior, pinchamos en **+ New Registration** y nos abrirá una nueva ventana, donde pondremos los siguiente valores, dejando el resto por defecto:
  - Name: cualquier nombre que queramos, y del cual nos acordemos después.

4 - Pinchamos en _Register_ y listo, ya tenemos nuestra entidad registrada.

5 - Antes de salir, vamos a tratar de guardar en algún sitio el _ClientId_, que nos va a hacer falta más adelante.

![Azure_AD_AppRegistration_ClientID](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Azure_AD_AppRegistration_ClientID.png)

6 - Ya tenemos creada la entidad dentro de Azure AD, pero ahora necesitamos asignarle el certificado digital del que hablamos al inicio de la tarea. Por ello, sin salirnos de la ventana de la _App Registration_ a la que hemos sido redirigidos al registrarla, en el menú izquierdo, seleccionamos la opción **Certificates & Secrets**.

7 - Ahora veremos un botón llamado _Upload certificate_ que vamos a clicar. Nos pedirá la clave pública de un certificado que podremos indicar desde nuestro equipo. Vamos a seleccionar la clave denominada _publicKey.pem_ que generamos en la tarea anterior y después **Add**. No necesitamos la contraseña aquí, porque esta es la clave que cualquier aplicación utilizará para comunicarse con nosotros (de ahí lo de pública).

![Certificate_Thumbprint](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Certificate_Thumbprint.png)

8 - Como podemos observar en la imagen anterior, al proporcionar la clave pública, se nos muestra el _thumbprint_ del propio certificado, que vamos a necesitar guardar para incrustarlo en nuestro código de C#. Este thumbprint nos servirá para obtener el certificado instalado en nuestro equipo local que contenga ese mismo thumbprint.

#### Tarea 4b: Crear un App Registration en Azure Active Directory (AD) con Azure Cloud Shell

Esta tarea no es necesaria si la App ya está registrada por UI como explicado en la Tarea 4a.

  > **Tip:** Si no tenemos el recurso creado para el shell de Azure, nos aparecerá una ventana como la siguiente, que nos pedirá que elijamos la subscripción de Azure donde poder montar el storage account para el shell. Si solo tenemos una subscripción, estará seleccionada por defecto, solo nos queda pinchar en _Create storage_.

  ![AzureShellWarning](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzureShellWarning.png)

1 - Registrar una nueva app in Azure AD.

`az ad app create --display-name Module4Lab1App`

2 - Para obtener el appId de la app registrada, su puede utilizar el comando de list 
`az ad app list --display-name Module4Lab1App`

alternativamente 

`az ad app list --show-mine`

3 - Ahora que tenemos una applicacion de AD, podemos crear un Service Principal associado a ella

`az ad sp create-for-rbac --name Module4Lab1App \
                         --role Reader \
                         --scopes /subscriptions/f1393387-5b44-4758-971a-257690e1f5d5/resourceGroups/AzureLabsModulo4Lab1 \
                         --cert "@~/publicKey.pem"`

Asegurando de haber informado y sustituido correctamente el nombre de la aplicación `Module4Lab1App`, el ID de la subscripcion `f1393387-5b44-4758-971a-257690e1f5d5`, el nombre del Resource Group `AzureLabsModulo4Lab1` y de haber cargado en la shell la clave publica del certificado `publicKey.pem` 

4 - Para configurar un nuevo certificado por CLI, se puede subir la public key con la opcion de Upload/download de cloud bash y utilizar el comando
`az ad app credential reset --id edcd0a4a-734e-46d4-bbf9-c5a2eb5542ea --cert "@~/publicKey.pem" --append`

el id de app se puede obtener como definido en los puntos anteriores.

5 - Para averiguar la lista de certificados configurados
`az ad app credential list --id edcd0a4a-734e-46d4-bbf9-c5a2eb5542ea --cert`

el thumbprint del certificado es mostrado en la sección customKeyIdentifier  

6 - Para averiguar la lista de Service Principal

`az ad sp list --show-mine`

7 - Para borrar la registración de la app 
`az ad app delete --id edcd0a4a-734e-46d4-bbf9-c5a2eb5542ea`

Este comandos se debe lanzar solo al finalizar la practica.

8 - Para borrar el Service Principal creado

`az ad sp delete --id 942b7c34-1bdc-45fe-ab1d-b7650c9a1cf0`

### Tarea 5: Configurar Access Policies del Key Vault

Ahora que tenemos un Service Principal associado a una aplicación registrada en Azure AD, podemos finalmente configurar el acceso al KeyVault con unas Access Policies. 

1 - Volvemos a nuestro resource group, y desde ahí al KeyVault.

2 - Dentro del menú izquierdo, pinchamos en **Access policies** y luego al link _+ Add Access Policy_.

  > ℹ️ Lo que vamos a hacer ahora, es darle permisos a esa entidad que acabamos de registrar en AD, para leer de nuestro Azure KeyVault.

3 - Lo único que vamos a necesitar configurar son los **Secret permissions**. Desmarcaremos todos excepto _Get_ and _List_, lo que corresponde al principio de  [_least privilege_](https://www.cyberark.com/what-is/least-privilege/).

![AzKeyVault_AcessPolicies](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzKeyVault_AcessPolicies.png)

4 - Ahora en la parte de **Select principal**, vamos a buscar por el nombre que le dimos a la entidad cuando la registramos anteriormente en Azure AD.

![AzKeyVault_AcessPolicies_Principal](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzKeyVault_AcessPolicies_Principal.png)

5 - Clicamos en _Select_ y luego en _Add_. Esto nos llevará de nuevo a la ventana principal del keyVault, donde no tendremos que olvidarnos de clicar en **Save**, o todos nuestros cambios anteriores se perderán, aunque estemos viendo aparecer nuestra nueva entidad en la sección de _APPLICATION_.

![AzKeyVault_AcessPolicies_Principal_registered](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzKeyVault_AcessPolicies_Principal_registered.png)

6 - Lo que hemos conseguido hasta el momento, es que cualquiera que tenga instalado el certificado anterior (con el thumbprint generado), tendrá permisos para leer y listar secretos de este keyVault.


### Tarea 6: Conectar nuestra aplicación web con Azure KeyVault mediante código.

1 - Desde el propio [portal de Azure](https://portal.azure.com/), nos dirigirnos al resource group que hemos creado al principio, para poder ver las propiedades del KeyVault _Modulo4Lab1-akv_.

![AzKeyVault_Menu](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzKeyVault_Menu.png)

2 - Una vez dentro, veremos un menú con opciones a la izquierda. Pinchamos en la opción **Secrets**. Una vez dentro, veremos un botón en la parte superior llamado **+ Generate/Import**, y aquí le vamos a dar los siguientes valores, (dejando el resto de campos con su valor por defecto):

  - Name: Module4Lab1-CosmosDb--Key
  - Value: pegamos la contraseña que hemos copiado desde nuestro Azure CosmosDB (y pegado en la tarea anterior en el código).

  > ℹ️ Nota: es importante mantener la estructura en el nombre, porque así nos será mucho más fácil recuperarla desde C#, ya que utilizaremos un proveedor de credenciales. Si nos fijamos detenidamente, los valores corresponden al nombre de la aplicación de C#, luego viene el nombre de la sección dentro del archivo de configuración, y finalmente el nombre de la propiedad a recuperar.

3 - Para que tenga sentido todo lo que estamos haciendo, es necesario también, que eliminemos la contraseña del código fuente. En su lugar podemos poner algo así _<Set-by-Keyvault>_ como se muestra a continuación.

![AzKeyVault_RemoveKeyFromCode](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/AzKeyVault_RemoveKeyFromCode.png)

4 - Antes de empezar con el código, es necesario instalar un paquete NuGet en nuestra aplicación:
  - Microsoft.Extensions.Configuration.AzureKeyVault

5 - Ahora tendremos que introducir en el archivo de configuracion _appsettings.json_ un nuevo grupo de variables, como se muestra a continuación (no importa si al principio o al final).

```json
  "KeyVault": {
    "Vault": "Modulo4Lab1-akv",
    "ClientId": "cccce832-8950-4f87-a713-5ffa9f37e0fa",
    "Thumbprint": "601F872ADCEECB4C496891044BB07D6C351EAC5E"
  }
```

6 - Ahora llega el momento de introducir nuevo código para que nuestra aplicación se conecte automáticamente con Azure al arrancar. Por eso nos vamos al código en el editor, y abrimos el archivo _Program.cs_. Ahora pegamos el siguiente código donde corresponde:

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

 7 - Como se puede observar, hay una nueva configuración creada, aparte de la que teníamos por defecto en la aplicación REST. Esto es lo que nos permitirá conectarnos automáticamente con nuestro Azure KeyVault, y ¿cómo lo hace?, pues vamos a verlo.

   - Lo primero es crear una nueva [AppConfiguration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihostbuilder.configureappconfiguration?view=dotnet-plat-ext-5.0), con el que construiremos la configuración de Az KeyVault.
    
   - Le indicamos cuál es el nombre del secreto:

```csharp
var vaultName = root["KeyVault:Vault"];
```

   - La siguiente llamada, conecta directamente con nuestro Az KeyVault, haciendo uso de varias cosas:

```csharp
  // Url en Azure del KeyVault: la obtenemos desde el portal de Azure, dentro de nuestro keyvault resource.
  $"https://{vaultName}.vault.azure.net/" 
```

```csharp
  // El ClientId: este es el Id que hemos copiado al hacer el registro de la WebApp en Azure AD.
  root["KeyVault:ClientId"] 
```

```csharp
  // El certificado self-signed instalado en nuestra computadora, creado en la tarea 4 
  // El código relativo a este método, lo encontraréis en punto 9 y lo podéis copiar al final de la clase **Program.cs**.
  GetCertificate(root["KeyVault:Thumbprint"]) 
```

```csharp  
  // Nuestro propio IKeyVaultSecretManager, en nuestro caso, _PrefixKeyVaultExample_.
  new PrefixKeyVaultExample("Module4Lab1") 
```

8 - El código relativo al Secret Manager va en una clase aparte, que tendremos que crear en la raíz del proyecto. Como podemos observar en el trozo de código anterior, se debe llamar _PrefixKeyVaultExample.cs_ y el código que contiene es este:

```csharp
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Module4Lab5
{
    public class PrefixKeyVaultExample :IKeyVaultSecretManager
    {

        private readonly string _prefix;
        public PrefixKeyVaultExample(string prefix)
        {
            this._prefix = $"{prefix}-";
        }

        public bool Load(SecretItem secret)
        {
            return secret.Identifier.Name.StartsWith(this._prefix);
        }

        public string GetKey(SecretBundle secret)
        {
            return secret.SecretIdentifier.Name.Substring(this._prefix.Length)
                .Replace("--", ConfigurationPath.KeyDelimiter);
        }
    }
}
```

> ℹ️ Para más información acerca del _Secret Manager_ podéis acceder a este [enlace](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-5.0#use-a-key-name-prefix).

9 - Ahora ya tenemos todo lo necesario para poder conectarnos desde nuestra aplicación a Azure CosmosDB, haciendo uso del secreto almacenado en nuestra cuenta de Azure KeyVault, de forma automática y segura. Nadie que tenga acceso a nuestro código fuente en C# será capaz de saber la contraseña de la base de datos.

Algunos pensaréis: la contraseña no está en texto plano en la configuración, pero ahora tenemos la URL + ClientId + thumbprint, ¿podríamos acceder nosotros también usando esos mismos datos, si encontrásemos la manera de acceder al código? La respuesta es no, porque necesitas tener el certificado instalado en tu propia máquina, y eso no lo tiene cualquiera, solo nosotros.

![CertificadoNoEncontrado](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/CertificadoNoEncontrado.png)


### Tarea 7: Comprobemos de nuevo la aplicación

Para poder ejecutar la aplicación desde nuestro pc u otro pc cualquier que contenga el código, vamos a necesitar instalar el certificado que creamos.

1 - Vamos a la carpeta donde tenemos el certificado con las claves, y le damos doble clic al archivo _certificate.pfx_.

![InstallCertificateLocalhost](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/InstallCertificateLocalhost.png)

2 - Seleccionamos _Current User_ y le damos a **Next**.

3 - En la siguiente ventana nos indica la ruta desde donde se está cargando el mismo. **Next**.

![InstallCertificateLocalhost_password](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/InstallCertificateLocalhost_password.png)

4 - Ahora nos pedirá la contraseña del certificado. Esta es la contraseña que nos pidió la consola de WSL cuando creamos el certificado con openSSL. La introducimos y le damos a **Next** (dejando el resto de campos por defecto).

5 - Si la contraseña es correcta, en la siguiente ventana solo tendremos que darle a **Next** una vez más.

6 - Clicar **Finish** y ya tendremos instalado nuestro certificado Self-Signed en nuestro pc.

![InstallCertificateLocalhost_done](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/InstallCertificateLocalhost_done.png)

7 - Ejecuta ahora la aplicación de C# y automáticamente se lanzará un navegador web con el contenido de la base de datos. Al mismo tiempo verás una consola, que no es otra cosa que el recurso web esperando peticiones REST. No lo cierres hasta que no termines de jugar con la app.

  ![RunAppWaiting](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/runappwaiting_localhost.PNG)

8 - Abrimos nuestro cliente REST y volvemos a ejecutar los mismos comandos que hicimos al inicio, cuando todavía teníamos la contraseña en texto plano.

9 - El resultado debería ser el mismo, es decir, deberíamos recibir la información de la base de datos Azure CosmosDB, pero ahora teniendo en cuenta que la contraseña se recupera de Azure KeyVault, gracias al certificado que tenemos instalado en nuestra máquina.

10 - Si quieres, intenta ejecutar algún comando PUT o DELETE para que veas que funciona correctamente.

| :zap:        Disclaimer!   |
|-----------------------------------------|

Ojo con lo que acabamos de hacer, es decir, hemos instalado un certificado productivo en nuestro pc local para comprobar cómo funciona esta funcionalidad, lo cual es totalmente desaconsejable. ¿Y cómo accedemos al KeyVault? Pues bien, lo que haremos será hacer un bypass de Azure Key Vault, y guardar directamente la contraseña de CosmosDB en local, que en definitiva era el valor que intentábamos obtener del KeyVault.

11 - No la vamos a guardar en el appsettings.json en plano, eso se queda igual. Abrimos un terminal de powershell y ejecutamos el siguiente código, que inicializará secretos de usuario de dotnet (como un vault local). El id de este vault local de dotnet se verá reflejado en el csproj de la app.

 ```shell
  dotnet user-secrets init
 ```

 12 - A continuación, vamos a crear nuestro secreto con la contraseña de CosmosDb, ejecutando el siguiente comando que sigue la estructura de los valores del appsetting.json:

  ```shell
  dotnet user-secrets set "CosmosDb:Key" "<put your CosmoDb key>"
  ```

  13 - Ahora para comprobar que funciona, vamos a borrar el nombre del keyvault del appsetting.json, y en el código de **Program.cs** introduciremos el segundo código:

```json
  "KeyVault": {
    "Vault": "",
    "ClientId": "<your_clientId>",
    "Thumbprint": "<your_thumbprint>"
  }
```

```csharp
  ...
  .ConfigureAppConfiguration(builder =>
  {
      var root = builder.Build();
      var vaultName = root["KeyVault:Vault"];

      if (!string.IsNullOrEmpty(vaultName)) // <--- aquí está lo importante
      {
          builder.AddAzureKeyVault($"https://{vaultName}.vault.azure.net/",
          root["KeyVault:ClientId"],
          GetCertificate(root["KeyVault:Thumbprint"]), new PrefixKeyVaultExample("Modulo8test1"));
      }
  })
  ...
```

### Tarea 8: Eliminar todos los recursos creados :bomb:

Al final de cada ejercicio es importante dejar nuestra cuenta de Azure limpia para evitar sobrecostes nos esperados por parte de Microsoft.
Para eliminar todos los recursos del ejercicio, vamos a hacer lo siguiente:

1 - En el portal de Azure, abrimos sesión de **Bash** dentro del panel de Cloud Shell.

2 - Eliminamos el resource groups creado en el lab, ejecutando el siguiente comando:
```bash
az group delete --name <your_resource_group_name> --no-wait --yes
```

3 - No olvidemos eliminar también el resto de recursos, como el certificado en local. No es necesario porque tiene fecha de expiración, pero es bueno no dejar basura.
  - Tecla "Windows" + "R" para sacar la aplicación de ejecución.
  - Escribimos el comando **certmgr.exe** y le damos a _OK_.
  - Dentro de la ventana del gestor de certificados, vamos al menú **Add/Remove Snap-in**, seleccionamos _Certificates_ en la parte izquierda y lo añadimos al listado de la derecha, luego selecciona _My User Account_ y _Finish_.

![Certificate_localhost_Delete](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab4/Certificate_localhost_Delete.png)

  - Clic _OK_ y ya veremos el listado de certificados en el árbol -> Console Root/Certificates - Current User/Personal/Certificates
