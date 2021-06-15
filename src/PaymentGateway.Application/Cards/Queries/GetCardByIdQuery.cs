using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards.Queries
{
    public record GetCardByIdQuery(Guid Id) : IRequest<CardDto>;

    public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;

        public GetCardByIdQueryHandler(IAppDbContext appDbContext, IMapper mapper, IEncryptionService encryptionService)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _encryptionService = encryptionService;
        }

        public async Task<CardDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _appDbContext.Cards.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (card == null)
            {
                throw new NotFoundException($"No card found with id {request.Id}");
            }

            var decryptedCardNumber = _encryptionService.Decrypt(card.CardNumber);
            var decryptedHolderName = _encryptionService.Decrypt(card.HolderName);

            card.HolderName = MaskCardHolderName(decryptedHolderName);
            card.CardNumber = MaskCardNumber(decryptedCardNumber);

            return _mapper.Map<CardDto>(card);
        }

        private static string MaskCardHolderName(string cardHolderName)
        {
            var sb = new StringBuilder();
            sb.Append("**** ");
            var lastName = cardHolderName.Split(" ");

            sb.Append(lastName[^1]);
            return sb.ToString();
        }

        private static string MaskCardNumber(string cardNumber)
        {
            var sb = new StringBuilder();
            var cardType = GetCreditCardType(cardNumber);
            sb.Append(cardType)
                .Append(" ****-");
            for (var i = cardNumber.Length - 4; i < cardNumber.Length; i++)
            {
                sb.Append(cardNumber[i]);
            }

            return sb.ToString();
        }

        private static string GetCreditCardType(string cardNumber)
        {
            var firstDigit = cardNumber[0];
            return firstDigit switch
            {
                '3' => "American express",
                '4' => "VISA",
                '5' => "Mastercard",
                '6' => "Discover",
                _ => "Unknown"
            };
        }
    }
}