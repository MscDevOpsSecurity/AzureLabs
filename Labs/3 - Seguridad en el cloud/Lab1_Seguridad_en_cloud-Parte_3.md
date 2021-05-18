| Lab |  1 |
| --  | -- |

| Parte | Título | 
| --  | -- |
| 3 | Desplegar Azure Container Instances mediante el uso del patrón sidecar |

### Lab overview

Como habrá notado en la parte anterior, el tráfico a la API de la aplicación se ejecuta sin cifrar. Sus clientes no están muy conformes con esto y exigen que utilice HTTPS en lugar de HTTP. 

Podemos mejorar el código de la aplicación para que admita HTTPS, pero no tenemos acceso al equipo de desarrollo original de la aplicación. Una alternativa es utilizar un contenedor sidecar que mejore la aplicación con la funcionalidad requerida. 

En esta parte, utilizaremos **NGINX**, un servidor web que se puede utilizar como proxy inverso frente a una aplicación web, para proporcionar la funcionalidad de descarga SSL. El tráfico cifrado accederá al grupo de contenedores ACI a través del contenedor NGINX, y NGINX redirigirá el tráfico descifrado al contenedor de la aplicación real.

![Imagen topologia parte 3](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part3.png)

El patrón Sidecar es un concepto poderoso en arquitecturas basadas en contenedores. Te permite descomponer la funcionalidad de la aplicación en diferentes imágenes de contenedores que se ejecutarán juntas en el mismo grupo de contenedores.

Los contenedores del mismo grupo de contenedores compartirán algunas propiedades, como el network stack subyacente. ACI en realidad representa un grupo de contenedores, en el que cada contenedor puede asumir parte de la funcionalidad requerida por la aplicación. En este ejemplo, implementaremos un grupo de contenedores donde uno de los contenedores realizará el cifrado y descifrado SSL, y el otro proporcionará la funcionalidad de la aplicación real (la misma API implementada en las partes anteriores).

> Nota: el concepto de "_container groups_" es equivalente a los _pods_ en Kubernetes.

### Objetivos

El objetivo de esta parte es utilizar una estructura ya creada (ACI), e introducir seguridad en las peticiones web, de manera que todo el tráfico se redireccione a través de un único puerto (en este caso será el 443) y solo se decodifique el contenido dentro del propio contenedor.

### Conceptos

- SSL ([Secure Socket Layer](https://www.websecurity.digicert.com/security-topics/what-is-ssl-tls-https))
- Pods: son la unidad de computación desplegable más pequeña de Kubernetes. Lee más [aquí](https://kubernetes.io/docs/concepts/workloads/pods/).
- nginx: Servidor web y proxy. Lee más [aquí](https://www.nginx.com/resources/wiki/).
- Certificado self-signed: es un certificado firmado por nosotros mismos, que solo tiene validez para entornos de desarrollo. No está validado para entornos de producción.

### Duración

35-50 minutos

### Instrucciones

#### Crear la configuración para NGINX

1 - Lo primero que vamos a necesitar es generar un certificado digital para la enciptación de datos. En esta parte usaremos un certificado self-signed, pero en producción utilizaremos un certificado generado por una entidad autorizada de certificación.

```bash
# Crear self-signed certificado
openssl req -new -newkey rsa:2048 -nodes -keyout ssl.key -out ssl.csr -subj "/C=US/ST=WA/L=Redmond/O=AppDev/OU=IT/CN=contoso.com"
openssl x509 -req -days 365 -in ssl.csr -signkey ssl.key -out ssl.crt
```
2 - Ahora crearemos un archivo que contendrá la configuración de nginx. No es necesario entender todo lo que contiene, solo algunos puntos son importantes:
- La propiedad **proxy_pass** apunta a _http://127.0.0.1:8080_. Esto es donde estará el contenedor que tiene la API. Los contenedores dentro de un mismo grupo de contenedores, comparten la misma espacio de red. En otras palabras, se pueden comunicar entre ellos utilizando la dirección _localhost_ o _127.0.0.1_.
- Los archivos del certificado (file, key) se leerán desde el directorio **/etc/nginx/** (del contenedor NGINX, no de tu local), así que tendremos que dejar dichos archivos en esa carpeta.
- El contenedor sidecar escuchará en el puerto TCP 443.

```bash
# Crear nginx.conf para SSL
nginx_config_file=/tmp/nginx.conf
cat <<EOF > $nginx_config_file
user nginx;
worker_processes auto;
events {
  worker_connections 1024;
}
pid        /var/run/nginx.pid;
http {
    server {
        listen [::]:443 ssl;
        listen 443 ssl;
        server_name localhost;
        ssl_protocols              TLSv1.2;
        ssl_ciphers                ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-AES256-GCM-SHA384:DHE-RSA-AES128-GCM-SHA256:DHE-DSS-AES128-GCM-SHA256:kEDH+AESGCM:ECDHE-RSA-AES128-SHA256:ECDHE-ECDSA-AES128-SHA256:ECDHE-RSA-AES128-SHA:ECDHE-ECDSA-AES128-SHA:ECDHE-RSA-AES256-SHA384:ECDHE-ECDSA-AES256-SHA384:ECDHE-RSA-AES256-SHA:ECDHE-ECDSA-AES256-SHA:DHE-RSA-AES128-SHA256:DHE-RSA-AES128-SHA:DHE-DSS-AES128-SHA256:DHE-RSA-AES256-SHA256:DHE-DSS-AES256-SHA:DHE-RSA-AES256-SHA:AES128-GCM-SHA256:AES256-GCM-SHA384:ECDHE-RSA-RC4-SHA:ECDHE-ECDSA-RC4-SHA:AES128:AES256:RC4-SHA:HIGH:!aNULL:!eNULL:!EXPORT:!DES:!3DES:!MD5:!PSK;
        ssl_prefer_server_ciphers  on;
        ssl_session_cache    shared:SSL:10m; # a 1mb cache can hold about 4000 sessions, so we can hold 40000 sessions
        ssl_session_timeout  24h;
        keepalive_timeout 75; # up from 75 secs default
        add_header Strict-Transport-Security 'max-age=31536000; includeSubDomains';
        ssl_certificate      /etc/nginx/ssl.crt;
        ssl_certificate_key  /etc/nginx/ssl.key;
        location / {
            proxy_pass http://127.0.0.1:8080 ;
            proxy_set_header Connection "";
            proxy_set_header Host \$host;
            proxy_set_header X-Real-IP \$remote_addr;
            proxy_set_header X-Forwarded-For \$remote_addr;
            proxy_buffer_size          128k;
            proxy_buffers              4 256k;
            proxy_busy_buffers_size    256k;
        }
    }
}
EOF
```

3 - Ahora vamos a almacenar los archivos que pasaremos al contenedor sidecar con NGINX en variables base64-encoded:

```bash
# Codificar en Base64
nginx_conf=$(cat $nginx_config_file | base64)
ssl_crt=$(cat ssl.crt | base64)
ssl_key=$(cat ssl.key | base64)
```

#### Desplegar el container group con el sidecar NGINX

1 - Utilizaremos YAML para implementar ACI. En la unidad anterior, usamos la interfaz de línea de comandos de Azure, pero las configuraciones más complejas, como los sidecars, requieren el uso de YAML. Otro beneficio de las configuraciones basadas en YAML es que podemos almacenarlas en sistemas de control de versiones y tratarlas igual que el código de su aplicación. 

Si observamos el YAML generado en la parte anterior, podemos ver que se hace referencia a un "Perfil de red" (Network profile). Puedes copiar el ID de perfil del YAML generado en la unidad anterior o buscarlo usando el comando _az network profile list_:

```bash
# Obtener network profile ID (fue creado  la primera vez que desplegamos un ACI en la Red Privada Virtual)
nw_profile_id=$(az network profile list -g $rg --query '[0].id' -o tsv) && echo $nw_profile_id
```

2 - Ahora tenemos toda la información necesaria para crear el archivo YAML. Puedes combinar toda la información que tengas almacenada en variables en esta unidad en un solo archivo. Aquí hay algunas cosas que debe tener en cuenta:

- La contraseña de SQL se transmite como variable de entorno segura, por lo que no se expone después de crear ACI
- El contenedor NGINX monta el volumen de configuración en el directorio **/etc/nginx/**, donde se espera que se encuentren los certificados, como vimos anteriormente en esta unidad. Los contenidos del volumen se especifican como secretos, es por eso que antes teníamos que codificar en base64 las variables.
- El contenedor NGINX expone el puerto 443 y el contenedor de la aplicación expone el puerto 8080. Sin embargo, el grupo de contenedores solo expone el puerto 443. Esto hace que la aplicación solo sea accesible a través del contenedor sidecar NGINX.
- Utilizaremos YAML para especificar estas propiedades para la instancia de contenedor de Azure:

```bash
# Create YAML
aci_yaml_file=/tmp/aci_ssl.yaml
cat <<EOF > $aci_yaml_file
apiVersion: 2019-12-01
location: westus
name: $aci_name
properties:
  networkProfile:
    id: $nw_profile_id
  containers:
  - name: nginx
    properties:
      image: nginx
      ports:
      - port: 443
        protocol: TCP
      resources:
        requests:
          cpu: 1.0
          memoryInGB: 1.5
      volumeMounts:
      - name: nginx-config
        mountPath: /etc/nginx
  - name: sqlapi
    properties:
      image: erjosito/sqlapi:1.0
      environmentVariables:
      - name: SQL_SERVER_USERNAME
        value: $sql_username
      - name: SQL_SERVER_PASSWORD
        secureValue: $sql_password
      - name: SQL_SERVER_FQDN
        value: $sql_server_fqdn
      ports:
      - port: 8080
        protocol: TCP
      resources:
        requests:
          cpu: 1.0
          memoryInGB: 1
  volumes:
  - secret:
      ssl.crt: "$ssl_crt"
      ssl.key: "$ssl_key"
      nginx.conf: "$nginx_conf"
    name: nginx-config
  ipAddress:
    ports:
    - port: 443
      protocol: TCP
    type: Private
  osType: Linux
tags: null
type: Microsoft.ContainerInstance/containerGroups
EOF
```

3 - Después de poner toda la configuración requerida en el archivo YAML, el comando para crear ACI es simple:

```bash
# Desplegar el ACI
az container create -g $rg --file $aci_yaml_file
```

> Nota: si no lo borraste primero en el paso anterior, ejecuta el comando: **az container delete -n $aci_name -g $rg -y**

![Close look at sidecar container](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part3_Azure_sidecar_pattern_closeLookAt.png)


Si nos vamos a la UI del portal de Azure, podremos ver cómo se ha creado esta configuración dentro cada contenedor. Para ello nos vamos al _resouce group_ que hemos creado para este lab, y accedemos al recurso llamado **learnaci**. Una vez dentro, en la parte izquierda, pinchamos en el submenú "_Settings/Containers_".

| NGINX container|
|--|
|![nginx container](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part3_NGINXView.png)|

| API container|
|--|
|![sqlapi container](../../Recursos/3%20-%20Seguridad%20en%20el%20cloud/lab1_module2_part3_SQLAPIView.png)|

4 - Ahora ya podemos extraer la dirección IP del ACI (debería ser una IP privada) y acceder via HTTPS desde la VM. Deberías utilizar el flag _-k_ con curl para deshabilitar la validación del certificado, ya que tenemos uno self-signed. El endpoint `/api/healthecheck` de la API debería responder **OK**.

```bash
# Test
aci_ip=$(az container show -n $aci_name -g $rg --query 'ipAddress.ip' -o tsv) && echo $aci_ip
ssh -n -o BatchMode=yes -o StrictHostKeyChecking=no $vm_pip "curl -ks https://$aci_ip/api/healthcheck"
```

> Nota: No elimines el ACI, porque lo vamos a necesitar en la siguiente parte.
