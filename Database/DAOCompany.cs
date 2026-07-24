using System.Linq.Expressions;
using HMS_Temp_Humdity_ApiManager.Database.Interface;
using HMS_Temp_Humdity_ApiManager.Models;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_ApiManager.Database
{
	public class DAOCompany : IDAOCompany
	{
		private readonly IMongoCollection<CompanyModel> _mongo;
		private readonly ILogger<DAOCompany> _logger;

		public DAOCompany(ILogger<DAOCompany> logger, MongodbContext mongoService)
		{
			_logger = logger;
			_mongo = mongoService.companies;
		}
		public async Task<List<CompanyModel>> GetAllAsync(FilterDefinition<CompanyModel>? filter = null)
		{

			return await _mongo.Find(filter).ToListAsync();
		}
		public async Task CreateAsync(CompanyModel location)
		{
			try
			{
				await _mongo.InsertOneAsync(location);
			}
			catch (MongoException)
			{
				throw;
			}
		}
		public async Task<bool> ModifyAsync(string id, UpdateDefinition<CompanyModel> updateDefinition)
		{
			try
			{
				var filter = Builders<CompanyModel>.Filter.Eq(x => x.CompanyId, id);
				var result = await _mongo.UpdateOneAsync(filter, updateDefinition);
				return result.MatchedCount > 0;
			}
			catch (MongoException)
			{
				throw;
			}
		}
		public async Task<bool> ExistsAsync(Expression<Func<CompanyModel, bool>> filter)
		{

			return await _mongo.Find(filter)
									.AnyAsync();
		}
		public async Task<bool> DeleteAsync(string id)
		{
			try
			{
				var result = await _mongo.DeleteOneAsync(x => x.CompanyId == id);
				return result.DeletedCount > 0;
			}
			catch (MongoException)
			{
				throw;
			}
		}

	}
}
