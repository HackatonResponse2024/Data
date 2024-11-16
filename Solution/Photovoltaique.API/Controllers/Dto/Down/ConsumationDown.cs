namespace Photovoltaique.API.Controllers.Dto.Down
{
    public class ConsumationDown
    {
        public double AutoConsumationRate { get; set; }
        public double AutoProductionRate { get; set; }
        public List<MonthlyConsumation> Consumation { get; set; }
    }

    public class MonthlyConsumation
    {
        public string Month { get; set; }
        public double ValuableProduction { get; set; }
        public double Surplus { get; set; }
    }
}
