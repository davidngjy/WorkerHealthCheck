using Microsoft.Extensions.Diagnostics.HealthChecks;
using WorkerHealthChecks;
using WorkerHealthChecks.HealthChecks;

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
		services.AddHostedService<TcpHealthCheckProbeService>();
		services.AddHealthChecks()
			.AddCheck<PostgreSqlHealthCheck>("PostgreSql Check", HealthStatus.Unhealthy, new []{ "Readiness" });
	})
	.Build();

await host.RunAsync();
