using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestIPersistenceProviderSqLite
    {
        [Theory]
        [InlineData(typeof(IPersistenceProviderSqLite), 4)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);
        
        [Theory]
        [InlineData(typeof(IPersistenceProviderSqLite), "SqliteConnection get_Connection()")]
        [InlineData(typeof(IPersistenceProviderSqLite), "ISqlNames get_SqlNames()")]
        [InlineData(typeof(IPersistenceProviderSqLite), "Void set_SqlNames(ISqlNames)")]
        [InlineData(typeof(IPersistenceProviderSqLite), "Void CreateStructure()")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}