using Microsoft.EntityFrameworkCore;
using Reklamna_agencija.Models;

namespace Reklamna_agencija.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<AdminAgencije> AdminiAgencije { get; set; }
        public DbSet<Klijent> Klijenti { get; set; }
        public DbSet<Posetilac> Posetioci { get; set; }
        public DbSet<Faktura> Fakture { get; set; }
        public DbSet<ReklamniPano> ReklamniPanoi { get; set; }
        public DbSet<Reklama> Reklame { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Faktura>()
                .HasOne(f => f.Reklama)
                .WithMany()
                .HasForeignKey(f => f.ReklamaId).OnDelete(DeleteBehavior.NoAction); // Postavka ON DELETE NO ACTION


            modelBuilder.Entity<ReklamniPano>()
                .HasOne(rp => rp.AdminAgencije)
                .WithMany()
                .HasForeignKey(rp => rp.AdminAgencijeId);

            modelBuilder.Entity<Reklama>()
                .HasOne(r => r.Klijent)
                .WithMany()
                .HasForeignKey(r => r.KlijentId);

            modelBuilder.Entity<Reklama>()
                .HasOne(r => r.ReklamniPano)
                .WithMany()
                .HasForeignKey(r => r.ReklamniPanoId);
        }
    }
}
