using System.Data.SqlClient;
using Strall.Exceptions;

namespace Strall.Persistence.SqlServer
{
    /// <summary>
    /// Informações para conexão com o banco de dados.
    /// </summary>
    public class SqlServerConnectionInfo: ConnectionInfo, ISqlServerConnectionInfo
    {
        /// <summary>
        /// Nome do banco de dados..
        /// </summary>
        public string? Database { get; set; }

        /// <summary>
        /// Cria o arquivo do banco de dados caso não exista.
        /// </summary>
        public bool CreateDatabaseIfNotExists { get; set; } = true;

        /// <summary>
        /// String de conexão pronta.
        /// </summary>
        public string ConnectionString => CreateConnectionString(Database);

        /// <summary>
        /// Cria o banco de dados.
        /// </summary>
        /// <returns>Indica se foi criado.</returns>
        public bool CreateDatabase()
        {
            if (!CreateDatabaseIfNotExists) throw new StrallConnectionException();
            try
            {
                using var connection = new SqlConnection();
                connection.ConnectToMaster();
                try
                {
                    if (!connection.DatabaseExists(Database))
                    {
                        connection.CreateDatabase(Database);
                        return true;
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            catch
            {
                // Ignora o erro.
            }
            return false;
        }
        
        /// <summary>
        /// Cria uma string de conexão.
        /// </summary>
        /// <param name="database">Nome do banco de dados.</param>
        /// <returns>String de conexão.</returns>
        private static string CreateConnectionString(string? database) =>
            $"Server=.;Database={database};Trusted_Connection=True;";
    }
}