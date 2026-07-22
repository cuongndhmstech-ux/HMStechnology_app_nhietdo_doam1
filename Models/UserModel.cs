using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS_NewProject_Temp_Humdity.Models
{

	public class UserModel
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BindNever]
		public string? Id { get; set; }
		public required string UserId { get; set; }
		public string? Fullname { get; set; }
		public string? CompanyId { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
		public required string PhoneNumber { get; set; }
		public string? Email { get; set; }

		public List<SharedAccessInfo>? SharedAccesses { get; set; }

		public CompanyPermission CompanyPermissions { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public DateTime UpdatedAt { get; set; } = DateTime.Now;

		public Role Role { get; set; }

	}
	public class SharedAccessInfo
	{
		public string? shareId { get; set; }

		public required string ownerId { get; set; }

		public Scope scope { get; set; }

		public List<string>? deviceIds { get; set; } // dùng khi scope == device

		public Permission permissions { get; set; }

		//public ShareAcessStatus shareStatus { get; set; }

		//public Role role { get; set; }

		public DateTime? createdAt { get; set; }

		public DateTime? updatedAt { get; set; }
	}

	[Flags]
	public enum CompanyPermission : long
	{
		None = 0,

		// Quản lý người dùng
		UserManage = 1L << 0,

		// Quản lý tất cả thiết bị
		DeviceManage = 1L << 1,

		// Quản lý tất cả vị trí
		LocationManage = 1L << 2,

		// Quản lý cảnh báo
		AlertManage = 1L << 3,

		// Xem Dashboard toàn công ty
		Dashboard = 1L << 4,

		// Quản lý công ty
		CompanyManage = 1L << 5,

		Full = UserManage
			 | DeviceManage
			 | LocationManage
			 | AlertManage
			 | Dashboard
			 | CompanyManage
	}

	[Flags]
	public enum Permission : long
	{
		None = 0,

		// ===== Quyền trong SharedAccessInfo =====
		DeviceManage = 1L << 0,   // Quản lý thiết bị (sửa/xóa/cấu hình/bật tắt)
		Dashboard = 1L << 1,   // Xem dữ liệu (nhiệt độ, độ ẩm, biểu đồ)
		AlertManage = 1L << 2,   // Quản lý cảnh báo

		SharedFull = DeviceManage | Dashboard | AlertManage,
	}
	public class UserRequestModel
	{
		public QueryType Type { get; set; }
		public string? Id { get; set; }

	}

	public class UserActionRequest
	{
		public UserActionType actionType { get; set; }

		public JsonElement Info { get; set; }
	}

	public enum QueryType
	{
		GetAll = 1,
		GetByCompanyId = 2,
		GetById = 3,
	}
	public enum UserActionType
	{
		Create = 1,
		Update = 2,
		Delete = 3,
		UserCreate = 4,
		UserUpdate = 5,
		UserDelete = 6,
	}
	public enum Scope
	{
		ACCOUNT,
		DEVICE
	}

	public enum ShareAcessStatus
	{
		ACTIVE,
		REVOKED,
		LEFT
	}

	public enum Role
	{
		Admin,
		Customer
	}



}
