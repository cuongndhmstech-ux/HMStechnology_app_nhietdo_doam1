using System.Security.Claims;
using System.Text.Json;
using HMS_Temp_Humdity_ApiManager.DTO;
using HMS_Temp_Humdity_ApiManager.Models;
using HMS_Temp_Humdity_ApiManager.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS_Temp_Humdity_ApiManager.Controllers
{
	[ApiController]
	[Route("company")]
	public class CompanyController : Controller
	{
		private readonly ICompanyService? _conmpanyService;
		private readonly IAuthService _authService;
		public CompanyController(ICompanyService companyService, IAuthService authService)
		{
			_conmpanyService = companyService;
			_authService = authService;
		}

		//public async Task<IActionResult> Query([FromBody] DeviceRequestModel request)
		//{
		//	switch (request.Type)
		//	{
		//		case DeviceQueryType.GetDeviceAndLocationByUserId:
		//			{
		//				return StatusCode(201, new ApiResponse<object>
		//				{

		//					Success = true,
		//					Message = "Lấy dánh sách thành công",
		//					Data = await _conmpanyService.GetAllDeviceAndLocation()
		//				});
		//			}

		//		default:
		//			return BadRequest("Invalid action");
		//	};

		//}
		[HttpPost("update")]
		[Authorize]
		public async Task<IActionResult> Update([FromBody] CompanyActionModel model)
		{
			switch (model.actionType)
			{
				case DeviceActionType.Create:
					{
						var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
						if (!await _authService.HasCompanyPermission(userId, CompanyPermission.CompanyManage))
						{
							return StatusCode(403, new ApiResponse<object>
							{
								Success = false,
								Message = "Tài khoản bạn Không đủ quyền",
								Data = null
							});
						}
						var dto = JsonSerializer.Deserialize<CompanyModel>(
							model.Info,
							new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

						if (dto == null)
							throw new ArgumentNullException();
						await _conmpanyService.CreateCompany(dto);

						return StatusCode(201, new ApiResponse<object>
						{
							Success = true,
							Message = "Tạo mới thành công",
							Data = null
						});
					}
				//case DeviceActionType.Update:
				//	{
				//		var dto = JsonSerializer.Deserialize<DeviceModel>(
				//									model.Info,
				//									new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				//		if (dto == null)
				//			return BadRequest("Invalid data");

				//		await _deviceService.UpdateInfoDevice(dto);

				//		return Ok(new ApiResponse<object>
				//		{
				//			Success = true,
				//			Message = "cập nhập  thành công",
				//			Data = null
				//		});
				//	}
				//case DeviceActionType.Delete:
				//	{
				//		var dto = JsonSerializer.Deserialize<int>(
				//			model.Info,
				//			new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				//		if (dto == null)
				//			return BadRequest("Invalid data");
				//		await _deviceService.DeleteDevice(dto);

				//		return Ok(new ApiResponse<object>
				//		{
				//			Success = true,
				//			Message = "Xóa thành công",
				//			Data = null
				//		});
				//	}
				//case DeviceActionType.UserCreate:
				//	{

				//		var dto = JsonSerializer.Deserialize<DeviceModel>(
				//			model.Info,
				//			new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				//		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				//		dto.UserId = userId;

				//		if (dto == null)
				//			return BadRequest("Invalid data");
				//		await _deviceService.CreateDevice(dto);

				//		return StatusCode(201, new ApiResponse<object>
				//		{
				//			Success = true,
				//			Message = "Tạo mới thành công",
				//			Data = null
				//		});
				//	}
				default:
					return BadRequest("Invalid action");
			}
		}
	}
}
