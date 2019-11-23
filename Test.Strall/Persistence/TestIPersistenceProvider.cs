using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Persistence
{
    public class TestIPersistenceProvider
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IPersistenceProvider<ConnectionInfo>);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations(typeof(IDataAccess));
            sut.AssertMyOwnPublicPropertiesCount(1);
            sut.AssertPublicPropertyPresence("PersistenceProviderMode Mode { get; }");
            sut.AssertMyOwnPublicMethodsCount(3);
            sut.AssertPublicMethodPresence("IPersistenceProvider<ConnectionInfo> CreateStructure()");
            sut.AssertPublicMethodPresence("IPersistenceProvider<ConnectionInfo> Open(ConnectionInfo)");
            sut.AssertPublicMethodPresence("IPersistenceProvider<ConnectionInfo> Close()");
        }
    }
}