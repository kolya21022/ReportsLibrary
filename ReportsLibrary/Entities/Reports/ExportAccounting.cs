using System;

namespace ReportsLibrary.Entities.Reports
{
	/// <summary>
	/// Запись отчета [Учет экспорта]
	/// </summary>
	/// <inheritdoc />
	public class ExportAccounting : IComparable<ExportAccounting>
	{
		/// <summary>
		/// Доп. параметр для облегчения подсчета вывода в самом отчете
		/// </summary>
		public int Group => 1;

		/// <summary>
		/// Код месяца (для сортировки)
		/// </summary>
		public decimal IdMonth { get; set; }

		/// <summary>
		/// Название месяца
		/// </summary>
		public string Month { get; set; }



		/// <summary>
		/// Ближайшее мес. бел. предыдущий год
		/// </summary>
		public decimal NearMonthOld { get; set; }

		/// <summary>
		/// Ближайшее мес. дол. предыдущий год
		/// </summary>
		public decimal NearMonthUsdOld { get; set; }

		/// <summary>
		/// Дальнее мес. бел. предыдущий год
		/// </summary>
		public decimal FartherMonthOld { get; set; }

		/// <summary>
		/// Дальнее мес. дол. предыдущий год
		/// </summary>
		public decimal FartherMonthUsdOld { get; set; }

		/// <summary>
		/// Ближайшее год бел. предыдущий год
		/// </summary>
		public decimal NearYearOld { get; set; }

		/// <summary>
		/// Ближайшее год дол. предыдущий год
		/// </summary>
		public decimal NearYearUsdOld { get; set; }

		/// <summary>
		/// Дальнее год бел. предыдущий год
		/// </summary>
		public decimal FartherYearOld { get; set; }

		/// <summary>
		/// Дальнее год дол. предыдущий год
		/// </summary>
		public decimal FartherYearUsdOld { get; set; }


		/// <summary>
		/// Ближайшее мес. бел. текущий год
		/// </summary>
		public decimal NearMonthToday { get; set; }

		/// <summary>
		/// Ближайшее мес. дол. текущий год
		/// </summary>
		public decimal NearMonthUsdToday { get; set; }

		/// <summary>
		/// Дальнее мес. бел. текущий год
		/// </summary>
		public decimal FartherMonthToday { get; set; }

		/// <summary>
		/// Дальнее мес. дол. текущий год
		/// </summary>
		public decimal FartherMonthUsdToday { get; set; }

		/// <summary>
		/// Ближайшее год бел. текущий год
		/// </summary>
		public decimal NearYearToday { get; set; }

		/// <summary>
		/// Ближайшее год дол. текущий год
		/// </summary>
		public decimal NearYearUsdToday { get; set; }

		/// <summary>
		/// Дальнее год бел. текущий год
		/// </summary>
		public decimal FartherYearToday { get; set; }

		/// <summary>
		/// Дальнее год дол. текущий год
		/// </summary>
		public decimal FartherYearUsdToday { get; set; }


		public int CompareTo(ExportAccounting other)
		{
			if (ReferenceEquals(this, other))
			{
				return 0;
			}

			if (ReferenceEquals(null, other))
			{
				return 1;
			}
			var idMonthComparison = IdMonth.CompareTo(other.IdMonth);
			if (idMonthComparison != 0)
			{
				return idMonthComparison;
			}
			var monthComparison = string.Compare(Month, other.Month, StringComparison.OrdinalIgnoreCase);
			if (monthComparison != 0)
			{
				return monthComparison;
			}
			var nearMonthOldComparison = NearMonthOld.CompareTo(other.NearMonthOld);
			if (nearMonthOldComparison != 0)
			{
				return nearMonthOldComparison;
			}
			var nearMonthUsdOldComparison = NearMonthUsdOld.CompareTo(other.NearMonthUsdOld);
			if (nearMonthUsdOldComparison != 0)
			{
				return nearMonthUsdOldComparison;
			}
			var fartherMonthOldComparison = FartherMonthOld.CompareTo(other.FartherMonthOld);
			if (fartherMonthOldComparison != 0)
			{
				return fartherMonthOldComparison;
			}
			var fartherMonthUsdOldComparison = FartherMonthUsdOld.CompareTo(other.FartherMonthUsdOld);
			if (fartherMonthUsdOldComparison != 0)
			{
				return fartherMonthUsdOldComparison;
			}
			var nearYearOldComparison = NearYearOld.CompareTo(other.NearYearOld);
			if (nearYearOldComparison != 0)
			{
				return nearYearOldComparison;
			}
			var nearYearUsdOldComparison = NearYearUsdOld.CompareTo(other.NearYearUsdOld);
			if (nearYearUsdOldComparison != 0)
			{
				return nearYearUsdOldComparison;
			}
			var fartherYearOldComparison = FartherYearOld.CompareTo(other.FartherYearOld);
			if (fartherYearOldComparison != 0)
			{
				return fartherYearOldComparison;
			}
			var fartherYearUsdOldComparison = FartherYearUsdOld.CompareTo(other.FartherYearUsdOld);
			if (fartherYearUsdOldComparison != 0)
			{
				return fartherYearUsdOldComparison;
			}
			var nearMonthTodayComparison = NearMonthToday.CompareTo(other.NearMonthToday);
			if (nearMonthTodayComparison != 0)
			{
				return nearMonthTodayComparison;
			}
			var nearMonthUsdTodayComparison = NearMonthUsdToday.CompareTo(other.NearMonthUsdToday);
			if (nearMonthUsdTodayComparison != 0)
			{
				return nearMonthUsdTodayComparison;
			}
			var fartherMonthTodayComparison = FartherMonthToday.CompareTo(other.FartherMonthToday);
			if (fartherMonthTodayComparison != 0)
			{
				return fartherMonthTodayComparison;
			}
			var fartherMonthUsdTodayComparison = FartherMonthUsdToday.CompareTo(other.FartherMonthUsdToday);
			if (fartherMonthUsdTodayComparison != 0)
			{
				return fartherMonthUsdTodayComparison;
			}
			var nearYearTodayComparison = NearYearToday.CompareTo(other.NearYearToday);
			if (nearYearTodayComparison != 0)
			{
				return nearYearTodayComparison;
			}
			var nearYearUsdTodayComparison = NearYearUsdToday.CompareTo(other.NearYearUsdToday);
			if (nearYearUsdTodayComparison != 0)
			{
				return nearYearUsdTodayComparison;
			}
			var fartherYearTodayComparison = FartherYearToday.CompareTo(other.FartherYearToday);
			if (fartherYearTodayComparison != 0)
			{
				return fartherYearTodayComparison;
			}
			return FartherYearUsdToday.CompareTo(other.FartherYearUsdToday);
		}

		protected bool Equals(ExportAccounting other)
		{
			return IdMonth == other.IdMonth 
			       && string.Equals(Month, other.Month, StringComparison.OrdinalIgnoreCase) 
			       && NearMonthOld == other.NearMonthOld 
			       && NearMonthUsdOld == other.NearMonthUsdOld 
			       && FartherMonthOld == other.FartherMonthOld 
			       && FartherMonthUsdOld == other.FartherMonthUsdOld 
			       && NearYearOld == other.NearYearOld 
			       && NearYearUsdOld == other.NearYearUsdOld 
			       && FartherYearOld == other.FartherYearOld 
			       && FartherYearUsdOld == other.FartherYearUsdOld 
			       && NearMonthToday == other.NearMonthToday 
			       && NearMonthUsdToday == other.NearMonthUsdToday 
			       && FartherMonthToday == other.FartherMonthToday 
			       && FartherMonthUsdToday == other.FartherMonthUsdToday 
			       && NearYearToday == other.NearYearToday 
			       && NearYearUsdToday == other.NearYearUsdToday 
			       && FartherYearToday == other.FartherYearToday 
			       && FartherYearUsdToday == other.FartherYearUsdToday;
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
			return Equals((ExportAccounting) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = IdMonth.GetHashCode();
				hashCode = (hashCode * 397) ^ (Month != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Month) : 0);
				hashCode = (hashCode * 397) ^ NearMonthOld.GetHashCode();
				hashCode = (hashCode * 397) ^ NearMonthUsdOld.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherMonthOld.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherMonthUsdOld.GetHashCode();
				hashCode = (hashCode * 397) ^ NearYearOld.GetHashCode();
				hashCode = (hashCode * 397) ^ NearYearUsdOld.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherYearOld.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherYearUsdOld.GetHashCode();
				hashCode = (hashCode * 397) ^ NearMonthToday.GetHashCode();
				hashCode = (hashCode * 397) ^ NearMonthUsdToday.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherMonthToday.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherMonthUsdToday.GetHashCode();
				hashCode = (hashCode * 397) ^ NearYearToday.GetHashCode();
				hashCode = (hashCode * 397) ^ NearYearUsdToday.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherYearToday.GetHashCode();
				hashCode = (hashCode * 397) ^ FartherYearUsdToday.GetHashCode();
				return hashCode;
			}
		}
	}
}
