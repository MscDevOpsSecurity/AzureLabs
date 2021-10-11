
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

### Ejercicios relacionados
N/A
