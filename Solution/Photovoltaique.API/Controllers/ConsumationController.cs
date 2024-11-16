using Microsoft.AspNetCore.Mvc;
using Photovoltaique.API.Controllers.Dto.Down;
using Photovoltaique.API.Controllers.Dto.Up;
using Photovoltaique.API.Entities;

namespace Photovoltaique.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsumationController : ControllerBase
    {
        // Rayon de la Terre en kilomètres
        private const double MaxPower = 1238;
        private const double EarthRadiusKm = 6371;
        private readonly string[] Months = { "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" };

        [HttpPost]
        public ConsumationDown RetrieveSites(Coordinate coordinate)
        {
            var selected = Data.Sites.Where(site => !site.Production 
                && IsPointInCircle(coordinate.Latitude, coordinate.Longitude, site.Latitude, site.Longitude, 1.0)).ToList();

            var sum = selected
                .SelectMany(site => site.Consomations)
                .GroupBy(conso => conso.Time)
                .Select(group => new Consomation()
                {
                    Time = group.Key,
                    Value = group.Sum(conso => conso.Value),
                });

            var production = Data.Production
                .Select(production => new Consomation() { Value = production.Value * MaxPower, Time = production.Time });

            var calcul = sum.Join(production, s => s.Time, 
                prod => prod.Time, 
                (s, prod) => new 
                {
                    Time = s.Time, 
                    Consomation = s.Value, 
                    TotalProduction = prod.Value,
                    ValuableProduction = s.Value <= prod.Value ? s.Value : prod.Value,
                    Surplus = prod.Value - s.Value });

            return new ConsumationDown()
            {
                AutoConsumationRate = calcul.Sum(group => group.ValuableProduction) / calcul.Sum(group => group.TotalProduction),
                AutoProductionRate = calcul.Sum(group => group.ValuableProduction) / calcul.Sum(group => group.Consomation),
                Consumation = calcul
                    .GroupBy(c => c.Time.Month)
                    .OrderBy(group => group.Key)
                    .Select(group => new MonthlyConsumation()
                    {
                        Month = Months[group.Key - 1],
                        ValuableProduction = group.Sum(g => g.ValuableProduction),
                        Surplus = group.Sum(g => g.Surplus),
                    })
                    .ToList(),
            };          
        }        

        /// <summary>
        /// Convertit un angle de degrés en radians.
        /// </summary>
        /// <param name="degrees">Angle en degrés.</param>
        /// <returns>Angle en radians.</returns>
        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// Calcule la distance entre deux points géographiques en utilisant la formule de Haversine.
        /// </summary>
        /// <param name="lat1">Latitude du premier point.</param>
        /// <param name="lon1">Longitude du premier point.</param>
        /// <param name="lat2">Latitude du second point.</param>
        /// <param name="lon2">Longitude du second point.</param>
        /// <returns>Distance en kilomètres.</returns>
        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double lat1Rad = ToRadians(lat1);
            double lat2Rad = ToRadians(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Vérifie si un point est situé dans un cercle géographique.
        /// </summary>
        /// <param name="centerLat">Latitude du centre du cercle.</param>
        /// <param name="centerLon">Longitude du centre du cercle.</param>
        /// <param name="pointLat">Latitude du point à tester.</param>
        /// <param name="pointLon">Longitude du point à tester.</param>
        /// <param name="radiusKm">Rayon du cercle en kilomètres.</param>
        /// <returns>True si le point est dans le cercle, sinon False.</returns>
        private static bool IsPointInCircle(double centerLat, double centerLon, double pointLat, double pointLon, double radiusKm)
        {
            double distance = HaversineDistance(centerLat, centerLon, pointLat, pointLon);
            return distance <= radiusKm;
        }
    }
}
