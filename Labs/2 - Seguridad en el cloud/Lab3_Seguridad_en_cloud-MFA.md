
| Lab |  3 |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Seguridad en el cloud | Multifactor authentication, acceso condicional y AAD Identity Protection |

### Lab overview
Imaginemos que se nos pide crear una prueba de concepto de una feature, que mejorará la autenticación en Azure Active Directory (Azure AD). En concreto, queremos evaluar estas tres cosas:
- Azure AD autenticación multi-factor
- Azure AD acceso condicional
- Azure AD protección de identidades

> Para todos los recursos de este lab, vamos a usar la region "West Europe".

### Objetivos
Los objetivos de esta práctica será completar los siguientes escenarios:
- Ejercicio 1: Desplegar una máquina virtual en Azure utilizando un template de ARM.
- Ejercicio 2: Implementar autenticación multi-factor.
- Ejercicio 3: Implementar políticas de acceso condicional en Azure AD.
- Ejercicio 4: Implementar protección de identidades en Azure AD.

### Acrónimos
- AD: [Active directory](https://docs.microsoft.com/es-es/azure/active-directory/fundamentals/active-directory-whatis)
- ARM: [Azure Resource Manager](https://docs.microsoft.com/en-us/azure/azure-resource-manager/management/overview).

### Prerrequisitos

- Tener una suscripción de Azure vigente.

### Duración

Aproximadamente, el reparto de tiempos por ejericio es el siguiente:
 - Ejercicio 1 => 10 minutos
 - Ejercicio 2 => 30 minutos
 - Ejercicio 3 => 15 minutos
 - Ejercicio 4 => 30 minutos

### Instrucciones

Lo primero que necesitamos es tener nuestra subscripción de Azure:
- Navegamos al [portal](https://portal.azure.com/#home). 
- Introducimos la cuenta de la suscripción y la contraseña.

#### Ejercicio 1 - Desplegar una máquina virtual en Azure usando ARM.

En esta tarea, crearemos una máquina virtual utilizando un template en ARM. Esta VM se utilizará en el último ejercicio de este lab.

- En el portal de Azure, en la barra superior de "Search resources, services, and docs", escribe **Deploy a custom template**.

  > Nota: También puedes acceder al Marketplace de Microsoft y buscar **Template Deployment (utilizar tus propios templates)**.

- En la ventana de implementación personalizada, haz clic en la opción "Build your own template in the editor".
- Una vez dentro, hacemos clic al botón _Load file_ y selecionamos el template ARM de este repositorio. [Aquí]( https://github.com/MscDevOpsSecurity/AzureLabs/blob/main/Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/az-500-04_azuredeploy.json).

  > Nota: Revisa el contenido del template y asegúrate que se despliega una VM con Windows Server 2019 Datacenter.   

- Ahora le damos a **Save**.
- Ahora desde la ventana de _Custom Deployment_, antes de introducir ningún valor, le damos arriba a **Edit parameters**.
- Esto nos abrirá una nueva ventana y le daremos a **Load file** para cargar el otro archivo ARM. [Aquí](https://github.com/MscDevOpsSecurity/AzureLabs/blob/main/Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/az-500-04_azuredeploy.parameters.json).

  > Nota: Revisa el contenido de los parámetros y date cuenta de los valores de _adminUsername_ y _adminPassword_.

- Ahora dale otra vez a **Save**.
- De vuelta en la ventana de creación de _Custom deployment_, seguramente solo tengamos que rellenar el _Resource group_ porque los demás ya estarán puestos. Asegúrate de que todo está como en esta tabla.

|Setting|Value|
| --  | -- |
|Subscription|	el nombre de la subscripción de Azure que estás usando en este lab|
|Resource group|	haz clic en _Create new_ y escribe el valor AZ500LAB04|
|Location|	Western Europe|
|Vm Size|	Standard_D2s_v3|
|Vm Name|	az500-04-vm1|
|Admin Username|	Student|
|Admin Password|	Pa55w.rd1234|
|Virtual Network Name|	az500-04-vnet1|

> Nota: para asegurarte y saber qué regiones permiten la provisión de VMs en Azure, revisa el enlace https://azure.microsoft.com/en-us/regions/offers/.

![Imagenvalidateok](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_ValidationPassed.png)

- Haz clic en _Review + create_ y luego en **Create**.

> Nota: No esperes a que se complete el despliegue para proceder al siguiente ejercicio. Usaremos esta VM en el último ejercicio de este lab.

#### Ejercicio 2 - Implementar Azure MFA.

##### Tarea 1: Crear un nuevo Azure AD tenant.

- En el portal de Azure, en la barra superior de "Search resources, services, and docs", escribe **Azure Active Directory** y presiona Enter.
- Una vez dentro del Azure AD, en el menú izquierdo de _Overview_ de tu tenant por defecto, clic en **Manage tenants** y justo en la nueva ventana que aparece, selecciona _+ Create_.
- En la pestaña _basics_ de crear un nuevo tenant, asegúrate que la opción **Azure Active Directory** está seleccionada y haz clic en "_Next: Configuration >_".
- En la pestaña de configuración de creación de nuevo tenant, especifica los siguiente valores:

  |Setting|Value|
  | --  | -- |
  |Organization name|	AdatumLab500-04 |
  |Initial domain name	| nombre único consistente en una combinación de letras y dígitos|
  |Country or region	|Spain|

  > Nota: guarda el nombre del dominio inicial, lo necesitaremos más adelante en este lab.

- Clic en _Review_ y después _Create_.
- Escribe el contenido del captcha en el apartado _Help us prove you're not a robot_ y haz clic en **Submit**.

![Imagencaptcha](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_Captcha.png)

> Espera a que se cree el nuevo tenant. Utiliza el icono de notificaciones para monitorizar el estado del despliegue.

##### Tarea 2: Activare AD Premium P2 trial

Si cuando se ha acabado de generar el tenant no has hecho clic en el icono que decía _Clic para ir al nuevo tenant_, entonces empieza por el primer paso. De lo contrario, salta al paso 2.

- En el portal de Azure, en la barra de herramientas, haz clic en el icono de **Directory and Subscription**, que se encuentra a la derecha del icono del shell.

![ImagenDirAndSubs](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_DirectoriosSubs.png)

- Una vez dentro, haz clic en botón **Switch** del tenant que acabamos de crear. Esto nos redirigirá a una nueva sesión de dicho tenant.
- Dentro del tenant _AdatumLab500-04_, en el menú izquierdo de _Manage_, seleccionamos **Licenses**.
- Ahora dentro de Licencias, en la parte de _Manage_, seleccionamos _All products_.
- En la parte superior nos aparecerá un botón llamado **+ Buy/Try** que seleccionaremos.
- En la pestaña que se abre de _Activate_, desplegamos el submenú que dice _Free Trial_ y le damos a **Activate**.

##### Tarea 3: Crear usuarios y grupos en el AD.

En esta tarea vamos a crear 3 usuarios: aaduser1 (Global Admin), aaduser2 (user) y aaduser3 (user). Vamos a necesitas guardar el nombre princial y la contraseña de cada uno de ellos para usarlos más adelante.

- Vamos de nuevo a la sección de AdatumLab500-04 del Azure AD, y en la sección de _Manage_ hacemos clic a **Users**.
- Dentro de los usuarios, hacemos clic en **+ New User**.
- Ahora nos aseguramos de que esté marcada la opción _Create user_, y especificaremos los siguientes valores (dejaremos el resto con sus valores por defecto). Después clic _Create_.

  |Setting|Value|
  | --  | -- |
  |User name|aaduser1|
  |Name|	aaduser1|
  |Password|	asegúrate que la opción 'Auto-generate password' esté seleccionada y marca la opción _Show Password_|
  |Groups|	0 groups selected|
  |Roles|	clic en User, luego busca Global administrator, y clic en Select|
  |Usage Location|	Spain|

  > Nota: Guarda el nombre completo. Puedes copiar su valor haciendo clic en el botón **Copy to clipboard** al lado derecho del desplegable que muestra el nombre de dominio.
  > Nota: Guarda la contraseña de usuario. Lo necesitarás más adelante en el lab.

- De vuelta en usuarios, volvemos a seleccionar la opción de **+ New User**.
- En la pestaña de creación de usuarios, nos aseguramos de que esté marcada la opción _Create user_, y especificaremos los siguientes valores (dejaremos el resto con sus valores por defecto). Después clic _Create_.

  |Setting|Value|
  | --  | -- |
  |User name|aaduser2|
  |Name|	aaduser2|
  |Password|	asegúrate que la opción 'Auto-generate password' esté seleccionada y marca la opción _Show Password_|
  |Groups|	0 groups selected|
  |Roles|	User|
  |Usage Location|	Spain|

  > Nota: Guarda el nombre completo y la contraseña.

- De vuelta en usuarios, volvemos a seleccionar la opción de **+ New User**.
- En la pestaña de creación de usuarios, nos aseguramos de que esté marcada la opción _Create user_, y especificaremos los siguientes valores (dejaremos el resto con sus valores por defecto). Después clic _Create_.

  |Setting|Value|
  | --  | -- |
  |User name|aaduser3|
  |Name|	aaduser3|
  |Password|	asegúrate que la opción 'Auto-generate password' esté seleccionada y marca la opción _Show Password_|
  |Groups|	0 groups selected|
  |Roles|	User|
  |Usage Location|	Spain|

> Nota: Guarda el nombre completo y la contraseña.
> Nota: a estas alturas, deberías tener ya 3 usuarios listados en la página de usuarios.

![ImagenUsers](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_Users.png)

##### Tarea 4: Asignar licencias de Azure Premium P2 a los usuarios de Azure AD.

En esta tarea, vamos a asignar a cada usuario del AD, una licencia de Azure AD Premium P2.

- Volvemos a la pestaña inicial del tenant AdatumLab500-04, en la sección de **Manage**, clic en _Licences_.
- Dentro de licencias, sección _Manage_, clic _All products_ y selecciona de la lista el checkbox de **Azure Active Directory Premium P2**, luego clic en **+ Assign** justo encima.
- En la nueva pestaña que se abre, clic en **+ Add users and groups**.
- Como tendremos pocos, no hará falta buscar. Selecciona los 3 usuarios creados recientemente y el tuyo propio. Clic _Select_.
- De vuelta en la pestaña de _Assign license_, pasamos al siguiente paso del asistente llamado "_Next: Assignment options >_". Asegúrate que todas las opciones están habilitadas.
- Clic en _Review + asssign_ y después **Assign**.
- Ahora tendremos que cerrar sesión con nuestro usuario y volver a hacer log in para que se puedan aplicar los cambios de licencias sobre el usuario activo.
  > Nota: en este momento, hemos asignado licencias de Azure AD Premium P2 a todos los usuarios que usaremos en este lab. Asegúrate de cerrar y abrir sesión.

##### Tarea 5: Configurar las settings de Azure MFA.

En esta tarea, configuraremos MFA y lo habilitaremos para el usuarios aaduser1.

- En el portal de Azure, navegamos de nuevo a la ventana del nuevo tenant _AdatumLab500-04_.
  > Nota: asegúrate de estar usando el tenant _AdatumLab500-04_.
- Desde la pestaña de nuevo tenant, en la sección _Manage_ hacemos clic en **Security**.
- Ahora, desde la propia pestaña de Security, en la sección de _Manage_, pinchamos en **MFA**. 
- Dentro de Multi Factor Authentication, veremos un enlace que dice _Additional cloud-based MFA settings_.
  > Nota: esto abrirá una nueva ventana del navegador, mostrando las opciones de MFA.
- En la página de MFA, aunque no se vea muy bien, hacemos clic en la pestaña de _service settings_.Revisamos las **verification options**. Asegúrate que _Text message to phone_, _Notification through mobile app_, y _Verification code from mobile app or hardware token_ estás habilitadas. Clic en _Save_ y luego en _close_.
- Ahora, dentro de esta misma pestaña, volvemos a los usuarios. Selecciona el usuario aaduser1, clic el enlace a la derecha de **Enable**, y cuando nos salga otra ventana encima, seleccionamos _enable multi-factor auth_ y después _close_.

![ImagenMFAAuth](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_MFAUser.png)

- Confirma que la columna de **Multi-Factor Auth status** para el usuario aaduser1 está ahora en estado **Enabled**

![ImagenMFAAuth2](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_MFAUserEnabled.png)

- Selecciona otra vez el usuario aaduser1 y date cuenta que ahora incluso tenemos un nuevo enlace a la derecha que dice **Enforce**.
  > Nota: cambiar el estado del usuario de Enable a Enforced impactará solo a apps viejas integradas en Azure AD, las cuales no soportan Azure MFA y, una vez que el estado cambia a Enforced, requerirá el uso de contraseñas para la misma.
- Con el usuario aaduser1 seleccionado, hacemos clic en **Manage user settings** y revisa las opciones mostradas (sin activar ninguna):
  - Require selected users to provide contact methods again.
  - Delete all existing app passwords generated by the selected users.
  - Restore multi-factor authentication on all remembered devices.
- Cancela y vuelve a la pestaña del navegador que muestra la pestaña **Multi-Factor Authentication | Getting started**, dentro de _Security_, en el portal de Azure.
- En el sección de **Settings**, selecciona _Fraud Alert_.
- Dentro de **Fraud Alert**, configura estos datos (si no están por defecto):

  |Setting|Value|
  | --  | -- |
  |Allow users to submit fraud alerts|	On|
  |Automatically block users who report fraud	|On|
  |Code to report fraud during initial greeting	|0|

- Clic en _Save_.
> Nota: en este punto, ya has habilitado MFA para el usuario aaduser1 y has configurado las alertas de fraude.
- Vuelve a la pestaña del nuevo tenant _AdatumLab500-04_ en Azure AD, y en la sección de _Manage_, clic en properties.
- Abajo verás un enlace que dice *Manage Security defaults*, el cual clicaremos.
- Se desplegará una pestaña lateral, en la cual pondremos el valor **Enable Security defaults** a _NO_, y justo debajo aparecerán varias opciones, donde marcaremos **My Organization is using Conditonal Access** como razón, y clic en _Save_.
  > Nota: Asegúrate de estar logueado en el tenant _AdatumLab500-04_ de Azure AD. Puedes utilizar el filtro de **Directory + subscription** para cambiar entre tenants del Azure AD. Asegúrate también de estar logueado con un usuario que pertenezca al role de _Global Administrator_ dentro de Azure AD.

##### Tarea 6: Validar la configuración de MFA.

En esta tarea vamos a validar la configuración de MFA testeando el registro con el usuario aaduser1.

- Abre una ventana InPrivate del navegador.
- Abre el portal de Azure y loguéate con el usuario aaduser1.
  > Nota: Para loguearte vas a necesitar proveer el denominado _fully qualified name_ de la cuenta de usuario aaduser1, incluyendo el nombre de dominio de Azure AD tenant DNS, que hemos almacenado previamente en este lab. Este usuario será de la forma aaduser1@<nombre_tenant>.onmicrosoft.com, donde _<nombre_tenant>_ es donde viene el nombre único
- Cuando nos salga la nueva ventana diciendo **More information required** clic en _Next_.
  > Nota: la sesión del navegador se redirigirá a la página de _Additional security verification_.
![ImagenMFALogin](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_MFALogin.png)

- En la página **Keep your account secure**, selecciona el link de abajo que dice **I want to set up a different method**. Nos saldrá otra ventana encima con varias opciones, de las cuales seleccionaremos la opción **Phone**, y le damos a _Confirm_.
- De vuelta en la página incial, selecciona el país or región, escribe tu número de teléfono en el campo de _Enter phone number_, asegúrate de que la opción de _Text me a code_ está seleccionada (por defecto lo está) y dale a _Next_.
- Introduce el código que te ha llegado al teléfono indicado y clic en _Next_.

![ImagenMFASmsOk](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_MFASmSOk.png)

- Cuando la validación sea positiva, clic otra vez en _Next_.
- Ahora nos pedirá descargarnos la aplicación de Microsoft para autenticarnos, pero le daremos de nuevo al enlace **I want to use a different method**, seleccionaremos el _Email_ de la lista desplegable y clic en _Confirm_.

![ImagenMFAApp](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part1_MFAApp.png)

- Escribimos la dirección de correo electrónico que queramos usar y le damos a _Next_. Cuando recibas el email con el código, lo pegamos en la página y le damos a _Done_.
- Cuando nos salga la nueva ventana, cambiaremos la contraseña. Asegúrate de guardarla.
- Cierra sesión con el usuario aaduser1 y cierra la ventana privada del navegador.
  > Resultado: Hemos creado un nuevo tenant en Azure AD, configurado usuarios, configurado MFA y testeado la experiencia de MFA para dicho usuario.

#### Ejercicio 3 - Implementar las políticas de acceso condicional en Azure.

##### Tarea 1: Configurar la política de acceso condicional.

En esta tarea, revisaremos la configuración de las políticas de acceso condicional, y crearemos una política que requiere MFA cuando nos logueamos en el portal de Azure.

- En el portal de Azure, navega a la pestaña de Azure AD del tenant **AdatumLab500-04**.
- En la sección de _Manage_ clic en _Security_.
- Dentro de _Security_, en la sección de _Protection_, clic en **Conditional access**.
- Ahora le damos a **+ New policy**.
- En la nueva pestaña, configuramos los siguientes valores.
  - En el nombre: **AZ500Policy1**
  - Clic **Users and groups**, incluye dentro de _Select users and groups_ el checkbox de _Users and groups_. Como nos pedirá un usuario, seleccionamos al usuario aaduser2.
  
![ImagenMFAApp](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part3_NewPolicy.png)
  
  - Clic ahora en **Cloud apps or actions**, clic en _Select apps_, y en la pestaña que sale a la derecha escribe _Microsoft Azure Management_ y luego dale a _Select_.
    > Nota: Revisa el warning que dice que esta política impacta el acceso al portal de Azure Portal.

![ImagenMFAApp](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part3_NewPolicy_Apps.png)
  
  - Clic en **Conditions**, clic _Sing-in risk_ y en la pestaña que sale, revisa los niveles de riesgo pero no hagas ningún cambio y cierra esa pestaña.
  - Clic en **Device platforms**, revisa las diferentes plataformas que se pueden incluir (Android, iOS, etc) y haz clic en _Done_.
  - Clic en **Locations** y revisa las diferentes locations que se muestran sin hacer cambios.
  - Clic en **Grant** en la sección de _Access control_, y en la pestaña que se abre, selecciona el checkbox de **Require multi-factor authentication** y luego clic en _Select_.
  - Ahora cambia la opción **Enable policy** a _On_ y luego a _Create_.
    > Nota: en este punto, ya tenemos una política de acceso condicional, que requiere MFA para loguearte en el portal de Azure.

##### Tarea 2: probar la política de acceso condicional.

En esta tarea, vamos a loguearnos en el portal de Azure con el usuario aaduser2, donde la verificación MFA será obligatoria. También eliminaremos la política creada en el paso anterior antes de continuar con el siguiente ejercicio.

- Abre una ventana del nagegador InPrivate.
- Ve al portal de Azure y logueate con el usuario aaduser2.
- Una vez introducidos usuario y contraseña, nos saldrá la ventana de **More information required**, donde le daremos a _Next_.
  > Nota: El navegador nos redirigirá a la página de **Keep your account secure**.
- Pincha el enlace **I want to set up a different method**, y en la ventana que aparece de **Which method would you like to use?** elige _Phone_ y dale a _Confirm_.
- De vuelta a la página principal, elige la región y el número de teléfono y dale a _Next_. Asegúrate que la opción **Text me a code** está seleccionada.
- Ahora introduce el código que has recibido en el teléfono y dale a _Next_.

![ImagenMFAUser2Sms](../../Recursos/2%20-%20Seguridad%20en%20el%20cloud/Lab3_Seguridad_en_cloud-MFA/lab3_module2_part3_NewPolicy_SmsVerified2.png)

- Asegúrate que todo sale correctamente y que ves una mensaje de **Success!!** como este. Clic en _Done_.
- Cuando te redirija, cambia la contraseña y guárdala. Asegúrate de que puedes acceder al portal de Azure.
- Cierra sesión con adduser2 y cierra la ventana InPrivate del navegador.
  > Nota: Acabas de verificar que la nueva política de acceso condicional te obliga a usar MFA cuando el usuario aaduser2 intenta loguearse en el portal de Azure.
- Navega de nuevo al portal de Azure, a la pestaña de Azure AD del tenant **AdatumLab500-04**.
- En la sección de _Manage_ selecciona _Security_.
- Dentro de _Security, en la sección **Protection** clic en _Conditional access_.
- Dentro de _Conditional access_, pincha en la elípsis a la derecha de la política existente y dale a **Delete**. Cuando te pregunte, elije _Yes_.
> Note: Resultado: En este ejercicio, hemos implementado una política de acceso condicional, que fuerza el uso de MFA cuando un usuario quiere loguearse en el portal de Azure.
> Note: Resultado: Ya hemos configurado y probado el acceso condicional de Azure AD.

#### Ejercicio 4 - Implementar Azure AD Identity Protection.

##### Tarea 1: Habilitar Azure AD Identity Protection.

En esta tarea, veremos las distintas opciones dentro de Azure AD Identity Protection dentro del portal de Azure.
- Si es necesario, loguéate en el portal de Azure.
  > Nota: Asegúrate de estas logueado en el tenant de Azure AD **AdatumLab500-04**. Puedes utilizar el friltro de **directory + subscription** para cambiar entre tenants. Asegúrate de estar logueado con un rol de _Global Administrator_.
- En la pestaña de **AdatumLab500-04**, en la sección de _Manage_, nos vamos a _Security_.
- Dentro de _Security_, en la sección de **Protect**, nos vamos a _Identity protection_.
- Aquí dentro, revisa las opciones de **Protect**, **Report**, y **Notify**.

##### Tarea 2: Configurar una política de riesgo de usuario.

- Desde la propia ventana de **Identity protection**, en la sección _Protect_, nos vamos a _user risk policy_.
- Configura el **User risk remediation policy** con los siguiente valores:
  - Clic en _users_ --> All users 
     - Dentro de la pestaña _Include_, asegúrate de tener seleccionada la opción **All users**.
     - Dentro de la pestaña _Exclude_, selecciona tu propio usuario y clic select.
  - Clic en la parte de _User risk_. Selecciona **Low and above** y después clic _Done_.
  - Clic en _Access_, y en la pestaña que sale, asegúrate que las opciones **Allow access** y **Require password change** están marcadas y dale a _Done_.
  - Cambia **Enforce policy** a _On_ y dale a _Save_.

##### Tarea 3: Configurar una política de riesgo de sign-in.

- Dentro de **Identity protection**, en la sección de _Protect_ nos vamos a _Sign-in risk policy_.
- Configura _Sign-in risk remediation policy_ con estos valores:
  - Clic _Users_ y en la pestaña de _Include_, asegúrate que la opción _All users_ está seleccionada.
  - Clic en _Sign-in risk_, y en la pestaña que aparece selecciona **Medium and above**, luego dale a _Done_.
  - Clic en _Access_, asegúrate que las opciones **Allow access** y **Require multi-factor authentication** están marcadas y dale a _Done_.
  - Cambia **Enforce policy** a _On_ y dale a _Save_.

##### Tarea 4: Simular eventos de riesgo contra las políticas de Azure AD Identity Protection .

> Nota: antes de comenzar con esta tarea, asegúrate que el despliegue que comenzaste en el primer ejercicio se ha completado. Este despliegue incluye una máquina virtual llamada **az500-04-vm1**.

- En el portal de Azure, en el filtro de **Directory + subscription**, selecciona el tenant asociado a la subscripción de Azure en la cual desplegamos la VM llamada **az500-04-vm1**.
- Desde el portal de Azure, en el campo de texto **Search resources, services, and docs** de arriba, escribe **Virtual machines** y dale a Enter.
- Dentro de la pestaña de _Virtual  machines_, selecciona la entrada relativa a la VM existente.
- Una vez en la pestaña de la VM, clic en **Connect** y en el desplegable selecciona _RDP_.
- Clic en **Download RDP File** y úsalo para conectarte a la VM **az500-04-vm1** via escritorio remoto. Cuando te pida la autenticación, provee los siguientes valores:

  |Setting|Value|
  |--|--|
  |User name	|Student|
  |Password	|Pa55w.rd1234|

  > Nota: espera a que cargue la sesión remota y el **Server Manager**.

  > Nota: Los siguientes pasos se ejecutarán en la sesión remota de la VM **az500-04-vm1**.

- En el **Server Manager**, clic en _Local Server_ y luego haz clic en **IE Enhanced Security Configuration**.
- Dentro de _Internet Explorer Enhanced Security Configuration_, pon las dos opciones a _Off_ y clic en _OK_.
- Abre Internet Explorer e inicia una sesión InPrivate.
- En la ventana InPrivate de Internet Explorer, navega al _ToR Browser Project_ en la url: https://www.torproject.org/projects/torbrowser.html.en
- Descarga e instala la version de Windows del navegador ToR con la configuración por defecto.
- Cuando termine la instalación, abre el navegador ToR, utiliza la opción **Connect** de la página inicial, y busca el **Application Access Panel** en https://myapps.microsoft.com.
- Cuando te salga la ventana, intenta loguearte con la cuenta de usuario de aaduser3.

  > Nota: Te saldrá el mensaje _Your sign-in was blocked_. Esto es lo que se espera, dado que esta cuenta no está configurada con autenticación multi-factor, la cual se requiere dado al incremento en el riesgo asociado con el uso del navegador ToR.

- Cierra sesión con aaduser3 y vuelve a abrir sesión con aaduser1, el cual si está previamente configurado con MFA.

  > Nota: Ahora mismo, te aparecerá el mensaje de **Suspicious activity detected**. De nuevo, esto es lo que esperamos, ya que esta cuenta sí está configurada con MFA. Considerando el incremento en el riesgo de usar el navegador ToR para loguearse, te verás obligado a usar MFA.

- Utiliza la opcion de verificación y especifica si quieres usar el método via mensaje de texto o una llamada.
- Completa la verificación y asegura que puedes loguearte satisfactoriamente en el **Application Access Panel**.
- Cierra tu sesión RDP.

  > Nota: En este momento, has intentado dos tipos de logueo diferente. A continuación, tendremos que revisar los informes generados en el Azure Identity Protection.

##### Tarea 5: Revisar los informes de Azure AD Identity Protection.

En esta tarea, revisaremos los informes de Azure AD Identity Protection generados desde logueos en el navegador ToR.

- De vuelta en el portal de Azure, utiliza el filtro **Directory + subscription** para cambiar al tenant **AdatumLab500-04** dentro del Azure AD.
- Dentro del tenant, en la sección de _Manage_ vamos a _Security_.
- Dentro de _Security_, en la sección **Reports**, clic en _Risky users_.
- Revisa el informe e identifica cualquier entrada referenciando al usuario aaduser3.
- En la ventana de _Security_, en la sección **Reports**, clic en _Risky sing-ins_.
- Revisa el informe e identifica cualquier entrada referenciando al usuario aaduser3.
- Dentro de _Reports_, clic **Risk detections**.
- Revisa el informe e identifica cualquier entrada referenciando logueos desde una IP anónima generada por el navegador ToR.

  > Nota: Puede demorarse entre 10-15 minutos la aparición de los riesgos en los informes.
 
  > Result: Hemos habilitado Azure AD Identity Protection, configurado _user risk policy_ y _sing-in risk policy_, así como lo hemos validado simulando eventos de riesgo.

#### Clean up resources

> Necesitamos eliminar todos los recursos de identity protection que ya no usaremos.

Utiliza los siguientes pasos para deshabilitar las políticas de identity protection en el nuevo tenant de Azure AD **AdatumLab500-04**.

- En el portal de Azure, navega a la ventana de dicho tenant.
- Una vez dentro, en la sección de _Manage_, ve a _Security.
- Dentro de _Security_, en la sección de _Protect_, ve a _Identity Protection_.
- Clic en _User risk policy_ y una vez dentro, marca la opcion **Enforce policy** a _Off_ y dale a _Save_.
- Clic en _ Sign-in risk policy_ y una vez dentro, marca la opcion **Enforce policy** a _Off_ y dale a _Save_.

Utiliza los siguientes pasos para detener la VM de Azure que provisionamos con anterioridad en el lab.
- En el portal de Azure, en el filtro de **Directory + subscription**, selecciona el tenant asociado a la subscripción de Azure en la cual desplegamos la VM llamada **az500-04-vm1**.
- Desde el portal de Azure, en el campo de texto **Search resources, services, and docs** de arriba, escribe **Virtual machines** y dale a Enter.
- Dentro de la pestaña de _Virtual  machines_, selecciona la entrada relativa a la VM existente.
- Presiona el botón de stop y cuando te pida confirmación le damos a _OK_.
