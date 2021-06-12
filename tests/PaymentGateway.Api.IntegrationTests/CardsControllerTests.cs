using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using PaymentGateway.Api.IntegrationTests.Extensions;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Models.Cards;
using Xunit;

namespace PaymentGateway.Api.IntegrationTests
{
    public class CardsControllerTests
    {
        private const string CardsUrl = "/api/cards";
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();


        [Fact]
        public async Task AssignPicking_DefaultCase_ReturnsErrorResponse()
        {
            _dateTimeProvider.GetCurrentTime().Returns(new DateTime(2021, 10, 20));
            var factory = new CustomWebApplicationFactory(services =>
            {
                services.Replace(ServiceDescriptor.Transient(typeof(IDateTimeProvider), x => _dateTimeProvider));
            });
            var client  = factory.CreateClientWithDefaultApiKey();
            var card = new CardDto
            {
                CardNumber = "5425209346554051", //randomly generated card number,
                ExpirationMonth = 11,
                ExpirationYear = 31
            };
            var response = await client.PostAsync($"{CardsUrl}/validation", card.CreateByteContent());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}