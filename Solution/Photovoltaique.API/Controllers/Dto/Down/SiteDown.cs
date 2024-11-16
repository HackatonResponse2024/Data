using Photovoltaique.API.Entities;

namespace Photovoltaique.API.Controllers.Dto.Down
{
    public class SiteDown
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Production { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public SiteDown(Site site)
        {
            Id = site.Id;
            Name = site.Name;
            Type = site.Type;
            Production = site.Production;
            Longitude = site.Longitude;
            Latitude = site.Latitude;
        }
    }
}
