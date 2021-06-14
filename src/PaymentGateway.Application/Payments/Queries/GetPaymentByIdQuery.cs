using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Application.Cards.Queries;
using PaymentGateway.Application.Common.Abstractions;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments.Queries
{
    public record GetPaymentByIdQuery(Guid ShopperId, Guid Id) : IRequest<PaymentDto>;
    
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetPaymentByIdQueryHandler(IMapper mapper, IAppDbContext appDbContext, IMediator mediator)
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
            _mediator = mediator;
        }

        public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var (shopperId, paymentId) = request;
            var payment = await _appDbContext.Payments.AsNoTracking().SingleOrDefaultAsync(
                x => x.Id == paymentId && x.ShopperId == shopperId, cancellationToken);

            if (payment == null)
            {
                throw new NotFoundException($"Payment with id {paymentId} was not found");
            }

            var card = await _mediator.Send(new GetCardByIdQuery(payment.CardId), cancellationToken);
            return _mapper.Map<PaymentDto>(payment) with {Card = card};
        }
    }
}