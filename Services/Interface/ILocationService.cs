using HMS_NewProject_Temp_Humdity.DTO;
using HMS_NewProject_Temp_Humdity.Models;

namespace HMS_NewProject_Temp_Humdity.Services.Interface
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
