using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIPersistenceProvider
    {
        [Theory]
        [InlineData(typeof(IPersistenceProvider), 7)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(IPersistenceProvider), "Boolean Exists(Guid)")]
        [InlineData(typeof(IPersistenceProvider), "Information Get(Guid)")]
        [InlineData(typeof(IPersistenceProvider), "Guid Create(Information)")]
        [InlineData(typeof(IPersistenceProvider), "Boolean Update(Information)")]
        [InlineData(typeof(IPersistenceProvider), "Boolean Delete(Guid)")]
        [InlineData(typeof(IPersistenceProvider), "Boolean HasChildren(Guid)")]
        [InlineData(typeof(IPersistenceProvider), "IEnumerable<Guid> Children(Guid)")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}