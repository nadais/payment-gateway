using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PaymentGateway.Application.Cards.Queries;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards.Commands
{
    public record CreateCardCommand(Guid ShopperId, CardRequest Request) : IRequest<CardDto>;
    
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, CardDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICardEncryptionService _cardEncryptionService;
        public CreateCardCommandHandler(IAppDbContext appDbContext, IMediator mediator, IMapper mapper, IDateTimeProvider dateTimeProvider, ICardEncryptionService cardEncryptionService)
        {
            _appDbContext = appDbContext;
            _mediator = mediator;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _cardEncryptionService = cardEncryptionService;
        }
        public async Task<CardDto> Handle(CreateCardCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ValidateCardQuery(command.Request), cancellationToken);


            var entity = _mapper.Map<Card>(command.Request);

            entity.Cvv = _cardEncryptionService.GetEncryptedCvv(command.Request.Cvv);
            entity.CreatedAt = _dateTimeProvider.GetCurrentTime();
            entity.ShopperId = command.ShopperId;
            
            await _appDbContext.Cards.AddAsync(entity, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return await _mediator.Send(new GetCardByIdQuery(entity.Id, command.ShopperId), cancellationToken);
        }
    }
}