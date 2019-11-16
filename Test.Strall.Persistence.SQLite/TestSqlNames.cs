using System;
using Cabrones.Test;
using FluentAssertions;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestSqlNames
    {
        [Theory]
        [InlineData(typeof(SqlNames), 9)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(SqlNames), typeof(ISqlNames))]
        public void verifica_se_classe_implementa_os_tipos_necessários(Type tipoDaClasse, params Type[] tiposQueDeveSerImplementado) =>
            tipoDaClasse.TestImplementations(tiposQueDeveSerImplementado);

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
            instância.TableInformationColumnParentId.Should().Be("ParentId");
            instância.TableInformationColumnParentRelation.Should().Be("ParentRelation");
            instância.TableInformationColumnCloneFromId.Should().Be("CloneFromId");
            instância.TableInformationColumnSiblingOrder.Should().Be("SiblingOrder");
        }
    }
}