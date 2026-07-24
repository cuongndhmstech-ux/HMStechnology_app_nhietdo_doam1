using HMS_Temp_Humdity_ApiManager.Models;
using static HMS_Temp_Humdity_ApiManager.DTO.AuthDTO;

namespace HMS_Temp_Humdity_ApiManager.Services.Interface
{
	public interface IAuthService
	{
		Task<LoginResponse> LoginAsync(LoginRequest request);

		Task<string> RegisterAsync(RegisterRequest request);

		Task<LoginResponse> RefreshTokenAsync(string refreshToken);

		Task<bool> updateUser(ChangePasswordRequest request, string userId);

		Task<bool> HasCompanyPermission(string userId, CompanyPermission permission);

	}
}
