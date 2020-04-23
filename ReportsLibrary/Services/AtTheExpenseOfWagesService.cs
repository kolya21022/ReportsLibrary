using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [В счет зарплаты]
	/// </summary>
	public class AtTheExpenseOfWagesService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosIzdel = "SELECT result.kizd as prrasosProductId, result.kol as prrasosCount, " +
		                                                        "result.Cenad as prrasosCost, " +
		                                                        "pnakl.nom_ttn as ttn, " +
		                                                        "pnakl.kval as pnaklValId, " +
		                                                        "pnakl.stavkands as stavkands, " +
		                                                        "pnakl.DataRas as exportDate, " +
		                                                        "izdel.kizd as izdelProductId, " +
		                                                        "izdel.nizd AS izdelProduct " +
		                                                        "FROM (SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                        "union all " +
																"SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) as result " +
																"LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
																"LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
		                                                        "WHERE pnakl.kpotr = 2657 and dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )";

	    private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
	                                        "kurs_val.kurs AS kursKurs, " +
	                                        "kurs_val.kval as kursValId " +
	                                        "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) " +
	                                        "ORDER BY kurs_val.data DESC";

        /// <summary>
        /// Логика формирование листа записей отчета [В счет зарплаты] 
        /// </summary>
        public static List<AtTheExpenseOfWages> GetAtTheExpenseOfWages(DateTime startDate, DateTime endDate)
		{
			var atTheExpenseOfWages = new List<AtTheExpenseOfWages>();

			var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdel, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy")), "SqlResult");
			
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
				var productName = ((string)row["izdelProduct"]).Trim();
				var ttn = (decimal)row["ttn"];
				var date = (DateTime)row["exportDate"];
				var count = (decimal)row["prrasosCount"];
				var stavkands = (decimal)row["stavkands"];
				var cost = (decimal)row["prrasosCost"];

			    var valId = (decimal)row["pnaklValId"];

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
			                    cost = cost * rateVal;
			                    break;
			                }
			            }
			        }
			    }

			    // Деноминация
			    if (date < new DateTime(2016, 7, 1))
			    {
			        cost = cost / 10000;
			    }

                var costResult = decimal.Round(count * cost + count * cost * stavkands / 100, 2);

				bool flag = false;
				foreach (var atTheExpenseOfWage in atTheExpenseOfWages)
				{
					if (atTheExpenseOfWage.Ttn == ttn
						&& atTheExpenseOfWage.Name == productName)
					{
						flag = true;
						atTheExpenseOfWage.Count += count;
						atTheExpenseOfWage.Cost += costResult;
						break;
					}
				}

				if (!flag)
				{
					atTheExpenseOfWages.Add(new AtTheExpenseOfWages()
					{
						Name = productName,
						Cost = costResult,
						ShipmentDate = date,
						Count = count,
						Ttn = ttn
					});
				}
			}

			atTheExpenseOfWages.Sort();

			return atTheExpenseOfWages;
		}

		
	}
}

