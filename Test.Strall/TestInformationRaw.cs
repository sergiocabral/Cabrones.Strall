using System;
using Cabrones.Test;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Strall
{
    public class TestInformationRaw
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(InformationRaw);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IInformationRaw), typeof(ICloneable));
            sut.AssertMyOwnImplementations(typeof(IInformationRaw));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }

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

        [Fact]
        public void a_classe_deve_implementar_ICloneable_corretamente()
        {
            // Arrange, Given

            var original = new InformationRaw
            {
                Id = Guid.NewGuid(),
                Description = this.Fixture<string>(),
                Content = this.Fixture<string>(),
                ContentType = this.Fixture<string>(),
                ParentId = Guid.NewGuid(),
                ParentRelation = this.Fixture<string>(),
                CloneFromId = Guid.NewGuid(),
                SiblingOrder = this.Fixture<int>()
            };

            // Act, When

            var clone = (InformationRaw)original.Clone();

            // Assert, Then

            clone.Should().NotBeNull();
            clone.Should().NotBeSameAs(original);
            clone.GetType().Should().Be(original.GetType());
            clone.Id.Should().Be(original.Id);
            clone.Description.Should().Be(original.Description);
            clone.Content.Should().Be(original.Content);
            clone.ContentType.Should().Be(original.ContentType);
            clone.ParentId.Should().Be(original.ParentId);
            clone.ParentRelation.Should().Be(original.ParentRelation);
            clone.CloneFromId.Should().Be(original.CloneFromId);
            clone.SiblingOrder.Should().Be(original.SiblingOrder);
        }
    }
}