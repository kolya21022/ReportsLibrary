using System;
using System.Collections.Generic;

using ReportsLibrary.Entities.Reports;
using ReportsLibrary.Util;

namespace ReportsLibrary.Services
{
	/// <summary>
	/// Сервисный класс формирование листа записей отчета [Остатки с годом выпуска]
	/// </summary>
	public class RemainWhithYearService
	{
		private static readonly string DbfPathBase = Properties.Settings.Default.FoxproDbFolder_Base;
		private static readonly string DbfPathFso = Properties.Settings.Default.FoxproDbFolder_Fso;

		private static readonly string QueryPrrasosWhithPnaklWithIzdelKatCenaIzd = "SELECT result.kizd as prrasosProductId, " +
		                                                        "result.nom_pp as prrasosNomPp, " +
		                                                        "result.kol as prrasosCount, " +
		                                                        "result.kolsn as prrasosKolsn, " +
		                                                        "result.nom_pn as prrasosNomPn, " +
		                                                        "result.data as prrasosDateSupply, " +
		                                                        "result.datar as prrasosDateR, " +
		                                                        "pnakl.dataotg as pnaklDateShipment, " +
		                                                        "izdel.kizd as izdelProductId, " +
		                                                        "izdel.nizd as izdelProduct, " +
		                                                        "izdel.vid as izdelTypeId, " +
		                                                        "izdel.kat as izdelKatId, " +
		                                                        "kat_prod.Nkat as katName, " +
		                                                        "vid_prod.nvid as vidprodType, " +
		                                                        "cenaizd.kizd as cenaizdProductId, " +
		                                                        "cenaizd.cena as cenaizdCost, " +
		                                                        "cenaizd.nom_pp as cenaizdNomPp " +
		                                                        "FROM ( SELECT kizd, kol, nom_pn, nom_pp, kolsn, data, datar FROM \"" + DbfPathFso + "prrasos.dbf\" ) " +
		                                                        "as result " +
																"LEFT JOIN \"" + DbfPathFso + "pnakl.dbf\" as pnakl on result.nom_pn = pnakl.nomdok " +
																"LEFT JOIN \"" + DbfPathBase + "izdel.dbf\" as izdel on result.kizd = izdel.kizd " +
																"LEFT JOIN \"" + DbfPathBase + "kat_prod.dbf\" as kat_prod on kat_prod.kat = izdel.kat  " +
																"LEFT JOIN \"" + DbfPathBase + "vid_prod.dbf\" as vid_prod on vid_prod.vid = izdel.vid " +
																"LEFT JOIN \"" + DbfPathBase + "cenaizd.dbf\" as cenaizd on cenaizd.kizd = result.kizd " +
		                                                        "and cenaizd.nom_pp = result.nom_pp ";

		/// <summary>
		/// Логика формирование листа записей отчета [Остатки с годом выпуска]
		/// </summary>
		public static List<RemainWhithYear> GetRemainWhithYear(DateTime loadDateTime)
		{
			var dateRemain = loadDateTime.AddDays(-1);

			var remainsWhithYear = new List<RemainWhithYear>();

			var linkSupplyExportProductWhithCost = DataTableHelper.LoadDataTableByQuery(DbfPathFso, QueryPrrasosWhithPnaklWithIzdelKatCenaIzd, "SqlResultSupplyExportProductWhithCost");

			foreach (var rowLink in linkSupplyExportProductWhithCost.Select())
			{
				DateTime? dateR;
				var dateSupply = (DateTime)rowLink["prrasosDateSupply"];
				var nomPn = (decimal)rowLink["prrasosNomPn"];

				if (nomPn == 0)
				{
					var kolsn = (decimal)rowLink["prrasosKolsn"];
					if (kolsn == 0)
					{
						dateR = null;
					}
					else
					{
						dateR = (DateTime)rowLink["prrasosDateR"];
					}
				}
				else
				{
					dateR = (DateTime)rowLink["pnaklDateShipment"];
				}

				if ((dateSupply < dateRemain && (dateR == null || dateR > dateRemain))
				    || (dateSupply >= dateRemain && dateSupply <= dateRemain && (dateR == null || dateR > dateRemain)))
				{
					var productName = ((string)rowLink["izdelProduct"]).Trim();
					var cost = (decimal)rowLink["cenaizdCost"];
					var count = (decimal)rowLink["prrasosCount"];
					var year = dateSupply.Year;

					var typeId = (decimal)rowLink["izdelTypeId"];

					var typeProduct = typeId != 0 ? ((string) rowLink["vidprodType"]).Trim() : "";
					var katProduct = ((string)rowLink["katName"]).Trim();

					bool flag = false;
					foreach (var remainWhithYear in remainsWhithYear)
					{
						if (remainWhithYear.Cost == cost
						    && remainWhithYear.Name == productName 
						    && remainWhithYear.Year == year)
						{
							flag = true;
							remainWhithYear.Count += count;
							break;
						}
					}

					if (!flag)
					{
						string t = typeId == 0 ? katProduct : typeProduct;
						remainsWhithYear.Add(new RemainWhithYear()
						{
							Cost = cost,
							Count = count,
							Name = productName,
							Year = year,
							Type = t
						});
					}
				}
			}

			remainsWhithYear.Sort();

			return remainsWhithYear;
		}
	}
}
