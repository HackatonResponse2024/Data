﻿using CsvHelper;
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
                            Latitude = double.Parse(site.Latitude),
                            Longitude = double.Parse(site.Longitude),
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
                            Latitude = double.Parse(site.Latitude),
                            Longitude = double.Parse(site.Longitude),
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
}
