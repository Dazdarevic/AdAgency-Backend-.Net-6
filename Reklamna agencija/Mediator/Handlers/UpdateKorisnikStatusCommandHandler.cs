using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Commands;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class UpdateKorisnikStatusCommandHandler : IRequestHandler<UpdateKorisnikStatusCommand, IActionResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateKorisnikStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Handle(UpdateKorisnikStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.KorisnikRepository.UpdateKorisnikStatus(request.Id, request.Email);
            await _unitOfWork.SaveAsync();
            return result;
        }
    }
}
