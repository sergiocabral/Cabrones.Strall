using Cabrones.Test;
using Xunit;

namespace Strall.Persistence
{
    public class TestConnectionInfo
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(ConnectionInfo);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IConnectionInfo));
            sut.AssertMyOwnImplementations(typeof(IConnectionInfo));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}