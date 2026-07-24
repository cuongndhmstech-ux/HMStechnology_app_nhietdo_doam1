using HMS_Temp_Humdity_ApiManager.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace HMS_Temp_Humdity_ApiManager.Signalr.Interface
{
	public interface IHubDevice
	{
		Task StartAsync(CancellationToken cancellationToken);
		HubConnectionState State { get; }

		// DEVICE
		Task NotifyDeviceAddedAsync(DeviceModel deviceModel);
		Task NotifyDeviceUpdatedAsync(DeviceModel deviceModel);
		Task NotifyDeviceDeletedAsync(DeviceModel deviceModel);

		// LOCATION
		Task NotifyLocationAddedAsync(LocationModel locationModel);
		Task NotifyLocationUpdatedAsync(LocationModel locationModel);
		Task NotifyLocationDeletedAsync(LocationModel locationModel);


	}
}
