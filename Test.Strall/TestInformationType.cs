using Cabrones.Test;
using FluentAssertions;
using Xunit;

namespace Strall
{
    public class TestInformationType
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(InformationType);

            // Assert, Then

            sut.AssertEnumValuesCount(2);
            InformationType.Text.Should().Be(0b_0010);
            InformationType.Numeric.Should().Be(0b_0100);
        }
    }
}