using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Отгрузка]
	/// </summary>
	public class ShipmentService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
		private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPrrasosWhithPnaklPotr = "SELECT result.cenad_u as prrasosAccountingCost, " +
		                                                        "result.cenad as prrasosCost, " +
		                                                        "result.kol as prrasosCount, " +
		                                                        "pnakl.kval as pnaklValId, " +
		                                                        "pnakl.dataotg as pnaklDateShipment, " +
		                                                        "pnakl.kpotr as pnaklCompanyId, " +
		                                                        "potr.kpotr as potrCompanyId, " +
		                                                        "potr.kter as potrTerritoryId " +
		                                                        "FROM (SELECT kol, nom_pn, cenad, cenad_u FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                        "union all " +
																"SELECT kol, nom_pn, cenad, cenad_u FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                        "as result " +
																"LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
																"LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
		                                                        "WHERE dataotg >= ctod( '{0}' ) and dataotg <= ctod( '{1}' )";

		private static readonly string QueryPrrasosForReturnWithCenaIzd = "SELECT result.nom_dok as returnId, " +
		                                                                  "result.kizd as prrasosProductId, " +
		                                                                  "result.nom_pp as prrasosOrdinalCost, " +
		                                                                  "result.kol as prrasosCount, " +
		                                                                  "result.pr_v as isReturn, " +
		                                                                  "result.Datar as prrasosReturnDate, " +
		                                                                  "cenaizd.kizd as cenaizdProductId, " +
		                                                                  "cenaizd.cena as cenaizdCost, " + //используется если был возврат
		                                                                  "cenaizd.nom_pp as cenaizdOrdinalCost " +
		                                                                  "FROM (SELECT nom_dok, kizd,  kol, nom_pp, pr_v, datar FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                                  "union all " +
																		  "SELECT nom_dok, kizd,  kol, nom_pp, pr_v, datar FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                                  "as result " +
																		  "LEFT JOIN \"" + DbfPathBase + "cenaizd.dbf\" as cenaizd on cenaizd.kizd = result.kizd " +
		                                                                  "and cenaizd.nom_pp = result.nom_pp " +
		                                                                  "WHERE pr_v = 1 and datar >= ctod( '{0}' ) " +
		                                                                  "and datar <= ctod( '{1}' )";

		private const string QueryReturnPrrasosWhithPnakl = "SELECT pnakl.kpotr as pnaklCompanyId FROM prrasos " +
		                                                   "LEFT JOIN pnakl on prrasos.nom_pn = pnakl.nomdok " +
		                                                   "where prrasos.nom_v = {0}";

		private const string QueryPotr = "SELECT potr.kpotr as potrCompanyId, potr.kter as potrTerritoryId FROM potr ";


		private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
		                                    "kurs_val.kurs AS kursKurs, " +
		                                    "kurs_val.kval as kursValId " +
		                                    "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		/// <summary>
		/// Логика формирование листа записей отчета [Отгрузка]
		/// </summary>
		public static List<Shipment> GetShipments(string monthOrYear,
			DateTime loadDateTime)
		{
			DateTime endDate = loadDateTime.AddDays(-1);

			var startDate = monthOrYear == "m" ? Common.GetBeginOfMonthWithOffset(loadDateTime)
				: new DateTime(loadDateTime.Year, 1, 1);

			var shipments = new List<Shipment>();

			var sqlResultExportList = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPrrasosWhithPnaklPotr, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy")), "SqlResultExportList");

			var sqlResultReturnList = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPrrasosForReturnWithCenaIzd, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy")), "SqlResultExportList");

			DataTable kursVal = null;
			// Необходимо получать лист курсов валют только если год < 2018 (Увеличение производительности)
			if (startDate <= new DateTime(2017, 12, 31))
			{
				kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
					query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
					tableName: "RateVal");
			}

			foreach (var rowExport in sqlResultExportList.Select())
			{
				var date = (DateTime) rowExport["pnaklDateShipment"];
				var month = ConvertDateInMonth(date);

				var territoryId = ((string) rowExport["potrTerritoryId"]).Trim();
				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

				var accountingCost = (decimal) rowExport["prrasosAccountingCost"];
				var cost = (decimal) rowExport["prrasosCost"];
				var valId = (decimal) rowExport["pnaklValId"];

				// до 2018 в cenad хранилась цена в валюте
				if (date <= new DateTime(2017, 12, 31))
				{
					if (kursVal != null)
					{
						foreach (var rate in kursVal.Select())
						{
							if ((decimal) rate["kursValId"] == valId
							    && (DateTime) rate["kursDate"] == date)
							{
								var rateVal = (decimal) rate["kursKurs"];
								cost = cost * rateVal;
								break;
							}
						}
					}
				}

				// Деноминация
				if (date < new DateTime(2016, 7, 1))
				{
					accountingCost = accountingCost / 10000;
					cost = cost / 10000;
				}

				if (accountingCost == 0M)
				{
					accountingCost = cost;
				}

				var count = (decimal) rowExport["prrasosCount"];

				var marketAccountingCost = 0M;
				var sngAccountingCost = 0M;
				var russiaAccountingCost = 0M;
				var exportAccountingCost = 0M;

				var marketContractCost = 0M;
				var sngContractCost = 0M;
				var russiaContractCost = 0M;
				var exportContractCost = 0M;

				if (territoryIdTwo == "15")
				{
					marketAccountingCost = accountingCost * count;
					marketContractCost = cost * count;
				}
				else if (territoryIdTwo != "50" && territoryIdTwo != "42"
				                                && territoryIdTwo != "31" && territoryIdTwo != "26")
				{
					sngAccountingCost = accountingCost * count;
					sngContractCost = cost * count;
					if (territoryIdTwo == "11")
					{
						russiaAccountingCost = accountingCost * count;
						russiaContractCost = cost * count;
					}
				}
				else
				{
					exportAccountingCost = accountingCost * count;
					exportContractCost = cost * count;
				}

				// Флаг - находится ли строка в результ. выборке
				bool flag = false;
				foreach (var shipment in shipments)
				{
					if (shipment.Month == month)
					{
						flag = true;
						shipment.MarketAccountingCost += marketAccountingCost;
						shipment.SngAccountingCost += sngAccountingCost;
						shipment.RussiaAccountingCost += russiaAccountingCost;
						shipment.ExportAccountingCost += exportAccountingCost;
						shipment.MarketContractCost += marketContractCost;
						shipment.SngContractCost += sngContractCost;
						shipment.RussiaContractCost += russiaContractCost;
						shipment.ExportContractCost += exportContractCost;
						break;
					}
				}

				if (!flag)
				{
					shipments.Add(new Shipment()
					{
						IdMonth = date.Month,
						Month = month,
						MarketAccountingCost = marketAccountingCost,
						SngAccountingCost = sngAccountingCost,
						RussiaAccountingCost = russiaAccountingCost,
						ExportAccountingCost = exportAccountingCost,
						MarketContractCost = marketContractCost,
						SngContractCost = sngContractCost,
						RussiaContractCost = russiaContractCost,
						ExportContractCost = exportContractCost
					});
				}
			}

			var company = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryPotr, "Company");

			foreach (var rowReturn in sqlResultReturnList.Select()) 
			{
				var dateReturn = (DateTime)rowReturn["prrasosReturnDate"];
					var returnId = (decimal)rowReturn["returnId"];
					var countReturn = (-1) * (decimal)rowReturn["prrasosCount"];
					var cost = (decimal)rowReturn["cenaizdCost"];

					//Поиск в действующей базе компании которая сделала возврат
					var idCompany = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
						query: string.Format(QueryReturnPrrasosWhithPnakl, returnId),
						tableName: "IdCompany");

					DataTable seachcompany;
					if (idCompany.Rows.Count != 0)
					{
						seachcompany = DataTableHelper.JoinTwoDataTablesOnOneColumn(idCompany, "pnaklCompanyId",
							company, "potrCompanyId", 1);
					}
					else
					{
						//Поиск в архиве компании которая сделала возврат
						var idCompanyArhiv = DataTableHelper.LoadDataTableByQuery(DbfPathFsoArhiv,
							query: string.Format(QueryReturnPrrasosWhithPnakl, returnId),
							tableName: "IdCompany");
						seachcompany = DataTableHelper.JoinTwoDataTablesOnOneColumn(idCompanyArhiv, "pnaklCompanyId",
							company, "potrCompanyId", 1);
					}

					var month = ConvertDateInMonth(dateReturn);

					string territoryIdTwo = "00";
					foreach (var sc in seachcompany.Select())
					{
						var territoryId = ((string)sc["potrTerritoryId"]).Trim();
						territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
						break;
					}

					if (territoryIdTwo == "00")
						break;

					var marketAccountingCost = 0M;
					var sngAccountingCost = 0M;
					var russiaAccountingCost = 0M;
					var exportAccountingCost = 0M;

					var marketContractCost = 0M;
					var sngContractCost = 0M;
					var russiaContractCost = 0M;
					var exportContractCost = 0M;

					if (territoryIdTwo == "15")
					{
						marketAccountingCost = cost * countReturn;
						marketContractCost = cost * countReturn;
					}
					else if (territoryIdTwo != "50" && territoryIdTwo != "42"
						&& territoryIdTwo != "31" && territoryIdTwo != "26")
					{
						sngAccountingCost = cost * countReturn;
						sngContractCost = cost * countReturn;
						if (territoryIdTwo == "11")
						{
							russiaAccountingCost = cost * countReturn;
							russiaContractCost = cost * countReturn;
						}
					}
					else
					{
						exportAccountingCost = cost * countReturn;
						exportContractCost = cost * countReturn;
					}

					// Флаг - находится ли строка в результ. выборке
					bool flag = false;
					foreach (var shipment in shipments)
					{
						if (shipment.Month == month)
						{
							flag = true;
							shipment.MarketAccountingCost += marketAccountingCost;
							shipment.SngAccountingCost += sngAccountingCost;
							shipment.RussiaAccountingCost += russiaAccountingCost;
							shipment.ExportAccountingCost += exportAccountingCost;
							shipment.MarketContractCost += marketContractCost;
							shipment.SngContractCost += sngContractCost;
							shipment.RussiaContractCost += russiaContractCost;
							shipment.ExportContractCost += exportContractCost;
							break;
						}
					}

					if (!flag)
					{
						shipments.Add(new Shipment()
						{
							IdMonth = dateReturn.Month,
							Month = month,
							MarketAccountingCost = marketAccountingCost,
							SngAccountingCost = sngAccountingCost,
							RussiaAccountingCost = russiaAccountingCost,
							ExportAccountingCost = exportAccountingCost,
							MarketContractCost = marketContractCost,
							SngContractCost = sngContractCost,
							RussiaContractCost = russiaContractCost,
							ExportContractCost = exportContractCost
						});
					}

			}

			var summMarketAccountingCost = 0M;
			var summSngAccountingCost = 0M;
			var summRussiaAccountingCost = 0M;
			var summExportAccountingCost = 0M;

			var summMarketContractCost = 0M;
			var summSngContractCost = 0M;
			var summRussiaContractCost = 0M;
			var summExportContractCost = 0M;
			foreach (var shipment in shipments)
			{
				summMarketAccountingCost += shipment.MarketAccountingCost;
				summSngAccountingCost += shipment.SngAccountingCost;
				summRussiaAccountingCost += shipment.RussiaAccountingCost;
				summExportAccountingCost += shipment.ExportAccountingCost;

				summMarketContractCost += shipment.MarketContractCost;
				summSngContractCost += shipment.SngContractCost;
				summRussiaContractCost += shipment.RussiaContractCost;
				summExportContractCost += shipment.ExportContractCost;
			}

			shipments.Add(new Shipment()
			{
				IdMonth = 99,
				Month = "",
				MarketAccountingCost = summMarketAccountingCost,
				SngAccountingCost = summSngAccountingCost,
				RussiaAccountingCost = summRussiaAccountingCost,
				ExportAccountingCost = summExportAccountingCost,
				MarketContractCost = summMarketContractCost,
				SngContractCost = summSngContractCost,
				RussiaContractCost = summRussiaContractCost,
				ExportContractCost = summExportContractCost
			});

			shipments.Sort();

			return shipments;
		}

		/// <summary>
		/// Получение из даты месяц прописью
		/// </summary>
		private static string ConvertDateInMonth(DateTime date)
		{		
			return date.ToString("MMMM");
		}
	}
}
