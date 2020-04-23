using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Расход по видам изделия]
	/// </summary>
	public class ExportByTypeProductService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryPnaklPrrasosIzdelKatVidProd = "SELECT result.kizd as prrasosProductId, " +
		                                                   "result.kol as prrasosCount, " +
		                                                   "result.Cenad as prrasosCost, " +
		                                                   "pnakl.nomdok as pnaklExportId, " +
		                                                   "pnakl.dataras as pnaklDateExport, " +
		                                                   "pnakl.kpotr as pnaklCompanyId, " +
		                                                   "pnakl.stavkands as pnaklNds, " +
		                                                   "pnakl.kval as pnaklValId, " +
		                                                   "izdel.kizd as izdelProductId, " +
		                                                   "izdel.nizd as izdelProduct, " +
		                                                   "izdel.vid_stat as vidstat, " +
		                                                   "izdel.vid as izdelTypeId, " +
		                                                   "izdel.kat as izdelKatId, " +
		                                                   "kat_prod.Nkat as katName, " +
		                                                   "vid_prod.vid as vidprodTypeId, " +
		                                                   "vid_prod.nvid as vidprodType  " +
		                                                   "FROM ( SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all " +
														   "SELECT kizd, kol, nom_pn, cenad FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on pnakl.nomdok = result.nom_pn " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on izdel.kizd = result.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on izdel.vid = vid_prod.vid " +
														   "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on izdel.kat = kat_prod.kat " +
		                                                   "WHERE dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )";

		private static readonly string QueryPrrasosReturnWithIzdelKatVidProd = "SELECT result.kizd as prrasosProductId, " +
		                                                    "result.nom_pp as prrasosOrdinalCost, " +
		                                                    "result.kol as prrasosCount, " +
		                                                    "result.datar as prrasosReturnDate, " +
		                                                    "izdel.nizd as izdelProduct, " +
		                                                    "izdel.vid_stat as vidstat, " +
		                                                    "izdel.vid as izdelTypeId, " +
		                                                    "izdel.kat as izdelKatId, " +
		                                                    "kat_prod.Nkat as katName, " +
		                                                    "vid_prod.vid as vidprodTypeId, " +
		                                                    "vid_prod.nvid as vidprodType " +
															"FROM ( SELECT kizd, nom_pp, kol, cenad, datar, pr_v FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                    "union all " +
															"SELECT kizd, nom_pp, kol, cenad, datar, pr_v FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                    "as result " +
		                                                    "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on izdel.kizd = result.kizd " +
															"LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on izdel.vid = vid_prod.vid " +
															"LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on izdel.kat = kat_prod.kat " +
															"WHERE result.pr_v = 1 and result.datar >= ctod( '{0}' ) and result.datar <= ctod( '{1}' )";

		private const string QueryPotr = "SELECT potr.kpotr as potrCompanyId, potr.kter as potrTerritoryId FROM potr ";

		private const string QueryTerr = "SELECT terr.kter as territoryId, terr.nter as terrName FROM terr ";

		private const string QueryCenaIzd = "SELECT cenaizd.kizd as cenaizdProductId, " +
		                                    "cenaizd.cena AS cenaizdCost, " + //используется если был возврат
		                                    "cenaizd.nom_pp AS cenaizdOrdinalCost " +
		                                    "FROM cenaizd ";

	    private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
	                                        "kurs_val.kurs AS kursKurs, " +
	                                        "kurs_val.kval as kursValId " +
	                                        "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

        /// <summary>
        /// Логика формирование листа записей отчета [Расход по видам продукции]
        /// </summary>
        public static List<ExportByTypeProduct> GetExportByTypeProduct(DateTime startDate, DateTime endDate, string type)
		{
			var exportsByTypeProduct = new List<ExportByTypeProduct>();

			var listTypeId = TypeList(type);

            var exportList = GetListExport(startDate, endDate, listTypeId);
		    var returnList = GetListReturn(startDate, endDate, listTypeId);

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
		        var typeId = (decimal)rowExport["izdelTypeId"];
		        var vidStatId = (decimal)rowExport["vidStat"];

		        string typeProduct;
		        var katProduct = ((string)rowExport["katName"]).Trim();

		        // Доп условие, разбиение группы(приказ сверху)
		        if (vidStatId == 18)
		        {
		            typeProduct = "токарные с ЧПУ";
		        }
		        else
		        {
		            typeProduct = typeId != 0 ? ((string)rowExport["vidprodType"]).Trim() : "";
		        }

		        var nds = (decimal)rowExport["pnaklNds"];
		        var cost = (decimal)rowExport["prrasosCost"];

		        var date = (DateTime)rowExport["pnaklDateExport"];
		        var valId = (decimal)rowExport["pnaklValId"];

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

                var productName = ((string)rowExport["izdelProduct"]).Trim();
		        var count = (decimal)rowExport["prrasosCount"];

		        // Флаг - находится ли строка в результ. выборке
		        bool flag = false;
		        foreach (var exportByTypeProduct in exportsByTypeProduct)
		        {
		            if (exportByTypeProduct.CostOne == cost
		                && exportByTypeProduct.Name == productName
		                && exportByTypeProduct.SummNdsOne == cost / 100 * nds)
		            {
		                flag = true;
		                exportByTypeProduct.Count += count;
		                break;
		            }
		        }

		        if (!flag)
		        {
		            var t = (typeId == 0 && vidStatId != 18) ? katProduct : typeProduct;
		            exportsByTypeProduct.Add(new ExportByTypeProduct()
		            {
		                CostOne = cost,
		                Count = count,
		                Name = productName,
		                Type = t,
		                TypeGroup = type,
		                SummNdsOne = cost / 100 * nds,
		                Category = katProduct
		            });
		        }
            }

		    foreach (var rowReturn in returnList.Select())
		    {
		        var typeId = (decimal)rowReturn["izdelTypeId"];
		        var vidStatId = (decimal)rowReturn["vidStat"];

		        string typeProduct;
		        var katProduct = ((string)rowReturn["katName"]).Trim();

		        // Доп условие, разбиение группы(приказ сверху)
		        if (vidStatId == 18)
		        {
		            typeProduct = "токарные с ЧПУ";
		        }
		        else
		        {
		            typeProduct = typeId != 0 ? ((string)rowReturn["vidprodType"]).Trim() : "";
		        }

		        // Если был возврат то берется цена прихода
		        var cost = (decimal)rowReturn["cenaizdCost"];
                var productName = ((string)rowReturn["izdelProduct"]).Trim(); 
                var count = (decimal)rowReturn["prrasosCount"];

		        // Флаг - находится ли строка в результ. выборке
		        bool flag = false;
		        foreach (var exportByTypeProduct in exportsByTypeProduct)
		        {
		            if (exportByTypeProduct.CostOne == cost
		                && exportByTypeProduct.Name == productName
		                && exportByTypeProduct.SummNdsOne == 0M)
		            {
		                flag = true;
		                // Если был возврат то кол-во минусуется
                        exportByTypeProduct.Count -= count;
		                break;
		            }
		        }

		        if (!flag)
		        {
		            var t = (typeId == 0 && vidStatId != 18) ? katProduct : typeProduct;
		            exportsByTypeProduct.Add(new ExportByTypeProduct()
		            {
		                CostOne = cost,
		                // Если был возврат то кол-во c минусом
                        Count = -1 * count,
		                Name = productName,
		                Type = t,
		                TypeGroup = type,
		                SummNdsOne = 0M,
		                Category = katProduct
		            });
		        }
		    }
			exportsByTypeProduct.Sort();

			return exportsByTypeProduct;
		}

		/// <summary>
		/// Логика формирование листа записей отчета [Расход по видам продукции c территорией] 
		/// </summary>
		public static List<ExportByTypeProduct> GetExportByTypeProductWhithTerritory(DateTime startDate,
			DateTime endDate, string type)
		{
            var exportsByTypeProduct = new List<ExportByTypeProduct>();

            var listTypeId = TypeList(type);

            var exportList = GetListExport(startDate, endDate, listTypeId);
            var returnList = GetListReturn(startDate, endDate, listTypeId);

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
                var typeId = (decimal)rowExport["izdelTypeId"];
                var vidStatId = (decimal)rowExport["vidStat"];

                string typeProduct;
                
                // Доп условие, разбиение группы(приказ сверху)
                if (vidStatId == 18)
                {
                    typeProduct = "токарные с ЧПУ";
                }
                else
                {
                    typeProduct = typeId != 0 ? ((string)rowExport["vidprodType"]).Trim() : "";
                }

                var nds = (decimal)rowExport["pnaklNds"];
                var cost = (decimal)rowExport["prrasosCost"];

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

                var katProduct = ((string)rowExport["katName"]).Trim();
                var territoryName = string.Empty;

                var territoryId = ((string)rowExport["potrTerritoryId"]).Trim();
                // Из id города формируем id страны
                var territoryIdzero = territoryId.Substring(0, 1) + territoryId.Substring(1, 1) + "00";

                // Получение названия страны по сформированному id
                foreach (var rowTeritory in territories.Select())
                {
                    if (((string)rowTeritory["territoryId"]).Trim() == territoryIdzero)
                    {
                        territoryName = ((string)rowTeritory["terrName"]).Trim();
                        break;
                    }
                }

                var productName = ((string)rowExport["izdelProduct"]).Trim();
                var count = (decimal)rowExport["prrasosCount"];

                // Флаг - находится ли строка в результ. выборке
                bool flag = false;
                foreach (var exportByTypeProduct in exportsByTypeProduct)
                {
                    if (exportByTypeProduct.CostOne == cost
                        && exportByTypeProduct.Name == productName
                        && exportByTypeProduct.SummNdsOne == cost / 100 * nds)
                    {
                        flag = true;
                        exportByTypeProduct.Count += count;
                        break;
                    }
                }

                if (!flag)
                {
                    string t = (typeId == 0 && vidStatId != 18) ? katProduct : typeProduct;

                    exportsByTypeProduct.Add(new ExportByTypeProduct()
                    {
                        CostOne = cost,
                        Count = count,
                        Name = productName,
                        Type = t,
                        TypeGroup = type,
                        SummNdsOne = cost / 100 * nds,
                        Territory = territoryName,
                        Category = katProduct
                    });
                }
            }

            foreach (var rowReturn in returnList.Select())
            {
                var typeId = (decimal)rowReturn["izdelTypeId"];
                var vidStatId = (decimal)rowReturn["vidStat"];

                string typeProduct;
                var katProduct = ((string)rowReturn["katName"]).Trim();

                // Доп условие, разбиение группы(приказ сверху)
                if (vidStatId == 18)
                {
                    typeProduct = "токарные с ЧПУ";
                }
                else
                {
                    typeProduct = typeId != 0 ? ((string)rowReturn["vidprodType"]).Trim() : "";
                }

                // Если был возврат то берется цена прихода
                var cost = (decimal)rowReturn["cenaizdCost"];
                var productName = ((string)rowReturn["izdelProduct"]).Trim();
                var count = (decimal)rowReturn["prrasosCount"];

                // Флаг - находится ли строка в результ. выборке
                bool flag = false;
                foreach (var exportByTypeProduct in exportsByTypeProduct)
                {
                    if (exportByTypeProduct.CostOne == cost
                        && exportByTypeProduct.Name == productName
                        && exportByTypeProduct.SummNdsOne == 0M)
                    {
                        flag = true;
                        // Если был возврат то кол-во минусуется
                        exportByTypeProduct.Count -= count;
                        break;
                    }
                }
                if (!flag)
                {
                    string t = (typeId == 0 && vidStatId != 18) ? katProduct : typeProduct;

                    exportsByTypeProduct.Add(new ExportByTypeProduct()
                    {
                        CostOne = cost,
                        // Если был возврат то кол-во c минусом
                        Count = -count,
                        Name = productName,
                        Type = t,
                        TypeGroup = type,
                        SummNdsOne = 0M,
                        Territory = string.Empty,
                        Category = katProduct
                    });
                }
            }
            exportsByTypeProduct.Sort();

			return exportsByTypeProduct;
		}

        /// <summary>
		/// Логика формирование листа записей расходов отчета [Расход по видам продукции]
		/// </summary>
		public static DataTable GetListExport(DateTime startDate, DateTime endDate, List<decimal> listTypeId)
        {
	        var sqlResultpnaklPrrasos = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QueryPnaklPrrasosIzdelKatVidProd, startDate.ToString("MM/dd/yyyy"),
			        endDate.ToString("MM/dd/yyyy")), "SqlResultpnaklPrrasos");

			var bufferDataTable = new DataTable();

            // Копирование столбцов в буферный DataTable
            foreach (DataColumn column in sqlResultpnaklPrrasos.Columns)
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

            foreach (var rowLink in sqlResultpnaklPrrasos.Select())
            {
                var vidprodTypeId = (decimal) rowLink["izdelTypeId"];
                foreach (var typeId in listTypeId)
                {
                    if (vidprodTypeId == typeId)
                    {
                        bufferDataTable.Rows.Add(rowLink.ItemArray);
                        break;
                    }
                }
            }

            var company = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryPotr, "Company");
            var result = DataTableHelper.JoinTwoDataTablesOnOneColumn
                (bufferDataTable, "pnaklCompanyId", company, "potrCompanyId", 1);

            return result;
        }

        /// <summary>
		/// Логика формирование листа записей возврата отчета [Расход по видам продукции]
		/// </summary>
		public static DataTable GetListReturn(DateTime startDate, DateTime endDate, List<decimal> listTypeId)
        {
	        var sqlResultprrasosReturn = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QueryPrrasosReturnWithIzdelKatVidProd, startDate.ToString("MM/dd/yyyy"),
			        endDate.ToString("MM/dd/yyyy")), "SqlResultprrasosReturn");

			var bufferDataTable = new DataTable();

            // Копирование столбцов в буферный DataTable
            foreach (DataColumn column in sqlResultprrasosReturn.Columns)
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

            foreach (var rowLink in sqlResultprrasosReturn.Select())
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
