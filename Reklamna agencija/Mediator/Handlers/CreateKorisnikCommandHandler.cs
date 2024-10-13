using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Commands;
using Reklamna_agencija.Models;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class CreateKorisnikCommandHandler : IRequestHandler<CreateKorisnikCommand, ActionResult<Korisnik>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateKorisnikCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult<Korisnik>> Handle(CreateKorisnikCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.KorisnikRepository.AddKorisnik(request.Korisnik);
            await _unitOfWork.SaveAsync();
            return result;
        }
    }
}
