using System;
using System.Data;
using System.Data.SqlClient;
using Cabrones.Test;
using FluentAssertions;
using Xunit;

namespace Strall.Persistence.SqlServer
{
    public class TestSqlServerConnectionExtensions
    {
        [Fact]
        public void ConnectToMaster_conectar_no_banco_master()
        {
            // Arrange, Given

            var conexão = new SqlConnection();

            Action qualquerConsultaSql = () =>
            {
                using var comando = conexão.CreateCommand();
                comando.CommandText = "SELECT * FROM sysdatabases;";
                comando.ExecuteNonQuery();
            };
            Action fecharConexão = () => conexão.Close();
            
            // Act, When

            var conexãoEstabelecida = conexão.ConnectToMaster();
            
            // Assert, Then

            conexãoEstabelecida.Should().BeSameAs(conexão);
            conexão.State.Should().Be(ConnectionState.Open);
            qualquerConsultaSql.Should().NotThrow();
            fecharConexão.Should().NotThrow();
        }
        
        [Fact]
        public void DatabaseExists_deve_verificar_se_um_banco_de_dados_existe()
        {
            // Arrange, Given

            var conexão = new SqlConnection();
            conexão.ConnectToMaster();
            Action fecharConexão = () => conexão.Close();
            
            // Act, When

            var resposta = conexão.DatabaseExists("master");
            
            // Assert, Then

            resposta.Should().BeTrue();
            fecharConexão.Should().NotThrow();
        }
        
        [Fact]
        public void CreateDatabase_deve_criar_um_banco_de_dados()
        {
            // Arrange, Given

            var nomeDoBancoDeDados = "temp_" + this.Fixture<string>().Substring(0, 8);
            var conexão = new SqlConnection();
            conexão.ConnectToMaster();
            Action fecharConexão = () => conexão.Close();
            
            // Act, When

            var existiaAntes = conexão.DatabaseExists(nomeDoBancoDeDados);
            conexão.CreateDatabase(nomeDoBancoDeDados);
            var existeDepois = conexão.DatabaseExists(nomeDoBancoDeDados);
            
            // Assert, Then

            existiaAntes.Should().BeFalse();
            existeDepois.Should().BeTrue();
            fecharConexão.Should().NotThrow();
        }
        
        [Fact]
        public void CreateDatabase_deve_criar_um_banco_de_dados_e_poder_se_conectar_logo_em_seguida()
        {
            // Arrange, Given

            var nomeDoBancoDeDados = "temp_" + this.Fixture<string>().Substring(0, 8);
            var conexãoComMaster = new SqlConnection();
            conexãoComMaster.ConnectToMaster();
            conexãoComMaster.CreateDatabase(nomeDoBancoDeDados);
            conexãoComMaster.Close();

            // Act, When

            Action conectarEmSeguida = () =>
            {
                var conexão = new SqlConnection($"Server=.;Database={nomeDoBancoDeDados};Trusted_Connection=True;");
                conexão.Open();
                conexão.Close();
            };
            
            // Assert, Then

            conectarEmSeguida.Should().NotThrow();
        }
    }
}