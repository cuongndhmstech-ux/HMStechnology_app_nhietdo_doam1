using System.Security.Claims;
using HMS_Temp_Humdity_ApiManager.DTO;
using HMS_Temp_Humdity_ApiManager.Models;
using HMS_Temp_Humdity_ApiManager.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS_Temp_Humdity_ApiManager.Controllers
{

	[ApiController]
	[Route("device")]
	public class DeviceController : Controller
	{
		private readonly IDeviceService? _deviceService;

		public DeviceController(IDeviceService deviceService)
		{
			_deviceService = deviceService;
		}
		[HttpPost("query")]
		//[ApiKey]
		public async Task<IActionResult> Query([FromBody] DeviceRequestModel request)
		{
			switch (request.Type)
			{
				case DeviceQueryType.GetByUserId:
					{
						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

						var data = await _deviceService.GetDevicesByUserIdAsync(userId);
						return Ok(new ApiResponse<List<LocationResponse>>
						{
							Success = true,
							Message = "Lấy danh sách thiết bị và phòng thành công",
							Data = data
						});
					}

				case DeviceQueryType.GetDeviceAndLocationByUserId:
					{
						return StatusCode(201, new ApiResponse<object>
						{
							Success = true,
							Message = "Lấy dánh sách thành công",
							Data = await _deviceService.GetAllDeviceAndLocation()
						});
					}
				case DeviceQueryType.GetLocationDetail:
					{
						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
						if (string.IsNullOrEmpty(request.LocationId))
						{
							return BadRequest("Thiếu LocationId để xem chi tiết");
						}
						var data = await _deviceService.GetLocationDetailAsync(userId, request.LocationId);
						return Ok(new ApiResponse<clsLocationDetailModel>
						{
							Success = true,
							Message = "Lấy chi tiết phòng thành công",
							Data = data
						});
					}

				default:
					return BadRequest("Invalid action");
			};

		}
		[HttpPost("update")]
		[Authorize]
		public async Task<IActionResult> Update([FromBody] DeviceActionModel model)
		{
			switch (model.actionType)
			{
				case DeviceActionType.Create:
					{

						var dto = model.Info;

						if (dto == null)
							return BadRequest("Invalid data");
						await _deviceService.CreateDevice(dto);

						return StatusCode(201, new ApiResponse<object>
						{
							Success = true,
							Message = "Tạo mới thành công",
							Data = null
						});
					}
				case DeviceActionType.Update:
					{
						var dto = model.Info;
						if (dto == null)
							return BadRequest("Invalid data");

						await _deviceService.UpdateInfoDevice(dto);

						return Ok(new ApiResponse<object>
						{
							Success = true,
							Message = "cập nhập  thành công",
							Data = null
						});
					}
				case DeviceActionType.Delete:
					{
						var dto = model.Info;

						if (dto == null)
							return BadRequest("Invalid data");
						await _deviceService.DeleteDevice(dto.DeviceId.Value);

						return Ok(new ApiResponse<object>
						{
							Success = true,
							Message = "Xóa thành công",
							Data = null
						});
					}
				case DeviceActionType.UserCreate:
					{

						var dto = model.Info;
						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
						dto.UserId = userId;

						if (dto == null)
							return BadRequest("Invalid data");
						await _deviceService.CreateDevice(dto);

						return StatusCode(201, new ApiResponse<object>
						{
							Success = true,
							Message = "Tạo mới thành công",
							Data = null
						});
					}
				case DeviceActionType.UserConfig:
					{

						var dto = model.Info;
						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
						dto.UserId = userId;

						if (dto == null)
							return BadRequest("Invalid data");
						if (!await _deviceService.ConfigDevice(dto))
						{
							return StatusCode(200, new ApiResponse<object>
							{
								Success = true,
								Message = "thay đổi thành công",
								Data = null
							});
						}

						return StatusCode(400, new ApiResponse<object>
						{
							Success = false,
							Message = "Không thay đổi",
							Data = null
						});
					}
				default:
					return BadRequest("Invalid action");
			}
		}
	}
}
