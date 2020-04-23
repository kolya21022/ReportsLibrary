using System;

namespace ReportsLibrary.Entities.External
{
	/// <summary>
	/// Цех предприятия
	/// </summary>
	/// <inheritdoc />
	[Serializable]
	public class WorkGuild : IComparable<WorkGuild>
	{
		public decimal Id { get; set; }
		public string Name { get; set; }

		private bool Equals(WorkGuild other)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			return Id == other.Id && string.Equals(Name, other.Name, comparisonIgnoreCase);
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
			return obj.GetType() == GetType() && Equals((WorkGuild)obj);
		}

		public override int GetHashCode()
		{
			var comparerIgnoreCase = StringComparer.OrdinalIgnoreCase;
			unchecked
			{
				return (Id.GetHashCode() * 397) ^ (Name != null ? comparerIgnoreCase.GetHashCode(Name) : 0);
			}
		}

		public int CompareTo(WorkGuild other)
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
			return idComparison != 0 ? idComparison : string.Compare(Name, other.Name, comparisonIgnoreCase);
		}
	}
}
