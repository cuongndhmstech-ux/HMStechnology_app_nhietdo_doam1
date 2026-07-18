using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS_NewProject_Temp_Humdity.Models
{
	public class DeviceModel
	{

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BindNever]
		public string? Id { get; set; }

		public int? DeviceId { get; set; }

		public required string Imei { get; set; }

		public string? Name { get; set; }

		public required string UserId { get; set; }

		public required string LocationId { get; set; }

		public DateTime? TimeStamp { get; set; } = DateTime.Now;

		public double TemperatureMin { get; set; }

		public double TemperatureMax { get; set; }

		public int HumidityMin { get; set; }

		public int HumidityMax { get; set; }

		public bool IsActive { get; set; } = true;

	}

	public class DeviceRequestModel
	{
		public DeviceQueryType Type { get; set; }

		public string? UserId { get; set; }



	}

	public class DeviceActionModel
	{
		public DeviceActionType actionType { get; set; }
		public JsonElement Info { get; set; }
	}
	public enum DeviceQueryType
	{
		GetAll = 1,
		GetDeviceAndLocationByUserId = 3,

	}
	public enum DeviceActionType
	{
		Create = 1,
		Update = 2,
		Delete = 3,
		UserCreate = 4
	}


}
