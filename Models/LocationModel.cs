using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS_NewProject_Temp_Humdity.Models
{
	public class LocationModel
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[BindNever]
		public string? Id { get; set; }

		public string? LocationId { get; set; }

		public string? UserId { get; set; }

		public string? Name { get; set; }

		public string? Description { get; set; }

	}


	public class LocationResponse
	{
		public required string LocationId { get; set; }
		public string? UserId { get; set; }

		public string? Name { get; set; }
		public List<DeviceModel>? Devices { get; set; }
	}
}
