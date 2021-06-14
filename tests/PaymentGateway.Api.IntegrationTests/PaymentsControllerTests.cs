using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using PaymentGateway.Api.IntegrationTests.Extensions;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Bank;
using PaymentGateway.Infrastructure.Clients;
using PaymentGateway.Models.Cards;
using PaymentGateway.Models.Payments;
using Refit;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace PaymentGateway.Api.IntegrationTests
{
    public class PaymentsControllerTests
    {
        private const string PaymentsUrl = "/api/payments";
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        [Fact]
        public async Task CreatePayment_ValidNumberAndExpirationDate_ReturnsOk()
        {
            // Arrange.
            var server = WireMockServer.Start();
            var factory = CreateFactory(server);
            var client = factory.CreateClientWithShopperId();

            server.Given(Request.Create().WithPath("/transfer").UsingPost())
                .RespondWith(
                    Response.Create().WithBodyAsJson(new BankPaymentResponse
                    {
                        Id = Guid.NewGuid(),
                        IsSuccessful = true
                    }));
            var payment = new CreatePaymentRequest
            {
                Card = new CardRequest
                {
                    CardNumber = "5425209346554051",
                    ExpirationMonth = 11,
                    ExpirationYear = 19,
                    Cvv = 123
                },
                Currency = "EUR",
                Amount = 10
            };

            // Act.
            var response = await client.PostAsync($"{PaymentsUrl}", payment.CreateByteContent());
            var createdPayment = await response.Content.ReadAsJsonAsync<PaymentDto>();

            // Assert.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotEqual(Guid.Empty, createdPayment.Id);
             Assert.Equal(PaymentStatus.Success, createdPayment.Status);
        }


        [Fact]
        public async Task CreatePayment_BankServiceIsUnavailable_ReturnsPaymentFailedButStatusOk()
        {
            // Arrange.
            var server = WireMockServer.Start();
            var factory = CreateFactory(server);
            var client = factory.CreateClientWithShopperId();

            server.Given(Request.Create().WithPath("/transfer").UsingPost())
                .RespondWith(
                    Response.Create().WithStatusCode(HttpStatusCode.GatewayTimeout));
            var payment = new CreatePaymentRequest
            {
                Card = new CardRequest
                {
                    CardNumber = "5425209346554051",
                    ExpirationMonth = 11,
                    ExpirationYear = 19,
                    Cvv = 123
                },
                Currency = "EUR",
                Amount = 10
            };

            // Act.
            var response = await client.PostAsync($"{PaymentsUrl}", payment.CreateByteContent());
            var createdPayment = await response.Content.ReadAsJsonAsync<PaymentDto>();
            // Assert.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(PaymentStatus.Failed, createdPayment.Status);
        }

        private WebApplicationFactory<Startup> CreateFactory(IWireMockServer server)
        {
            _dateTimeProvider.GetCurrentTime().Returns(new DateTime(2018, 11, 20));

            var factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services
                            .AddRefitClient<IBankServiceClient>()
                            .ConfigureHttpClient(c => c.BaseAddress = new Uri(server.Urls[0]));
                        services.AddScoped(_ => _dateTimeProvider);
                    });
                });
            return factory;
        }
    }
}