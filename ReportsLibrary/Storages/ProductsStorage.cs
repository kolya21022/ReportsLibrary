using System.Collections.Generic;
using System.Data.OleDb;

using ReportsLibrary.Db;
using ReportsLibrary.Entities.External;

namespace ReportsLibrary.Storages
{
	/// <summary>
	/// Обработчик запросов хранилища данных для изделий/продукции/услуг [Products]
	/// </summary>
	public static class ProductsStorage
	{
		/// <summary>
		/// Тестирование соединения с таблицей 
		/// (для проверки перед последующей модификацией, ввиду отсутствия транзакций для этого ввида таблиц)
		/// </summary>
		public static void TestConnection(OleDbConnection oleDbConnection)
		{
			const string query = "SELECT * FROM [izdel]";
			using (var command = new OleDbCommand(query, oleDbConnection))
			{
				using (command.ExecuteReader()) { }
			}
		}

		/// <summary>
		/// Получение имен коллекции [Изделий/продукции/услуг]
		/// </summary>
		public static List<Product> GetProductsOnlyName()
		{
			var dbFolder = Properties.Settings.Default.FoxproDbFolder_Base;
			const string query = "SELECT kizd AS id, nizd AS name FROM [izdel]";

			var products = new List<Product>();
			try
			{
				using (var connection = DbControl.GetConnection(dbFolder))
				{
					connection.TryConnectOpen();

					// Проверки наличия установленных кодировок в DBF-файлах и проверки соединений с этими файлами
					connection.VerifyInstalledEncoding("izdel");
					using (var oleDbCommand = new OleDbCommand(query, connection))
					{
						using (var reader = oleDbCommand.ExecuteReader())
						{
							while (reader != null && reader.Read())
							{
								var id = reader.GetDecimal(0);
								var name = reader.GetString(1).Trim();

								var product = new Product
								{
									Id = id,
									Name = name
								};
								products.Add(product);
							}
						}
					}
				}
				return products;
			}
			catch (OleDbException ex)
			{
				throw DbControl.HandleKnownDbFoxProAndMssqlServerExceptions(ex);
			}
		}
	}
}

