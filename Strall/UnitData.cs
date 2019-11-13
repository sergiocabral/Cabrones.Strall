namespace Strall
{
    public class UnitData: IUnitData
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Conteúdo.
        /// </summary>
        public string? Content { get; set; }
        
        /// <summary>
        /// Tipo.
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        public UnitData? Parent { get; set; }
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        public UnitData? CloneFrom { get; set; }
        
        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        public int Order { get; set; }
    }
}
