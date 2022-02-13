
| Lab |  2 |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Seguridad en el cloud | Protege tus aplicaciones mediante el uso de WAF y Azure Front Door |

### Lab overview
Una de las mejores razones para utilizar Azure con tus aplicaciones y servicios, es la de aprovecharse del amplio rango de herramientas de seguridad que incluye. Estas herramientas ayudan a hacer posible la creación de soluciones seguras en la plataforma de Azure.

![WAF Picture](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_mainPicture.jpg)

### Objetivos
En este lab aprenderemos algunas herramientas de Azure con las cuales podremos asegurar nuestras aplicaciones web. Existen varias como Azure Application Gateway, Azure Front Door, Azure Web Application Firewall (WAF), etcétera. Nosotros nos vamos a centrar en estas 2 últimas.

### Acrónimos

### Prerrequisitos

- Tener una suscripción de Azure vigente.
- Tener instalado VS Code.
- Tener instalado el plug-in de VS Code para Azure App Services.

### Duración
TBD

### Instrucciones

Lo primero que necesitamos es tener nuestra subscripción de Azure:
- Navegamos al [portal](https://portal.azure.com/#home). 
- Introducimos la cuenta de la suscripción y la contraseña.

#### 1 - Creación en Azure de una Service App con su base de datos SQL Server.

Lo primero que vamos a necesitar es crear un App service donde alojaremos nuestra web hackeable, y una base de datos donde se almacenarán los datos que queremos explotar. Por suerte, Azure ya nos provee de esta opción en un combo, para que no tengamos que crearlo por separado.

- Nos vamos al menú lateral izquierdo, y le damos a la opción _+ Create resource_ (Crear recurso).
- En la siguiente ventana, en la barra de búsqueda, escribimos **Web App + SQL** y seleccionamos la opción que no sale con dicho nombre (intenta no dejar espacios porque sino tendrás problemas para encontrarlo).

![Web+SQL](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL.jpg)

- Le damos a _Create_ y en los campos que nos pide a continiación le damos los valores siguiente:

|Propiedad | Valor |
| --  | -- |
| App name | afdoor-app |
| Subcription | <La vuestra seguramente estará para elegir de una lista> |
| Resource Group | afdoor-rg |

![WebSQL_plan](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_plan.png)

- Ahora todavía necesitamos seleccionar un plan, que no es otra cosa que la forma en la que nos van a facturar el uso de dicha aplicación web. Pinchamos en dicha opción y se nos abrirá otra ventana.

  - Le damos a _Create new_.
  - En el nombre pondremos **afdoor-sp** y en location **West Europe**. 
  - Pricing tier lo dejamos como está por defecto.
  - Clic 'OK'.

![WebSQL_DB](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_db.png)

- Lo siguiente es crear la base de datos SQL Server. Lo primero que nos pide es el nombre de la base de datos, el cual será **afdoordatabase**. El siguiente campo es el servidor bajo el cual se alojará dicha base de datos, pero como nunca hemos creado ninguno, tendremos que hacerlo en el siguiente paso, pinchando en _Select server_.
  - En el nombre le ponemos **afdoorsqlserver** 
  - El usuario y contraseña a tú elección, pero recuérdalos.
  - Location será la misma que antes, por mantener una consistencia (aunque podría ser otra).
  - Clic 'Select'.

Hemos vuelto atrás, y solo nos queda elegir el _Pricing tier_ y el _collation_. Ambos los dejaremos por defecto y haremos clic sobre 'Select'. Con todo configurado, solo nos queda darle a 'Create' para que arranque el proceso de creación de todo lo que hemos seleccionado.

![WebSQL_Deploy](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_deploying.png)

Puede tardar varios minutos en levantarse toda la infraestructura, pero cuando todo esté listo, dentro del grupo de recursos que acabamos de crear, deberíamos tener todos los recursos que se muestran a continuación.

![WebSQL_DeploymentCompleto](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_deployed.png)

#### 2 - Publicar el contenido web que necesitamos para el laboratorio.

Vamos a comprobar que efecticamente podemos navegar hacia la web que acabamos de desplegar. Para ello, dentro del grupo de recursos, seleccionamos el del App Service. Una vez dentro, en la parte superior vamos a encontrar la **URL** que contendrá la web desplegada. La copiamos y la pegamos en el navegador (o pinchamos sobre la propia url en azul) donde veremos algo así.

![WebSQL_DeploymentCompleto](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_DummyWeb.png)

Esto significa que el desplegado ha funcionado como esperábamos, pero claro, esta no es la web que necesitaremos para poder explotar ciertos agujeros de seguridad. Vamos a necesitar una que esté preparada.

En la carpeta de [recursos](../../Recursos/) vais a encontrar un archivo zip llamado _ContosoClinic.zip_, que contendrá la página web que estamos buscando. 

- Descargamos el archivo zip y lo descomprimimos en local.
- Abrimos VS Code.
- Ahora 'Archivo > Abrir carpeta' y mostramos dicha carpeta de nuestro local con la web descomprimida.
- El siguiente paso será publicar el contenido de la carpeta actual en el App Service que creamos dentro de nuestro grupo de recursos de Azure. Para ello, pinchamos en el icono de Azure de la barra lateral izquieda y se nos mostrarán las suscripciones activas.

![WebSQL_VSCode](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_VSCode_Azure.png)

- A continuación, con botón derecho sobre la suscripción, seleccionamos la opción **Deploy to Web App**.

![WebSQL_VSCode](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_VSCode_Azure2.png)

- En la parte superior de VS Code, nos pedirá la ruta donde está el contenido web a desplegar. Lo seleccionamos y automáticamente nos preguntará si queremos hacer el despliegue, y confirmamos en **Deploy**.

![WebSQL_VSCode](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_VSCode_Azure4_Overwrite.png)

![WebSQL_VSCode](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_VSCode_Azure5_Deploy.png)

Cuando acabe de desplegar, le podemos dar a _Browse Website_ para ver el resultado. Puede tardar un poco porque tenga que cachear la web primero, pero ya deberíamos ver 'Contoso clinic' abierto en el navegador como nuestra web de uso.

#### 3 - Creamos el Azure Front Door

Ahora mismo, el dibujo inicial está así:

![WAF Picture2](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_mainPicture_b.png)

Necesitaremos definir todo el sistema de seguridad que da acceso a nuestra web, para lo cual empezaremos por el AFD. Además, cuando vayamos a crear el WAF, en uno de los pasos nos pedirá que seleccionemos el _frontend host_, que no es otra cosa que esto que vamos a definir a continuación.

![AzureFD_create](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_frontdoor.png)

- Vamos a la barra de búsquedas y escribimos 'front door' y seleccionamos el recurso del mismo nombre. Le damos a **Create**.
- Una vez dentro del menú _basics_, le damos estos valores a los campos y seleccionamos '> Next: Configuration':

|Propiedad | Valor |
| --  | -- |
| Subscription | la nuestra |
| Resource group | afdoor-rg |
| Resource group location | Cogerá automáticamente la del resource group |

![FrontDoor_FE](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_frontdoor_diagram.png)

- Aquí vamos a definir nuestro **FrontEnd**, **BackEnd** y las **reglas de enrutado**. 
  - Hacemos clic sobre el botón para añadir el frontEnd primero.Le damos el valor 'frontenddomainub' al _Host name_ y dejamos el resto de campos como están. Clic Add.  
  - Lo siguiente es definir el backend. Para ello pinchamos en el botón para añadir uno. Le damos el valor 'backendpoolub' al nombre. A continuación necesitamos indicarle cuál es/son realmente nuestro/s backend/s. Le damos a _+Add a backend_ y lo rellenamos con los valores de la tabla de abajo (los demás por defecto). Podemos añadir tantos backends como queramos, pero en este caso ya es suficiente. Le damos a _Add_ y listo.
  - Finalmente vamos a definir las reglas de enrutamiento. Hay que darle un nombre, que será 'rulesetub'. Todos los demás valores por defecto.
  
|Propiedad | Valor |
| --  | -- |
| Backend host type | App service |
| Subscription | La nuestra |
| Backend host name | afdoor2-app.azurewebsites.net  |
| Backend host header | afdoor2-app.azurewebsites.net |

> NOTA: el frontEnd dentro del AFD, no es otra cosa que una dirección url o subdominio al que apuntará nuestra aplicación web, de manera que cuando se hagan peticiones a ella, si no hay ningún problema detectado por el frontdoor, se redirigirá el tráfico a la web real (osea nuestro App Service).

- Ahora ya podemos movernos a _Review + create_, y cuando la validación esté en verde, _Create_ para finalizar.

#### 4 - Creamos el Azure WAF asociado al AFD del paso anterior.

Una vez el front door está levantado, solo nos queda definir el AFD que se asociará a dicho front Door.

- Podemos acceder de dos maneras, bien nos vamos al buscador de arriba o al menu lateral izquierdo y seleccionamos la opción _Create a resource_.
- Escribimos **Web Application** y seleccionamos la que tiene el nombre completo "Web Application Firewall (WAF)" (no confundir con el recurso 'Azure Web Application Firewall (WAF)'). Le damos a botón crear.

![WAF_create](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_waf_create.png)

- Ahora nos aparecen varios campos para rellenar, a los cuales les daremos los valores siguientes:

|Propiedad | Valor |
| --  | -- |
| Policy for | Global Front Door |
| Subscription | La que tengáis activa |
| Resource group | afdoor-rg |
| Policy name | afdoorpolicyub |
| Policy state | 'True' o seleccionado |
| Policy mode | Detección |

Si nos fijamos, al seleccionar el _Front Door_, nos aparece un desplegable más llamado **Front Door SKU**, que no es otra cosa que la capa de coste que Azure le mete por medio, para que controlemos el gasto que hacemos con dicho recurso. Lo dejamos con su valor por defecto.

- Le damos a "> Next: Managed Rules". Las revisamos, pero las dejamos como están, no hace falta tocar nada.
- Le damos a "> Next: Policy settings". En esta ventana, podemos indicar dónde queremos que nos redireccione en caso de que el AFD detecte alguna intrusión. Para hacerlo más simple, nosotros le vamos a pegar en siguiente código en el campo **Block response body**.

```html
! Alto ¡ Estás intentando una acción maliciosa.
```

- Le damos a "> Next: Custom rules" y sin cambiar nada, a "> Next: Association". Aquí vamos a indicarle el frontend host al que queremos asociar este _Front Door_. Para ello, le damos donde poner **+ Add frontend host** y se nos abrirá una ventana lateral en la derecha. Si lo hemos configurado bien antes, nos debería aparecer seleccionado por defecto, el _Frontdoor_, con lo que solo deberemos seleccionar el _Frontend host_ de la lista desplegable siguiente. Clic en _Add_.

- Le damos ya directamente a **Review + create** y cuando esté en verde, le damos a _Create_ y esperamos que se despliegue.

#### 5 - Desabilitamos acceso a App Service directamente.

Ahora mismo, la única manera en la que los usuarios deben ser capaces de acceder a la web, es mediante el Azure Front Door. Esto implica que tenemos un punto de acceso que bloquear, y ese es la url/ip que nos provee el _App Service_.

- Para bloquear este acceso, vamos a dirigirnos al App Service que tenemos creado dentro del grupo de recursos.
- Ahora en el menú lateral izquierdo, dentro de **Settings**, pinchamos en _Networking_.

![BlockAccess](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_blockaccess.png)

- Vamos abajo donde dice _Access restrictions/Configure Access Restrictions_.
- Crearemos una regla nueva, le damos estos valores y a continuación hacemos clic en _Add Rule_.

|**Propiedad** | **Valor** |
| --  | -- |
| Nombre | BloqueoAccesoDirecto |
| Action | Allow |
| Priority | 1 |
| Description | \<Algo que tenga sentido>\ |
| Source/Type | AzureFrontDoor.Backend | 
  
- Comprueba que accediendo desde tu navegador a la ip pública del App Service, ya no eres capaz de ver la web. De lo contrario, hemos hecho algún paso mal. 

#### 6 - Probamos la inyección SQL sobre la web

Ahora mismo, solo deberíamos ser capaces de acceder al contenido de la web a través de la url del AFD. 

- Por tanto, vamos a copiar dicha url y la vamos a pegar en el navegador web.

- Cuando se nos abra la web, nos vamos a la pestaña de **Patients**, e introduciremos un par de registros para jugar con ellos.

- Ahora, en la barra de búsqueda vamos a introducir el siguiente texto, el cuál va a simular una inyección SQL en toda regla.

![lab2_module2_webappTestAttack](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappTestAttack.png)

- Hacemos click en buscar, y si todo está bien configurado, veremos el mensaje previamente definido en el bloque 4.

![ErrorAtaque](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/lab2_module2_frontdoor_alert.png)
  

### Ejercicios relacionados

Existen varios ejemplos de Microsoft parecidos que podéis intentar por vuestra cuenta, para los cuales ya se os facilita toda la infraestructura:
- [SQL Inyection Attack](https://github.com/Azure/azure-quickstart-templates/tree/master/demos/SQL-Injection-Attack-Prevention#detect)
- [Cross Site Scripting Attack](https://github.com/Azure/azure-quickstart-templates/tree/master/demos/SQL-Injection-Attack-Prevention#detect)
