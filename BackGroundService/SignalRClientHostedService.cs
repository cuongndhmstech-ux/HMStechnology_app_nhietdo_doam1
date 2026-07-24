using HMS_Temp_Humdity_ApiManager.Signalr.Interface;

namespace HMS_Temp_Humdity_ApiManager.BackGroundService
{
	public class SignalRClientHostedService : BackgroundService
	{
		private readonly IHubDevice _hub;
		private readonly ILogger<SignalRClientHostedService> _logger;

		public SignalRClientHostedService(
			IHubDevice hub,
			ILogger<SignalRClientHostedService> logger)
		{
			_hub = hub;
			_logger = logger;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("SignalRClientHostedService started.");

			return _hub.StartAsync(stoppingToken);
		}
	}
}
