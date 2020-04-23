using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
    /// <summary>
    /// Сервисный класс формирование листа записей отчета [Выпуск]
    /// </summary>
    public class RealaseService
    {
        private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
        private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;
        private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

	    private static readonly string QuerySupply = "SELECT result.kizd as prrasosProductId, " +
	                                                 "result.kol as prrasosCount, " +
	                                                 "result.data as supplyDate, " +
	                                                 "izdel.kizd as izdelProductId, " +
	                                                 "izdel.nizd as izdelProduct, " +
	                                                 "izdel.kat as izdelKatId, " +
	                                                 "kat_prod.Nkat as katName, " +
	                                                 "izdel.vid as izdelTypeId " +
	                                                 "FROM (SELECT kizd, kol, data, pr_v FROM \"" + DbfPathFso + "prrasos.dbf\" " +
	                                                 "union all " +
													 "SELECT kizd, kol, data, pr_v FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\"  ) " +
	                                                 "as result " +
													 "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
													 "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat " +
													 "WHERE data >= ctod( '{0}' ) and data <= ctod( '{1}' ) and pr_v <> 1";


	    private static readonly string QueryRemoving = "SELECT result.kizd as prrasosProductId, " +
	                                                   "result.datar as prrasosUpdateDate, " +
	                                                   "result.vid_sn as typeRemoving, " +
	                                                   "result.kolsn as Kolsn, " +
	                                                   "izdel.kizd as izdelProductId, " +
	                                                   "izdel.nizd as izdelProduct, " +
	                                                   "izdel.kat as izdelKatId, " +
	                                                   "kat_prod.Nkat as katName, " +
	                                                   "izdel.vid as izdelTypeId " +
	                                                   "FROM (SELECT kizd, datar, vid_sn, kolsn FROM \"" + DbfPathFso + "prrasos.dbf\" " +
	                                                   "union all " +
													   "SELECT kizd, datar, vid_sn, kolsn FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
	                                                   "as result " +
													   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
													   "LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat " +
	                                                   "WHERE result.datar >= ctod( '{0}' ) and result.datar <= ctod( '{1}' ) " +
	                                                   "and result.vid_sn = '{2}' and kolsn <> 0";


	    private const string QueryIzdelKatVidProd = "SELECT izdel.kizd as izdelProductId, " +
                                                    "izdel.nizd AS izdelProduct, " +
                                                    "izdel.kat as izdelKatId, " +
                                                    "kat_prod.Nkat as katName, " +
                                                    "izdel.vid as izdelTypeId " +
                                                    "FROM izdel LEFT JOIN kat_prod on izdel.kat = kat_prod.kat";

        /// <summary>
		/// Логика формирование листа записей отчета [Приход по видам изделия]
		/// </summary>
		public static List<Realase> GetRealases(DateTime date)
        {
            var endDate = date.AddDays(-1);
            var startDate = Common.GetBeginOfMonthWithOffset(endDate);

            var realases = new List<Realase>();

            var product = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryIzdelKatVidProd, "Product");

            var exportList = GetListExport(startDate, endDate, product);
			
			var sqlResultprrasosRemoving = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QuerySupply, startDate.ToString("MM/dd/yyyy"),
					endDate.ToString("MM/dd/yyyy"),"v"), "SqlResult");

			foreach (var rowExport in exportList.Select())
            {
                var katProduct = ((string)rowExport["katName"]).Trim();
                var count = (decimal)rowExport["prrasosCount"];

                bool flag = false;
                foreach (var realase in realases)
                {
                    if (realase.Name == katProduct)
                    {
                        flag = true;
                        realase.Count += count;
                        break;
                    }
                }

                if (!flag)
                {
                    realases.Add(new Realase()
                    {
                        Name = katProduct,
                        Count = count
                    });
                }
            }

			//foreach (var rowRemove in sqlResultprrasosRemoving.Select())
			//{
			//    var katProduct = ((string)rowRemove["katName"]).Trim();
			//    var kolsn = (decimal)rowRemove["Kolsn"];

			//    bool flag = false;
			//    foreach (var realase in realases)
			//    {
			//        if (realase.Name == katProduct)
			//        {
			//            flag = true;
			//            if (realase.Count == kolsn)
			//            {
			//                realases.Remove(realase);
			//                break;
			//            }
			//            realase.Count -= kolsn;
			//            break;
			//        }
			//    }
			//    if (!flag)
			//    {
			//        realases.Add(new Realase()
			//        {
			//            Name = katProduct,
			//            Count = -kolsn
			//        });
			//    }
			//}

			realases.Sort();

            return realases;
        }

        /// <summary>
		/// Логика формирование листа записей прихода продукции отчета [Выпуск]
		/// </summary>
		public static DataTable GetListExport(DateTime startDate, DateTime endDate,
            DataTable product)
        {
	        var sqlResultprrasosSupply = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QuerySupply, startDate.ToString("MM/dd/yyyy"),
			        endDate.ToString("MM/dd/yyyy")), "SqlResultprrasosSupply");

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
                if (vidprodTypeId == 12)
                {
                    continue;
                }
                var productName = ((string)rowLink["izdelProduct"]).Trim();
                if (vidprodTypeId == 0 && productName.IndexOf("комплект узл", StringComparison.Ordinal) != 0)
                {
                    continue;
                }
                bufferDataTable.Rows.Add(rowLink.ItemArray);
            }

            return bufferDataTable;
        }
    }
}
