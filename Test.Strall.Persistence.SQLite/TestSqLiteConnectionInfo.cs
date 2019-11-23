using System;
using System.IO;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Xunit;

namespace Strall.Persistence.SqLite
{
    public class TestSqLiteConnectionInfo
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(SqLiteConnectionInfo);

            // Assert, Then

            sut.AssertMyImplementations(typeof(ISqLiteConnectionInfo), typeof(ConnectionInfo), typeof(IConnectionInfo));
            sut.AssertMyOwnImplementations(typeof(ISqLiteConnectionInfo));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }

        [Fact]
        public void deve_ter_construtor_sem_parâmetros()
        {
            // Arrange, Given
            // Act, When
            
            Func<SqLiteConnectionInfo> construir = () =>
            {
                var constructorInfo = typeof(SqLiteConnectionInfo).GetConstructor(new Type [0]);
                return constructorInfo?.Invoke(new object[0]) as SqLiteConnectionInfo;
            };
            
            // Assert, Then

            construir.Should().NotThrow().Which.Should().NotBeNull();
        }

        [Fact]
        public void a_string_de_conexão_deve_ser_válida_para_SQLite()
        {
            // Arrange, Given

            var arquivo = this.Fixture<string>();
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = arquivo
            } as ISqLiteConnectionInfo;
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

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>());
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = false
            } as ISqLiteConnectionInfo;
            
            // Act, When

            Action criarBancoDeDados = () => connectionInfo.CreateDatabase();
            
            // Assert, Then

            criarBancoDeDados.Should().ThrowExactly<StrallConnectionException>();
        }

        [Fact]
        public void deve_criar_arquivo_do_banco_de_dados_se_for_autorizado_e_não_existir()
        {
            // Arrange, Given

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>());
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            
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

            var arquivo = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>());
            var conteúdoEscrito = this.Fixture<string>();
            File.WriteAllText(arquivo, conteúdoEscrito);

            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = arquivo,
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;
            
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
            
            var connectionInfo = new SqLiteConnectionInfo() as ISqLiteConnectionInfo;

            // Assert, Then

            connectionInfo.CreateDatabaseIfNotExists.Should().BeTrue();
        }

        [Fact]
        public void deve_ser_possível_modificar_os_valores_das_propriedades_a_qualquer_momento()
        {
            // Arrange, Given

            var valorInicialParaFilename = this.Fixture<string>();
            var valorInicialParaCreateDatabaseIfNotExists = this.Fixture<bool>();
            
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = valorInicialParaFilename,
                CreateDatabaseIfNotExists = valorInicialParaCreateDatabaseIfNotExists
            } as ISqLiteConnectionInfo;
            
            // Act, When
            
            connectionInfo.Filename = this.Fixture<string>();
            connectionInfo.CreateDatabaseIfNotExists = !connectionInfo.CreateDatabaseIfNotExists;

            // Assert, Then

            connectionInfo.Filename.Should().NotBe(valorInicialParaFilename);
            connectionInfo.CreateDatabaseIfNotExists.Should().Be(!valorInicialParaCreateDatabaseIfNotExists);

        }
    }
}