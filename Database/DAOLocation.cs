using System.Linq.Expressions;
using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Models;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Database
{
    public class DAOLocation : IDAOLocation
    {
        private readonly IMongoCollection<LocationModel> _mongo;
        private readonly ILogger<DAOLocation> _logger;

        public DAOLocation(ILogger<DAOLocation> logger, MongodbContext mongoService)
        {
            _logger = logger;
            _mongo = mongoService.Locations;
        }


        public async Task<List<LocationModel>> GetAllAsync(FilterDefinition<LocationModel>? filter = null)
        {

            return await _mongo.Find(filter).ToListAsync();
        }
        public async Task<LocationModel?> GetAsync(Expression<Func<LocationModel, bool>> filter)
        {
            try
            {
                return await _mongo.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chi tiết phòng");
                return null;
            }
        }
        public async Task<List<LocationModel>> GetLocationByUserId(string userId)
        {
            var filter = Builders<LocationModel>.Filter.Eq(lc => lc.UserId, userId);
            var result = await _mongo.Find(filter).ToListAsync();
            return result;
        }

        public async Task CreateAsync(LocationModel location)
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


        public async Task<bool> ModifyAsync(string id, UpdateDefinition<LocationModel> updateDefinition)
        {
            try
            {
                var filter = Builders<LocationModel>.Filter.Eq(x => x.LocationId, id);
                var result = await _mongo.UpdateOneAsync(filter, updateDefinition);
                return result.MatchedCount > 0;
            }
            catch (MongoException)
            {
                throw;
            }
        }



        public async Task<bool> ExistsAsync(Expression<Func<LocationModel, bool>> filter)
        {

            return await _mongo.Find(filter)
                                    .AnyAsync();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var result = await _mongo.DeleteOneAsync(x => x.LocationId == id);
                return result.DeletedCount > 0;
            }
            catch (MongoException)
            {
                throw;
            }
        }
    }
}

