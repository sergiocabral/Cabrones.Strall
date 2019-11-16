namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Informações para conexão com o SQLite.
    /// </summary>
    public interface IConnectionInfo
    {
        /// <summary>
        /// Arquivo.
        /// </summary>
        string? Filename { get; set; }

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