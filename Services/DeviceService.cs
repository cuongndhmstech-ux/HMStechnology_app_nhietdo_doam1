using HMS_NewProject_Temp_Humdity.BaseException;
using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Models;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using HMS_NewProject_Temp_Humdity.Signalr.Interface;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Services
{
	public class DeviceService : IDeviceService
	{
		private readonly IDAODevice _dAODevice;
		private readonly IDAOLocation _dAOLocation;
		private readonly ILogger<DeviceService> _logger;
		private readonly IHubDevice _hubDevice;


		public DeviceService(ILogger<DeviceService> logger, IDAODevice dAODevice, IDAOLocation dAOLocation, IHubDevice hubDevice)
		{
			_dAODevice = dAODevice;
			_dAOLocation = dAOLocation;
			_logger = logger;
			_hubDevice = hubDevice;
		}


		public async Task<List<LocationResponse>> GetAllDeviceAndLocation()
		{
			_logger.LogInformation("Getting all devices and locations.");

			var locationFilter = Builders<LocationModel>.Filter.Empty;
			var deviceFilter = Builders<DeviceModel>.Filter.Empty;

			var locations = await _dAOLocation.GetAllAsync(locationFilter);
			var devices = await _dAODevice.GetAsync(deviceFilter);

			_logger.LogInformation(
				"Found {LocationCount} locations and {DeviceCount} devices.",
				locations.Count,
				devices.Count);

			var lookup = devices.ToLookup(x => x.LocationId);

			var result = locations.Select(x =>
			{
				var deviceCount = lookup[x.LocationId].Count();
				_logger.LogInformation(
					"Location {LocationId} ({LocationName}) has {DeviceCount} devices.",
					x.LocationId,
					x.Name,
					deviceCount);

				return new LocationResponse
				{
					LocationId = x.LocationId,
					UserId = x.UserId,
					Name = x.Name,
					Devices = lookup[x.LocationId].ToList()
				};
			}).ToList();

			_logger.LogInformation("Returning {LocationCount} locations.", result.Count);
			return result;
		}


		public async Task<List<DeviceModel>> getDeviceAndLocation2(LocationModel location, DeviceModel deviceModel)
		{
			var builder = Builders<LocationModel>.Filter;
			var filters = new List<FilterDefinition<LocationModel>>();

			filters.Add(builder.Eq(x => x.UserId, location.UserId));
			if (location.Name != null)
				filters.Add(builder.Eq(x => x.Name, location.Name));

			var mongoFilter = filters.Any()
							? builder.And(filters)
							: FilterDefinition<LocationModel>.Empty;
			var locations = await _dAOLocation.GetAllAsync(mongoFilter);


			return await _dAODevice.GetAsync();
		}

		public async Task<DeviceModel> GetDeviceByIMEI(string? IMEI)
		{
			var device = await _dAODevice.GetAsyncByImei(IMEI);
			if (device == null) throw new ResourceNotFoundException("thiết bị ko tồn tại");

			return device;
		}

		public async Task<bool> UpdateInfoDevice(DeviceModel request)
		{

			if (!await _dAODevice.ExistsAsync(x => x.DeviceId == request.DeviceId))
			{
				throw new ResourceNotFoundException("thiết bị ko tồn tại");
			}


			if (!await _dAOLocation.ExistsAsync(x => x.LocationId == request.LocationId))
			{
				throw new ResourceNotFoundException("địa điểm chưa có");
			}

			// cập nhập từng trường
			var updateDef = new List<UpdateDefinition<DeviceModel>>();
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.LocationId, request.LocationId));
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.TemperatureMax, request.TemperatureMax));
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.TemperatureMin, request.TemperatureMin));
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.HumidityMax, request.HumidityMax));
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.HumidityMin, request.HumidityMin));
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.TimeStamp, DateTime.Now));
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.IsActive, request.IsActive));


			var combinedUpdate = Builders<DeviceModel>.Update.Combine(updateDef);
			bool isSuccess = await _dAODevice.ModifyAsync(request.DeviceId, combinedUpdate);
			// cái này xử lí redis bên process data
			//await _hub.NotifyDeviceAddedAsync(request);
			return isSuccess;
		}

		public async Task CreateDevice(DeviceModel request)
		{
			if (request == null)
				throw new BadRequestException("Dữ liệu không hợp lệ");

			if (await _dAODevice.ExistsAsync(x => x.Imei == request.Imei))
			{
				throw new DuplicateResourceException("thiết bị đã tồn tại");
			}
			if (!await _dAOLocation.ExistsAsync(x => x.LocationId == request.LocationId))
			{
				throw new ResourceNotFoundException("địa điểm chưa có");
			}
			var randomCode = new Random().Next(100000, 999999);

			var device = new DeviceModel
			{
				DeviceId = randomCode,
				UserId = request.UserId,
				Imei = request.Imei,
				LocationId = request.LocationId,
				HumidityMax = 60,
				TemperatureMin = 15,
				HumidityMin = 45,
				TemperatureMax = 25,

			};
			// cái này xử lí redis bên process data
			await _hubDevice.NotifyDeviceAddedAsync(request);
			await _dAODevice.CreateAsync(device);
		}

		public async Task DeleteDevice(int deviceId)
		{
			//var device = await _dAODevice.GetByDeviceIdAsync(deviceId); // lấy trước khi xóa
			var deleted = await _dAODevice.DeleteAsync(deviceId);
			if (!deleted) throw new ResourceNotFoundException("Device không tồn tại để xóa");
			// cái này xử lí redis bên process data
			//await _hub.NotifyDeviceDeletedAsync(device); 

		}


		//public async Task<List<DeviceModel>?> GetDeviceOnline(int? Companyid)
		//{
		//	if (!Companyid.HasValue) return null;

		//	var logs = await _reportApiClient.GetLatestTimestampAsync();

		//	return await _dAODevice.GetDeviceStatusAsync(Companyid.Value, logs);
		//}
	}
}
