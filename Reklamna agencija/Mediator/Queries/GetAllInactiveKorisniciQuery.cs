using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reklamna_agencija.Mediator.Queries
{
    public class GetAllInactiveKorisniciQuery : IRequest<ActionResult<IEnumerable<object>>> { }

}
