using System;
using System.Linq;
using AutoMapper;

namespace PaymentGateway.Application.Tests
{
    public class MapperFixture
    {
        public IMapper CreateMapper()
        {
            return GetMapperConfiguration().CreateMapper();
        }

        public static MapperConfiguration GetMapperConfiguration()
        {
            var profiles =
                from t in typeof(DependencyInjection).Assembly.GetTypes()
                where typeof(Profile).IsAssignableFrom(t)
                select (Profile)Activator.CreateInstance(t);

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });
            return config;
        }
    }


}