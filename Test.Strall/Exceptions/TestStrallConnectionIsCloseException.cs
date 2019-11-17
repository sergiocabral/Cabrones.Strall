using System;
using System.Runtime.Serialization;
using Cabrones.Test;
using Xunit;

namespace Strall.Exceptions
{
    public class TestStrallConnectionIsCloseException
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(StrallConnectionIsCloseException);

            // Assert, Then

            sut.AssertMyImplementations(typeof(StrallConnectionException), typeof(StrallException), typeof(Exception), typeof(ISerializable));
            sut.AssertMyOwnImplementations(typeof(StrallConnectionException));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}