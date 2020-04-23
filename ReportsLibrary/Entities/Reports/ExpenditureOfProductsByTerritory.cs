using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Расход изделий по территориям] Расход за месяц
	/// </summary>
	class ExpenditureOfProductsByTerritory : IComparable<ExpenditureOfProductsByTerritory>
	{
		/// <summary>
		/// Доп. параметр для облегчения подсчета вывода в самом отчете
		/// </summary>
		public int Group => 1;

		/// <summary>
		/// Наименование территории
		/// </summary>
		public string TerritoryName { get; set; }

		/// <summary>
		/// Цех ID
		/// </summary>
		public decimal Ceh { get; set; }
		
		/// <summary>
		/// ТТН
		/// </summary>
		public decimal NumberTtn { get; set; }

		/// <summary>
		/// Номер акта
		/// </summary>
		public decimal NumberAkt { get; set; }

		/// <summary>
		/// Дата отгрузки
		/// </summary>
		public DateTime ShipmentDate { get; set; }

		/// <summary>
		/// Наименование продукции
		/// </summary>
		public string ProductName { get; set; }

		/// <summary>
		/// Наименование потребителя
		/// </summary>
		public string CompanyName { get; set; }

		/// <summary>
		/// Количество
		/// </summary>
		public decimal ProductCount { get; set; }

		/// <summary>
		/// Цена договорная без НДС
		/// </summary>
		public decimal CostWithoutNds { get; set; }

		/// <summary>
		/// Сумма без НДС
		/// </summary>
		public decimal SummaWithoutNds { get; set; }

		/// <summary>
		/// Сумма НДС
		/// </summary>
		public decimal SummaNds { get; set; }

		/// <summary>
		/// Всего с НДС
		/// </summary>
		public decimal SummaWithNds { get; set; }

		/// <summary>
		/// Сумма без НДС и налогов
		/// </summary>
		public  decimal SummaWithoutNdsAndTaxes { get; set; }

		/// <summary>
		/// Сумма в долларах
		/// </summary>
		public decimal SummaUsd { get; set; }

		/// <summary>
		/// Курс доллара
		/// </summary>
		public decimal UsdRate { get; set; }

		/// <summary>
		/// Курс Российского рубля
		/// </summary>
		public decimal RusRate { get; set; }


		public int CompareTo(ExpenditureOfProductsByTerritory other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			if (ReferenceEquals(this, other))
			{
				return 0;
			}

			if (ReferenceEquals(null, other))
			{
				return 1;
			}
			var cehComparison = Ceh.CompareTo((other.Ceh));
			if (cehComparison != 0)
			{
				return cehComparison;
			}
			var territoryNameComparison = string.Compare(TerritoryName, other.TerritoryName, comparisonIgnoreCase);
			if (territoryNameComparison != 0)
			{
				return -territoryNameComparison;
			}
			var numberTtnComparison = NumberTtn.CompareTo(other.NumberTtn);
			if (numberTtnComparison != 0)
			{
				return -numberTtnComparison;
			}
			var numberAktComparison = NumberAkt.CompareTo(other.NumberAkt);
			if (numberAktComparison != 0)
			{
				return -numberAktComparison;
			}
			var shipmentDateComparison = ShipmentDate.CompareTo(other.ShipmentDate);
			if (shipmentDateComparison != 0)
			{
				return shipmentDateComparison;
			}
			var productNameComparison = string.Compare(ProductName, other.ProductName, comparisonIgnoreCase);
			if (productNameComparison != 0)
			{
				return productNameComparison;
			}
			var companyNameComparison = string.Compare(CompanyName, other.CompanyName, comparisonIgnoreCase);
			if (companyNameComparison != 0)
			{
				return companyNameComparison;
			}
			var productCountComparison = ProductCount.CompareTo(other.ProductCount);
			if (productCountComparison != 0)
			{
				return productCountComparison;
			}
			var costWithoutNdsComparison = CostWithoutNds.CompareTo(other.CostWithoutNds);
			if (costWithoutNdsComparison != 0)
			{
				return costWithoutNdsComparison;
			}
			var summaWithoutNdsComparison = SummaWithoutNds.CompareTo(other.SummaWithoutNds);
			if (summaWithoutNdsComparison != 0)
			{
				return summaWithoutNdsComparison;
			}
			var summaNdsComparison = SummaNds.CompareTo(other.SummaNds);
			if (summaNdsComparison != 0)
			{
				return summaNdsComparison;
			}
			var summaWithNdsComparison = SummaWithNds.CompareTo(other.SummaWithNds);
			if (summaWithNdsComparison != 0)
			{
				return summaWithNdsComparison;
			}
			var summaWithoutNdsAndTaxesComparison = SummaWithoutNdsAndTaxes.CompareTo(other.SummaWithoutNdsAndTaxes);
			if (summaWithoutNdsAndTaxesComparison != 0)
			{
				return summaWithoutNdsAndTaxesComparison;
			}
			var summaUsdComparison = SummaUsd.CompareTo(other.SummaUsd);
			if (summaUsdComparison != 0)
			{
				return summaUsdComparison;
			}
			var usdRateComparison = UsdRate.CompareTo(other.UsdRate);
			if (usdRateComparison != 0)
			{
				return usdRateComparison;
			}
			return RusRate.CompareTo(other.RusRate);
		}

		protected bool Equals(ExpenditureOfProductsByTerritory other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(TerritoryName, other.TerritoryName, comparisonIgnoreCase) 
			       && Ceh == other.Ceh 
			       && NumberTtn == other.NumberTtn 
			       && NumberAkt == other.NumberAkt 
			       && ShipmentDate.Equals(other.ShipmentDate) 
			       && string.Equals(ProductName, other.ProductName, comparisonIgnoreCase) 
			       && string.Equals(CompanyName, other.CompanyName, comparisonIgnoreCase) 
			       && ProductCount == other.ProductCount 
			       && CostWithoutNds == other.CostWithoutNds 
			       && SummaWithoutNds == other.SummaWithoutNds 
			       && SummaNds == other.SummaNds 
			       && SummaWithNds == other.SummaWithNds 
			       && SummaWithoutNdsAndTaxes == other.SummaWithoutNdsAndTaxes 
			       && SummaUsd == other.SummaUsd 
			       && UsdRate == other.UsdRate 
			       && RusRate == other.RusRate;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((ExpenditureOfProductsByTerritory) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (TerritoryName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(TerritoryName) : 0);
				hashCode = (hashCode * 397) ^ Ceh.GetHashCode();
				hashCode = (hashCode * 397) ^ NumberTtn.GetHashCode();
				hashCode = (hashCode * 397) ^ NumberAkt.GetHashCode();
				hashCode = (hashCode * 397) ^ ShipmentDate.GetHashCode();
				hashCode = (hashCode * 397) ^ (ProductName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ProductName) : 0);
				hashCode = (hashCode * 397) ^ (CompanyName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(CompanyName) : 0);
				hashCode = (hashCode * 397) ^ ProductCount.GetHashCode();
				hashCode = (hashCode * 397) ^ CostWithoutNds.GetHashCode();
				hashCode = (hashCode * 397) ^ SummaWithoutNds.GetHashCode();
				hashCode = (hashCode * 397) ^ SummaNds.GetHashCode();
				hashCode = (hashCode * 397) ^ SummaWithNds.GetHashCode();
				hashCode = (hashCode * 397) ^ SummaWithoutNdsAndTaxes.GetHashCode();
				hashCode = (hashCode * 397) ^ SummaUsd.GetHashCode();
				hashCode = (hashCode * 397) ^ UsdRate.GetHashCode();
				hashCode = (hashCode * 397) ^ RusRate.GetHashCode();
				return hashCode;
			}
		}
	}
}
