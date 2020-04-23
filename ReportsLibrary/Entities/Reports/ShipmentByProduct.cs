using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Отгрузка по изделию]
	/// </summary>
	/// <inheritdoc />
	public class ShipmentByProduct : IComparable<ShipmentByProduct>
	{
		/// <summary>
		/// Наименование [Продукта]
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Ед. измерения [Продукта]
		/// </summary>
		public string Measure { get; set; }

		/// <summary>
		/// Потребитель
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Количество
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// Указанная [Цена] [Продукции/Изделия/Услуги]
		/// </summary>
		public decimal Cost { get; set; }

		public decimal Summa => Count * Cost;

		/// <summary>
		/// Дата отгрузки
		/// </summary>
		public DateTime Date { get; set; }

		protected bool Equals(ShipmentByProduct other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && string.Equals(Measure, other.Measure, comparisonIgnoreCase) 
			       && string.Equals(Company, other.Company, comparisonIgnoreCase) 
			       && Date.Equals(other.Date) 
			       && Cost == other.Cost 
			       && Count == other.Count;
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
			return Equals((ShipmentByProduct) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ (Measure != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Measure) : 0);
				hashCode = (hashCode * 397) ^ (Company != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Company) : 0);
				hashCode = (hashCode * 397) ^ Date.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				return hashCode;
			}
		}


		public int CompareTo(ShipmentByProduct other)
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
			var dateComparison = Date.CompareTo(other.Date);
			if (dateComparison != 0)
			{
				return -dateComparison;
			}
			var nameComparison = string.Compare(Name, other.Name, comparisonIgnoreCase);
			if (nameComparison != 0)
			{
				return nameComparison;
			}
			var measureComparison = string.Compare(Measure, other.Measure, comparisonIgnoreCase);
			if (measureComparison != 0)
			{
				return measureComparison;
			}
			var companyComparison = string.Compare(Company, other.Company, comparisonIgnoreCase);
			if (companyComparison != 0)
			{
				return companyComparison;
			}
			var costComparison = Cost.CompareTo(other.Cost);
			if (costComparison != 0)
			{
				return costComparison;
			}
			return Count.CompareTo(other.Count);
		}
	}
}
