using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Расход по видам изделия]
	/// </summary>
	/// <inheritdoc />
	public class ExportByTypeProduct : IComparable<ExportByTypeProduct>
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
		/// Территория
		/// </summary>
		public string Territory { get; set; }

		/// <summary>
		/// Категория [Продукта]
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Наименование [Продукта]
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Количество
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// [Цена] без НДС за единицу
		/// </summary>
		public decimal CostOne { get; set; }

		/// <summary>
		/// [Цена] без НДС
		/// </summary>
		public decimal Cost => Count * CostOne;

		/// <summary>
		/// [Цена] сумма НДС за единицу
		/// </summary>
		public decimal SummNdsOne { get; set; }

		/// <summary>
		/// [Цена] сумма НДС
		/// </summary>
		public decimal SummNds => Count * SummNdsOne;


		public decimal Summa => Cost + SummNds;

		/// <summary>
		/// Указанная [Цена] в Usd за еденицу
		/// </summary>
		public decimal CostUsdOne { get; set; }

		/// <summary>
		/// Указанная [Цена] в Usd
		/// </summary>
		public decimal CostUsd => Count * CostUsdOne;

		protected bool Equals(ExportByTypeProduct other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(TypeGroup, other.TypeGroup, comparisonIgnoreCase) 
			       && string.Equals(Type, other.Type, comparisonIgnoreCase) 
			       && string.Equals(Territory, other.Territory, comparisonIgnoreCase) 
			       && string.Equals(Category, other.Category, comparisonIgnoreCase) 
			       && string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && Count == other.Count 
			       && CostOne == other.CostOne 
			       && SummNdsOne == other.SummNdsOne 
			       && CostUsdOne == other.CostUsdOne;
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
			return Equals((ExportByTypeProduct) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (TypeGroup != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(TypeGroup) : 0);
				hashCode = (hashCode * 397) ^ (Type != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Type) : 0);
				hashCode = (hashCode * 397) ^ (Territory != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Territory) : 0);
				hashCode = (hashCode * 397) ^ (Category != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Category) : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ CostOne.GetHashCode();
				hashCode = (hashCode * 397) ^ SummNdsOne.GetHashCode();
				hashCode = (hashCode * 397) ^ CostUsdOne.GetHashCode();
				return hashCode;
			}
		}

		public int CompareTo(ExportByTypeProduct other)
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
			var territoryComparison = string.Compare(Territory, other.Territory, comparisonIgnoreCase);
			if (territoryComparison != 0)
			{
				return territoryComparison;
			}
			var categoryComparison = string.Compare(Category, other.Category, comparisonIgnoreCase);
			if (categoryComparison != 0)
			{
				return categoryComparison;
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
			var costOneComparison = CostOne.CompareTo(other.CostOne);
			if (costOneComparison != 0)
			{
				return costOneComparison;
			}
			var summNdsOneComparison = SummNdsOne.CompareTo(other.SummNdsOne);
			if (summNdsOneComparison != 0)
			{
				return summNdsOneComparison;
			}
			return CostUsdOne.CompareTo(other.CostUsdOne);
		}
	}
}
