namespace HMS_NewProject_Temp_Humdity.Models
{
	public class LocationModel
	{
		public string LocationId { get; set; } = null!;

		public string Name { get; set; } = null!;

		// nếu là cá nhân thì UserId có giá trị
		public string? UserId { get; set; }

		// nếu là doanh nghiệp thì CompanyId có giá trị
		public string? CompanyId { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}


	public class LocationResponse
	{
		public required string LocationId { get; set; }
		public string? UserId { get; set; }

		public string? Name { get; set; }
		public List<DeviceModel>? Devices { get; set; }
	}
}
