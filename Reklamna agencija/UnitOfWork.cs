using Reklamna_agencija.Data;
using Reklamna_agencija.Interfaces;
using Reklamna_agencija.Repositories;

namespace Reklamna_agencija
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext dc;

        public UnitOfWork(DataContext dc)
        {
            this.dc = dc;
        }

        public IKorisnikRepository KorisnikRepository => new KorisnikRepository(dc);


        public async Task<bool> SaveAsync()
        {
            return await dc.SaveChangesAsync() > 0;
        }
    }
}
