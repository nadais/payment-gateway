using AutoMapper;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Models.Cards;

namespace PaymentGateway.Application.Cards
{
    public class CardsMappingProfile : Profile
    {
        public CardsMappingProfile()
        {
            CreateMap<CardDto, Card>();
            CreateMap<CreateCardRequest, CardDto>()
                .ForMember(to => to.Id, how => how.Ignore())
                .ForMember(to => to.CreatedAt, how => how.Ignore())
                .ForMember(to => to.ModifiedAt, how => how.Ignore());
            CreateMap<CreateCardRequest, Card>()
                .ForMember(to => to.Id, how => how.Ignore())
                .ForMember(to => to.CreatedAt, how => how.Ignore())
                .ForMember(to => to.ModifiedAt, how => how.Ignore());
        }
    }
}