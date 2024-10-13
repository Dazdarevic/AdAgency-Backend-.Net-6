using MediatR;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Queries;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class EmailExistsQueryHandler : IRequestHandler<EmailExistsQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmailExistsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(EmailExistsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.KorisnikRepository.EmailExists(request.Email);
        }
    }
}
