using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Отгрузка]
	/// </summary>
	/// <inheritdoc />
	public class Shipment : IComparable<Shipment>
	{
		/// <summary>
		/// Код месяца
		/// </summary>
		public int IdMonth { get; set; }

		/// <summary>
		/// Месяц
		/// </summary>
		public string Month { get; set; }

		/// <summary>
		/// Внутренний рынок в учетных ценах
		/// </summary>
		public decimal MarketAccountingCost { get; set; }

		/// <summary>
		/// СНГ в учетных ценах
		/// </summary>
		public decimal SngAccountingCost { get; set; }

		/// <summary>
		/// Россия в учетных ценах
		/// </summary>
		public decimal RussiaAccountingCost { get; set; }

		/// <summary>
		/// Экспорт в учетных ценах
		/// </summary>
		public decimal ExportAccountingCost { get; set; }

		/// <summary>
		/// Всего в учетных ценах
		/// </summary>
		public decimal AccountingCost => MarketAccountingCost + SngAccountingCost + ExportAccountingCost;

		/// <summary>
		/// Внутренний рынок в договорных ценах
		/// </summary>
		public decimal MarketContractCost { get; set; }

		/// <summary>
		/// СНГ в договорных ценах
		/// </summary>
		public decimal SngContractCost { get; set; }

		/// <summary>
		/// Россия в договорных ценах
		/// </summary>
		public decimal RussiaContractCost { get; set; }

		/// <summary>
		/// Экспорт в договорных ценах
		/// </summary>
		public decimal ExportContractCost { get; set; }

		/// <summary>
		/// Всего в договорных ценах
		/// </summary>
		public decimal ContractCost => MarketContractCost + SngContractCost + ExportContractCost;

		protected bool Equals(Shipment other)
		{
			return IdMonth == other.IdMonth 
			       && string.Equals(Month, other.Month, StringComparison.OrdinalIgnoreCase) 
			       && MarketAccountingCost == other.MarketAccountingCost 
			       && SngAccountingCost == other.SngAccountingCost 
			       && RussiaAccountingCost == other.RussiaAccountingCost 
			       && ExportAccountingCost == other.ExportAccountingCost 
			       && MarketContractCost == other.MarketContractCost 
			       && SngContractCost == other.SngContractCost 
			       && RussiaContractCost == other.RussiaContractCost 
			       && ExportContractCost == other.ExportContractCost;
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
			return Equals((Shipment) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = IdMonth;
				hashCode = (hashCode * 397) ^ (Month != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Month) : 0);
				hashCode = (hashCode * 397) ^ MarketAccountingCost.GetHashCode();
				hashCode = (hashCode * 397) ^ SngAccountingCost.GetHashCode();
				hashCode = (hashCode * 397) ^ RussiaAccountingCost.GetHashCode();
				hashCode = (hashCode * 397) ^ ExportAccountingCost.GetHashCode();
				hashCode = (hashCode * 397) ^ MarketContractCost.GetHashCode();
				hashCode = (hashCode * 397) ^ SngContractCost.GetHashCode();
				hashCode = (hashCode * 397) ^ RussiaContractCost.GetHashCode();
				hashCode = (hashCode * 397) ^ ExportContractCost.GetHashCode();
				return hashCode;
			}
		}

		public int CompareTo(Shipment other)
		{
			if (ReferenceEquals(this, other))
			{
				return 0;
			}

			if (ReferenceEquals(null, other))
			{
				return 1;
			}
			return IdMonth.CompareTo(other.IdMonth);
		}
	}
}
