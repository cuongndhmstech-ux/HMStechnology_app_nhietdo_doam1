using HMS_NewProject_Temp_Humdity.Models;

namespace HMS_NewProject_Temp_Humdity.DTO
{
	public class AuthDTO
	{
		public class LoginRequest
		{
			public required string Phone { get; set; }
			public required string Password { get; set; }

			public bool? RememberLogin { get; set; }

		}

		public class RegisterRequest
		{
			public required string Phone { get; set; }
			public required string Password { get; set; }

			public required string Fullname { get; set; }
		}

		public class ChangePasswordRequest
		{
			public string OldPassword { get; set; } = string.Empty;
			public required string Password { get; set; }
		}
		public class LoginResponse
		{
			public required string AccessToken { get; set; }
			public string? RefreshToken { get; set; }
			public required string Phone { get; set; }
			public string? FullName { get; set; }

			public Role role { get; set; }
			public long permission { get; set; }

			//public UserInfoDto UserInfo { get; set; } = new();

		}

		public class RefreshTokenRequest
		{
			public string RefreshToken { get; set; } = null!;
		}

	}
}
