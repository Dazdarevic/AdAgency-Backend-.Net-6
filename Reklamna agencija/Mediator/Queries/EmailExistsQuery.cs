using MediatR;

namespace Reklamna_agencija.Mediator.Queries
{
    public class EmailExistsQuery : IRequest<bool>
    {
        public string Email { get; set; }
    }

}
