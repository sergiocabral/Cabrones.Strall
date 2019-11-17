using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestISqlNames
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(ISqlNames);

            // Assert, Then

            sut.AssertMyImplementations();
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(9);
            sut.AssertPublicPropertyPresence("String TableInformation { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnId { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnDescription { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnContent { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnContentType { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnParentId { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnParentRelation { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnCloneFromId { get; }");
            sut.AssertPublicPropertyPresence("String TableInformationColumnSiblingOrder { get; }");
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}