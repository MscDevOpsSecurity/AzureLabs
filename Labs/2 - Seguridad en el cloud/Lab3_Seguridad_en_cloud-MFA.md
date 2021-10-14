
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
TBD
1 => 10 minutos
2 =>  minutos
3 =>  minutos
4 =>  minutos

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
