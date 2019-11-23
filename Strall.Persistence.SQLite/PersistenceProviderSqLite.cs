using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Strall.Exceptions;
using Strall.Persistence.Sql;

namespace Strall.Persistence.SQLite
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQLite.
    /// </summary>
    public class PersistenceProviderSqLite: PersistenceProviderSql<IConnectionInfo>, IPersistenceProviderSqLite
    {
        /// <summary>
        /// Valor para propriedade ConnectionSpecialized.
        /// </summary>
        private SqliteConnection? _connection;

        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        SqliteConnection? IPersistenceProviderSqLite.Connection 
            => _connection;
        
        /// <summary>
        /// Conexão com o banco de dados.
        /// </summary>
        public override DbConnection? Connection =>
            _connection;

        /// <summary>
        /// Cria um parâmetro para comando SQL.
        /// </summary>
        /// <param name="parameterName">Nome do parâmetro.</param>
        /// <param name="type">Tipo de dados.</param>
        /// <param name="value">Valor do parâmetro.</param>
        /// <returns>Instância do parâmetro.</returns>
        protected override DbParameter CreateParameter(string parameterName, DbType type, object value) =>
            new SqliteParameter
            {
                ParameterName = parameterName,
                DbType = type,
                Value = value
            };

        /// <summary>
        /// Cria a estrutura do banco de dados.
        /// </summary>
        public override IPersistenceProvider<IConnectionInfo> CreateStructure()
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
                
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();

            return this;
        }    
        
        /// <summary>
        /// Inicia a conexão.
        /// </summary>
        /// <param name="connectionInfo">Informações para conexão.</param>
        public override IPersistenceProvider<IConnectionInfo> Open(IConnectionInfo connectionInfo)
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
        public override IPersistenceProvider<IConnectionInfo> Close()
        {
            if (ConnectionOpened != null) Dispose();
            return this;
        }

        /// <summary>
        /// Liberação tipo IDispose.
        /// </summary>
        public override void Dispose()
        {
            if (_connection == null) return;
            _connection.Close();
            _connection.Dispose();
            _connection = null;
            Mode = PersistenceProviderMode.Closed;
        }
    }
}