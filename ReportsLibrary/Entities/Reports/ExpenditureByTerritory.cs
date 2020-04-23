using System;

namespace ReportsLibrary.Entities.Reports
{
    /// <summary>
    /// Запись отчета [Расход по территориям]
    /// </summary>
    public class ExpenditureByTerritory : IComparable<ExpenditureByTerritory>
    {
        /// <summary>
        /// Доп. параметр для облегчения подсчета вывода в самом отчете
        /// </summary>
        public int Group => 1;

        /// <summary>
        /// Код месяца (для сортировки)
        /// </summary>
        public int MonthId { get; set; }

        /// <summary>
        /// Название месяца
        /// </summary>
        public string Month { get; set; }

        /// <summary>
        /// Наименование предприятия
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Наименование продукции
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Сумма в рублях
        /// </summary>
        public decimal SummRb { get; set; }


        /// <summary>
        /// Сумма в долларах
        /// </summary>
        public decimal SummUsd { get; set; }

        /// <summary>
        /// Деньги или бартер
        /// </summary>
        public string MoneyOrBarter { get; set; }

        public int CompareTo(ExpenditureByTerritory other)
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

            var monthIdComparison = MonthId.CompareTo(other.MonthId);
            if (monthIdComparison != 0)
            {
                return monthIdComparison;
            }

            var monthComparison = string.Compare(Month, other.Month, comparisonIgnoreCase);
            if (monthComparison != 0)
            {
                return monthComparison;
            }

	        var productNameComparison = string.Compare(ProductName, other.ProductName, comparisonIgnoreCase);
	        if (productNameComparison != 0)
	        {
		        return productNameComparison;
	        }

			var companyNameComparison = string.Compare(CompanyName, other.CompanyName, comparisonIgnoreCase);
            if (companyNameComparison != 0)
            {
                return companyNameComparison;
            }

            var summRbComparison = SummRb.CompareTo(other.SummRb);
            if (summRbComparison != 0)
            {
                return summRbComparison;
            }

            var summUsdComparison = SummUsd.CompareTo(other.SummUsd);
            if (summUsdComparison != 0)
            {
                return summUsdComparison;
            }

            return string.Compare(MoneyOrBarter, other.MoneyOrBarter, comparisonIgnoreCase);
        }

        protected bool Equals(ExpenditureByTerritory other)
        {
            const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
            return MonthId == other.MonthId 
                   && string.Equals(Month, other.Month, comparisonIgnoreCase) 
                   && string.Equals(CompanyName, other.CompanyName, comparisonIgnoreCase) 
                   && string.Equals(ProductName, other.ProductName, comparisonIgnoreCase) 
                   && SummRb == other.SummRb 
                   && SummUsd == other.SummUsd 
                   && string.Equals(MoneyOrBarter, other.MoneyOrBarter, comparisonIgnoreCase);
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
            return Equals((ExpenditureByTerritory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MonthId;
                hashCode = (hashCode * 397) ^ (Month != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Month) : 0);
                hashCode = (hashCode * 397) ^ (CompanyName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(CompanyName) : 0);
                hashCode = (hashCode * 397) ^ (ProductName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ProductName) : 0);
                hashCode = (hashCode * 397) ^ SummRb.GetHashCode();
                hashCode = (hashCode * 397) ^ SummUsd.GetHashCode();
                hashCode = (hashCode * 397) ^ (MoneyOrBarter != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(MoneyOrBarter) : 0);
                return hashCode;
            }
        }
    }
}