using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photovoltaique.API.Controllers.Dto.Down;
using Photovoltaique.API.Entities;
using System.Globalization;
using System.Text;
using CsvHelper.Configuration.Attributes;

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
            using var reader = new StreamReader(memoryStream);
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
                .Select(group => new
                {
                    Timestamp = group.Key,  // Clé du groupe : le début de l'heure
                    AverageValue = group.Average(r => r.Valeur) // Moyenne des valeurs
                })
                .OrderBy(result => result.Timestamp) // Trier par ordre chronologique
                .ToList();

            // Retourner les résultats
            return Ok(groupedByHour);
        }

        public class ElectricityRecord
        {
            [Name("date")]
            public DateTime Date { get; set; }
            [Name("valeur")]
            public double Valeur { get; set; }
        }
    }
}
