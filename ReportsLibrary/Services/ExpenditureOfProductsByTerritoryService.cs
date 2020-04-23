using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Расход изделий по территориям за месяц]
	/// </summary>
	class ExpenditureOfProductsByTerritoryService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
		private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosIzdelPotrKursValUsdRus = "SELECT result.kizd as prrasosProductId, " +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.Ceh as prrasosCeh, " +
		                                                   "result.Cenad as prrasosCost, " +
		                                                   "result.Cena_dol as prrasosCostUsd, " +
		                                                   "pnakl.dataotg AS pnaklDateShipment, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.nomdok as pnaklExportId, " +
		                                                   "pnakl.nom_ttn as ttn, " +
		                                                   "pnakl.stavkands as pnaklNds, " +
		                                                   "pnakl.TTN as nomActs, " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "izdel.nizd as productName, " +
		                                                   "potr.kpotr as potrCompanyId, " +
		                                                   "potr.npotr AS potrCompany, " +
		                                                   "potr.gorod as potrCity, " +
		                                                   "potr.kter as potrTerritoryId, " +
		                                                   "kurs_valUsd.data as kursDate, " +
		                                                   "kurs_valUsd.kurs AS kursKurs, " +
		                                                   "kurs_valRus.data as kursRusDate, " +
		                                                   "kurs_valRus.kurs AS kursRusKurs " +
		                                                   "FROM (SELECT ceh, kizd, kol, nom_pn, cenad, cena_dol FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT ceh, kizd,  kol, nom_pn, cenad, cena_dol FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_valUsd on pnakl.dataotg = kurs_valUsd.data " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_valRus on pnakl.dataotg = kurs_valRus.data " +
		                                                   "WHERE dataotg >= ctod( '{0}' ) and dataotg <= ctod( '{1}' ) " +
		                                                   "and kurs_valUsd.kval = 1 and kurs_valUsd.data >= ctod( '{0}' ) " +
		                                                   "and kurs_valRus.kval = 6 and kurs_valRus.data >= ctod( '{0}' ) " +
		                                                   "ORDER BY kurs_valUsd.data, kurs_valRus.data DESC";

		private const string QueryTerr = "SELECT terr.kter as territoryId, terr.nter as territoryName FROM terr ";
		/// <summary>
		/// Логика формирование листа записей отчета [Расход изделий по территориям за месяц] 
		/// </summary>

		public static List<ExpenditureOfProductsByTerritory> GetExpenditureOfProductsByTerritory(DateTime startDate,
			DateTime endDate)
		{
			var endDateMinusDay = endDate.AddDays(-1);

			var expenditureOfProductsByTerritory = new List<ExpenditureOfProductsByTerritory>();

			var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotrKursValUsdRus, startDate.ToString("MM/dd/yyyy"), endDateMinusDay.ToString("MM/dd/yyyy")), "SqlResult");

			var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

			foreach (var row in sqlResult.Select())
			{
				var territoryId = ((string)row["potrTerritoryId"]).Trim();
				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
				var territoryIdZero = territoryIdTwo != "50" ? territoryIdTwo + "00" : territoryId;
				
				var territoryName = string.Empty;

				// Получение названия страны по сформированному id
				foreach (var rowTeritory in territories.Select())
				{

					if (((string)rowTeritory["territoryId"]).Trim() == territoryIdZero)
					{
						territoryName = ((string)rowTeritory["territoryName"]).Trim();
						break;
					}

				}

				var count = (decimal)row["prrasosCount"];
				var cost = (decimal) row["prrasosCost"];
				var costUsd = (decimal)row["prrasosCostUsd"];
				var nds = (decimal)row["pnaklNds"];
				//Сумма без НДС
				var summaWithOutNds = cost * count;

				//Сумма НДС
				var summaNds = summaWithOutNds * nds / 100;

				//Всего с НДС
				var summaWithNds = summaWithOutNds + summaNds;

				////Сумма без НДС и налогов
				//var summaWithOutNdsAndTaxes = summaWithNds - summaNds;

				//Сумма в долларах + курс
				var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
				var summaUsd = costUsd != 0 ? (costUsd + costUsd * nds / 100) * count : summaWithNds / rateUsd;

				//Курс руб. РФ
				var rateRus = row["kursRusKurs"] == DBNull.Value ? 1 : (decimal)row["kursRusKurs"];

				var productName = ((string)row["productName"]).Trim();
				var date = (DateTime)row["pnaklDateShipment"];
				var ceh = (decimal) row["prrasosCeh"];
				var ttnNumber = (decimal) row["ttn"];
				var actsNumber = (decimal) row["nomActs"];
				if (actsNumber != 0)
				{
					var x = ttnNumber;
					ttnNumber = actsNumber;
					actsNumber = x;
				}
				var company = ((string)row["potrCompany"]).Trim();
				var city = ((string)row["potrCity"]).Trim();
				var companyWithCity = city + ", " + company;

				// Флаг - находится ли строка в результ. выборке
				bool flag = false;
				foreach (var expenditureOfProductByTerritory in expenditureOfProductsByTerritory)
				{
					if (expenditureOfProductByTerritory.ProductName == productName
					    && expenditureOfProductByTerritory.NumberAkt == actsNumber
					    && expenditureOfProductByTerritory.NumberTtn == ttnNumber)
					{
						flag = true;
						expenditureOfProductByTerritory.ProductCount += count;
						//expenditureOfProductByTerritory.CostWithoutNds +=  cost;
						expenditureOfProductByTerritory.SummaWithoutNds += summaWithOutNds;
						expenditureOfProductByTerritory.SummaNds += summaNds;
						expenditureOfProductByTerritory.SummaWithNds += summaWithNds;
						expenditureOfProductByTerritory.SummaWithoutNdsAndTaxes += summaWithOutNds;
						expenditureOfProductByTerritory.SummaUsd += summaUsd;
						expenditureOfProductByTerritory.RusRate += rateRus;
						expenditureOfProductByTerritory.UsdRate += rateUsd;

					}

				}

				if (!flag)
				{
					expenditureOfProductsByTerritory.Add(new ExpenditureOfProductsByTerritory()
						{
							ProductName = productName,
							CompanyName = companyWithCity,
							NumberTtn = ttnNumber,
							NumberAkt = actsNumber,
							ShipmentDate = date,
							Ceh = ceh,
							TerritoryName = territoryName,
							ProductCount = count,
							CostWithoutNds = cost,
							SummaWithoutNds = summaWithOutNds,
							SummaNds = summaNds,
							SummaWithNds = summaWithNds,
							SummaWithoutNdsAndTaxes = summaWithOutNds,
							SummaUsd = summaUsd,
							UsdRate = rateUsd,
							RusRate = rateRus
						}
					);
				}
			}

			expenditureOfProductsByTerritory.Sort();
			return expenditureOfProductsByTerritory;
		}

	}
}
