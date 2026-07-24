namespace HMS_Temp_Humdity_ApiManager.Models.Config
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
