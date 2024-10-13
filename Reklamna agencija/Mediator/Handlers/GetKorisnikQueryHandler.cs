using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Queries;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class GetKorisnikQueryHandler : IRequestHandler<GetKorisnikQuery, ActionResult<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetKorisnikQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult<object>> Handle(GetKorisnikQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.KorisnikRepository.GetKorisnik(request.Id);
        }
    }
}
