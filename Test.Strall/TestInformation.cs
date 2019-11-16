using System;
using Cabrones.Test;
using FluentAssertions;
using Xunit;

namespace Strall
{
    public class TestInformation
    {
        [Theory]
        [InlineData(typeof(Information), 16)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(Information), typeof(Information))]
        public void verifica_se_classe_implementa_os_tipos_necessários(Type tipo, params Type[] tiposQueDeveSerImplementado) =>
            tipo.TestImplementations(tiposQueDeveSerImplementado);

        [Fact]
        public void verificar_se_as_propriedades_foram_inicializadas()
        {
            // Arrange, Given
            // Act, When

            var instância = new Information();
            
            // Assert, Then

            instância.Id.Should().BeEmpty();
            instância.Description.Should().BeEmpty();
            instância.Content.Should().BeEmpty();
            instância.ContentType.Should().BeEmpty();
            instância.ParentId.Should().BeEmpty();
            instância.ParentRelation.Should().BeEmpty();
            instância.CloneId.Should().BeEmpty();
            instância.SiblingOrder.Should().Be(0);
        }

    }
}