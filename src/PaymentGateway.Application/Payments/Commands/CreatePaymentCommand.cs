using System;
using System.Collections.Concurrent;
using System.Linq;
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
        private readonly IAppDbContext _appDbContext;
        private readonly IBankService _bankService;
        private readonly IEncryptionService _encryptionService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private ConcurrentDictionary<string, Payment> _paymentsBeingProcessed = new();

        public CreatePaymentCommandHandler(IAppDbContext appDbContext,
            IBankService bankService,
            IDateTimeProvider dateTimeProvider,
            IMapper mapper,
            IMediator mediator, ILogger<CreatePaymentCommandHandler> logger,
            IEncryptionService encryptionService)
        {
            _appDbContext = appDbContext;
            _bankService = bankService;
            _dateTimeProvider = dateTimeProvider;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        public async Task<PaymentDto> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            var preprocessedPayment = await GetAlreadyProcessedPayment(command, cancellationToken);

            if (preprocessedPayment != null)
            {
                return _mapper.Map<PaymentDto>(preprocessedPayment);
            }

            var isPaymentSuccessful = await SendPaymentToBankApiAsync(command);

            var payment = await StorePaymentInDatabaseAsync(command,
                isPaymentSuccessful,
                cancellationToken);

            var keyValuePair = _paymentsBeingProcessed.ToArray()
                .Single(x => x.Key == payment.Key);
            _paymentsBeingProcessed.TryRemove(keyValuePair);

            return await _mediator.Send(
                new GetPaymentByIdQuery(
                    command.ShopperId, payment.Id),
                cancellationToken);
        }

        private async Task<Payment> GetAlreadyProcessedPayment(CreatePaymentCommand command,
            CancellationToken cancellationToken)
        {
            var (shopperId, createPaymentRequest) = command;
            var uniqueKey = GetUniqueKey(shopperId, createPaymentRequest);
            if (_paymentsBeingProcessed.ContainsKey(uniqueKey))
            {
                return _paymentsBeingProcessed[uniqueKey];
            }

            var finishedPayment =
                await _appDbContext.Payments.FirstOrDefaultAsync(x => x.Key == uniqueKey, cancellationToken);
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

        private async Task<BankPaymentResponse> SendPaymentToBankApiAsync(CreatePaymentCommand command)
        {
            try
            {
                var (shopperId, createPaymentRequest) = command;
                var bankPaymentResponse = await _bankService.ProcessCardPaymentAsync(new BankPaymentRequest
                {
                    Currency = createPaymentRequest.Currency,
                    FromCard = createPaymentRequest.Card,
                    Amount = createPaymentRequest.Amount,
                    ToAccountId = shopperId
                });
                return bankPaymentResponse;
            }
            catch (ApiException ex)
            {
                _logger.LogWarning("Error occured sending payment", ex);
                return new BankPaymentResponse
                {
                    IsSuccessful = false
                };
            }
        }

        private async Task<Payment> StorePaymentInDatabaseAsync(CreatePaymentCommand request, 
            BankPaymentResponse response,
            CancellationToken cancellationToken)
        {
            var (shopperId, createPaymentRequest) = request;
            var payment = _paymentsBeingProcessed[GetUniqueKey(shopperId, createPaymentRequest)];
            payment.Status = response.IsSuccessful ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.ExternalId = response.Id;

            await AddCardToPaymentAsync(createPaymentRequest.Card, payment, cancellationToken);

            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return payment;
        }

        private async Task AddCardToPaymentAsync(CardRequest cardRequest, Payment payment, CancellationToken cancellationToken)
        {
            var card = await GetCardToAddAsync(cardRequest, cancellationToken);
            if (card.Id == Guid.Empty)
            {
                payment.Card = card;
            }
            else
            {
                payment.CardId = card.Id;
                payment.Card = null;
            }
        }

        private async Task<Card> GetCardToAddAsync(CardRequest cardRequest, CancellationToken cancellationToken)
        {
            var encryptedCardNumber = _encryptionService.Encrypt(cardRequest.CardNumber);
            var cardId = await _appDbContext.Cards
                .Where(x => x.CardNumber == encryptedCardNumber)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (cardId != Guid.Empty)
            {
                return new Card
                {
                    Id = cardId
                };
            }

            var returnedCard = _mapper.Map<Card>(cardRequest);
            returnedCard.CreatedAt = _dateTimeProvider.GetCurrentTime();
            returnedCard.CardNumber = encryptedCardNumber;
            returnedCard.HolderName = _encryptionService.Encrypt(cardRequest.HolderName);
            return returnedCard;
        }
    }
}