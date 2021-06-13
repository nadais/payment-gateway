using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Application.Payments.Queries;
using PaymentGateway.Domain.Bank;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Cards;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments.Commands
{

    public record CreatePaymentCommand(Guid ShopperId, CreatePaymentRequest Request) : IRequest<PaymentDto>;
    
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAppDbContext _appDbContext;
        private readonly ICardEncryptionService _cardEncryptionService;
        private readonly IBankService _bankService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;

        public CreatePaymentCommandHandler(IAppDbContext appDbContext,
            ICardEncryptionService cardEncryptionService,
            IBankService bankService,
            IDateTimeProvider dateTimeProvider,
            IMapper mapper,
            IMediator mediator, ILogger<CreatePaymentCommandHandler> logger)
        {
            _appDbContext = appDbContext;
            _cardEncryptionService = cardEncryptionService;
            _bankService = bankService;
            _dateTimeProvider = dateTimeProvider;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<PaymentDto> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            var card = await GetCardById(command.Request.CardId, cancellationToken);

            ValidateProvidedCvv(command, card);

            var targetCard = _mapper.Map<CardRequest>(card) with {Cvv = command.Request.Cvv};

            var isPaymentSuccessful = await SendPaymentToBankApi(command, targetCard);

            var payment = await StorePaymentInDatabaseAsync(command, card, isPaymentSuccessful, cancellationToken);

            return await _mediator.Send(new GetPaymentByIdQuery(command.ShopperId, payment.Id), cancellationToken);
        }

        private async Task<BankPaymentResponse> SendPaymentToBankApi(CreatePaymentCommand command, CardRequest targetCard)
        {
            try
            {
                var isPaymentSuccessful = await _bankService.ProcessCardPaymentAsync(new BankPaymentRequest
                {
                    Currency = command.Request.Currency,
                    FromCard = targetCard,
                    Quantity = command.Request.Quantity,
                    ToAccountId = command.ShopperId
                });
                return isPaymentSuccessful;
            }
            catch (ApiException ex)
            {
                _logger.LogError("Error occured sending payment", ex);
                return new BankPaymentResponse
                {
                    IsSuccessful = false
                };
            }
        }

        private async Task<Card> GetCardById(Guid cardId, CancellationToken cancellationToken)
        {
            var card = await _appDbContext.Cards.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == cardId, cancellationToken);

            if (card == null)
            {
                throw new NotFoundException($"No card found with id {cardId}");
            }

            return card;
        }

        private void ValidateProvidedCvv(CreatePaymentCommand request, Card card)
        {
            var newCvv = _cardEncryptionService.GetEncryptedCvv(request.Request.Cvv);
            if (newCvv != card.Cvv)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The cvv provided does not match the card's cvv",
                    "INVALID_CVV");
            }
        }

        private async Task<Payment> StorePaymentInDatabaseAsync(CreatePaymentCommand request,
            Card card, BankPaymentResponse response, CancellationToken cancellationToken)
        {
            var payment = _mapper.Map<Payment>(request.Request);
            payment.CardNumber = card.CardNumber;
            payment.CreatedAt = _dateTimeProvider.GetCurrentTime();
            payment.CardId = card.Id;
            payment.Status = response.IsSuccessful ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.ExternalId = response.Id;

            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return payment;
        }
    }
}