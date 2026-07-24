using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using HMS_Temp_Humdity_ApiManager.BaseException;
using HMS_Temp_Humdity_ApiManager.Database.Interface;
using HMS_Temp_Humdity_ApiManager.Models;
using HMS_Temp_Humdity_ApiManager.Services.Interface;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using static HMS_Temp_Humdity_ApiManager.DTO.AuthDTO;
using LoginRequest = HMS_Temp_Humdity_ApiManager.DTO.AuthDTO.LoginRequest;
namespace HMS_Temp_Humdity_ApiManager.Services
{
	public class AuthService : IAuthService
	{
		private readonly IDAOUser _dAOUser;
		private readonly IConfiguration _config;
		private readonly ILogger<AuthService> _logger;
		private readonly IUserService _userService;
		private readonly IDAOCounter _dAOCounter;

		public AuthService(IDAOUser daoUser, IUserService userService, IConfiguration config, ILogger<AuthService> logger, IDAOCounter dAOCounter)
		{
			_dAOUser = daoUser;
			_config = config;
			_logger = logger;
			_userService = userService;
			_dAOCounter = dAOCounter;
		}

		public async Task<LoginResponse> LoginAsync(LoginRequest request)
		{
			_logger.LogInformation(request.Phone);
			var user = await _dAOUser.GetByUserNameAsync(request.Phone);
			if (user == null)
				throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");

			bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
			if (!isValid)
				throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");

			var token = GenerateJwtToken(user);
			string? refreshToken = null;

			if (request.RememberLogin == true) refreshToken = GenerateRefreshToken(user);

			var expiresAt = DateTime.UtcNow.AddMinutes(
				_config.GetValue<int>("Jwt:ExpiresInMinutes"));


			return new LoginResponse { AccessToken = token, FullName = user.Fullname, Phone = user.Username, role = user.Role, RefreshToken = refreshToken };
		}
		public async Task<string> RegisterAsync(RegisterRequest request)
		{
			if (!Regex.IsMatch(request.Phone, @"^(03|05|07|08|09)\d{8}$"))
			{
				return "Số điện thoại không hợp lệ";
			}
			var next = await _dAOCounter.GetNextSequenceAsync("User");
			var userId = $"U{next:D6}";
			var user = new UserModel
			{
				Username = request.Phone,
				Password = request.Password,
				PhoneNumber = request.Phone,
				Fullname = request.Fullname,
				UserId = userId,
				CreatedAt = DateTime.Now
			};

			await _userService.createUser(user);
			return "Đăng ký thành công";
		}

		public async Task<bool> updateUser(ChangePasswordRequest request, string userId)
		{
			var updateDef = new List<UpdateDefinition<UserModel>>();

			var currentUser = await _dAOUser.GetByIdAsync(userId);
			if (currentUser == null)
				throw new BadRequestException("Người dùng không tồn tại");

			if (request.Password.Length < 6)
				throw new BadRequestException("Mật khẩu phải >= 6 ký tự");

			if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, currentUser.Password))
				throw new BadRequestException("Mật khẩu không đúng");
			updateDef.Add(Builders<UserModel>.Update.Set(
				x => x.Password,
				BCrypt.Net.BCrypt.HashPassword(request.Password)
			));
			if (updateDef.Count == 0)
				return false;

			var combinedUpdate = Builders<UserModel>.Update.Combine(updateDef);

			bool isSuccess = await _dAOUser.ModifyAsync(userId, combinedUpdate);

			return isSuccess;
		}
		public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
		{
			// Validate refresh token
			var principal = ValidateRefreshToken(refreshToken);
			if (principal == null)
				throw new UnauthorizedAccessException("Refresh token không hợp lệ hoặc đã hết hạn");
			var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
			_logger.LogInformation("userId from token: {userId}", userId); // thêm dòng này

			var user = await _dAOUser.GetByIdAsync(userId!);
			_logger.LogInformation("user found: {found}", user != null); // thêm dòng này

			if (user == null)
				throw new UnauthorizedAccessException("User không tồn tại");

			return new LoginResponse
			{
				AccessToken = GenerateJwtToken(user),
				FullName = user.Fullname,
				Phone = user.Username,
			};
		}
		public async Task<bool> HasCompanyPermission(string userId, CompanyPermission permission)
		{
			var user = await _dAOUser.GetByIdAsync(userId);

			if (user == null)
				return false;

			return user.CompanyPermissions.HasFlag(permission);
		}
		private ClaimsPrincipal? ValidateRefreshToken(string token)
		{
			try
			{
				var parameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!)),
					ValidateIssuer = true,
					ValidIssuer = _config["Jwt:Issuer"],
					ValidateAudience = true,
					ValidAudience = _config["Jwt:Audience"],
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				};

				var principal = new JwtSecurityTokenHandler()
					.ValidateToken(token, parameters, out _);

				// Kiểm tra đúng loại token
				if (principal.FindFirstValue("token_type") != "refresh")
					return null;

				return principal;
			}
			catch { return null; }
		}

		private string GenerateJwtToken(UserModel userModel)
		{
			var secretKey = _config["Jwt:SecretKey"]!;
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.UtcNow.AddMinutes(
				_config.GetValue<int>("Jwt:ExpiresInMinutes"));

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, userModel.UserId!),
				new Claim(JwtRegisteredClaimNames.UniqueName, userModel.Username),
				new Claim("role", userModel!.Role.ToString()?? "Customer"),

			};
			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		private string GenerateRefreshToken(UserModel userModel)
		{
			var secretKey = _config["Jwt:SecretKey"]!;
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, userModel.Id!),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("token_type", "refresh") // ← điểm khác biệt quan trọng
			};

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:MobileExpiresInDays")),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
