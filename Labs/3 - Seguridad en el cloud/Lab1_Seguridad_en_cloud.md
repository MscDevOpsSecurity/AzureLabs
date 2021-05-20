
| Lab |  1 |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Seguridad en el cloud | Asegura tus aplicaciones mediante el uso del patrón "sidecar" en Azure Container Instances |

### Lab overview
Con Azure Container Instances (ACI), podemos lanzar contenedores individuales en Azure y utilizar funcionalidades avanzadas para cumplir requerimientos de seguridad y redes de forma sofisticada.

Vamos a suponer que trabajamos para un proveedor de red, y nosotros somos los responsables de crear una API para nuestros clientes. Estas APIs son un servicio premium por el cual cobramos a nuestros clientes, por lo que ellos tienen que ser capaces de arrancar y parar dichas APIs bajo demanda. Una vez el cliente lanza la API, ellos son los responsables de su ciclo de vida. 

Un requerimiento crítico será que ninguna de estas API esté expuesta públicamente a Internet, y que solo dicho cliente tiene el poder de acceder a ella.

![Final picture](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/Mod2_Lab1_MainPicture.png)

### Objetivos
En este lab aprenderemos algunas herramientas avanzadas con las cuales podremos crear Azure Container Instances.

Al final del lab, seremos capaces de:
- Desplegar ACI en una Azure Virtual Network.
- Configurar un ACI utilizando yaml.
- Insertar un contenedor sidecar en la definición yaml.
- Integrar con un servicio de Azure PaaS utilizando "Private links"

### Acrónimos
- ACI: [Azure Container Instances](https://sec.ch9.ms/ch9/6409/039f527a-81ea-4b04-8081-0f4e8ec76409/usingazurecontainerinstances.mp4) 
- AVN: [Azure Virtual Network](https://docs.microsoft.com/en-us/azure/virtual-network/virtual-networks-overview)
- YAML: [Yet Another Markup Language](https://en.wikipedia.org/wiki/YAML)
- PaaS: [Platform as a Service](https://en.wikipedia.org/wiki/Platform_as_a_service) (en nuestro caso será una base de datos SQL Server).
- [Sidecar Pattern](https://medium.com/nerd-for-tech/microservice-design-pattern-sidecar-sidekick-pattern-dbcea9bed783)

### Duración
TBD

### Instrucciones
#### [1 - Desplegar Azure Container Instances en una red virtual](Lab1_Seguridad_en_cloud-Parte_1.md)
#### [2 - Entender ACIs mediante definiciones en YAML](Lab1_Seguridad_en_cloud-Parte_2.md)
#### [3 - Desplegar ACIs mediante el uso del patrón sidecar](Lab1_Seguridad_en_cloud-Parte_3.md)
#### [4 - Acceder a servicios habilitados mediante el uso de private links desde ACI.](Lab1_Seguridad_en_cloud-Parte_4.md)
#### [5 - (Extra) Desplegar ACIs usando el contenedor de inicialización.](Lab1_Seguridad_en_cloud-Parte_5.md)

### Ejercicio para casa

#### [Test final de práctica](Lab1_Seguridad_en_cloud-Test.md)
