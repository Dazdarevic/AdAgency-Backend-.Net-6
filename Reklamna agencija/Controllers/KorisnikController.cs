using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Commands;
using Reklamna_agencija.Mediator.Queries;
using Reklamna_agencija.Models;

namespace Reklamna_agencija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly IMediator _mediator;

        public KorisnikController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("dodaj-korisnika")]
        public async Task<ActionResult<Korisnik>> AddKorisnik(Korisnik korisnik)
        {
            return await _mediator.Send(new CreateKorisnikCommand { Korisnik = korisnik });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetKorisnik(int id)
        {
            return await _mediator.Send(new GetKorisnikQuery { Id = id });
        }

        [HttpGet("mojprofil")]
        public async Task<ActionResult<object>> GetKorisnikInfo(int id, string role)
        {
            return await _mediator.Send(new GetKorisnikInfoQuery { Id = id, Role = role });
        }

        [HttpGet("korisnici")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllKorisnici()
        {
            return await _mediator.Send(new GetAllKorisniciQuery());
        }

        [HttpGet("neodobreni-korisnici")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllInactiveKorisnici()
        {
            return await _mediator.Send(new GetAllInactiveKorisniciQuery());
        }

        [HttpPut("odobrena-registracija")]
        public async Task<IActionResult> UpdateKorisnikStatus(int id, string email)
        {
            return await _mediator.Send(new UpdateKorisnikStatusCommand { Id = id, Email = email });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteKorisnik(int id, string email)
        {
            return await _mediator.Send(new DeleteKorisnikCommand { Id = id, Email = email });
        }

        [HttpPost("update-profile-picture")]
        public async Task<IActionResult> UpdateProfilePictureUrlAsync(int userId, string role, string profilePictureUrl)
        {
            return await _mediator.Send(new UpdateProfilePictureCommand { UserId = userId, Role = role, ProfilePictureUrl = profilePictureUrl });
        }

        [HttpGet("email-exists/{email}")]
        public async Task<bool> EmailExists(string email)
        {
            return await _mediator.Send(new EmailExistsQuery { Email = email });
        }
    }
}