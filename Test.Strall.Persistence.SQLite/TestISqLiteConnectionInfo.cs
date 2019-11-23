using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SqLite
{
    public class TestISqLiteConnectionInfo
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(ISqLiteConnectionInfo);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IConnectionInfo));
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