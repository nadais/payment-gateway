using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<Card> Cards { get; set; }
        DbSet<Account> Accounts { get; set; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}