using Cabrones.Test;
using FluentAssertions;
using Xunit;

namespace Strall.Persistence.Sql
{
    public class TestSqlNames
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(SqlNames);

            // Assert, Then

            sut.AssertMyImplementations(typeof(ISqlNames));
            sut.AssertMyOwnImplementations(typeof(ISqlNames));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
        
        [Fact]
        public void verifica_se_os_valores_das_propriedades_estão_corretas()
        {
            // Arrange, Given

            var instância = new SqlNames() as ISqlNames;
            
            // Act, When
            // Assert, Then

            instância.TableInformation.Should().Be("Information");
            instância.TableInformationColumnId.Should().Be("Id");
            instância.TableInformationColumnDescription.Should().Be("Description");
            instância.TableInformationColumnContent.Should().Be("Content");
            instância.TableInformationColumnContentType.Should().Be("ContentType");
            instância.TableInformationColumnContentFromId.Should().Be("ContentFromId");
            instância.TableInformationColumnParentId.Should().Be("ParentId");
            instância.TableInformationColumnParentRelation.Should().Be("ParentRelation");
            instância.TableInformationColumnSiblingOrder.Should().Be("SiblingOrder");
        }
    }
}