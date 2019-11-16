using System.IO;
using Strall.Exceptions;

namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Informações para conexão com o SQLite.
    /// </summary>
    public class ConnectionInfo: IConnectionInfo
    {
        /// <summary>
        /// Arquivo.
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// Cria o arquivo do banco de dados caso não exista.
        /// </summary>
        public bool CreateDatabaseIfNotExists { get; set; }

        /// <summary>
        /// String de conexão pronta.
        /// </summary>
        public string ConnectionString => $"Data Source={Filename};";

        /// <summary>
        /// Cria o arquivo do banco de dados.
        /// </summary>
        public ConnectionInfo CreateDatabase()
        {
            if (File.Exists(Filename)) return this;
            if (!CreateDatabaseIfNotExists) throw new StrallConnectionException();
            File.Create(Filename).Close();
            return this;
        }
    }
}