
| Lab |  5  |
| --  | -- |

| Módulo | Título | 
| --  | -- |
| Manejo de secretos, tokens y certificados | Políticas de seguridad en Azure Key Vault |

## Lab overview

## Objetivos
El objetivo de esta práctica es 

## Duración
 min aprox.

## Instrucciones

### Antes de comenzar

Para la preparación de este lab, os vamos a proveer de todo lo necesario para que podáis levantar la infraestructura en Azure de forma automática, mediante la ejecución de ARM templates. 


### Tarea 1 : Vamos a cargar los ARM templates en vuestra cuenta de Azure.


### Tarea 7: Eliminar todos los recursos creados.

Al final de cada ejercicio es importante dejar nuestra cuenta de Azure limpia para evitar sobrecostes nos esperados por parte de Microsoft.
Para eliminar todos los recursos del ejercicio, vamos a hacer lo siguiente:

1 - En el portal de Azure, abrimos sesión de **Bash** dentro del panel de Cloud Shell.
2 - Listamos todos los resource groups creados a lo largo del lab de este módulo, ejecutando el siguiente comando:
```bash
az group list --query "[?starts_with(name,'AzureLabsModulo4Lab1')].name" --output tsv
```
3 - A continuación eliminamos todos los resource groups creados en el lab, ejecutando el siguiente comando:
```bash
az group list --query "[?starts_with(name,'AzureLabsModulo4Lab1')].[name]" --output tsv | xargs -L1 bash -c 'az group delete --name $0 --no-wait --yes'
```