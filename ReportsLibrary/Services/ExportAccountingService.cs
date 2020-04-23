using System;
using System.Collections.Generic;
using System.Data;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Учет экспорта]
	/// TODO без возвратов
	/// </summary>
	public class ExportAccountingService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;

		private static readonly string DbfPathSkl = Properties.Settings.Default.FoxproDbFolder_Skl;
		private static readonly string DbfPathBuh = Properties.Settings.Default.FoxproDbFolder_Buh;

		private static readonly string DbfPathBuhArhiv = Properties.Settings.Default.FoxproDbFolder_BuhArhiv;
	    private static readonly string DbfPathFsoArhiv = Properties.Settings.Default.FoxproDbFolder_FsoArhiv;

        public static List<ExportAccounting> ExportsAccounting = new List<ExportAccounting>();

		private static readonly string QueryOplPotrKursUsd = "SELECT opl.nom_dok as oplExportId, " +
		                                          "opl.sklad AS oplSklad, " +
		                                          "opl.data as oplDateExport, " +
		                                          "opl.sum_d as oplCost, " +
		                                          "opl.kpotr as oplCompanyId, " +
		                                          "potr.kpotr as potrCompanyId, " +
		                                          "potr.kter as potrTerritoryId, " +
		                                          "kurs_val.data as kursDate, " +
		                                          "kurs_val.kurs AS kursKurs  " +
												  "FROM \"" + DbfPathSkl + "opl.dbf\" " +
												  "LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on potr.kpotr = opl.kpotr " +
												  "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on kurs_val.data = opl.data " +
		                                          "WHERE opl.p_s not like '{0}' and opl.data >= ctod( '{1}' ) " +
												  "and opl.data <= ctod( '{2}' ) and kurs_val.kval = 1 " +
												  "and kurs_val.data >= ctod( '{0}' ) " +
		                                          "ORDER BY kurs_val.data DESC";

	    private const string QueryKursVal = "SELECT kurs_val.data as kursDate, " +
	                                        "kurs_val.kurs AS kursKurs, " +
                                            "kurs_val.kval as kursValId " +
	                                        "FROM kurs_val where kurs_val.data >= ctod( '{0}' ) ORDER BY kurs_val.data DESC";

		private static readonly string QueryRas07PotrKursValUsd = "SELECT result.Data as ras07DateExport, " +
		                                            "result.kpotr as ras07CompanyId, " +
		                                            "result.Cenadog as ras07Cost, " +
		                                            "result.Nds as ras07Nds, " +
		                                            "result.kol as ras07Count, " +
		                                            "potr.kpotr as potrCompanyId, " +
		                                            "potr.kter as potrTerritoryId, " +
		                                            "kurs_val.data as kursDate, " +
		                                            "kurs_val.kurs AS kursKurs " +
		                                            "FROM ( SELECT data, kol, cenadog, nds, kpotr FROM \"" + DbfPathBuh + "ras07.dbf\" " +
		                                            "union all " +
													"SELECT data, kol, cenadog, nds, kpotr FROM \"" + DbfPathBuhArhiv + "ras07.dbf\" ) " +
		                                            "as result " +
													"LEFT JOIN \"" + DbfPathBase + "potr.dbf\" as potr on result.kpotr = potr.kpotr " +
													"LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on result.Data = kurs_val.data " +
		                                            "WHERE result.kpotr <> 0 and result.cenadog <> 0 " +
													"and result.data >= ctod( '{0}' ) and result.data <= ctod( '{1}' ) " +
													"and kurs_val.kval = 1 and kurs_val.data >= ctod( '{0}' ) " +
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
		                                                   "potr.kpotr as potrCompanyId, " +
		                                                   "potr.kter as potrTerritoryId, " +
		                                                   "kurs_val.data as kursDate, " +
		                                                   "kurs_val.kurs as kursKurs " +
														   "FROM (SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFso + "prrasos.dbf\" " +
		                                                   "union all  " +
														   "SELECT kizd, kol, nom_pn, cenad, cena_dol, cena_val FROM \"" + DbfPathFsoArhiv + "prrasos.dbf\" ) " +
		                                                   "as result " +
														   "LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on pnakl.nomdok  =  result.nom_pn " +
														   "LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on izdel.kizd = result.kizd " +
														   "LEFT JOIN \"" + DbfPathBase + "potr.dbf\"  as potr on pnakl.kpotr = potr.kpotr " +
														   "LEFT JOIN \"" + DbfPathBase + "kurs_val.dbf\" as kurs_val on pnakl.dataras = kurs_val.data " +
		                                                   "WHERE (dataras >= ctod( '{0}' ) and dataras <= ctod( '{1}' )) " +
		                                                   "and (kurs_val.kval = 1 and kurs_val.data >= ctod( '{0}' )) " +
		                                                   "ORDER BY kurs_val.data DESC";


        public static void AdditionalInitializeComponent()
		{
		    ExportsAccounting = new List<ExportAccounting>
		    {
		        new ExportAccounting()
		        {
		            IdMonth = 1,
		            Month = "Январь"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 2,
		            Month = "Февраль"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 3,
		            Month = "Март"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 4,
		            Month = "Апрель"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 5,
		            Month = "Май"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 6,
		            Month = "Июнь"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 7,
		            Month = "Июль"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 8,
		            Month = "Август"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 9,
		            Month = "Сентябрь"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 10,
		            Month = "Октябрь"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 11,
		            Month = "Ноябрь"
		        },
		        new ExportAccounting()
		        {
		            IdMonth = 12,
		            Month = "Декабрь"
		        }
		    };
		}

	    /// <summary>
	    /// Логика формирование листа записей [Учет экспорта]
	    /// </summary>
	    public static List<ExportAccounting> GetExportAccountingsAll(DateTime startDate,
	        DateTime endDate)
	    {
	        AdditionalInitializeComponent();
	        GetMaterialOrProduct(startDate, endDate, true);
	        GetMaterialOrProduct(startDate, endDate, false);
            GetEquipment(startDate, endDate);
	        GetFinishedProducts(startDate, endDate);
            return ExportsAccounting;
	    }

		/// <summary>
		/// Логика формирование листа записей готовой продукции [Учет экспорта] в ценах на дату отгрузки
		/// </summary>
		public static List<ExportAccounting> GetExportAccountingsFinishedProductsOnDateOfShipment(DateTime startDate,
			DateTime endDate)
		{
			AdditionalInitializeComponent();
			GetFinishedProductsOnDateOfShipment(startDate, endDate);
			return ExportsAccounting;
		}

		/// <summary>
		/// Логика формирование листа записей материалов или товаров отчета [Учет экспорта материалов] или 
		/// [Учет экспорта товаров]
		/// </summary>
		public static List<ExportAccounting> GetExportAccountingsMaterialOrProduct(DateTime startDate,
			DateTime endDate, bool isMaterial)
		{
			AdditionalInitializeComponent();
			GetMaterialOrProduct(startDate, endDate, isMaterial);
			return ExportsAccounting;
		}

	    /// <summary>
	    /// Логика формирование листа записей материалов или товаров отчета [Учет экспорта оборудования]
	    /// </summary>
	    public static List<ExportAccounting> GetExportAccountingsEquipment(DateTime startDate,
	        DateTime endDate)
	    {
	        AdditionalInitializeComponent();
            GetEquipment(startDate, endDate);
	        return ExportsAccounting;
	    }

	    /// <summary>
	    /// Логика формирование листа записей материалов или товаров отчета [Учет экспорта готовой продукции]
	    /// </summary>
	    public static List<ExportAccounting> GetExportAccountingsFinishedProducts(DateTime startDate,
	        DateTime endDate)
	    {
	        AdditionalInitializeComponent();
            GetFinishedProducts(startDate, endDate);
	        return ExportsAccounting;
	    }

        /// <summary>
        /// Логика формирование листа записей материалов или товаров
        /// </summary>
        public static void GetMaterialOrProduct(DateTime startDate,
			DateTime endDate, bool isMaterial)
		{
		    var endDateMinusDay = endDate.AddDays(-1);
		    var oldYearStartDate = new DateTime(endDateMinusDay.Year - 1, 1, 1);

			var sqlResultOpl = DataTableHelper.LoadDataTableByQuery(DbfPathSkl,
				string.Format(QueryOplPotrKursUsd, "d", oldYearStartDate.ToString("MM/dd/yyyy"),
					endDateMinusDay.ToString("MM/dd/yyyy")), "SqlResultOpl");

			foreach (var row in sqlResultOpl.Select())
			{
				decimal sklad = (decimal) row["oplSklad"];
				//Материал
				if (isMaterial)
				{
					if (sklad == 38M) continue;
					var rate = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
					var cost = (decimal) row["oplCost"];
					var costUsd = cost / rate;
					var date = (DateTime) row["oplDateExport"];

					// Деноминация
					if (date < new DateTime(2016, 7, 1))
					{
						cost = cost / 10000;
					}

					var territoryId = ((string) row["potrTerritoryId"]).Trim();
					var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

					foreach (var export in ExportsAccounting)
					{
						// заполнени месяца
						if (export.IdMonth == date.Month)
						{
							if (territoryIdTwo == "50"
								|| territoryIdTwo == "26"
								|| territoryIdTwo == "31"
								|| territoryIdTwo == "42")
							{
								if (date.Year == endDateMinusDay.Year)
								{
									export.FartherMonthToday += cost;
									export.FartherMonthUsdToday += costUsd / 1000;

									export.FartherYearToday += cost;
									export.FartherYearUsdToday += costUsd / 1000;
								}
								else
								{
									export.FartherMonthOld += cost;
									export.FartherMonthUsdOld += costUsd / 1000;

									export.FartherYearOld += cost;
									export.FartherYearUsdOld += costUsd / 1000;
								}
							}
							else if (territoryIdTwo != "15")
							{
								if (date.Year == endDateMinusDay.Year)
								{
									export.NearMonthToday += cost;
									export.NearMonthUsdToday += costUsd / 1000;

									export.NearYearToday += cost;
									export.NearYearUsdToday += costUsd / 1000;
								}
								else
								{
									export.NearMonthOld += cost;
									export.NearMonthUsdOld += costUsd / 1000;

									export.NearYearOld += cost;
									export.NearYearUsdOld += costUsd / 1000;
								}
							}
						}

						// заполнение года если месяц больше
						if (export.IdMonth <= date.Month)
						{
							continue;
						}

						if (territoryIdTwo == "50"
							|| territoryIdTwo == "26"
							|| territoryIdTwo == "31"
							|| territoryIdTwo == "42")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.FartherYearToday += cost;
								export.FartherYearUsdToday += costUsd / 1000;
							}
							else
							{
								export.FartherYearOld += cost;
								export.FartherYearUsdOld += costUsd / 1000;
							}
						}
						else if (territoryIdTwo != "15")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.NearYearToday += cost;
								export.NearYearUsdToday += costUsd / 1000;
							}
							else
							{
								export.NearYearOld += cost;
								export.NearYearUsdOld += costUsd / 1000;
							}
						}
					}
				}
				// Товар
				else
				{
					if (sklad != 38M)
					{
						continue;
					}

					var rate = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
					var cost = (decimal) row["oplCost"];
					var costUsd = cost / rate;
					var date = (DateTime) row["oplDateExport"];

					// Деноминация
					if (date < new DateTime(2016, 7, 1))
					{
						cost = cost / 10000;
					}

					var territoryId = ((string) row["potrTerritoryId"]).Trim();
					var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

					foreach (var export in ExportsAccounting)
					{
						// заполнение месяца
						if (export.IdMonth == date.Month)
						{
							if (territoryIdTwo == "50"
								|| territoryIdTwo == "26"
								|| territoryIdTwo == "31"
								|| territoryIdTwo == "42")
							{
								if (date.Year == endDateMinusDay.Year)
								{
									export.FartherMonthToday += cost;
									export.FartherMonthUsdToday += costUsd / 1000;

									export.FartherYearToday += cost;
									export.FartherYearUsdToday += costUsd / 1000;
								}
								else
								{
									export.FartherMonthOld += cost;
									export.FartherMonthUsdOld += costUsd / 1000;

									export.FartherYearOld += cost;
									export.FartherYearUsdOld += costUsd / 1000;
								}
							}
							else if (territoryIdTwo != "15")
							{
								if (date.Year == endDateMinusDay.Year)
								{
									export.NearMonthToday += cost;
									export.NearMonthUsdToday += costUsd / 1000;

									export.NearYearToday += cost;
									export.NearYearUsdToday += costUsd / 1000;
								}
								else
								{
									export.NearMonthOld += cost;
									export.NearMonthUsdOld += costUsd / 1000;

									export.NearYearOld += cost;
									export.NearYearUsdOld += costUsd / 1000;
								}
							}
						}

						// заполнение года если месяц больше
						if (export.IdMonth <= date.Month)
						{
							continue;
						}

						if (territoryIdTwo == "50"
							|| territoryIdTwo == "26"
							|| territoryIdTwo == "31"
							|| territoryIdTwo == "42")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.FartherYearToday += cost;
								export.FartherYearUsdToday += costUsd / 1000;
							}
							else
							{
								export.FartherYearOld += cost;
								export.FartherYearUsdOld += costUsd / 1000;
							}
						}
						else if (territoryIdTwo != "15")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.NearYearToday += cost;
								export.NearYearUsdToday += costUsd / 1000;
							}
							else
							{
								export.NearYearOld += cost;
								export.NearYearUsdOld += costUsd / 1000;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Логика формирование листа записей оборудований TODO не протестил, не было в ближайшие 3 года
		/// </summary>
		public static void GetEquipment(DateTime startDate, DateTime endDate)
		{
		    var endDateMinusDay = endDate.AddDays(-1);
		    var oldYearStartDate = new DateTime(endDateMinusDay.Year - 1, 1, 1);

			var sqlResultRas07 = DataTableHelper.LoadDataTableByQuery(DbfPathBuh,
				string.Format(QueryRas07PotrKursValUsd, oldYearStartDate.ToString("MM/dd/yyyy"),
					endDateMinusDay.ToString("MM/dd/yyyy")), "SqlResultRas07");

			foreach (var row in sqlResultRas07.Select())
			{
				var territoryId = ((string) row["potrTerritoryId"]).Trim();
				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);

				if (territoryIdTwo == "15")
				{
					continue;
				}

				var rate = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
				var cost = (decimal) row["ras07Cost"];
				var nds = (decimal) row["ras07Nds"];
				var count = (decimal) row["ras07Count"];
				var costresult = (cost + nds) * count;
				var costUsd = costresult / rate;
				var date = (DateTime) row["ras07DateExport"];

				// Деноминация
				if (date < new DateTime(2016, 7, 1))
				{
					costresult = costresult / 10000;
				}

				foreach (var export in ExportsAccounting)
				{
					// заполнени месяца
					if (export.IdMonth == date.Month)
					{
						if (date.Year == endDateMinusDay.Year)
						{
							export.FartherMonthToday += costresult;
							export.FartherMonthUsdToday += costUsd / 1000;

							export.FartherYearToday += costresult;
							export.FartherYearUsdToday += costUsd / 1000;
						}
						else
						{
							export.FartherMonthOld += costresult;
							export.FartherMonthUsdOld += costUsd / 1000;

							export.FartherYearOld += costresult;
							export.FartherYearUsdOld += costUsd / 1000;
						}
					}

					// заполнение года если месяц больше
					if (export.IdMonth <= date.Month)
					{
						continue;
					}

					if (date.Year == endDateMinusDay.Year)
					{
						export.FartherYearToday += costresult;
						export.FartherYearUsdToday += costUsd / 1000;
					}
					else
					{
						export.FartherYearOld += costresult;
						export.FartherYearUsdOld += costUsd / 1000;
					}
				}
			}
		}

		/// <summary>
		/// Логика формирование листа записей готовой продукции
		/// </summary>
		public static void GetFinishedProducts(DateTime startDate, DateTime endDate)
		{
			Dictionary<decimal, decimal> exportWhithCostUsd = new Dictionary<decimal, decimal>();

			var endDateMinusDay = endDate.AddDays(-1);
			var oldYearStartDate = new DateTime(endDateMinusDay.Year - 1, 1, 1);

			var kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				query: string.Format(QueryKursVal, oldYearStartDate.ToString("MM/dd/yyyy")),
				tableName: "RateVal");

			var sqlResultPrrasos = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotrKursValUsd, oldYearStartDate.ToString("MM/dd/yyyy"),
					endDateMinusDay.ToString("MM/dd/yyyy")), "SqlResultPrrasos");

			// Создание коллекции код расхода, цена в usd
			foreach (var row in sqlResultPrrasos.Select())
			{
				var exportId = (decimal) row["pnaklExportId"];

				var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
				var valId = (decimal) row["pnaklValId"];
				var cost = (decimal) row["prrasosCost"];
				var costUsd = (decimal) row["prrasosCostUsd"];
				var nds = (decimal) row["pnaklNds"];
				var count = (decimal) row["prrasosCount"];

				var date = (DateTime) row["pnaklDateExport"];

				// до 2018 в cenad хранилась цена в валюте
				if (date <= new DateTime(2017, 12, 31))
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

			foreach (var row in sqlResultPrrasos.Select())
			{
				var territoryId = ((string) row["potrTerritoryId"]).Trim();
				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
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
				var cost = (decimal) row["prrasosCost"];
				var costUsd = (decimal) row["prrasosCostUsd"];
				var nds = (decimal) row["pnaklNds"];
				var count = (decimal) row["prrasosCount"];

				var date = (DateTime) row["pnaklDateExport"];

				// до 2018 в cenad хранилась цена в валюте
				if (date <= new DateTime(2017, 12, 31))
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
					costUsd = cost / rateUsd;
				}

				var costresult = (cost + cost * nds / 100) * count;
				var costUsdresult = (costUsd + costUsd * nds / 100) * count;

				// Деноминация
				if (date < new DateTime(2016, 7, 1))
				{
					costresult = costresult / 10000;
				}

				foreach (var export in ExportsAccounting)
				{
					// заполнени месяца
					if (export.IdMonth == date.Month)
					{
						if (territoryIdTwo == "50"
						    || territoryIdTwo == "26"
						    || territoryIdTwo == "31"
						    || territoryIdTwo == "42")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.FartherMonthToday += costresult;
								export.FartherMonthUsdToday += costUsdresult / 1000;

								export.FartherYearToday += costresult;
								export.FartherYearUsdToday += costUsdresult / 1000;
							}
							else
							{
								export.FartherMonthOld += costresult;
								export.FartherMonthUsdOld += costUsdresult / 1000;

								export.FartherYearOld += costresult;
								export.FartherYearUsdOld += costUsdresult / 1000;
							}
						}
						else if (territoryIdTwo != "15")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.NearMonthToday += costresult;
								export.NearMonthUsdToday += costUsdresult / 1000;

								export.NearYearToday += costresult;
								export.NearYearUsdToday += costUsdresult / 1000;
							}
							else
							{
								export.NearMonthOld += costresult;
								export.NearMonthUsdOld += costUsdresult / 1000;

								export.NearYearOld += costresult;
								export.NearYearUsdOld += costUsdresult / 1000;
							}
						}
					}

					// заполнение года если месяц больше
					if (export.IdMonth <= date.Month)
					{
						continue;
					}

					if (territoryIdTwo == "50"
					    || territoryIdTwo == "26"
					    || territoryIdTwo == "31"
					    || territoryIdTwo == "42")
					{
						if (date.Year == endDateMinusDay.Year)
						{
							export.FartherYearToday += costresult;
							export.FartherYearUsdToday += costUsdresult / 1000;
						}
						else
						{
							export.FartherYearOld += costresult;
							export.FartherYearUsdOld += costUsdresult / 1000;
						}
					}
					else if (territoryIdTwo != "15")
					{
						if (date.Year == endDateMinusDay.Year)
						{
							export.NearYearToday += costresult;
							export.NearYearUsdToday += costUsdresult / 1000;
						}
						else
						{
							export.NearYearOld += costresult;
							export.NearYearUsdOld += costUsdresult / 1000;
						}
					}
				}
			}
		}

		/// <summary>
		/// Логика формирование листа записей готовой продукции
		/// </summary>
		public static void GetFinishedProductsOnDateOfShipment(DateTime startDate, DateTime endDate)
		{
			Dictionary<decimal, decimal> exportWhithCostUsd = new Dictionary<decimal, decimal>();

			var endDateMinusDay = endDate.AddDays(-1);
			var oldYearStartDate = new DateTime(endDateMinusDay.Year - 1, 1, 1);

			var kursVal = DataTableHelper.LoadDataTableByQuery(DbfPathBase,
				query: string.Format(QueryKursVal, oldYearStartDate.ToString("MM/dd/yyyy")),
				tableName: "RateVal");

			var sqlResultPrrasos = DataTableHelper.LoadDataTableByQuery(DbfPathFso,
				string.Format(QueryPnaklPrrasosIzdelPotrKursValUsd, oldYearStartDate.ToString("MM/dd/yyyy"),
					endDateMinusDay.ToString("MM/dd/yyyy")), "SqlResultPrrasos");

			// Создание коллекции код расхода, цена в usd
			foreach (var row in sqlResultPrrasos.Select())
			{
				var exportId = (decimal) row["pnaklExportId"];

				var rateUsd = row["kursKurs"] == DBNull.Value ? 1 : (decimal) row["kursKurs"];
				var valId = (decimal) row["pnaklValId"];
				var costVal = (decimal) row["prrasosCostVal"];
				var cost = (decimal) row["prrasosCost"];
				var nds = (decimal) row["pnaklNds"];
				var count = (decimal) row["prrasosCount"];

				var date = (DateTime) row["pnaklDateExport"];
				if (date < new DateTime(2018, 1, 1))
				{
					costVal = cost;
				}

				foreach (var rate in kursVal.Select())
				{
					if ((decimal) rate["kursValId"] == valId
					    && (DateTime) rate["kursDate"] == date)
					{
						var rateVal = (decimal) rate["kursKurs"];
						cost = costVal * rateVal;
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

			foreach (var row in sqlResultPrrasos.Select())
			{
				var territoryId = ((string) row["potrTerritoryId"]).Trim();
				var territoryIdTwo = territoryId.Substring(0, 1) + territoryId.Substring(1, 1);
				var exportId = (decimal) row["pnaklExportId"];

				// Приказ сверху в 2017 году выкинуть расход с id 170463
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
				var costVal = (decimal) row["prrasosCostVal"];
				var cost = (decimal) row["prrasosCost"];

				var nds = (decimal) row["pnaklNds"];
				var count = (decimal) row["prrasosCount"];

				var date = (DateTime) row["pnaklDateExport"];
				if (date < new DateTime(2018, 1, 1))
				{
					costVal = cost;
				}

				foreach (var rate in kursVal.Select())
				{
					if ((decimal) rate["kursValId"] == valId
					    && (DateTime) rate["kursDate"] == date)
					{
						var rateVal = (decimal) rate["kursKurs"];
						cost = costVal * rateVal;
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

				foreach (var export in ExportsAccounting)
				{
					// заполнени месяца
					if (export.IdMonth == date.Month)
					{
						if (territoryIdTwo == "50"
						    || territoryIdTwo == "26"
						    || territoryIdTwo == "31"
						    || territoryIdTwo == "42")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.FartherMonthToday += costresult;
								export.FartherMonthUsdToday += costUsdresult / 1000;

								export.FartherYearToday += costresult;
								export.FartherYearUsdToday += costUsdresult / 1000;
							}
							else
							{
								export.FartherMonthOld += costresult;
								export.FartherMonthUsdOld += costUsdresult / 1000;

								export.FartherYearOld += costresult;
								export.FartherYearUsdOld += costUsdresult / 1000;
							}
						}
						else if (territoryIdTwo != "15")
						{
							if (date.Year == endDateMinusDay.Year)
							{
								export.NearMonthToday += costresult;
								export.NearMonthUsdToday += costUsdresult / 1000;

								export.NearYearToday += costresult;
								export.NearYearUsdToday += costUsdresult / 1000;
							}
							else
							{
								export.NearMonthOld += costresult;
								export.NearMonthUsdOld += costUsdresult / 1000;

								export.NearYearOld += costresult;
								export.NearYearUsdOld += costUsdresult / 1000;
							}
						}
					}

					// заполнение года если месяц больше
					if (export.IdMonth <= date.Month)
					{
						continue;
					}

					if (territoryIdTwo == "50"
					    || territoryIdTwo == "26"
					    || territoryIdTwo == "31"
					    || territoryIdTwo == "42")
					{
						if (date.Year == endDateMinusDay.Year)
						{
							export.FartherYearToday += costresult;
							export.FartherYearUsdToday += costUsdresult / 1000;
						}
						else
						{
							export.FartherYearOld += costresult;
							export.FartherYearUsdOld += costUsdresult / 1000;
						}
					}
					else if (territoryIdTwo != "15")
					{
						if (date.Year == endDateMinusDay.Year)
						{
							export.NearYearToday += costresult;
							export.NearYearUsdToday += costUsdresult / 1000;
						}
						else
						{
							export.NearYearOld += costresult;
							export.NearYearUsdOld += costUsdresult / 1000;
						}
					}
				}
			}
		}
	}
}
