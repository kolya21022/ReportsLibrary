using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Для отчета по учету поставок (за месяц)]
	/// </summary>
	/// <inheritdoc />
	public class ForReportOnAccountingSuppliesMonth : IComparable<ForReportOnAccountingSuppliesMonth>
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
		/// Название продукта
		/// </summary>
		public string Name { get; set; }

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
		/// Название потребителя
		/// </summary>
		public string NameCompany { get; set; }

		/// <summary>
		/// ТТН
		/// </summary>
		public decimal Ttn { get; set; }

		/// <summary>
		/// Дата отгрузки
		/// </summary>
		public DateTime ShipmentDate { get; set; }

		/// <summary>
		/// Номер договора
		/// </summary>
		public string ContractNumber { get; set; }

		/// <summary>
		/// Дата договора
		/// </summary>
		public DateTime ContractDate { get; set; }

		/// <summary>
		/// Цель приобретения
		/// </summary>
		public string IntentionBay { get; set; }

		/// <summary>
		/// Форма оплаты
		/// </summary>
		public string PaymentForm { get; set; }

		/// <summary>
		/// Ответственный отдел
		/// </summary>
		public string WorkGuild { get; set; }

		public int CompareTo(ForReportOnAccountingSuppliesMonth other)
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
			var summNdsComparison = SummNds.CompareTo(other.SummNds);
			if (summNdsComparison != 0)
			{
				return summNdsComparison;
			}
			var nameCompanyComparison = string.Compare(NameCompany, other.NameCompany, comparisonIgnoreCase);
			if (nameCompanyComparison != 0)
			{
				return nameCompanyComparison;
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
			var contractNumberComparison = string.Compare(ContractNumber, other.ContractNumber, comparisonIgnoreCase);
			if (contractNumberComparison != 0)
			{
				return contractNumberComparison;
			}
			var contractDateComparison = ContractDate.CompareTo(other.ContractDate);
			if (contractDateComparison != 0)
			{
				return contractDateComparison;
			}
			var intentionBayComparison = string.Compare(IntentionBay, other.IntentionBay, comparisonIgnoreCase);
			if (intentionBayComparison != 0)
			{
				return intentionBayComparison;
			}
			var paymentFormComparison = string.Compare(PaymentForm, other.PaymentForm, comparisonIgnoreCase);
			if (paymentFormComparison != 0)
			{
				return paymentFormComparison;
			}
			return string.Compare(WorkGuild, other.WorkGuild, comparisonIgnoreCase);
		}

		protected bool Equals(ForReportOnAccountingSuppliesMonth other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(TypeGroup, other.TypeGroup, comparisonIgnoreCase) 
			       && string.Equals(Type, other.Type, comparisonIgnoreCase) 
			       && string.Equals(Category, other.Category, comparisonIgnoreCase) 
			       && string.Equals(Territory, other.Territory, comparisonIgnoreCase) 
			       && string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && Count == other.Count 
			       && Cost == other.Cost 
			       && SummNds == other.SummNds 
			       && string.Equals(NameCompany, other.NameCompany, comparisonIgnoreCase) 
			       && Ttn == other.Ttn 
			       && ShipmentDate.Equals(other.ShipmentDate) 
			       && string.Equals(ContractNumber, other.ContractNumber, comparisonIgnoreCase) 
			       && ContractDate.Equals(other.ContractDate) 
			       && string.Equals(IntentionBay, other.IntentionBay, comparisonIgnoreCase) 
			       && string.Equals(PaymentForm, other.PaymentForm, comparisonIgnoreCase) 
			       && string.Equals(WorkGuild, other.WorkGuild, comparisonIgnoreCase);
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
			return Equals((ForReportOnAccountingSuppliesMonth) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (TypeGroup != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(TypeGroup) : 0);
				hashCode = (hashCode * 397) ^ (Type != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Type) : 0);
				hashCode = (hashCode * 397) ^ (Category != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Category) : 0);
				hashCode = (hashCode * 397) ^ (Territory != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Territory) : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				hashCode = (hashCode * 397) ^ SummNds.GetHashCode();
				hashCode = (hashCode * 397) ^ (NameCompany != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(NameCompany) : 0);
				hashCode = (hashCode * 397) ^ Ttn.GetHashCode();
				hashCode = (hashCode * 397) ^ ShipmentDate.GetHashCode();
				hashCode = (hashCode * 397) ^ (ContractNumber != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ContractNumber) : 0);
				hashCode = (hashCode * 397) ^ ContractDate.GetHashCode();
				hashCode = (hashCode * 397) ^ (IntentionBay != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(IntentionBay) : 0);
				hashCode = (hashCode * 397) ^ (PaymentForm != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(PaymentForm) : 0);
				hashCode = (hashCode * 397) ^ (WorkGuild != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(WorkGuild) : 0);
				return hashCode;
			}
		}
	}
}
