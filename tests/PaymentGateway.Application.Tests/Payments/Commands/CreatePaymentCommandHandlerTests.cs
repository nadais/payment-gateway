using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PaymentGateway.Application.Cards;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Payments;
using Xunit;

namespace PaymentGateway.Application.Tests.Payments.Commands
{
    public class CreatePaymentCommandHandlerTests
    {
        private CreatePaymentCommandHandler _systemUnderTest;
        private readonly IMapper _mapper = new MapperFixture().CreateMapper();
        private readonly IMediator _mediator = Substitute.For<IMediator>();
        private readonly IAppDbContext _appDbContext;
        private readonly ICardEncryptionService _cardEncryptionService = new CardEncryptionService();
        private readonly IBankService _bankService = Substitute.For<IBankService>();
        private readonly ILogger<CreatePaymentCommandHandler> _logger = Substitute.For<ILogger<CreatePaymentCommandHandler>>();
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        private CreatePaymentCommandHandler CreateSystemUnderTests()
        {
            return new(_appDbContext,
                _cardEncryptionService,
                _bankService,
                _dateTimeProvider,
                _mapper,
                _mediator, 
                _logger);
        }

        public CreatePaymentCommandHandlerTests()
        {
            _appDbContext = DbContextHelper.GetDbContext();
        }

        [Fact]
        public async Task Handle_CardIdNotFound_ThrowsNotFoundException()
        {
            // Arrange.
            _systemUnderTest = CreateSystemUnderTests();

            // Assert.
            var error = await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                // Act.
                await _systemUnderTest.Handle(new CreatePaymentCommand(new CreatePaymentRequest
                {
                    CardId = Guid.NewGuid(),
                    Cvv = 12
                }), CancellationToken.None);
            });
            
            Assert.Equal(HttpStatusCode.NotFound, error.StatusCode);
            Assert.Equal("NOT_FOUND", error.ErrorCode);
        }
        
        [Fact]
        public async Task Handle_CvvProvidedDoesNotMatchDatabase_ThrowsException()
        {
            // Arrange.
            var id = Guid.NewGuid();
            _appDbContext.Cards.Add(new Card
            {
                Id = id,
                Cvv = _cardEncryptionService.GetEncryptedCvv(123)
            });
            await _appDbContext.SaveChangesAsync();
            _systemUnderTest = CreateSystemUnderTests();

            // Assert.
            var error = await Assert.ThrowsAsync<ApiException>(async () =>
            {
                // Act.
                await _systemUnderTest.Handle(new CreatePaymentCommand(new CreatePaymentRequest
                {
                    CardId = id,
                    Cvv = 12
                }), CancellationToken.None);
            });
            
            Assert.Equal(HttpStatusCode.BadRequest, error.StatusCode);
            Assert.Equal("INVALID_CVV", error.ErrorCode);
        }
    }
}