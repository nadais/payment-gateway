using System;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Application.Tests
{
    public static class DbContextHelper
    {
        public static IAppDbContext GetDbContext()
        {
            var options = GetOptions<AppDbContext>();
            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }
        
        private static DbContextOptions<T> GetOptions<T>() where T : DbContext
        {
            return new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }
    }
}