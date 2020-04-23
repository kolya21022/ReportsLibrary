using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Для отчета по учету поставок (за год)]
	/// </summary>
	/// <inheritdoc />
	public class ForReportOnAccountingSupplies : IComparable<ForReportOnAccountingSupplies>
	{
		/// <summary>
		/// Тип продукта
		/// </summary>
		public string TypeGroup { get; set; }

		/// <summary>
		/// Подтип продукта
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Категория продукта
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Территория
		/// </summary>
		public string Territory { get; set; }

		/// <summary>
		/// Количество
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// [Цена] без НДС
		/// </summary>
		public decimal Cost { get; set; }

		/// <summary>
		/// [Цена] сумма НДС
		/// </summary>
		public decimal SummNds { get; set; }

		public decimal Summa => Cost + SummNds;

		/// <summary>
		/// [Цена] в Usd
		/// </summary>
		public decimal CostUsd { get; set; }

		protected bool Equals(ForReportOnAccountingSupplies other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(TypeGroup, other.TypeGroup, comparisonIgnoreCase) 
			       && string.Equals(Type, other.Type, comparisonIgnoreCase) 
			       && string.Equals(Category, other.Category, comparisonIgnoreCase) 
			       && string.Equals(Territory, other.Territory, comparisonIgnoreCase) 
			       && Count == other.Count 
			       && Cost == other.Cost 
			       && SummNds == other.SummNds 
			       && CostUsd == other.CostUsd;
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
			return Equals((ForReportOnAccountingSupplies) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (TypeGroup != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(TypeGroup) : 0);
				hashCode = (hashCode * 397) ^ (Type != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Type) : 0);
				hashCode = (hashCode * 397) ^ (Category != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Category) : 0);
				hashCode = (hashCode * 397) ^ (Territory != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Territory) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				hashCode = (hashCode * 397) ^ SummNds.GetHashCode();
				hashCode = (hashCode * 397) ^ CostUsd.GetHashCode();
				return hashCode;
			}
		}

		public int CompareTo(ForReportOnAccountingSupplies other)
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
			var typeGroupComparison = string.Compare(TypeGroup, other.TypeGroup, comparisonIgnoreCase);
			if (typeGroupComparison != 0)
			{
				return typeGroupComparison;
			}
			var typeComparison = string.Compare(Type, other.Type, comparisonIgnoreCase);
			if (typeComparison != 0)
			{
				return typeComparison;
			}
			var categoryComparison = string.Compare(Category, other.Category, comparisonIgnoreCase);
			if (categoryComparison != 0)
			{
				return categoryComparison;
			}
			var territoryComparison = string.Compare(Territory, other.Territory, comparisonIgnoreCase);
			if (territoryComparison != 0)
			{
				return territoryComparison;
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
			var summNdsComparison = SummNds.CompareTo(other.SummNds);
			if (summNdsComparison != 0)
			{
				return summNdsComparison;
			}
			return CostUsd.CompareTo(other.CostUsd);
		}
	}
}
