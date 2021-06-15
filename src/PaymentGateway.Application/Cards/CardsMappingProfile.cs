using AutoMapper;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards
{
    public class CardsMappingProfile : Profile
    {
        public CardsMappingProfile()
        {
            CreateMap<Card, CardDto>();
            CreateMap<CardRequest, Card>()
                .ForMember(to => to.Id, how=> how.Ignore())
                .ForMember(to => to.CreatedAt, how=> how.Ignore())
                .ReverseMap()
                .ForMember(to => to.Cvv, how => how.Ignore());
        }
    }
}