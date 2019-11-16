using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIPersistenceProvider
    {
        [Theory]
        [InlineData(typeof(IPersistenceProvider<object>), 11)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(IPersistenceProvider<object>), "IPersistenceProvider<Object> CreateStructure()")]
        [InlineData(typeof(IPersistenceProvider<object>), "IPersistenceProvider<Object> Open(Object)")]
        [InlineData(typeof(IPersistenceProvider<object>), "IPersistenceProvider<Object> Close()")]
        [InlineData(typeof(IPersistenceProvider<object>), "PersistenceProviderMode get_Mode()")]
        [InlineData(typeof(IPersistenceProvider<object>), "Boolean Exists(Guid)")]
        [InlineData(typeof(IPersistenceProvider<object>), "Information Get(Guid)")]
        [InlineData(typeof(IPersistenceProvider<object>), "Guid Create(Information)")]
        [InlineData(typeof(IPersistenceProvider<object>), "Boolean Update(Information)")]
        [InlineData(typeof(IPersistenceProvider<object>), "Boolean Delete(Guid)")]
        [InlineData(typeof(IPersistenceProvider<object>), "Boolean HasChildren(Guid)")]
        [InlineData(typeof(IPersistenceProvider<object>), "IEnumerable<Guid> Children(Guid)")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}