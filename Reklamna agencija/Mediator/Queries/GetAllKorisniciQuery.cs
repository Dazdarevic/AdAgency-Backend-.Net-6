using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reklamna_agencija.Mediator.Queries
{
    public class GetAllKorisniciQuery : IRequest<ActionResult<IEnumerable<object>>> { }

}
