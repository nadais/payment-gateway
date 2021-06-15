using System;
using System.Linq;
using NSubstitute;
using PaymentGateway.Application.Cards;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Models.Cards;
using Xunit;

namespace PaymentGateway.Application.Tests.Cards
{
    public class ValidateCardQueryHandlerTests
    {
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        private CardRequestValidator _systemUnderTest;

        private CardRequestValidator CreateSystemUnderTests()
        {
            return new(_dateTimeProvider);
        }

        [Theory]
        [InlineData(20,10)]
        [InlineData(21,9)]
        public void Validate_InvalidExpirationDate_ReturnsError(int expirationYear, int expirationMonth)
        {
            // Arrange.

            var referenceDate = new DateTime(3021, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();

            // Act.
            var result = _systemUnderTest.Validate(new CardRequest
            {
                ExpirationMonth = expirationMonth,
                ExpirationYear = expirationYear
            });

            // Assert.
            Assert.False(result.IsValid);
            var error = result.Errors.Single(x => x.PropertyName == string.Empty);
            Assert.Equal("The provided card is expired", error.ErrorMessage);
        }

        [Fact]
        public void Validate_CardYearAfterTurnOfTheCentury_ReturnsTrue()
        {
            // Arrange.
        
            var referenceDate = new DateTime(2095, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();
        
            // Act.
            var result = _systemUnderTest.Validate(new CardRequest
            {
                CardNumber = "5425209346554051",
                ExpirationMonth = 10,
                ExpirationYear = 04,
                Cvv = 1234
            });
        
            // Assert.
            Assert.True(result.IsValid);
        }
        
        [Theory]
        [InlineData(12)]
        [InlineData(12345)]
        public void Handle_CvvWithInvalidValue_ReturnsInvalid(int cvv)
        {
            // Arrange.
        
            var referenceDate = new DateTime(3021, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();

            // Act.
            var result = _systemUnderTest.Validate(new CardRequest
            {
                Cvv = cvv
            });

            // Assert.
            Assert.False(result.IsValid);
            var cardError =result.Errors.Single(x => x.PropertyName == nameof(CardRequest.Cvv));
            
            Assert.Equal("Cvv must be a number with between 3 and 4 digits", cardError.ErrorMessage);
        }
        
        [Fact]
        public void Handle_CardNumberIsNotValidCreditCard_ReturnsInvalid()
        {
            // Arrange.
        
            var referenceDate = new DateTime(3021, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();

            // Act.
            var result = _systemUnderTest.Validate(new CardRequest
            {
                CardNumber = "123456789"
            });

            // Assert.
            Assert.False(result.IsValid);
            var cardError =result.Errors.Single(x => x.PropertyName == nameof(CardDto.CardNumber));
            
            Assert.Equal("CreditCardValidator", cardError.ErrorCode);
        }
        
        [Fact]
        public void Validate_CardNumberIsEmpty_ReturnsInvalid()
        {
            // Arrange.
        
            var referenceDate = new DateTime(3021, 10, 10);
            _dateTimeProvider.GetCurrentTime().Returns(referenceDate);
            _systemUnderTest = CreateSystemUnderTests();

            // Act.
            var result = _systemUnderTest.Validate(new CardRequest());

            // Assert.
            Assert.False(result.IsValid);
            var cardError =result.Errors.Single(x => x.PropertyName == nameof(CardDto.CardNumber));
            
            Assert.Equal("NotEmptyValidator", cardError.ErrorCode);
        }
    }
}