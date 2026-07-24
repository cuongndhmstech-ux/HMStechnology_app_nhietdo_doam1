using System.Linq.Expressions;
using HMS_Temp_Humdity_ApiManager.Models;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_ApiManager.Database.Interface
{
	public interface IDAOCompany
	{
		Task<List<CompanyModel>> GetAllAsync(FilterDefinition<CompanyModel>? filter = null);


		Task CreateAsync(CompanyModel device);

		Task<bool> ModifyAsync(string id, UpdateDefinition<CompanyModel> updateDefinition);

		Task<bool> ExistsAsync(Expression<Func<CompanyModel, bool>> filter);

		Task<bool> DeleteAsync(string id);
	}
}
