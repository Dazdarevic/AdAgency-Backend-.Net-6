using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Queries;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class GetAllInactiveKorisniciQueryHandler : IRequestHandler<GetAllInactiveKorisniciQuery, ActionResult<IEnumerable<object>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllInactiveKorisniciQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult<IEnumerable<object>>> Handle(GetAllInactiveKorisniciQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.KorisnikRepository.GetAllInactiveKorisnici();
        }
    }
}
