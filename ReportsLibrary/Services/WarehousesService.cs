using System.Collections.Generic;

using ReportsLibrary.Storages;
using ReportsLibrary.Entities.External;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса складов предприятия - [Warehouse]
	/// </summary>
	public static class WarehousesService
	{
		/// <summary>
		/// Получение коллекции [Складов предприятия]
		/// </summary>
		public static List<Warehouse> GetAll()
		{
			return WarehousesStorage.GetAll();
		}
	}
}
