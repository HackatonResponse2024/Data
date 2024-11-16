using CsvHelper.Configuration.Attributes;

namespace Photovoltaique.API.Entities
{
    public class Site
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Name("Nom")]
        public string Name { get; set; }
        [Name("Type d'utilisation")]
        public string Type { get; set; }
        public bool Production { get; set; }
        [Name("Longitude")]
        public double Longitude { get; set; }
        [Name("Latitude")]
        public double Latitude { get; set; }

        public List<Consomation> Consomations { get; set; }
    }
}
