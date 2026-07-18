using HMS_NewProject_Temp_Humdity.Models;

namespace HMS_NewProject_Temp_Humdity.Signalr.Interface
{
	public interface IHubDevice
	{
		Task NotifyLocationAddedAsync(LocationModel locationModel);
		Task NotifyDeviceAddedAsync(DeviceModel deviceModel);
		Task NotifyDeviceUpdatedAsync(DeviceModel deviceModel);
		Task NotifyDeviceDeletedAsync(DeviceModel deviceModel);
		Task NotifyLocationUpdatedAsync(LocationModel locationModel);
		Task NotifyLocationDeletedAsync(LocationModel locationModel);


	}
}
