using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cabrones.Test;
using FluentAssertions;
using Strall.Persistence.SQLite;
using Xunit;

namespace Strall
{
    public class TestInformation: IDisposable
    {
        /// <summary>
        /// Uma persistência qualquer para fazer os testes de integração
        /// </summary>
        private readonly IDataAccess _persistence;

        /// <summary>
        /// Arquivo do banco de dados para este teste.
        /// </summary>
        private const string Database = "TestInformation";

        /// <summary>
        /// Setup do teste.
        /// </summary>
        public TestInformation()
        {
            var persistence = new PersistenceProviderSqLite();
            persistence.Open(new ConnectionInfo { Filename = Path.Combine(Environment.CurrentDirectory, Database) });
            _persistence = persistence;
        }

        /// <summary>
        /// Liberação de recursos.
        /// </summary>
        public void Dispose() => ((PersistenceProviderSqLite)_persistence)?.Close();

        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(Information);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IInformation), typeof(IInformationRaw), typeof(ICloneable));
            sut.AssertMyOwnImplementations(typeof(IInformation));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
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
                ParentId = Guid.NewGuid(),
                ParentRelation = this.Fixture<string>(),
                CloneFromId = Guid.NewGuid(),
                SiblingOrder = this.Fixture<int>()
            };

            // Act, When

            var clone = (Information)original.Clone();

            // Assert, Then

            clone.Should().NotBeNull();
            clone.GetType().Should().Be(original.GetType());
            clone.Should().NotBeSameAs(original);
            clone.Id.Should().Be(original.Id);
            clone.Description.Should().Be(original.Description);
            clone.Content.Should().Be(original.Content);
            clone.ContentType.Should().Be(original.ContentType);
            clone.ParentId.Should().Be(original.ParentId);
            clone.ParentRelation.Should().Be(original.ParentRelation);
            clone.CloneFromId.Should().Be(original.CloneFromId);
            clone.SiblingOrder.Should().Be(original.SiblingOrder);
        }

        [Fact]
        public void confere_se_as_propriedades_foram_inicializadas_como_em_InformationRaw()
        {
            // Arrange, Given
            
            ((InformationRaw)null).SetDataAccess(_persistence);
            
            var informationRaw = new InformationRaw();

            // Act, When

            var information = new Information();
            
            // Assert, Then

            information.Id.Should().Be(informationRaw.Id);
            information.Description.Should().Be(informationRaw.Description);
            information.Content.Should().Be(informationRaw.Content);
            ((IInformationRaw)information).ContentType.Should().Be(informationRaw.ContentType);
            information.ParentId.Should().Be(informationRaw.ParentId);
            information.ParentRelation.Should().Be(informationRaw.ParentRelation);
            information.CloneFromId.Should().Be(informationRaw.CloneFromId);
            information.SiblingOrder.Should().Be(informationRaw.SiblingOrder);
            
            information.ContentType.ToString().Should().Be(informationRaw.ContentType);
            information.Parent.Should().BeNull();
            information.CloneFrom.Should().BeNull();
        }

        [Theory]
        [InlineData(InformationType.Text)]
        [InlineData(InformationType.Numeric)]
        public void as_propriedades_ContentType_devem_ser_equivalente_entre_as_duas_interfaces(InformationType valor)
        {
            // Arrange, Given
            
            var information1AsIInformation = (IInformation)new Information();
            var information1AsIInformationRaw = (IInformationRaw)information1AsIInformation;

            var information2AsIInformation = (IInformation)new Information();
            var information2AsIInformationRaw = (IInformationRaw)information2AsIInformation;

            // Act, When

            information1AsIInformation.ContentType = valor;
            information2AsIInformationRaw.ContentType = valor.ToString();

            // Assert, Then

            information1AsIInformationRaw.ContentType.Should().Be(information1AsIInformation.ContentType.ToString());
            information2AsIInformation.ContentType.ToString().Should().Be(information2AsIInformationRaw.ContentType);
        }

        [Theory]
        [InlineData("", InformationType.Text)]
        [InlineData("inválido", InformationType.Text)]
        [InlineData("Text", InformationType.Text)]
        [InlineData("Numeric", InformationType.Numeric)]
        public void a_propriedade_ContentType_devem_sempre_ter_um_valor_válido(string valorADefinir, InformationType valorEsperado)
        {
            // Arrange, Given
            
            var informationAsIInformation = (IInformation)new Information();
            var informationAsIInformationRaw = (IInformationRaw)informationAsIInformation;

            // Act, When

            informationAsIInformationRaw.ContentType = valorADefinir;

            // Assert, Then

            informationAsIInformation.ContentType.Should().Be(valorEsperado);
        }
        
        [Fact]
        public void consultar_propriedade_tipo_IInformation_para_Guid_inexistente_deve_retornar_nulo()
        {
            // Arrange, Given

            ((InformationRaw)null).SetDataAccess(_persistence);

            var information = new Information
            {
                ParentId = Guid.NewGuid(), 
                CloneFromId = Guid.NewGuid()
            } as IInformation;

            // Act, When

            var informationParent = information.Parent;
            var informationCloneFrom = information.CloneFrom;

            // Assert, Then

            informationParent.Should().BeNull();
            informationCloneFrom.Should().BeNull();
        }

        [Fact]
        public void consultar_propriedade_tipo_IInformation_para_Guid_existente_deve_retornar_a_instância()
        {
            // Arrange, Given

            ((InformationRaw)null).SetDataAccess(_persistence);

            var informationRaw = new InformationRaw
            {
                Id = Guid.NewGuid(),
                Description = this.Fixture<string>()
            } as IInformationRaw;
            _persistence.Create(informationRaw);

            var information = new Information
            {
                ParentId = informationRaw.Id, 
                CloneFromId = informationRaw.Id
            } as IInformation;

            // Act, When

            var informationParent = information.Parent;
            var informationCloneFrom = information.CloneFrom;

            // Assert, Then

            informationParent.Should().NotBeNull();
            informationParent.Description.Should().Be(informationRaw.Description);
            informationCloneFrom.Should().NotBeNull();
            informationCloneFrom.Description.Should().Be(informationRaw.Description);
        }

        [Fact]
        public void consultar_propriedade_tipo_IInformation_deve_usar_cache()
        {
            // Arrange, Given

            ((InformationRaw)null).SetDataAccess(_persistence);

            var informationRaw = new InformationRaw
            {
                Id = Guid.NewGuid(),
                Description = this.Fixture<string>()
            } as IInformationRaw;
            _persistence.Create(informationRaw);

            var information = new Information
            {
                ParentId = informationRaw.Id, 
                CloneFromId = informationRaw.Id
            } as IInformation;

            // Act, When

            var informationParent1 = information.Parent;
            var informationCloneFrom1 = information.CloneFrom;
            
            informationRaw.Content = this.Fixture<string>();
            _persistence.Update(informationRaw);

            var informationParent2 = information.Parent;
            var informationCloneFrom2 = information.CloneFrom;

            // Assert, Then

            informationParent1.Should().NotBeNull();
            informationParent1.Should().BeSameAs(informationParent2);
            informationParent1.Description.Should().Be(informationRaw.Description);
            informationParent1.Content.Should().NotBe(informationRaw.Content);

            informationCloneFrom1.Should().NotBeNull();
            informationCloneFrom1.Should().BeSameAs(informationCloneFrom2);
            informationCloneFrom1.Description.Should().Be(informationRaw.Description);
            informationCloneFrom1.Content.Should().NotBe(informationRaw.Content);
        }

        [Fact]
        public void consultar_propriedade_tipo_IInformation_deve_usar_cache_mesmo_para_valores_nulos()
        {
            // Arrange, Given

            ((InformationRaw)null).SetDataAccess(_persistence);

            var informationRaw = (IInformationRaw) new InformationRaw
            {
                Id = Guid.NewGuid()
            };

            var information = new Information
            {
                ParentId = informationRaw.Id, 
                CloneFromId = informationRaw.Id
            } as IInformation;

            // Act, When

            var informationParent1 = information.Parent;
            var informationCloneFrom1 = information.CloneFrom;
            
            _persistence.Create(informationRaw);

            var informationParent2 = information.Parent;
            var informationCloneFrom2 = information.CloneFrom;

            // Assert, Then

            informationParent1.Should().BeNull();
            informationParent2.Should().BeNull();

            informationCloneFrom1.Should().BeNull();
            informationCloneFrom2.Should().BeNull();
        }

        [Fact]
        public void ao_definir_propriedade_tipo_IInformation_deve_atualizar_seu_respectivo_id()
        {
            // Arrange, Given

            var information = new Information() as IInformation;
            
            var informationParaAtribuir = new Information
            {
                Id = Guid.NewGuid()
            } as IInformation;

            // Act, When

            information.Parent = informationParaAtribuir;
            information.CloneFrom = informationParaAtribuir;

            // Assert, Then

            information.ParentId.Should().Be(informationParaAtribuir.Id);
            information.CloneFromId.Should().Be(informationParaAtribuir.Id);
        }

        [Fact]
        public void consultar_propriedade_CloneFrom_deve_obter_a_origem_do_clone()
        {
            // Arrange, Given

            ((InformationRaw)null).SetDataAccess(_persistence);

            var informações = new List<IInformation>();
            for (var i = 0; i < 5; i++)
            {
                var informação = new InformationRaw
                {
                    Id = Guid.NewGuid(),
                    CloneFromId = informações.LastOrDefault()?.Id ?? Guid.Empty
                }.Copy(new Information());
                informações.Add(informação);
                _persistence.Create(informação);
            }

            // Act, When

            var informationInicial = informações.First();
            var informaçãoFinal = informações.Last().CloneFrom;

            // Assert, Then

            informaçãoFinal.Should().NotBeNull();
            informaçãoFinal.Id.Should().Be(informationInicial.Id);
        }
    }
}