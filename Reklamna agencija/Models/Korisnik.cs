using System.ComponentModel.DataAnnotations;

namespace Reklamna_agencija.Models
{
    public class Korisnik
    {
        [Key]
        public int Id { get; set; }
        public Boolean? Status { get; set; }
        public string? Prezime { get; set; }
        public string? Ime { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? BrTel { get; set; }
        public string? Uloga { get; set; }
        public string? Pol { get; set; }
        public string? Profilna { get; set; }
        public DateTime? DatumRodjenja { get; set; }
    }
}
