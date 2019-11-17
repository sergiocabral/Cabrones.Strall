using System;
using Cabrones.Test;
using FluentAssertions;
using Xunit;

namespace Strall
{
    public class TestInformationExtensions
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(InformationExtensions);

            // Assert, Then

            sut.AssertMyImplementations();
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(2);
            sut.AssertPublicMethodPresence("TInformacao Copy<TInformacao>(IInformationRaw, TInformacao)");
            sut.AssertPublicMethodPresence("IInformationRaw Copy(IInformationRaw)");
        }

        [Fact]
        public void ao_copiar_sem_passar_parâmetro_uma_nova_instância_deve_ser_criada_com_os_mesmos_valores()
        {
            // Arrange, Given

            var informationRaw = new InformationRaw
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

            var novaCópia = informationRaw.Copy();

            // Assert, Then

            novaCópia.Should().NotBeNull();
            novaCópia.Should().NotBeSameAs(informationRaw);
            novaCópia.Id.Should().Be(informationRaw.Id);
            novaCópia.Description.Should().Be(informationRaw.Description);
            novaCópia.Content.Should().Be(informationRaw.Content);
            novaCópia.ContentType.Should().Be(informationRaw.ContentType);
            novaCópia.ParentId.Should().Be(informationRaw.ParentId);
            novaCópia.ParentRelation.Should().Be(informationRaw.ParentRelation);
            novaCópia.CloneFromId.Should().Be(informationRaw.CloneFromId);
            novaCópia.SiblingOrder.Should().Be(informationRaw.SiblingOrder);
        }

        [Fact]
        public void ao_copiar_sem_passar_parâmetro_a_nova_instância_deve_ser_do_tipo_da_instância_de_origem()
        {
            // Arrange, Given

            var origem = new Information();

            // Act, When

            var cópia = origem.Copy();

            // Assert, Then

            cópia.Should().NotBeNull();
            cópia.Should().NotBeSameAs(origem);
            cópia.GetType().Should().Be(origem.GetType());
            cópia.GetType().Should().NotBe(typeof(InformationRaw));
        }

        [Fact]
        public void ao_copiar_passando_parâmetro_este_deve_receber_os_mesmos_valores_e_ser_retornado()
        {
            // Arrange, Given

            InformationRaw Create() =>
                new InformationRaw
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
            
            var origem = Create();
            var destino = Create();

            // Act, When

            var cópia = origem.Copy(destino);

            // Assert, Then

            cópia.Should().NotBeNull();
            cópia.Should().BeSameAs(destino);
            cópia.Id.Should().Be(origem.Id);
            cópia.Description.Should().Be(origem.Description);
            cópia.Content.Should().Be(origem.Content);
            cópia.ContentType.Should().Be(origem.ContentType);
            cópia.ParentId.Should().Be(origem.ParentId);
            cópia.ParentRelation.Should().Be(origem.ParentRelation);
            cópia.CloneFromId.Should().Be(origem.CloneFromId);
            cópia.SiblingOrder.Should().Be(origem.SiblingOrder);
        }
    }
}