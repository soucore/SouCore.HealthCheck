# HealthCheck.MongoDb

HealthCheck MongoDb é uma extensão para Via.HealthCheck que verifica a dependência de mongo nos projetos Workers.
Sua implementação é simples e de fácil intergração com Via.HealthCheck. </br>

Há duas forma de configurar HealthCheck.MongoDb:

Fazer com que o a instância de conexão a ser verificada, seja a injetada no provider. Dessa forma, a instância 
verificada pelo HealthCheck, é a mesma da aplicação.

```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probes.AddMongoHealthCheck<IMongoClient>();
    });
```

A interface "IMongoClient" é derivada do drive MongoDb. Sabendo disso, o HealthCheck investiga no provider se já há uma 
instância que implemente IMongoClient.
Você pode cria uma classe customizada que herde MongoClient, caso queira mais de uma conexão injetada
no container de dependência e passar esse tipo para que o mongo busque no provider também.

```cs
    public class MongoNewConnection : MongoClient
    {
        ...
    }
```

```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probes.AddMongoHealthCheck<IMongoClient>();
        probes.AddMongoHealthCheck<MongoNewConnection>();
    });
```
