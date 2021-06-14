using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Cards.Queries;
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
        private readonly IAppDbContext _appDbContext;
        private readonly IBankService _bankService;
        private readonly ICardEncryptionService _cardEncryptionService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

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
            var cardRequest = command.Request.Card;

            var isPaymentSuccessful = await SendPaymentToBankApi(command, cardRequest);

            var payment = await StorePaymentInDatabaseAsync(command, cardRequest, isPaymentSuccessful, cancellationToken);

            return await _mediator.Send(new GetPaymentByIdQuery(command.ShopperId, payment.Id), cancellationToken);
        }

        private async Task<BankPaymentResponse> SendPaymentToBankApi(CreatePaymentCommand command, CardRequest targetCard)
        {
            try
            {
                var bankPaymentResponse = await _bankService.ProcessCardPaymentAsync(new BankPaymentRequest
                {
                    Currency = command.Request.Currency,
                    FromCard = targetCard,
                    Quantity = command.Request.Amount,
                    ToAccountId = command.ShopperId
                });
                return bankPaymentResponse;
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

        private async Task<Payment> StorePaymentInDatabaseAsync(CreatePaymentCommand request,
            CardRequest cardRequest, BankPaymentResponse response, CancellationToken cancellationToken)
        {
            var payment = _mapper.Map<Payment>(request.Request);
            payment.CreatedAt = _dateTimeProvider.GetCurrentTime();
            payment.ShopperId = request.ShopperId;
            payment.Status = response.IsSuccessful ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.ExternalId = response.Id;

            var card = await GetCardToAdd(cardRequest, cancellationToken);
            if (card.Id == Guid.Empty)
            {
                payment.Card = card;
            }
            else
            {
                payment.CardId = card.Id;
            }
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return payment;
        }

        private async Task<Card> GetCardToAdd(CardRequest cardRequest, CancellationToken cancellationToken)
        {
            try
            {
                var storedCard =
                    await _mediator.Send(new GetCardByNumberQuery(cardRequest.CardNumber), cancellationToken);
                return new Card
                {
                    Id = storedCard.Id
                };
            }
            catch (NotFoundException)
            {
                var returnedCard = _mapper.Map<Card>(cardRequest);
                returnedCard.CreatedAt = _dateTimeProvider.GetCurrentTime();
                returnedCard.Cvv = _cardEncryptionService.GetEncryptedCvv(cardRequest.Cvv);
                return returnedCard;
            }
        }
    }
}