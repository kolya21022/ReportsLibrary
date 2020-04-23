using System;

namespace ReportsLibrary.Entities.External
{
	/// <summary>
	/// Банк
	/// </summary>
	/// <inheritdoc />
	public class Bank : IComparable<Bank>
	{
		public string Name { get; set; }
		public string Mfo { get; set; }

		/// <summary>
		/// Расчетный счет
		/// </summary>
		public string CurrentAccount { get; set; }
		public decimal CompanyId { get; set; }

		public int CompareTo(Bank other)
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
			var nameComparison = string.Compare(Name, other.Name, comparisonIgnoreCase);
			if (nameComparison != 0)
			{
				return nameComparison;
			}
			var companyIdComparison = CompanyId.CompareTo(other.CompanyId);
			if (companyIdComparison != 0)
			{
				return companyIdComparison;
			}
			var mfoComparison = string.Compare(Mfo, other.Mfo, comparisonIgnoreCase);
			if (mfoComparison != 0)
			{
				return mfoComparison;
			}
			return string.Compare(CurrentAccount, other.CurrentAccount, comparisonIgnoreCase);
		}

		protected bool Equals(Bank other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return string.Equals(Name, other.Name, comparisonIgnoreCase)
				   && CompanyId == other.CompanyId
				   && string.Equals(Mfo, other.Mfo, comparisonIgnoreCase)
				   && string.Equals(CurrentAccount, other.CurrentAccount, comparisonIgnoreCase);
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
			return Equals((Bank)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
				hashCode = (hashCode * 397) ^ CompanyId.GetHashCode();
				hashCode = (hashCode * 397) ^ (Mfo != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Mfo) : 0);
				hashCode = (hashCode * 397) ^ (CurrentAccount != null ?
							   StringComparer.OrdinalIgnoreCase.GetHashCode(CurrentAccount) : 0);
				return hashCode;
			}
		}
	}
}
