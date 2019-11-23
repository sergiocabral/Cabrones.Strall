using System.IO;
using Strall.Exceptions;

namespace Strall.Persistence.SqLite
{
    /// <summary>
    /// Informações para conexão com o banco de dados.
    /// </summary>
    public class SqLiteConnectionInfo: ConnectionInfo, ISqLiteConnectionInfo
    {
        /// <summary>
        /// Arquivo.
        /// </summary>
        public string? Filename { get; set; }

        /// <summary>
        /// Cria o arquivo do banco de dados caso não exista.
        /// </summary>
        public bool CreateDatabaseIfNotExists { get; set; } = true;

        /// <summary>
        /// String de conexão pronta.
        /// </summary>
        public string ConnectionString => $"Data Source={Filename};";

        /// <summary>
        /// Cria o arquivo do banco de dados.
        /// </summary>
        /// <returns>Indica se foi criado.</returns>
        public bool CreateDatabase()
        {
            if (File.Exists(Filename)) return false;
            if (!CreateDatabaseIfNotExists) throw new StrallConnectionException();
            File.Create(Filename).Close();
            return true;
        }
    }
}