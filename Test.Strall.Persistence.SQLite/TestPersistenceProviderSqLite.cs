using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Strall.Persistence.Sql;
using Xunit;

namespace Strall.Persistence.SqLite
{
    
    public class TestPersistenceProviderSqLite
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(PersistenceProviderSqLite);

            // Assert, Then

            sut.AssertMyImplementations(typeof(PersistenceProviderSql<ISqLiteConnectionInfo>), typeof(IPersistenceProviderSql), typeof(IPersistenceProvider<ISqLiteConnectionInfo>), typeof(IPersistenceProviderSqLite), typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations(typeof(IPersistenceProviderSqLite));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
        
        [Fact]
        public void não_aceita_fechar_conexão_se_já_estiver_fechada()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            
            // Act, When

            Action fecharConexao = () => persistenceProviderSqLite.Close();
            
            // Assert, Then

            fecharConexao.Should().ThrowExactly<StrallConnectionIsCloseException>();
        }
        
        [Fact]
        public void não_aceita_abrir_conexão_se_já_estiver_aberta()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>()),
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            
            // Act, When

            Action abrirConexao = () => persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then

            abrirConexao.Should().NotThrow();
            abrirConexao.Should().ThrowExactly<StrallConnectionIsOpenException>();
        }
        
        [Fact]
        public void após_abertura_da_conexão_o_sqlite_deve_estar_conectado()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>()),
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            
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

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>()),
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            
            // Act, When

            persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then

            persistenceProviderSqLite.Mode.Should().Be(PersistenceProviderMode.Opened);
        }
        
        [Fact]
        public void após_fechamento_da_conexão_isso_deve_ser_sinalizado()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>()),
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
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

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>()),
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            persistenceProviderSqLite.Open(connectionInfo);
            
            // Act, When

            persistenceProviderSqLite.Close();
            
            // Assert, Then

            persistenceProviderSqLite.Connection.Should().BeNull();
        }
        
        [Fact]
        public void não_deve_ser_possível_criar_estrutura_do_banco_de_dados_se_ainda_não_fez_a_conexão()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            
            // Act, When

            Action criarEstrutura = () => persistenceProviderSqLite.CreateStructure();
            
            // Assert, Then

            criarEstrutura.Should().ThrowExactly<StrallConnectionIsCloseException>();
        }
        
        [Fact]
        public void deve_ser_possível_solicitar_explicitamente_a_criação_da_estrutura_do_banco_de_dados()
        {
            // Arrange, Given

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>());
            File.Create(arquivo).Close();
            
            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = false
            } as ISqLiteConnectionInfo;
            persistenceProviderSqLite.Open(connectionInfo);
            
            // Act, When

            Action criarEstrutura = () => persistenceProviderSqLite.CreateStructure();
            
            // Assert, Then

            new FileInfo(arquivo).Length.Should().Be(0);
            criarEstrutura.Should().NotThrow();
            new FileInfo(arquivo).Length.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public void se_o_banco_de_dados_for_criado_na_abertura_da_conexão_a_estrutura_também_deve_ser_criada()
        {
            // Arrange, Given

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>());
            
            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            
            // Act, When

            persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then

            using var command = persistenceProviderSqLite.Connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master";
            var totalDeTabelas = (long)command.ExecuteScalar();

            totalDeTabelas.Should().BeGreaterThan(0);
            new FileInfo(arquivo).Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void não_deve_causar_exceção_chamar_Disposable_repetidas_vezes()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            
            // Act, When

            Action disposable = () => persistenceProviderSqLite.Dispose();
            
            // Assert, Then
            
            disposable.Should().NotThrow();
            disposable.Should().NotThrow();
            disposable.Should().NotThrow();
        }

        [Fact]
        public void verifica_se_métodos_que_retornam_uma_auto_referência_está_fazendo_isso()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProvider<ISqLiteConnectionInfo>;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>()),
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            
            // Act, When

            var retornos = new List<object>
            {
                persistenceProviderSqLite.Open(connectionInfo),
                persistenceProviderSqLite.CreateStructure(),
                persistenceProviderSqLite.Close()
            };

            // Assert, Then

            foreach (var retorno in retornos)
            {
                retorno.Should().BeSameAs(persistenceProviderSqLite);
            }
        }
    }
}