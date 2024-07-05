using Soucore.HealthCheck.Attributes;
using Soucore.HealthCheck.HealthCheck.Interface;
using Soucore.HealthCheck.Model;

namespace SampleWorker.CustomizedHealthCheck;

[HealthCheckTimeout(2)]
internal class CustomHealthCheckAuto<T> : IHealthCheckCustom where T : InjectableInstance
{
    private readonly ILogger<CustomHealthCheckAuto<T>> logger;
    private readonly IServiceProvider provider;

    public bool Disabled { get; set; }

    public CustomHealthCheckAuto(ILogger<CustomHealthCheckAuto<T>> logger, IServiceProvider provider)
    {
        this.logger = logger;
        this.provider = provider;
    }
#pragma warning disable CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
    public async Task<HealthCheckResult> ExecuteAsync(CancellationToken stoppingToken)
#pragma warning restore CS1998 // O método assíncrono não possui operadores 'await' e será executado de forma síncrona
    {
        try
        {
            var obj = provider.GetService<T>();
            if (obj is not null)
                return new HealthCheckResult(obj.Example());

            return new HealthCheckResult(false);
        }
        catch (Exception ex)
        {
            logger.LogCritical("{name} => {error}", nameof(CustomHealthCheckAuto<T>), ex.ToString());
            return new HealthCheckResult(false, ex);
        }
    }
}


internal class CustomHealthCheckConfiguration
{
    public string Url { get; set; }
}

internal class InjectableInstance
{
    private const bool Status = true;
    public bool Example() => Status;
}
