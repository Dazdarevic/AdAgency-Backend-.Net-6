// IKorisnikRepository.cs
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Models;

namespace Reklamna_agencija.Interfaces
{
    public interface IKorisnikRepository
    {
        Task<ActionResult<Korisnik>> AddKorisnik(Korisnik korisnik);
        Task<ActionResult<object>> GetKorisnik(int id);
        Task<ActionResult<object>> GetKorisnikInfo(int id, string role);
        Task<ActionResult<IEnumerable<object>>> GetAllKorisnici();
        Task<ActionResult<IEnumerable<object>>> GetAllInactiveKorisnici();
        Task<IActionResult> UpdateKorisnikStatus(int id, string email);
        Task<IActionResult> DeleteKorisnik(int id, string email);
        Task UpdateProfilePictureUrlAsync(int userId, string role, string profilePictureUrl);
        Task<bool> EmailExists(string email);
    }
}