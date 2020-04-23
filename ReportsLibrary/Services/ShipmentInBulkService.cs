using System;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
    /// <summary>
    /// Сервисный класс формирование записей отчета [Сводки] Отгрузка в натуральном выражении
    /// </summary>
    public class ShipmentInBulkService
    {
        private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
        private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
        private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

	    private static readonly string QueryPnaklPrrasosIzdelPotr = "SELECT result.kizd as prrasosProductId, " +
	                                                       "result.kol as prrasosCount, " +
	                                                       "result.nom_v, " +
	                                                       "result.pr_v, " +
	                                                       "result.data, " +
	                                                       "result.cenad_u, " +
	                                                       "result.Cenad as prrasosCost, " +
	                                                       "pnakl.nomdok as pnaklExportId, " +
	                                                       "pnakl.dataras AS pnaklDateShipment, " +
	                                                       "pnakl.kpotr as pnaklCompanyId, " +
	                                                       "pnakl.Stavkands as pnaklNds, " +
	                                                       "pnakl.kval as pnaklValId, " +
	                                                       "pnakl.p_dog, " +
	                                                       "potr.kpotr as potrCompanyId, " +
	                                                       "potr.kter as potrTerritoryId, " +
	                                                       "izdel.kizd as izdelProductId, " +
	                                                       "izdel.vid_p as vid_p, " +
	                                                       "izdel.vid as vid, " +
	                                                       "izdel.nizd as izdelProductName, " +
	                                                       "izdel.cena_sop " +
	                                                       "FROM ( SELECT kizd, kol, nom_pn, nom_v, pr_v, data, cenad_u, cenad  FROM \"" + DbfPathFso + "prrasos.dbf\" " +
	                                                       "union all " +
	                                                       "SELECT kizd, kol, nom_pn, nom_v, pr_v, data, cenad_u, cenad FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
	                                                       "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
	                                                       "WHERE dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )";
        /// <summary>
        /// Логика формирование записей отчета [Сводки] Отгрузка в натуральном выражении
        /// </summary>
        public static ShipmentInBulk GetShipmentInBulk(int year)
        {
            DateTime endDate = new DateTime(year+1, 1, 1).AddDays(-1);
            DateTime startDate = new DateTime(year, 1, 1);

            var shipmentInBulk = new ShipmentInBulk() {RasStan = 0, CountDoor = 0, CountMeb = 0, CountPilo = 0,
                CountStan = 0, CountWindow = 0, MebSop = 0, RasDoor = 0, RasMeb = 0, RasPilo = 0, RasWindow = 0};

	        var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QueryPnaklPrrasosIzdelPotr, startDate.ToString("MM/dd/yyyy"),
			        startDate.ToString("MM/dd/yyyy")), "SqlResult");

			//DataTable kursVal = null;
			//// Необходимо получать лист курсов валют только если год < 2018 (Увеличение производительности)
			//if (startDate <= new DateTime(2017, 12, 31))
			//{
			//    kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
			//        query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
			//        tableName: "RateVal");
			//}

			foreach (var row in sqlResult.Select())
            {
                var territoryId = ((string)row["potrTerritoryId"]).Trim();
                var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

                if (territoryIdTwo != "15")
                {
                    continue;
                }

                var cost = (decimal)row["prrasosCost"];
                var count = (decimal)row["prrasosCount"];

                //// до 2018 в cenad хранилась цена в валюте
                //if (date <= new DateTime(2017, 12, 31))
                //{
                //    if (kursVal != null)
                //    {
                //        foreach (var rate in kursVal.Select())
                //        {
                //            if ((decimal)rate["kursValId"] == valId
                //                && (DateTime)rate["kursDate"] == date)
                //            {
                //                var rateVal = (decimal)rate["kursKurs"];
                //                cost = cost * rateVal;
                //                break;
                //            }
                //        }
                //    }
                //}

                //// Деноминация
                //if (date < new DateTime(2016, 7, 1))
                //{
                //    cost = cost / 10000;
                //}

                var vid = (decimal)row["vid"];
                var vidP = (decimal)row["vid_p"];

                if (vid >= 1 && vid <= 6 || vid == 10 || vid == 14)
                {
                    shipmentInBulk.RasStan += cost;
                    shipmentInBulk.CountStan += count;
                    continue;
                }
                if (vidP == 3)
                {
                    shipmentInBulk.RasMeb += cost;
                    shipmentInBulk.CountMeb += count;
                    shipmentInBulk.MebSop += 0;
                    continue;
                }
                if (vidP == 9)
                {
                    shipmentInBulk.RasPilo += cost;
                    shipmentInBulk.CountPilo += count;
                    continue;
                }
                if (vidP == 10)
                {
                    shipmentInBulk.RasDoor += cost;
                    shipmentInBulk.CountDoor += count;
                    continue;
                }
                if (vidP == 11)
                {
                    shipmentInBulk.RasWindow += cost;
                    shipmentInBulk.CountWindow += count;
                }
            }

            return shipmentInBulk;
        }
    }
}
