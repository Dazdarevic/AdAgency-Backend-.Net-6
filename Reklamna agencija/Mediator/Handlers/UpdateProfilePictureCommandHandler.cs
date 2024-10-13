using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Mediator.Commands;

namespace Reklamna_agencija.Mediator.Handlers
{
    public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, IActionResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfilePictureCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.KorisnikRepository.UpdateProfilePictureUrlAsync(request.UserId, request.Role, request.ProfilePictureUrl);
            await _unitOfWork.SaveAsync();
            return new OkObjectResult("Profile picture updated successfully.");
        }
    }
}
