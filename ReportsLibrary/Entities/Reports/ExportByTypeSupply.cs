using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Экспорт по виду поставки]
	/// </summary>
	/// <inheritdoc />
	public class ExportByTypeSupply : IComparable<ExportByTypeSupply>
	{
		/// <summary>
		/// Поле для объеденения всех записей отчета в 1 группу (проще итог подсчитывать)
		/// </summary>
		public decimal Group => 1;

		/// <summary>
		/// Тип отгрузки
		/// </summary>
		public string TypeSupply { get; set; }

		/// <summary>
		/// Территория
		/// </summary>
		public string Territory { get; set; }

		/// <summary>
		/// Компания
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Наименование [Продукта]
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
		/// Указанная [Цена] в Usd
		/// </summary>
		public decimal CostUsd { get; set; }

		/// <summary>
		/// Указанная [Цена] в выбранной валюте
		/// </summary>
		public decimal CostVal { get; set; }

		protected bool Equals(ExportByTypeSupply other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(TypeSupply, other.TypeSupply, comparisonIgnoreCase) 
			       && string.Equals(Territory, other.Territory, comparisonIgnoreCase) 
			       && string.Equals(Company, other.Company, comparisonIgnoreCase) 
			       && string.Equals(Name, other.Name, comparisonIgnoreCase) 
			       && Count == other.Count 
			       && Cost == other.Cost 
			       && CostUsd == other.CostUsd 
			       && CostVal == other.CostVal;
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
			return Equals((ExportByTypeSupply) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (TypeSupply != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(TypeSupply) : 0);
				hashCode = (hashCode * 397) ^ (Territory != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Territory) : 0);
				hashCode = (hashCode * 397) ^ (Company != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Company) : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ Count.GetHashCode();
				hashCode = (hashCode * 397) ^ Cost.GetHashCode();
				hashCode = (hashCode * 397) ^ CostUsd.GetHashCode();
				hashCode = (hashCode * 397) ^ CostVal.GetHashCode();
				return hashCode;
			}
		}


		public int CompareTo(ExportByTypeSupply other)
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
			var typeShipmentComparison = string.Compare(TypeSupply, other.TypeSupply, comparisonIgnoreCase);
			if (typeShipmentComparison != 0)
			{
				return typeShipmentComparison;
			}
			var territoryComparison = string.Compare(Territory, other.Territory, comparisonIgnoreCase);
			if (territoryComparison != 0)
			{
				return territoryComparison;
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
			var countComparison = Count.CompareTo(other.Count);
			if (countComparison != 0)
			{
				return countComparison;
			}
			var costOneComparison = Cost.CompareTo(other.Cost);
			if (costOneComparison != 0)
			{
				return costOneComparison;
			}
			var costUsdOneComparison = CostUsd.CompareTo(other.CostUsd);
			if (costUsdOneComparison != 0)
			{
				return costUsdOneComparison;
			}
			return CostVal.CompareTo(other.CostVal);
		}
	}
}
