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
		private readonly IDAOCounter _dAOCounter;

		public UserService(ILogger<UserService> logger, IDAOUser dAOUser, IDAOCounter dAOCounter)
		{
			_logger = logger;
			_dAOUser = dAOUser;
			_dAOCounter = dAOCounter;
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
			var next = await _dAOCounter.GetNextSequenceAsync("User");
			var userId = $"U{next:D6}";
			var user = new UserModel
			{
				UserId = userId,
				Username = request.Username,
				Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
				PhoneNumber = request.PhoneNumber,
				Email = request.Email,
				Fullname = userFullName,
				CreatedAt = DateTime.Now,
			};

			await _dAOUser.CreateAsync(user);
		}

		public async Task<bool> updateUser(UserModel request)
		{
			var updateDef = new List<UpdateDefinition<UserModel>>();

			var errors = new Dictionary<string, string>();

			var currentUser = await _dAOUser.GetByIdAsync(request.UserId);
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

			if (await _dAOUser.ExistsAsync(x => x.Username == request.Username && x.UserId != request.UserId))
			{
				errors["UserName"] = "userName đã tồn tại";
			}
			if (await _dAOUser.ExistsAsync(x => x.Email == request.Email && x.UserId != request.UserId))
			{
				errors["Email"] = "email đã tồn tại";
			}
			if (await _dAOUser.ExistsAsync(x => x.PhoneNumber == request.PhoneNumber && x.UserId != request.UserId))
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





			if (updateDef.Count == 0)
				return false;

			var combinedUpdate = Builders<UserModel>.Update.Combine(updateDef);

			bool isSuccess = await _dAOUser.ModifyAsync(request.UserId, combinedUpdate);

			_logger.LogInformation("check " + isSuccess);

			return isSuccess;
		}
		public async Task<bool> AddSharedAccesses(SharedAccessInfo request)
		{
			var updateDef = new List<UpdateDefinition<UserModel>>();

			var errors = new Dictionary<string, string>();

			var currentUser = await _dAOUser.GetByIdAsync(request.ownerId);
			if (currentUser == null)
				throw new BadRequestException("Người dùng không tồn tại");


			if (request.scope == Scope.ACCOUNT)
			{
				var share = new SharedAccessInfo
				{
					shareId = Guid.NewGuid().ToString(),
					ownerId = request.ownerId,
					scope = Scope.ACCOUNT,
					deviceIds = null,
					permissions = request.permissions,
					//shareStatus = ShareAcessStatus.Active,
					//role = Role.User,
					createdAt = DateTime.Now,
					updatedAt = DateTime.Now
				};

				var update = Builders<UserModel>.Update.Push(
					x => x.SharedAccesses,
					share);
			}

			var share1 = new SharedAccessInfo
			{
				shareId = Guid.NewGuid().ToString(),
				ownerId = request.ownerId,
				scope = Scope.DEVICE,
				deviceIds = request.deviceIds,
				permissions = request.permissions,
				//shareStatus = ShareAcessStatus.Active,
				//role = Role.User,
				createdAt = DateTime.Now,
				updatedAt = DateTime.Now
			};

			var update1 = Builders<UserModel>.Update.Push(
				x => x.SharedAccesses,
				share1);




			if (updateDef.Count == 0)
				return false;

			var combinedUpdate = Builders<UserModel>.Update.Combine(updateDef);

			bool isSuccess = await _dAOUser.ModifyAsync(request.ownerId, combinedUpdate);

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
