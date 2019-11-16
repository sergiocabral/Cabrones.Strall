using System;
using System.Collections.Generic;

namespace Strall
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// </summary>
    public interface IPersistenceProvider<in TConnectionInfo>: IDisposable
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
        
        /// <summary>
        /// Verifica se uma informação existe.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId">Id.</param>
        /// <returns>Resposta de existência.</returns>
        bool Exists(Guid informationId);
        
        /// <summary>
        /// Obtem uma informação.
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Informação.</returns>
        Information? Get(Guid informationId);

        /// <summary>
        /// Cria uma informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Id.</returns>
        Guid Create(Information information);
        
        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        bool Update(Information information);

        /// <summary>
        /// Apaga uma informação.
        /// Equivalente a DELETE.
        /// Não é recursivo para seus filhos.
        /// </summary>
        /// <param name="informationId">Id.</param>
        /// <returns>Resposta de sucesso.</returns>
        bool Delete(Guid informationId);

        /// <summary>
        /// Verifica se tem filhos.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns></returns>
        bool HasChildren(Guid informationId);
        
        /// <summary>
        /// Retorna a lista de filhos imediatos.
        /// Não é recursivo.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        IEnumerable<Guid> Children(Guid informationId);
    }
}