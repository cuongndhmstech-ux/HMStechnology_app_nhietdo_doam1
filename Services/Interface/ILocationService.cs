using HMS_NewProject_Temp_Humdity.Models;

namespace HMS_NewProject_Temp_Humdity.Services.Interface
{
	public interface ILocationService
	{
		Task<List<LocationModel>> getAll();

		Task<bool> UpdateInfoLocation(LocationModel request);

		Task CreateLocation(string name, string userId);

		Task DeleteLocation(LocationModel request);
	}
}
