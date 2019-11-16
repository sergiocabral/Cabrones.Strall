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
    {SqlNames.TableInformationColumnCloneId} TEXT,
    {SqlNames.TableInformationColumnSiblingOrder} INTEGER NOT NULL DEFAULT 0,
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
        /// <param name="information">Informação.</param>
        /// <returns>Lista de parâmetros.</returns>
        private IEnumerable<SqliteParameter> FactoryParameters(IInformation information) =>
            new List<SqliteParameter>
            {
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnId,
                    SqliteType = SqliteType.Text,
                    Value = information.Id != Guid.Empty ? (object)information.Id : DBNull.Value
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnDescription,
                    SqliteType = SqliteType.Text,
                    Value = information.Description
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnContent,
                    SqliteType = SqliteType.Text,
                    Value = information.Content
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnContentType,
                    SqliteType = SqliteType.Text,
                    Value = information.ContentType
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnParentId,
                    SqliteType = SqliteType.Text,
                    Value = information.ParentId != Guid.Empty ? (object)information.ParentId : DBNull.Value
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnParentRelation,
                    SqliteType = SqliteType.Text,
                    Value = information.ParentRelation
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnCloneId,
                    SqliteType = SqliteType.Text,
                    Value = information.CloneId != Guid.Empty ? (object)information.CloneId : DBNull.Value
                },
                new SqliteParameter
                {
                    ParameterName = SqlNames.TableInformationColumnSiblingOrder,
                    SqliteType = SqliteType.Integer,
                    Value = information.SiblingOrder
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
                    Value = informationId != Guid.Empty ? (object)informationId : DBNull.Value
                }
            };

        /// <summary>
        /// Converte um valor para Guid.
        /// </summary>
        /// <param name="value">Valor.</param>
        /// <returns>Guid</returns>
        private static Guid ConvertToGuid(object value) => 
            value != null && value != DBNull.Value ? Guid.Parse($"{value}") : Guid.Empty;
        
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
        public Information? Get(Guid informationId)
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
       {SqlNames.TableInformationColumnCloneId},
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
            return new Information
            {
                Id = ConvertToGuid(reader.GetValue(++i)),
                Description = $"{reader.GetValue(++i)}",
                Content = $"{reader.GetValue(++i)}",
                ContentType = $"{reader.GetValue(++i)}",
                ParentId = ConvertToGuid(reader.GetValue(++i)),
                ParentRelation = $"{reader.GetValue(++i)}",
                CloneId = ConvertToGuid(reader.GetValue(++i)),
                SiblingOrder = reader.GetFieldValue<int>(++i)
            };
        }

        /// <summary>
        /// Cria uma informação.
        /// Equivalente a INSERT.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Id.</returns>
        public Guid Create(Information information)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (information == null) return Guid.Empty;
            
            var id = information.Id != Guid.Empty ? information.Id : Guid.NewGuid();
            var commandText = $@"
INSERT INTO {SqlNames.TableInformation} (
    {SqlNames.TableInformationColumnId},
    {SqlNames.TableInformationColumnDescription},
    {SqlNames.TableInformationColumnContent},
    {SqlNames.TableInformationColumnContentType},
    {SqlNames.TableInformationColumnParentId},
    {SqlNames.TableInformationColumnParentRelation},
    {SqlNames.TableInformationColumnCloneId},
    {SqlNames.TableInformationColumnSiblingOrder}
) VALUES (
    @{SqlNames.TableInformationColumnId},
    @{SqlNames.TableInformationColumnDescription},
    @{SqlNames.TableInformationColumnContent},
    @{SqlNames.TableInformationColumnContentType},
    @{SqlNames.TableInformationColumnParentId},
    @{SqlNames.TableInformationColumnParentRelation},
    @{SqlNames.TableInformationColumnCloneId},
    @{SqlNames.TableInformationColumnSiblingOrder}
);
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(information));
            command.Parameters[SqlNames.TableInformationColumnId].Value = id;
            command.ExecuteNonQuery();

            return id;
        }

        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        public bool Update(Information information)
        {
            if (Connection == null) throw new StrallConnectionIsCloseException();
            if (information == null) return false;

            var commandText = $@"
UPDATE {SqlNames.TableInformation} SET
    {SqlNames.TableInformationColumnDescription} = @{SqlNames.TableInformationColumnDescription},
    {SqlNames.TableInformationColumnContent} = @{SqlNames.TableInformationColumnContent},
    {SqlNames.TableInformationColumnContentType} = @{SqlNames.TableInformationColumnContentType},
    {SqlNames.TableInformationColumnParentId} = @{SqlNames.TableInformationColumnParentId},
    {SqlNames.TableInformationColumnParentRelation} = @{SqlNames.TableInformationColumnParentRelation},
    {SqlNames.TableInformationColumnCloneId} = @{SqlNames.TableInformationColumnCloneId},
    {SqlNames.TableInformationColumnSiblingOrder} = @{SqlNames.TableInformationColumnSiblingOrder}
WHERE
    {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId};
";
            
            using var command = Connection.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(information));
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
        public bool HasClones(Guid informationId) =>
            HasRecords(SqlNames.TableInformationColumnCloneId, informationId);

        /// <summary>
        /// Retorna a lista de clones.
        /// Não é recursivo.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        public IEnumerable<Guid> Clones(Guid informationId) =>
            Records(SqlNames.TableInformationColumnCloneId, informationId);

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
            while (reader.Read()) yield return ConvertToGuid(reader.GetValue(0));
        }
    }
}