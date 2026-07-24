using System.Linq.Expressions;
using HMS_Temp_Humdity_ApiManager.Models;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_ApiManager.Database.Interface
{
	public interface IDAOLocation
	{
		Task<List<LocationModel>> GetAllAsync(FilterDefinition<LocationModel>? filter = null);

		Task<LocationModel?> GetAsync(Expression<Func<LocationModel, bool>> filter);

		Task CreateAsync(LocationModel device);

		Task<bool> ModifyAsync(string id, UpdateDefinition<LocationModel> updateDefinition);

		Task<bool> ExistsAsync(Expression<Func<LocationModel, bool>> filter);

		Task<bool> DeleteAsync(string id);
		Task<List<LocationModel>> GetLocationByUserId(string userId);

	}
}
