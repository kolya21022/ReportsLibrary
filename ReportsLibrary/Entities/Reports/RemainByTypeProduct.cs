using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Остатки по видам продукции]
	/// </summary>
	/// <inheritdoc />
	public class RemainByTypeProduct : IComparable<RemainByTypeProduct>
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
		/// Наименование [Продукта]
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Количество
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// [Цена] указанная в приходе [Продукции/Изделия/Услуги]
		/// </summary>
		public decimal Cost { get; set; }

		public decimal Summa => Count * Cost;

		protected bool Equals(RemainByTypeProduct other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(TypeGroup, other.TypeGroup, comparisonIgnoreCase) 
			       && string.Equals(Type, other.Type, comparisonIgnoreCase) 
			       && string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && Count == other.Count 
			       && Cost == other.Cost;
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
			return Equals((RemainByTypeProduct) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (TypeGroup != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(TypeGroup) : 0);
				hashCode = (hashCode * 397) ^ (Type != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Type) : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				return hashCode;
			}
		}

		public int CompareTo(RemainByTypeProduct other)
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
			return Cost.CompareTo(other.Cost);
		}
	}
}
