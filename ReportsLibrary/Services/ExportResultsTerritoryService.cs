using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;
// ReSharper disable UseFormatSpecifierInFormatString

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Расход итог по территориям]
	/// </summary>
	public class ExportResultsTerritoryService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
		private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPrrasosWhithPnaklIzdelKatVidProd = "SELECT result.kizd as prrasosProductId, " +
		                                                        "result.kol as prrasosCount, " +
		                                                        "result.cenad as prrasosCost, " +
		                                                        "result.nom_pn as prrasosExportId, " +
		                                                        "pnakl.kval as pnaklValId, " +
		                                                        "pnakl.stavkands as stavkands, " +
		                                                        "pnakl.dataRas as pnaklDateExport, " +
		                                                        "pnakl.dataotg as pnaklDateShipment, " +
		                                                        "pnakl.kpotr as pnaklCompanyId, " +
		                                                        "izdel.kizd as izdelProductId, " +
		                                                        "izdel.vid as izdelTypeId, " +
		                                                        "izdel.vid_stat as vidstat, " +
		                                                        "izdel.kat as izdelKatId, " +
		                                                        "kat_prod.Nkat as katName, " +
		                                                        "vid_prod.vid as vidprodTypeId, " +
		                                                        "vid_prod.nvid as vidprodType " +
		                                                        "FROM ( SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                        "union all " +
																"SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                        "as result " +
																"LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on pnakl.nomdok = result.nom_pn " +
																"LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on izdel.kizd = result.kizd    " +
																"LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on vid_prod.vid = izdel.vid  " +
																"LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat  " +
		                                                        "WHERE dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )";

		private static readonly string QueryPrrasosforReturnWithIzdelKatVidProd = "SELECT prrasos.kizd as prrasosProductId, " +
		                                                       "prrasos.nom_pp as prrasosOrdinalCost, " +
		                                                       "prrasos.kol as prrasosCount, " +
		                                                       "prrasos.datar as prrasosReturnDate, " +
		                                                       "izdel.kizd as izdelProductId, " +
		                                                       "izdel.vid as izdelTypeId, " +
		                                                       "izdel.vid_stat as vidstat, " +
		                                                       "izdel.kat as izdelKatId, " +
		                                                       "kat_prod.Nkat as katName, " +
		                                                       "vid_prod.vid as vidprodTypeId, " +
		                                                       "vid_prod.nvid as vidprodType " +
		                                                       "FROM ( SELECT kizd, kol, nom_pp, datar, pr_v FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                       "union all " +
															   "SELECT kizd, kol, nom_pp, datar, pr_v FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                       "as result " +
															   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on izdel.kizd = result.kizd  " +
															   "LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on vid_prod.vid = izdel.vid " +
															   "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat  " +
		                                                       "WHERE result.pr_v = 1 and result.datar >= ctod( '{0}' ) and result.datar <= ctod( '{1}' )";

		private const string QueryIzdelKatVidProd = "SELECT izdel.kizd as izdelProductId, " +
		                                            "izdel.vid as izdelTypeId, " +
		                                            "izdel.vid_stat AS vidstat, " +
		                                            "izdel.kat as izdelKatId, " +
		                                            "kat_prod.Nkat as katName, " +
		                                            "vid_prod.vid as vidprodTypeId, " +
		                                            "vid_prod.nvid AS vidprodType " +
		                                            "FROM izdel LEFT JOIN vid_prod on izdel.vid = vid_prod.vid " +
		                                            "LEFT JOIN kat_prod on izdel.kat = kat_prod.kat";

		private const string QueryPotr = "SELECT potr.kpotr as potrCompanyId, potr.kter as potrTerritoryId FROM potr ";

		private const string QueryTerr = "SELECT terr.kter as territoryId, terr.nter as territoryName FROM terr ";


		private const string QueryCenaIzd = "SELECT cenaizd.kizd as cenaizdProductId, " +
		                                    "cenaizd.cena AS cenaizdCost, " + //используется если был возврат
		                                    "cenaizd.nom_pp AS cenaizdOrdinalCost " +
		                                    "FROM cenaizd ";

		private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
		                                    "kurs_val.kurs AS kursKurs, " +
		                                    "kurs_val.kval as kursValId " +
		                                    "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		private const string QueryKursUsd = "SELECT kurs_val.data as kursDate, kurs_val.kurs AS kursKurs " +
		                                       "FROM kurs_val where kurs_val.kval = 1 and " +
		                                       "kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		/// <summary>
		/// Логика формирование листа записей отчета [Расход итоги по территориям]
		/// </summary>
		public static List<ExportResultsTerritory> GetExportResultsTerritory(DateTime startDate, DateTime endDate,
			string type)
		{
			var exportsResultsTerritory = new List<ExportResultsTerritory>();

			var listTypeId = ExportByTypeProductService.TypeList(type);

			var product = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryIzdelKatVidProd, "Product");
			var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

			var exportList = GetListForExport(startDate, endDate, listTypeId, product);
			var returnList = GetListForReturn(startDate, endDate, listTypeId, product);

			DataTable kursVal = null;
			// Необходимо получать лист курсов валют только если год < 2018 (Увеличение производительности)
			if (startDate <= new DateTime(2017, 12, 31))
			{
				kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
					query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
					tableName: "RateVal");
			}

			// Если не возврат
			foreach (var rowExport in exportList.Select())
			{
				var typeId = (decimal) rowExport["izdelTypeId"];
				var vidStatId = (decimal) rowExport["vidStat"];

				string typeProduct;
				var katProduct = ((string) rowExport["katName"]).Trim();

				// Доп условие, разбиение группы(приказ сверху)
				if (vidStatId == 18)
				{
					typeProduct = "токарные с ЧПУ";
				}
				else
				{
					typeProduct = typeId != 0 ? ((string) rowExport["vidprodType"]).Trim() : "";
				}

				string t = (typeId == 0 && vidStatId != 18) ? katProduct : typeProduct;

				var territoryName = string.Empty;
				var territoryId = ((string) rowExport["potrTerritoryId"]).Trim();
				// Из id города формируем id страны
				var territoryIdzero = territoryId.Substring(0, 1) + territoryId.Substring(1, 1) + "00";
				// Получение названия страны по сформированному id
				foreach (var rowTeritory in territories.Select())
				{

					if (((string) rowTeritory["territoryId"]).Trim() == territoryIdzero)
					{
						territoryName = ((string) rowTeritory["territoryName"]).Trim();
						break;
					}
				}

				var nds = (decimal)rowExport["stavkands"];
				var costOne = (decimal)rowExport["prrasosCost"];
				var rateUsdOnDate = rowExport["kursKurs"] == DBNull.Value ? 1 : (decimal)rowExport["kursKurs"];
				var costUsdOne = 0M;

				var date = (DateTime)rowExport["pnaklDateExport"];

				var valId = (decimal)rowExport["pnaklValId"];

				// до 2018 в cenad хранилась цена в валюте
				if (date <= new DateTime(2017, 12, 31))
				{
					if (kursVal != null)
					{
						foreach (var rate in kursVal.Select())
						{
							if ((decimal)rate["kursValId"] == valId
							    && (DateTime)rate["kursDate"] == date)
							{
								var rateVal = (decimal)rate["kursKurs"];
								costOne = costOne * rateVal;
								break;
							}
						}
						costUsdOne = decimal.Round(costOne / rateUsdOnDate,0);
					}
				}

				if (costUsdOne == 0M)
				{				
					costUsdOne = costOne / rateUsdOnDate;
				}

				// Деноминация
				if (date < new DateTime(2016, 7, 1))
				{
					costOne = costOne / 10000;
				}

				var summNdsOne = costOne * nds / 100;
				var count = (decimal)rowExport["prrasosCount"];

				// Флаг - находится ли строка в результ. выборке
				bool flag = false;
				foreach (var exportResultsTerritory in exportsResultsTerritory)
				{
					if (exportResultsTerritory.Territory == territoryName
					    && exportResultsTerritory.Type == t)
					{
						flag = true;
						exportResultsTerritory.Count += count;
						exportResultsTerritory.Cost += costOne * count;
						exportResultsTerritory.SummNds += summNdsOne * count;
						exportResultsTerritory.CostUsd += (costUsdOne + costUsdOne * nds / 100) * count;
						break;
					}
				}

				if (!flag)
				{
					exportsResultsTerritory.Add(new ExportResultsTerritory()
					{
						Count = count,
						Cost = costOne * count,
						SummNds = summNdsOne * count,
						CostUsd = (costUsdOne + costUsdOne * nds / 100) * count,
						Type = t,
						TypeGroup = type,
						Territory = territoryName,
					});
				}
			}

			foreach (var rowReturn in returnList.Select())
			{
				var typeId = (decimal) rowReturn["izdelTypeId"];
				var vidStatId = (decimal) rowReturn["vidStat"];

				string typeProduct;
				var katProduct = ((string) rowReturn["katName"]).Trim();

				// Доп условие, разбиение группы(приказ сверху)
				if (vidStatId == 18)
				{
					typeProduct = "токарные с ЧПУ";
				}
				else
				{
					typeProduct = typeId != 0 ? ((string) rowReturn["vidprodType"]).Trim() : "";
				}

				string t = (typeId == 0 && vidStatId != 18) ? katProduct : typeProduct;

				var costOne = (decimal)rowReturn["cenaizdCost"];
				var count = (-1) * (decimal) rowReturn["prrasosCount"];

				var rateUsdOnDate = rowReturn["kursKurs"] == DBNull.Value ? 1 : (decimal)rowReturn["kursKurs"];
				var costUsdOne = decimal.Round(costOne / rateUsdOnDate, 2);

				// Флаг - находится ли строка в результ. выборке
				bool flag = false;
				foreach (var exportResultsTerritory in exportsResultsTerritory)
				{
					if (exportResultsTerritory.Territory == string.Empty
					    && exportResultsTerritory.Type == t)
					{
						flag = true;
						exportResultsTerritory.Count += count;
						exportResultsTerritory.Cost += costOne * count;
						exportResultsTerritory.CostUsd += costUsdOne * count;
						break;
					}
				}

				if (!flag)
				{
					exportsResultsTerritory.Add(new ExportResultsTerritory()
					{
						Count = count,
						Cost = costOne * count,
						SummNds = 0,
						CostUsd = costUsdOne * count,
						Type = t,
						TypeGroup = type,
						Territory = string.Empty,
					});
				}
			}

			exportsResultsTerritory.Sort();

			return exportsResultsTerritory;
		}

		/// <summary>
		/// Логика формирование листа записей расхода отчета [Расход итог по территориям]
		/// </summary>
		/// 
		public static DataTable GetListForExport(DateTime startDate, DateTime endDate, List<decimal> listTypeId, 
			DataTable product)
		{ 
			var sqlResultprrasosWhithPnaklForExport = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPrrasosWhithPnaklIzdelKatVidProd, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy")), "SqlResultprrasosWhithPnaklForExport");

			var bufferDataTable = new DataTable();

			// Копирование столбцов в буферный DataTable
			foreach (DataColumn column in sqlResultprrasosWhithPnaklForExport.Columns)
			{
				if (bufferDataTable.Columns[column.ColumnName] == null)
				{
					bufferDataTable.Columns.Add(column.ColumnName, column.DataType);
				}
				else
				{
					throw new ApplicationException();
				}
			}

			foreach (var rowLink in sqlResultprrasosWhithPnaklForExport.Select())
			{
				var vidprodTypeId = (decimal) rowLink["izdelTypeId"];
				foreach (var typeId in listTypeId)
				{
					if (vidprodTypeId == typeId)
					{
						bufferDataTable.Rows.Add(rowLink.ItemArray);
						break;
					}
				}
			}

			var company = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryPotr, "Company");
			var linkWhithCompany = DataTableHelper.JoinTwoDataTablesOnOneColumn
					(bufferDataTable, "pnaklCompanyId", company, "potrCompanyId", 1);

			var kursUsd = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				query: string.Format(QueryKursUsd, startDate.ToString("MM/dd/yyyy")),
				tableName: "Rate");

			var result = DataTableHelper.JoinTwoDataTablesOnOneColumn
				(linkWhithCompany, "pnaklDateShipment", kursUsd, "kursDate", 1);

			return result;
		}

		/// <summary>
		/// Логика формирование листа записей возврата отчета [Расход итог по территориям]
		/// </summary>
		
		public static DataTable GetListForReturn(DateTime startDate, DateTime endDate, List<decimal> listTypeId,
			DataTable product)
		{
			var sqlResultPrrasosforReturn = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPrrasosforReturnWithIzdelKatVidProd, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy")), "SqlResultPrrasosforReturn");

			var bufferDataTable = new DataTable();

			// Копирование столбцов в буферный DataTable
			foreach (DataColumn column in sqlResultPrrasosforReturn.Columns)
			{
				if (bufferDataTable.Columns[column.ColumnName] == null)
				{
					bufferDataTable.Columns.Add(column.ColumnName, column.DataType);
				}
				else
				{
					throw new ApplicationException();
				}
			}
			foreach (var rowLink in sqlResultPrrasosforReturn.Select())
			{
				var vidprodTypeId = (decimal)rowLink["izdelTypeId"];
				foreach (var typeId in listTypeId)
				{
					if (vidprodTypeId == typeId)
					{
						bufferDataTable.Rows.Add(rowLink.ItemArray);
						break;
					}
				}
			}

			var costProduct = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryCenaIzd, "CostProduct");
			var linkWhithCostProduct = DataTableHelper.LeftJoin_TwoTable_By_TwoFields<decimal?, decimal?>(bufferDataTable,
				"prrasosProductId", "prrasosOrdinalCost", costProduct, "cenaizdProductId", "cenaizdOrdinalCost");

			var kursUsd = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				query: string.Format(QueryKursUsd, startDate.ToString("MM/dd/yyyy")),
				tableName: "Rate");

			var result = DataTableHelper.JoinTwoDataTablesOnOneColumn
				(linkWhithCostProduct, "prrasosReturnDate", kursUsd, "kursDate", 1);

			return result;

		}
		/// <summary>
        /// Формирование листа Id типов изделия для указанной группы
        /// </summary>
        
        public static List<decimal> TypeList(string type)
		{
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
			return listTypeId;
		}
	}
}
