using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Для контроля дебюторской задолжности]
	/// </summary>
	public class ControlDebtsService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosIzdelPotrFormaR = "SELECT result.kizd as prrasosProductId, " +
																		  "result.kol as prrasosCount, " +
																		  "result.Cenad as prrasosCost, " +
		                                                                  "pnakl.nom_ttn as ttn, " +
		                                                                  "pnakl.nomdog as numberContract, " +
		                                                                  "pnakl.stavkands as stavkands, " +
		                                                                  "pnakl.kval as pnaklValId, " +
		                                                                  "pnakl.DataRas as exportDate, " +
		                                                                  "pnakl.kpotr as pnaklCompanyId, " +
		                                                                  "pnakl.p_f as pnaklFormPayement, " +
		                                                                  "pnakl.dopl_dog as pnaklPaymentDate, " +
		                                                                  "izdel.kizd as izdelProductId, " +
		                                                                  "izdel.nizd AS izdelProduct, " +
		                                                                  "potr.kpotr as potrCompanyId, " +
		                                                                  "potr.kter as potrTerritoryId, " +
		                                                                  "potr.gorod as City, " +
		                                                                  "potr.npotr as potrCompanyName, " +
		                                                                  "forma_r.forma as formaId, " +
		                                                                  "forma_r.nforma as formaName " +
		                                                                  "FROM (SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                                  "union all " +
																		  "SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                                  "as result " +
																		  "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
																		  "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
																		  "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
																		  "LEFT JOIN \"" + DbfPathBase + "forma_r.dbf\" as forma_r on pnakl.p_f = forma_r.forma " +
		                                                                  "WHERE (pnakl.p_f = 10 or pnakl.p_f = 13) and " +
		                                                                  "dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )";


	    private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
	                                        "kurs_val.kurs AS kursKurs, " +
	                                        "kurs_val.kval as kursValId " +
	                                        "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

        /// <summary>
        /// Логика формирование листа записей отчета [Для контроля дебюторской задолжности] 
        /// </summary>
        public static List<ControlDebts> GetControlDebts(DateTime startDate, DateTime endDate, string forma)
		{
			var controlsDebts = new List<ControlDebts>();

			var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotrFormaR, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy")), "SqlResult");

		    DataTable kursVal = null;
		    // Необходимо получать лист курсов валют только если год < 2018 (Увеличение производительности)
		    if (startDate <= new DateTime(2017, 12, 31))
		    {
		        kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
		            query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
		            tableName: "RateVal");
		    }

            foreach (var row in sqlResult.Select())
			{
				var formaName = ((string)row["formaName"]).Trim();
				if (forma != "общая")
				{
					if (formaName != forma)
					{
						continue;
					}
				}

				var company = ((string)row["City"]).Trim() + ", " + ((string)row["potrCompanyName"]).Trim();
				var productName = ((string)row["izdelProduct"]).Trim();
				var ttn = (decimal)row["ttn"];
				var numberContract = ((string)row["numberContract"]).Trim();
				var shipmentDate = (DateTime) row["exportDate"];
				var paymentDate = (DateTime)row["pnaklPaymentDate"];
				var count = (decimal)row["prrasosCount"];
				var stavkands = (decimal)row["stavkands"];
				var cost = (decimal)row["prrasosCost"];

			    var valId = (decimal)row["pnaklValId"];

                // до 2018 в cenad хранилась цена в валюте
                if (shipmentDate <= new DateTime(2017, 12, 31))
			    {
			        if (kursVal != null)
			        {
			            foreach (var rate in kursVal.Select())
			            {
			                if ((decimal) rate["kursValId"] == valId
			                    && (DateTime) rate["kursDate"] == shipmentDate)
			                {
			                    var rateVal = (decimal) rate["kursKurs"];
			                    cost = cost * rateVal;
			                    break;
			                }
			            }
			        }
			    }

			    var costResult = decimal.Round(count * cost + count * cost * stavkands / 100, 2);

                // Деноминация
                if (shipmentDate < new DateTime(2016, 7, 1))
			    {
			        costResult = costResult / 10000;
			    }

				bool flag = false;
				foreach (var controlDebts in controlsDebts)
				{
					if (controlDebts.NumberContract == numberContract
						&& controlDebts.PaymentDate == paymentDate
					    && controlDebts.Name == productName
					    && controlDebts.Ttn == ttn)
					{
						flag = true;
						controlDebts.Cost += costResult;
						break;
					}
				}

				if (!flag)
				{
					controlsDebts.Add(new ControlDebts()
					{
						Company = company,
						Cost = costResult,
						FormPayment = formaName,
						Ttn = ttn,
						Name = productName,
						NumberContract = numberContract,
						PaymentDate = paymentDate,
						ShipmentDate = shipmentDate
					});
				}
			}

			controlsDebts.Sort();

			return controlsDebts;
		}

	}
}

