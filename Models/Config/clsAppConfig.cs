namespace HMS_NewProject_Temp_Humdity.Models.Config
{
	public class clsAppConfig
	{
		public required SignalRConfig SignalR { get; set; }
	}

	public class SignalRConfig
	{
		public required string HubUrl { get; set; }
	}
}
