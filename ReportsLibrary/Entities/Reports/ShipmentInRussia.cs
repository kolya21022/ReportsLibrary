using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Отгрузка в РФ]
	/// </summary>
	/// <inheritdoc />
	public class ShipmentInRussia : IComparable<ShipmentInRussia>
	{
		/// <summary>
		/// ТТН
		/// </summary>
		public decimal NumberTtn { get; set; }

		/// <summary>
		/// Дата отгрузки
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Город и потребитель
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Наименование [Продукта]
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Количество
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// Указанная [Цена] в РФ
		/// </summary>
		public decimal Cost { get; set; }

		public decimal Summa => Count * Cost;


		/// <summary>
		/// Указанная [Цена] в Usd
		/// </summary>
		public decimal CostUsd { get; set; }

		public decimal SummaUsd => Count * CostUsd;

		/// <summary>
		/// Сначало года отгружено в руб РФ
		/// </summary>
		public decimal YearSumma { get; set; }
		/// <summary>
		/// Сначало года отгружено в USD
		/// </summary>
		public decimal YearSummaUsd { get; set; }


		public int CompareTo(ShipmentInRussia other)
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
			var numberTtnComparison = NumberTtn.CompareTo(other.NumberTtn);
			if (numberTtnComparison != 0)
			{
				return numberTtnComparison;
			}
			var dateComparison = Date.CompareTo(other.Date);
			if (dateComparison != 0)
			{
				return dateComparison;
			}
			var companyComparison = string.Compare(Company, other.Company, comparisonIgnoreCase);
			if (companyComparison != 0)
			{
				return companyComparison;
			}
			var nameComparison = string.Compare(Name, other.Name, comparisonIgnoreCase);
			if (nameComparison != 0)
			{
				return nameComparison;
			}
			var countComparison = Count.CompareTo(other.Count);
			if (countComparison != 0)
			{
				return countComparison;
			}
			var costComparison = Cost.CompareTo(other.Cost);
			if (costComparison != 0)
			{
				return costComparison;
			}
			var costUsdComparison = CostUsd.CompareTo(other.CostUsd);
			if (costUsdComparison != 0)
			{
				return costUsdComparison;
			}
			var yearSummaComparison = YearSumma.CompareTo(other.YearSumma);
			if (yearSummaComparison != 0)
			{
				return yearSummaComparison;
			}
			return YearSummaUsd.CompareTo(other.YearSummaUsd);
		}

		protected bool Equals(ShipmentInRussia other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return NumberTtn == other.NumberTtn 
			       && Date.Equals(other.Date) 
			       && string.Equals(Company, other.Company, comparisonIgnoreCase) 
			       && string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && Count == other.Count 
			       && Cost == other.Cost 
			       && CostUsd == other.CostUsd 
			       && YearSumma == other.YearSumma 
			       && YearSummaUsd == other.YearSummaUsd;
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
			return Equals((ShipmentInRussia) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = NumberTtn.GetHashCode();
				hashCode = (hashCode * 397) ^ Date.GetHashCode();
				hashCode = (hashCode * 397) ^ (Company != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Company) : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				hashCode = (hashCode * 397) ^ CostUsd.GetHashCode();
				hashCode = (hashCode * 397) ^ YearSumma.GetHashCode();
				hashCode = (hashCode * 397) ^ YearSummaUsd.GetHashCode();
				return hashCode;
			}
		}
	}
}
