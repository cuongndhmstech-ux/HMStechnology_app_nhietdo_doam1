using System.Linq.Expressions;
using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Models;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Database
{
	public class DAODevice : IDAODevice
	{
		private readonly IMongoCollection<DeviceModel> _mongo;
		private readonly ILogger<DAODevice> _logger;

		public DAODevice(ILogger<DAODevice> logger, MongodbContext mongoService)
		{
			_logger = logger;
			_mongo = mongoService.Devices;
		}


		public async Task<List<DeviceModel>> GetAsync(FilterDefinition<DeviceModel>? filter = null)
		{
			filter ??= Builders<DeviceModel>.Filter.Empty;

			return await _mongo.Find(filter).ToListAsync();
		}
		public async Task<List<DeviceModel>> GetDeviceByUserIdAsync(string userId)
		{
			try
			{
				var filter = Builders<DeviceModel>.Filter.Eq(dv => dv.UserId, userId);
				var result = await _mongo.Find(filter).ToListAsync();
				return result;
			}
			catch(Exception ex)
			{
				_logger.LogError(ex, "Error when get list device, detail: {Message}", ex.Message);
				return new List<DeviceModel>();
			}

        }

        public async Task<DeviceModel?> GetAsyncByImei(string imei)
			=> await _mongo.Find(x => x.Imei == imei).FirstOrDefaultAsync();
		public async Task CreateAsync(DeviceModel device)
		{
			try
			{
				await _mongo.InsertOneAsync(device);
			}
			catch (MongoException)
			{
				throw;
			}
		}


		public async Task<bool> ModifyAsync(int? id, UpdateDefinition<DeviceModel> updateDefinition)
		{
			try
			{
				var filter = Builders<DeviceModel>.Filter.Eq(x => x.DeviceId, id);
				var result = await _mongo.UpdateOneAsync(filter, updateDefinition);

				return result.MatchedCount > 0;
			}
			catch (MongoException)
			{
				throw;
			}
		}


		public async Task<bool> ExistsAsync(Expression<Func<DeviceModel, bool>> filter)
		{
			return await _mongo.Find(filter)
									.AnyAsync();
		}

		public async Task<bool> DeleteAsync(int id)
		{
			try
			{
				var result = await _mongo.DeleteOneAsync(x => x.DeviceId == id);
				return result.DeletedCount > 0;
			}
			catch (MongoException)
			{
				throw;
			}
		}

	}
}
