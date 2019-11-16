using System;

namespace Strall
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// </summary>
    public interface IPersistenceProvider<in TConnectionInfo>: IDataAccess, IDisposable
    {
        /// <summary>
        /// Cria a estrutura do banco de dados.
        /// </summary>
        IPersistenceProvider<TConnectionInfo> CreateStructure();
        
        /// <summary>
        /// Inicia a conexão.
        /// </summary>
        /// <param name="connection">Informações para conexão.</param>
        IPersistenceProvider<TConnectionInfo> Open(TConnectionInfo connection);

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        IPersistenceProvider<TConnectionInfo> Close();
        
        /// <summary>
        /// Modo atual.
        /// </summary>
        PersistenceProviderMode Mode { get; }
    }
}