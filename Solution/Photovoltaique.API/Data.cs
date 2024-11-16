using Photovoltaique.API.Entities;
using System.Collections.Generic;

namespace Photovoltaique.API
{
    public static class Data
    {
        public static List<Site> Sites;
        public static List<Consomation> Production;

        public static void Seed()
        {
            /*Sites = new List<Site>()
            {
                new Site()
                {
                    Name = "APPT N1 GS CHAMPS PERDRIX",
                    Production = false,
                    Type = "Groupe Scolaire",
                    /*Longitude = 5.00188970565796,
                    Latitude = 47.3207778930664,
                    Longitude = 0,
                    Latitude = 0,
                    Consomations = GenerateConsomation(),
                },
                new Site()
                {
                    Name = "APPT N1 GS LAMARTINE",
                    Production = false,
                    Type = "Groupe Scolaire",
                    /*Longitude = 5.05582141876221,
                    Latitude = 47.3408164978027,
                    Longitude = 0,
                    Latitude = 0,
                    Consomations = GenerateConsomation(),
                },
                new Site()
                {
                    Name = "APPT N1 GS MANSART CGT",
                    Production = false,
                    Type = "Groupe Scolaire",
                   /* Longitude = 5.0612645149231,
                    Latitude = 47.3092956542969,
                    Longitude = 0,
                    Latitude = 0,
                    Consomations = GenerateConsomation(),
                },
                new Site()
                {
                    Name = "AUXILIAIRES CENTRALE PV DEPÔT DU TRAMWAY",
                    Production = true,
                    Type = "Equipement Public",
                    /*Longitude = 5.02460050582886,
                    Latitude = 47.2956848144531,
                },
            };*/

            Sites = new List<Site>();

            for (int i = 0; i < 100; i++)
            {
                Sites.Add(new Site()
                {
                    Production = false,
                    Longitude = 0,
                    Latitude = 0,
                    Consomations = GenerateConsomation(),
                });
            }


            Production = GenerateConsomation(800);
        }

        private static List<Consomation> GenerateConsomation(int maxConso = 1)
        {
            var conso = new List<Consomation>();
            Random random = new Random();

            var periods = 365 * 24 * 2;

            for (int i = 0; i < periods; i++)
            {
                conso.Add(new Consomation()
                {
                    Time = (new DateTime(2023, 1, 1)).AddMinutes(i * 15),
                    Value = random.NextDouble() * maxConso,
                });
            }

            return conso;
        }
    }
}
