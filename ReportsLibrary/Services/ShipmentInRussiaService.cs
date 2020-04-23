using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Отгрузка в РФ]
	/// </summary>
	public class ShipmentInRussiaService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
		private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosPotrIzdelKursValUsdRus = "SELECT result.kizd as prrasosProductId, " +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.cenad as prrasosCost, " +
		                                                   "result.cena_dol as prrasosCostUsd, " +
		                                                   "result.cena_val as prrasosCostRus, " +
		                                                   "pnakl.nomdok as pnaklExportId, " +
		                                                   "pnakl.dataras as pnaklDateShipment, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.stavkands as pnaklNds, " +
		                                                   "pnakl.kval as pnaklValId, " +
		                                                   "pnakl.nom_ttn as pnaklNomTtn, " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "izdel.nizd as izdelProduct, " +
		                                                   "potr.kpotr as potrCompanyId, " +
		                                                   "potr.npotr as potrCompany, " +
		                                                   "potr.gorod as potrCity, " +
		                                                   "potr.kter as potrTerritoryId, " +
		                                                   "kurs_valUsd.data as kursDate, " +
		                                                   "kurs_valUsd.kurs as kursKurs, " +
		                                                   "kurs_valRus.data as kursRusDate, " +
		                                                   "kurs_valRus.kurs as kursRusKurs " +
		                                                   "FROM(SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_valUsd on pnakl.dataras = kurs_valUsd.data " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_valRus  on pnakl.dataras =kurs_valRus.data  " +
		                                                   "WHERE pnakl.dataras >= ctod( '{0}' ) and pnakl.dataras <= ctod( '{1}' ) " +
		                                                   "and kurs_valUsd.kval = 1 and kurs_valUsd.data >= ctod( '{0}' ) " +
		                                                   "and kurs_valRus.kval = 6 and kurs_valRus.data >= ctod( '{0}' ) " +
		                                                   "ORDER BY kurs_valUsd.data, kurs_valRus.data DESC";

		private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
		                                    "kurs_val.kurs AS kursKurs, " +
		                                    "kurs_val.kval as kursValId " +
		                                    "FROM kurs_val where kurs_val.kval = 6 and " +
		                                    "kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		/// <summary>
		/// Логика формирование листа записей отчета [Отгрузка в РФ]
		/// </summary>
		public static List<ShipmentInRussia> GetShipmentInRussia(DateTime startDate, DateTime endDate)
		{
			var endDateMinusDay = endDate.AddDays(-1);
			var startDateYear = new DateTime(startDate.Year, 1, 1);

			var summUsdYear = 0M;
			var summRusYear = 0M;

			var shipmentsInRussia = new List<ShipmentInRussia>();

			var rateVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				query: string.Format(QueryKursVal, startDateYear.ToString("MM/dd/yyyy")),
				tableName: "RateVal");

			var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosPotrIzdelKursValUsdRus, startDateYear.ToString("MM/dd/yyyy"),
					endDateMinusDay.ToString("MM/dd/yyyy")), "SqlResult");

			foreach (var row in sqlResult.Select())
			{
				var territoryId = ((string) row["potrTerritoryId"]).Trim();
				var territoryIdTwo = "" + territoryId[0] + territoryId[1];

				if (territoryIdTwo != "11")
				{
					continue;
				}

				var date = (DateTime) row["pnaklDateShipment"];
				var valId = (decimal) row["pnaklValId"];
				var count = (decimal) row["prrasosCount"];
				var costUsd = (decimal) row["prrasosCostUsd"];
				var costRus = (decimal) row["prrasosCostRus"];
				if (valId != 6)
				{
					var rateRusOnDate = row["kursRusKurs"] == DBNull.Value ? 1 : (decimal) row["kursRusKurs"];
					var cost = (decimal) row["prrasosCost"];
					costRus = cost / rateRusOnDate;
				}

				// до 2018 в cenad хранилась цена в валюте
				if (date <= new DateTime(2017, 12, 31))
				{
					var rateRusOnDate = row["kursRusKurs"] == DBNull.Value ? 1 : (decimal) row["kursRusKurs"];
					var rateUsdOnDate = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
					var cost = (decimal) row["prrasosCost"];

					foreach (var rate in rateVal.Select())
					{
						if ((decimal) rate["kursValId"] == valId
						    && (DateTime) rate["kursDate"] == date)
						{
							var rateValOnDate = (decimal) rate["kursKurs"];
							cost = cost * rateValOnDate;
							break;
						}
					}

					costRus = cost / rateRusOnDate;
					costUsd = cost / rateUsdOnDate;
				}

				summUsdYear += count * costUsd;
				summRusYear += count * costRus;



				// Подсчет отгруженного в РФ за указанный период
				if (date >= startDate && date <= endDate)
				{
					var numberTtn = (decimal) row["pnaklNomTtn"];
					var productName = ((string) row["izdelProduct"]).Trim();
					var company = ((string) row["potrCompany"]).Trim();
					var city = ((string) row["potrCity"]).Trim();

					// Флаг - находится ли строка в результ. выборке
					bool flag = false;
					foreach (var shipmentInRussia in shipmentsInRussia)
					{
						if (shipmentInRussia.NumberTtn == numberTtn
						    && shipmentInRussia.Name == productName
						    && shipmentInRussia.Cost == decimal.Round(costRus, 2)
						    && shipmentInRussia.CostUsd == decimal.Round(costUsd, 0))
						{
							flag = true;
							shipmentInRussia.Count += count;
							break;
						}
					}

					if (!flag)
					{
						shipmentsInRussia.Add(new ShipmentInRussia()
						{
							NumberTtn = numberTtn,
							Name = productName,
							Company = city + ", " + company,
							Cost = decimal.Round(costRus, 2),
							CostUsd = decimal.Round(costUsd, 0),
							Count = count,
							Date = date
						});
					}
				}
			}

			if (!shipmentsInRussia.Any())
			{
				shipmentsInRussia.Add(new ShipmentInRussia());
			}

			foreach (var shipmentInRussia in shipmentsInRussia)
			{
				shipmentInRussia.YearSumma = decimal.Round(summRusYear, 2);
				shipmentInRussia.YearSummaUsd = decimal.Round(summUsdYear, 2);
			}

			shipmentsInRussia.Sort();

			return shipmentsInRussia;
		}
	}
}