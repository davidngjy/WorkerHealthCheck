using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WorkerHealthChecks;

public class TcpHealthCheckProbeService : BackgroundService
{
	private readonly HealthCheckService _healthCheckService;
	private readonly ILogger<TcpHealthCheckProbeService> _logger;
	private readonly TcpListener _listener;

	public TcpHealthCheckProbeService(HealthCheckService healthCheckService, ILogger<TcpHealthCheckProbeService> logger, IConfiguration configuration)
	{
		_healthCheckService = healthCheckService;
		_logger = logger;
		_listener = new TcpListener(IPAddress.Any, configuration.GetValue<int>("HealthProbePort"));
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			var healthReport = await _healthCheckService.CheckHealthAsync(stoppingToken);

			if (healthReport.Status == HealthStatus.Healthy)
			{
				_listener.Start();
				_logger.LogInformation("Successful health check.");
			}
			else
			{
				_listener.Stop();
				_logger.LogWarning("Unsuccessful health check.");
			}

			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
		}
	}
}