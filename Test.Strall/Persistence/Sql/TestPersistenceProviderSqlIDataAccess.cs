using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Strall.Persistence.SqLite;
using Xunit;

namespace Strall.Persistence.Sql
{
    public class TestPersistenceProviderSqlIDataAccess: IDisposable
    {
        /// <summary>
        /// Sistema sob teste (SUT = System Under Test)
        /// </summary>
        private readonly IDataAccess _sut;

        /// <summary>
        /// Arquivo do banco de dados para este teste.
        /// </summary>
        private const string Database = "TestPersistenceProviderSqLiteIDataAccess";

        /// <summary>
        /// Setup do teste.
        /// </summary>
        public TestPersistenceProviderSqlIDataAccess()
        {
            // Qualquer persistência para testar a interface.
            _sut = new PersistenceProviderSqLite();
            ((PersistenceProviderSqLite)_sut).Open(new SqLiteConnectionInfo { Filename = Path.Combine(Environment.CurrentDirectory, Database) });
        }

        /// <summary>
        /// Liberação de recursos.
        /// </summary>
        public void Dispose() => ((PersistenceProviderSqLite)_sut)?.Close();

        [Fact]
        public void para_manipular_dados_a_conexão_com_o_banco_de_dados_deve_estar_aberta()
        {
            // Arrange, Given

            var persistence = new PersistenceProviderSqLite() as IDataAccess;
            
            // Act, When

            var métodosParaException = new List<Action>
            {
                () => persistence.Exists(Guid.NewGuid()),
                () => persistence.Get(Guid.NewGuid()),
                () => persistence.Create(new Information()),
                () => persistence.Update(new Information()),
                () => persistence.Delete(Guid.NewGuid()),
                () => persistence.HasContentTo(Guid.NewGuid()),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => persistence.ContentTo(Guid.NewGuid()).ToList(),
                () => persistence.HasChildren(Guid.NewGuid()),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => persistence.Children(Guid.NewGuid()).ToList()
            };

            // Assert, Then

            foreach (var método in métodosParaException)
            {
                método.Should().ThrowExactly<StrallConnectionIsCloseException>();
            }
        }
        
        [Fact]
        public void para_manipulações_conhecidas_por_não_chegar_no_banco_de_dados_não_deve_gerar_exception()
        {
            // Arrange, Given

            var persistence = new PersistenceProviderSqLite() as IDataAccess;
            
            // Act, When
            
            var métodosParaNãoException = new List<Action>
            {
                () => persistence.Exists(Guid.Empty),
                () => persistence.Get(Guid.Empty),
                () => persistence.Delete(Guid.Empty),
                () => persistence.HasContentTo(Guid.Empty),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => persistence.ContentTo(Guid.Empty).ToList(),
                () => persistence.HasChildren(Guid.Empty),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => persistence.Children(Guid.Empty).ToList()
            };

            // Assert, Then

            foreach (var método in métodosParaNãoException)
            {
                método.Should().NotThrow();
            }
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Exists()
        {
            // Arrange, Given

            var informaçãoIndefinida = new Information{ Id = Guid.Empty };
            
            var informaçãoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoQueExiste = new Information();
            informaçãoQueExiste.Id = _sut.Create(informaçãoQueExiste);
            
            // Act, When

            var resultadoParaInformaçãoIndefinida = _sut.Exists(informaçãoIndefinida.Id);
            var resultadoParaInformaçãoQueNãoExiste = _sut.Exists(informaçãoQueNãoExiste.Id);
            var resultadoParaInformaçãoQueExiste = _sut.Exists(informaçãoQueExiste.Id);

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().BeFalse();
            resultadoParaInformaçãoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoQueExiste.Should().BeTrue();
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Get()
        {
            // Arrange, Given

            var informaçãoIndefinida = new Information{ Id = Guid.Empty };
            
            var informaçãoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoQueExiste = new Information();
            informaçãoQueExiste.Id = _sut.Create(informaçãoQueExiste);
            
            // Act, When

            var resultadoParaInformaçãoIndefinida = _sut.Get(informaçãoIndefinida.Id);
            var resultadoParaInformaçãoQueNãoExiste = _sut.Get(informaçãoQueNãoExiste.Id);
            var resultadoParaInformaçãoQueExiste = _sut.Get(informaçãoQueExiste.Id);

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().BeNull();
            resultadoParaInformaçãoQueNãoExiste.Should().BeNull();
            resultadoParaInformaçãoQueExiste.Should().NotBeNull().And
                .Subject.As<IInformation>().Id.Should().Be(informaçãoQueExiste.Id);
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Create()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformado = new Information{ Id = Guid.NewGuid() };
            
            // Act, When

            var resultadoParaInformaçãoNula = _sut.Create(null);
            var resultadoParaInformaçãoComIdVazio = _sut.Create(informaçãoComIdVazio);
            var resultadoParaInformaçãoComIdInformado = _sut.Create(informaçãoComIdInformado);
            Action criarDuplicado = () => _sut.Create(informaçãoComIdInformado);

            // Assert, Then

            resultadoParaInformaçãoNula.Should().BeEmpty();
            resultadoParaInformaçãoComIdVazio.Should().NotBeEmpty();
            resultadoParaInformaçãoComIdInformado.Should().NotBeEmpty().And
                .Subject.Should().Be(informaçãoComIdInformado.Id);
            criarDuplicado.Should().Throw<DbException>()
                .Which.Message.Should().Contain("UNIQUE");
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Update()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            // Act, When

            var resultadoParaInformaçãoNula = _sut.Update(null);
            var resultadoParaInformaçãoComIdVazio = _sut.Update(informaçãoComIdVazio);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.Update(informaçãoComIdInformadoQueNãoExiste);
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.Update(informaçãoComIdInformadoQueExiste);

            // Assert, Then

            resultadoParaInformaçãoNula.Should().BeFalse();
            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeTrue();
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Delete()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = _sut.Delete(informaçãoComIdVazio.Id);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.Delete(informaçãoComIdInformadoQueNãoExiste.Id);
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.Delete(informaçãoComIdInformadoQueExiste.Id);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeTrue();
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_HasChildren()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.FixtureMany<Information>();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = Guid.Empty;
                filho.ParentId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                _sut.Create(filho);
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = _sut.HasChildren(informaçãoComIdVazio.Id);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.HasChildren(informaçãoComIdInformadoQueNãoExiste.Id);
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.HasChildren(informaçãoComIdInformadoQueExiste.Id);
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = _sut.HasChildren(informaçãoComIdInformadoQueExisteComFilhos.Id);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().BeTrue();
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Children()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.FixtureMany<Information>().ToList();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = Guid.Empty;
                filho.ParentId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                _sut.Create(filho);
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = _sut.Children(informaçãoComIdVazio.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.Children(informaçãoComIdInformadoQueNãoExiste.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.Children(informaçãoComIdInformadoQueExiste.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = _sut.Children(informaçãoComIdInformadoQueExisteComFilhos.Id).ToList();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().HaveCount(filhos.Count).And
                .Subject.Should().BeEquivalentTo(filhos.Select(a => a.Id));
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_HasClones()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.FixtureMany<Information>();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.ParentId = Guid.Empty;
                _sut.Create(filho);
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = _sut.HasContentTo(informaçãoComIdVazio.Id);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.HasContentTo(informaçãoComIdInformadoQueNãoExiste.Id);
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.HasContentTo(informaçãoComIdInformadoQueExiste.Id);
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = _sut.HasContentTo(informaçãoComIdInformadoQueExisteComFilhos.Id);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().BeTrue();
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_ContentTo()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.FixtureMany<Information>().ToList();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.ParentId = Guid.Empty;
                _sut.Create(filho);
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = _sut.ContentTo(informaçãoComIdVazio.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.ContentTo(informaçãoComIdInformadoQueNãoExiste.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.ContentTo(informaçãoComIdInformadoQueExiste.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = _sut.ContentTo(informaçãoComIdInformadoQueExisteComFilhos.Id).ToList();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().HaveCount(filhos.Count).And
                .Subject.Should().BeEquivalentTo(filhos.Select(a => a.Id));
        }

        [Fact]
        public void verifica_funcionamento_do_método_ContentFrom()
        {
            // Arrange, Given

            var informações = new List<IInformation>();
            for (var i = 0; i < 3; i++)
            {
                var informação = new Information
                {
                    Id = Guid.NewGuid(),
                    ContentFromId = informações.LastOrDefault()?.Id ?? Guid.Empty
                };
                _sut.Create(informação);
                informações.Add(informação);
            }

            var informaçãoEmLoop = new Information {Id = Guid.NewGuid()};
            informaçãoEmLoop.ContentFromId = informaçãoEmLoop.Id;
            _sut.Create(informaçãoEmLoop);
            
            // Act, When

            var informaçãoVazia = _sut.ContentFrom(Guid.Empty);
            var origemDoPrimeiro = _sut.ContentFrom(informações.First().Id);
            var origemDoÚltimo = _sut.ContentFrom(informações.Last().Id);
            var origemDoLoop = _sut.ContentFrom(informaçãoEmLoop.Id);

            // Assert, Then

            informaçãoVazia.Should().BeEmpty();
            origemDoPrimeiro.Should().Be(informações.First().Id);
            origemDoÚltimo.Should().Be(informações.First().Id);
            origemDoLoop.Should().BeEmpty();
        }

        [Fact]
        public void ao_localizar_a_origem_do_clone_retorna_vazio_se_a_sequência_de_apontamentos_dos_clones_estiver_quebrada()
        {
            // Arrange, Given

            void RemoverConstraintsDoBancoDeDados(IPersistenceProviderSqLite persistence)
            {
                using var command = persistence.Connection.CreateCommand();
                command.CommandText =
                    $"SELECT sql FROM sqlite_master WHERE name='{persistence.SqlNames.TableInformation}';";
                var sql = (string) command.ExecuteScalar();
                sql = $"DROP TABLE {persistence.SqlNames.TableInformation}; " +
                      Regex.Replace(sql, @",\s*?FOREIGN.*(?=\))", string.Empty,
                          RegexOptions.Singleline | RegexOptions.IgnoreCase);
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            
            var persistenceProviderSqLite =
                (IPersistenceProviderSqLite)
                new PersistenceProviderSqLite()
                    .Open(
                        new SqLiteConnectionInfo
                        {
                            Filename = Path.Combine(Environment.CurrentDirectory, this.Fixture<string>())
                        });

            RemoverConstraintsDoBancoDeDados(persistenceProviderSqLite);

            var informações = new List<IInformation>();
            for (var i = 0; i < 3; i++)
            {
                var informação = new Information
                {
                    Id = Guid.NewGuid(),
                    ContentFromId = informações.LastOrDefault()?.Id ?? Guid.NewGuid()
                };
                informações.Add(informação);
                persistenceProviderSqLite.Create(informação);
            }
            
            // Act, When

            var origemDoClone = persistenceProviderSqLite.ContentFrom(informações.Last().Id);

            // Assert, Then

            origemDoClone.Should().BeEmpty();
        }
    }
}