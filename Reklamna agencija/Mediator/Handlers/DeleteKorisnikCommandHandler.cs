using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Commands;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class DeleteKorisnikCommandHandler : IRequestHandler<DeleteKorisnikCommand, IActionResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteKorisnikCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Handle(DeleteKorisnikCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.KorisnikRepository.DeleteKorisnik(request.Id, request.Email);
            await _unitOfWork.SaveAsync();
            return result;
        }
    }
}
