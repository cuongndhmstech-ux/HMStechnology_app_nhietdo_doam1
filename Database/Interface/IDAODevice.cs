using System.Linq.Expressions;
using HMS_NewProject_Temp_Humdity.Models;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Database.Interface
{
	public interface IDAODevice
	{
		Task<List<DeviceModel>> GetAsync(FilterDefinition<DeviceModel>? filter = null);

		Task CreateAsync(DeviceModel device);

		Task<bool> ModifyAsync(int? id, UpdateDefinition<DeviceModel> updateDefinition);

		Task<DeviceModel?> GetAsyncByImei(string imei);
		//Task<DeviceModel?> GetByDeviceIdAsync(int deviceId);

		Task<bool> ExistsAsync(Expression<Func<DeviceModel, bool>> filter);

		Task<bool> DeleteAsync(int id);

		//Task<List<DeviceModel>> GetDeviceStatusAsync(int Companyid, List<clsDeviceLastLogModel> deviceModels);
	}
}
