namespace HMS_NewProject_Temp_Humdity.Database.Interface
{
	public interface IDAOCounter
	{
		Task<long> GetNextSequenceAsync(string counterName);
	}
}
