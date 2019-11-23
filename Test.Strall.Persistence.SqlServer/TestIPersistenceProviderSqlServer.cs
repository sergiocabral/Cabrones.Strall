using System;
using Cabrones.Test;
using Strall.Persistence.Sql;
using Xunit;

namespace Strall.Persistence.SqlServer
{
    public class TestIPersistenceProviderSqlServer
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IPersistenceProviderSqlServer);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IPersistenceProviderSql), typeof(IPersistenceProvider<ISqlServerConnectionInfo>), typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(1);
            sut.AssertPublicPropertyPresence("SqlConnection Connection { get; }");
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}