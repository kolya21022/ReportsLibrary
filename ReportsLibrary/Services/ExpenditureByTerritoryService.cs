using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Расход по территориям(итог с начала года)]
	/// </summary>
	public class ExpenditureByTerritoryService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;

		private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;


		private static readonly string QueryPnaklPrrasosIzdelPotrKursValUsd = "SELECT result.kizd as prrasosProductId, " +
														   "result.kol as prrasosCount, " +
														   "result.Cenad as prrasosCost, " +
														   "result.Cena_dol as prrasosCostUsd, " +
														   "pnakl.nomdok as pnaklExportId, " +
														   "pnakl.dataras as pnaklDateExport, " +
														   "pnakl.kpotr as pnaklCompanyId, " +
														   "pnakl.Stavkands as pnaklNds, " +
														   "pnakl.kval as pnaklValId, " +
														   "pnakl.p_dog as pnaklFormPayment, " +
														   "potr.kpotr as potrCompanyId, " +
														   "potr.kter as potrTerritoryId, " +
														   "potr.gorod as potrCity, " +
														   "potr.npotr as potrCompanyName, " +
														   "izdel.kizd as izdelProductId, " +
														   "izdel.nizd AS izdelProduct, " +
														   "kurs_val.data as kursDate, " +
														   "kurs_val.kurs as kursKurs " +
														   "FROM (SELECT kizd, kol, nom_pn, cenad, cena_dol FROM \"" + DbfPathFso + "prrasos.dbf\" " +
														   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad, cena_dol FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
														   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on pnakl.dataras = kurs_val.data " +
														   "WHERE dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' ) and kurs_val.kval = 1 and " +
		                                                   "kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
		                                    "kurs_val.kurs AS kursKurs, " +
		                                    "kurs_val.kval as kursValId " +
		                                    "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		/// <summary>
		/// Логика формирование листа записей отчета  [Расход по территориям(итог с начала года)]
		/// </summary>
		public static List<ExpenditureByTerritory> GetExpenditureByTerritory(DateTime startDate, DateTime endDate, string abroad)
		{
			var expenditureByTerritory = new List<ExpenditureByTerritory>();

			var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotrKursValUsd, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy")), "SqlResult");

			var rateVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
				tableName: "RateVal");

			foreach (var row in sqlResult.Select())
			{
				var territoryId = ((string)row["potrTerritoryId"]).Trim();
				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

				if (territoryIdTwo == "15")
				{
					continue;
				}

				if (abroad == "дальнее" && territoryIdTwo != "50")
				{
					continue;
				}
				if (abroad == "ближнее" && territoryIdTwo == "50")
				{
					continue;
				}

				var rateUsdOnDate = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
				var valId = (decimal)row["pnaklValId"];
				var cost = (decimal)row["prrasosCost"];
				var costUsd = (decimal)row["prrasosCostUsd"];
				var nds = (decimal)row["pnaklNds"];
				var count = (decimal)row["prrasosCount"];
				var formPayer = ((string)row["pnaklFormPayment"]).Trim() == "b" ? "бартер" : "деньги";
				var companyWhithCity = ((string)row["potrCity"]).Trim()
							  + ", "
							  + ((string)row["potrCompanyName"]).Trim();
				var productName = ((string)row["izdelProduct"]).Trim();

				var date = (DateTime)row["pnaklDateExport"];
				var dictionaryMonth = Common.MonthsFullNames();
				var month = dictionaryMonth[date.Month];

				// до 2018 в cenad хранилась цена в валюте
				if (date <= new DateTime(2017, 12, 31))
				{
					foreach (var rate in rateVal.Select())
					{
						if ((decimal)rate["kursValId"] == valId
						    && (DateTime)rate["kursDate"] == date)
						{
							var rateValOnDate = (decimal)rate["kursKurs"];
							cost = cost * rateValOnDate;
							break;
						}
					}
					costUsd = cost / rateUsdOnDate;
				}

				var costResult = (cost + cost * nds / 100) * count;
				var costUsdResult = (costUsd + costUsd * nds / 100) * count;

				// Деноминация
				if (date < new DateTime(2016, 7, 1))
				{
					costResult = costResult / 10000;
				}

				// Флаг - находится ли строка в результ. выборке
				bool flag = false;
				foreach (var exByTerritory in expenditureByTerritory)
				{
					if (exByTerritory.CompanyName == companyWhithCity
						&& exByTerritory.ProductName == productName
						&& exByTerritory.MoneyOrBarter == formPayer
					    && exByTerritory.Month == month)
					{
						flag = true;
						exByTerritory.SummRb += costResult;
						exByTerritory.SummUsd += costUsdResult;
						break;
					}
				}

				if (!flag)
				{
					expenditureByTerritory.Add(new ExpenditureByTerritory()
					{
						ProductName = productName,
						CompanyName = companyWhithCity,
						MoneyOrBarter = formPayer,
						Month = month,
						MonthId = date.Month,
						SummRb = costResult,
						SummUsd = costUsdResult
					});
				}
			}

			expenditureByTerritory.Sort();
			return expenditureByTerritory;
		}
	}
}
