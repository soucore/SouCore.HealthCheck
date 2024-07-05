using HealthCheck.Smtp.Example;
using Soucore.HealthCheck;
using Soucore.HealthCheck.Smtp;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();

        var configuration = context.Configuration;
        var smtpSettings = new SmtpHealthCheckSettings();
        configuration.GetSection("Smtp").Bind(smtpSettings);

        services.AddViaHealthCheck((services, probes) =>
        {
            probes.AddSmtpHealthCheck(null, config =>
            {
                config.Port = smtpSettings.Port;
                config.Host = smtpSettings.Host;
            });

            probes.AddHealthCheck(8081, "/readiness");
            probes.AddHealthCheck(8080, "/liveness", true);
        });
    })
    .Build();

await host.RunAsync();
