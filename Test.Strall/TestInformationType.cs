using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestInformationType
    {
        [Theory]
        [InlineData(typeof(InformationType), 1)]
        public void verifica_se_o_total_de_valores_declarados_está_correto_neste_enum(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestEnumValuesCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(InformationType), "Text", 0b_0010)]
        public void verifica_se_os_valores_do_enum_existem_com_base_no_nome_e_valor(Type tipo, string nomeEsperado, int valorEsperado) =>
            tipo.TestEnumValuePresence(nomeEsperado, valorEsperado);
    }
}