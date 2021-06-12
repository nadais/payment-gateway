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
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasIndex(x => new {x.CardNumber})
                    .IsUnique();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}