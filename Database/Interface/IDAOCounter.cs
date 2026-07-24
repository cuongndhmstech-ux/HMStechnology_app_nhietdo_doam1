namespace HMS_Temp_Humdity_ApiManager.Database.Interface
{
	public interface IDAOCounter
	{
		Task<long> GetNextSequenceAsync(string counterName);
	}
}
