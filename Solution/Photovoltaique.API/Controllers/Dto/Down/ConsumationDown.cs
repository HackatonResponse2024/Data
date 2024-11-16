namespace Photovoltaique.API.Controllers.Dto.Down
{
    public class ConsumationDown
    {
        public double AutoConsumationRate { get; set; }
        public double AutoProductionRate { get; set; }
        public List<string> Months { get; set; }
        public List<double> ValuableProduction { get; set; }
        public List<double> Surplus { get; set; }
    }
}
