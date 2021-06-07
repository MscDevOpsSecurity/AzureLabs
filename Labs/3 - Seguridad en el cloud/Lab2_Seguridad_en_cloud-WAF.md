
| Lab |  2 |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Seguridad en el cloud | Protege tus aplicaciones mediante el uso de WAF y Azure Front Door |

### Lab overview
Una de las mejores razones para utilizar Azure con tus aplicaciones y servicios, es la de aprovecharse del amplio rango de herramientas de seguridad que incluye. Estas herramientas ayudan a hacer posible la creación de soluciones seguras en la plataforma de Azure.

![WAF Picture](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab2_module2_mainPicture.jpg)

### Objetivos
En este lab aprenderemos algunas herramientas de Azure con las cuales podremos asegurar nuestras aplicaciones web. Existen varias como Azure Application Gateway, Azure Front Door, Azure Web Application Firewall (WAF), etcétera. Nosotros nos vamos a centrar en estas 2 últimas.

### Acrónimos

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

![Web+SQL](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL.jpg)

- Le damos a _Create_ y en los campos que nos pide a continiación le damos los valores siguiente:

|Propiedad | Valor |
| --  | -- |
| App name | afdoor-app |
| Subcription | <La vuestra seguramente estará para elegir de una lista> |
| Resource Group | afdoor-rg |

![WebSQL_plan](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_plan.png)

- Ahora todavía necesitamos seleccionar un plan, que no es otra cosa que la forma en la que nos van a facturar el uso de dicha aplicación web. Pinchamos en dicha opción y se nos abrirá otra ventana.

  - Le damos a _Create new_.
  - En el nombre pondremos **afdoor-sp** y en location **West Europe**. 
  - Pricing tier lo dejamos como está por defecto.
  - Clic 'OK'.

![WebSQL_DB](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_db.png)

- Lo siguiente es crear la base de datos SQL Server. Lo primero que nos pide es el nombre de la base de datos, el cual será **afdoordatabase**. El siguiente campo es el servidor bajo el cual se alojará dicha base de datos, pero como nunca hemos creado ninguno, tendremos que hacerlo en el siguiente paso, pinchando en _Select server_.
  - En el nombre le ponemos **afdoorsqlserver** 
  - El usuario y contraseña a tú elección, pero recuérdalos.
  - Location será la misma que antes, por mantener una consistencia (aunque podría ser otra).
  - Clic 'Select'.

Hemos vuelto atrás, y solo nos queda elegir el _Pricing tier_ y el _collation_. Ambos los dejaremos por defecto y haremos clic sobre 'Select'. Con todo configurado, solo nos queda darle a 'Create' para que arranque el proceso de creación de todo lo que hemos seleccionado.

![WebSQL_Deploy](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_deploying.png)

Puede tardar varios minutos en levantarse toda la infraestructura, pero cuando todo esté listo, dentro del grupo de recursos que acabamos de crear, deberíamos tener todos los recursos que se muestran a continuación.

![WebSQL_DeploymentCompleto](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab2_module2_webappSQL_deployed.png)

#### 2 - Publicar el contenido web que necesitamos para el laboratorio.
#### 3 - Crear el WAF
#### 4 - Creamos el Azure Front Door asociado al WAF
#### 5 - Probamos la inyección SQL sobre la web
