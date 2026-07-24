using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS_Temp_Humdity_ApiManager.Models
{
	public class DeviceModel
	{

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BindNever]
		public string? Id { get; set; }

		public int? DeviceId { get; set; }

		public string? Imei { get; set; }
		public string? CompanyId { get; set; } = string.Empty; // cty quan ly 

		public string? DeviceName { get; set; }

		public string? UserId { get; set; } // nguoi phu trach / nguoi so huu neu cty null

		public string? LocationId { get; set; } = string.Empty;
		public double TemperatureMin { get; set; }

		public double TemperatureMax { get; set; }

		public double HumidityMin { get; set; }

		public double HumidityMax { get; set; }
		public DateTime? TimeStamp { get; set; } = DateTime.Now;

		public bool IsActive { get; set; } = true;

		public List<Sensor> Sensors { get; set; } = new List<Sensor>();

	}
	public class Sensor
	{
		public string? NameSensor { get; set; }

	}
	public class DeviceRequestModel
	{
		public DeviceQueryType Type { get; set; }

		public string? UserId { get; set; }
		public string? LocationId { get; set; }

	}

	public class DeviceActionModel
	{
		public DeviceActionType actionType { get; set; }
		public required DeviceModel Info { get; set; }

	}
	public enum DeviceQueryType
	{
		GetAll = 1,
		GetByUserId = 2,
		GetDeviceAndLocationByUserId = 3,
		GetLocationDetail = 4

	}
	public enum DeviceActionType
	{
		Create = 1,
		Update = 2,
		Delete = 3,
		UserCreate = 4,
		UserConfig = 5,
	}


}
