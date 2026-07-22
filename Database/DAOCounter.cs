using HMS_NewProject_Temp_Humdity.Database.Interface;
using HMS_NewProject_Temp_Humdity.Models;
using MongoDB.Driver;

namespace HMS_NewProject_Temp_Humdity.Database
{
	public class DAOCounter : IDAOCounter
	{
		private readonly IMongoCollection<CounterModel> _collection;

		public DAOCounter(IMongoDatabase database)
		{
			_collection = database.GetCollection<CounterModel>("Counters");
		}

		public async Task<long> GetNextSequenceAsync(string counterName)
		{
			var filter = Builders<CounterModel>.Filter.Eq(x => x.Id, counterName);

			var update = Builders<CounterModel>.Update.Inc(x => x.Sequence, 1);

			var options = new FindOneAndUpdateOptions<CounterModel>
			{
				IsUpsert = true,
				ReturnDocument = ReturnDocument.After
			};

			var counter = await _collection.FindOneAndUpdateAsync(
				filter,
				update,
				options);

			return counter.Sequence;
		}
	}
}
