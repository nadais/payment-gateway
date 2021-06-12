using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Application.Common.Abstractions
{
    public interface IAppDbContext
    {
        DbSet<Card> Cards { get; set; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}