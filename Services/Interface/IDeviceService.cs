using HMS_Temp_Humdity_ApiManager.Models;

namespace HMS_Temp_Humdity_ApiManager.Services.Interface
{
	public interface IDeviceService
	{
		Task<List<LocationResponse>> GetAllDeviceAndLocation();
		Task<List<DeviceModel>> getDeviceAndLocation2(LocationModel location, DeviceModel deviceModel);

		Task<bool> UpdateInfoDevice(DeviceModel request);
		Task<clsLocationDetailModel> GetLocationDetailAsync(string userId, string locationId);

		Task CreateDevice(DeviceModel request);
		Task<bool> ConfigDevice(DeviceModel request);
		Task<DeviceModel> GetDeviceByIMEI(string? IMEI);

		Task DeleteDevice(int deviceId);
		Task<List<LocationResponse>> GetDevicesByUserIdAsync(string userId);
		//Task<List<DeviceModel>> GetDeviceOnline(int? Companyid);
	}
}
