# HealthCheck.Kafka

HealthCheck Kafka é uma extensão para Via.HealthCheck que verifica a dependência de kafka nos projetos Workers.
Sua implementação é simples e de fácil intergração com Via.HealthCheck. </br>

## 📦 Configuração
Há três forma de usar e configurar HealthCheck.Kafka:

### **2 - Monitorando log da instância conectada**
A forma mais simples de verificar a mesma conexão que sua lógica de negócio está usando, é interceptando o build de
ConsumerBuilder e atribuindo ao HealthCheck, o resultado de *Facility** em SetLogHandler

```cs
    using var consumer = new ConsumerBuilder<Ignore, string>(config)
        .SetLogHandler((_, log) => KafkaHealthCheckLogMonitor.InvokeFacilityLog(log.Facility))
        .Build();
```

*KafkaHealthCheckLogMonitor* é uma uma classe estática que provê uma forma simples de invokar um evento que vai passar ao
healthcheck o resultado de Facility.
Para configurar essa possibilidade, é necessário configurar o healthCheck para entender esse comportamento.


```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probe.AddKafkaHealthCheckLogMonitor("alias");
    });
```
No cenário de configuração acima, é importante que nas configurações de ConsumerConfig e ProducerConfig seja informado "protocol" á propriedade *Debug*. Isso vai ativar o log por parte da biblioteca do kafka. 
Com SetLogHandler configurado no ato de instanciar um ConsumerBuilder ou um ProducerBuilder, automaticamente Via.HealthCheck começa a fazer o monitoramento do log *Facility*. Ele é quem vai dizer como está a conexão da real instância em trabalho com o kafka. 


### **2 - Usando um IConsumer<,> injetado (DI)**

Caso você tenha injetado IConsumer no provider, vc pode informar para o HealthCheck para monitore a intancia Injetada.
Nesse caso, basta implemntar AddKafkaHealthCheckConsumer, seguindo dos tipos da chave e valor conforme IConsumer<,> estabelece.

```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probe.AddKafkaHealthCheckConsumer<Ignore, string>("alias");
    });
```


### **3 - Produzindo mensagem em um tópico**

HealthCheck Kafka também provê um forma usando configurações para conectar. No cenário abaixo, ele trabalha como um producer, sempre verificando o status da publicação de uma mensagem no tópico.

```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probe.AddKafkaHealthCheck("alias", config =>
        {
            config.BootstrapServer = "host.docker.internal:9092";
            config.Topic = "your-test";
        });
    });
```
</br>

## 🏗 WIP

***Há mais estudos que estamos analisando, inclusive com ProducerBuilder injetado no provider, o que pode ser uma
possibilidade.***