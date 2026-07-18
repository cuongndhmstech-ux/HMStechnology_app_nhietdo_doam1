using HMS_NewProject_Temp_Humdity.BaseException;
using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Models;
using HMS_NewProject_Temp_Humdity.Services.Interface;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Services
{
	public class UserService : IUserService
	{
		private readonly ILogger<UserService> _logger;
		private readonly IDAOUser _dAOUser;
		public UserService(ILogger<UserService> logger, IDAOUser dAOUser)
		{
			_logger = logger;
			_dAOUser = dAOUser;
		}

		public async Task<List<UserModel>> getUsers()
		{
			var users = await _dAOUser.GetAllAsync();
			return users;
		}


		public async Task createUser(UserModel request)
		{
			var errors = new Dictionary<string, string>();

			if (await _dAOUser.ExistsAsync(x => x.Username == request.Username))
			{
				errors["Username"] = "Tên đăng nhập đã tồn tại";
			}
			if (!string.IsNullOrWhiteSpace(request.Email) && await _dAOUser.ExistsAsync(x => x.Email == request.Email))
			{
				errors["Email"] = "Email đã tồn tại";
			}
			if (await _dAOUser.ExistsAsync(x => x.PhoneNumber == request.PhoneNumber))
			{
				errors["Phone"] = "số đã tồn tại";
			}
			if (errors.Any())
			{
				throw new DuplicateResourceException(errors);
			}

			string userFullName = request.Fullname ?? "user_" + Guid.NewGuid().ToString("n").Substring(0, 8);
			var randomCode = new Random().Next(100000, 999999); // Sinh số từ 100000 đến 999999
			var user = new UserModel
			{
				userId = $"U{randomCode}",
				Username = request.Username,
				Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
				PhoneNumber = request.PhoneNumber,
				Email = request.Email,
				Fullname = userFullName,
				CreatedAt = DateTime.Now,
				Permissions = 0
			};

			await _dAOUser.CreateAsync(user);
		}

		public async Task<bool> updateUser(UserModel request)
		{
			var updateDef = new List<UpdateDefinition<UserModel>>();

			var errors = new Dictionary<string, string>();

			var currentUser = await _dAOUser.GetByIdAsync(request.userId);
			if (currentUser == null)
				throw new BadRequestException("Người dùng không tồn tại");
			// ----  cấp tài khoản
			if (!string.IsNullOrEmpty(request.Password))
			{
				if (request.Password.Length < 6)
					throw new BadRequestException("Mật khẩu phải >= 6 ký tự");
				updateDef.Add(Builders<UserModel>.Update.Set(
					x => x.Password,
					BCrypt.Net.BCrypt.HashPassword(request.Password)
				));
			}

			if (await _dAOUser.ExistsAsync(x => x.Username == request.Username && x.userId != request.userId))
			{
				errors["UserName"] = "userName đã tồn tại";
			}
			if (await _dAOUser.ExistsAsync(x => x.Email == request.Email && x.userId != request.userId))
			{
				errors["Email"] = "email đã tồn tại";
			}
			if (await _dAOUser.ExistsAsync(x => x.PhoneNumber == request.PhoneNumber && x.userId != request.userId))
			{
				errors["Phone"] = "số đã tồn tại";
			}
			if (errors.Any())
			{
				throw new DuplicateResourceException(errors);
			}

			// ----  cập nhập
			if (!string.IsNullOrEmpty(request.Username))
				updateDef.Add(Builders<UserModel>.Update.Set(x => x.Username, request.Username));
			if (!string.IsNullOrEmpty(request.Fullname))
				updateDef.Add(Builders<UserModel>.Update.Set(x => x.Fullname, request.Fullname));

			if (!string.IsNullOrEmpty(request.Email))
				updateDef.Add(Builders<UserModel>.Update.Set(x => x.Email, request.Email));

			if (!string.IsNullOrEmpty(request.PhoneNumber))
				updateDef.Add(Builders<UserModel>.Update.Set(x => x.PhoneNumber, request.PhoneNumber));

			// thêm trường cho app
			updateDef.Add(Builders<UserModel>.Update.Set(x => x.UpdatedAt, DateTime.Now));

			// cấp quyền

			updateDef.Add(Builders<UserModel>.Update.Set(x => x.Permissions, request.Permissions));



			if (updateDef.Count == 0)
				return false;

			var combinedUpdate = Builders<UserModel>.Update.Combine(updateDef);

			bool isSuccess = await _dAOUser.ModifyAsync(request.userId, combinedUpdate);

			_logger.LogInformation("check " + isSuccess);

			return isSuccess;
		}

		public async Task deleteUser(string id)
		{
			var deleted = await _dAOUser.DeleteAsync(id);

			if (!deleted)
			{
				throw new ResourceNotFoundException("User không tồn tại để xóa");
			}
		}
	}
}
