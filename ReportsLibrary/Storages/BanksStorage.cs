using System.Collections.Generic;
using System.Data.OleDb;

using ReportsLibrary.Db;
using ReportsLibrary.Entities.External;

namespace ReportsLibrary.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для таблицы банков [Bank]
	/// </summary>
	public class BanksStorage
	{
		/// <summary>
		/// Получение коллекции [Банк]
		/// </summary>
		public static List<Bank> GetAll()
		{
			var dbFolder = Properties.Settings.Default.FoxproDbFolder_Base;
			const string query = "SELECT kpotr AS companyId, naimb AS name, mfo, rsch as currentAccount " +
			                     "FROM [pot_bank]";
			var banks = new List<Bank>();
			try
			{
				using (var connection = DbControl.GetConnection(dbFolder))
				{
					connection.TryConnectOpen();
					connection.VerifyInstalledEncoding("pot_bank");

					using (var oleDbCommand = new OleDbCommand(query, connection))
					{
						using (var reader = oleDbCommand.ExecuteReader())
						{
							while (reader != null && reader.Read())
							{
								var companyId = reader.GetDecimal(0);
								var name = reader.GetString(1).Trim();
								var mfo = reader.GetString(2).Trim();
								var currentAccount = reader.GetString(3).Trim();
								var bank = new Bank
								{
									CompanyId = companyId,
									Name = name,
									Mfo = mfo,
									CurrentAccount = currentAccount
								};
								banks.Add(bank);
							}
						}
					}
				}
				return banks;
			}
			catch (OleDbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}

