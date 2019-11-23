using System.Data.Common;

namespace Strall.Persistence.Sql
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQLite.
    /// </summary>
    public interface IPersistenceProviderSql<in TConnectionInfo>: IPersistenceProvider<TConnectionInfo>
    {
        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        DbConnection? Connection { get; }
        
        /// <summary>
        /// Nomes no contexto do SQL.
        /// </summary>
        ISqlNames SqlNames { get; set; }
    }
}