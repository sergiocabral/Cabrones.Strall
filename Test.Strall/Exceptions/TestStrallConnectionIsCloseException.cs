using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Exceptions
{
    public class TestStrallConnectionIsCloseException
    {
        [Theory]
        [InlineData(typeof(StrallConnectionIsCloseException), 0)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);
    }
}