using System.Collections.Generic;

using ReportsLibrary.Storages;
using ReportsLibrary.Entities.External;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Обработчик сервисного слоя для класса цехов предприятия - [WorkGuild]
	/// </summary>
	public static class WorkGuildsService
	{
		/// <summary>
		/// Получение коллекции [Цехов предприятия]
		/// </summary>
		public static List<WorkGuild> GetAll()
		{
			return WorkGuildsStorage.GetAll();
		}
	}
}
