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

        [Theory]
        [InlineData(typeof(ConnectionInfo), typeof(IConnectionInfo))]
        public void verifica_se_classe_implementa_os_tipos_necessários(Type tipoDaClasse, params Type[] tiposQueDeveSerImplementado) =>
            tipoDaClasse.TestImplementations(tiposQueDeveSerImplementado);

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
            } as IConnectionInfo;
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
            } as IConnectionInfo;
            
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
            } as IConnectionInfo;
            
            // Act, When

            Func<bool> criarBancoDeDados = () => connectionInfo.CreateDatabase();
            
            // Assert, Then

            criarBancoDeDados.Should().NotThrow()
                .Which.Should().BeTrue();
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
            } as IConnectionInfo;
            
            // Act, When

            var criouArquivo = connectionInfo.CreateDatabase();
            var conteúdoLido = File.ReadAllText(arquivo);
            
            // Assert, Then

            criouArquivo.Should().BeFalse();
            conteúdoLido.Should().Be(conteúdoEscrito);
        }

        [Fact]
        public void por_padrão_deve_sempre_criar_o_arquivo_caso_não_exista()
        {
            // Arrange, Given
            // Act, When
            
            var connectionInfo = new ConnectionInfo() as IConnectionInfo;

            // Assert, Then

            connectionInfo.CreateDatabaseIfNotExists.Should().BeTrue();
        }

        [Fact]
        public void deve_ser_possível_modificar_os_valores_das_propriedades_a_qualquer_momento()
        {
            // Arrange, Given

            var valorInicialParaFilename = this.Fixture().Create<string>();
            var valorInicialParaCreateDatabaseIfNotExists = this.Fixture().Create<bool>();
            
            var connectionInfo = new ConnectionInfo
            {
                Filename = valorInicialParaFilename,
                CreateDatabaseIfNotExists = valorInicialParaCreateDatabaseIfNotExists
            } as IConnectionInfo;
            
            // Act, When
            
            connectionInfo.Filename = this.Fixture().Create<string>();
            connectionInfo.CreateDatabaseIfNotExists = !connectionInfo.CreateDatabaseIfNotExists;

            // Assert, Then

            connectionInfo.Filename.Should().NotBe(valorInicialParaFilename);
            connectionInfo.CreateDatabaseIfNotExists.Should().Be(!valorInicialParaCreateDatabaseIfNotExists);

        }
    }
}