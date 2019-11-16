﻿using System;
using System.Collections.Generic;
using Cabrones.Utils.Converter;
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
        public IPersistenceProvider<IConnectionInfo> CreateStructure()
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
    {SqlNames.TableInformationColumnCloneFromId} TEXT,
    {SqlNames.TableInformationColumnSiblingOrder} INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY ({SqlNames.TableInformationColumnId}), 
    FOREIGN KEY ({SqlNames.TableInformationColumnParentId})
        REFERENCES {SqlNames.TableInformation} ({SqlNames.TableInformationColumnId})
            ON DELETE RESTRICT
            ON UPDATE RESTRICT,
    FOREIGN KEY ({SqlNames.TableInformationColumnCloneFromId})
        REFERENCES {SqlNames.TableInformation} ({SqlNames.TableInformationColumnId})
            ON DELETE RESTRICT
            ON UPDATE RESTRICT
) WITHOUT ROWID;

CREATE INDEX IF NOT EXISTS IDX_{SqlNames.TableInformation}_{SqlNames.TableInformationColumnSiblingOrder}
    ON {SqlNames.TableInformation} (
        {SqlNames.TableInformationColumnParentId},
        {SqlNames.TableInformationColumnSiblingOrder}
    );
";
                
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();

            return this;
        }    
        
        /// <summary>
        /// Inicia a conexão.
        /// </summary>
        /// <param name="connectionInfo">Informações para conexão.</param>
        public IPersistenceProvider<IConnectionInfo> Open(IConnectionInfo connectionInfo)
        {
            if (Connection != null) throw new StrallConnectionIsOpenException();
            
            var createdDatabase = connectionInfo.CreateDatabase();
            
            Connection = new SqliteConnection(connectionInfo.ConnectionString);
            Connection.Open();
            
            if (createdDatabase) CreateStructure();
            
            Mode = PersistenceProviderMode.Opened;

            return this;
        }

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        public IPersistenceProvider<IConnectionInfo> Close()
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            Dispose();
            return this;
        }

        /// <summary>
        /// Modo atual.
        /// </summary>
        public PersistenceProviderMode Mode { get; private set; } = PersistenceProviderMode.Closed;

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

        /// <summary>
        /// Monta uma lista de parâmetros.
        /// </summary>
        /// <param name="informationRaw">Informação.</param>
        /// <returns>Lista de parâmetros.</returns>
        private IEnumerable<SqliteParameter> FactoryParameters(IInformationRaw informationRaw) =>
            new List<SqliteParameter>
            {
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnId,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.Id.ToDatabaseText()
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnDescription,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.Description
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnContent,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.Content
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnContentType,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.ContentType
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnParentId,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.ParentId.ToDatabaseText()
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnParentRelation,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.ParentRelation
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnCloneFromId,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.CloneFromId.ToDatabaseText()
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnSiblingOrder,
                    SqliteType = SqliteType.Integer,
                    Value = informationRaw.SiblingOrder
                }
            };

        /// <summary>
        /// Monta uma lista de parâmetros.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista de parâmetros.</returns>
        private IEnumerable<SqliteParameter> FactoryParameters(Guid informationId) =>
            new List<SqliteParameter>
            {
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnId,
                    SqliteType = SqliteType.Text,
                    Value = informationId.ToDatabaseText()
                }
            };
        
        /// <summary>
        /// Verifica se uma informação existe.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId">Id.</param>
        /// <returns>Resposta de existência.</returns>
        public bool Exists(Guid informationId)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (informationId == Guid.Empty) return false;

            var commandText = $@"
SELECT {SqlNames.TableInformationColumnId}
  FROM {SqlNames.TableInformation}
 WHERE {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId}
 LIMIT 1;
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId));
            return command.ExecuteScalar() != null;
        }

        /// <summary>
        /// Obtem uma informação.
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Informação.</returns>
        public IInformationRaw? Get(Guid informationId)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (informationId == Guid.Empty) return null;

            var commandText = $@"
SELECT {SqlNames.TableInformationColumnId},
       {SqlNames.TableInformationColumnDescription},
       {SqlNames.TableInformationColumnContent},
       {SqlNames.TableInformationColumnContentType},
       {SqlNames.TableInformationColumnParentId},
       {SqlNames.TableInformationColumnParentRelation},
       {SqlNames.TableInformationColumnCloneFromId},
       {SqlNames.TableInformationColumnSiblingOrder}
  FROM {SqlNames.TableInformation}
 WHERE {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId}
 LIMIT 1;
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId));
            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            var i = -1;
            return new InformationRaw
            {
                Id = reader.GetValue(++i).ToGuid(),
                Description = $"{reader.GetValue(++i)}",
                Content = $"{reader.GetValue(++i)}",
                ContentType = $"{reader.GetValue(++i)}",
                ParentId = reader.GetValue(++i).ToGuid(),
                ParentRelation = $"{reader.GetValue(++i)}",
                CloneFromId = reader.GetValue(++i).ToGuid(),
                SiblingOrder = reader.GetFieldValue<int>(++i)
            };
        }

        /// <summary>
        /// Cria uma informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <param name="informationRaw">Informação.</param>
        /// <returns>Id.</returns>
        public Guid Create(IInformationRaw informationRaw)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (informationRaw == null) return Guid.Empty;
            
            var id = informationRaw.Id != Guid.Empty ? informationRaw.Id : Guid.NewGuid();
            var commandText = $@"
INSERT INTO {SqlNames.TableInformation} (
    {SqlNames.TableInformationColumnId},
    {SqlNames.TableInformationColumnDescription},
    {SqlNames.TableInformationColumnContent},
    {SqlNames.TableInformationColumnContentType},
    {SqlNames.TableInformationColumnParentId},
    {SqlNames.TableInformationColumnParentRelation},
    {SqlNames.TableInformationColumnCloneFromId},
    {SqlNames.TableInformationColumnSiblingOrder}
) VALUES (
    @{SqlNames.TableInformationColumnId},
    @{SqlNames.TableInformationColumnDescription},
    @{SqlNames.TableInformationColumnContent},
    @{SqlNames.TableInformationColumnContentType},
    @{SqlNames.TableInformationColumnParentId},
    @{SqlNames.TableInformationColumnParentRelation},
    @{SqlNames.TableInformationColumnCloneFromId},
    @{SqlNames.TableInformationColumnSiblingOrder}
);
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationRaw));
            command.Parameters[SqlNames.TableInformationColumnId].Value = id.ToDatabaseText();
            command.ExecuteNonQuery();

            return id;
        }

        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="informationRaw">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        public bool Update(IInformationRaw informationRaw)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (informationRaw == null) return false;

            var commandText = $@"
UPDATE {SqlNames.TableInformation} SET
    {SqlNames.TableInformationColumnDescription} = @{SqlNames.TableInformationColumnDescription},
    {SqlNames.TableInformationColumnContent} = @{SqlNames.TableInformationColumnContent},
    {SqlNames.TableInformationColumnContentType} = @{SqlNames.TableInformationColumnContentType},
    {SqlNames.TableInformationColumnParentId} = @{SqlNames.TableInformationColumnParentId},
    {SqlNames.TableInformationColumnParentRelation} = @{SqlNames.TableInformationColumnParentRelation},
    {SqlNames.TableInformationColumnCloneFromId} = @{SqlNames.TableInformationColumnCloneFromId},
    {SqlNames.TableInformationColumnSiblingOrder} = @{SqlNames.TableInformationColumnSiblingOrder}
WHERE
    {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId};
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationRaw));
            return command.ExecuteNonQuery() > 0;
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
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (informationId == Guid.Empty) return false;

            var commandText = $@"
DELETE FROM {SqlNames.TableInformation}
WHERE
    {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId};
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId));
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Verifica se tem filhos.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Resposta de existência.</returns>
        public bool HasChildren(Guid informationId) =>
            HasRecords(SqlNames.TableInformationColumnParentId, informationId);

        /// <summary>
        /// Retorna a lista de filhos imediatos.
        /// Não é recursivo.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        public IEnumerable<Guid> Children(Guid informationId) =>
            Records(SqlNames.TableInformationColumnParentId, informationId);

        /// <summary>
        /// Verifica se tem clones.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Resposta de existência.</returns>
        public bool HasClonesTo(Guid informationId) =>
            HasRecords(SqlNames.TableInformationColumnCloneFromId, informationId);

        /// <summary>
        /// Retorna a lista de clones.
        /// Não é recursivo.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        public IEnumerable<Guid> ClonesTo(Guid informationId) =>
            Records(SqlNames.TableInformationColumnCloneFromId, informationId);

        /// <summary>
        /// Verifica se tem registros.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="column">Coluna de verificação.</param>
        /// <param name="informationId">Id</param>
        /// <returns>Resposta de existência.</returns>
        private bool HasRecords(string column, Guid informationId)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (informationId == Guid.Empty) return false;
            
            var commandText = $@"
SELECT {SqlNames.TableInformationColumnId}
  FROM {SqlNames.TableInformation}
 WHERE {column} = @{SqlNames.TableInformationColumnId}
 LIMIT 1;
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId));
            return command.ExecuteScalar() != null;
        }

        /// <summary>
        /// Retorna a lista de clones.
        /// Não é recursivo.
        /// </summary>
        /// <param name="column">Coluna de verificação.</param>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        private IEnumerable<Guid> Records(string column, Guid informationId)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (informationId == Guid.Empty) yield break;

            var commandText = $@"
SELECT {SqlNames.TableInformationColumnId}
  FROM {SqlNames.TableInformation}
 WHERE {column} = @{SqlNames.TableInformationColumnId};
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId));
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return reader.GetValue(0).ToGuid();
        }
    }
}