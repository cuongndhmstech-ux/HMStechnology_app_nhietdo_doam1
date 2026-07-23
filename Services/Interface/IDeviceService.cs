using HMS_NewProject_Temp_Humdity.Models;

namespace HMS_NewProject_Temp_Humdity.Services.Interface
{
	public interface IDeviceService
	{
		Task<List<LocationResponse>> GetAllDeviceAndLocation();

		Task<bool> UpdateInfoDevice(DeviceModel request);

		Task CreateDevice(DeviceModel request);

		Task<DeviceModel> GetDeviceByIMEI(string? IMEI);

		Task DeleteDevice(int deviceId);
		Task<List<LocationResponse>> GetDevicesByUserIdAsync(string userId);
        //Task<List<DeviceModel>> GetDeviceOnline(int? Companyid);
    }
}
