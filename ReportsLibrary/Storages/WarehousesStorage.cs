using System.Data.OleDb;
using System.Collections.Generic;

using ReportsLibrary.Db;
using ReportsLibrary.Entities.External;

namespace ReportsLibrary.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы складов предприятия [Warehouse]
	/// </summary>
	public static class WarehousesStorage
	{
		/// <summary>
		/// Получение коллекции [Складов предприятия]
		/// </summary>
		public static List<Warehouse> GetAll()
		{
			var dbFolder = Properties.Settings.Default.FoxproDbFolder_Base;
			const string query = "SELECT sklad AS id, nsklad AS name FROM [sklad]";

			var warehouses = new List<Warehouse>();
			try
			{
				using (var connection = DbControl.GetConnection(dbFolder))
				{
					connection.TryConnectOpen();
					// Проверки наличия установленных кодировок в DBF-файлах и проверки соединений с этими файлами
					connection.VerifyInstalledEncoding("sklad");

					using (var oleDbCommand = new OleDbCommand(query, connection))
					{
						using (var reader = oleDbCommand.ExecuteReader())
						{
							while (reader != null && reader.Read())
							{
								var id = reader.GetDecimal(0);
								var name = reader.GetString(1).Trim();
								var warehouse = new Warehouse { Id = id, Name = name };
								warehouses.Add(warehouse);
							}
						}
					}
				}
				return warehouses;
			}
			catch (OleDbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}

