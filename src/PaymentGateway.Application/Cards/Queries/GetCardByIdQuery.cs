using System;
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

    public record GetCardByIdQuery(Guid Id, Guid ShopperId) : IRequest<CardDto>;
    
    public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, CardDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly ICardEncryptionService _cardEncryptionService;
        private readonly IMapper _mapper;

        public GetCardByIdQueryHandler(IAppDbContext appDbContext, ICardEncryptionService cardEncryptionService, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _cardEncryptionService = cardEncryptionService;
            _mapper = mapper;
        }

        public async Task<CardDto> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _appDbContext.Cards.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.Id && x.ShopperId == request.ShopperId, cancellationToken);
            if (card == null)
            {
                throw new NotFoundException($"No card found with id {request.Id}");
            }

            card.CardNumber = _cardEncryptionService.GetMaskedCardNumber(card.CardNumber);

            return _mapper.Map<CardDto>(card);
        }
    }
}