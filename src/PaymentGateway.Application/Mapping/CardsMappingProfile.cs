using AutoMapper;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Models.Dtos.Cards;

namespace PaymentGateway.Application.Mapping
{
    public class CardsMappingProfile : Profile
    {
        public CardsMappingProfile()
        {
            CreateMap<CardDto, Card>();
        }
    }
}