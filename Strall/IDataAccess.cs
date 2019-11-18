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
        IInformationRaw? Get(Guid informationId);

        /// <summary>
        /// Cria uma informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <param name="informationRaw">Informação.</param>
        /// <returns>Id.</returns>
        Guid Create(IInformationRaw informationRaw);
        
        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="informationRaw">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        bool Update(IInformationRaw informationRaw);

        /// <summary>
        /// Apaga uma informação.
        /// Equivalente a DELETE.
        /// Não é recursivo para seus filhos.
        /// </summary>
        /// <param name="informationId">Id.</param>
        /// <returns>Resposta de sucesso.</returns>
        bool Delete(Guid informationId);

        /// <summary>
        /// Verifica se tem clones.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns></returns>
        bool HasContentTo(Guid informationId);
        
        /// <summary>
        /// Retorna a lista de clones.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        IEnumerable<Guid> ContentTo(Guid informationId);

        /// <summary>
        /// Localiza a origem de um clone
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>
        /// Id da origem. Em caso de loop retorna Guid.Empty.
        /// Caso não seja clone retorna o mesmo id.
        /// </returns>
        Guid ContentFrom(Guid informationId);

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