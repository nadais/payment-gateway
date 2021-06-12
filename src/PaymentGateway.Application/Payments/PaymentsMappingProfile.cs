using AutoMapper;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Models.Cards;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments
{
    public class PaymentsMappingProfile : Profile
    {
        public PaymentsMappingProfile()
        {
            CreateMap<Payment, PaymentDto>();
            CreateMap<CreatePaymentRequest, Payment>()
                .ForMember(to => to.Id, how=> how.Ignore())
                .ForMember(to => to.CreatedAt, how=> how.Ignore())
                .ForMember(to => to.CardNumber, how => how.Ignore())
                .ForMember(to => to.Status, how => how.Ignore());
        }
    }
}