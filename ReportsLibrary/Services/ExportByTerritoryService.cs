using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
    /// <summary>
    /// Сервисный класс формирование листа записей отчета [Экспорт по территориям]
    /// TODO без возвратов
    /// </summary>
    public class ExportByTerritoryService
    {
        private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
        private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;

        private static readonly string DbfPathSkl = Properties.Settings.Default.FoxproDbFolder_Skl;
        private static readonly string DbfPathBuh = Properties.Settings.Default.FoxproDbFolder_Buh;

        private static readonly string DbfPathBuhArhiv = Properties.Settings.Default.FoxproDbFolder_BuhArhiv;
        private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

        public static List<ExportByTerritory> ExportByTerritory;

	    private static readonly string QueryOplPotrKursValUsd = "SELECT opl.nom_dok as oplExportId, " +
	                                                            "opl.sklad as oplSklad, " +
	                                                            "opl.data as oplDateExport, " +
	                                                            "opl.sum_d as oplCost, " +
	                                                            "opl.kpotr as oplCompanyId, " +
	                                                            "potr.kpotr as potrCompanyId, " +
	                                                            "potr.kter as potrTerritoryId, " +
	                                                            "kurs_val.data as kursDate, " +
	                                                            "kurs_val.kurs AS kursKurs " +
	                                                            "FROM \"" + DbfPathSkl + "opl.dbf\" " +
																"LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on potr.kpotr = opl.kpotr " +
																"LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as  kurs_val on kurs_val.data = opl.data " +
																"WHERE (opl.p_s not like '{0}' and (opl.data >= ctod( '{1}' ) and opl.data <= ctod( '{2}' ) or opl.data >= ctod( '{3}' ) " +
																"and opl.data <= ctod( '{4}' ))) " +
																"and (kurs_val.kval = 1 and kurs_val.data >= ctod( '{0}' )) " +
	                                                            "ORDER BY kurs_val.data DESC";

        private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
                                            "kurs_val.kurs AS kursKurs, " +
                                            "kurs_val.kval as kursValId " +
                                            "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

	    private static readonly string QueryRas07PotrKursValUsd = "SELECT result.data as ras07DateExport, " +
	                                                "result.kpotr as ras07CompanyId, " +
	                                                "result.cenadog as ras07Cost, " +
	                                                "result.nds as ras07Nds, " +
	                                                "result.kol as ras07Count, " +
	                                                "potr.kpotr as potrCompanyId, " +
	                                                "potr.kter as potrTerritoryId, " +
	                                                "kurs_val.data as kursDate, " +
	                                                "kurs_val.kurs as kursKurs " +
	                                                "FROM (SELECT data, kol, cenadog, nds, kpotr FROM \"" + DbfPathBuh + "ras07.dbf\" " +
													"union all " +
													"SELECT data, kol, cenadog, nds, kpotr FROM \"" + DbfPathBuhArhiv + "ras07.dbf\" ) " +
	                                                "as result " +
													"LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on potr.kpotr = result.kpotr " +
													"LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on kurs_val.data = result.data " +
	                                                "WHERE (result.kpotr <> 0 and result.cenadog <> 0 and (result.data >= ctod( '{0}' ) " +
													"and result.data <= ctod( '{1}' ) or result.data >= ctod( '{2}' ) " +
													"and result.data <= ctod( '{3}' ))) " +
													"and (kurs_val.kval = 1 and kurs_val.data >= ctod( '{0}' )) " +
	                                                "ORDER BY kurs_val.data DESC";


	    private static readonly string QueryPnaklPrrasosIzdelPotrKursValUsd = "SELECT result.kizd as prrasosProductId, " +
	                                                                          "result.kol as prrasosCount, " +
	                                                                          "result.cenad as prrasosCost, " +
	                                                                          "result.cena_dol as prrasosCostUsd, " +
	                                                                          "result.cena_val as prrasosCostVal, " +
	                                                                          "pnakl.nomdok as pnaklExportId, " +
	                                                                          "pnakl.dataras as pnaklDateExport, " +
	                                                                          "pnakl.kpotr as pnaklCompanyId, " +
	                                                                          "pnakl.Stavkands as pnaklNds, " +
	                                                                          "pnakl.kval as pnaklValId, " +
	                                                                          "izdel.kizd as izdelProductId, " +
	                                                                          "izdel.pr_usl as izdelProvide, " +
	                                                                          "izdel.pr_t as izdelTehnology, " +
	                                                                          "potr.kpotr as potrCompanyId, " +
	                                                                          "potr.kter as potrTerritoryId, " +
	                                                                          "kurs_val.data as kursDate, " +
	                                                                          "kurs_val.kurs as kursKurs " +
																			  "FROM (SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFso + "prrasos.dbf\" " +
	                                                                          "union all " +
																			  "SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
	                                                                          "as result " +
																			  "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
																			  "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd =  izdel.kizd " +
																			  "LEFT JOIN \"" + DbfPathBase + "potr.dbf\"  as potr on pnakl.kpotr = potr.kpotr " +
																			  "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on pnakl.dataras = kurs_val.data  " +
																			  "WHERE (dataras >= ctod('{0}') and dataras <= ctod('{1}') " +
																			  "or dataras >= ctod('{2}') and dataras <= ctod('{3}')) " +
																			  "and kurs_val.kval = 1 and kurs_val.data >= ctod('{0}') " +
	                                                                          "ORDER BY kurs_val.data DESC";

        private const string QueryTerr = "SELECT terr.kter as territoryId, terr.nter as territoryName FROM terr ";

        /// <summary>
        /// Логика формирование листа записей материалов или товаров отчета [Учет экспорта]
        /// </summary>
        public static List<ExportByTerritory> GetExportByTerritoryAll(DateTime startDate,
            DateTime endDate)
        {
            ExportByTerritory = new List<ExportByTerritory>();
            GetMaterialOrProduct(startDate, endDate, true);
            GetMaterialOrProduct(startDate, endDate, false);
            GetEquipment(startDate, endDate);
            GetFinishedProducts(startDate, endDate);
            return ExportByTerritory;
        }

        /// <summary>
        /// Логика формирование листа записей материалов или товаров отчета [Учет экспорта материалов] или 
        /// [Учет экспорта товаров]
        /// </summary>
        public static List<ExportByTerritory> GetExportByTerritoryMaterialOrProduct(DateTime startDate,
            DateTime endDate, bool isMaterial)
        {
            ExportByTerritory = new List<ExportByTerritory>();
            GetMaterialOrProduct(startDate, endDate, isMaterial);
            return ExportByTerritory;
        }

        /// <summary>
        /// Логика формирование листа записей материалов или товаров отчета [Учет экспорта оборудования]
        /// </summary>
        public static List<ExportByTerritory> GetExportByTerritoryEquipment(DateTime startDate,
            DateTime endDate)
        {
            ExportByTerritory = new List<ExportByTerritory>();
            GetEquipment(startDate, endDate);
            return ExportByTerritory;
        }

        /// <summary>
        /// Логика формирование листа записей материалов или товаров отчета [Учет экспорта готовой продукции]
        /// </summary>
        public static List<ExportByTerritory> GetExportByTerritoryFinishedProductsByDateShipment(DateTime startDate,
            DateTime endDate)
        {
            ExportByTerritory = new List<ExportByTerritory>();
            GetFinishedProductsByDateShipment(startDate, endDate);
            return ExportByTerritory;
        }

        /// <summary>
        /// Логика формирование листа записей материалов или товаров отчета [Учет экспорта готовой продукции] с ценой на дату отгрузки
        /// </summary>
        public static List<ExportByTerritory> GetExportByTerritoryFinishedProducts(DateTime startDate,
            DateTime endDate)
        {
            ExportByTerritory = new List<ExportByTerritory>();
            GetFinishedProducts(startDate, endDate);
            return ExportByTerritory;
        }

        /// <summary>
        /// Логика формирование листа записей материалов или товаров
        /// </summary>
        public static void GetMaterialOrProduct(DateTime startDate,
            DateTime endDate, bool isMaterial)
        {
            var endDateYearMinusDay = endDate.AddDays(-1);
            var startDateYaer = new DateTime(endDateYearMinusDay.Year, 1, 1);
            var oldYearStartDate = new DateTime(endDateYearMinusDay.Year - 1, 1, 1);
            var oldYearEndDate = endDateYearMinusDay.AddYears(-1);

            var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

	        var sqlResultOpl = DataTableHelper.LoadDataTableByQuery(DbfPathSkl,
		        string.Format(QueryOplPotrKursValUsd, "d", oldYearStartDate.ToString("MM/dd/yyyy"), oldYearEndDate.ToString("MM/dd/yyyy"),
			        startDateYaer.ToString("MM/dd/yyyy"), endDateYearMinusDay.ToString("MM/dd/yyyy")), "SqlResultOpl");

			foreach (var row in sqlResultOpl.Select())
            {
                var sklad = (decimal)row["oplSklad"];
                //Материал
                if (isMaterial)
                {
                    if (sklad == 38M) continue;
                    var rate = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
                    var cost = (decimal)row["oplCost"];
                    var costUsd = cost / rate;
                    var date = (DateTime)row["oplDateExport"];

                    // Деноминация
                    if (date < new DateTime(2016, 7, 1))
                    {
                        cost = cost / 10000;
                    }

                    var territoryName = string.Empty;
                    var territoryId = ((string)row["potrTerritoryId"]).Trim();
                    var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
                    var territoryIdZero = territoryIdTwo == "50" ? territoryId : territoryIdTwo + "00";

                    if (territoryIdTwo == "15")
                    {
                        continue;
                    }

                    // Получение названия страны по сформированному id
                    foreach (var rowTeritory in territories.Select())
                    {

                        if (((string)rowTeritory["territoryId"]).Trim() == territoryIdZero)
                        {
                            territoryName = ((string)rowTeritory["territoryName"]).Trim();
                            break;
                        }
                    }

                    var flag = false;
                    foreach (var export in ExportByTerritory)
                    {
                        if (export.Country == territoryName)
                        {
                            flag = true;

                            // заполнение месяца
                            if (endDateYearMinusDay.Month == date.Month)
                            {
                                if (endDateYearMinusDay.Year == date.Year)
                                {
                                    export.MonthToday += cost;
                                    export.MonthUsdToday += costUsd / 1000;

                                    export.YearToday += cost;
                                    export.YearUsdToday += costUsd / 1000;
                                }
                                else
                                {
                                    export.MonthOld += cost;
                                    export.MonthUsdOld += costUsd / 1000;

                                    export.YearOld += cost;
                                    export.YearUsdOld += costUsd / 1000;
                                }
                            }

                            // заполнение года если месяц больше
                            if (endDateYearMinusDay.Month > date.Month)
                            {
                                if (endDateYearMinusDay.Year == date.Year)
                                {
                                    export.YearToday += cost;
                                    export.YearUsdToday += costUsd / 1000;
                                }
                                else
                                {
                                    export.YearOld += cost;
                                    export.YearUsdOld += costUsd / 1000;
                                }
                            }

                            break;
                        }
                    }

                    if (flag == false)
                    {
                        var export = new ExportByTerritory
                        {
                            Country = territoryName,
                            MonthCountToday = 0M,
                            YearCountToday = 0M,
                            MonthCountOld = 0M,
                            YearCountOld = 0M
                        };

                        // заполнение месяца
                        if (endDateYearMinusDay.Month == date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                export.MonthToday += cost;
                                export.MonthUsdToday += costUsd / 1000;

                                export.YearToday += cost;
                                export.YearUsdToday += costUsd / 1000;
                            }
                            else
                            {
                                export.MonthOld += cost;
                                export.MonthUsdOld += costUsd / 1000;

                                export.YearOld += cost;
                                export.YearUsdOld += costUsd / 1000;
                            }
                        }

                        // заполнение года если месяц больше
                        if (endDateYearMinusDay.Month > date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                export.YearToday += cost;
                                export.YearUsdToday += costUsd / 1000;
                            }
                            else
                            {
                                export.YearOld += cost;
                                export.YearUsdOld += costUsd / 1000;
                            }
                        }
                        ExportByTerritory.Add(export);
                    }
                }
                // Товар
                else
                {
                    if (sklad != 38M)
                    {
                        continue;
                    }

                    var rate = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
                    var cost = (decimal)row["oplCost"];
                    var costUsd = cost / rate;
                    var date = (DateTime)row["oplDateExport"];

                    // Деноминация
                    if (date < new DateTime(2016, 7, 1))
                    {
                        cost = cost / 10000;
                    }

                    var territoryName = string.Empty;
                    var territoryId = ((string)row["potrTerritoryId"]).Trim();
                    var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
                    var territoryIdZero = territoryIdTwo == "50" ? territoryId : territoryIdTwo + "00";

                    if (territoryIdTwo == "15")
                    {
                        continue;
                    }

                    // Получение названия страны по сформированному id
                    foreach (var rowTeritory in territories.Select())
                    {

                        if (((string)rowTeritory["territoryId"]).Trim() == territoryIdZero)
                        {
                            territoryName = ((string)rowTeritory["territoryName"]).Trim();
                            break;
                        }
                    }

                    var flag = false;
                    foreach (var export in ExportByTerritory)
                    {
                        if (export.Country == territoryName)
                        {
                            flag = true;

                            // заполнение месяца
                            if (endDateYearMinusDay.Month == date.Month)
                            {
                                if (endDateYearMinusDay.Year == date.Year)
                                {
                                    export.MonthToday += cost;
                                    export.MonthUsdToday += costUsd / 1000;

                                    export.YearToday += cost;
                                    export.YearUsdToday += costUsd / 1000;
                                }
                                else
                                {
                                    export.MonthOld += cost;
                                    export.MonthUsdOld += costUsd / 1000;

                                    export.YearOld += cost;
                                    export.YearUsdOld += costUsd / 1000;
                                }
                            }

                            // заполнение года если месяц больше
                            if (endDateYearMinusDay.Month > date.Month)
                            {
                                if (endDateYearMinusDay.Year == date.Year)
                                {
                                    export.YearToday += cost;
                                    export.YearUsdToday += costUsd / 1000;
                                }
                                else
                                {
                                    export.YearOld += cost;
                                    export.YearUsdOld += costUsd / 1000;
                                }
                            }
                            break;
                        }
                    }

                    if (flag == false)
                    {
                        var export = new ExportByTerritory
                        {
                            Country = territoryName,
                            MonthCountToday = 0M,
                            YearCountToday = 0M,
                            MonthCountOld = 0M,
                            YearCountOld = 0M
                        };

                        // заполнение месяца
                        if (endDateYearMinusDay.Month == date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                export.MonthToday += cost;
                                export.MonthUsdToday += costUsd / 1000;

                                export.YearToday += cost;
                                export.YearUsdToday += costUsd / 1000;
                            }
                            else
                            {
                                export.MonthOld += cost;
                                export.MonthUsdOld += costUsd / 1000;

                                export.YearOld += cost;
                                export.YearUsdOld += costUsd / 1000;
                            }
                        }

                        // заполнение года если месяц больше
                        if (endDateYearMinusDay.Month > date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                export.YearToday += cost;
                                export.YearUsdToday += costUsd / 1000;
                            }
                            else
                            {
                                export.YearOld += cost;
                                export.YearUsdOld += costUsd / 1000;
                            }
                        }
                        ExportByTerritory.Add(export);
                    }
                }
            }
        }

        /// <summary>
		/// Логика формирование листа записей оборудований TODO не протестил, не было в ближайшие 3 года
		/// </summary>
		public static void GetEquipment(DateTime startDate, DateTime endDate)
        {
            var endDateYearMinusDay = endDate.AddDays(-1);
            var startDateYaer = new DateTime(endDateYearMinusDay.Year, 1, 1);
            var oldYearStartDate = new DateTime(endDateYearMinusDay.Year - 1, 1, 1);
            var oldYearEndDate = endDateYearMinusDay.AddYears(-1);
           
            var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

	        var sqlResultras07 = DataTableHelper.LoadDataTableByQuery(DbfPathBuh,
		        string.Format(QueryRas07PotrKursValUsd, oldYearStartDate.ToString("MM/dd/yyyy"), oldYearEndDate.ToString("MM/dd/yyyy"),
			        startDateYaer.ToString("MM/dd/yyyy"), endDateYearMinusDay.ToString("MM/dd/yyyy")), "SqlResultras07");

			foreach (var row in sqlResultras07.Select())

            {
                var territoryName = string.Empty;
                var territoryId = ((string)row["potrTerritoryId"]).Trim();
                var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

                var territoryIdZero = territoryIdTwo == "50" ? territoryId : territoryIdTwo + "00";

                if (territoryIdTwo == "15")
                {
                    continue;
                }

                // Получение названия страны по сформированному id
                foreach (var rowTeritory in territories.Select())
                {

                    if (((string)rowTeritory["territoryId"]).Trim() == territoryIdZero)
                    {
                        territoryName = ((string)rowTeritory["territoryName"]).Trim();
                        break;
                    }
                }

                var rate = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
                var cost = (decimal)row["ras07Cost"];
                var nds = (decimal)row["ras07Nds"];
                var count = (decimal)row["ras07Count"];
                var costresult = (cost + nds) * count;
                var costUsd = costresult / rate;
                var date = (DateTime)row["ras07DateExport"];

                // Деноминация
                if (date < new DateTime(2016, 7, 1))
                {
                    costresult = costresult / 10000;
                }

                var flag = false;
                foreach (var export in ExportByTerritory)
                {
                    if (export.Country == territoryName)
                    {
                        flag = true;

                        // заполнение месяца
                        if (endDateYearMinusDay.Month == date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                export.MonthToday += costresult;
                                export.MonthUsdToday += costUsd / 1000;

                                export.YearToday += costresult;
                                export.YearUsdToday += costUsd / 1000;
                            }
                            else
                            {
                                export.MonthOld += costresult;
                                export.MonthUsdOld += costUsd / 1000;

                                export.YearOld += costresult;
                                export.YearUsdOld += costUsd / 1000;
                            }
                        }

                        // заполнение года если месяц больше
                        if (endDateYearMinusDay.Month > date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                export.YearToday += costresult;
                                export.YearUsdToday += costUsd / 1000;
                            }
                            else
                            {
                                export.YearOld += costresult;
                                export.YearUsdOld += costUsd / 1000;
                            }
                        }
                        break;
                    }
                }

                if (flag == false)
                {
                    var export = new ExportByTerritory
                    {
                        Country = territoryName,
                        MonthCountToday = 0M,
                        YearCountToday = 0M,
                        MonthCountOld = 0M,
                        YearCountOld = 0M
                    };

                    // заполнение месяца
                    if (endDateYearMinusDay.Month == date.Month)
                    {
                        if (endDateYearMinusDay.Year == date.Year)
                        {
                            export.MonthToday += cost;
                            export.MonthUsdToday += costUsd / 1000;

                            export.YearToday += cost;
                            export.YearUsdToday += costUsd / 1000;
                        }
                        else
                        {
                            export.MonthOld += cost;
                            export.MonthUsdOld += costUsd / 1000;

                            export.YearOld += cost;
                            export.YearUsdOld += costUsd / 1000;
                        }
                    }

                    // заполнение года если месяц больше
                    if (endDateYearMinusDay.Month > date.Month)
                    {
                        if (endDateYearMinusDay.Year == date.Year)
                        {
                            export.YearToday += cost;
                            export.YearUsdToday += costUsd / 1000;
                        }
                        else
                        {
                            export.YearOld += cost;
                            export.YearUsdOld += costUsd / 1000;
                        }
                    }
                    ExportByTerritory.Add(export);
                }
            }
        }

        /// <summary>
		/// Логика формирование листа записей готовой продукции
		/// </summary>
		public static void GetFinishedProducts(DateTime startDate, DateTime endDate)
        {
            Dictionary<decimal, decimal> exportWhithCostUsd = new Dictionary<decimal, decimal>();

            var endDateYearMinusDay = endDate.AddDays(-1);
            var startDateYaer = new DateTime(endDateYearMinusDay.Year, 1, 1);
            var oldYearStartDate = new DateTime(endDateYearMinusDay.Year - 1, 1, 1);
            var oldYearEndDate = endDateYearMinusDay.AddYears(-1);
           
            var kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
                query: string.Format(QueryKursVal, oldYearStartDate.ToString("MM/dd/yyyy")),
                tableName: "RateVal");

	        var sqlResultPrrasos = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
		        string.Format(QueryPnaklPrrasosIzdelPotrKursValUsd, oldYearStartDate.ToString("MM/dd/yyyy"), oldYearEndDate.ToString("MM/dd/yyyy"),
			        startDateYaer.ToString("MM/dd/yyyy"), endDateYearMinusDay.ToString("MM/dd/yyyy")), "SqlResultPrrasos");

			// Создание коллекции код расхода, цена в usd
			foreach (var row in sqlResultPrrasos.Select())
            {
                var exportId = (decimal)row["pnaklExportId"];

                var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
                var valId = (decimal)row["pnaklValId"];
                var cost = (decimal)row["prrasosCost"];
                var costUsd = (decimal)row["prrasosCostUsd"];
                var nds = (decimal)row["pnaklNds"];
                var count = (decimal)row["prrasosCount"];

                var date = (DateTime)row["pnaklDateExport"];

                // до 2018 в cenad хранилась цена в валюте
                if (date <= new DateTime(2017, 12, 31))
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
                    costUsd = cost / rateUsd;
                }

                var costUsdresult = (costUsd + costUsd * nds / 100) * count;

                if (exportWhithCostUsd.ContainsKey(exportId))
                {
                    exportWhithCostUsd[exportId] += costUsdresult;
                }
                else
                {
                    exportWhithCostUsd.Add(exportId, costUsdresult);
                }
            }

            var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

            foreach (var row in sqlResultPrrasos.Select())
            {
                var territoryName = string.Empty;
                var territoryId = ((string)row["potrTerritoryId"]).Trim();
                var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
                var territoryIdZero = territoryIdTwo == "50" ? territoryId : territoryIdTwo + "00";

                // Получение названия страны по сформированному id
                foreach (var rowTeritory in territories.Select())
                {

                    if (((string)rowTeritory["territoryId"]).Trim() == territoryIdZero)
                    {
                        territoryName = ((string)rowTeritory["territoryName"]).Trim();
                        break;
                    }
                }

                var exportId = (decimal)row["pnaklExportId"];

                // Приказ сверху в 2017году выкинуть расход с id 170463
                if (exportId == 170463)
                {
                    continue;
                }

                if (territoryIdTwo == "15")
                {
                    continue;
                }

                // Если услуга - не учитывать 
                if ((decimal)row["izdelProvide"] == 1)
                {
                    continue;
                }

                // Если общая стоймость < 50$ - не учитывать 
                if (exportWhithCostUsd[exportId] < 50M)
                {
                    continue;
                }

                var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal)row["kursKurs"];
                var valId = (decimal)row["pnaklValId"];
                var cost = (decimal)row["prrasosCost"];
                var costUsd = (decimal)row["prrasosCostUsd"];
                var nds = (decimal)row["pnaklNds"];
                var count = (decimal)row["prrasosCount"];

                var tehnology = ((string) row["izdelTehnology"]).Trim();

                var date = (DateTime)row["pnaklDateExport"];

                // до 2018 в cenad хранилась цена в валюте
                if (date <= new DateTime(2017, 12, 31))
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
                    costUsd = cost / rateUsd;
                }

                var costresult = (cost + cost * nds / 100) * count;
                var costUsdresult = (costUsd + costUsd * nds / 100) * count;

                // Деноминация
                if (date < new DateTime(2016, 7, 1))
                {
                    costresult = costresult / 10000;
                }

                var flag = false;
                foreach (var export in ExportByTerritory)
                {
                    if (export.Country == territoryName)
                    {
                        flag = true;

                        // заполнение месяца
                        if (endDateYearMinusDay.Month == date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                if (tehnology == "t")
                                {
                                    export.MonthCountToday += count;
                                    export.YearCountToday += count;
                                }
                                export.MonthToday += costresult;
                                export.MonthUsdToday += costUsdresult / 1000;

                                export.YearToday += costresult;
                                export.YearUsdToday += costUsdresult / 1000;
                            }
                            else
                            {
                                if (tehnology == "t")
                                {
                                    export.MonthCountOld += count;
                                    export.YearCountOld += count;
                                }
                                export.MonthOld += costresult;
                                export.MonthUsdOld += costUsdresult / 1000;

                                export.YearOld += costresult;
                                export.YearUsdOld += costUsdresult / 1000;
                            }
                        }

                        // заполнение года если месяц больше
                        if (endDateYearMinusDay.Month != date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                if (tehnology == "t")
                                {
                                    export.YearCountToday += count;
                                }
                                export.YearToday += costresult;
                                export.YearUsdToday += costUsdresult / 1000;
                            }
                            else
                            {
                                if (tehnology == "t")
                                {
                                    export.YearCountOld += count;
                                }
                                export.YearOld += costresult;
                                export.YearUsdOld += costUsdresult / 1000;
                            }
                        }
                        break;
                    }
                }

                if (flag == false)
                {
                    var export = new ExportByTerritory
                    {
                        Country = territoryName,
                        MonthCountToday = 0M,
                        YearCountToday = 0M,
                        MonthCountOld = 0M,
                        YearCountOld = 0M
                    };

                    // заполнение месяца
                    if (endDateYearMinusDay.Month == date.Month)
                    {
                        if (endDateYearMinusDay.Year == date.Year)
                        {
                            if (tehnology == "t")
                            {
                                export.MonthCountToday += count;
                                export.YearCountToday += count;
                            }
                            export.MonthToday += costresult;
                            export.MonthUsdToday += costUsdresult / 1000;

                            export.YearToday += costresult;
                            export.YearUsdToday += costUsdresult / 1000;
                        }
                        else
                        {
                            if (tehnology == "t")
                            {
                                export.MonthCountOld += count;
                                export.YearCountOld += count;
                            }
                            export.MonthOld += costresult;
                            export.MonthUsdOld += costUsdresult / 1000;

                            export.YearOld += costresult;
                            export.YearUsdOld += costUsdresult / 1000;
                        }
                    }

                    // заполнение года если месяц больше
                    if (endDateYearMinusDay.Month > date.Month)
                    {
                        if (endDateYearMinusDay.Year == date.Year)
                        {
                            if (tehnology == "t")
                            {
                                export.YearCountToday += count;
                            }
                            export.YearToday += costresult;
                            export.YearUsdToday += costUsdresult / 1000;
                        }
                        else
                        {
                            if (tehnology == "t")
                            {
                                export.YearCountOld += count;
                            }
                            export.YearOld += costresult;
                            export.YearUsdOld += costUsdresult / 1000;
                        }
                    }
                    ExportByTerritory.Add(export);
                }
            }
        }

        /// <summary>
		/// Логика формирование листа записей готовой продукции с ценой на дату отгрузки
		/// </summary>
		public static void GetFinishedProductsByDateShipment(DateTime startDate, DateTime endDate)
        {
            Dictionary<decimal, decimal> exportWhithCostUsd = new Dictionary<decimal, decimal>();

            var endDateYearMinusDay = endDate.AddDays(-1);
            var startDateYaer = new DateTime(endDateYearMinusDay.Year, 1, 1);
            var oldYearStartDate = new DateTime(endDateYearMinusDay.Year - 1, 1, 1);
            var oldYearEndDate = endDateYearMinusDay.AddYears(-1);
           
            var kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
                query: string.Format(QueryKursVal, oldYearStartDate.ToString("MM/dd/yyyy")),
                tableName: "RateVal");

			var sqlResultPrrasos = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotrKursValUsd, oldYearStartDate.ToString("MM/dd/yyyy"), oldYearEndDate.ToString("MM/dd/yyyy"),
					startDateYaer.ToString("MM/dd/yyyy"), endDateYearMinusDay.ToString("MM/dd/yyyy")), "SqlResultPrrasos");

			// Создание коллекции код расхода, цена в usd
			foreach (var row in sqlResultPrrasos.Select())
            {
                var exportId = (decimal) row["pnaklExportId"];

                var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
                var valId = (decimal) row["pnaklValId"];
                var cost = (decimal) row["prrasosCostVal"];
                var nds = (decimal) row["pnaklNds"];
                var count = (decimal) row["prrasosCount"];

                var date = (DateTime) row["pnaklDateExport"];

                // до 2018 в cenad хранилась цена в валюте
                if (date <= new DateTime(2017, 12, 31))
                {
                    cost = (decimal)row["prrasosCost"];
                }

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

                var costUsd = cost / rateUsd;
                var costUsdresult = (costUsd + costUsd * nds / 100) * count;

                if (exportWhithCostUsd.ContainsKey(exportId))
                {
                    exportWhithCostUsd[exportId] += costUsdresult;
                }
                else
                {
                    exportWhithCostUsd.Add(exportId, costUsdresult);
                }
            }

            var territories = DataTableHelper.LoadDataTableByQuery(DbfPathBase, QueryTerr, "Territory");

            foreach (var row in sqlResultPrrasos.Select())
            {
                var territoryName = string.Empty;
                var territoryId = ((string) row["potrTerritoryId"]).Trim();
                var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
                var territoryIdZero = territoryIdTwo == "50" ? territoryId : territoryIdTwo + "00";

                // Получение названия страны по сформированному id
                foreach (var rowTeritory in territories.Select())
                {

                    if (((string) rowTeritory["territoryId"]).Trim() == territoryIdZero)
                    {
                        territoryName = ((string) rowTeritory["territoryName"]).Trim();
                        break;
                    }
                }

                var exportId = (decimal) row["pnaklExportId"];

                // Приказ сверху в 2017году выкинуть расход с id 170463
                if (exportId == 170463)
                {
                    continue;
                }

                if (territoryIdTwo == "15")
                {
                    continue;
                }

                // Если услуга - не учитывать 
                if ((decimal) row["izdelProvide"] == 1)
                {
                    continue;
                }

                // Если общая стоймость < 50$ - не учитывать 
                if (exportWhithCostUsd[exportId] < 50M)
                {
                    continue;
                }

                var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
                var valId = (decimal) row["pnaklValId"];
                var cost = (decimal) row["prrasosCostVal"];
                var nds = (decimal) row["pnaklNds"];
                var count = (decimal) row["prrasosCount"];

                var tehnology = ((string) row["izdelTehnology"]).Trim();

                var date = (DateTime) row["pnaklDateExport"];

                // до 2018 в cenad хранилась цена в валюте
                if (date <= new DateTime(2017, 12, 31))
                {
                    cost = (decimal)row["prrasosCost"];
                }

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

                var costUsd = cost / rateUsd;

                var costresult = (cost + cost * nds / 100) * count;
                var costUsdresult = (costUsd + costUsd * nds / 100) * count;

                // Деноминация
                if (date < new DateTime(2016, 7, 1))
                {
                    costresult = costresult / 10000;
                }

                var flag = false;
                foreach (var export in ExportByTerritory)
                {
                    if (export.Country == territoryName)
                    {
                        flag = true;

                        // заполнение месяца
                        if (endDateYearMinusDay.Month == date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                if (tehnology == "t")
                                {
                                    export.MonthCountToday += count;
                                    export.YearCountToday += count;
                                }

                                export.MonthToday += costresult;
                                export.MonthUsdToday += costUsdresult / 1000;

                                export.YearToday += costresult;
                                export.YearUsdToday += costUsdresult / 1000;
                            }
                            else
                            {
                                if (tehnology == "t")
                                {
                                    export.MonthCountOld += count;
                                    export.YearCountOld += count;
                                }

                                export.MonthOld += costresult;
                                export.MonthUsdOld += costUsdresult / 1000;

                                export.YearOld += costresult;
                                export.YearUsdOld += costUsdresult / 1000;
                            }
                        }

                        // заполнение года если месяц больше
                        if (endDateYearMinusDay.Month != date.Month)
                        {
                            if (endDateYearMinusDay.Year == date.Year)
                            {
                                if (tehnology == "t")
                                {
                                    export.YearCountToday += count;
                                }

                                export.YearToday += costresult;
                                export.YearUsdToday += costUsdresult / 1000;
                            }
                            else
                            {
                                if (tehnology == "t")
                                {
                                    export.YearCountOld += count;
                                }

                                export.YearOld += costresult;
                                export.YearUsdOld += costUsdresult / 1000;
                            }
                        }
                        break;
                    }
                }

                if (flag == false)
                {
                    var export = new ExportByTerritory
                    {
                        Country = territoryName,
                        MonthCountToday = 0M,
                        YearCountToday = 0M,
                        MonthCountOld = 0M,
                        YearCountOld = 0M
                    };

                    // заполнение месяца
                    if (endDateYearMinusDay.Month == date.Month)
                    {
                        if (endDateYearMinusDay.Year == date.Year)
                        {
                            if (tehnology == "t")
                            {
                                export.MonthCountToday += count;
                                export.YearCountToday += count;
                            }

                            export.MonthToday += costresult;
                            export.MonthUsdToday += costUsdresult / 1000;

                            export.YearToday += costresult;
                            export.YearUsdToday += costUsdresult / 1000;
                        }
                        else
                        {
                            if (tehnology == "t")
                            {
                                export.MonthCountOld += count;
                                export.YearCountOld += count;
                            }

                            export.MonthOld += costresult;
                            export.MonthUsdOld += costUsdresult / 1000;

                            export.YearOld += costresult;
                            export.YearUsdOld += costUsdresult / 1000;
                        }
                    }

                    // заполнение года если месяц больше
                    if (endDateYearMinusDay.Month > date.Month)
                    {
                        if (endDateYearMinusDay.Year == date.Year)
                        {
                            if (tehnology == "t")
                            {
                                export.YearCountToday += count;
                            }

                            export.YearToday += costresult;
                            export.YearUsdToday += costUsdresult / 1000;
                        }
                        else
                        {
                            if (tehnology == "t")
                            {
                                export.YearCountOld += count;
                            }

                            export.YearOld += costresult;
                            export.YearUsdOld += costUsdresult / 1000;
                        }
                    }
                    ExportByTerritory.Add(export);
                }
            }
        }
    }
}
