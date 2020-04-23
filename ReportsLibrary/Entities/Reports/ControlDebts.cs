using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Для контроля дебюторской задолжности]
	/// </summary>
	/// <inheritdoc />
	public class ControlDebts : IComparable<ControlDebts>
	{
		/// <summary>
		/// Компания
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Наименование [Продукта]
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// ТТН
		/// </summary>
		public decimal Ttn { get; set; }

		/// <summary>
		/// [Цена] с НДС
		/// </summary>
		public decimal Cost { get; set; }

		/// <summary>
		/// Номер договора
		/// </summary>
		public string NumberContract { get; set; }

		/// <summary>
		/// Форма оплаты
		/// </summary>
		public string FormPayment { get; set; }

		/// <summary>
		/// Дата отгрузки
		/// </summary>
		public DateTime ShipmentDate { get; set; }

		/// <summary>
		/// Срок оплаты
		/// </summary>
		public DateTime PaymentDate { get; set; }

		protected bool Equals(ControlDebts other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(Company, other.Company, comparisonIgnoreCase) 
			       && string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && Ttn == other.Ttn 
			       && Cost == other.Cost 
			       && string.Equals(NumberContract, other.NumberContract, comparisonIgnoreCase) 
			       && string.Equals(FormPayment, other.FormPayment, comparisonIgnoreCase) 
			       && ShipmentDate.Equals(other.ShipmentDate) 
			       && PaymentDate.Equals(other.PaymentDate);
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
			return Equals((ControlDebts) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Company != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Company) : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Ttn.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				hashCode = (hashCode * 397) ^ (NumberContract != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(NumberContract) : 0);
				hashCode = (hashCode * 397) ^ (FormPayment != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(FormPayment) : 0);
				hashCode = (hashCode * 397) ^ ShipmentDate.GetHashCode();
				hashCode = (hashCode * 397) ^ PaymentDate.GetHashCode();
				return hashCode;
			}
		}


		public int CompareTo(ControlDebts other)
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
			var ttnComparison = Ttn.CompareTo(other.Ttn);
			if (ttnComparison != 0)
			{
				return ttnComparison;
			}
			var costComparison = Cost.CompareTo(other.Cost);
			if (costComparison != 0)
			{
				return costComparison;
			}
			var numberContractComparison = string.Compare(NumberContract, other.NumberContract, comparisonIgnoreCase);
			if (numberContractComparison != 0)
			{
				return numberContractComparison;
			}
			var formPaymentComparison = string.Compare(FormPayment, other.FormPayment, comparisonIgnoreCase);
			if (formPaymentComparison != 0)
			{
				return formPaymentComparison;
			}
			var shipmentDateComparison = ShipmentDate.CompareTo(other.ShipmentDate);
			if (shipmentDateComparison != 0)
			{
				return shipmentDateComparison;
			}
			return PaymentDate.CompareTo(other.PaymentDate);
		}
	}
}
