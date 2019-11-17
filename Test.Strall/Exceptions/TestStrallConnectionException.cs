using System;
using System.Runtime.Serialization;
using Cabrones.Test;
using Xunit;

namespace Strall.Exceptions
{
    public class TestStrallConnectionException
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(StrallConnectionException);

            // Assert, Then

            sut.AssertMyImplementations(typeof(StrallException), typeof(Exception), typeof(ISerializable));
            sut.AssertMyOwnImplementations(typeof(StrallException));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}