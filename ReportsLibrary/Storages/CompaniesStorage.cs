using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;

using ReportsLibrary.Db;
using ReportsLibrary.Entities.External;

namespace ReportsLibrary.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для внешней таблицы организаций с указанным УНП/Городом [Company].
	/// Используемая база данных - стороннего приложения АРМ Поставщиков-Потребителей, редактирование запрещено.
	/// </summary>
	public static class CompaniesStorage
	{
		/// <summary>
		/// Получение коллекции [Организаций с указанным УНП/Городом]
		/// </summary>
		public static List<Company> GetAll()
		{
			var dbFolder = Properties.Settings.Default.FoxproDbFolder_Base;
			const string query = "SELECT * FROM [potr]";

			var companies = new List<Company>();
			try
			{
				using (var connection = DbControl.GetConnection(dbFolder))
				{
					connection.TryConnectOpen();
					connection.VerifyInstalledEncoding("potr");
					using (var oleDbCommand = new OleDbCommand(query, connection))
					{
						using (var reader = oleDbCommand.ExecuteReader())
						{
							while (reader != null && reader.Read())
							{
								var id = reader.GetDecimal(0);
								var name = reader.IsDBNull(2) ? null : reader.GetString(2).Trim();
								var city = reader.IsDBNull(1) ? null : reader.GetString(1).Trim();
								var codeTerritory = reader.IsDBNull(5) ? null : reader.GetString(5).Trim();
								var unp = reader.IsDBNull(4) ? null : reader.GetString(4).Trim();
								if (string.IsNullOrWhiteSpace(name))
								{
									continue;
								}
								var company = new Company
								{
									Id = id,
									Name = name,
									City = city,
									CodeTerritory = codeTerritory,
									Unp = unp
								};
								companies.Add(company);
							}
						}
					}
				}
				return companies;
			}
			catch (SqlException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}

