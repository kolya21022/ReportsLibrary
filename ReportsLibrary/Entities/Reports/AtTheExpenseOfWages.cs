using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [В счет зарплаты]
	/// </summary>
	/// <inheritdoc />
	public class AtTheExpenseOfWages : IComparable<AtTheExpenseOfWages>
	{
		/// <summary>
		/// Для упрощения подсчета итога в отчете
		/// </summary>
		public int Group => 1;

		/// <summary>
		/// ТТН
		/// </summary>
		public decimal Ttn { get; set; }

		/// <summary>
		/// Дата отгрузки
		/// </summary>
		public DateTime ShipmentDate { get; set; }

		/// <summary>
		/// Наименование [Продукта]
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Кол-во
		/// </summary>
		public decimal Count { get; set; }

		/// <summary>
		/// [Цена] с НДС
		/// </summary>
		public decimal Cost { get; set; }

		protected bool Equals(AtTheExpenseOfWages other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return Ttn == other.Ttn 
			       && ShipmentDate.Equals(other.ShipmentDate) 
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
			return Equals((AtTheExpenseOfWages) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Ttn.GetHashCode();
				hashCode = (hashCode * 397) ^ ShipmentDate.GetHashCode();
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				return hashCode;
			}
		}


		public int CompareTo(AtTheExpenseOfWages other)
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
			var ttnComparison = Ttn.CompareTo(other.Ttn);
			if (ttnComparison != 0)
			{
				return ttnComparison;
			}
			var shipmentDateComparison = ShipmentDate.CompareTo(other.ShipmentDate);
			if (shipmentDateComparison != 0)
			{
				return shipmentDateComparison;
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
