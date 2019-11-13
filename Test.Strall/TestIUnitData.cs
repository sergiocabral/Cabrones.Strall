using System;
using Xunit;

namespace Strall
{
    public class TestIUnitData: BaseForTest
    {
        [Theory]
        [InlineData(typeof(IUnitData), 12)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            verifica_se_o_total_de_métodos_públicos_declarados_está_correto_no_tipo(tipo, totalDeMétodosEsperado);

        [Theory]
        [InlineData(typeof(IUnitData), "Int64 get_Id()")]
        [InlineData(typeof(IUnitData), "Void set_Id(Int64)")]
        [InlineData(typeof(IUnitData), "String get_Content()")]
        [InlineData(typeof(IUnitData), "Void set_Content(String)")]
        [InlineData(typeof(IUnitData), "String get_Type()")]
        [InlineData(typeof(IUnitData), "Void set_Type(String)")]
        [InlineData(typeof(IUnitData), "UnitData get_Parent()")]
        [InlineData(typeof(IUnitData), "Void set_Parent(UnitData)")]
        [InlineData(typeof(IUnitData), "UnitData get_CloneFrom()")]
        [InlineData(typeof(IUnitData), "Void set_CloneFrom(UnitData)")]
        [InlineData(typeof(IUnitData), "Int32 get_Order()")]
        [InlineData(typeof(IUnitData), "Void set_Order(Int32)")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            verifica_se_o_método_existe_com_base_na_assinatura(tipo, assinaturaEsperada);
    }
}