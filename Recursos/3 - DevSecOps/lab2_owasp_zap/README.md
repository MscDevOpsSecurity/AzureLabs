# Guia Práctica.

## 1 - Crear un proyecto de Azure DevOps

## 2 - Crear un repositorio 

## 3 - Importar archivo adjunto zip

## 4 - Añadir extension a AzureDevOps: OWASP ZAP
https://marketplace.visualstudio.com/items?itemName=CSE-DevOps.zap-scanner&targetId=318e8bcf-5e58-4f0c-a09c-14dfa5f9c1ce&utm_source=vstsproduct&utm_medium=ExtHubManageList

## 5 - Añadir Subscripcion de Azure a Azure DevOPs

1.- Entrar en ... Azure DevOps -> Proyect Settings -> Pipelines -> Server Connection --> Añadir Subscripcion Azure

2.- Create service connection -> Azure Resource Manager -> Name: myARMConnection

<img width="1136" alt="DevOps Project Settings" src="https://github.com/MscDevOpsSecurity/AzureLabs/assets/1581524/40577955-027e-4800-9cfd-8e3db44b4105">
<img width="1069" alt="Pipelines - Server Connections" src="https://github.com/MscDevOpsSecurity/AzureLabs/assets/1581524/0197d5c2-517a-4a62-b936-7620307383b4">
<img width="1069" alt="Server connections details" src="https://github.com/MscDevOpsSecurity/AzureLabs/assets/1581524/6b94220c-f46f-4adb-b606-c4a7abc72754">





## 6 - Importar pipeline, cambiar la siguiente linea para que tome la conexión creada en el paso 5
```
  azureSubscriptionValue: 'myARMConnection'
```

# Errores conocidos

## 1 - Al ejecutar la pipeline, puede devolver el siguiente error.

```
##[error]No hosted parallelism has been purchased or granted. To request a free parallelism grant, please fill out the following form https://aka.ms/azpipelines-parallelism-request
```
Pedir acceso gratuito mediante el siguiente formulario: https://aka.ms/azpipelines-parallelism-request

## 2 - Las ultimas versiones del OWASP ZAP Scanner han introducido un error en la generación del reporte. Como consecuencia, las análisis  se ejecutan correctamente como visualizado en la tarea de ***Scan***  

   ![](./docs/DAST_analysis.png)

Pero, el reporte que se genera resulta vacío y de consecuencia los test-results.xml no se ven reflejado en la pipeline. 

![](./docs/zap_report.png)
