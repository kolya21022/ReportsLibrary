using System.Collections.Generic;

using ReportsLibrary.Entities.External;
using ReportsLibrary.Storages;

namespace ReportsLibrary.Services
{
	class ProductsService
	{
		/// <summary>
		/// Получение коллекции [Изделий/продукций/услуг]
		/// </summary>
		public static List<Product> GetProductsOnlyName()
		{
			var products = ProductsStorage.GetProductsOnlyName();

			return products;
		}
	}
}
