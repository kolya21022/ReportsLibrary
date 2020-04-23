using System.Collections.Generic;
using System.Linq;
using ReportsLibrary.Entities.External;
using ReportsLibrary.Storages;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для классов организаций - [Company]
	/// </summary>
	public static class CompaniesService
	{
		/// <summary>
		/// Получение коллекции [Организаций]
		/// </summary>
		public static List<Company> GetAll()
		{
			var companies = CompaniesStorage.GetAll();
			var banks = BanksService.GetAll();

		    var result = companies
		        .Join(
		            banks,
		            e => e.Id,
		            o => o.CompanyId,
		            (e, o) => new Company
		            {
		                Id = e.Id,
		                Bank = o,
		                City = e.City,
                        CodeTerritory = e.CodeTerritory,
                        Name = e.Name,
                        Unp = e.Unp
		            });
			//foreach (var company in companies)
			//{
			//	foreach (var bank in banks)
			//	{
			//		if (company.Id != bank.CompanyId)
			//		{
			//			continue;
			//		}
			//		company.Bank = bank;
			//		break;
			//	}
			//}
		    return result.ToList();

		}
	}
}
