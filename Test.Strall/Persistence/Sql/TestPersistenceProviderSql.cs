using System;
using System.IO;
using Cabrones.Test;
using FluentAssertions;
using NSubstitute;
using Strall.Persistence.SqLite;
using Xunit;

namespace Strall.Persistence.Sql
{
    public class TestPersistenceProviderSql
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(PersistenceProviderSql<ConnectionInfo>);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IPersistenceProviderSql), typeof(IPersistenceProvider<ConnectionInfo>), typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations(typeof(IPersistenceProviderSql), typeof(IPersistenceProvider<ConnectionInfo>));
            sut.AssertMyOwnPublicPropertiesCount(0);
            sut.AssertMyOwnPublicMethodsCount(0);
        }

        [Fact]
        public void deve_ser_possível_substituir_o_ISqlNames_para_alterar_os_nomes_dos_objetos_do_banco_de_dados()
        {
            // Arrange, Given

            var persistenceProviderSql = new PersistenceProviderSqLite() as IPersistenceProviderSql;
            var connectionInfo = new SqLiteConnectionInfo
            {
                Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>()),
                CreateDatabaseIfNotExists = true
            } as ISqLiteConnectionInfo;

            var sqlNames = Substitute.For<ISqlNames>();
            sqlNames.TableInformation.Returns($"tb_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnId.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnDescription.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnContent.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnContentType.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnContentFromId.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnParentId.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnParentRelation.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");
            sqlNames.TableInformationColumnSiblingOrder.Returns($"col_{this.Fixture<string>().Substring(0, 8)}");

            void ConectarECriarEstrutura() => ((PersistenceProviderSqLite) persistenceProviderSql).Open(connectionInfo);

            // Act, When

            persistenceProviderSql.SqlNames = sqlNames;
            ConectarECriarEstrutura();

            // Assert, Then

            using var command = persistenceProviderSql.Connection.CreateCommand();
            command.CommandText = $"SELECT sql FROM sqlite_master WHERE name = '{sqlNames.TableInformation}'";
            var ddl = (string) command.ExecuteScalar();

            persistenceProviderSql.SqlNames.Should().BeSameAs(sqlNames);
            ddl.Should().NotBeNull();
            ddl.Should().Contain(sqlNames.TableInformationColumnId);
            ddl.Should().Contain(sqlNames.TableInformationColumnDescription);
            ddl.Should().Contain(sqlNames.TableInformationColumnContent);
            ddl.Should().Contain(sqlNames.TableInformationColumnContentType);
            ddl.Should().Contain(sqlNames.TableInformationColumnContentFromId);
            ddl.Should().Contain(sqlNames.TableInformationColumnParentId);
            ddl.Should().Contain(sqlNames.TableInformationColumnParentRelation);
            ddl.Should().Contain(sqlNames.TableInformationColumnSiblingOrder);
        }
    }
}