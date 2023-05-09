# Guia Práctica. (to-do)

1.- Crear un proyecto de Azure DevOps

2.- Crear un repositorio 

3.- Importar archivo adjunto zip

4.- Añadir extension a AzureDevOps: OWASP ZAP

5.- Azure DevOps -> Proyect Settings -> Pipelines -> Server Connection --> Añadir Subscripcion Azure:
* Create service connection -> Azure Resource Manager -> Name: myARMConnection

6.- Importar pipeline, cambiar la siguiente linea para que tome la conexión creada en el paso 5
```
  azureSubscriptionValue: 'myARMConnection'
```

