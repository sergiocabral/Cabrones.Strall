using Microsoft.Data.Sqlite;

namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQLite.
    /// </summary>
    public interface IPersistenceProviderSqLite: IPersistenceProvider<IConnectionInfo>
    {
        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        SqliteConnection? Connection { get; }
        
        /// <summary>
        /// Nomes no contexto do SQL.
        /// </summary>
        ISqlNames SqlNames { get; set; }
    }
}