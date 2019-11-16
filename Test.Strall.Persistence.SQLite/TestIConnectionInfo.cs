using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestIConnectionInfo
    {
        [Theory]
        [InlineData(typeof(IConnectionInfo), 6)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);
        
        [Theory]
        [InlineData(typeof(IConnectionInfo), "String get_Filename()")]
        [InlineData(typeof(IConnectionInfo), "Void set_Filename(String)")]
        [InlineData(typeof(IConnectionInfo), "Boolean get_CreateDatabaseIfNotExists()")]
        [InlineData(typeof(IConnectionInfo), "Void set_CreateDatabaseIfNotExists(Boolean)")]
        [InlineData(typeof(IConnectionInfo), "String get_ConnectionString()")]
        [InlineData(typeof(IConnectionInfo), "ConnectionInfo CreateDatabase()")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}