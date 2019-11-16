using System;
using Cabrones.Test;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestISqlNames
    {
        [Theory]
        [InlineData(typeof(ISqlNames), 9)]
        public void verifica_se_o_total_de_métodos_públicos_declarados_está_correto_neste_tipo(Type tipo, int totalDeMétodosEsperado) =>
            tipo.TestMethodsCount(totalDeMétodosEsperado);
        
        [Theory]
        [InlineData(typeof(ISqlNames), "String get_TableInformation()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnId()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnDescription()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnContent()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnContentType()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnParentId()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnParentRelation()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnCloneId()")]
        [InlineData(typeof(ISqlNames), "String get_TableInformationColumnSiblingOrder()")]
        public void verifica_se_os_métodos_existem_com_base_na_assinatura(Type tipo, string assinaturaEsperada) =>
            tipo.TestMethodPresence(assinaturaEsperada);
    }
}