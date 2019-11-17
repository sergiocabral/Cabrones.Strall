using System;
using System.Runtime.Serialization;
using Cabrones.Test;
using Xunit;

namespace Strall.Exceptions
{
    public class TestStrallException
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(StrallException);

            // Assert, Then

            sut.AssertMyImplementations(typeof(Exception), typeof(ISerializable));
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}