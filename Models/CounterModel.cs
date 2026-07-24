using MongoDB.Bson.Serialization.Attributes;

namespace HMS_Temp_Humdity_ApiManager.Models
{
	public class CounterModel
	{
		[BsonId]
		public string Id { get; set; } = null!;
		public long Sequence { get; set; }
	}
}
