using System.Security.Claims;
using HMS_Temp_Humdity_ApiManager.DTO;
using HMS_Temp_Humdity_ApiManager.Models;
using HMS_Temp_Humdity_ApiManager.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS_Temp_Humdity_ApiManager.Controllers
{
	[ApiController]
	[Route("location")]
	[Authorize]
	public class LocationController : Controller
	{


		private readonly ILocationService _locationService;

		public LocationController(ILocationService locationService)
		{
			_locationService = locationService;
		}

		[HttpPost("query")]
		public async Task<IActionResult> Query([FromBody] UserRequestModel request)
		{
			switch (request.Type)
			{
				case QueryType.GetByUserId:
					{
						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
						var result = await _locationService.GetLocationByUserIdAsync(userId);
						if (!result.Success)
						{
							return StatusCode(500, result);
						}
						return Ok(result);
					}
				default:
					return BadRequest("Invalid action");

			}
		}

		[HttpPost("update")]
		public async Task<IActionResult> Update([FromBody] LocationActionRequest request)
		{
			switch (request.actionType)
			{
				case UserActionType.Create:
					{
						var dto = request.Info;
						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
						var result = await _locationService.CreateLocation(dto.Name, userId);
						if (!result.Success)
						{
							return BadRequest(result);
						}
						return Ok(result);

					}
				case UserActionType.Update:
					{
						var dto = request.Info;
						dto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
						if (!await _locationService.UpdateInfoLocation(dto))
						{
							return Ok(new ApiResponse<object>
							{
								Success = false,
								Message = "Không thể cập nhập",
								Data = null
							});
						}

						return Ok(new ApiResponse<object>
						{
							Success = true,
							Message = "cập nhập thành công",
							Data = null
						});
					}
				case UserActionType.Delete:
					{
						var dto = request.Info;
						if (dto == null)
							return BadRequest("Invalid data");

						await _locationService.DeleteLocation(dto);

						return Ok(new ApiResponse<object>
						{
							Success = true,
							Message = "Xóa thành công",
							Data = null
						});
					}
				case UserActionType.UserCreate:
					{
						var dto = request.Info;
						if (dto == null)
							return BadRequest("Invalid data");

						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
						dto.UserId = userId;
						await _locationService.CreateLocation(dto.Name, dto.UserId);

						return StatusCode(201, new ApiResponse<object>
						{
							Success = true,
							Message = "Tạo mới thành công",
							Data = null
						});
					}
				case UserActionType.UserUpdate:
					{
						var dto = request.Info;

						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

						dto.UserId = userId;
						if (!await _locationService.UpdateInfoLocation(dto))
						{
							return Ok(new ApiResponse<object>
							{
								Success = false,
								Message = "Không thể cập nhập",
								Data = null
							});
						}

						return Ok(new ApiResponse<object>
						{
							Success = true,
							Message = "cập nhập thành công",
							Data = null
						});
					}
				case UserActionType.UserDelete:
					{
						var dto = request.Info;

						if (dto == null)
							return BadRequest("Invalid data");
						var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

						dto.UserId = userId;
						await _locationService.DeleteLocation(dto);

						return Ok(new ApiResponse<object>
						{
							Success = true,
							Message = "Xóa thành công",
							Data = null
						});
					}
				default:
					return BadRequest("Invalid action");

			}
		}
	}
}
