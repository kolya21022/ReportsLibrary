using System;
using System.Collections.Generic;

namespace ReportsLibrary.Entities.External
{
	/// <summary>
	/// Организация
	/// </summary>
	/// <inheritdoc />
	public class Company : IComparable<Company>
	{
		public decimal Id { get; set; }
		public string Name { get; set; }
		public string Unp { get; set; }
		public string City { get; set; }
		public string CodeTerritory { get; set; }
		public Bank Bank { get; set; }

		/// <summary>
		/// Используется для подстановки строкового значения при поиске
		/// </summary>
		public string ServiceSearchResultDisplayed =>
			$"{Name}{(City == null ? string.Empty : " г." + City)}";

		public string ServiceSearchResultPayerDisplayed =>
			$"Организация: {Name} || {Bank.CurrentAccount ?? string.Empty} || Банк: {Bank.Name}";

		public int CompareTo(Company other)
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
			var idComparison = Id.CompareTo(other.Id);
			if (idComparison != 0)
			{
				return idComparison;
			}
			var nameComparison = string.Compare(Name, other.Name, comparisonIgnoreCase);
			if (nameComparison != 0)
			{
				return nameComparison;
			}
			var unpComparison = string.Compare(Unp, other.Unp, comparisonIgnoreCase);
			if (unpComparison != 0)
			{
				return unpComparison;
			}
			var cityComparison = string.Compare(City, other.City, comparisonIgnoreCase);
			if (cityComparison != 0)
			{
				return cityComparison;
			}
			var codeTerritoryComparison = string.Compare(CodeTerritory, other.CodeTerritory, comparisonIgnoreCase);
			if (codeTerritoryComparison != 0)
			{
				return codeTerritoryComparison;
			}
			return Comparer<Bank>.Default.Compare(Bank, other.Bank);
		}

		protected bool Equals(Company other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return Id == other.Id
				   && string.Equals(Name, other.Name, comparisonIgnoreCase)
				   && string.Equals(Unp, other.Unp, comparisonIgnoreCase)
				   && string.Equals(City, other.City, comparisonIgnoreCase)
				   && string.Equals(CodeTerritory, other.CodeTerritory, comparisonIgnoreCase)
				   && Equals(Bank, other.Bank);
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

			if (obj.GetType() != this.GetType())
			{
				return false;
			}
			return Equals((Company)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ (Unp != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Unp) : 0);
				hashCode = (hashCode * 397) ^ (City != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(City) : 0);
				hashCode = (hashCode * 397) ^ (CodeTerritory != null
							   ? StringComparer.OrdinalIgnoreCase.GetHashCode(CodeTerritory) : 0);
				hashCode = (hashCode * 397) ^ (Bank != null ? Bank.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}
