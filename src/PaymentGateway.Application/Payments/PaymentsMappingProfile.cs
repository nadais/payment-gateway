using AutoMapper;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Models.Payments;

namespace PaymentGateway.Application.Payments
{
    public class PaymentsMappingProfile : Profile
    {
        public PaymentsMappingProfile()
        {
            CreateMap<Payment, PaymentDto>();
            CreateMap<CreatePaymentRequest, Payment>()
                .ForMember(to => to.CardId, how => how.Ignore())
                .ForMember(to => to.ShopperId, how => how.Ignore())
                .ForMember(to => to.Id, how=> how.Ignore())
                .ForMember(to => to.CreatedAt, how=> how.Ignore())
                .ForMember(to => to.Status, how => how.Ignore())
                .ForMember(to => to.ExternalId, how => how.Ignore())
                .ForMember(to => to.Key, how => how.Ignore());
        }
    }
}