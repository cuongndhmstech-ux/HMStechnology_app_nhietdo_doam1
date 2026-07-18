using System.Text.Json;
using HMS_NewProject_Temp_Humdity.DTO;
using HMS_NewProject_Temp_Humdity.Models;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HMS_NewProject_Temp_Humdity.Controllers
{
	[ApiController]
	[Route("user")]
	public class UserController : Controller
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("query")]
		public async Task<IActionResult> Query([FromBody] UserRequestModel request)
		{
			var result = request.Type switch
			{
				QueryType.GetAll =>

					Ok(new ApiResponse<List<UserModel>>
					{
						Success = true,
						Message = "Lấy hết danh sách người  thành công",
						Data = await _userService.getUsers()
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
				case UserActionType.Create:
					{
						var dto = JsonSerializer.Deserialize<UserModel>(
							request.Info,
							new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
						if (dto == null)
							return BadRequest("Invalid data");
						await _userService.createUser(dto);

						return StatusCode(201, new ApiResponse<object>
						{
							Success = true,
							Message = "Tạo mới thành công",
							Data = null
						});
					}
				case UserActionType.Update:
					{
						var dto = JsonSerializer.Deserialize<UserModel>(
													request.Info,
													new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

						if (!await _userService.updateUser(dto))
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
						var dto = JsonSerializer.Deserialize<string>(
							request.Info,
							new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

						if (dto == null)
							return BadRequest("Invalid data");

						await _userService.deleteUser(dto);

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
