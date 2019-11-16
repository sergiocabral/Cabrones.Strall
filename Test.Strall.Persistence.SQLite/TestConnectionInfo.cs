using System;
using System.IO;
using AutoFixture;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestConnectionInfo
    {
        [Theory]
        [InlineData(typeof(ConnectionInfo), 6)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Fact]
        public void deve_ter_construtor_sem_parâmetros()
        {
            // Arrange, Given
            // Act, When
            
            Func<ConnectionInfo> construir = () =>
            {
                var constructorInfo = typeof(ConnectionInfo).GetConstructor(new Type [0]);
                return constructorInfo?.Invoke(new object[0]) as ConnectionInfo;
            };
            
            // Assert, Then

            construir.Should().NotThrow().Which.Should().NotBeNull();
        }

        [Fact]
        public void a_string_de_conexão_deve_ser_válida_para_SQLite()
        {
            // Arrange, Given

            var arquivo = this.Fixture().Create<string>();
            var connectionInfo = new ConnectionInfo
            {
                Filename = arquivo
            };
            var stringDeConexãoEsperada = $"Data Source={arquivo};";
            
            // Act, When

            var stringDeConexãoRecebida = connectionInfo.ConnectionString;
            
            // Assert, Then

            stringDeConexãoRecebida.Should().Be(stringDeConexãoEsperada);
        }

        [Fact]
        public void não_pode_criar_arquivo_do_banco_de_dados_se_não_for_autorizado()
        {
            // Arrange, Given

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>());
            var connectionInfo = new ConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = false
            };
            
            // Act, When

            Action criarBancoDeDados = () => connectionInfo.CreateDatabase();
            
            // Assert, Then

            criarBancoDeDados.Should().ThrowExactly<StrallConnectionException>();
        }

        [Fact]
        public void deve_criar_arquivo_do_banco_de_dados_se_for_autorizado_e_não_existir()
        {
            // Arrange, Given

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>());
            var connectionInfo = new ConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = true
            };
            
            // Act, When

            Action criarBancoDeDados = () => connectionInfo.CreateDatabase();
            
            // Assert, Then

            criarBancoDeDados.Should().NotThrow();
        }

        [Fact]
        public void não_deve_criar_arquivo_do_banco_de_dados_se_for_autorizado_mas_arquivo_já_existir()
        {
            // Arrange, Given

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture().Create<string>());
            var conteúdoEscrito = this.Fixture().Create<string>();
            File.WriteAllText(arquivo, conteúdoEscrito);

            var connectionInfo = new ConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = true
            };
            
            // Act, When

            connectionInfo.CreateDatabase();
            var conteúdoLido = File.ReadAllText(arquivo);
            
            // Assert, Then

            conteúdoLido.Should().Be(conteúdoEscrito);
        }
    }
}