namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Nomes no contexto do SQL.
    /// </summary>
    public class SqlNames : ISqlNames
    {
        /// <summary>
        /// Tabela: Information
        /// </summary>
        public string TableInformation => "Information";

        /// <summary>
        /// Tabela: Information
        /// Coluna: Id 
        /// </summary>
        public string TableInformationColumnId => "Id";

        /// <summary>
        /// Tabela: Information
        /// Coluna: Description
        /// </summary>
        public string TableInformationColumnDescription => "Description";
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: Content 
        /// </summary>
        public string TableInformationColumnContent => "Content";
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: ContentType 
        /// </summary>
        public string TableInformationColumnContentType => "ContentType";
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: CloneId 
        /// </summary>
        public string TableInformationColumnContentFromId => "ContentFromId";
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: ParentId 
        /// </summary>
        public string TableInformationColumnParentId => "ParentId";
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: ParentRelation 
        /// </summary>
        public string TableInformationColumnParentRelation => "ParentRelation";
        
        /// <summary>
        /// Tabela: Information
        /// Coluna: SiblingOrder 
        /// </summary>
        public string TableInformationColumnSiblingOrder => "SiblingOrder";
    }
}