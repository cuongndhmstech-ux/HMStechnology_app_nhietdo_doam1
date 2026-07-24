using System.Linq.Expressions;
using HMS_Temp_Humdity_ApiManager.Database.Interface;
using HMS_Temp_Humdity_ApiManager.Models;
using HMS_Temp_Humdity_ApiManager.Services.Interface;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_ApiManager.Services
{
	public class CompanyService : ICompanyService
	{
		private readonly IDAOCompany _dAOCompany;
		private readonly IDAOCounter _dAOCounter;
		//private readonly IHubDevice _hubDevice;
		public CompanyService(IDAOCompany dAOCompany, IDAOCounter dAOCounter)
		{
			_dAOCompany = dAOCompany;
			_dAOCounter = dAOCounter;
			//_hubDevice = hubDevice;
		}


		public async Task<List<CompanyModel>> getAll()
		{

			var builder = Builders<CompanyModel>.Filter;
			var filters = new List<FilterDefinition<CompanyModel>>();

			//filters.Add(builder.Eq(x => x.UserId, location.UserId));
			//if (location.Name != null)
			//	filters.Add(builder.Eq(x => x.Name, location.Name));

			var mongoFilter = filters.Any()
							? builder.And(filters)
							: FilterDefinition<CompanyModel>.Empty;

			return await _dAOCompany.GetAllAsync(mongoFilter);
		}

		public bool UpdateInfoLocation(CompanyModel request)
		{
			//if (!await _dAOCompany.ExistsAsync(x => x.CompanyId == request.CompanyId))
			//{
			//	throw new ResourceNotFoundException("Công ty không tồn tại");
			//}

			//// cập nhập từng trường
			//var updateDef = new List<UpdateDefinition<CompanyModel>>();
			//updateDef.Add(Builders<CompanyModel>.Update.Set(x => x.Name, request.Name));

			//var combinedUpdate = Builders<CompanyModel>.Update.Combine(updateDef);
			//bool isSuccess = await _dAOCompany.ModifyAsync(request.LocationId, combinedUpdate);
			// cái này xử lí redis bên process data
			//await _hubDevice.NotifyLocationUpdatedAsync(request);
			return false;
		}

		public async Task CreateCompany(CompanyModel comp)
		{
			var next = await _dAOCounter.GetNextSequenceAsync("Company");
			var companyId = $"C{next:D6}";
			var company = new CompanyModel
			{
				CompanyId = companyId,
				CompanyName = comp.CompanyName,
				Address = comp.Address,
				PhoneNumber = comp.PhoneNumber,
				Email = comp.Email,
				IsActive = true,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			await _dAOCompany.CreateAsync(company);
		}

		public Task<List<CompanyModel>> GetAllAsync(FilterDefinition<CompanyModel>? filter = null)
		{
			throw new NotImplementedException();
		}

		public Task CreateAsync(CompanyModel device)
		{
			throw new NotImplementedException();
		}

		public Task<bool> ModifyAsync(string id, UpdateDefinition<CompanyModel> updateDefinition)
		{
			throw new NotImplementedException();
		}

		public Task<bool> ExistsAsync(Expression<Func<CompanyModel, bool>> filter)
		{
			throw new NotImplementedException();
		}

		public Task<bool> DeleteAsync(string id)
		{
			throw new NotImplementedException();
		}

		//public async Task DeleteLocation(CompanyModel request)
		//{
		//	//var device = await _dAOLocation.GetByDeviceIdAsync(deviceId); // lấy trước khi xóa
		//	var deleted = await _dAOCompany.DeleteAsync(request.LocationId);
		//	if (!deleted) throw new ResourceNotFoundException("Device không tồn tại để xóa");
		//	// cái này xử lí redis bên process data
		//	//await _hubDevice.NotifyLocationDeletedAsync(request);

	}

}

