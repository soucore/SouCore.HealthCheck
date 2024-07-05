# HealthCheck.AzureQueueStorage

HealthCheck AzureQueueStorage é uma extensão para Via.HealthCheck que verifica a disponibilidade e exitência da fila
no Azurequeue Storage.
Sua implementação é simples e de fácil intergração com Via.HealthCheck. </br>

## 📦 Configuração

É preciso passas as configurações para que seja possível a verificação.

```cs
    services.AddViaHealthCheck((services, probes) =>
    {     
        probes.AddAzureQueueStorageHealthCheck(config =>
        {
            config.ConnectionString = "DefaultEndpointsProtocol=http;AccountName=myAccount;AccountKey=myKey;";
            config.Queue = "queue";
        });
    });
```
## 🏗 WIP

***O processo de verificação de dependência injetada, ainda está em processo de desenvolvimento***
