using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS_NewProject_Temp_Humdity.Models
{
	public class CompanyModel
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BindNever]
		public string? Id { get; set; }

		public required string CompanyId { get; set; }

		public required string CompanyName { get; set; }

		public string? Address { get; set; }

		public string? PhoneNumber { get; set; }

		public string? Email { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}

	public class CompanyActionModel
	{
		public DeviceActionType actionType { get; set; }
		public JsonElement Info { get; set; }
	}
	public enum CompanyQueryType
	{
		GetAll = 1,
		GetDeviceAndLocationByUserId = 3,

	}
	public enum CompanyActionType
	{
		Create = 1,
		Update = 2,
		Delete = 3,
		UserCreate = 4
	}
}
