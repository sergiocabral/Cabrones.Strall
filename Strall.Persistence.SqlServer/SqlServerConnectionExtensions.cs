using System.Data.SqlClient;

namespace Strall.Persistence.SqlServer
{
    /// <summary>
    /// Funcionalidades para administração de banco de dados SQL Server.
    /// </summary>
    public static class SqlServerConnectionExtensions
    {
        /// <summary>
        /// Conecta no banco de dados master
        /// </summary>
        /// <param name="connection">Conexão.</param>
        /// <returns>Auto referência.</returns>
        public static SqlConnection ConnectToMaster(this SqlConnection connection)
        {
            connection.ConnectionString = "Server=.;Database=master;Trusted_Connection=True;";
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Verifica se um banco de dados existe.
        /// </summary>
        /// <param name="connection">Conexão.</param>
        /// <param name="database">Banco de dados</param>
        /// <returns>Resposta.</returns>
        public static bool DatabaseExists(this SqlConnection connection, string? database)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $@"
SELECT name
  FROM sysdatabases 
 WHERE '[' + name + ']' = '{database}'
    OR name = '{database}';";
            var exists = command.ExecuteScalar() != null;
            return exists;
        }

        /// <summary>
        /// Cria um banco de dados.
        /// </summary>
        /// <param name="connection">Conexão.</param>
        /// <param name="database">Banco de dados</param>
        public static void CreateDatabase(this SqlConnection connection, string? database)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE {database};";
            command.ExecuteNonQuery();
            SqlConnection.ClearAllPools();
        }
    }
}