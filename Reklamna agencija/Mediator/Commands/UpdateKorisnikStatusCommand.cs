﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reklamna_agencija.Mediator.Commands
{
    public class UpdateKorisnikStatusCommand : IRequest<IActionResult>
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }

}
