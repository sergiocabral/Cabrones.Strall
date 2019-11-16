using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIDataAccess
    {
        [Theory]
        [InlineData(typeof(IDataAccess), 9)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(IDataAccess), "Boolean Exists(Guid)")]
        [InlineData(typeof(IDataAccess), "InformationRaw Get(Guid)")]
        [InlineData(typeof(IDataAccess), "Guid Create(InformationRaw)")]
        [InlineData(typeof(IDataAccess), "Boolean Update(InformationRaw)")]
        [InlineData(typeof(IDataAccess), "Boolean Delete(Guid)")]
        [InlineData(typeof(IDataAccess), "Boolean HasChildren(Guid)")]
        [InlineData(typeof(IDataAccess), "IEnumerable<Guid> Children(Guid)")]
        [InlineData(typeof(IDataAccess), "Boolean HasClones(Guid)")]
        [InlineData(typeof(IDataAccess), "IEnumerable<Guid> Clones(Guid)")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}