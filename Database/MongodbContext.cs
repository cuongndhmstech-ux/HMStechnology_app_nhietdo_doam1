using HMS_NewProject_Temp_Humdity.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Database
{
	public class MongodbContext
	{
		protected readonly IMongoDatabase _database;
		private readonly ILogger<MongodbContext> _logger;

		public MongodbContext(IMongoDatabase database, ILogger<MongodbContext> logger)
		{
			_database = database;
			_logger = logger;
		}

		public IMongoDatabase mongoDatabase => _database;
		public IMongoCollection<UserModel> Users => _database.GetCollection<UserModel>("Users");
		public IMongoCollection<DeviceModel> Devices => _database.GetCollection<DeviceModel>("Devices");
		public IMongoCollection<LocationModel> Locations => _database.GetCollection<LocationModel>("Locations");

		public async Task<bool> IsConnected()
		{
			try
			{
				await _database.RunCommandAsync<BsonDocument>(
					new BsonDocument("ping", 1));
				_logger.LogInformation("MongoDB connection successful.");
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "MongoDB connection error: {Message}", ex.Message);
				return false;
			}
		}
	}
}
