using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reklamna_agencija.Mediator.Queries
{
    public class GetKorisnikInfoQuery : IRequest<ActionResult<object>>
    {
        public int Id { get; set; }
        public string Role { get; set; }
    }
}
