using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Card> Cards { get; set; }

        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupCardEntity(modelBuilder);
            SetupPaymentEntity(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void SetupCardEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasIndex(x => new {x.CardNumber})
                    .IsUnique();
                entity.HasKey(x => x.Id);

                entity.Property(x => x.CardNumber)
                    .HasMaxLength(200);

                entity.Property(x => x.HolderName);
            });
        }

        private static void SetupPaymentEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Currency)
                    .HasMaxLength(50);

                entity.Property(x => x.Key)
                    .HasMaxLength(150);

                entity.Property(c => c.Status)
                    .HasConversion<string>()
                    .HasMaxLength(100);

                entity.HasOne(x => x.Card)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}