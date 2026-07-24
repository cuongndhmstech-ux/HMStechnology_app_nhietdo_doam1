using HMS_Temp_Humdity_ApiManager.Models;

namespace HMS_Temp_Humdity_ApiManager.Services.Interface
{
	public interface ICompanyService
	{
		Task CreateCompany(CompanyModel comp);
	}
}
