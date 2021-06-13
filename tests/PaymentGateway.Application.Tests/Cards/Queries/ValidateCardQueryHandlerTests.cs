using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using PaymentGateway.Application.Cards.Queries;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Cards;
using Xunit;

namespace PaymentGateway.Application.Tests.Cards.Queries
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
            var error = await Assert.ThrowsAsync<ApiException>(async () =>
            {
                // Act.
                await _systemUnderTest.Handle(new ValidateCardQuery(new CardRequest
                {
                    ExpirationMonth = expirationMonth,
                    ExpirationYear = expirationYear
                }), CancellationToken.None);
            });

            var cardError =error.ErrorInformation[nameof(ValidateCardQuery.Card)];
            Assert.Single(cardError);
            Assert.Equal("The provided card is expired", cardError.First());
        }
        
        [Fact]
        public async Task Handle_CardYearAfterTurnOfCentury_ReturnsTrue()
        {
            // Arrange.

            var referenceDate = new DateTime(2095, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();

            // Act.
            var result = await _systemUnderTest.Handle(new ValidateCardQuery(new CardRequest
            {
                CardNumber = "5425209346554051",
                ExpirationMonth = 10,
                ExpirationYear = 04
            }), CancellationToken.None);

            // Assert.
            Assert.True(result.IsValid);
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
            var error = await Assert.ThrowsAsync<ApiException>(async () =>
            {
                // Act.
                await _systemUnderTest.Handle(new ValidateCardQuery(new CardRequest
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