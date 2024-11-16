namespace Photovoltaique.API.Entities
{
    public class Site
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Production { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
