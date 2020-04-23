using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.External;
using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;


namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Отгрузка по потребителю]
	/// </summary>
	public class ShipmentByCompanyService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosIzdelPotrKursValUsd = "SELECT result.kizd as prrasosProductId, " +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.cenad as prrasosCost, " +
		                                                   "result.cena_dol as prrasosCostUsd, " +
		                                                   "pnakl.nomdok as pnaklExportId, " +
		                                                   "pnakl.dataotg as pnaklDateShipment, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.Stavkands as pnaklNds, " +
		                                                   "pnakl.kval as pnaklValId, " +
		                                                   "potr.kpotr as potrCompanyId, " +
		                                                   "potr.npotr as potrCompany, " +
		                                                   "gorod as potrCity, " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "izdel.nizd AS izdelProduct, " +
		                                                   "izdel.edizm as izdelMeasure, " +
		                                                   "kurs_val.data as kursDate, " +
		                                                   "kurs_val.kurs as kursKurs " +
		                                                   "FROM ( SELECT kizd, kol, nom_pn, cenad, cena_dol FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad, cena_dol FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on pnakl.kpotr = potr.kpotr " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on pnakl.dataotg = kurs_val.data " +
		                                                   "WHERE (dataotg >= ctod( '{0}' ) and dataotg <= ctod( '{1}' )) " +
														   "and (kurs_val.kval = 1 and kurs_val.data >= ctod( '{0}' )) " +
		                                                   "ORDER BY kurs_val.data DESC";

        private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
                                            "kurs_val.kurs AS kursKurs, " +
                                            "kurs_val.kval as kursValId " +
                                            "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

        /// <summary>
        /// Логика формирование листа записей отчета [Отгрузка по изделию]
        /// </summary>
        public static List<ShipmentByCompany> GetShipmentByCompany(DateTime startDate, DateTime endDate, Company company)
		{
			var shipmentsByCompany = new List<ShipmentByCompany>();

			var sqlResult = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotrKursValUsd, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy")), "SqlResult");
			
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
				var date = (DateTime)row["pnaklDateShipment"];

				var companyName = ((string)row["potrCompany"]).Trim();
				var city = ((string)row["potrCity"]).Trim();

				if (company.Name == companyName 
				    && company.City == city)
				{
					var productName = ((string)row["izdelProduct"]).Trim();
					var measure = ((string)row["izdelMeasure"]).Trim();

					var count = (decimal)row["prrasosCount"];
					var nds = (decimal)row["pnaklNds"];

				    var cost = (decimal)row["prrasosCost"];
				    var costUsd = (decimal)row["prrasosCostUsd"];

				    var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
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
                        costUsd = cost / rateUsd;
                    }

				    var costresult = cost + cost * nds / 100;
				    var costUsdresult = costUsd + costUsd * nds / 100;

				    // Деноминация
				    if (date < new DateTime(2016, 7, 1))
				    {
				        costresult = costresult / 10000;
				    }

				    // Флаг - находится ли строка в результ. выборке
				    bool flag = false;
				    foreach (var shipmentByCompany in shipmentsByCompany)
				    {
				        if (shipmentByCompany.Name == productName
                            && shipmentByCompany.Cost == costresult
				            && shipmentByCompany.CostUsd == costUsdresult)
				        {
				            flag = true;
				            shipmentByCompany.Count += count;
				            break;
				        }
				    }

				    if (!flag)
				    {
				        shipmentsByCompany.Add(new ShipmentByCompany()
				        {
				            Name = productName,
				            Measure = measure,
				            Company = city + ", " + companyName,
				            Cost = costresult,
				            CostUsd = costUsdresult,
				            Count = count,
				            Date = date
				        });
                    }
				}
			}

			shipmentsByCompany.Sort();

			return shipmentsByCompany;
		}

	}
}
