using System.Security.Claims;
using System.Text.Json;
using HMS_NewProject_Temp_Humdity.DTO;
using HMS_NewProject_Temp_Humdity.Middleware;
using HMS_NewProject_Temp_Humdity.Models;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS_NewProject_Temp_Humdity.Controllers
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

                        // Gọi hàm Service đã sửa ở trên
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
				default:
					return BadRequest("Invalid action");
			}
		}
	}
}
