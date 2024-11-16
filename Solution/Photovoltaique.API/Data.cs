using Photovoltaique.API.Entities;

namespace Photovoltaique.API
{
    public static class Data
    {
        public static List<Site> Sites;

        public static void Seed()
        {
            Sites = new List<Site>()
            {
                new Site()
                {
                    Name = "APPT N1 GS CHAMPS PERDRIX",
                    Production = false,
                    Type = "Groupe Scolaire",
                    Longitude = 5.00188970565796,
                    Latitude = 47.3207778930664,
                },
                new Site()
                {
                    Name = "APPT N1 GS LAMARTINE",
                    Production = false,
                    Type = "Groupe Scolaire",
                    Longitude = 5.05582141876221,
                    Latitude = 47.3408164978027,
                },
                new Site()
                {
                    Name = "APPT N1 GS MANSART CGT",
                    Production = false,
                    Type = "Groupe Scolaire",
                    Longitude = 5.0612645149231,
                    Latitude = 47.3092956542969,
                },
                new Site()
                {
                    Name = "AUXILIAIRES CENTRALE PV DEPÔT DU TRAMWAY",
                    Production = true,
                    Type = "Equipement Public",
                    Longitude = 5.02460050582886,
                    Latitude = 47.2956848144531,
                },
            };
        }
    }
}
