using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Abstractions;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Card> Cards { get; set; }

        public DbSet<Account> Accounts { get; set; }
    }
}