using System;

namespace ReportsLibrary.Entities.Reports
{
    /// <summary>
    /// Запись отчета [Экспорт по территориям]
    /// </summary>
    /// <inheritdoc />
    public class ExportByTerritory : IComparable<ExportByTerritory>
    {
        /// <summary>
		/// Доп. параметр для облегчения подсчета вывода в самом отчете
		/// </summary>
		public int Group => 1;

        /// <summary>
        /// Страна
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Мес кол-во предыдущий год
        /// </summary>
        public decimal MonthCountOld { get; set; }

        /// <summary>
        /// Мес. бел. предыдущий год
        /// </summary>
        public decimal MonthOld { get; set; }

        /// <summary>
        /// Мес. дол. предыдущий год
        /// </summary>
        public decimal MonthUsdOld { get; set; }

        /// <summary>
        /// Год бел. предыдущий год
        /// </summary>
        public decimal YearOld { get; set; }

        /// <summary>
        /// Год дол. предыдущий год
        /// </summary>
        public decimal YearUsdOld { get; set; }

        /// <summary>
        /// Год. кол-во предыдущий год
        /// </summary>
        public decimal YearCountOld { get; set; }


        /// <summary>
        /// Мес. бел. текущий год
        /// </summary>
        public decimal MonthToday { get; set; }

        /// <summary>
        /// Мес. дол. текущий год
        /// </summary>
        public decimal MonthUsdToday { get; set; }

        /// <summary>
        /// Мес. кол-во текущий год
        /// </summary>
        public decimal MonthCountToday { get; set; }

        /// <summary>
        /// Год бел. текущий год
        /// </summary>
        public decimal YearToday { get; set; }

        /// <summary>
        /// Год дол. текущий год
        /// </summary>
        public decimal YearUsdToday { get; set; }

        /// <summary>
        /// Год кол-во текущий год
        /// </summary>
        public decimal YearCountToday { get; set; }

        public int CompareTo(ExportByTerritory other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            var countryComparison = string.Compare(Country, other.Country, StringComparison.OrdinalIgnoreCase);
            if (countryComparison != 0)
            {
                return countryComparison;
            }
            var monthOldComparison = MonthOld.CompareTo(other.MonthOld);
            if (monthOldComparison != 0)
            {
                return monthOldComparison;
            }
            var monthUsdOldComparison = MonthUsdOld.CompareTo(other.MonthUsdOld);
            if (monthUsdOldComparison != 0)
            {
                return monthUsdOldComparison;
            }
            var monthCountOldComparison = MonthCountOld.CompareTo(other.MonthCountOld);
            if (monthCountOldComparison != 0)
            {
                return monthCountOldComparison;
            }
            var yearOldComparison = YearOld.CompareTo(other.YearOld);
            if (yearOldComparison != 0)
            {
                return yearOldComparison;
            }
            var yearUsdOldComparison = YearUsdOld.CompareTo(other.YearUsdOld);
            if (yearUsdOldComparison != 0)
            {
                return yearUsdOldComparison;
            }
            var yearCountOldComparison = YearCountOld.CompareTo(other.YearCountOld);
            if (yearCountOldComparison != 0)
            {
                return yearCountOldComparison;
            }
            var monthTodayComparison = MonthToday.CompareTo(other.MonthToday);
            if (monthTodayComparison != 0)
            {
                return monthTodayComparison;
            }
            var monthUsdTodayComparison = MonthUsdToday.CompareTo(other.MonthUsdToday);
            if (monthUsdTodayComparison != 0)
            {
                return monthUsdTodayComparison;
            }
            var monthCountTodayComparison = MonthCountToday.CompareTo(other.MonthCountToday);
            if (monthCountTodayComparison != 0)
            {
                return monthCountTodayComparison;
            }
            var yearTodayComparison = YearToday.CompareTo(other.YearToday);
            if (yearTodayComparison != 0)
            {
                return yearTodayComparison;
            }
            var yearUsdTodayComparison = YearUsdToday.CompareTo(other.YearUsdToday);
            if (yearUsdTodayComparison != 0)
            {
                return yearUsdTodayComparison;
            }
            return YearCountToday.CompareTo(other.YearCountToday);
        }

        protected bool Equals(ExportByTerritory other)
        {
            return string.Equals(Country, other.Country, StringComparison.OrdinalIgnoreCase) 
                   && MonthOld == other.MonthOld 
                   && MonthUsdOld == other.MonthUsdOld 
                   && MonthCountOld == other.MonthCountOld 
                   && YearOld == other.YearOld 
                   && YearUsdOld == other.YearUsdOld 
                   && YearCountOld == other.YearCountOld 
                   && MonthToday == other.MonthToday 
                   && MonthUsdToday == other.MonthUsdToday 
                   && MonthCountToday == other.MonthCountToday 
                   && YearToday == other.YearToday 
                   && YearUsdToday == other.YearUsdToday 
                   && YearCountToday == other.YearCountToday;
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
            return Equals((ExportByTerritory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Country != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Country) : 0);
                hashCode = (hashCode * 397) ^ MonthOld.GetHashCode();
                hashCode = (hashCode * 397) ^ MonthUsdOld.GetHashCode();
                hashCode = (hashCode * 397) ^ MonthCountOld.GetHashCode();
                hashCode = (hashCode * 397) ^ YearOld.GetHashCode();
                hashCode = (hashCode * 397) ^ YearUsdOld.GetHashCode();
                hashCode = (hashCode * 397) ^ YearCountOld.GetHashCode();
                hashCode = (hashCode * 397) ^ MonthToday.GetHashCode();
                hashCode = (hashCode * 397) ^ MonthUsdToday.GetHashCode();
                hashCode = (hashCode * 397) ^ MonthCountToday.GetHashCode();
                hashCode = (hashCode * 397) ^ YearToday.GetHashCode();
                hashCode = (hashCode * 397) ^ YearUsdToday.GetHashCode();
                hashCode = (hashCode * 397) ^ YearCountToday.GetHashCode();
                return hashCode;
            }
        }
    }
}
