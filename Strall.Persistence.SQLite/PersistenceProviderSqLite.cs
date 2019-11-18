using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Valor para propriedade Connection.
        /// </summary>
        private SqliteConnection? _connection;

        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        SqliteConnection? IPersistenceProviderSqLite.Connection 
            => _connection;
        
        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        private SqliteConnection Connection => 
            _connection ??  throw new StrallConnectionIsCloseException();

        /// <summary>
        /// Nomes no contexto do SQL.
        /// </summary>
        public ISqlNames SqlNames { get; set; } = new SqlNames() as ISqlNames;
        
        /// <summary>
        /// Cria a estrutura do banco de dados.
        /// </summary>
        public IPersistenceProvider<IConnectionInfo> CreateStructure()
        {
            var commandText = $@"
CREATE TABLE IF NOT EXISTS {SqlNames.TableInformation} (
    {SqlNames.TableInformationColumnId} TEXT,
    {SqlNames.TableInformationColumnDescription} TEXT,
    {SqlNames.TableInformationColumnContent} TEXT,
    {SqlNames.TableInformationColumnContentType} TEXT,
    {SqlNames.TableInformationColumnContentFromId} TEXT,
    {SqlNames.TableInformationColumnParentId} TEXT,
    {SqlNames.TableInformationColumnParentRelation} TEXT,
    {SqlNames.TableInformationColumnSiblingOrder} INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY ({SqlNames.TableInformationColumnId}), 
    FOREIGN KEY ({SqlNames.TableInformationColumnContentFromId})
        REFERENCES {SqlNames.TableInformation} ({SqlNames.TableInformationColumnId})
            ON DELETE RESTRICT
            ON UPDATE RESTRICT,
    FOREIGN KEY ({SqlNames.TableInformationColumnParentId})
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
            if (_connection != null) throw new StrallConnectionIsOpenException();
            
            var createdDatabase = connectionInfo.CreateDatabase();
            
            _connection = new SqliteConnection(connectionInfo.ConnectionString);
            _connection.Open();
            
            if (createdDatabase) CreateStructure();
            
            Mode = PersistenceProviderMode.Opened;

            return this;
        }

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        public IPersistenceProvider<IConnectionInfo> Close()
        {
            if (Connection != null) Dispose();
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
            if (_connection == null) return;
            _connection.Close();
            _connection.Dispose();
            _connection = null;
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
                    ParameterName = SqlNames.TableInformationColumnContentFromId,
                    SqliteType = SqliteType.Text,
                    Value = informationRaw.ContentFromId.ToDatabaseText()
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
            if (informationId == Guid.Empty) return null;

            var commandText = $@"
SELECT {SqlNames.TableInformationColumnId},
       {SqlNames.TableInformationColumnDescription},
       {SqlNames.TableInformationColumnContent},
       {SqlNames.TableInformationColumnContentType},
       {SqlNames.TableInformationColumnContentFromId},
       {SqlNames.TableInformationColumnParentId},
       {SqlNames.TableInformationColumnParentRelation},
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
                ContentFromId = reader.GetValue(++i).ToGuid(),
                ParentId = reader.GetValue(++i).ToGuid(),
                ParentRelation = $"{reader.GetValue(++i)}",
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
            if (informationRaw == null) return Guid.Empty;
            
            var id = informationRaw.Id != Guid.Empty ? informationRaw.Id : Guid.NewGuid();
            var commandText = $@"
INSERT INTO {SqlNames.TableInformation} (
    {SqlNames.TableInformationColumnId},
    {SqlNames.TableInformationColumnDescription},
    {SqlNames.TableInformationColumnContent},
    {SqlNames.TableInformationColumnContentType},
    {SqlNames.TableInformationColumnContentFromId},
    {SqlNames.TableInformationColumnParentId},
    {SqlNames.TableInformationColumnParentRelation},
    {SqlNames.TableInformationColumnSiblingOrder}
) VALUES (
    @{SqlNames.TableInformationColumnId},
    @{SqlNames.TableInformationColumnDescription},
    @{SqlNames.TableInformationColumnContent},
    @{SqlNames.TableInformationColumnContentType},
    @{SqlNames.TableInformationColumnContentFromId},
    @{SqlNames.TableInformationColumnParentId},
    @{SqlNames.TableInformationColumnParentRelation},
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
            if (informationRaw == null) return false;

            var commandText = $@"
UPDATE {SqlNames.TableInformation} SET
    {SqlNames.TableInformationColumnDescription} = @{SqlNames.TableInformationColumnDescription},
    {SqlNames.TableInformationColumnContent} = @{SqlNames.TableInformationColumnContent},
    {SqlNames.TableInformationColumnContentType} = @{SqlNames.TableInformationColumnContentType},
    {SqlNames.TableInformationColumnContentFromId} = @{SqlNames.TableInformationColumnContentFromId},
    {SqlNames.TableInformationColumnParentId} = @{SqlNames.TableInformationColumnParentId},
    {SqlNames.TableInformationColumnParentRelation} = @{SqlNames.TableInformationColumnParentRelation},
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
        /// Verifica se tem clones.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Resposta de existência.</returns>
        public bool HasContentTo(Guid informationId) =>
            HasRecords(SqlNames.TableInformationColumnContentFromId, informationId);

        /// <summary>
        /// Retorna a lista de clones.
        /// Não é recursivo.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista</returns>
        public IEnumerable<Guid> ContentTo(Guid informationId) =>
            Records(SqlNames.TableInformationColumnContentFromId, informationId);

        /// <summary>
        /// Localiza a origem de um clone
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>
        /// Id da origem.
        /// Caso não seja clone retorna o mesmo id.
        /// Em caso de loop ou de referência não encontrada retorna Guid.Empty.
        /// </returns>
        public Guid ContentFrom(Guid informationId)
        {
            if (informationId == Guid.Empty) return Guid.Empty;

            var commandText = $@"
SELECT {SqlNames.TableInformationColumnContentFromId}
  FROM {SqlNames.TableInformation}
 WHERE {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId}
 LIMIT 1;
";

            var chain = new List<Guid> { informationId };
            do
            {
                using var command = Connection.CreateCommand();
                command.CommandText = commandText;
                command.Parameters.AddRange(FactoryParameters(informationId));
                using var reader = command.ExecuteReader();
                
                // Referência do clone ficou perdida no meio do caminho.
                if (!reader.Read()) return Guid.Empty;

                // Lê o próximo id para chegar na origem.
                informationId = reader.GetValue(0).ToGuid();
                
                //Adiciona na lista de id já verificados.
                if (informationId != Guid.Empty) chain.Add(informationId);
                
                // Fim da linha. Origem encontrada.
                if (informationId == Guid.Empty) return chain.Last();
                    
                // Loop detectado. 
                if (chain.Count(a => a == informationId) > 1) return Guid.Empty;
            } while (true);
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
        /// Verifica se tem registros.
        /// Equivalente a SELECT TOP 1
        /// </summary>
        /// <param name="column">Coluna de verificação.</param>
        /// <param name="informationId">Id</param>
        /// <returns>Resposta de existência.</returns>
        private bool HasRecords(string column, Guid informationId)
        {
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