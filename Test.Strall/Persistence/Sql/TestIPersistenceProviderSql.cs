using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.Sql
{
    public class TestIPersistenceProviderSql
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IPersistenceProviderSql<object>);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IPersistenceProvider<object>), typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations(typeof(IPersistenceProvider<object>));
            sut.AssertMyOwnPublicPropertiesCount(3);
            sut.AssertPublicPropertyPresence("DbConnection Connection { get; }");
            sut.AssertPublicPropertyPresence("ISqlNames SqlNames { get; set; }");
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}