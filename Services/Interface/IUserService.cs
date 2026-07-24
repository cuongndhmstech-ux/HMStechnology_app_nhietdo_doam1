using HMS_Temp_Humdity_ApiManager.Models;

namespace HMS_Temp_Humdity_ApiManager.Services.Interface
{
	public interface IUserService
	{
		Task<List<UserModel>> getUsers();


		Task createUser(UserModel request);

		Task<bool> updateUser(UserModel request);

		Task deleteUser(string id);
	}
}
