using HMS_NewProject_Temp_Humdity.Models;

namespace HMS_NewProject_Temp_Humdity.Services.Interface
{
	public interface IUserService
	{
		Task<List<UserModel>> getUsers();


		Task createUser(UserModel request);

		Task<bool> updateUser(UserModel request);

		Task deleteUser(string id);
	}
}
