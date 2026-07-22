using HMS_NewProject_Temp_Humdity.BaseException;
using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Models;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using HMS_NewProject_Temp_Humdity.Signalr.Interface;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Services
{
	public class LocationService : ILocationService
	{

		private readonly IDAOLocation _dAOLocation;
		private readonly IHubDevice _hubDevice;
		public LocationService(IDAOLocation dAOLocation, IHubDevice hubDevice)
		{
			_dAOLocation = dAOLocation;
			_hubDevice = hubDevice;
		}


		public async Task<List<LocationModel>> getAll()
		{

			var builder = Builders<LocationModel>.Filter;
			var filters = new List<FilterDefinition<LocationModel>>();

			//filters.Add(builder.Eq(x => x.UserId, location.UserId));
			//if (location.Name != null)
			//	filters.Add(builder.Eq(x => x.Name, location.Name));

			var mongoFilter = filters.Any()
							? builder.And(filters)
							: FilterDefinition<LocationModel>.Empty;

			return await _dAOLocation.GetAllAsync(mongoFilter);
		}

		public async Task<bool> UpdateInfoLocation(LocationModel request)
		{




			if (!await _dAOLocation.ExistsAsync(x => x.LocationId == request.LocationId && x.UserId == request.UserId))
			{
				throw new ResourceNotFoundException("địa điểm chưa có");
			}

			// cập nhập từng trường
			var updateDef = new List<UpdateDefinition<LocationModel>>();
			updateDef.Add(Builders<LocationModel>.Update.Set(x => x.Name, request.Name));

			var combinedUpdate = Builders<LocationModel>.Update.Combine(updateDef);
			bool isSuccess = await _dAOLocation.ModifyAsync(request.LocationId, combinedUpdate);
			// cái này xử lí redis bên process data
			//await _hubDevice.NotifyLocationUpdatedAsync(request);
			return isSuccess;
		}

		public async Task CreateLocation(string name, string userId)
		{

			var randomCode = new Random().Next(100000, 999999);
			var updateDef = new List<UpdateDefinition<LocationModel>>();

			if (await _dAOLocation.ExistsAsync(x => x.Name == name && x.UserId == userId))
			{
				throw new DuplicateResourceException("Địa điểm đã tồn tại");
			}

			var device = new LocationModel
			{
				LocationId = $"L{randomCode}",
				Name = name,
				UserId = userId,

			};
			// cái này xử lí redis bên process data
			//await _hubDevice.NotifyLocationAddedAsync(device);
			await _dAOLocation.CreateAsync(device);
		}

		public async Task DeleteLocation(LocationModel request)
		{
			//var device = await _dAOLocation.GetByDeviceIdAsync(deviceId); // lấy trước khi xóa
			var deleted = await _dAOLocation.DeleteAsync(request.LocationId);
			if (!deleted) throw new ResourceNotFoundException("Device không tồn tại để xóa");
			// cái này xử lí redis bên process data
			//await _hubDevice.NotifyLocationDeletedAsync(request);
		}
	}
}
