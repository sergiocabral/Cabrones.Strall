using Microsoft.Data.Sqlite;
using Strall.Persistence.Sql;

namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQLite.
    /// </summary>
    public interface IPersistenceProviderSqLite: IPersistenceProviderSql<IConnectionInfo>
    {
        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        new SqliteConnection? Connection { get; }
    }
}