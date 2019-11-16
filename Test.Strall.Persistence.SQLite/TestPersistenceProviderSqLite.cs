using System;
using System.Data;
using System.IO;
using AutoFixture;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestPersistenceProviderSqLite
    {
        [Theory]
        [InlineData(typeof(PersistenceProviderSqLite), 12)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(PersistenceProviderSqLite), typeof(IPersistenceProviderSqLite), typeof(IPersistenceProvider<IConnectionInfo>), typeof(IDisposable))]
        public void verifica_se_classe_implementa_os_tipos_necessários(Type tipoDaClasse, params Type[] tiposQueDeveSerImplementado) =>
            tipoDaClasse.TestImplementations(tiposQueDeveSerImplementado);
        
        [Fact]
        public void não_aceita_fechar_conexão_se_já_estiver_fechada()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite();
            
            // Act, When

            Action fecharConexao = () => persistenceProviderSqLite.Close();
            
            // Assert, Then

            fecharConexao.Should().ThrowExactly<StrallConnectionIsAlreadyCloseException>();
        }
        
        [Fact]
        public void não_aceita_abrir_conexão_se_já_estiver_aberta()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite();
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            };
            
            // Act, When

            Action abrirConexao = () => persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then

            abrirConexao.Should().NotThrow();
            abrirConexao.Should().ThrowExactly<StrallConnectionIsAlreadyOpenException>();
        }
        
        [Fact]
        public void após_abertura_da_conexão_o_sqlite_deve_estar_conectado()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite();
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            };
            
            // Act, When

            persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then

            persistenceProviderSqLite.Connection.Should().NotBeNull();
            persistenceProviderSqLite.Connection.State.Should().Be(ConnectionState.Open);
        }
        
        [Fact]
        public void após_abertura_da_conexão_isso_deve_ser_sinalizado()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite();
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            };
            
            // Act, When

            persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then

            persistenceProviderSqLite.Mode.Should().Be(PersistenceProviderMode.Opened);
        }
        
        [Fact]
        public void após_fechamento_da_conexão_isso_deve_ser_sinalizado()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite();
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            };
            persistenceProviderSqLite.Open(connectionInfo);
            
            // Act, When

            persistenceProviderSqLite.Close();
            
            // Assert, Then

            persistenceProviderSqLite.Mode.Should().Be(PersistenceProviderMode.Closed);
        }
        
        [Fact]
        public void após_fechamento_da_conexão_o_sqlite_deve_estar_desconectado()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite();
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            };
            persistenceProviderSqLite.Open(connectionInfo);
            
            // Act, When

            persistenceProviderSqLite.Close();
            
            // Assert, Then

            persistenceProviderSqLite.Connection.Should().BeNull();
        }
    }
}