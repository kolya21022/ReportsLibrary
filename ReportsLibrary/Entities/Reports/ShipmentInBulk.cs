namespace ReportsLibrary.Entities.Reports
{
    //TODO пока что не используется (будущий отчет [Отгрузка в натуральном выражении])
    public class ShipmentInBulk
    {
        public decimal RasStan { get; set; }
        public decimal RasMeb { get; set; }
        public decimal MebSop { get; set; }
        public decimal RasDoor { get; set; }
        public decimal RasWindow { get; set; }
        public decimal RasPilo { get; set; }
        public decimal CountStan { get; set; }
        public decimal CountMeb { get; set; }
        public decimal CountDoor { get; set; }
        public decimal CountWindow { get; set; }
        public decimal CountPilo { get; set; }
    }
}
