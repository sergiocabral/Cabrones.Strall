using System;
using System.Collections.Generic;

namespace Strall
{
    /// <summary>
    /// Responsável pelo acesso e manipulação dos dados em alto nível
    /// </summary>
    public interface IDataAccess
    {
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

        /// <summary>
        /// Verifica se tem clones.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns></returns>
        bool HasClones(Guid informationId);
        
        /// <summary>
        /// Retorna a lista de clones.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        IEnumerable<Guid> Clones(Guid informationId);
    }
}