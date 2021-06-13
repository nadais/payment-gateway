using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Extensions.Authentication;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Application.Payments.Queries;
using PaymentGateway.Models;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize(Policy = ShopperIdPolicy.HasShopperIdPolicy)]
    public class PaymentsController : BaseController
    {
        private readonly IMediator _mediator;
        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        public async Task<PaymentDto> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new CreatePaymentCommand(GetShopperId(), request), cancellationToken);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<PaymentDto> GetPaymentById([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new GetPaymentByIdQuery(GetShopperId(),id), cancellationToken);

        }
        
        [HttpGet]
        public async Task<PageResult<PaymentDto>> GetPayments([FromQuery] GetPaymentsRequest request, CancellationToken cancellationToken = default)
        {
            request = request with {Top = request.Top == 0? 100 : request.Top };
            return await _mediator.Send(new GetPaymentsQuery(GetShopperId(),request), cancellationToken);

        }
    }
}