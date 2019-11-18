using System;
using Cabrones.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Strall
{
    public class TestInformation
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(Information);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IInformation), typeof(ICloneable));
            sut.AssertMyOwnImplementations(typeof(IInformation));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }

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
            instância.ContentType.Should().Be(InformationType.Text);
            instância.ContentFromId.Should().BeEmpty();
            instância.ParentId.Should().BeEmpty();
            instância.ParentRelation.Should().BeEmpty();
            instância.SiblingOrder.Should().Be(0);
        }

        [Fact]
        public void verificar_se_as_propriedades_são_capazes_de_receber_valores()
        {
            // Arrange, Given

            var instânciaDeComparação = Substitute.For<IInformation>();
            var instância = new Information() as IInformation;
            
            // Act, When
            
            instância.Id = instânciaDeComparação.Id;
            instância.Description = instânciaDeComparação.Description;
            instância.Content = instânciaDeComparação.Content;
            instância.ContentType = instânciaDeComparação.ContentType;
            instância.ContentFromId = instânciaDeComparação.ContentFromId;
            instância.ParentId = instânciaDeComparação.ParentId;
            instância.ParentRelation = instânciaDeComparação.ParentRelation;
            instância.SiblingOrder = instânciaDeComparação.SiblingOrder;
            
            // Assert, Then

            instância.Id.Should().Be(instânciaDeComparação.Id);
            instância.Description.Should().Be(instânciaDeComparação.Description);
            instância.Content.Should().Be(instânciaDeComparação.Content);
            instância.ContentType.Should().Be(instânciaDeComparação.ContentType);
            instância.ContentFromId.Should().Be(instânciaDeComparação.ContentFromId);
            instância.ParentId.Should().Be(instânciaDeComparação.ParentId);
            instância.ParentRelation.Should().Be(instânciaDeComparação.ParentRelation);
            instância.SiblingOrder.Should().Be(instânciaDeComparação.SiblingOrder);
        }

        [Fact]
        public void a_classe_deve_implementar_ICloneable_corretamente()
        {
            // Arrange, Given

            var original = new Information
            {
                Id = Guid.NewGuid(),
                Description = this.Fixture<string>(),
                Content = this.Fixture<string>(),
                ContentType = InformationType.Numeric,
                ContentFromId = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                ParentRelation = this.Fixture<string>(),
                SiblingOrder = this.Fixture<int>()
            };

            // Act, When

            var clone = (Information)original.Clone();

            // Assert, Then

            clone.Should().NotBeNull();
            clone.Should().NotBeSameAs(original);
            clone.GetType().Should().Be(original.GetType());
            clone.Id.Should().Be(original.Id);
            clone.Description.Should().Be(original.Description);
            clone.Content.Should().Be(original.Content);
            clone.ContentType.Should().Be(original.ContentType);
            clone.ContentFromId.Should().Be(original.ContentFromId);
            clone.ParentId.Should().Be(original.ParentId);
            clone.ParentRelation.Should().Be(original.ParentRelation);
            clone.SiblingOrder.Should().Be(original.SiblingOrder);
        }

        [Theory]
        [InlineData(0, InformationType.Text)]
        [InlineData(1, InformationType.Text)]
        [InlineData((int)InformationType.Numeric, InformationType.Numeric)]
        [InlineData((int)InformationType.Text, InformationType.Text)]
        public void a_propriedade_ContentType_sempre_deve_ter_um_valor_válido(int valor, InformationType valorEsperado)
        {
            // Arrange, Given

            // ReSharper disable once UseObjectOrCollectionInitializer
            var informação = new Information();

            // Act, When

            informação.ContentType = (InformationType)valor;
            
            // Assert, Then

            informação.ContentType.Should().Be(valorEsperado);
        }
    }
}