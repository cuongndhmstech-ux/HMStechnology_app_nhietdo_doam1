using HMS_NewProject_Temp_Humdity.Models;
using HMS_NewProject_Temp_Humdity.Models.Config;
using HMS_NewProject_Temp_Humdity.Signalr.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;

namespace HMS_NewProject_Temp_Humdity.Signalr
{
	public class HubDeviceMonitor : IHubDevice, IAsyncDisposable
	{
		private readonly ILogger<HubDeviceMonitor> _logger;
		private readonly HubConnection _connection;
		private readonly SemaphoreSlim _connectLock = new(1, 1);

		public HubDeviceMonitor(ILogger<HubDeviceMonitor> logger, clsAppConfig config)
		{
			_logger = logger;

			_connection = new HubConnectionBuilder()
				.WithUrl(config.SignalR.HubUrl)
				.WithAutomaticReconnect(new[]
				{
					TimeSpan.Zero,
					TimeSpan.FromSeconds(2),
					TimeSpan.FromSeconds(5),
					TimeSpan.FromSeconds(10)
				})
				.Build();

			_connection.Reconnecting += error =>
			{
				_logger.LogWarning(error, "[Hub] Đang reconnect...");
				return Task.CompletedTask;
			};

			_connection.Reconnected += connectionId =>
			{
				_logger.LogInformation("[Hub] Reconnected. ConnectionId={ConnectionId}", connectionId);
				return Task.CompletedTask;
			};

			_connection.Closed += error =>
			{
				_logger.LogWarning(error, "[Hub] Connection closed.");
				return Task.CompletedTask;
			};
		}

		private async Task EnsureConnected()
		{
			if (_connection.State == HubConnectionState.Connected)
				return;

			await _connectLock.WaitAsync();
			try
			{
				if (_connection.State == HubConnectionState.Disconnected)
					await _connection.StartAsync();
			}
			finally
			{
				_connectLock.Release();
			}
		}

		private async Task SendAsync(string eventName, object payload, string logContext)
		{
			try
			{
				await EnsureConnected();
				string json = JsonConvert.SerializeObject(payload);
				await _connection.InvokeAsync("SendMessage", eventName, json);
				_logger.LogInformation("[Hub] Gửi {EventName} thành công. {Context}", eventName, logContext);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "[Hub] Gửi {EventName} thất bại. {Context}", eventName, logContext);
			}
		}

		// ================== DEVICE ==================
		public async Task NotifyDeviceAddedAsync(DeviceModel deviceModel)
			=> await SendAsync("DeviceCreated", deviceModel, $"IMEI={deviceModel.Imei}");

		public async Task NotifyDeviceUpdatedAsync(DeviceModel deviceModel)
			=> await SendAsync("DeviceUpdated", deviceModel, $"IMEI={deviceModel.Imei}");

		public async Task NotifyDeviceDeletedAsync(DeviceModel deviceModel)
			=> await SendAsync("DeviceDeleted", deviceModel, $"IMEI={deviceModel.Imei}");

		// ================== LOCATION ==================
		public async Task NotifyLocationAddedAsync(LocationModel locationModel)
			=> await SendAsync("LocationCreated", locationModel, $"LocationId={locationModel.LocationId}");

		public async Task NotifyLocationUpdatedAsync(LocationModel locationModel)
			=> await SendAsync("LocationUpdated", locationModel, $"LocationId={locationModel.LocationId}");

		public async Task NotifyLocationDeletedAsync(LocationModel locationModel)
			=> await SendAsync("LocationDeleted", locationModel, $"LocationId={locationModel.LocationId}");

		public async ValueTask DisposeAsync()
		{
			if (_connection != null)
				await _connection.DisposeAsync();
		}


	}
}