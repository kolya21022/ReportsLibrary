using System.Collections.Generic;

using ReportsLibrary.Entities.External;
using ReportsLibrary.Storages;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса [Банк]
	/// </summary>
	public class BanksService
	{
		/// <summary>
		/// Получение коллекции [Банк]
		/// </summary>
		public static List<Bank> GetAll()
		{
			return BanksStorage.GetAll();
		}
	}
}

