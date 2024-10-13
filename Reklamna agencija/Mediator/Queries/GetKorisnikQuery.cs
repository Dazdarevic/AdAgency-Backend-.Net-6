using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reklamna_agencija.Mediator.Queries
{
    public class GetKorisnikQuery : IRequest<ActionResult<object>>
    {
        public int Id { get; set; }
    }
}
