using System.Security.Cryptography;
using System.Text;
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
    public record CreateCardCommand(CreateCardRequest Request) : IRequest<CardDto>;
    
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, CardDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        public CreateCardCommandHandler(IAppDbContext appDbContext, IMediator mediator, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _appDbContext = appDbContext;
            _mediator = mediator;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }
        public async Task<CardDto> Handle(CreateCardCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ValidateCardQuery(_mapper.Map<CardDto>(command.Request)), cancellationToken);
            var entity = _mapper.Map<Card>(command.Request);

            entity.Cvv = GetEncryptedCvv(command.Request.Cvv);
            entity.CreatedAt = _dateTimeProvider.GetCurrentTime();
            
            await _appDbContext.Cards.AddAsync(entity, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<CardDto>(entity);
        }

        private string GetEncryptedCvv(int cvv)
        {
            var bytes = Encoding.UTF8.GetBytes(cvv.ToString());
            var hashString = new SHA256Managed();
            var hash = hashString.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var x in hash)
            {
                sb.Append($"{x:x2}");
            }
            return sb.ToString();
            
        }
    }
}