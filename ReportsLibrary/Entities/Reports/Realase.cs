using System;

namespace ReportsLibrary.Entities.Reports
{
    /// <summary>
    /// Запись отчета [Выпуск]
    /// </summary>
    /// <inheritdoc />
    public class Realase : IComparable<Realase>
    {
        /// <summary>
        /// Наименование категории продукта
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        public decimal Count { get; set; }

        public int CompareTo(Realase other)
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
            return Count.CompareTo(other.Count);
        }

        protected bool Equals(Realase other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) 
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
            return Equals((Realase) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0) * 397) ^ Count.GetHashCode();
            }
        }
    }
}
