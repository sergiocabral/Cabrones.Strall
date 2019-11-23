namespace Strall.Persistence.SqlServer
{
    /// <summary>
    /// Informações para conexão com o banco de dados.
    /// </summary>
    public interface ISqlServerConnectionInfo: IConnectionInfo
    {
        /// <summary>
        /// Nome do banco de dados.
        /// </summary>
        string? Database { get; set; }

        /// <summary>
        /// Cria o arquivo do banco de dados caso não exista.
        /// </summary>
        bool CreateDatabaseIfNotExists { get; set; }

        /// <summary>
        /// String de conexão pronta.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Cria o arquivo do banco de dados.
        /// </summary>
        /// <returns>Indica se foi criado.</returns>
        bool CreateDatabase();
    }
}