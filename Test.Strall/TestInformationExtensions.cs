using System;
using System.IO;
using System.Threading;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Strall.Persistence.SQLite;
using Xunit;

namespace Strall
{
    public class TestInformationExtensions: IDisposable
    {
        /// <summary>
        /// Uma persistência qualquer para fazer os testes de integração
        /// </summary>
        private readonly IDataAccess _persistence;

        /// <summary>
        /// Arquivo do banco de dados para este teste.
        /// </summary>
        private const string Database = "TestInformationExtensions";

        /// <summary>
        /// Setup do teste.
        /// </summary>
        public TestInformationExtensions()
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

            var sut = typeof(InformationExtensions);

            // Assert, Then

            sut.AssertMyImplementations();
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(14);
            sut.AssertPublicMethodPresence("TInformacao Copy<TInformacao>(IInformationRaw, TInformacao)");
            sut.AssertPublicMethodPresence("IInformationRaw Copy(IInformationRaw)");
            sut.AssertPublicMethodPresence("IInformationRaw SetDataAccess(IInformationRaw, IDataAccess)");
            sut.AssertPublicMethodPresence("IDataAccess GetDataAccess(IInformationRaw)");
            sut.AssertPublicMethodPresence("Boolean Exists(IInformationRaw)");
            sut.AssertPublicMethodPresence("IInformationRaw Get(IInformationRaw)");
            sut.AssertPublicMethodPresence("IInformationRaw Create(IInformationRaw)");
            sut.AssertPublicMethodPresence("Boolean Update(IInformationRaw)");
            sut.AssertPublicMethodPresence("Boolean Delete(IInformationRaw)");
            sut.AssertPublicMethodPresence("Boolean HasContentTo(IInformationRaw)");
            sut.AssertPublicMethodPresence("IEnumerable<Guid> ContentTo(IInformationRaw)");
            sut.AssertPublicMethodPresence("Guid ContentFrom(IInformationRaw)");
            sut.AssertPublicMethodPresence("Boolean HasChildren(IInformationRaw)");
            sut.AssertPublicMethodPresence("IEnumerable<Guid> Children(IInformationRaw)");
        }
        
        [Fact]
        public void deve_existir_um_IDataAccess_padrão_disponível()
        {
            // Arrange, Given
            
            var informationRaw = new InformationRaw();
            
            // Act, When

            informationRaw.SetDataAccess(_persistence);
            var persistênciaDefinida = informationRaw.GetDataAccess(); 
            
            // Assert, Then
            
            persistênciaDefinida.Should().BeSameAs(_persistence);
        }
        
        [Fact]
        public void o_método_que_define_o_IDataAccess_deve_retornar_a_instância_de_entrada()
        {
            // Arrange, Given
            
            var informationRawNula = (InformationRaw)null;
            var informationRaw = new InformationRaw();
            
            // Act, When

            // ReSharper disable once ExpressionIsAlwaysNull
            var retornoParaInformationRawNula = informationRawNula.SetDataAccess(_persistence);
            var retornoParaInformationRaw = informationRaw.SetDataAccess(_persistence);
            
            // Assert, Then
            
            // ReSharper disable once ExpressionIsAlwaysNull
            retornoParaInformationRawNula.Should().BeSameAs(informationRawNula);
            retornoParaInformationRaw.Should().BeSameAs(informationRaw);
        }

        [Fact]
        public void usar_o_IDataAccess_padrão_quando_não_foi_definido_deve_disparar_exception()
        {
            // Arrange, Given
            
            // Como a manipulação é em uma propriedade estática um tempo de espera
            // será aplicado para evitar conflitos com outros testes.
            Thread.Sleep(3000);
            
            var informationRaw = new InformationRaw();
            informationRaw.SetDataAccess(null);

            // Act, When

            Func<IDataAccess> lerDataAccessDefault = () => informationRaw.GetDataAccess();
            
            // Assert, Then

            lerDataAccessDefault.Should().ThrowExactly<StrallDataAccessIsNullException>();
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
                ContentFromId = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                ParentRelation = this.Fixture<string>(),
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
            novaCópia.ContentFromId.Should().Be(informationRaw.ContentFromId);
            novaCópia.ParentId.Should().Be(informationRaw.ParentId);
            novaCópia.ParentRelation.Should().Be(informationRaw.ParentRelation);
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
                    ContentFromId = Guid.NewGuid(),
                    ParentId = Guid.NewGuid(),
                    ParentRelation = this.Fixture<string>(),
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
            cópia.ContentFromId.Should().Be(origem.ContentFromId);
            cópia.ParentId.Should().Be(origem.ParentId);
            cópia.ParentRelation.Should().Be(origem.ParentRelation);
            cópia.SiblingOrder.Should().Be(origem.SiblingOrder);
        }
    }
}