using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments.Queries
{
    public record GetPaymentByIdQuery(Guid Id) : IRequest<PaymentDto>;
    
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
    {
        private readonly IMapper _mapper;
        private readonly IAppDbContext _appDbContext;
        private readonly ICardEncryptionService _cardEncryptionService;

        public GetPaymentByIdQueryHandler(IMapper mapper, IAppDbContext appDbContext, ICardEncryptionService cardEncryptionService)
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
            _cardEncryptionService = cardEncryptionService;
        }

        public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _appDbContext.Payments.AsNoTracking().SingleOrDefaultAsync(
                x => x.Id == request.Id, cancellationToken);

            if (payment == null)
            {
                throw new NotFoundException($"Payment with id {request.Id} was not found");
            }

            payment.CardNumber = _cardEncryptionService.GetMaskedCardNumber(payment.CardNumber);
            return _mapper.Map<PaymentDto>(payment);
        }
    }
}