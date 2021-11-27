
| Lab |  1  |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| DevSecOps | Desplegar y ejecutar una aplicación web containerizada con Azure App Service |

### Lab overview

### Objetivos

En este módulo vamos a conseguir lo siguiente
- Crear imágenes Docker y almacenarlas en un repositorio de ACR.
- Utilizar Azure App Service para ejecutar aplicaciones web basadas en imágenes Docker almacenadas en ACR.
- Utilizar webhooks para configurar despliegue continuo de una web basada en imágenes Docker.
  
### Acrónimos

- ACR: [Azure Container Registry](https://docs.microsoft.com/en-us/azure/container-registry/)

### Prerrequisitos

- Tener una suscripción de Azure vigente.

### Duración

35 minutos

### Instrucciones

Lo primero que necesitamos es tener nuestra subscripción de Azure:
- Navegamos al [portal](https://portal.azure.com/#home). 
- Introducimos la cuenta de la suscripción y la contraseña.

### Ejercicio 1: Compilar y almacenar una imagen utilizando el ACR.

ACR provee almacenamiento para imángenes Docker en la nube.

En este ejercicio de ejemplo, el equipo necesita crear iun registro para almacenar imágenes para sus aplicaciones web.

En este módulo, utilizaremos el portal de Azure para crear un nuevo registro en ACR. Compilaremos una imagen de Docker desde el código fuente de una aplicación web, y lo subiremos al repositorio dentro de nuestro registro de Azure. Finalmente, examinaremos el contenido del registro y del repositorio.

#### Create a registry in Azure Container Registry

1.- Logueate en el portal de Azure con tu subscripción.
2.- En la página de inicio del portal de Azure, bajo **Azure Services** selecciona _Create a resource_. Aparecerá el panel de _Create a resource_.
3.- En el panel izquierdo, selecciona **Containers** y bajo el listado de productos más populares, selecciona **Container Registry** (también podemos buscarlo en el cuadro de texto superior).

![ContainerRegistry](../../Recursos/3%20-%20DevSecOps/lab1_modulo3_ContainerRegistry.png)

Aparecerá el panel para crear el registro de contenedores.

4.- En la pestaña de _basics_, introduce la siguiente información:

|Setting|Value|
|---|---|
|**Project Details**||
|Subscription|Selecciona tu suscripción (si solo hay una, se marcará por defecto)|
|Resource group|Selecciona _Create new_, y escribe `learn-deploy-container-acr-rg` y dale a OK. Este método te ayudará a deshacerte de los recursos una vez terminada la práctica. Si elijes otro nombre, asegúrate de mantenerlo a lo largo de todo el ejercicio.|
|**Instance Details**||
|Registry name|Introduce un nombre único y recuérdalo para más adelante|
|Location|Selecciona una localización cercana (West Europe por ejemplo)|
|SKU|Standard|

5.- Selecciona **Review+create**. Tras la validación positiva, selecciona **Create**. Espera hasta que el registro de contenedores se haya creado antes de proceder al siguiente paso. No debería tardar mucho.

#### Compilar una imagen Docker y subirla al ACR.

1.- Desde el shell de Azure dentro del portal (selecciona el primer icono en la barra de herramientas superior), ejecuta el siguiente comando para descargarte el código fuente de una web de ejemplo. Esta web es simple. Nos muestra una única página que contiene texto estático.

```shell
git clone https://github.com/MicrosoftDocs/mslearn-deploy-run-container-app-service.git
```

2.- Muévete a la carpeta del código fuente:

```shell
cd mslearn-deploy-run-container-app-service/dotnet
```

3.- Ejecuta el siguiente comando.

```shell
az acr build --registry <container_registry_name> --image webimage .
```

Este comando envía el contenido de la carpeta al registro de contenedores, el cual utiliza las instrucciones del Dockerfile para compilar la imagen y almacenarla. Reemplaza `<container_registry_name>` con el nombre del registro que hemos creado previamente. No te olvides del `.` al final de la línea.

> Nota: el archivo Dockerfile contiene instrucciones paso a paso para compilar una imagen Docker desde el código fuente de la aplicación web. El registro ejecuta estos pasos para compilar la imagen, y con cada comando, se muestra un nuevo mensaje. El proceso debería terminar pasados un par de minutos sin errores ni warnings.

#### Examina el registro

1.- Volvemos al portal de Azure, y en el menú **Overview** de nuestro registro de contenedores, selecciona _Go to resource_. Aparecerá ahora el panel de nuestro registro.

2.- En el menú lateral izquierdo, bajo _Services_, selecciona **Repositories**. Aparecerá el panel de los repositorios, donde veremos uno llamado `webimage`.

3.- Selecciona el repositorio `webimage`. Aparecerá el panel de dicho repositorio, el cual contendrá una sola imagen con la etiqueta de `latest`. Esta será la imagen Docker de nuestra aplicación web.

![ContainerRegistryRepo](../../Recursos/3%20-%20DevSecOps/lab1_modulo3_ContainerRegistryRepo.png)

La imagen Docker que contiene nuestra aplicación web, está ahora disponible en nuestro registro para ser desplegada a un App Service.

### Ejercicio 2: Crear y desplegar una aplicación web desde una imagen de Docker.

Azure App Service proporciona el entorno de hospedaje para una aplicación web basada en Azure. Podemos configurar un App Service para recuperar la imagen de la aplicación web desde un repositorio en ACR.

En este ejemplo, el equipo ha cargado la imagen de la aplicación web en ACR y ahora está listo para implementar la aplicación web.

En esta unidad, crearemos una nueva aplicación web mediante la imagen de Docker almacenada en Azure Container Registry. Utilizaremos el App Service con un plan de App Service predefinido para alojar la aplicación web.

#### Enable Docker access to the Azure Container Registry

Utilizaremos Docker para loguearnos en el registro, y descargaremos la imagen web que queremos desplegar. Docker necesita un usuario y una contraseña para realizar esta operación. El registro de contenedores nos proporciona la posibilidad de habilitar el nombre del registro como nombre de usuario y la _admin access key_ como la contraseña, para permitir a Docker loguearse en tu registro de contenedores.

1.- Desde el portal de Azure, vamos a **All resources**.

2.- Selecciona el registro que hemos creado en el primer ejercicio.

3.- En el panel del menú izquierdo, bajo _Settings_, selecciona **Access keys**. Aparecerá el panel de nuestro registro de contenedores.

4.- Cambia la opción **Admin user** a _Enabled_, cuyo cambio se guardará automáticamente.

Ahora ya estamos listos para crear nuestra aplicación web.

#### Create a web app

1.- Regresamos a la página principal del portal, y bajo _Azure services_, seleccionamos **Create resource**. Aparecerá entonces el panel de para crear recursos.

2.- En el menú izquierdo, selecciona **Web**, y bajo _Popular offers_, seleciona **Web App** (también lo podemos buscar en el cuadro superior directamente).

![Crear webapp](../../Recursos/3%20-%20DevSecOps/lab1_modulo3_Part2_WebApp.png)

Aparecerá entonces el panel para crear una Web App.

3.- En la pestaña de _basics_, entramos los siguientes valores:

|Setting|Value|
|---|---|
|**Project Details**||
|Subscription|Selecciona tu suscripción (si solo hay una, se marcará por defecto)|
|Resource group|De la lista desplegable, seleccionamos el ya existente llamado `learn-deploy-container-acr-rg`|
|**Instance Details**||
|Name|Introduce un nombre único y recuérdalo para más adelante|
|Publish|**Docker Container**|
|Operating system|Linux|
|Region||
|**App Service Plan**||
|App Service Plan| Utiliza el valor por defecto|

4.- Seleciona **Next: Docker >**

5.- En la pestaña de _Docker_, inserta los siguiente valores:

|Setting|Value|
|---|---|
|Options|**Single Container**|
|Image source|**Azure Container Registry**|
|**Azure container registry options**||
|Registry|Selecciona tu registro|
|Image|`webimage`|
|Tag|latest|
|Startup command|Deja esta valor en blanco|

6.- Selecciona **Review and create**, y después selecciona **Create**. Espera a que termine de desplegarse para continuar.

#### Test the web app

1.- Después de que finalice correctamente el despliegue, selecciona _Go to resource_ para ver la aplicación web que acabamos de crear. Nos aparecerá el panel del App Service para nuestra web.

2.- En el menú superior, selecciona **Browse** o pincha en la _url_ que nos provee.

3.- Después de unos segundos de retraso, mientras nuestra imagen de Docker se descarga desde el repositorio y se carga, veremos una página que se parece algo a esta.

![Mostrar webapp](../../Recursos/3%20-%20DevSecOps/lab1_modulo3_Part2_WebAppSample.png)

El App Service está ahora hospedando la aplicación contenida en nuestra imagen de Docker.

### Ejercicio 3: Actualizar la imagen Docker y desplegar de nuevo la aplicación web.
