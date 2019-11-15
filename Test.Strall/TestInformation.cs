using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestInformation
    {
        [Theory]
        [InlineData(typeof(Information), 14)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(Information), typeof(Information))]
        public void verifica_se_classe_implementa_os_tipos_necessários(Type tipo, params Type[] tiposQueDeveSerImplementado) =>
            tipo.TestImplementations(tiposQueDeveSerImplementado);

    }
}