using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Models;

namespace Reklamna_agencija.Mediator.Commands
{
    public class CreateKorisnikCommand : IRequest<ActionResult<Korisnik>>
    {
        public Korisnik Korisnik { get; set; }
    }
}
