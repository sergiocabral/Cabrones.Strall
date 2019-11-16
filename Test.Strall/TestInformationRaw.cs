using System;
using Cabrones.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Strall
{
    public class TestInformationRaw
    {
        [Theory]
        [InlineData(typeof(InformationRaw), 16)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(InformationRaw), typeof(InformationRaw))]
        public void verifica_se_classe_implementa_os_tipos_necessários(Type tipo, params Type[] tiposQueDeveSerImplementado) =>
            tipo.TestImplementations(tiposQueDeveSerImplementado);

        [Fact]
        public void verificar_se_as_propriedades_foram_inicializadas()
        {
            // Arrange, Given
            // Act, When

            var instância = new InformationRaw();
            
            // Assert, Then

            instância.Id.Should().BeEmpty();
            instância.Description.Should().BeEmpty();
            instância.Content.Should().BeEmpty();
            instância.ContentType.Should().Be(InformationType.Text.ToString());
            instância.ParentId.Should().BeEmpty();
            instância.ParentRelation.Should().BeEmpty();
            instância.CloneFromId.Should().BeEmpty();
            instância.SiblingOrder.Should().Be(0);
        }

        [Fact]
        public void verificar_se_as_propriedades_são_capazes_de_receber_valores()
        {
            // Arrange, Given

            var instânciaDeComparação = Substitute.For<IInformationRaw>();
            var instância = new InformationRaw() as IInformationRaw;
            
            // Act, When
            
            instância.Id = instânciaDeComparação.Id;
            instância.Description = instânciaDeComparação.Description;
            instância.Content = instânciaDeComparação.Content;
            instância.ContentType = instânciaDeComparação.ContentType;
            instância.ParentId = instânciaDeComparação.ParentId;
            instância.ParentRelation = instânciaDeComparação.ParentRelation;
            instância.CloneFromId = instânciaDeComparação.CloneFromId;
            instância.SiblingOrder = instânciaDeComparação.SiblingOrder;
            
            // Assert, Then

            instância.Id.Should().Be(instânciaDeComparação.Id);
            instância.Description.Should().Be(instânciaDeComparação.Description);
            instância.Content.Should().Be(instânciaDeComparação.Content);
            instância.ContentType.Should().Be(instânciaDeComparação.ContentType);
            instância.ParentId.Should().Be(instânciaDeComparação.ParentId);
            instância.ParentRelation.Should().Be(instânciaDeComparação.ParentRelation);
            instância.CloneFromId.Should().Be(instânciaDeComparação.CloneFromId);
            instância.SiblingOrder.Should().Be(instânciaDeComparação.SiblingOrder);
        }

    }
}