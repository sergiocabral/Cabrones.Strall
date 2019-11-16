using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using AutoFixture;
using Cabrones.Test;
using FluentAssertions;
using NSubstitute;
using Strall.Exceptions;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public partial class TestPersistenceProviderSqLite
    {
        [Theory]
        [InlineData(typeof(PersistenceProviderSqLite), 17)]
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

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            
            // Act, When

            Action fecharConexao = () => persistenceProviderSqLite.Close();
            
            // Assert, Then

            fecharConexao.Should().ThrowExactly<StrallConnectionIsCloseException>();
        }
        
        [Fact]
        public void não_aceita_abrir_conexão_se_já_estiver_aberta()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;
            
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
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;
            
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

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;
            
            // Act, When

            persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then

            persistenceProviderSqLite.Mode.Should().Be(PersistenceProviderMode.Opened);
        }
        
        [Fact]
        public void após_fechamento_da_conexão_isso_deve_ser_sinalizado()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;
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
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;
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

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            
            // Act, When

            Action criarEstrutura = () => persistenceProviderSqLite.CreateStructure();
            
            // Assert, Then

            criarEstrutura.Should().ThrowExactly<StrallConnectionIsCloseException>();
        }
        
        [Fact]
        public void deve_ser_possível_solicitar_explicitamente_a_criação_da_estrutura_do_banco_de_dados()
        {
            // Arrange, Given

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>());
            File.Create(arquivo).Close();
            
            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new ConnectionInfo
            {
                Filename = arquivo
            } as IConnectionInfo;
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

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>());
            
            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new ConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;
            
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
        public void deve_ser_possível_substituir_o_ISqlNames_para_alterar_os_nomes_dos_objetos_do_banco_de_dados()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;

            var sqlNames = Substitute.For<ISqlNames>();
            sqlNames.TableInformation.Returns($"tb_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnId.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnDescription.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnContent.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnContentType.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnParentId.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnParentRelation.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnCloneId.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnSiblingOrder.Returns($"col_{this.Fixture().Create<string>().Substring(0, 8)}");
            
            // Act, When

            persistenceProviderSqLite.SqlNames = sqlNames;
            persistenceProviderSqLite.Open(connectionInfo);
            
            // Assert, Then
            
            using var command = persistenceProviderSqLite.Connection.CreateCommand();
            command.CommandText = $"SELECT sql FROM sqlite_master WHERE name = '{sqlNames.TableInformation}'";
            var ddl = (string)command.ExecuteScalar();

            persistenceProviderSqLite.SqlNames.Should().BeSameAs(sqlNames);
            ddl.Should().NotBeNull();
            ddl.Should().Contain(sqlNames.TableInformationColumnId);
            ddl.Should().Contain(sqlNames.TableInformationColumnDescription);
            ddl.Should().Contain(sqlNames.TableInformationColumnContent);
            ddl.Should().Contain(sqlNames.TableInformationColumnContentType);
            ddl.Should().Contain(sqlNames.TableInformationColumnParentId);
            ddl.Should().Contain(sqlNames.TableInformationColumnParentRelation);
            ddl.Should().Contain(sqlNames.TableInformationColumnCloneId);
            ddl.Should().Contain(sqlNames.TableInformationColumnSiblingOrder);
        }

        [Fact]
        public void não_deve_causar_exceção_chamar_Disposable_repetidas_vezes()
        {
            // Arrange, Given

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            
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

            var persistenceProviderSqLite = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            var connectionInfo = new ConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>()),
                CreateDatabaseIfNotExists = true
            } as IConnectionInfo;
            
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