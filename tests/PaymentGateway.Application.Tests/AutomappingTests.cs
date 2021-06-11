using Xunit;

namespace PaymentGateway.Application.Tests
{
    public class AutoMappingTests
    {
        [Fact]
        public void ValidateAutoMapperProfiles()
        {
            var config = MapperFixture.GetMapperConfiguration();
            config.AssertConfigurationIsValid();
        }
    }

}