using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photovoltaique.API.Controllers.Dto.Down;
using Photovoltaique.API.Controllers.Dto.Up;
using Photovoltaique.API.Entities;
using System.Globalization;

namespace Photovoltaique.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitesController : ControllerBase
    {
        [HttpGet]
        public List<SiteDown> GetSites()
        {
            return Data.Sites.Select(site => new SiteDown(site)).ToList();
        }

        [HttpPost("Consumers")]
        public async Task<IActionResult> SetGeographicalConsumers(IFormFile file)
        {
            if (file != null)
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                };

                using (var stream = new StreamReader(file.OpenReadStream()))
                {
                    using (var csv = new CsvReader(stream, config))
                    {
                        var sites = csv.GetRecords<SiteWrapper>();
                        Data.Sites = sites.Select(site => new Site()
                        {
                            Latitude = site.Latitude,
                            Longitude = site.Longitude,
                            Name = site.Name,
                            Production = false,
                            Type = site.Type,
                        }).ToList();
                    }
                }

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("Productors")]
        public async Task<IActionResult> SetGeographicalProductors(IFormFile file)
        {
            if (file != null)
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture);

                using (var stream = new StreamReader(file.OpenReadStream()))
                {
                    using (var csv = new CsvReader(stream, config))
                    {
                        var sites = csv.GetRecords<SiteWrapper>();
                        Data.Sites = sites.Select(site => new Site()
                        {
                            Latitude = site.Latitude,
                            Longitude = site.Longitude,
                            Name = site.Name,
                            Production = true,
                            Type = site.Type,
                        }).ToList();
                    }
                }

                return Ok();
            }

            return BadRequest();
        }
    }

    public class SiteWrapper
    {
        [Name("Nom")]
        public string Name { get; set; }
        [Name("Type d'utilisation")]
        public string Type { get; set; }
        [Name("Longitude")]
        public double Longitude { get; set; }
        [Name("Latitude")]
        public double Latitude { get; set; }
    }
}
