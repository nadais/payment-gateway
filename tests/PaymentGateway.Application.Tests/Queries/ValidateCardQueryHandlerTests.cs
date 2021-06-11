using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using PaymentGateway.Application.Abstractions;
using PaymentGateway.Application.Queries.Cards;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Dtos.Cards;
using Xunit;

namespace PaymentGateway.Application.Tests.Queries
{
    public class ValidateCardQueryHandlerTests
    {
        private ValidateCardQueryHandler _systemUnderTest;
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        private ValidateCardQueryHandler CreateSystemUnderTests()
        {
            return new(_dateTimeProvider);
        }

        [Theory]
        [InlineData(20,10)]
        [InlineData(21,9)]
        public async Task Handle_InvalidExpirationDate_ThrowsException(int expirationYear, int expirationMonth)
        {
            // Arrange.

            var referenceDate = new DateTime(3021, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();

            // Assert.
            var error = await Assert.ThrowsAsync<CardInvalidException>(async () =>
            {
                // Act.
                await _systemUnderTest.Handle(new ValidateCardQuery(new CardDto
                {
                    ExpirationMonth = expirationMonth,
                    ExpirationYear = expirationYear
                }), CancellationToken.None);
            });

            var cardError =error.ErrorInformation[nameof(ValidateCardQuery.Card)];
            Assert.Single(cardError);
            Assert.Equal("The provided card is expired", cardError.First());
        }
        
        [Theory]
        [InlineData("123456789")]
        [InlineData(null)]
        public async Task Handle_CardNumberDoesNotPassLuhn_ThrowsException(string cardNumber)
        {
            // Arrange.

            var referenceDate = new DateTime(3021, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();

            // Assert.
            var error = await Assert.ThrowsAsync<CardInvalidException>(async () =>
            {
                // Act.
                await _systemUnderTest.Handle(new ValidateCardQuery(new CardDto
                {
                    CardNumber = cardNumber
                }), CancellationToken.None);
            });

            var cardError =error.ErrorInformation[nameof(CardDto.CardNumber)];
            Assert.Single(cardError);
            Assert.Equal("The provided card number is not valid", cardError.First());
        }
    }
}