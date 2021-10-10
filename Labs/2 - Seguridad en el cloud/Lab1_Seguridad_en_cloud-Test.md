| Lab |  1 |
| --  | -- |

| Parte | Título | 
| --  | -- |
| Test | Veamos si has adquirido los conocimientos básicos |

### Cuestionario rápido

1. ¿Qué feature de un **Azure Container Instance**  puedes utilizar para ejecutar tareas de inicialización requeridas por tu aplicación?

    > a) Otras Azure Container Instances ejecutándose en paralelo.
  
    > b) Contenedores de inicialización
  
    > c) Modificar el código de la aplicación para llevar a cabo la inicialización
  
2. ¿Cómo se comunican los contenedores sidecar con el conteneder que contiene la aplicación principal?

    > a) Los contenedores sidecar pueden comunicarse con otros contenedores en el mismo grupo de contenedores utiliando la dirección IP de destino 127.0.0.1

    > b) Los contenedores sidecar pueden utilizar el DNS para averiguar la dirección de otros contenedores en el grupo de contenedores.

    > c) No es posible, porque los contenedores que están en un grupo están aislados entre sí.
    
3. ¿Qué imagenes se pueden utiliar para los contenedores sidecar?

    > a) Los contenedores sidecar se pueden crear desde la misma imagen que la del contenedor que contiene la aplicación principal.

    > b) Los contenedores sidecar se pueden crear desde cualquier imagen, desde cualquier repositorio de imágenes.

    > c) Los contenedores sidecar se pueden crear desde cualquier imagen de contenedor, pero todos los contenedores dentro del grupo deben estar instanciados desde el mismo repositorio de imágenes.
    
4. ¿Qué dirección IP recivirá un ACI cuando se despliega en una Red Privada Virtual?

    > a) El ACI recibirá una dirección IP privada en la subnet donde se despliega, así como una dirección IP pública.

    > b) El ACI recibirá una dirección IP pública.

    > c) El ACI recibirá una dirección IP privada dentro de la subnet donde está desplegado.
    
5. ¿Desde dónde se puede acceder a un ACI desplegado en una Red Privada Virtual?

    > a) Únicamente desde dentro de la Red Virtual.

    > b) Dentro de la Red Virtual y por otras Redes Virtuales emparejadas.

    > c) Desde dentro de la Red Virtual, desde otras emparejadas y desde redes on-premises conectadas mediante VPN o ExpressRoute.


### Ejercicio práctico

![Test imagen](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_Test_DestroyPrivateEndpointAndDNS.png)
El ejercicio consiste en estos pasos:

- Eliminar desde el portal de Azure los elementos marcados con las _X_, de manera que el ACI no sea capaz de localizar la base de datos (lanzando los comandos curl desde la VM).
- Utilizando los comandos descritos en los ejercicios anteriores, debes ser capaz de reconstruir la estructura que conseguimos al final de la parte 3 del laboratorio.

> NOTA: Por supuesto, no vale destruir todo el grupo de recursos y volver a crearlo desde cero. Las estructuras que no están marcadas con la _X_ deben permanecer en Azure, y debes simplemente ejecutar los comandos que alteran las comunicaciones entre los recursos).

![Test objetivo](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part3.png)


:smiling_imp: ¡¡ Buena suerte !! :muscle:
