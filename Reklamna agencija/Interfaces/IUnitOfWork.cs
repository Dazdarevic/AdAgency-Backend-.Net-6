namespace Reklamna_agencija.Interfaces
{
    public interface IUnitOfWork
    {
        IKorisnikRepository KorisnikRepository { get; }

        Task<bool> SaveAsync();
    }
}
