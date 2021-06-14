using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        private ConcurrentDictionary<string, Payment> _paymentsBeingProcessed = new();

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
            var preprocessedPayment = await GetAlreadyProcessedPayment(command, cancellationToken);

            if (preprocessedPayment != null)
            {
                return _mapper.Map<PaymentDto>(preprocessedPayment);
            }

            var isPaymentSuccessful = await SendPaymentToBankApi(command, cardRequest);

            var payment = await StorePaymentInDatabaseAsync(command, cardRequest, isPaymentSuccessful, cancellationToken);
            var keyValuePair = _paymentsBeingProcessed.ToArray()
                .Single(x => x.Key == payment.Key);
            _paymentsBeingProcessed.TryRemove(keyValuePair);

            return await _mediator.Send(new GetPaymentByIdQuery(command.ShopperId, payment.Id), cancellationToken);
        }

        private async Task<Payment> GetAlreadyProcessedPayment(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            var (shopperId, createPaymentRequest) = command;
            var uniqueKey = GetUniqueKey(shopperId, createPaymentRequest);
            if (_paymentsBeingProcessed.ContainsKey(uniqueKey))
            {
                return _paymentsBeingProcessed[uniqueKey];
            }
            var finishedPayment = await _appDbContext.Payments.FirstOrDefaultAsync(x => x.Key == uniqueKey, cancellationToken);
            if (finishedPayment != null)
            {
                return finishedPayment;
            }
            var payment = _mapper.Map<Payment>(createPaymentRequest);
            payment.Key = uniqueKey;
            payment.CreatedAt = _dateTimeProvider.GetCurrentTime();
            payment.ShopperId = shopperId;
            payment.Status = PaymentStatus.InProgress;
            _paymentsBeingProcessed.TryAdd(uniqueKey, payment);
            return null;
        }

        private static string GetUniqueKey(Guid shopperId, CreatePaymentRequest createPaymentRequest)
        {
            return $"{shopperId}_{createPaymentRequest.Card.CardNumber}_{createPaymentRequest.SentAt}";
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
            var payment = _paymentsBeingProcessed[GetUniqueKey(request.ShopperId, request.Request)];
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
                payment.Card = null;
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