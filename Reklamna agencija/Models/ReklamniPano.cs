namespace Reklamna_agencija.Models
{
    public class ReklamniPano
    {
        public int Id { get; set; }
        public int AdminAgencijeId { get; set; }
        public AdminAgencije? AdminAgencije { get; set; }
        public string? UrlSlike { get; set; }
        public string? Adresa { get; set; }
        public string? Dimenzija { get; set; }
        public string? Osvetljenost { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Grad { get; set; }
        public string? Zona { get; set; }
        public int? Cijena { get; set; }
        public Boolean? StatusZauzetosti { get; set; }

    }
}
