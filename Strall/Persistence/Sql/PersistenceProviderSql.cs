using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Cabrones.Utils.Converter;
using Strall.Exceptions;

namespace Strall.Persistence.Sql
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados tipo SQL.
    /// </summary>
    public abstract class PersistenceProviderSql<TConnectionInfo>: IPersistenceProviderSql, IPersistenceProvider<TConnectionInfo>
        where TConnectionInfo: IConnectionInfo
    {
        /// <summary>
        /// Conexão com o banco de dados.
        /// </summary>
        public abstract DbConnection? Connection { get; }
        
        /// <summary>
        /// Conexão com o banco de dados já aberta.
        /// Do contrário gera exception.
        /// </summary>
        protected DbConnection ConnectionOpened => 
            Connection ?? throw new StrallConnectionIsCloseException();
        
        /// <summary>
        /// Nomes no contexto do SQL.
        /// </summary>
        public ISqlNames SqlNames { get; set; } = new SqlNames() as ISqlNames;

        /// <summary>
        /// Cria a estrutura do banco de dados.
        /// </summary>
        public abstract IPersistenceProvider<TConnectionInfo> CreateStructure();
        
        /// <summary>
        /// Inicia a conexão.
        /// </summary>
        /// <param name="connectionInfo">Informações para conexão.</param>
        public abstract IPersistenceProvider<TConnectionInfo> Open(TConnectionInfo connectionInfo);

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        public abstract IPersistenceProvider<TConnectionInfo> Close();

        /// <summary>
        /// Modo atual.
        /// </summary>
        public PersistenceProviderMode Mode { get; protected set; } = PersistenceProviderMode.Closed;

        /// <summary>
        /// Liberação tipo IDispose.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Cria um parâmetro para comando SQL.
        /// </summary>
        /// <param name="parameterName">Nome do parâmetro.</param>
        /// <param name="type">Tipo de dados.</param>
        /// <param name="value">Valor do parâmetro.</param>
        /// <returns>Instância do parâmetro.</returns>
        protected abstract DbParameter CreateParameter(string parameterName, DbType type, object value);

        /// <summary>
        /// Monta uma lista de parâmetros.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Lista de parâmetros.</returns>
        private IEnumerable<DbParameter> FactoryParameters(IInformation information) =>
            new List<DbParameter>
            {
                CreateParameter(
                    SqlNames.TableInformationColumnId,
                    DbType.String,
                    information.Id.ToDatabaseText()
                ),
                CreateParameter(
                    SqlNames.TableInformationColumnDescription,
                    DbType.String,
                    information.Description
                ),
                CreateParameter(
                    SqlNames.TableInformationColumnContent,
                    DbType.String,
                    information.Content
                ),
                CreateParameter(
                    SqlNames.TableInformationColumnContentType,
                    DbType.String,
                    information.ContentType.ToString()
                ),
                CreateParameter(
                    SqlNames.TableInformationColumnContentFromId,
                    DbType.String,
                    information.ContentFromId.ToDatabaseText()
                ),
                CreateParameter(
                    SqlNames.TableInformationColumnParentId,
                    DbType.String,
                    information.ParentId.ToDatabaseText()
                ),
                CreateParameter(
                    SqlNames.TableInformationColumnParentRelation,
                    DbType.String,
                    information.ParentRelation
                ),
                CreateParameter(
                    SqlNames.TableInformationColumnSiblingOrder,
                    DbType.Int32,
                    information.SiblingOrder
                )
            };

        /// <summary>
        /// Monta uma lista de parâmetros.
        /// </summary>
        /// <param name="informationId">Id</param>
        /// <returns>Lista de parâmetros.</returns>
        private IEnumerable<DbParameter> FactoryParameters(Guid informationId) =>
            new List<DbParameter>
            {
                CreateParameter(
                    SqlNames.TableInformationColumnId,
                    DbType.String,
                    informationId.ToDatabaseText()
                )
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
 WHERE {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId};
";
            
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId).ToArray());
            return command.ExecuteScalar() != null;
        }

        /// <summary>
        /// Obtem uma informação.
        /// </summary>
        /// <param name="informationId"></param>
        /// <returns>Informação.</returns>
        public IInformation? Get(Guid informationId)
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
 WHERE {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId};
";
            
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId).ToArray());
            using var reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            var i = -1;
            return new Information
            {
                Id = reader.GetValue(++i).ToGuid(),
                Description = $"{reader.GetValue(++i)}",
                Content = $"{reader.GetValue(++i)}",
                ContentType =
                    Enum.TryParse<InformationType>($"{reader.GetValue(++i)}", true, out var contentType)
                        ? contentType
                        : InformationType.Text,
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
        /// <param name="information">Informação.</param>
        /// <returns>Id.</returns>
        public Guid Create(IInformation information)
        {
            if (information == null) return Guid.Empty;
            
            var id = information.Id != Guid.Empty ? information.Id : Guid.NewGuid();
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
            
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(information).ToArray());
            command.Parameters[SqlNames.TableInformationColumnId].Value = id.ToDatabaseText();
            command.ExecuteNonQuery();

            return id;
        }

        /// <summary>
        /// Atualiza uma informação.
        /// Equivalente a UPDATE.
        /// </summary>
        /// <param name="information">Informação.</param>
        /// <returns>Resposta de sucesso.</returns>
        public bool Update(IInformation information)
        {
            if (information == null) return false;

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
            
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(information).ToArray());
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
            
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId).ToArray());
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Apaga uma informação.
        /// Equivalente a DELETE.
        /// É recursivo para seus filhos.
        /// </summary>
        /// <param name="informationId">Id.</param>
        /// <returns>Total de registros apagados.</returns>
        public int DeleteAll(Guid informationId)
        {
            if (informationId == Guid.Empty) return 0;

            IEnumerable<KeyValuePair<Guid, int>> GetListToDelete(Guid id)
            {
                var commandText = $@"
SELECT {SqlNames.TableInformationColumnId}
  FROM {SqlNames.TableInformation}
 WHERE {SqlNames.TableInformationColumnParentId} = @{SqlNames.TableInformationColumnId};
";

                var index = 0;
                var result = new Dictionary<Guid, int> {[id] = index};
                do
                {
                    using var command = ConnectionOpened.CreateCommand();
                    command.CommandText = commandText;
                    command.Parameters.AddRange(FactoryParameters(result.ElementAt(index++).Key).ToArray());
                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var readId = reader.GetValue(0).ToGuid();
                        if (readId != Guid.Empty && !result.ContainsKey(readId)) result.Add(readId, index);
                    }
                } while (index < result.Count);

                return result;
            }

            string SqlWhere(string fieldName, ICollection<Guid> ids)
            {
                var statementIn = new List<string>();
                const int inOperatorLimite = 1000;
                var page = 0;
                while (page * inOperatorLimite < ids.Count)
                {
                    statementIn.Add($"{fieldName} IN (" + ids.Skip(inOperatorLimite * page++).Take(inOperatorLimite).Aggregate("", 
                        (acc, cur) => acc == "" ? $"'{cur.ToDatabaseText()}'" : $"{acc}, '{cur.ToDatabaseText()}'") + ")");
                }

                var result = statementIn.Aggregate("", 
                    (acc, cur) => acc == "" ? cur : $"{acc} OR {cur}");

                return result;
            }

            int Delete(IEnumerable<Guid> ids)
            {
                var commandText = $@"
DELETE 
  FROM {SqlNames.TableInformation} 
 WHERE {SqlWhere(SqlNames.TableInformationColumnId, ids.ToList())};
";

                using var command = ConnectionOpened.CreateCommand();
                command.CommandText = commandText;
                return command.ExecuteNonQuery();
            }

            return GetListToDelete(informationId)
                .GroupBy(a => a.Value)
                .OrderByDescending(a => a.Key)
                .Select(a => a.Select(b => b.Key).ToList())
                .Sum(Delete);
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
 WHERE {SqlNames.TableInformationColumnId} = @{SqlNames.TableInformationColumnId};
";

            var chain = new List<Guid> { informationId };
            do
            {
                using var command = ConnectionOpened.CreateCommand();
                command.CommandText = commandText;
                command.Parameters.AddRange(FactoryParameters(informationId).ToArray());
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
SELECT COUNT({SqlNames.TableInformationColumnId})
  FROM {SqlNames.TableInformation}
 WHERE {column} = @{SqlNames.TableInformationColumnId};
";
            
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId).ToArray());
            return (long)command.ExecuteScalar() > 0;
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
            
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.Parameters.AddRange(FactoryParameters(informationId).ToArray());
            using var reader = command.ExecuteReader();
            while (reader.Read()) yield return reader.GetValue(0).ToGuid();
        }
    }
}