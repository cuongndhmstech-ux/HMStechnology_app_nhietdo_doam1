using System.Security.Claims;
using System.Text.Json;
using HMS_NewProject_Temp_Humdity.DTO;
using HMS_NewProject_Temp_Humdity.Models;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS_NewProject_Temp_Humdity.Controllers
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
			var result = request.Type switch
			{
				QueryType.GetAll =>

					Ok(new ApiResponse<List<LocationModel>>
					{
						Success = true,
						Message = "Lấy hết danh sách người  thành công",
						Data = await _locationService.getAll()
					}),
				_ => null
			};
			if (result is null)
			{
				return BadRequest("Invalid query type");
			}
			return result;
		}

		[HttpPost("update")]
		public async Task<IActionResult> Update([FromBody] UserActionRequest request)
		{
			switch (request.actionType)
			{
				//case UserActionType.Create:
				//	{
				//		var dto = JsonSerializer.Deserialize<CompanyModel>(
				//			request.Info,
				//			new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
				//		if (dto == null)
				//			return BadRequest("Invalid data");
				//		await _locationService.CreateLocation(dto.LocationId, dto.Name, dto.UserId);

				//		return StatusCode(201, new ApiResponse<object>
				//		{
				//			Success = true,
				//			Message = "Tạo mới thành công",
				//			Data = null
				//		});
				//	}
				case UserActionType.Update:
					{
						var dto = JsonSerializer.Deserialize<LocationModel>(
													request.Info,
													new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
						var dto = JsonSerializer.Deserialize<LocationModel>(
							request.Info,
							new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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


						var dto = JsonSerializer.Deserialize<LocationModel>(
							request.Info,
							new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
						var dto = JsonSerializer.Deserialize<LocationModel>(
																			request.Info,
																			new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
						var dto = JsonSerializer.Deserialize<LocationModel>(
							request.Info,
							new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
