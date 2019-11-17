using Cabrones.Test;
using FluentAssertions;
using Xunit;

namespace Strall
{
    public class TestPersistenceProviderMode
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(PersistenceProviderMode);

            // Assert, Then

            sut.AssertEnumValuesCount(2);
            PersistenceProviderMode.Closed.Should().Be(0b_0010);
            PersistenceProviderMode.Opened.Should().Be(0b_0100);
        }
    }
}