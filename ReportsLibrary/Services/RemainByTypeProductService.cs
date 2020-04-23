using System;
using System.Collections.Generic;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Остатки по видам продукции]
	/// </summary>
	public class RemainByTypeProductService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;

		private const string QueryPrrasosWhithPnakl = "SELECT prrasos.kizd as prrasosProductId, " +
		                                              "prrasos.Nom_Pp AS prrasosNomPp, " +
		                                              "prrasos.kol as prrasosCount, " +
		                                              "prrasos.Kolsn as prrasosKolsn, " +
		                                              "prrasos.Nom_pn as prrasosNomPn, " +
		                                              "prrasos.Data as prrasosDateSupply, " +
		                                              "prrasos.Datar as prrasosDateR, " +
		                                              "pnakl.dataotg AS pnaklDateShipment " +
		                                              "FROM prrasos LEFT JOIN pnakl on prrasos.nom_pn = pnakl.nomdok";

		private const string QueryIzdelKat = "SELECT izdel.kizd as izdelProductId, izdel.nizd AS izdelProduct, " +
											 "izdel.vid as izdelTypeId, izdel.kat as izdelKatId, kat_prod.Nkat as katName " +
											 "FROM izdel LEFT JOIN kat_prod on izdel.kat = kat_prod.kat";


		private const string QueryVidProd = "SELECT vid_prod.vid as vidprodTypeId, vid_prod.nvid AS vidprodType " +
		                                    "FROM vid_prod ";

		private const string QueryCenaIzd = "SELECT cenaizd.kizd as cenaizdProductId, cenaizd.cena AS cenaizdCost, " +
		                                    "cenaizd.nom_pp AS cenaizdNomPp FROM cenaizd ";

		/// <summary>
		/// Логика формирование листа записей отчета [Остатки по видам продукции]
		/// </summary>
		public static List<RemainByTypeProduct> GetRemainByTypeProduct(DateTime loadDateTime, string type)
		{
			var dateRemain = loadDateTime.AddDays(-1);
			List<decimal> listTypeId = new List<decimal>();
			if (type == "Узлы")
			{
				listTypeId.Add(7M);
				listTypeId.Add(8M);
				listTypeId.Add(9M);
			}
			else if (type == "Металлорежущие станки")
			{
				listTypeId.Add(1M);
				listTypeId.Add(2M);
				listTypeId.Add(3M);
				listTypeId.Add(4M);
				listTypeId.Add(5M);
				listTypeId.Add(6M);
				listTypeId.Add(14M);
			}
			else if (type == "Деревообрабатывающие станки")
			{
				listTypeId.Add(10M);
				listTypeId.Add(11M);
			}
			else if (type == "ТНП")
			{
				listTypeId.Add(12M);
			}
			else if (type == "Прочая")
			{
				listTypeId.Add(0M);
			}

			var bufferClass = new List<ServiceClassRemainByTypeProduct>();

			var prrasosWhithPnakl = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				query: QueryPrrasosWhithPnakl,
				tableName: "prrasosWhithPnakl");

			foreach (var row in prrasosWhithPnakl.Select())
			{
				DateTime? dateR;
				var dateSupply = (DateTime) row["prrasosDateSupply"];
				var nomPn = (decimal) row["prrasosNomPn"];

				if (nomPn == 0)
				{
					var kolsn = (decimal) row["prrasosKolsn"];
					if (kolsn == 0)
					{
						dateR = null;
					}
					else
					{
						dateR = (DateTime) row["prrasosDateR"];
					}
				}
				else
				{
					dateR = (DateTime) row["pnaklDateShipment"];
				}

				if ((dateSupply < dateRemain && (dateR == null || dateR > dateRemain))
				    || (dateSupply >= dateRemain && dateSupply <= dateRemain && (dateR == null || dateR > dateRemain)))
				{
					var productId = (decimal) row["prrasosProductId"];
					var ordinalCost = (decimal) row["prrasosNomPp"];
					var count = (decimal) row["prrasosCount"];
					bufferClass.Add(new ServiceClassRemainByTypeProduct()
					{
						ProductId = productId,
						OrdinalCost = ordinalCost,
						Count = count
					});
				}
			}

			var izdel = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryIzdelKat, "izdel");

			foreach (var bcItem in bufferClass)
			{
				foreach (var row in izdel.Select())
				{
					if (bcItem.ProductId == (decimal) row["izdelProductId"])
					{
						bcItem.ProductName = ((string) row["izdelProduct"]).Trim();
						bcItem.TypeId = (decimal) row["izdelTypeId"];
						bcItem.KatId = (decimal)row["izdelKatId"];
						bcItem.Kat = ((string)row["katName"]).Trim();
						break;
					}
				}
			}

			var filterServerClass = new List<ServiceClassRemainByTypeProduct>();

			foreach (var bcItem in bufferClass)
			{
				foreach (var typeId in listTypeId)
				{
					if (type == "Узлы")
					{
						if (bcItem.TypeId == typeId 
						    || bcItem.KatId == 268 
						    || bcItem.KatId == 45 
						    || bcItem.ProductId == 5071)
						{
							filterServerClass.Add(bcItem);
							break;
						}
					} 
					else if (type == "Прочая")
					{
						if (bcItem.TypeId == typeId
						    && bcItem.KatId != 268
						    && bcItem.KatId != 45
						    && bcItem.ProductId != 5071)
						{
							filterServerClass.Add(bcItem);
							break;
						}
					}
					else if (bcItem.TypeId == typeId
					         && bcItem.ProductId != 5071)
					{
						filterServerClass.Add(bcItem);
						break;
					}

				}
			}

			var vidProd = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryVidProd, "vidProd");

			foreach (var fscItem in filterServerClass)
			{
				foreach (var row in vidProd.Select())
				{
					if (fscItem.TypeId == (decimal) row["vidprodTypeId"])
					{
						fscItem.Type = ((string) row["vidprodType"]).Trim();
						break;
					}
				}
			}

			var cenaizd = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryCenaIzd, "cenaizd");

			foreach (var fscItem in filterServerClass)
			{
				foreach (var row in cenaizd.Select())
				{
					if (fscItem.OrdinalCost == (decimal) row["cenaizdNomPp"] 
					    && fscItem.ProductId == (decimal)row["cenaizdProductId"])
					{
						fscItem.Cost = (decimal) row["cenaizdCost"];
						break;
					}
				}
			}

			var remainsByTypeProduct = new List<RemainByTypeProduct>();

			foreach (var fscItem in filterServerClass)
			{
				bool flag = false;
				foreach (var remainByTypeProduct in remainsByTypeProduct)
				{
					if (remainByTypeProduct.Cost == fscItem.Cost
					    && remainByTypeProduct.Name == fscItem.ProductName)
					{
						flag = true;
						remainByTypeProduct.Count += fscItem.Count;
						break;
					}
				}

				if (!flag)
				{
					string t = fscItem.TypeId == 0 ? fscItem.Kat : fscItem.Type;
					remainsByTypeProduct.Add(new RemainByTypeProduct()
					{
						Cost = fscItem.Cost,
						Count = fscItem.Count,
						Name = fscItem.ProductName,
						Type = t,
						TypeGroup = type
					});
				}

			}

			remainsByTypeProduct.Sort();

			return remainsByTypeProduct;
		}

		private class ServiceClassRemainByTypeProduct
		{
			public decimal ProductId { get; set; }
			public decimal OrdinalCost { get; set; }
			public decimal Cost { get; set; }
			public decimal Count { get; set; }
			public string ProductName { get; set; }
			public decimal TypeId { get; set; }
			public string Type { get; set; }
			public decimal KatId { get; set; }
			public string Kat { get; set; }
		}
	}
}