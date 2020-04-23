using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{

	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Сводки] Отгружено дилерам
	/// </summary>
	public class ShipmentForDilerService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
		private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosPotrIzdelKursUsdRus = "SELECT result.kizd as prrasosProductId, " +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.cenad as prrasosCost, " +
		                                                   "result.cena_dol as prrasosCostUsd, " +
		                                                   "result.cena_val as prrasosCostRus, " +
		                                                   "pnakl.nomdok as pnaklExportId, " +
		                                                   "pnakl.dataotg as pnaklDateShipment, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.Stavkands as pnaklNds, " +
		                                                   "pnakl.kval as pnaklValId, " +
		                                                   "potr.kpotr as potrCompanyId, " +
		                                                   "potr.npotr as potrCompany, " +
		                                                   "potr.gorod as potrCity, " +
		                                                   "potr.kter as potrTerritoryId, " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "kat_prod.kat as KategoryId, " +
		                                                   "kat_prod.nkat as ProductKat, " +
		                                                   "kurs_valUsd.data as kursDate, " +
		                                                   "kurs_valUsd.kurs as kursKurs, " +
		                                                   "kurs_valRus.data as kursRusDate, " +
		                                                   "kurs_valRus.kurs as kursRusKurs " +
		                                                   "FROM( SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result  " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_valUsd on pnakl.dataotg = kurs_valUsd.data " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_valRus  on pnakl.dataotg = kurs_valRus.data " +
														   "WHERE (pnakl.dataotg >= ctod( '{0}' ) and pnakl.dataotg <= ctod( '{1}' ) " +
														   "and (pnakl.postavka = '{2}' or pnakl.postavka = '{3}' or pnakl.kpotr = 9716 )) " +
														   "and (kurs_valUsd.kval = 1 and kurs_valUsd.data >= ctod( '{0}' )) " +
														   "and (kurs_valRus.kval = 6 and kurs_valRus.data >= ctod( '{0}' )) " +
		                                                   "ORDER BY kurs_valUsd.data, kurs_valRus.data DESC";

		private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
											"kurs_val.kurs AS kursKurs, " +
											"kurs_val.kval as kursValId " +
											"FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		/// <summary>
		/// Логика формирование листа записей отчета [Сводки] Отгружено дилеру
		/// </summary>

		public static List<ShipmentForDiler> GetShipmentForDiler(DateTime startDate, DateTime endDate)
		{
			var endDateMinusDay = endDate.AddDays(-1);

			var shipmentForDiler = new List<ShipmentForDiler>();

			var kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
				tableName: "RateVal");

			var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosPotrIzdelKursUsdRus, startDate.ToString("MM/dd/yyyy"),
					endDateMinusDay.ToString("MM/dd/yyyy"), "08", "09"), "SqlResult");

			foreach (var row in sqlResult.Select())
			{
				var territoryId = ((string)row["potrTerritoryId"]).Trim();
				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

				var date = (DateTime) row["pnaklDateShipment"];
				var valId = (decimal) row["pnaklValId"];
				var count = (decimal) row["prrasosCount"];
				var costUsd = (decimal) row["prrasosCostUsd"];
				var cost = (decimal) row["prrasosCost"];
				var nds = (decimal) row["pnaklNds"];
				var costRus = 0M;
				var companyId = (decimal) row["pnaklCompanyId"];
				if (companyId == 9716)
				{
					costRus = cost;
					var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];

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

				if (territoryIdTwo == "11")
				{
					if (valId == 6)
					{
						costRus = (decimal) row["prrasosCostRus"];
					}
					else
					{
						var costVal = (decimal) row["prrasosCostRus"];
						var rateRus = row["kursRusKurs"] == DBNull.Value ? 1 : (decimal) row["kursRusKurs"];

						foreach (var rate in kursVal.Select())
						{
							if ((decimal) rate["kursValId"] == valId
							    && (DateTime) rate["kursDate"] == date)
							{
								var rateVal = (decimal) rate["kursKurs"];
								cost = costVal * rateVal;
								break;
							}
						}

						costRus = cost / rateRus;
					}
				}

				//Формирование цены продукта в РФ,РБ в зависимости от территории и цены в usd
				var summaRfRb = count * (costRus + costRus * nds /100);
				var summaUsd = count * (costUsd + costUsd * nds / 100);

				// Подсчет отгруженного дилеру за указанный период
				var productName = ((string) row["ProductKat"]).Trim();
				var company = ((string) row["potrCompany"]).Trim();
				var city = ((string) row["potrCity"]).Trim();
				var companyWithCity = city + ", " + company;


				// Флаг - находится ли строка в результ. выборке
				bool flag = false;
				foreach (var shipmentsForDiler in shipmentForDiler)
				{
					if (shipmentsForDiler.ProductName == productName &&
					shipmentsForDiler.PotrName == companyWithCity)
					{
						flag = true;
						shipmentsForDiler.ProductCount += count;
						shipmentsForDiler.SummaUsd += summaUsd;
						shipmentsForDiler.SummaRfRb += summaRfRb;
					}
				}

				if (!flag)
				{
					shipmentForDiler.Add(new ShipmentForDiler()
					{
						ProductName = productName,
						PotrName = companyWithCity,
						ProductCount = count,
						SummaRfRb = summaRfRb,
						SummaUsd = summaUsd
					});
				}
			}

			shipmentForDiler.Sort();
			return shipmentForDiler;
		}
	}
}
