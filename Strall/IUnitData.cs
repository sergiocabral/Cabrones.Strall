namespace Strall
{
    public interface IUnitData
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        long Id { get; set; }
        
        /// <summary>
        /// Conteúdo.
        /// </summary>
        string? Content { get; set; }
        
        /// <summary>
        /// Tipo.
        /// </summary>
        string? Type { get; set; }
        
        /// <summary>
        /// Informação de onde esta é filha.
        /// </summary>
        UnitData? Parent { get; set; }
        
        /// <summary>
        /// Informação de onde esta é um clone.
        /// </summary>
        UnitData? CloneFrom { get; set; }
        
        /// <summary>
        /// Ordem de exibição entre informações irmãs.
        /// </summary>
        int Order { get; set; }
    }
}