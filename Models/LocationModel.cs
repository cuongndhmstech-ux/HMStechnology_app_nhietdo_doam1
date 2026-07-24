using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS_Temp_Humdity_ApiManager.Models
{
	public class LocationModel
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BindNever]
		public string? Id { get; set; }
		public string? LocationId { get; set; }

		public string? Name { get; set; }

		// nếu là cá nhân thì UserId có giá trị
		public string? UserId { get; set; }

		// nếu là doanh nghiệp thì CompanyId có giá trị
		public string? CompanyId { get; set; }

		public DateTime? CreatedAt { get; set; } = DateTime.Now;
	}
	public class LocationActionRequest
	{
		public UserActionType actionType { get; set; }

		public LocationModel? Info { get; set; }
	}


	public class LocationResponse
	{
		public required string LocationId { get; set; }
		public string? UserId { get; set; }

		public string? Name { get; set; }
		public List<DeviceModel>? Devices { get; set; }
	}

	public class clsLocationDetailModel
	{
		public string LocationId { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		//danh sách thiết bị trong phòng
		public List<DeviceModel> AssignedDevices { get; set; } = new();
		//danh sách thiết bị chưa được gán
		public List<DeviceModel> AvailableDevices { get; set; } = new();
	}
}
