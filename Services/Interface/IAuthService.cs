using HMS_NewProject_Temp_Humdity.Models;
using static HMS_NewProject_Temp_Humdity.DTO.AuthDTO;

namespace HMS_NewProject_Temp_Humdity.Services.Interface
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
