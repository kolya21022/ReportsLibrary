using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Приход по видам изделия]
	/// </summary>
	public class SupplyByTypeProductService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

		private static readonly string QueryIzdelKatVidProdSupply = "SELECT result.kizd as prrasosProductId, " +
		                                             "result.kol as prrasosCount, " +
		                                             "result.data as supplyDate, " +
		                                             "result.nom_pp as prrasosOrdinalCost, " +
		                                             "izdel.kizd as izdelProductId, " +
		                                             "izdel.nizd as izdelProduct, " +
		                                             "izdel.vid as izdelTypeId, " +
		                                             "izdel.kat as izdelKatId, " +
		                                             "kat_prod.Nkat as katName, " +
		                                             "vid_prod.vid as vidprodTypeId, " +
		                                             "vid_prod.nvid as vidprodType " +
		                                             "FROM ( SELECT kizd, kol, data, nom_pp, pr_v FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                             "union all " +
													 "SELECT kizd, kol, data, nom_pp, pr_v FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                             "as result " +
													 "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
													 "LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on vid_prod.vid = izdel.vid " +
													 "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat " +
		                                             "WHERE data >= ctod( '{0}' ) and data <= ctod( '{1}' ) and pr_v <> 1";


		private static readonly string QueryIzdelKatVidProdRemoving = "SELECT result.kizd as prrasosProductId, " +
		                                               "result.nom_pp as prrasosOrdinalCost, " +
		                                               "result.datar as prrasosUpdateDate, " +
		                                               "result.vid_sn as typeRemoving, " +
		                                               "result.kolsn as Kolsn, " +
		                                               "izdel.kizd as izdelProductId, " +
		                                               "izdel.nizd AS izdelProduct, " +
		                                               "izdel.vid as izdelTypeId, " +
		                                               "izdel.kat as izdelKatId, " +
		                                               "kat_prod.Nkat as katName, " +
		                                               "vid_prod.vid as vidprodTypeId, " +
		                                               "vid_prod.nvid as vidprodType " +
		                                               "FROM ( SELECT kizd, datar, nom_pp, vid_sn, kolsn FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                               "union all " +
													   "SELECT kizd, datar, nom_pp, vid_sn, kolsn FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                               "as result  " +
													   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd  " +
													   "LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on vid_prod.vid = izdel.vid " +
													   "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat  " +
		                                               "WHERE result.datar >= ctod( '{0}' ) and result.datar <= ctod( '{1}' ) " +
		                                               "and result.vid_sn = '{2}' and result.kolsn <> 0";


		private const string QueryIzdelKatVidProd = "SELECT izdel.kizd as izdelProductId, " +
		                                            "izdel.nizd AS izdelProduct, " +
		                                            "izdel.vid as izdelTypeId, izdel.kat as izdelKatId, " +
		                                            "kat_prod.Nkat as katName, " +
		                                            "vid_prod.vid as vidprodTypeId, vid_prod.nvid AS vidprodType " +
		                                            "FROM izdel LEFT JOIN vid_prod on izdel.vid = vid_prod.vid " +
		                                            "LEFT JOIN kat_prod on izdel.kat = kat_prod.kat";

		private const string QueryCenaIzd = "SELECT cenaizd.kizd as cenaizdProductId, cenaizd.cena AS cenaizdCost, " +
											"cenaizd.nom_pp AS cenaizdOrdinalCost FROM cenaizd ";

		/// <summary>
		/// Логика формирование листа записей отчета [Приход по видам изделия]
		/// </summary>
		public static List<SupplyByTypeProduct> GetSupplyByTypeProduct(DateTime startDate, DateTime endDate, string type)
		{
			var suppliesByTypeProduct = new List<SupplyByTypeProduct>();

		    var listTypeId = TypeList(type);

		    var product = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryIzdelKatVidProd, "Product");

		    var exportList = GetListExport(startDate, endDate, listTypeId, product);
		    var removingList = GetListRemoving(startDate, endDate, listTypeId, product);

            foreach (var rowExport in exportList.Select())
            {
                var typeId = (decimal)rowExport["izdelTypeId"];
                var typeProduct = typeId != 0 ? ((string)rowExport["vidprodType"]).Trim() : string.Empty;
                var katProduct = ((string)rowExport["katName"]).Trim();

                var productName = ((string)rowExport["izdelProduct"]).Trim();
                var costOne = (decimal)rowExport["cenaizdCost"];
                var count = (decimal)rowExport["prrasosCount"];

                bool flag = false;
                foreach (var supplyByTypeProduct in suppliesByTypeProduct)
                {
                    if (supplyByTypeProduct.CostOne == costOne
                        && supplyByTypeProduct.Name == productName)
                    {
                        flag = true;
                        supplyByTypeProduct.Count += count;
                        break;
                    }
                }

                if (!flag)
                {
                    string t = typeId == 0 ? katProduct : typeProduct;
                    suppliesByTypeProduct.Add(new SupplyByTypeProduct()
                    {
                        Type = t,
                        TypeGroup = type,
                        Category = katProduct,
                        Name = productName,
                        CostOne = costOne,
                        Count = count
                    });
                }
            }

		    foreach (var rowRemove in removingList.Select())
		    {
		        var typeId = (decimal)rowRemove["izdelTypeId"];
		        var typeProduct = typeId != 0 ? ((string)rowRemove["vidprodType"]).Trim() : string.Empty;
		        var katProduct = ((string)rowRemove["katName"]).Trim();

		        var productName = ((string)rowRemove["izdelProduct"]).Trim();
		        var costOne = (decimal)rowRemove["cenaizdCost"];
		        var kolsn = (decimal)rowRemove["Kolsn"];

                bool flag = false;
		        foreach (var supplyByTypeProduct in suppliesByTypeProduct)
		        {
		            if (supplyByTypeProduct.CostOne == costOne
		                && supplyByTypeProduct.Name == productName)
		            {
		                flag = true;
		                if (supplyByTypeProduct.Count == kolsn)
		                {
		                    suppliesByTypeProduct.Remove(supplyByTypeProduct);
		                    break;
		                }
		                supplyByTypeProduct.Count -= kolsn;
		                break;
		            }
		        }
		        if (!flag)
		        {
		            string t = typeId == 0 ? katProduct : typeProduct;
		            suppliesByTypeProduct.Add(new SupplyByTypeProduct()
		            {
		                Type = t,
		                TypeGroup = type,
		                Category = katProduct,
		                Name = productName,
		                CostOne = costOne,
		                Count = -kolsn
		            });
		        }
            }

			suppliesByTypeProduct.Sort();

			return suppliesByTypeProduct;
		}

        /// <summary>
		/// Логика формирование листа записей прихода продукции отчета [Приход по видам продукции]
		/// </summary>
		public static DataTable GetListExport(DateTime startDate, DateTime endDate, List<decimal> listTypeId,
            DataTable product)
        {
	        var sqlResultprrasosSupply = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QueryIzdelKatVidProdSupply, startDate.ToString("MM/dd/yyyy"),
			        endDate.ToString("MM/dd/yyyy")), "SqlResultSupply");

			var bufferDataTable = new DataTable();

            // Копирование столбцов в буферный DataTable
            foreach (DataColumn column in sqlResultprrasosSupply.Columns)
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

            foreach (var rowLink in sqlResultprrasosSupply.Select())
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
		/// Логика формирование листа записей снятой продукции отчета [Приход по видам продукции]
		/// </summary>
		public static DataTable GetListRemoving(DateTime startDate, DateTime endDate, List<decimal> listTypeId,
            DataTable product)
        {
	        var sqlResultprrasosRemoving = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QueryIzdelKatVidProdRemoving, startDate.ToString("MM/dd/yyyy"), endDate.ToString("MM/dd/yyyy"), "v"), "SqlResultRemoving");
			var bufferDataTable = new DataTable();

            // Копирование столбцов в буферный DataTable
            foreach (DataColumn column in sqlResultprrasosRemoving.Columns)
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

            foreach (var rowLink in sqlResultprrasosRemoving.Select())
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
