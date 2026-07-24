using HMS_Temp_Humdity_ApiManager.BaseException;
using HMS_Temp_Humdity_ApiManager.Database.Interface;
using HMS_Temp_Humdity_ApiManager.Models;
using HMS_Temp_Humdity_ApiManager.Services.Interface;
using HMS_Temp_Humdity_ApiManager.Signalr.Interface;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_ApiManager.Services
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

			var locationFilter = Builders<LocationModel>.Filter.Empty;
			var deviceFilter = Builders<DeviceModel>.Filter.Empty;

			var locations = await _dAOLocation.GetAllAsync(locationFilter);
			var devices = await _dAODevice.GetAsync(deviceFilter);

			_logger.LogInformation("Found {LocationCount} locations and {DeviceCount} devices.", locations.Count, devices.Count);

			var lookup = devices.ToLookup(x => x.LocationId);

			var result = locations.Select(x =>
			{
				var deviceCount = lookup[x.LocationId].Count();

				_logger.LogInformation("Location {LocationId} ({LocationName}) has {DeviceCount} devices.", x.LocationId, x.Name, deviceCount);

				return new LocationResponse
				{
					LocationId = x.LocationId!,
					UserId = x.UserId,
					Name = x.Name,
					Devices = lookup[x.LocationId].ToList()
				};
			}).ToList();
			var locationIds = locations
				.Where(x => !string.IsNullOrWhiteSpace(x.LocationId))
				.Select(x => x.LocationId!)
				.ToHashSet();

			var unassignedDevices = devices
				.Where(d => string.IsNullOrWhiteSpace(d.LocationId) || !locationIds.Contains(d.LocationId))
				.ToList();

			if (unassignedDevices.Any())
			{
				_logger.LogInformation("Found {DeviceCount} unassigned devices.", unassignedDevices.Count);

				result.Add(new LocationResponse
				{
					LocationId = "UNASSIGNED",
					UserId = null,
					Name = "Chưa gán vị trí",
					Devices = unassignedDevices
				});
			}

			_logger.LogInformation("Returning {LocationCount} locations.", result.Count);

			return result;
		}

		public async Task<List<LocationResponse>> GetDevicesByUserIdAsync(string userId)
		{
			try
			{
				//if (string.IsNullOrEmpty(userId)) return new List<DeviceModel>();
				var locationFilter = Builders<LocationModel>.Filter.Eq(x => x.UserId, userId);
				var locations = await _dAOLocation.GetAllAsync(locationFilter);
				var devices = await _dAODevice.GetDeviceByUserIdAsync(userId);
				//Phân nhóm thiết bị theo LocationId
				var lookup = devices.ToLookup(x => x.LocationId);
				var result = locations.Select(x => new LocationResponse
				{
					LocationId = x.LocationId!,
					UserId = x.UserId,
					Name = x.Name,
					Devices = lookup[x.LocationId].ToList()
				}).ToList();
				var assignedLocationIds = locations.Select(l => l.LocationId).ToHashSet();
				var unassignedDevices = devices
					.Where(d => string.IsNullOrEmpty(d.LocationId) || !assignedLocationIds.Contains(d.LocationId))
					.ToList();

				if (unassignedDevices.Any())
				{
					result.Insert(0, new LocationResponse
					{
						LocationId = "UNASSIGNED",
						UserId = userId,
						Name = "Thiết bị chưa gán phòng",
						Devices = unassignedDevices
					});
				}

				return result;
			}
			catch (Exception)
			{
				return new List<LocationResponse>();
			}
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
			updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.Sensors, request.Sensors));
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
			var randomCode = new Random().Next(100000, 999999);

			//for (int i = 1; i <= 4; i++)
			//{
			//	sensors.Add(new Sensor
			//	{
			//		NameSensor = $"Sensor {i}",
			//		TemperatureMin = 18,
			//		TemperatureMax = 30,
			//		HumidityMin = 40,
			//		HumidityMax = 80
			//	});
			//}

			var device = new DeviceModel
			{
				DeviceId = randomCode,
				DeviceName = request.DeviceName,
				UserId = request.UserId,
				Imei = request.Imei,
				LocationId = string.IsNullOrEmpty(request.LocationId) ? string.Empty : request.LocationId,
				Sensors = new List<Sensor>(),
				TimeStamp = DateTime.Now
			};
			// cái này xử lí redis bên process data
			await _hubDevice.NotifyDeviceAddedAsync(request);
			await _dAODevice.CreateAsync(device);
		}

		public async Task<bool> ConfigDevice(DeviceModel request)
		{
			if (!await _dAODevice.ExistsAsync(x => x.Imei == request.Imei && x.UserId == request.UserId))
			{
				throw new ResourceNotFoundException("thiết bị ko sở hữu");
			}
			// cập nhập từng trường
			var updateDef = new List<UpdateDefinition<DeviceModel>>();
			if (request.HumidityMin != null)
				updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.HumidityMin, request.HumidityMin));

			if (request.HumidityMax != null)
				updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.HumidityMax, request.HumidityMax));

			if (request.TemperatureMin != null)
				updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.TemperatureMin, request.TemperatureMin));

			if (request.TemperatureMax != null)
				updateDef.Add(Builders<DeviceModel>.Update.Set(x => x.TemperatureMax, request.TemperatureMax));

			// Không có trường nào cần cập nhật
			if (!updateDef.Any())
				return false;


			var combinedUpdate = Builders<DeviceModel>.Update.Combine(updateDef);
			bool isSuccess = await _dAODevice.ModifyAsync(request.DeviceId, combinedUpdate);
			// cái này xử lí redis bên process data
			await _hubDevice.NotifyDeviceAddedAsync(request);
			return isSuccess;
		}

		public async Task DeleteDevice(int deviceId)
		{
			//var device = await _dAODevice.GetByDeviceIdAsync(deviceId); // lấy trước khi xóa
			var deleted = await _dAODevice.DeleteAsync(deviceId);
			if (!deleted) throw new ResourceNotFoundException("Device không tồn tại để xóa");
			// cái này xử lí redis bên process data
			//await _hub.NotifyDeviceDeletedAsync(device); 

		}

		public async Task<clsLocationDetailModel> GetLocationDetailAsync(string userId, string locationId)
		{
			//lấy thông tin phòng
			var location = await _dAOLocation.GetAsync(x => x.LocationId == locationId && x.UserId == userId);
			if (location == null) { throw new ResourceNotFoundException("Phòng không tồn tại"); }
			var allDevices = await _dAODevice.GetDeviceByUserIdAsync(userId);
			var response = new clsLocationDetailModel
			{
				LocationId = location.LocationId,
				Name = location.Name,
				AssignedDevices = allDevices.Where(d => d.LocationId == locationId).ToList(),
				AvailableDevices = allDevices.Where(d => string.IsNullOrEmpty(d.LocationId)).ToList()
			};
			return response;
		}



		//public async Task<List<DeviceModel>?> GetDeviceOnline(int? Companyid)
		//{
		//	if (!Companyid.HasValue) return null;

		//	var logs = await _reportApiClient.GetLatestTimestampAsync();

		//	return await _dAODevice.GetDeviceStatusAsync(Companyid.Value, logs);
		//}



	}
}
