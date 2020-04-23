using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.External;
using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Отгрузка по изделию]
	/// </summary>
	public class ShipmentByProductService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosIzdelPotr = "SELECT result.kizd as prrasosProductId, " +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.cenad as prrasosCost, " +
		                                                   "pnakl.dataotg as pnaklDateShipment, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.kval as pnaklValId, " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "izdel.nizd as izdelProduct, " +
		                                                   "izdel.edizm as izdelMeasure, " +
		                                                   "potr.kpotr as potrCompanyId, " +
		                                                   "potr.npotr AS potrCompany, " +
		                                                   "potr.gorod as City " +
		                                                   "FROM (SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
		                                                   "WHERE dataotg >= ctod( '{0}' ) and dataotg <= ctod( '{1}' ) " +
		                                                   "and result.kizd = {2}";

	    private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
	                                        "kurs_val.kurs AS kursKurs, " +
	                                        "kurs_val.kval as kursValId " +
	                                        "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

        /// <summary>
        /// Логика формирование листа записей отчета [Отгрузка по изделию]
        /// </summary>
        public static List<ShipmentByProduct> GetShipmentByProduct(string monthOrYear, 
			DateTime loadDateTime, Product product)
		{
			DateTime endDate = loadDateTime.AddDays(-1);
			DateTime startDate = monthOrYear == "m" ? Common.GetBeginOfMonthWithOffset(loadDateTime)
			: new DateTime(loadDateTime.Year, 1, 1);

			var shipmentsByProduct = new List<ShipmentByProduct>();

			var sqlResultprrasospnaklpotrizdel = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotr, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy"), product.Id), "SqlResultprrasospnaklpotrizdel");
		
		    DataTable kursVal = null;
		    // Необходимо получать лист курсов валют только если год < 2018 (Увеличение производительности)
		    if (startDate <= new DateTime(2017, 12, 31))
		    {
		        kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
		            query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
		            tableName: "RateVal");
		    }

            foreach (var row in sqlResultprrasospnaklpotrizdel.Select())
		    {
		        var date = (DateTime) row["pnaklDateShipment"];

		        var productName = ((string) row["izdelProduct"]).Trim();
		        var measure = ((string) row["izdelMeasure"]).Trim();
		        var company = ((string) row["potrCompany"]).Trim();
		        var city = ((string) row["City"]).Trim();
		        var cost = (decimal) row["prrasosCost"];
		        var count = (decimal) row["prrasosCount"];

		        var valId = (decimal)row["pnaklValId"];

                // до 2018 в cenad хранилась цена в валюте
                if (date <= new DateTime(2017, 12, 31))
                {
                    if (kursVal != null)
                    {
                        foreach (var rate in kursVal.Select())
                        {
                            if ((decimal) rate["kursValId"] == valId
                                && (DateTime) rate["kursDate"] == date)
                            {
                                var rateVal = (decimal) rate["kursKurs"];
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

                shipmentsByProduct.Add(new ShipmentByProduct()
		        {
		            Name = productName,
		            Measure = measure,
		            Company = city + ", " + company,
		            Cost = cost,
		            Count = count,
		            Date = date
		        });

		    }

		    shipmentsByProduct.Sort();

			return shipmentsByProduct;
		}
	}
}




