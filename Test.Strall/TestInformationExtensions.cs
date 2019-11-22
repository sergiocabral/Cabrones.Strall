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
            sut.AssertMyOwnPublicMethodsCount(17);
            sut.AssertPublicMethodPresence("TInformacao CopyTo<TInformacao>(IInformation, TInformacao)");
            sut.AssertPublicMethodPresence("IInformation CopyTo(IInformation)");
            sut.AssertPublicMethodPresence("IInformation SetDataAccess(IInformation, IDataAccess)");
            sut.AssertPublicMethodPresence("IDataAccess GetDataAccess(IInformation)");
            sut.AssertPublicMethodPresence("Boolean Exists(IInformation)");
            sut.AssertPublicMethodPresence("IInformation GetInformation(Guid)");
            sut.AssertPublicMethodPresence("IInformation Get(IInformation)");
            sut.AssertPublicMethodPresence("IInformation Create(IInformation)");
            sut.AssertPublicMethodPresence("Boolean Update(IInformation)");
            sut.AssertPublicMethodPresence("IInformation UpdateOrCreate(IInformation)");
            sut.AssertPublicMethodPresence("Int32 Delete(IInformation, Boolean = 'False')");
            sut.AssertPublicMethodPresence("Boolean HasContentTo(IInformation)");
            sut.AssertPublicMethodPresence("IEnumerable<Guid> ContentTo(IInformation)");
            sut.AssertPublicMethodPresence("Guid ContentFrom(IInformation)");
            sut.AssertPublicMethodPresence("Boolean HasChildren(IInformation)");
            sut.AssertPublicMethodPresence("IEnumerable<Guid> Children(IInformation)");
        }
        
        [Fact]
        public void deve_existir_um_IDataAccess_padrão_disponível()
        {
            // Arrange, Given
            
            var information = new Information();
            
            // Act, When

            information.SetDataAccess(_persistence);
            var persistênciaDefinida = information.GetDataAccess(); 
            
            // Assert, Then
            
            persistênciaDefinida.Should().BeSameAs(_persistence);
        }
        
        [Fact]
        public void o_método_que_define_o_IDataAccess_deve_retornar_a_instância_de_entrada()
        {
            // Arrange, Given
            
            var informationNula = (Information)null;
            var information = new Information();
            
            // Act, When

            // ReSharper disable once ExpressionIsAlwaysNull
            var retornoParaInformationNula = informationNula.SetDataAccess(_persistence);
            var retornoParaInformation = information.SetDataAccess(_persistence);
            
            // Assert, Then
            
            // ReSharper disable once ExpressionIsAlwaysNull
            retornoParaInformationNula.Should().BeSameAs(informationNula);
            retornoParaInformation.Should().BeSameAs(information);
        }

        [Fact]
        public void usar_o_IDataAccess_padrão_quando_não_foi_definido_deve_disparar_exception()
        {
            // Arrange, Given
            
            // Como a manipulação é em uma propriedade estática um tempo de espera
            // será aplicado para evitar conflitos com outros testes.
            Thread.Sleep(3000);
            
            var information = new Information();
            information.SetDataAccess(null);

            // Act, When

            Func<IDataAccess> lerDataAccessDefault = () => information.GetDataAccess();
            
            // Assert, Then

            lerDataAccessDefault.Should().ThrowExactly<StrallDataAccessIsNullException>();
        }

        [Fact]
        public void ao_copiar_sem_passar_parâmetro_uma_nova_instância_deve_ser_criada_com_os_mesmos_valores()
        {
            // Arrange, Given

            var information = new Information
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

            var novaCópia = information.CopyTo();

            // Assert, Then

            novaCópia.Should().NotBeNull();
            novaCópia.Should().NotBeSameAs(information);
            novaCópia.Id.Should().Be(information.Id);
            novaCópia.Description.Should().Be(information.Description);
            novaCópia.Content.Should().Be(information.Content);
            novaCópia.ContentType.Should().Be(information.ContentType);
            novaCópia.ContentFromId.Should().Be(information.ContentFromId);
            novaCópia.ParentId.Should().Be(information.ParentId);
            novaCópia.ParentRelation.Should().Be(information.ParentRelation);
            novaCópia.SiblingOrder.Should().Be(information.SiblingOrder);
        }

        [Fact]
        public void ao_copiar_sem_passar_parâmetro_a_nova_instância_deve_ser_do_tipo_da_instância_de_origem()
        {
            // Arrange, Given

            var origem = new Information();

            // Act, When

            var cópia = origem.CopyTo();

            // Assert, Then

            cópia.Should().NotBeNull();
            cópia.Should().NotBeSameAs(origem);
            cópia.GetType().Should().Be(origem.GetType());
        }

        [Fact]
        public void ao_copiar_passando_parâmetro_este_deve_receber_os_mesmos_valores_e_ser_retornado()
        {
            // Arrange, Given

            Information Create() =>
                new Information
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
            
            var origem = Create();
            var destino = Create();

            // Act, When

            var cópia = origem.CopyTo(destino);

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