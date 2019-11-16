using System;
using System.Collections.Generic;

namespace Strall.Persistence.SQLite
{
    public class PersistenceProviderSqLite: IPersistenceProvider<string>
    {
        /// <summary>
        /// Inicia a conexão.
        /// </summary>
        /// <param name="connection">Informações para conexão.</param>
        public void Open(string connection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        public void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Modo atual.
        /// </summary>
        public PersistenceProviderMode Mode { get; } = PersistenceProviderMode.Closed;
        
        /// <summary>
        /// Verifica se uma informação existe.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId">Id.</param>
        /// <returns>Resposta de existência.</returns>
        public bool Exists(Guid informationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtem uma informação.
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Informação.</returns>
        public Information? Get(Guid informationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cria uma informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Id.</returns>
        public Guid Create(Information information)
        {
            throw new NotImplementedException();   
        }

        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        public bool Update(Information information)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Apaga uma informação.
        /// Equivalente a DELETE.
        /// Não é recursivo para seus filhos.
        /// </summary>
        /// <param name="informationId">Id.</param>
        /// <returns>Resposta de sucesso.</returns>
        public bool Delete(Guid informationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifica se tem filhos.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns></returns>
        public bool HasChildren(Guid informationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retorna a lista de filhos imediatos.
        /// Não é recursivo.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        public IEnumerable<Guid> Children(Guid informationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Liberação tipo IDispose.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}