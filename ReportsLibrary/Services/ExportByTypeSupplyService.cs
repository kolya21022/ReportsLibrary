using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Экспорт по виду поставки]
	/// TODO без возвратов
	/// </summary>
	public class ExportByTypeSupplyService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosPotrVidPostIzdelKursUsd = "SELECT result.kizd as prrasosProductId, " +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.cenad as prrasosCost, " +
		                                                   "result.cena_dol as prrasosCostUsd, " +
		                                                   "result.cena_val as prrasosCostVal, " +
		                                                   "pnakl.nomdok as pnaklExportId, " +
		                                                   "pnakl.dataras AS pnaklDateExport, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.stavkands as pnaklNds, " +
		                                                   "pnakl.kval as pnaklValId, " +
		                                                   "pnakl.postavka as pnaklTypeSupplyId,  " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "izdel.nizd AS izdelProduct, " +
		                                                   "izdel.pr_usl as izdelProvide, " +
		                                                   "vid_post.kod as vidPostTypeSupplyId, " +
		                                                   "vid_post.naim as typeSupplyName, " +
		                                                   "potr.kpotr as potrCompanyId, " +
		                                                   "potr.kter as potrTerritoryId, " +
		                                                   "potr.gorod as City, " +
		                                                   "potr.npotr as potrCompanyName, " +
		                                                   "kurs_val.data as kursDate, " +
		                                                   "kurs_val.kurs AS kursKurs " +
		                                                   "FROM (SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM  \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on pnakl.nomdok = result.nom_pn " +
		                                                   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on potr.kpotr  = pnakl.kpotr " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on izdel.kizd  = result.kizd  " +
														   "LEFT JOIN \"" + DbfPathFso + "vid_post.dbf\" as vid_post on vid_post.kod = pnakl.postavka " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on  kurs_val.data = pnakl.dataras " +
														   "WHERE (dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )) " +
														   "and (kurs_val.kval = 1 and kurs_val.data >= ctod( '{0}' )) " +
		                                                   "ORDER BY kurs_val.data DESC";

		private const string QueryTerr = "SELECT terr.kter as territoryId, terr.nter as territoryName FROM terr ";

	    private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
	                                        "kurs_val.kurs AS kursKurs, " +
	                                        "kurs_val.kval as kursValId " +
	                                        "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

        /// <summary>
        /// Логика формирование листа записей отчета [Экспорт по виду поставки] num = 1 в договорных, num = 2 в учетных
        /// </summary>
        public static List<ExportByTypeSupply> GetExportByTypeSupply(DateTime startDate, DateTime endDate, int num)
		{
		    Dictionary<decimal, decimal> exportWhithCostUsd = new Dictionary<decimal, decimal>();
            var exportsByTypeSupply = new List<ExportByTypeSupply>();

			var sqlResultPnaklPrrasos = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosPotrVidPostIzdelKursUsd, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy")), "SqlResultPnaklPrrasos");

			var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

			DataTable kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				    query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
				    tableName: "RateVal");

		    // Создание коллекции код расхода, цена в usd
		    foreach (var row in sqlResultPnaklPrrasos.Select())
		    {
		        var exportId = (decimal)row["pnaklExportId"];

		        var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
		        var valId = (decimal)row["pnaklValId"];
		        var cost = (decimal)row["prrasosCost"];
		        var costUsd = (decimal)row["prrasosCostUsd"];
		        var nds = (decimal)row["pnaklNds"];
		        var count = (decimal)row["prrasosCount"];

		        var date = (DateTime)row["pnaklDateExport"];

		        // до 2018 в cenad хранилась цена в валюте
		        if (date <= new DateTime(2017, 12, 31))
		        {
		            foreach (var rate in kursVal.Select())
		            {
		                if ((decimal)rate["kursValId"] == valId
		                    && (DateTime)rate["kursDate"] == date)
		                {
		                    var rateVal = (decimal)rate["kursKurs"];
		                    cost = cost * rateVal;
		                    break;
		                }
		            }
		            costUsd = cost / rateUsd;
		        }

		        var costUsdresult = (costUsd + costUsd * nds / 100) * count;

		        if (exportWhithCostUsd.ContainsKey(exportId))
		        {
		            exportWhithCostUsd[exportId] += costUsdresult;
		        }
		        else
		        {
		            exportWhithCostUsd.Add(exportId, costUsdresult);
		        }
		    }

            foreach (var row in sqlResultPnaklPrrasos.Select())
			{
			    var exportId = (decimal)row["pnaklExportId"];

			    // Если общая стоймость < 50$ - не учитывать 
			    if (exportWhithCostUsd[exportId] < 50M)
			    {
			        continue;
			    }

                var provide = (decimal)row["izdelProvide"];

				var territoryName = "";
				var territoryId = ((string) row["potrTerritoryId"]).Trim();

				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

				//Если экспорт в Беларусь или это услуга, то не учитывать
				if (territoryIdTwo == "15" || provide != 0)
				{
					continue;
				}
				// Из id города формируем id страны
				var territoryIdzero = territoryIdTwo + "00";

				// Получение названия страны по сформированному id
				foreach (var rowTeritory in territories.Select())
				{

					if (((string) rowTeritory["territoryId"]).Trim() == territoryIdzero)
					{
						territoryName = ((string) rowTeritory["territoryName"]).Trim();
						break;
					}
				}

                var typeSupplyName = ((string)row["typeSupplyName"]).Trim();
				var company = ((string)row["City"]).Trim() + ", " + ((string) row["potrCompanyName"]).Trim();
				var productName = ((string)row["izdelProduct"]).Trim();

				var count = (decimal)row["prrasosCount"];
			    var nds = (decimal)row["pnaklNds"];
                var costVal = (decimal)row["prrasosCostVal"];
				var cost = (decimal)row["prrasosCost"];
			    if (costVal == 0M)
			    {
			        costVal = cost;
			    }
                var costUsd = 0M;

			    var date = (DateTime)row["pnaklDateExport"];
                if (num == 1 && date > new DateTime(2017, 12, 31))
				{
					costUsd = (decimal) row["prrasosCostUsd"]; 
				}

				if (num == 2 || date <= new DateTime(2017, 12, 31))
				{
					var valId = (decimal)row["pnaklValId"];
					if (kursVal != null)
						foreach (var rowVal in kursVal.Select())
						{
							if ((decimal)rowVal["kursValId"] == valId
							    && (DateTime)rowVal["kursDate"] == date)
							{
								var kurs = (decimal)rowVal["kursKurs"];
								cost = decimal.Round(costVal * kurs,2);
								break;
							}
						}

				    var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
				    costUsd = decimal.Round(cost / rateUsd, 2); 
				}

			    // Деноминация
			    if (date < new DateTime(2016, 7, 1))
			    {
			        cost = cost / 10000;
			    }

                var costResult = decimal.Round((count * cost), 2)
				                 + decimal.Round(decimal.Round((count * cost), 2) * nds / 100, 2);

				var costValResult = decimal.Round((count * costVal), 2)
				                    + decimal.Round(decimal.Round((count * costVal), 2) * nds / 100, 2);

				decimal costUsdResult = decimal.Round((count * costUsd), 2)
				                        + decimal.Round(decimal.Round((count * costUsd), 2) * nds / 100, 2);

				// Флаг - находится ли строка в результ. выборке
				bool flag = false;
				foreach (var exportByTypeSupply in exportsByTypeSupply)
				{
					if (exportByTypeSupply.TypeSupply == typeSupplyName
						&& exportByTypeSupply.Company == company
					    && exportByTypeSupply.Name == productName)
					{
						flag = true;
						exportByTypeSupply.Count += count;
						exportByTypeSupply.Cost += costResult;
						exportByTypeSupply.CostUsd += costUsdResult;
						exportByTypeSupply.CostVal += costValResult;
						break;
					}
				}

				if (!flag)
				{
					exportsByTypeSupply.Add(new ExportByTypeSupply()
					{
						TypeSupply = typeSupplyName,
						Company = company,
						Name = productName,
						Cost = costResult,
						CostUsd = costUsdResult,
						CostVal = costValResult,
						Count = count,
						Territory = territoryName
					});
				}
			}

			exportsByTypeSupply.Sort();

			return exportsByTypeSupply;
		}

	}
}
