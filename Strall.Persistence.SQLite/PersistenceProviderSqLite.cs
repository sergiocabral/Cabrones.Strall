using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Strall.Exceptions;

namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQLite.
    /// </summary>
    public class PersistenceProviderSqLite: IPersistenceProviderSqLite
    {
        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        public SqliteConnection? Connection { get; private set; }

        /// <summary>
        /// Nomes no contexto do SQL.
        /// </summary>
        public ISqlNames SqlNames { get; set; } = new SqlNames() as ISqlNames;
        
        /// <summary>
        /// Cria a estrutura do banco de dados.
        /// </summary>
        public void CreateStructure()
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            
            var commandText = $@"
CREATE TABLE IF NOT EXISTS {SqlNames.TableInformation} (
    {SqlNames.TableInformationColumnId} TEXT,
    {SqlNames.TableInformationColumnDescription} TEXT,
    {SqlNames.TableInformationColumnContent} TEXT,
    {SqlNames.TableInformationColumnContentType} TEXT,
    {SqlNames.TableInformationColumnParentId} TEXT,
    {SqlNames.TableInformationColumnParentRelation} TEXT,
    {SqlNames.TableInformationColumnCloneId} TEXT,
    {SqlNames.TableInformationColumnSiblingOrder} INTEGER,
    PRIMARY KEY ({SqlNames.TableInformationColumnId}), 
    FOREIGN KEY ({SqlNames.TableInformationColumnParentId})
        REFERENCES {SqlNames.TableInformation} ({SqlNames.TableInformationColumnId})
            ON DELETE RESTRICT
            ON UPDATE RESTRICT,
    FOREIGN KEY ({SqlNames.TableInformationColumnCloneId})
        REFERENCES {SqlNames.TableInformation} ({SqlNames.TableInformationColumnId})
            ON DELETE RESTRICT
            ON UPDATE RESTRICT
) WITHOUT ROWID;

CREATE INDEX IDX_{SqlNames.TableInformation}_{SqlNames.TableInformationColumnSiblingOrder}
    ON {SqlNames.TableInformation} (
        {SqlNames.TableInformationColumnParentId},
        {SqlNames.TableInformationColumnSiblingOrder}
    );
".Trim();
                
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery(); 
        }    
        
        /// <summary>
        /// Inicia a conexão.
        /// </summary>
        /// <param name="connectionInfo">Informações para conexão.</param>
        public void Open(IConnectionInfo connectionInfo)
        {
            if (Connection != null) throw new StrallConnectionIsOpenException();
            
            var createdDatabase = connectionInfo.CreateDatabase();
            
            Connection = new SqliteConnection(connectionInfo.ConnectionString);
            Connection.Open();
            
            if (createdDatabase) CreateStructure();
            
            Mode = PersistenceProviderMode.Opened;
        }

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        public void Close()
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            Dispose();
        }

        /// <summary>
        /// Modo atual.
        /// </summary>
        public PersistenceProviderMode Mode { get; private set; } = PersistenceProviderMode.Closed;
        
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
            if (Connection == null) return;
            Connection.Close();
            Connection.Dispose();
            Connection = null;
            Mode = PersistenceProviderMode.Closed;
        }
    }
}