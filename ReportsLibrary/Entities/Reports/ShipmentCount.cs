using System;

namespace ReportsLibrary.Entities.Reports
{
    /// <summary>
    /// Запись отчета [Отгрузка(кол-во)]
    /// </summary>
    /// <inheritdoc />
    public class ShipmentCount : IComparable<ShipmentCount>
    {
        /// <summary>
        /// Наименование [Продукта]
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ед. измерения [Продукта]
        /// </summary>
        public string Measure { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public decimal Count { get; set; }

        protected bool Equals(ShipmentCount other)
        {
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            return string.Equals(Name, other.Name, comparisonIgnoreCase) 
                   && string.Equals(Measure, other.Measure, comparisonIgnoreCase) 
                   && Count == other.Count;
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
            return Equals((ShipmentCount) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
                hashCode = (hashCode * 397) ^ (Measure != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Measure) : 0);
                hashCode = (hashCode * 397) ^ Count.GetHashCode();
                return hashCode;
            }
        }


        public int CompareTo(ShipmentCount other)
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
            var measureComparison = string.Compare(Measure, other.Measure, comparisonIgnoreCase);
            if (measureComparison != 0)
            {
                return measureComparison;
            }
            return Count.CompareTo(other.Count);
        }
    }
}
