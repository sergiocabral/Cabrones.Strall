namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Nomes no contexto do SQL.
    /// </summary>
    public interface ISqlNames
    {
        /// <summary>
        /// Tabela: Information
        /// </summary>
        string TableInformation { get; }

        /// <summary>
        /// Tabela: Information
        /// Coluna: Id 
        /// </summary>
        string TableInformationColumnId { get; }

        /// <summary>
        /// Tabela: Information
        /// Coluna: Description
        /// </summary>
        string TableInformationColumnDescription { get; }
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: Content 
        /// </summary>
        string TableInformationColumnContent { get; }
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: ContentType 
        /// </summary>
        string TableInformationColumnContentType { get; }
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: ParentId 
        /// </summary>
        string TableInformationColumnParentId { get; }
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: ParentRelation 
        /// </summary>
        string TableInformationColumnParentRelation { get; }
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: CloneId 
        /// </summary>
        string TableInformationColumnCloneId { get; }
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: SiblingOrder 
        /// </summary>
        string TableInformationColumnSiblingOrder { get; }
    }
}