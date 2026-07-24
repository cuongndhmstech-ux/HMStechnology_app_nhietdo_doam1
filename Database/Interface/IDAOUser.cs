using System.Linq.Expressions;
using HMS_Temp_Humdity_ApiManager.Models;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_ApiManager.Database.Interface
{
	public interface IDAOUser
	{
		Task<List<UserModel>> GetAllAsync();

		Task CreateAsync(UserModel user);

		Task<bool> ModifyAsync(string id, UpdateDefinition<UserModel> updateDefinition);

		Task<bool> DeleteAsync(string id);

		Task<UserModel?> GetByUserNameAsync(string name);

		Task<UserModel?> GetByUserNameOrPhoneAsync(string input);

		Task<bool> ExistsAsync(Expression<Func<UserModel, bool>> filter);

		Task<UserModel> GetByIdAsync(string id);
	}
}
