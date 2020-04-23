using System.Data.OleDb;
using System.Data.Common;
using System.Collections.Generic;

using ReportsLibrary.Db;
using ReportsLibrary.Entities.External;

namespace ReportsLibrary.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы цехов предприятия [WorkGuild]
	/// </summary>
	public static class WorkGuildsStorage
	{
		/// <summary>
		/// Получение коллекции [Цехов предприятия]
		/// </summary>
		public static List<WorkGuild> GetAll()
		{
			var dbFolder = Properties.Settings.Default.FoxproDbFolder_Base;
			const string query = "SELECT DISTINCT kc, kcname FROM [fspodraz] WHERE kc <> 98";

			var workGuilds = new List<WorkGuild>();
			try
			{
				using (var connection = DbControl.GetConnection(dbFolder))
				{
					connection.TryConnectOpen();
					// Проверки наличия установленных кодировок в DBF-файлах и проверки соединений с этими файлами
					connection.VerifyInstalledEncoding("fspodraz");

					using (var oleDbCommand = new OleDbCommand(query, connection))
					{
						using (var reader = oleDbCommand.ExecuteReader())
						{
							while (reader != null && reader.Read())
							{
								var id = reader.GetDecimal(0);
								var name = reader.GetString(1).Trim();
								var workGuild = new WorkGuild { Id = id, Name = name };
								workGuilds.Add(workGuild);
							}
						}
					}
				}
				return workGuilds;
			}
			catch (DbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}

