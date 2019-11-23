using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Strall.Exceptions;
using Strall.Persistence.Sql;

namespace Strall.Persistence.SqlServer
{
    /// <summary>
    /// Provê os meios de gravação das informações.
    /// Banco de dados SQL Server.
    /// </summary>
    public class PersistenceProviderSqlServer: PersistenceProviderSql<ISqlServerConnectionInfo>, IPersistenceProviderSqlServer
    {
        /// <summary>
        /// Valor para propriedade ConnectionSpecialized.
        /// </summary>
        private SqlConnection? _connection;

        /// <summary>
        /// Conexão com o SQLite.
        /// </summary>
        SqlConnection? IPersistenceProviderSqlServer.Connection 
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
            new SqlParameter
            {
                ParameterName = parameterName,
                DbType = type,
                Value = value
            };

        /// <summary>
        /// Cria a estrutura do banco de dados.
        /// </summary>
        public override IPersistenceProvider<ISqlServerConnectionInfo> CreateStructure()
        {
            var commandText = $@"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{SqlNames.TableInformation}' AND xtype='U') BEGIN
    CREATE TABLE {SqlNames.TableInformation} (
        {SqlNames.TableInformationColumnId} UNIQUEIDENTIFIER,
        {SqlNames.TableInformationColumnDescription} NVARCHAR(MAX),
        {SqlNames.TableInformationColumnContent} NVARCHAR(MAX),
        {SqlNames.TableInformationColumnContentType} NVARCHAR(MAX),
        {SqlNames.TableInformationColumnContentFromId} UNIQUEIDENTIFIER,
        {SqlNames.TableInformationColumnParentId} UNIQUEIDENTIFIER,
        {SqlNames.TableInformationColumnParentRelation} NVARCHAR(MAX),
        {SqlNames.TableInformationColumnSiblingOrder} INTEGER NOT NULL DEFAULT 0,
        PRIMARY KEY ({SqlNames.TableInformationColumnId}), 
        FOREIGN KEY ({SqlNames.TableInformationColumnContentFromId})
            REFERENCES {SqlNames.TableInformation} ({SqlNames.TableInformationColumnId}),
        FOREIGN KEY ({SqlNames.TableInformationColumnParentId})
            REFERENCES {SqlNames.TableInformation} ({SqlNames.TableInformationColumnId})
    );

    CREATE INDEX IDX_{SqlNames.TableInformation}_{SqlNames.TableInformationColumnSiblingOrder}
        ON {SqlNames.TableInformation} (
            {SqlNames.TableInformationColumnParentId},
            {SqlNames.TableInformationColumnSiblingOrder}
        );
END
";
                
            using var command = ConnectionOpened.CreateCommand();
            command.CommandText = commandText;
            command.ExecuteNonQuery();

            return this;
        }    
        
        /// <summary>
        /// Inicia a conexão.
        /// </summary>
        /// <param name="sqlServerConnectionInfo">Informações para conexão.</param>
        public override IPersistenceProvider<ISqlServerConnectionInfo> Open(ISqlServerConnectionInfo sqlServerConnectionInfo)
        {
            if (_connection != null) throw new StrallConnectionIsOpenException();

            var createStructure = false;
            _connection = new SqlConnection(sqlServerConnectionInfo.ConnectionString);
            try
            {
                _connection.Open();
            }
            catch
            {
                if (sqlServerConnectionInfo.CreateDatabaseIfNotExists)
                {
                    createStructure = sqlServerConnectionInfo.CreateDatabase();
                    if (createStructure) _connection.Open();
                }
            }

            if (createStructure) CreateStructure();
            
            Mode = PersistenceProviderMode.Opened;

            return this;
        }

        /// <summary>
        /// Fecha a conexão.
        /// </summary>
        public override IPersistenceProvider<ISqlServerConnectionInfo> Close()
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