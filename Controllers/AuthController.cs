using System.Security.Claims;
using HMS_NewProject_Temp_Humdity.DTO;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using static HMS_NewProject_Temp_Humdity.DTO.AuthDTO;
namespace HMS_NewProject_Temp_Humdity.Controllers
{
	[ApiController]
	[Route("auth")]
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var result = await _authService.LoginAsync(request);
			return StatusCode(200, new ApiResponse<object>
			{
				Success = true,
				Message = "đăng nhập thành công",
				Data = result
			});
		}

		[HttpPost("register")]
		public async Task<IActionResult> register([FromBody] RegisterRequest request)
		{
			var result = await _authService.RegisterAsync(request);
			return StatusCode(201, new ApiResponse<object>
			{
				Success = true,
				Message = "đăng ký thành công",
				Data = null
			});
		}

		[HttpPost("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
				return Unauthorized();
			var result = await _authService.updateUser(request, userId);
			return Ok(new ApiResponse<object>
			{
				Success = result,
				Message = result ? "Đổi mật khẩu thành công" : "Đổi mật khẩu thất bại",
				Data = null
			});
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			var result = await _authService.RefreshTokenAsync(request.RefreshToken);
			return Ok(result);
		}
	}
}
