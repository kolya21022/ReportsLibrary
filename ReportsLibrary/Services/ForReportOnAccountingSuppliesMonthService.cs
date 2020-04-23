using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Для отчета по учету поставок (за месяц)]
	/// </summary>
	public class ForReportOnAccountingSuppliesMonthService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
		static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosIzdelKatVidProd = "SELECT result.kizd as prrasosProductId," +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.Cenad as prrasosCost, " +
		                                                   "pnakl.nomdok as pnaklExportId, " +
		                                                   "pnakl.dataras AS pnaklDateExport, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.stavkands as pnaklNds, " +
		                                                   "pnakl.kval as pnaklValId, " +
		                                                   "pnakl.nom_ttn as pnaklNomTtn, " +
		                                                   "pnakl.nomdog as pnaklContractNumber, " +
		                                                   "pnakl.DataDog as pnaklContractDate, " +
		                                                   "pnakl.otdel as pnaklWorkGuild, " +
		                                                   "pnakl.p_f as pnaklPaymentFormId, " +
		                                                   "pnakl.p_chel as pnaklChelId, " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "izdel.vid as izdelTypeId, " +
		                                                   "izdel.nizd as izdelNameProduct, " +
		                                                   "kat_prod.Nkat as katName, " +
		                                                   "vid_prod.nvid as vidprodType " +
		                                                   "FROM (SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on vid_prod.vid = izdel.vid " +
														   "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat " +
		                                                   "WHERE dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )";

		private const string QueryChel = "SELECT chel.p as chelId, " +
                                         "chel.nchel as chelIntentionBayName FROM chel";

		private static readonly string QueryPrrasosReturnWithIzdelKatVidProd = "SELECT result.kizd as prrasosProductId, " +
		                                                    "result.nom_pp as prrasosOrdinalCost, " +
		                                                    "result.prrasos.kol as prrasosCount, " +
		                                                    "result.nom_dok as prrasosSupplyId, " +
		                                                    "result.Datar as prrasosReturnDate, " +
		                                                    "izdel.kizd as izdelProductId, " +
		                                                    "izdel.vid as izdelTypeId, " +
		                                                    "izdel.nizd as izdelNameProduct, " +
		                                                    "kat_prod.Nkat as katName,  " +
		                                                    "vid_prod.nvid as vidprodType " +
		                                                    "FROM (SELECT nom_dok, kizd, kol, nom_pp, pr_v, datar FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                    "union all " +
															"SELECT nom_dok, kizd, kol, nom_pp, pr_v, datar FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                    "as result " +
															"LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
															"LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on vid_prod.vid = izdel.vid " +
															"LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat " +
		                                                    "WHERE result.pr_v = 1 " +
		                                                    "and result.datar >= ctod( '{0}' ) and result.datar <= ctod( '{1}' )";

		private const string QueryReturnPrrasosWhithPnakl = "SELECT pnakl.kpotr as pnaklCompanyId FROM prrasos " +
                                                            "LEFT JOIN pnakl on prrasos.nom_pn = pnakl.nomdok " +
                                                            "where prrasos.nom_v = {0}";

        private const string QueryIzdelKatVidProd = "SELECT izdel.kizd as izdelProductId, " +
													"izdel.vid as izdelTypeId, " +
													"izdel.nizd as izdelNameProduct, " +
													"kat_prod.Nkat as katName, " +
													"vid_prod.nvid AS vidprodType " +
													"FROM izdel LEFT JOIN vid_prod on izdel.vid = vid_prod.vid " +
													"LEFT JOIN kat_prod on izdel.kat = kat_prod.kat";

		private const string QueryFormaR = "SELECT forma_r.nforma as formaPaymentFormName, " +
										   "forma_r.forma as formaPaymentFormId FROM forma_r";

		private const string QueryPotr = "SELECT potr.kpotr as potrCompanyId, " +
										 "potr.npotr as potrCompanyName, " +
		                                 "potr.gorod as potrCity, " +
										 "potr.kter as potrTerritoryId FROM potr ";

		private const string QueryTerr = "SELECT terr.kter as territoryId, terr.nter as territoryName FROM terr ";

	    private const string QueryCenaIzd = "SELECT cenaizd.kizd as cenaizdProductId, " +
	                                        "cenaizd.cena AS cenaizdCost, " + //используется если был возврат
	                                        "cenaizd.nom_pp AS cenaizdOrdinalCost " +
	                                        "FROM cenaizd ";

	    private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
	                                        "kurs_val.kurs AS kursKurs, " +
	                                        "kurs_val.kval as kursValId " +
	                                        "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

        /// <summary>
        /// Логика формирование листа записей отчета [Для отчета по учету поставок]
        /// </summary>
        public static List<ForReportOnAccountingSuppliesMonth> GetForReportOnAccountingSuppliesMonth(DateTime startDate,
			DateTime endDate, string type)
		{
			var forReportOnAccountingSuppliesMonth = new List<ForReportOnAccountingSuppliesMonth>();

			var listTypeId = TypeList(type);

		    var product = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryIzdelKatVidProd, "Product");
		    var exportList = GetListExport(startDate, endDate, listTypeId, product);
		    var returnList = GetListReturn(startDate, endDate, listTypeId, product);

            var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

            DataTable kursVal = null;
            // Необходимо получать лист курсов валют только если год < 2018 (Увеличение производительности)
            if (startDate <= new DateTime(2017, 12, 31))
            {
                kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
                    query: string.Format(QueryKursVal, startDate.ToString("MM/dd/yyyy")),
                    tableName: "RateVal");
            }

            foreach (var rowExport in exportList.Select())
            {
                var territoryId = ((string)rowExport["potrTerritoryId"]).Trim();
                var territoryName = String.Empty;
                var companyName = ((string)rowExport["potrCompanyName"]).Trim();
                var companyCity = ((string)rowExport["potrCity"]).Trim();
                var ttn = (decimal)rowExport["pnaklNomTtn"];
                var contractNumber = ((string)rowExport["pnaklContractNumber"]).Trim();
                var contractDate = (DateTime)rowExport["pnaklContractDate"];
                var intentionBay = ((string)rowExport["chelIntentionBayName"]).Trim();
                var paymentForm = ((string)rowExport["formaPaymentFormName"]).Trim();
                var workGuild = ((string)rowExport["pnaklWorkGuild"]).Trim();
                var productName = ((string)rowExport["izdelNameProduct"]).Trim();

                var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
                // Из id города формируем id страны
                var territoryIdzero = territoryIdTwo + "00";

                // Получение названия страны по сформированному id
                foreach (var rowTeritory in territories.Select())
                {
                    if (((string)rowTeritory["territoryId"]).Trim() == territoryIdzero)
                    {
                        territoryName = ((string)rowTeritory["territoryName"]).Trim();
                        break;
                    }
                }
                var typeId = (decimal)rowExport["izdelTypeId"];

                string typeProduct = typeId != 0 ? ((string)rowExport["vidprodType"]).Trim() : string.Empty;
                var category = ((string)rowExport["katName"]).Trim();

                var nds = (decimal)rowExport["pnaklNds"];
                var cost = (decimal)rowExport["prrasosCost"];
                var count = (decimal)rowExport["prrasosCount"];

                var date = (DateTime)rowExport["pnaklDateExport"];
                var valId = (decimal)rowExport["pnaklValId"];

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

                var summNds = decimal.Round((cost / 100 * nds) * count, 2);
                var costResult = decimal.Round(cost * count, 2);

                // Флаг - находится ли строка в результ. выборке
                bool flag = false;
                string t = typeId == 0 ? category : typeProduct;
                foreach (var forReportOnAccountingSupply in forReportOnAccountingSuppliesMonth)
                {
                    if (forReportOnAccountingSupply.Territory == territoryName
                        && forReportOnAccountingSupply.Category == category
                        && forReportOnAccountingSupply.Type == t
                        && forReportOnAccountingSupply.ContractNumber == contractNumber
                        && forReportOnAccountingSupply.Name == productName)
                    {
                        flag = true;
                        forReportOnAccountingSupply.Count += count;
                        forReportOnAccountingSupply.Cost += costResult;
                        forReportOnAccountingSupply.SummNds += summNds;
                        break;
                    }
                }

                if (!flag)
                {
                    forReportOnAccountingSuppliesMonth.Add(new ForReportOnAccountingSuppliesMonth()
                    {
                        TypeGroup = type,
                        Type = t,
                        Territory = territoryName,
                        Category = category,
                        Cost = costResult,
                        Count = count,
                        SummNds = summNds,
                        ContractDate = contractDate,
                        ContractNumber = contractNumber,
                        IntentionBay = intentionBay,
                        ShipmentDate = date,
                        NameCompany = companyCity + ", " + companyName,
                        Ttn = ttn,
                        PaymentForm = paymentForm,
                        WorkGuild = workGuild,
                        Name = productName
                    });
                }
            }

		    foreach (var rowReturn in returnList.Select())
		    {
		        var territoryName = string.Empty;
		        var productName = ((string)rowReturn["izdelNameProduct"]).Trim();

		        DataTable seachcompany;
		        var returnId = (decimal)rowReturn["prrasosSupplyId"];
		        //Поиск в действующей базе компании которая сделала возврат
		        var idCompany = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		            query: string.Format(QueryReturnPrrasosWhithPnakl, returnId),
		            tableName: "IdCompany");
		        var company = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryPotr, "Company");
		        if (idCompany.Rows.Count != 0)
		        {
		            seachcompany = DataTableHelper.JoinTwoDataTablesOnOneColumn(idCompany, "pnaklCompanyId",
		                company, "potrCompanyId", 1);
		        }
		        else
		        {
		            //Поиск в архиве компании которая сделала возврат
		            var idCompanyArhiv = DataTableHelper.LoadDataTableByQuery(DbfPathFsoArhiv,
		                query: string.Format(QueryReturnPrrasosWhithPnakl, returnId),
		                tableName: "IdCompany");
		            seachcompany = DataTableHelper.JoinTwoDataTablesOnOneColumn(idCompanyArhiv, "pnaklCompanyId",
		                company, "potrCompanyId", 1);
		        }
		        var territoryId = ((string)seachcompany.Rows[0]["potrTerritoryId"]).Trim();

		        var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
		        // Из id города формируем id страны
		        var territoryIdzero = territoryIdTwo + "00";

		        // Получение названия страны по сформированному id
		        foreach (var rowTeritory in territories.Select())
		        {
		            if (((string)rowTeritory["territoryId"]).Trim() == territoryIdzero)
		            {
		                territoryName = ((string)rowTeritory["territoryName"]).Trim();
		                break;
		            }
		        }

		        var typeId = (decimal)rowReturn["izdelTypeId"];

		        string typeProduct = typeId != 0 ? ((string)rowReturn["vidprodType"]).Trim() : string.Empty;
		        var category = ((string)rowReturn["katName"]).Trim();

		        // Если был возврат то берется цена прихода
		        var cost = (decimal)rowReturn["cenaizdCost"];
		        var count = (-1) * (decimal)rowReturn["prrasosCount"];

		        var costResult = decimal.Round(cost * count, 2);

		        // Флаг - находится ли строка в результ. выборке
		        bool flag = false;
		        string t = typeId == 0 ? category : typeProduct;
		        foreach (var forReportOnAccountingSupply in forReportOnAccountingSuppliesMonth)
		        {
		            if (forReportOnAccountingSupply.Territory == territoryName
		                && forReportOnAccountingSupply.Category == category
		                && forReportOnAccountingSupply.Type == t
		                && forReportOnAccountingSupply.ContractNumber == string.Empty
		                && forReportOnAccountingSupply.Name == productName)
		            {
		                flag = true;
		                forReportOnAccountingSupply.Count += count;
		                forReportOnAccountingSupply.Cost += costResult;
		                break;
		            }
		        }

		        if (!flag)
		        {
		            forReportOnAccountingSuppliesMonth.Add(new ForReportOnAccountingSuppliesMonth()
		            {
		                TypeGroup = type,
		                Type = t,
		                Territory = territoryName,
		                Category = category,
		                Cost = costResult,
		                Count = count,
		                SummNds = 0M,
		                ContractDate = new DateTime(1899, 12, 30),
		                ContractNumber = string.Empty,
		                IntentionBay = string.Empty,
		                ShipmentDate = new DateTime(1899, 12, 30),
		                NameCompany = string.Empty,
		                Ttn = 0M,
		                PaymentForm = string.Empty,
		                WorkGuild = string.Empty,
		                Name = productName
		            });
		        }
            }

			forReportOnAccountingSuppliesMonth.Sort();

			return forReportOnAccountingSuppliesMonth;
		}

        /// <summary>
        /// Логика формирование листа записей расходов отчета [Для отчета по учету поставок]
        /// </summary>
        public static DataTable GetListExport(DateTime startDate, DateTime endDate, List<decimal> listTypeId,
            DataTable product)
        {
            var linkWhithpnaklPrrasosProduct = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
	            string.Format(QueryPnaklPrrasosIzdelKatVidProd, startDate.ToString("MM/dd/yyyy"),
		            endDate.ToString("MM/dd/yyyy")), "SqlResultPnaklPrrasosProduct");

            var bufferDataTable = new DataTable();

            // Копирование столбцов в буферный DataTable
            foreach (DataColumn column in linkWhithpnaklPrrasosProduct.Columns)
            {
                if (bufferDataTable.Columns[column.ColumnName] == null)
                {
                    bufferDataTable.Columns.Add(column.ColumnName, column.DataType);
                }
                else
                {
                    throw new ApplicationException();
                }
            }

            foreach (var rowLink in linkWhithpnaklPrrasosProduct.Select())
            {

                var vidprodTypeId = (decimal)rowLink["izdelTypeId"];
                foreach (var typeId in listTypeId)
                {
                    if (vidprodTypeId == typeId)
                    {
                        bufferDataTable.Rows.Add(rowLink.ItemArray);
                        break;
                    }
                }
            }

            var chel = DataTableHelper.LoadDataTableByQuery(DbfPathFso, QueryChel, "Chel");
            var linkWhithChel = DataTableHelper.JoinTwoDataTablesOnOneColumn
                (bufferDataTable, "pnaklChelId", chel, "chelId", 1);

            var company = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryPotr, "Company");
            var linkWhithCompany = DataTableHelper.JoinTwoDataTablesOnOneColumn
                (linkWhithChel, "pnaklCompanyId", company, "potrCompanyId", 1);

            var paymentForm = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryFormaR, "PaymentForm");
            var result = DataTableHelper.JoinTwoDataTablesOnOneColumn
                (linkWhithCompany, "pnaklPaymentFormId", paymentForm, "formaPaymentFormId", 1);

            return result;
        }

        /// <summary>
		/// Логика формирование листа записей возврата отчета [Для отчета по учету поставок]
		/// </summary>
		public static DataTable GetListReturn(DateTime startDate, DateTime endDate, List<decimal> listTypeId,
            DataTable product)
        {
            var linkWhithPrrasosReturnWithProduct = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
	            query: string.Format(QueryPrrasosReturnWithIzdelKatVidProd, startDate.ToString("MM/dd/yyyy"),
		            endDate.ToString("MM/dd/yyyy")),
	            tableName: "SqlResultPrrasosReturnWithProduct");

            var bufferDataTable = new DataTable();

            // Копирование столбцов в буферный DataTable
            foreach (DataColumn column in linkWhithPrrasosReturnWithProduct.Columns)
            {
                if (bufferDataTable.Columns[column.ColumnName] == null)
                {
                    bufferDataTable.Columns.Add(column.ColumnName, column.DataType);
                }
                else
                {
                    throw new ApplicationException();
                }
            }

            foreach (var rowLink in linkWhithPrrasosReturnWithProduct.Select())
            {

                var vidprodTypeId = (decimal)rowLink["izdelTypeId"];
                foreach (var typeId in listTypeId)
                {
                    if (vidprodTypeId == typeId)
                    {
                        bufferDataTable.Rows.Add(rowLink.ItemArray);
                        break;
                    }
                }
            }

            var costProduct = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryCenaIzd, "CostProduct");
            var result = DataTableHelper.LeftJoin_TwoTable_By_TwoFields<decimal?, decimal?>(bufferDataTable,
                "prrasosProductId", "prrasosOrdinalCost", costProduct, "cenaizdProductId", "cenaizdOrdinalCost");

            return result;
        }

        /// <summary>
        /// Формирование листа Id типов изделия для указанной группы
        /// </summary>
        public static List<decimal> TypeList(string type)
		{
			List<decimal> listTypeId = new List<decimal>();
			if (type == "Узлы")
			{
				listTypeId.Add(7M);
				listTypeId.Add(8M);
				listTypeId.Add(9M);
			}
			else if (type == "Металлорежущие станки")
			{
				listTypeId.Add(1M);
				listTypeId.Add(2M);
				listTypeId.Add(3M);
				listTypeId.Add(4M);
				listTypeId.Add(5M);
				listTypeId.Add(6M);
				listTypeId.Add(14M);
			}
			else if (type == "Деревообрабатывающие станки")
			{
				listTypeId.Add(10M);
				listTypeId.Add(11M);
			}
			else if (type == "ТНП")
			{
				listTypeId.Add(12M);
			}
			else if (type == "Прочая")
			{
				listTypeId.Add(0M);
			}
			return listTypeId;
		}
	}
}


