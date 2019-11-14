using System;
using Xunit;

namespace Strall
{
    public class TestIInformation: BaseForTest
    {
        [Theory]
        [InlineData(typeof(IInformation), 14)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            verifica_se_o_total_de_métodos_públicos_declarados_está_correto_no_tipo(tipo, totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(IInformation), "Guid get_Id()")]
        [InlineData(typeof(IInformation), "Void set_Id(Guid)")]
        [InlineData(typeof(IInformation), "String get_Description()")]
        [InlineData(typeof(IInformation), "Void set_Description(String)")]
        [InlineData(typeof(IInformation), "Object get_Content()")]
        [InlineData(typeof(IInformation), "Void set_Content(Object)")]
        [InlineData(typeof(IInformation), "InformationType get_Type()")]
        [InlineData(typeof(IInformation), "Void set_Type(InformationType)")]
        [InlineData(typeof(IInformation), "IInformation get_Parent()")]
        [InlineData(typeof(IInformation), "Void set_Parent(IInformation)")]
        [InlineData(typeof(IInformation), "IInformation get_Clone()")]
        [InlineData(typeof(IInformation), "Void set_Clone(IInformation)")]
        [InlineData(typeof(IInformation), "Int32 get_Order()")]
        [InlineData(typeof(IInformation), "Void set_Order(Int32)")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            verifica_se_o_método_existe_com_base_na_assinatura(tipo, assinaturaEsperada);
    }
}