## 🚑 SouCore.HealthCheck 

Esta biblioteca tem por objetivo ser uma solução padrão para implementação do pattern Health Probe para Workers no .NET.
Através dela você poderá utilizar implementações padrões de verificação da saúde de seus Workers de maneira simples e rápida, configurando apenas endpoints e suas dependências. Você poderá incluir nas suas verificações, dependências como MongoDB, Kafka, SMTP entre outras, além de poder criar suas próprias verificações de dependências customizadas. 

# O que é Health Check e como é o seu uso no kubernetes

A funcionalidade Health Checks, nada mais é do que uma possibilidade de testar de forma automatizada a saúde de uma aplicação através de chamadas a recursos por algum meio que disponibilize esses status, sendo um desses meios o mais comum, APIs. Através de um health check é possível verificar o conteúdo da chamada para checar se um determinado recurso está funcionando corretamente.
Kubernetes possui uma forma elegante para trabalhar com essa funcionalidade, de forma que, apenas alguns passos, já podemos automatizar esse processo. Kubernetes usa o pattern Health Probe e possui três formas de verificar a integridade de uma aplicativo:
Sobre pattern Probe, você pode ler mais em [kubernetes-patterns](https://www.oreilly.com/library/view/kubernetes-patterns/9781492050278/ch04.html)
### StartupProbe
    StartupProbe faz um teste de inicialização, isso faz com os testes de atividades readiness e liveness aguardem até que a conclusão do teste seja bem-sucedido. Somente depois desse resultado, é que os testes de atividade entram em ação. Esse é o momento ideal para verificar inclusive, as dependências do aplicativo.
### ReadinessProbe
    ReadinessProbe é executado durante todo o clico de vida do container após startupProbe for bem sucedido. Esse teste de atividade, vai estabelecer baseado no resultado do teste, se o container está disponível ou não na rede. O processo no container continua sendo executado, mas se ele está visível ou não, vai depender do resultado do teste de atividade ser bem-sucedido ou não. Quando trabalhamos com aplicações são consumidas por outros serviços, esse também, pode ser um bom local para testar as dependencias do aplicativo.
### LivenessProbe
    LivenessProbe é parecido com ReadinessProbe, contudo ele vai interferir na execução do processo do container. Esse teste de atividade juntamente com outras regras configuradas atreladas á ele, vai ficar responsável por dizer ao kubernet se o container deve ou não ser reiniciado. Essa é uma verificação importante, pois uma exception pode deixar uma aplicação zumbi, assim como um erro de acesso aos endpoints do aplicativo. 

## Como configurar Soucore.HealthCheck em seu projeto?

Soucore.HealthCheck possui duas formas distintas de verificação: pode verificar a saúde do seu projeto, 
onde você pode determinar em momentos estratégicos do seu código, se sua aplicação está saudável ou não, 
e verificar também as dependências da sua aplicação, onde este, verifica de forma automática as dependências
existentes. O Soucore.HealthCheck hoje, possui algumas customizações prontas de alguns conectores 
padrões de mercado, como MongoDB e Kafka.

### HealthCheck da aplicação

Você pode dizer para Soucore.HealthCheck quando sua aplicação está saudável ou não, definindo como "Healthy" ou "Unhealthy" em pontos estratégicos da sua lógica de negócio.
Exemplo:

Primeiro injetamos no construtor, IHealthCheck, que é um contrato de Soucore.HealthCheck
```cs
    public class Worker : BackgroundService
    {
        public Worker(IHealthCheck healthCheck, ...)
        {
            _healthCheck = healthCheck;
            ...
        }
```

Depois determinamos quando a aplicação está saudável ou não dessa forma, por exemplo:

```cs
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ...
                    await _healthCheck.Healthy();
                }
                catch (Exception ex)
                {
                    await _healthCheck.Unhealthy();
                    ...
                }
                await Task.Delay(100000, stoppingToken);
            }
        }
```

Em seguida adicionamos Soucore.HealthCheck na coleção de serviços (IServiceCollection) com as seguintes configurações. 

```cs
    services.AddViaHealthCheck((services, probes) =>
    { 
        probes.AddHealthCheck(8080, "/startup", true);
        probes.AddHealthCheck(8081, "/liveness");
    });
```

### HealthCheck de dependências

Aqui vamos focar em como usar um health check de dependência que já está pronto. Neste [ponto](##-Health-Checks-customizado), você pode aprender a como criar um health check customizado usando IHealthCheckCustom.
De forma geral, você pode usar "Dependency<>" para adicionar suas dependências de health check, contudo, o mais comum que veremos é uma extensão abstraindo essa verbosidade.
Segue exemplo:

```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probes.AddDependency<CustomHealthCheck<InjectableInstance>>("alias1");
        probes.AddDependency<CustomHealthCheck, CustomHealthCheckConfig>(config => {
            config.ConnectionString = "localhost"
        }, "alias2");
    });
```
*Repare que no exemplo acima, adicionamos "alias1" como parâmetro de AddDependency. Isso é útil para que você possa adicionar uma mesma dependência com configurações diferentes. Como por exemplo uma aplicação que consume uma mensagem de cluster kafka diferente do produtor.*

Acima, temos um health check customizado que está pronto para verificar uma nova conexão ou obter de forma transparente a
conexão de uma objeto já instanciado na injeção de dependência.


### Endpoints de consulta e verificação

Soucore.HealthCheck disponibiliza pontos de consulta via http de forma customizada, onde é possivel informar a porta e URI
para ser configurada.

```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probes.AddHealthCheck(8080, "/startup", true);
        probes.AddHealthCheck(8081, "/liveness");
    });
```

Nesse exemplo temos um endpoint startup e liveness configurados para Kubernetes. Lembrando que nesse caso, a verificação 
é somente do health check da aplicação. 

```cs
    probes.AddHealthCheck(8080, "/startup");
    probes.AddHealthCheck(8081, "/liveness");
```

Há 3 formas de configurarmos o terceiro parametro como exemplificado acima:
* Se não informarmos o 3° parêmetro ou informar *false*, o padrão é que, **não verifica** nenhuma dependência configurada.
* Se informarmos true, ele **verifica** todas as dependências configuradas.
* Se informarmos um array de strings, ele verificará somente as dependências configuradas com o alias informado.</br>
    *Vale lemebrar que essas strings precisam ser os alias das dependências configuradas*

Exemplo completo da configuração
```cs
    services.AddViaHealthCheck((services, probes) =>
    {
        probes.AddDependency<CustomHealthCheck<InjectableInstance>>("alias1");
        probes.AddDependency<CustomHealthCheck, CustomHealthCheckConfig>(config => {
            config.ConnectionString = "localhost"
        }, "alias2");

        probes.AddHealthCheck(8080, "/startup", true);
        probes.AddHealthCheck(8081, "/liveness", new string[]{"alias1"});
    });   
```

## Health Checks customizado

Soucore.HealthCheck disponibiliza uma interface simples para você criar seus próprios health checks. Para isto, basta implementar a interface *IHealthCheckCustom*, que fornece um punico método "ExecuteAsync" que tem como retorno um "Task<bool>" que internamente será chamado.

```cs
    public class CustomHealthCheck : IHealthCheckCustom
    {

        public CustomHealthCheck(ILogger<CustomHealthCheck> logger)
        {
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                ...
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "HealthCheck - CustomHealthCheck unhealthy");
                return false;
            }
        }
    }
```

## 🏗 WIP

***Está em desenvolvimento para Prometheus uma extensão para informar todas as dependências e seus respectivos
status. Essa extensão é usual para verificar mais detalhes das dependências e já está incluso em Soucore.HealthCheck
como experimento***

```cs
    probes.AddHealthCheckPrometheus(8082, "/dependencies");
```

## 📦 Pacotes

## Lista de HealthChecks de dependências

✅ Sim |
❌ Não


| Conector | Verifica instância Injetada? | Lançado? | Documentação |
| :------: | :--------------------------: | :------: | :----------: |
| MongoDb  | ✅                           | ✅       | [link](/src/Soucore.HealthCheck.MongoDb/README.md)   |
| Kafka    | ✅                           | ✅       |[link](/src/Soucore.HealthCheck.Kafka/README.md)   |
| Azure Queue Storage  | ❌               | ✅       |[link](/src/Soucore.HealthCheck.AzureQueueStorage/README.md)   |
| Smtp     | ❌                           | ✅       | [link](/src/Soucore.HealthCheck.Smtp/README.md)      |
