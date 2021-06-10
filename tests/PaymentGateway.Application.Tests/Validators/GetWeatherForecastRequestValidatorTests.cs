using System;
using System.Linq;
using PaymentGateway.Application.Validators;
using PaymentGateway.Models.Dtos.WeatherForecast;
using Xunit;

namespace PaymentGateway.Application.Tests.Validators
{
    public class GetWeatherForecastRequestValidatorTests
    {
        private readonly GetWeatherForecastRequestValidator _systemUnderTest = new();

        [Fact]
        public void Validate_StartDateIsNotDefined_ThrowsError()
        {
            // Arrange.
            var request = new GetWeatherForecastRequest(DateTime.MinValue);
            
            // Act.
            var result = _systemUnderTest.Validate(request);
            var error = result.Errors.First(x =>
                x.PropertyName == nameof(GetWeatherForecastRequest.FromDate));
            // Assert.
            Assert.False(result.IsValid);
            Assert.Equal("GreaterThanValidator", error.ErrorCode);
        }
        
        [Fact]
        public void Validate_ToDateLessThanFromDate_ThrowsError()
        {
            // Arrange.
            var request = new GetWeatherForecastRequest(DateTime.Today, DateTime.MinValue);
            
            // Act.
            var result = _systemUnderTest.Validate(request);
            var error = result.Errors.First(x =>
                x.PropertyName == nameof(GetWeatherForecastRequest.ToDate));
            // Assert.
            Assert.False(result.IsValid);
            Assert.Equal("To date has to be greater than from date if defined", error.ErrorMessage);
        }
    }
}