﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Application.Payments.Queries;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController
    {
        private readonly IMediator _mediator;
        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        public async Task<PaymentDto> CreatePayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new CreatePaymentCommand(request), cancellationToken);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<PaymentDto> GetPaymentById([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(new GetPaymentByIdQuery(id), cancellationToken);
        }
    }
}