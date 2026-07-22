using MongoDB.Bson.Serialization.Attributes;

namespace HMS_NewProject_Temp_Humdity.Models
{
	public class CounterModel
	{
		[BsonId]
		public string Id { get; set; } = null!;
		public long Sequence { get; set; }
	}
}
