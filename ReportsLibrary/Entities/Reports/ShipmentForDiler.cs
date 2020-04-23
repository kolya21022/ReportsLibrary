using System;

namespace ReportsLibrary.Entities.Reports
{

	/// <summary>
	/// Запись отчета [Отгружено дилерам]
	/// </summary>
	/// <inheritdoc />
	public class ShipmentForDiler : IComparable<ShipmentForDiler>
	{

		/// <summary>
		/// Наименование потребителя
		/// </summary>
		public string PotrName { get; set; }


		/// <summary>
		/// Наименование продукции
		/// </summary>
		public string ProductName { get; set; }

		/// <summary>
		/// Количество продукции
		/// </summary>
		public decimal ProductCount { get; set; }

		/// <summary>
		/// Сумма в долларах
		/// </summary>
		public decimal SummaUsd { get; set; }

		/// <summary>
		/// Сумма в РФ,РБ
		/// </summary>
		public decimal SummaRfRb { get; set; }

		public int CompareTo(ShipmentForDiler other)
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
			var potrNameComparison = string.Compare(PotrName, other.PotrName, comparisonIgnoreCase);
			if (potrNameComparison != 0)
			{
				return potrNameComparison;
			}
			var productNameComparison = string.Compare(ProductName, other.ProductName, comparisonIgnoreCase);
			if (productNameComparison != 0)
			{
				return productNameComparison;
			}
			var productCountComparison = ProductCount.CompareTo(other.ProductCount);
			if (productCountComparison != 0)
			{
				return productCountComparison;
			}
			var summaUsdComparison = SummaUsd.CompareTo(other.SummaUsd);
			if (summaUsdComparison != 0)
			{
				return summaUsdComparison;
			}
			return SummaRfRb.CompareTo(other.SummaRfRb);
		}

		protected bool Equals(ShipmentForDiler other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(PotrName, other.PotrName, comparisonIgnoreCase) 
			       && string.Equals(ProductName, other.ProductName, comparisonIgnoreCase) 
			       && ProductCount == other.ProductCount 
			       && SummaUsd == other.SummaUsd 
			       && SummaRfRb == other.SummaRfRb;
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
			return Equals((ShipmentForDiler) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (PotrName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(PotrName) : 0);
				hashCode = (hashCode * 397) ^ (ProductName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ProductName) : 0);
				hashCode = (hashCode * 397) ^ ProductCount.GetHashCode();
				hashCode = (hashCode * 397) ^ SummaUsd.GetHashCode();
				hashCode = (hashCode * 397) ^ SummaRfRb.GetHashCode();
				return hashCode;
			}
		}
	}
}
