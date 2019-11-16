using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestPersistenceProviderMode
    {
        [Theory]
        [InlineData(typeof(PersistenceProviderMode), 2)]
        public void verifica_se_o_total_de_valores_declarados_está_correto_neste_enum(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestEnumValuesCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(PersistenceProviderMode), "Closed", 0b_0010)]
        [InlineData(typeof(PersistenceProviderMode), "Opened", 0b_0100)]
        public void verifica_se_os_valores_do_enum_existem_com_base_no_nome_e_valor(Type tipo, string nomeEsperado, int valorEsperado) =>
            tipo.TestEnumValuePresence(nomeEsperado, valorEsperado);

    }
}