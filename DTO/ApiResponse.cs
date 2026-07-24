namespace HMS_Temp_Humdity_ApiManager.DTO
{
	public class ApiResponse<T>
	{
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
		public T? Data { get; set; }
	}
}
