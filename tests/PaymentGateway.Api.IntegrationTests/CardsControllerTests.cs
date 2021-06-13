using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using PaymentGateway.Api.IntegrationTests.Extensions;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Models.Cards;
using Xunit;

namespace PaymentGateway.Api.IntegrationTests
{
    public class CardsControllerTests
    {
        internal const string CardsUrl = "/api/cards";
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        
        
        [Fact]
        public async Task CreateCard_ValidNumberAndExpirationDate_ReturnsOk()
        {
            // Arrange.
            _dateTimeProvider.GetCurrentTime().Returns(new DateTime(2018, 11, 20));
            var factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddScoped(_ => _dateTimeProvider);
                    });
                });
            var client  = factory.CreateClientWithDefaultApiKey();
            var card = new CardRequest
            {
                CardNumber = "5425209346554051",
                ExpirationMonth = 11,
                ExpirationYear = 19
            };
            
            // Act.
            var response = await client.PostAsync($"{CardsUrl}", card.CreateByteContent());
            var createdCard = await response.Content.ReadAsJsonAsync<CardDto>();
            // Assert.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEqual(Guid.Empty, createdCard.Id);
            Assert.Equal("************4051", createdCard.CardNumber);
            Assert.Equal(card.ExpirationMonth, createdCard.ExpirationMonth);
            Assert.Equal(card.ExpirationYear, createdCard.ExpirationYear);
        }
    }
}