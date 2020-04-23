using System;

namespace ReportsLibrary.Entities.External
{
	/// <summary>
	/// Изделие/Продукция/Услуга
	/// </summary>
	public class Product : IComparable<Product>
	{
		public decimal Id { get; set; }
		public string Name { get; set; }

	    public int CompareTo(Product other)
	    {
	        if (ReferenceEquals(this, other))
	        {
	            return 0;
	        }

	        if (ReferenceEquals(null, other))
	        {
	            return 1;
	        }
	        var nameComparison = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
	        if (nameComparison != 0)
	        {
	            return nameComparison;
	        }
	        return Id.CompareTo(other.Id);
	    }

	    protected bool Equals(Product other)
	    {
	        return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) 
	               && Id == other.Id;
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
	        return Equals((Product) obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return ((Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0) * 397) ^ Id.GetHashCode();
	        }
	    }
	}
}

