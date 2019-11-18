using System;
using System.Runtime.Serialization;
using Cabrones.Test;
using Xunit;

namespace Strall.Exceptions
{
    public class TestStrallDataAccessIsNullException
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(StrallDataAccessIsNullException);

            // Assert, Then

            sut.AssertMyImplementations(typeof(StrallDataAccessException), typeof(StrallException), typeof(Exception), typeof(ISerializable));
            sut.AssertMyOwnImplementations(typeof(StrallDataAccessException));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}