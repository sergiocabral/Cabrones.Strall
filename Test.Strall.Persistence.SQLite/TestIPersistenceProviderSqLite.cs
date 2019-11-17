using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestIPersistenceProviderSqLite
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IPersistenceProviderSqLite);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IPersistenceProvider<IConnectionInfo>), typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(3);
            sut.AssertPublicPropertyPresence("SqliteConnection Connection { get; }");
            sut.AssertPublicPropertyPresence("ISqlNames SqlNames { get; set; }");
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}