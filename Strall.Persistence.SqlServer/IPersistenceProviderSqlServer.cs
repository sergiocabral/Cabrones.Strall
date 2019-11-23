using System.Data.SqlClient;
using Strall.Persistence.Sql;

namespace Strall.Persistence.SqlServer
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQL Server
    /// </summary>
    public interface IPersistenceProviderSqlServer: IPersistenceProviderSql, IPersistenceProvider<ISqlServerConnectionInfo>
    {
        /// <summary>
        /// Conexão com o SQl Server.
        /// </summary>
        new SqlConnection? Connection { get; }
    }
}