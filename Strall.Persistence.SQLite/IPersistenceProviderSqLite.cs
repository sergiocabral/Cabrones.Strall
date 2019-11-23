using Microsoft.Data.Sqlite;
using Strall.Persistence.Sql;

namespace Strall.Persistence.SqLite
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQLite.
    /// </summary>
    public interface IPersistenceProviderSqLite: IPersistenceProviderSql, IPersistenceProvider<ISqLiteConnectionInfo>
    {
        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        new SqliteConnection? Connection { get; }
    }
}