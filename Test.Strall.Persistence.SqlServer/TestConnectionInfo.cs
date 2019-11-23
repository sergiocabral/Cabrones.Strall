using System;
using System.Data.SqlClient;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Xunit;

namespace Strall.Persistence.SqlServer
{
    public class TestConnectionInfo
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(SqlServerConnectionInfo);

            // Assert, Then

            sut.AssertMyImplementations(typeof(ISqlServerConnectionInfo), typeof(ConnectionInfo), typeof(IConnectionInfo));
            sut.AssertMyOwnImplementations(typeof(ISqlServerConnectionInfo));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }

        [Fact]
        public void deve_ter_construtor_sem_parâmetros()
        {
            // Arrange, Given
            // Act, When
            
            Func<SqlServerConnectionInfo> construir = () =>
            {
                var constructorInfo = typeof(SqlServerConnectionInfo).GetConstructor(new Type [0]);
                return constructorInfo?.Invoke(new object[0]) as SqlServerConnectionInfo;
            };
            
            // Assert, Then

            construir.Should().NotThrow().Which.Should().NotBeNull();
        }

        [Fact]
        public void a_string_de_conexão_deve_ser_válida_para_SqlServer()
        {
            // Arrange, Given
            
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = "master"
            } as ISqlServerConnectionInfo;
            
            // Act, When

            Action conectar = () =>
            {
                var conexão = new SqlConnection(connectionInfo.ConnectionString);
                conexão.Open();
                conexão.Close();
            };

            // Assert, Then
            
            conectar.Should().NotThrow();
        }

        [Fact]
        public void não_pode_criar_o_banco_de_dados_se_não_for_autorizado()
        {
            // Arrange, Given

            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = this.Fixture<string>(),
                CreateDatabaseIfNotExists = false
            } as ISqlServerConnectionInfo;
            
            // Act, When

            Action criarBancoDeDados = () => connectionInfo.CreateDatabase();
            
            // Assert, Then

            criarBancoDeDados.Should().ThrowExactly<StrallConnectionException>();
        }

        [Fact]
        public void deve_criar_o_banco_de_dados_se_for_autorizado_e_não_existir()
        {
            // Arrange, Given

            var database = "temp_" + this.Fixture<string>().Substring(0, 8);
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = database,
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            
            // Act, When

            Func<bool> criarBancoDeDados = () => connectionInfo.CreateDatabase();
            
            // Assert, Then

            criarBancoDeDados.Should().NotThrow()
                .Which.Should().BeTrue();
        }

        [Fact]
        public void não_deve_criar_o_banco_de_dados_se_for_autorizado_mas_arquivo_já_existir()
        {
            // Arrange, Given

            var database = "temp_" + this.Fixture<string>().Substring(0, 8);
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = database,
                CreateDatabaseIfNotExists = true
            } as ISqlServerConnectionInfo;
            
            // Act, When

            var primeiraVez = connectionInfo.CreateDatabase();
            var segundaVez = connectionInfo.CreateDatabase();
            
            // Assert, Then

            primeiraVez.Should().BeTrue();
            segundaVez.Should().BeFalse();
        }
        
        [Fact]
        public void por_padrão_deve_sempre_criar_o_banco_de_dados_caso_não_exista()
        {
            // Arrange, Given
            // Act, When
            
            var connectionInfo = new SqlServerConnectionInfo() as ISqlServerConnectionInfo;

            // Assert, Then

            connectionInfo.CreateDatabaseIfNotExists.Should().BeTrue();
        }

        [Fact]
        public void deve_ser_possível_modificar_os_valores_das_propriedades_a_qualquer_momento()
        {
            // Arrange, Given

            var valorInicialParaDatabase = this.Fixture<string>();
            var valorInicialParaCreateDatabaseIfNotExists = this.Fixture<bool>();
            
            var connectionInfo = new SqlServerConnectionInfo
            {
                Database = valorInicialParaDatabase,
                CreateDatabaseIfNotExists = valorInicialParaCreateDatabaseIfNotExists
            } as ISqlServerConnectionInfo;
            
            // Act, When
            
            connectionInfo.Database = this.Fixture<string>();
            connectionInfo.CreateDatabaseIfNotExists = !connectionInfo.CreateDatabaseIfNotExists;

            // Assert, Then

            connectionInfo.Database.Should().NotBe(valorInicialParaDatabase);
            connectionInfo.CreateDatabaseIfNotExists.Should().Be(!valorInicialParaCreateDatabaseIfNotExists);
        }
    }
}