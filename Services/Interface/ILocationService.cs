using HMS_Temp_Humdity_ApiManager.DTO;
using HMS_Temp_Humdity_ApiManager.Models;

namespace HMS_Temp_Humdity_ApiManager.Services.Interface
{
	public interface ILocationService
	{
		Task<List<LocationModel>> getAll();
		Task<ApiResponse<List<LocationModel>>> GetLocationByUserIdAsync(string userId);

		Task<bool> UpdateInfoLocation(LocationModel request);

		Task<ApiResponse<object>> CreateLocation(string name, string userId);

		Task DeleteLocation(LocationModel request);
	}
}
