using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Queries;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class GetKorisnikInfoQueryHandler : IRequestHandler<GetKorisnikInfoQuery, ActionResult<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetKorisnikInfoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult<object>> Handle(GetKorisnikInfoQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.KorisnikRepository.GetKorisnikInfo(request.Id, request.Role);
        }
    }
}
