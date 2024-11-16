using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photovoltaique.API.Controllers.Dto.Down;
using Photovoltaique.API.Controllers.Dto.Up;
using Photovoltaique.API.Entities;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Text;

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

                using (var stream = new StreamReader(file.OpenReadStream(), Encoding.Latin1))
                {
                    using (var csv = new CsvReader(stream, config))
                    {
                        var sites = csv.GetRecords<SiteWrapper>();
                        Data.Sites = sites.Select(site => new Site()
                        {
                            Latitude = double.Parse(site.Latitude),
                            Longitude = double.Parse(site.Longitude),
                            Name = site.Name,
                            Production = false,
                            Type = site.Type,
                        }).ToList();

                        Data.Sites.Add(new Site()
                        {
                            Latitude = 47.2956848144531,
                            Longitude = 5.02460050582886,
                            Name = "AUXILIAIRES CENTRALE PV DEPÔT DU TRAMWAY",
                            Production = true,
                            Type = "Equipement Public",
                        });
                    }
                }

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SetSites2023(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier n'a été fourni.");

            // Temporairement stocker le fichier dans un flux mémoire
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Réinitialiser la position pour la lecture

            // Lire les données CSV
            using var reader = new StreamReader(memoryStream, Encoding.Latin1);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // Assurez-vous qu'il y a une ligne d'en-tête
                Delimiter = ";"
            });

            // Charger toutes les lignes
            var records = csv.GetRecords<ElectricityRecord>().ToList();

            // Filtrer les lignes où la colonne "date" commence par "2023"
            var filteredRecords = records.Where(r => r.Date != null && r.Date.Year == 2023).ToList();

            // Grouper les données par tranches horaires
            var groupedByHour = filteredRecords
                .GroupBy(record => new DateTime(record.Date.Year, record.Date.Month, record.Date.Day, record.Date.Hour, 0, 0)) // Grouper par heure
                .Select(group => new Consomation
                {
                    Time = group.Key,  // Clé du groupe : le début de l'heure
                    Value = group.Average(r => r.Valeur) // Moyenne des valeurs
                })
                .OrderBy(result => result.Time) // Trier par ordre chronologique
                .ToList();

            Site current = Data.Sites.Single(s => s.Name == records[0].Name);

            current.Consomations = groupedByHour;

            return Ok(groupedByHour.Count);
        }

        [HttpPost("Production")]
        public async Task<IActionResult> SetProduction(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier n'a été fourni.");

            // Temporairement stocker le fichier dans un flux mémoire
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Réinitialiser la position pour la lecture

            // Lire les données CSV
            using var reader = new StreamReader(memoryStream, Encoding.Latin1);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // Assurez-vous qu'il y a une ligne d'en-tête
                Delimiter = ";"
            });

            // Charger toutes les lignes
            var records = csv.GetRecords<Production>().ToList();

            // Filtrer les lignes où la colonne "date" commence par "2023"
            var filteredRecords = records.Where(r => r.Year == 2023).ToList();

            // Grouper les données par tranches horaires
            var groupedByHour = filteredRecords
                .GroupBy(record => new DateTime(record.Year, record.Month, record.Day, record.Hour, 0, 0)) // Grouper par heure
                .Select(group => new Consomation
                {
                    Time = group.Key,  // Clé du groupe : le début de l'heure
                    Value = group.Average(r => r.Valeur) // Moyenne des valeurs
                })
                .OrderBy(result => result.Time) // Trier par ordre chronologique
                .ToList();


            Data.Production = groupedByHour;

            return Ok(groupedByHour.Count);
        }
    }

    public class SiteWrapper
    {
        [Name("Nom")]
        public string Name { get; set; }
        [Name("Type d'utilisation")]
        public string Type { get; set; }
        [Name("Longitude")]
        public string Longitude { get; set; }
        [Name("Latitude")]
        public string Latitude { get; set; }
    }

    public class ElectricityRecord
    {
        [Name("nom")]
        public string Name { get; set; }
        [Name("date")]
        public DateTime Date { get; set; }
        [Name("valeur")]
        public double Valeur { get; set; }
    }

    public class Production
    {
        [Name("Année")]
        public int Year { get; set; }
        [Name("Mois")]
        public int Month { get; set; }
        [Name("Jour")]
        public int Day { get; set; }
        [Name("Heure")]
        public int Hour { get; set; }
        [Name("Valeur")]
        public double Valeur { get; set; }
    }
}
