namespace HMS_Temp_Humdity_ApiManager.BaseException
{
	public class DuplicateResourceException : Exception
	{
		public DuplicateResourceException(string message) : base(message) { }

		public Dictionary<string, string> Errors { get; set; }

		public DuplicateResourceException(Dictionary<string, string> errors)
		{
			Errors = errors;

		}
	}
}
