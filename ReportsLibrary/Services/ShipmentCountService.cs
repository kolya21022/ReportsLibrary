using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;
namespace ReportsLibrary.Services
{
    /// <summary>
    /// Сервисный класс формирование листа записей отчета [Отгрузка(кол-во)]
    /// </summary>
    public class ShipmentCountService
    {
        private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
        private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
        private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

	    private static readonly string QueryPnaklPrrasosIzdel = "SELECT result.kizd as prrasosProductId, " +
	                                                       "result.kol as prrasosCount, " +
	                                                       "izdel.kizd as izdelProductId, " +
	                                                       "izdel.nizd as izdelProduct, " +
	                                                       "izdel.edizm as izdelMeasure  " +
	                                                       "FROM (SELECT kizd, kol, nom_pn FROM \"" + DbfPathFso + "prrasos.dbf\" " +
	                                                       "union all " +
														   "SELECT kizd, kol, nom_pn FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
	                                                       "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
	                                                       "WHERE pnakl.dataras >= ctod( '{0}' ) and pnakl.dataras <= ctod( '{1}' )";

	    private static readonly string QueryPrrasosIzdelReturn = "SELECT prrasos.kizd as prrasosProductId, " +
	                                                        "prrasos.kol as prrasosCount, " +
	                                                        "prrasos.datar as prrasosReturnDate, " +
	                                                        "izdel.kizd as izdelProductId, " +
	                                                        "izdel.nizd as izdelProduct, " +
	                                                        "izdel.edizm as izdelMeasure  " +
	                                                        "FROM (SELECT kizd, kol, datar, pr_v FROM \"" + DbfPathFso + "prrasos.dbf\" " +
	                                                        "union all " +
	                                                        "SELECT kizd, kol, datar, pr_v FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
	                                                        "as result " +
															"LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
	                                                        "WHERE pr_v = 1 and datar >= ctod( '{0}' ) and datar <= ctod( '{1}' )";

        /// <summary>
        /// Логика формирование листа записей отчета [Отгрузка(кол-во)]
        /// </summary>
        public static List<ShipmentCount> GetShipmentCount(DateTime startDate,
            DateTime endDate)
        {
            var shipmentsCount = new List<ShipmentCount>();

			var sqlResultExportList = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdel, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy")), "SqlResultExport");

	        var sqlResultReturnList = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QueryPrrasosIzdelReturn, startDate.ToString("MM/dd/yyyy"),
			        endDate.ToString("MM/dd/yyyy")), "SqlResultReturn");

			foreach (var rowExport in sqlResultExportList.Select())
            {
                var productName = ((string)rowExport["izdelProduct"]).Trim();
                var measure = ((string)rowExport["izdelMeasure"]).Trim();
                var count = (decimal)rowExport["prrasosCount"];

                // Флаг - находится ли строка в результ. выборке
                bool flag = false;
                foreach (var shipmentCount in shipmentsCount)
                {
                    if (shipmentCount.Name == productName
                        && shipmentCount.Measure == measure)
                    {
                        flag = true;
                        shipmentCount.Count += count;
                        break;
                    }
                }

                if (!flag)
                {
                    shipmentsCount.Add(new ShipmentCount()
                    {
                        Name = productName,
                        Measure = measure,
                        Count = count
                    });
                }
            }

            //foreach (var rowReturn in returnList.Select())
            //{
            //    var productName = ((string)rowReturn["izdelProduct"]).Trim();
            //    var measure = ((string)rowReturn["izdelMeasure"]).Trim();
            //    var count = (decimal)rowReturn["prrasosCount"];

            //    // Флаг - находится ли строка в результ. выборке
            //    bool flag = false;
            //    foreach (var shipmentCount in shipmentsCount)
            //    {
            //        if (shipmentCount.Name == productName
            //            && shipmentCount.Measure == measure)
            //        {
            //            flag = true;
            //            shipmentCount.Count -= count;
            //            break;
            //        }
            //    }

            //    if (!flag)
            //    {
            //        shipmentsCount.Add(new ShipmentCount()
            //        {
            //            Name = productName,
            //            Measure = measure,
            //            Count = -count
            //        });
            //    }
            //}
            shipmentsCount.Sort();

            return shipmentsCount;
        }

    }
}
