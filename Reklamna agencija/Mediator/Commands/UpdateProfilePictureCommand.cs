using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reklamna_agencija.Mediator.Commands
{
    public class UpdateProfilePictureCommand : IRequest<IActionResult>
    {
        public int UserId { get; set; }
        public string Role { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
