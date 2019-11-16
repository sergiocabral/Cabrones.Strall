using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIInformationRaw
    {
        [Theory]
        [InlineData(typeof(IInformationRaw), 16)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(IInformationRaw), "Guid get_Id()")]
        [InlineData(typeof(IInformationRaw), "Void set_Id(Guid)")]
        [InlineData(typeof(IInformationRaw), "String get_Description()")]
        [InlineData(typeof(IInformationRaw), "Void set_Description(String)")]
        [InlineData(typeof(IInformationRaw), "String get_Content()")]
        [InlineData(typeof(IInformationRaw), "Void set_Content(String)")]
        [InlineData(typeof(IInformationRaw), "String get_ContentType()")]
        [InlineData(typeof(IInformationRaw), "Void set_ContentType(String)")]
        [InlineData(typeof(IInformationRaw), "Guid get_ParentId()")]
        [InlineData(typeof(IInformationRaw), "Void set_ParentId(Guid)")]
        [InlineData(typeof(IInformationRaw), "String get_ParentRelation()")]
        [InlineData(typeof(IInformationRaw), "Void set_ParentRelation(String)")]
        [InlineData(typeof(IInformationRaw), "Guid get_CloneFromId()")]
        [InlineData(typeof(IInformationRaw), "Void set_CloneFromId(Guid)")]
        [InlineData(typeof(IInformationRaw), "Int32 get_SiblingOrder()")]
        [InlineData(typeof(IInformationRaw), "Void set_SiblingOrder(Int32)")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}