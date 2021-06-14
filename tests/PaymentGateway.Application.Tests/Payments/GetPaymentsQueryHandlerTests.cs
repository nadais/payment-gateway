using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Application.Payments.Queries;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Models.Payments;
using Xunit;

namespace PaymentGateway.Application.Tests.Payments
{
    public class GetPaymentsQueryHandlerTests
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IMapper _mapper = new MapperFixture().CreateMapper();
        private GetPaymentsQueryHandler _systemUnderTest;

        private GetPaymentsQueryHandler CreateSystemUnderTest()
        {
            return new(_appDbContext,
                _mapper);
        }

        public GetPaymentsQueryHandlerTests()
        {
            _appDbContext = DbContextHelper.GetDbContext();
        }

        [Fact]
        public async Task Handle_TopAndSkipDefined_ReturnsOneRecordAndSkipsOne()
        {
            // Arrange.
            var shopperId = Guid.NewGuid();
            _appDbContext.Payments.AddRange(new Payment
            {
                ShopperId = shopperId
            }, new Payment
            {
                ShopperId = shopperId
            });
            var query = new GetPaymentsRequest
            {
                Top = 1,
                Skip = 1
            };
            await _appDbContext.SaveChangesAsync();

            _systemUnderTest = CreateSystemUnderTest();

            var result = await _systemUnderTest.Handle(new GetPaymentsQuery(shopperId, query), CancellationToken.None);

            Assert.Equal(2, result.Total);
            Assert.Equal(1, result.Top);
            Assert.Equal(1, result.Skip);
            Assert.Equal(1, result.Records.Count);
        }

        [Fact]
        public async Task Handle_MultiplePaymentsWithDifferentQuantities_ReturnsPaymentsOrderedByQuantity()
        {
            // Arrange.
            var shopperId = Guid.NewGuid();
            for (var i = 0; i < 100; i++)
            {
                var random = new Random();
                _appDbContext.Payments.Add(new Payment
                {
                    ShopperId = shopperId,
                    Amount = random.Next(int.MaxValue)
                });
            }

            var query = new GetPaymentsRequest
            {
                Top = 100,
                OrderBy = "amount",
                OrderByDescending = true
            };
            await _appDbContext.SaveChangesAsync();

            _systemUnderTest = CreateSystemUnderTest();

            // Act.
            var result = await _systemUnderTest.Handle(new GetPaymentsQuery(shopperId, query), CancellationToken.None);

            var isOrderedDescending = true;
            for (var i = 0; i < result.Records.Count - 1; i++)
            {
                isOrderedDescending = isOrderedDescending &&
                                      result.Records.ElementAt(i).Amount > result.Records.ElementAt(i + 1).Amount;
            }

            // Assert.
            Assert.NotEmpty(result.Records);
            Assert.True(isOrderedDescending);
        }

        [Fact]
        public async Task Handle_MultiplePaymentsOnDifferentShopper_ReturnsEmpty()
        {
            // Arrange.
            var shopperId = Guid.NewGuid();
            for (var i = 0; i < 100; i++)
            {
                _appDbContext.Payments.Add(new Payment
                {
                    ShopperId = shopperId
                });
            }

            var query = new GetPaymentsRequest
            {
                Top = 100
            };
            await _appDbContext.SaveChangesAsync();

            _systemUnderTest = CreateSystemUnderTest();

            // Act.
            var result =
                await _systemUnderTest.Handle(new GetPaymentsQuery(Guid.NewGuid(), query), CancellationToken.None);

            // Assert.
            Assert.Empty(result.Records);
        }
    }
}