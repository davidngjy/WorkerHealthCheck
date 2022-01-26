using System.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace WorkerHealthChecks.HealthChecks;

internal class PostgreSqlHealthCheck : IHealthCheck
{
	private readonly ILogger<TcpHealthCheckProbeService> _logger;
	private readonly string _connectionString;

	public PostgreSqlHealthCheck(ILogger<TcpHealthCheckProbeService> logger, IConfiguration configuration)
	{
		_logger = logger;
		_connectionString = configuration.GetConnectionString("DefaultConnection");
	}

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
	{
		await using var dbConnection = new NpgsqlConnection(_connectionString);

		try
		{
			await dbConnection.OpenAsync(cancellationToken);

			if (dbConnection.State == ConnectionState.Open)
			{
				_logger.LogInformation("Connection successful");
				return HealthCheckResult.Healthy();
			}
			else
			{
				_logger.LogWarning("Connection refused");
				return HealthCheckResult.Unhealthy("Connection refused");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error");
			return HealthCheckResult.Unhealthy("Unable to connect", ex);
		}
		finally
		{
			await dbConnection.CloseAsync();
			NpgsqlConnection.ClearPool(dbConnection);
		}
	}
}