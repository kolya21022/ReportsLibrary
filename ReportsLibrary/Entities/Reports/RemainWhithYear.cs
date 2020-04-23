using System;


namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Остатки с годом выпуска]
	/// </summary>
	/// <inheritdoc />
	public class RemainWhithYear : IComparable<RemainWhithYear>
	{
		/// <summary>
		/// Подтип продукта
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Год выпуска
		/// </summary>
		public decimal Year { get; set; }

		/// <summary>
		/// Наименование [Продукта]
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Количество
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// [Цена] указанная в приходе за ед [Продукции/Изделия/Услуги]
		/// </summary>
		public decimal Cost { get; set; }

		public decimal Summa => Count * Cost;


		public int CompareTo(RemainWhithYear other)
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
			var costComparison = Cost.CompareTo(other.Cost);
			if (costComparison != 0)
			{
				return costComparison;
			}
			return Year.CompareTo(other.Year);
		}

		protected bool Equals(RemainWhithYear other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(Type, other.Type, comparisonIgnoreCase) 
			       && string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && Count == other.Count 
			       && Cost == other.Cost 
			       && Year == other.Year;
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
			return Equals((RemainWhithYear) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Type != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Type) : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				hashCode = (hashCode * 397) ^ Year.GetHashCode();
				return hashCode;
			}
		}
	}
}
