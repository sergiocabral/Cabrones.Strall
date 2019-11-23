using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SqlServer
{
    public class TestIConnectionInfo
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(ISqlServerConnectionInfo);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IConnectionInfo));
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(5);
            sut.AssertPublicPropertyPresence("String Database { get; set; }");
            sut.AssertPublicPropertyPresence("Boolean CreateDatabaseIfNotExists { get; set; }");
            sut.AssertPublicPropertyPresence("String ConnectionString { get; }");
            sut.AssertMyOwnPublicMethodsCount(1);
            sut.AssertPublicMethodPresence("Boolean CreateDatabase()");
        }
    }
}