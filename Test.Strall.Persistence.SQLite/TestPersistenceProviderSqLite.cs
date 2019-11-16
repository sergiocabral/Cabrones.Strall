using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestPersistenceProviderSqLite
    {
        [Theory]
        [InlineData(typeof(PersistenceProviderSqLite), 11)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(PersistenceProviderSqLite), typeof(IPersistenceProvider<string>), typeof(IDisposable))]
        public void verifica_se_classe_implementa_os_tipos_necessários(Type tipoDaClasse, params Type[] tiposQueDeveSerImplementado) =>
            tipoDaClasse.TestImplementations(tiposQueDeveSerImplementado);
    }
}