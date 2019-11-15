using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIInformation
    {
        [Theory]
        [InlineData(typeof(IInformation), 14)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(IInformation), "Guid get_Id()")]
        [InlineData(typeof(IInformation), "Void set_Id(Guid)")]
        [InlineData(typeof(IInformation), "String get_Description()")]
        [InlineData(typeof(IInformation), "Void set_Description(String)")]
        [InlineData(typeof(IInformation), "String get_Content()")]
        [InlineData(typeof(IInformation), "Void set_Content(String)")]
        [InlineData(typeof(IInformation), "String get_ContentType()")]
        [InlineData(typeof(IInformation), "Void set_ContentType(String)")]
        [InlineData(typeof(IInformation), "Guid get_ParentId()")]
        [InlineData(typeof(IInformation), "Void set_ParentId(Guid)")]
        [InlineData(typeof(IInformation), "Guid get_CloneId()")]
        [InlineData(typeof(IInformation), "Void set_CloneId(Guid)")]
        [InlineData(typeof(IInformation), "Int32 get_SiblingOrder()")]
        [InlineData(typeof(IInformation), "Void set_SiblingOrder(Int32)")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}