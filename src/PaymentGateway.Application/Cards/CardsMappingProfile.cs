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
            CreateMap<CreateCardRequest, CardDto>();
            CreateMap<CreateCardRequest, Card>();
        }
    }
}