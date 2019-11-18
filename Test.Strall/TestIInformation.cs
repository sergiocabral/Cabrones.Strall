using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIInformation
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IInformation);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IInformationRaw), typeof(ICloneable));
            sut.AssertMyOwnImplementations(typeof(IInformationRaw));
            sut.AssertMyOwnPublicPropertiesCount(6);
            sut.AssertPublicPropertyPresence("InformationType ContentType { get; set; }");
            sut.AssertPublicPropertyPresence("IInformation ContentFrom { get; set; }");
            sut.AssertPublicPropertyPresence("IInformation Parent { get; set; }");
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}