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

    public record GetCardByNumberQuery(string CardNumber) : IRequest<CardDto>;
    
    public class GetCardByNumberQueryHandler : IRequestHandler<GetCardByNumberQuery, CardDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly ICardEncryptionService _cardEncryptionService;
        private readonly IMapper _mapper;

        public GetCardByNumberQueryHandler(IAppDbContext appDbContext, ICardEncryptionService cardEncryptionService, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _cardEncryptionService = cardEncryptionService;
            _mapper = mapper;
        }

        public async Task<CardDto> Handle(GetCardByNumberQuery request, CancellationToken cancellationToken)
        {
            var card = await _appDbContext.Cards.AsNoTracking().SingleOrDefaultAsync(x => x.CardNumber == request.CardNumber, cancellationToken);
            if (card == null)
            {
                throw new NotFoundException($"No card found with number {request.CardNumber}");
            }

            card.CardNumber = _cardEncryptionService.GetMaskedCardNumber(card.CardNumber);

            return _mapper.Map<CardDto>(card);
        }
    }
}