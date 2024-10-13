using System.ComponentModel.DataAnnotations;

namespace Reklamna_agencija.Models
{
    public class Reklama
    {
        [Key]
        public int Id { get; set; }
        public int KlijentId { get; set; }
        public Klijent? Klijent { get; set; }
        public int ReklamniPanoId { get; set; }
        public ReklamniPano? ReklamniPano { get; set; }
        public string? UrlSlike { get; set; }
        public DateTime OdDatum { get; set; }
        public DateTime DoDatum { get; set; }
        public string? Opis { get; set; }
        public Boolean? Status { get; set; }
    }
}
