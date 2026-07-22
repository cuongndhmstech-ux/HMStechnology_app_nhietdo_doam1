using System.Linq.Expressions;
using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Models;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Database
{
	public class DAOUser : IDAOUser
	{
		private readonly IMongoCollection<UserModel> _mongo;
		private readonly ILogger<DAOUser> _logger;

		public DAOUser(ILogger<DAOUser> logger, MongodbContext mongoService)
		{
			_logger = logger;
			_mongo = mongoService.Users;
		}

		public async Task CreateAsync(UserModel user)
		{
			try
			{
				await _mongo.InsertOneAsync(user);
			}
			catch (MongoException ex)
			{
				_logger.LogError(ex, "Lỗi MongoDB khi tạo User");
				throw;
			}
		}

		public async Task<bool> ModifyAsync(string id, UpdateDefinition<UserModel> updateDefinition)
		{
			try
			{
				var filter = Builders<UserModel>.Filter.Eq(x => x.UserId, id);
				var result = await _mongo.UpdateOneAsync(filter, updateDefinition);
				return result.MatchedCount > 0;
			}
			catch (MongoException ex)
			{
				_logger.LogError(ex, "Lỗi MongoDB khi update User {Id}", id);
				throw;
			}
		}
		public async Task<bool> DeleteAsync(string id)
		{
			try
			{
				var result = await _mongo.DeleteOneAsync(x => x.UserId == id);
				return result.DeletedCount > 0;
			}
			catch (MongoException ex)
			{
				_logger.LogError(ex, "Lỗi MongoDB khi xóa User {Id}", id);
				throw;
			}
		}

		public async Task<List<UserModel>> GetAllAsync()
			=> await _mongo.Find(_ => true).ToListAsync();


		public async Task<UserModel?> GetByUserNameAsync(string name)
		{
			_logger.LogInformation("Bắt đầu tìm user theo PhoneNumber: {PhoneNumber}", name);

			var result = await _mongo.Find(x => x.PhoneNumber == name).FirstOrDefaultAsync();

			if (result == null)
				_logger.LogWarning("Không tìm thấy user với PhoneNumber: {PhoneNumber}", name);
			else
				_logger.LogInformation("Tìm thấy user. UserId: {UserId}, PhoneNumber: {PhoneNumber}", result.UserId, result.PhoneNumber);

			return result;
		}

		public async Task<UserModel?> GetByUserNameOrPhoneAsync(string input)
		{
			var filter = Builders<UserModel>.Filter.Or(
				Builders<UserModel>.Filter.Eq(x => x.Username, input),
				Builders<UserModel>.Filter.Eq(x => x.PhoneNumber, input)
			);
			return await _mongo.Find(filter).FirstOrDefaultAsync();
		}
		public async Task<UserModel?> GetByIdAsync(string id)
			=> await _mongo.Find(x => x.UserId == id).FirstOrDefaultAsync();

		public async Task<bool> ExistsAsync(Expression<Func<UserModel, bool>> filter)
			=> await _mongo.Find(filter).AnyAsync();




	}
}
