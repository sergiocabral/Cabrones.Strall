using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestIConnectionInfo
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IConnectionInfo);

            // Assert, Then

            sut.AssertMyImplementations();
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(5);
            sut.AssertPublicPropertyPresence("String Filename { get; set; }");
            sut.AssertPublicPropertyPresence("Boolean CreateDatabaseIfNotExists { get; set; }");
            sut.AssertPublicPropertyPresence("String ConnectionString { get; }");
            sut.AssertMyOwnPublicMethodsCount(1);
            sut.AssertPublicMethodPresence("Boolean CreateDatabase()");
        }
    }
}