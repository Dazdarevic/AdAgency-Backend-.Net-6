namespace Reklamna_agencija.Models
{
    public class Faktura
    {
        public int Id { get; set; }
        public int AdminAgencijeId { get; set; }
        public AdminAgencije? AdminAgencije { get; set; }
        public int ReklamaId { get; set; }
        public Reklama? Reklama { get; set; }
        public string? Opis { get; set; }
        public DateTime? Datum { get; set; }
        public string? IznosNovcani { get; set; }
        public Boolean Status { get; set; }
    }
}
