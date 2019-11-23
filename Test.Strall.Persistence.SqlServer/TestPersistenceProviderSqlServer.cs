using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Strall.Persistence.Sql;
using Xunit;

namespace Strall.Persistence.SqlServer
{
    
    public class TestPersistenceProviderSqlServer
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(PersistenceProviderSqlServer);

            // Assert, Then

            sut.AssertMyImplementations(typeof(PersistenceProviderSql<ISqlServerConnectionInfo>), typeof(IPersistenceProviderSql), typeof(IPersistenceProvider<ISqlServerConnectionInfo>), typeof(IPersistenceProviderSqlServer), typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations(typeof(IPersistenceProviderSqlServer));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }
        
        [Fact]
        public void não_aceita_fechar_conexão_se_já_estiver_fechada()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProvider<ISqlServerConnectionInfo>;
            
            // Act, When

            Action fecharConexao = () => persistenceProviderSqlServer.Close();
            
            // Assert, Then

            fecharConexao.Should().ThrowExactly<StrallConnectionIsCloseException>();
        }
        
        [Fact]
        public void não_aceita_abrir_conexão_se_já_estiver_aberta()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProvider<ISqlServerConnectionInfo>;
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "temp_" + this.Fixture<string>().Substring(0, 8),
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            
            // Act, When

            Action abrirConexao = () => persistenceProviderSqlServer.Open(connectionInfo);
            
            // Assert, Then

            abrirConexao.Should().NotThrow();
            abrirConexao.Should().ThrowExactly<StrallConnectionIsOpenException>();
        }
        
        [Fact]
        public void após_abertura_da_conexão_o_SqlServer_deve_estar_conectado()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProviderSqlServer;
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "temp_" + this.Fixture<string>().Substring(0, 8),
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            
            // Act, When

            persistenceProviderSqlServer.Open(connectionInfo);
            
            // Assert, Then

            persistenceProviderSqlServer.Connection.Should().NotBeNull();
            persistenceProviderSqlServer.Connection.State.Should().Be(ConnectionState.Open);
        }
        
        [Fact]
        public void após_abertura_da_conexão_isso_deve_ser_sinalizado()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProvider<ISqlServerConnectionInfo>;
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "temp_" + this.Fixture<string>().Substring(0, 8),
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            
            // Act, When

            persistenceProviderSqlServer.Open(connectionInfo);
            
            // Assert, Then

            persistenceProviderSqlServer.Mode.Should().Be(PersistenceProviderMode.Opened);
        }
        
        [Fact]
        public void após_fechamento_da_conexão_isso_deve_ser_sinalizado()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProvider<ISqlServerConnectionInfo>;
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "temp_" + this.Fixture<string>().Substring(0, 8),
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            persistenceProviderSqlServer.Open(connectionInfo);
            
            // Act, When

            persistenceProviderSqlServer.Close();
            
            // Assert, Then

            persistenceProviderSqlServer.Mode.Should().Be(PersistenceProviderMode.Closed);
        }
        
        [Fact]
        public void após_fechamento_da_conexão_o_SqlServer_deve_estar_desconectado()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProviderSqlServer;
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "temp_" + this.Fixture<string>().Substring(0, 8),
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            persistenceProviderSqlServer.Open(connectionInfo);
            
            // Act, When

            persistenceProviderSqlServer.Close();
            
            // Assert, Then

            persistenceProviderSqlServer.Connection.Should().BeNull();
        }
        
        [Fact]
        public void não_deve_ser_possível_criar_estrutura_do_banco_de_dados_se_ainda_não_fez_a_conexão()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProvider<ISqlServerConnectionInfo>;
            
            // Act, When

            Action criarEstrutura = () => persistenceProviderSqlServer.CreateStructure();
            
            // Assert, Then

            criarEstrutura.Should().ThrowExactly<StrallConnectionIsCloseException>();
        }
        
        [Fact]
        public void deve_ser_possível_solicitar_explicitamente_a_criação_da_estrutura_do_banco_de_dados()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer();
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "temp_" + this.Fixture<string>().Substring(0, 8),
                CreateDatabaseIfNotExists = false
            } as ISqlServerConnectionInfo;

            using var conexãoComMaster = new SqlConnection();
            conexãoComMaster.ConnectToMaster();
            conexãoComMaster.CreateDatabase(connectionInfo.Database);
            conexãoComMaster.Close();

            bool EstruturaDoBancoDeDadosExiste(string database)
            {
                using var conexão = new SqlConnection($"Server=.;Database={database};Trusted_Connection=True;");
                try
                {
                    conexão.Open();
                    using var command = conexão.CreateCommand();
                    command.CommandText = $@"
SELECT table_name 
  FROM information_schema.tables
 WHERE table_catalog = '{database}' 
   AND table_name = '{persistenceProviderSqlServer.SqlNames.TableInformation}';
";
                    return command.ExecuteScalar() != null;
                }
                finally
                {
                    conexão.Close();
                }
            }

            persistenceProviderSqlServer.Open(connectionInfo);
            
            // Act, When

            Action criarEstrutura = () => persistenceProviderSqlServer.CreateStructure();
            
            // Assert, Then

            EstruturaDoBancoDeDadosExiste(connectionInfo.Database).Should().BeFalse();
            criarEstrutura.Should().NotThrow();
            EstruturaDoBancoDeDadosExiste(connectionInfo.Database).Should().BeTrue();
        }
        
        [Fact]
        public void se_o_banco_de_dados_for_criado_na_abertura_da_conexão_a_estrutura_também_deve_ser_criada()
        {
            // Arrange, Given

            var database = "temp_" + this.Fixture<string>().Substring(0, 8);
            
            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProviderSqlServer;
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = database,
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            
            // Act, When

            persistenceProviderSqlServer.Open(connectionInfo);
            
            // Assert, Then

            using var command = persistenceProviderSqlServer.Connection.CreateCommand();
            command.CommandText = $@"
SELECT table_name 
  FROM information_schema.tables
 WHERE table_catalog = '{database}' 
   AND table_name = '{persistenceProviderSqlServer.SqlNames.TableInformation}';
";
            var estruturaExiste = command.ExecuteScalar() != null;

            estruturaExiste.Should().BeTrue();
        }

        [Fact]
        public void não_deve_causar_exceção_chamar_Disposable_repetidas_vezes()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProvider<ISqlServerConnectionInfo>;
            
            // Act, When

            Action disposable = () => persistenceProviderSqlServer.Dispose();
            
            // Assert, Then
            
            disposable.Should().NotThrow();
            disposable.Should().NotThrow();
            disposable.Should().NotThrow();
        }

        [Fact]
        public void verifica_se_métodos_que_retornam_uma_auto_referência_está_fazendo_isso()
        {
            // Arrange, Given

            var persistenceProviderSqlServer = new PersistenceProviderSqlServer() as IPersistenceProvider<ISqlServerConnectionInfo>;
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "temp_" + this.Fixture<string>().Substring(0, 8),
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            
            // Act, When

            var retornos = new List<object>
            {
                persistenceProviderSqlServer.Open(connectionInfo),
                persistenceProviderSqlServer.CreateStructure(),
                persistenceProviderSqlServer.Close()
            };

            // Assert, Then

            foreach (var retorno in retornos)
            {
                retorno.Should().BeSameAs(persistenceProviderSqlServer);
            }
        }
    }
}