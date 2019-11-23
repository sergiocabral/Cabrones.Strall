using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Strall.Persistence.SqLite;
using Xunit;
// ReSharper disable RedundantArgumentDefaultValue

namespace Strall
{
    public class TestInformationExtensionsDataAccess: IDisposable
    {
        /// <summary>
        /// Uma persistência qualquer para fazer os testes de integração
        /// </summary>
        private readonly IDataAccess _persistence;

        /// <summary>
        /// Arquivo do banco de dados para este teste.
        /// </summary>
        private const string Database = "TestInformationExtensionsDataAccess";

        /// <summary>
        /// Setup do teste.
        /// </summary>
        public TestInformationExtensionsDataAccess()
        {
            var persistence = new PersistenceProviderSqLite();
            persistence.Open(new SqLiteConnectionInfo { Filename = Path.Combine(Environment.CurrentDirectory, Database) });
            _persistence = persistence;
        }

        /// <summary>
        /// Liberação de recursos.
        /// </summary>
        public void Dispose() => ((PersistenceProviderSqLite)_persistence)?.Close();

        /// <summary>
        /// Configura o IDataAccess usado nos extensions methods.
        /// </summary>
        /// <param name="active">Ativa ou desativa o acesso ao IDataAccess</param>
        private void SetDataAccess(bool active = true)
        {
            // Como a manipulação é em uma propriedade estática um tempo de espera
            // será aplicado para evitar conflitos com outros testes.
            Thread.Sleep(active ? 100 : 3000);
            
            ((Information) null).SetDataAccess(active ? _persistence : null);
        }

        [Fact]
        public void métodos_de_manipulação_de_dados_só_devem_funcionar_com_a_conexão_com_o_banco_de_dados_aberta()
        {
            // Arrange, Given
            
            SetDataAccess(false);

            var informacao = new Information();
            
            // Act, When

            var métodos = new List<Action>
            {
                () => informacao.Exists(),
                () => informacao.Get(false),
                () => informacao.Get(true),
                () => Guid.Empty.GetInformation(true),
                () => informacao.Create(false),
                () => informacao.Create(true),
                () => informacao.Update(true),
                () => informacao.Delete(),
                () => informacao.HasContentTo(),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => informacao.ContentTo().ToList(),
                () => informacao.ContentLoad(true),
                () => informacao.ContentSave(true),
                () => informacao.HasChildren(),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => informacao.Children().ToList()
            };

            // Assert, Then

            foreach (var método in métodos)
            {
                método.Should().ThrowExactly<StrallDataAccessIsNullException>();
            }
        }

        [Fact]
        public void Exists_deve_verificar_se_uma_informação_existe()
        {
            // Arrange, Given

            SetDataAccess();
            
            var informaçãoIndefinida = new Information{ Id = Guid.Empty };
            var informaçãoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            var informaçãoQueExiste = new Information().Create();
            
            // Act, When

            var resultadoParaInformaçãoIndefinida = informaçãoIndefinida.Exists();
            var resultadoParaInformaçãoQueNãoExiste = informaçãoQueNãoExiste.Exists();
            var resultadoParaInformaçãoQueExiste = informaçãoQueExiste.Exists();

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().BeFalse();
            resultadoParaInformaçãoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoQueExiste.Should().BeTrue();
        }
        
        [Fact]
        public void Get_deve_carregar_do_banco_de_dados_os_valores_dos_campos_da_informação()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoIndefinida = new Information{ Id = Guid.Empty };
            var informaçãoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            var informaçãoQueExiste = new Information().Create();
            
            // Act, When

            var resultadoParaInformaçãoIndefinida = informaçãoIndefinida.Get(false);
            var resultadoParaInformaçãoQueNãoExiste = informaçãoQueNãoExiste.Get(false);
            var resultadoParaInformaçãoQueExiste = informaçãoQueExiste.Get(false);

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().NotBeNull();
            resultadoParaInformaçãoIndefinida.Id.Should().Be(informaçãoIndefinida.Id);
            resultadoParaInformaçãoQueNãoExiste.Should().NotBeNull();
            resultadoParaInformaçãoQueNãoExiste.Id.Should().Be(informaçãoQueNãoExiste.Id);
            resultadoParaInformaçãoQueExiste.Should().NotBeNull().And
                .Subject.As<IInformation>().Id.Should().Be(informaçãoQueExiste.Id);
        }

        [Fact]
        public void Get_deve_carregar_do_banco_de_dados_os_valores_dos_campos_da_informação_com_opção_de_sincronizar_o_conteúdo_ou_não()
        {
            // Arrange, Given
            
            SetDataAccess();

            var conteúdoDaInformaçãoOriginal = this.Fixture<string>();
            var informaçãoOriginal = new Information
            {
                Content = conteúdoDaInformaçãoOriginal
            }.Create(false);

            var conteúdoDaInformaçãoClone = this.Fixture<string>();
            var informaçãoClone = new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginal.Id
            }.Create(false);
            
            // Act, When

            var conteúdoSemSincronizar = informaçãoClone.Get(false).Content;
            var conteúdoSincronizando = informaçãoClone.Get(true).Content;

            // Assert, Then

            conteúdoSemSincronizar.Should().Be(conteúdoDaInformaçãoClone);
            conteúdoSincronizando.Should().Be(conteúdoDaInformaçãoOriginal);
        }
        
        [Fact]
        public void GetInformation_deve_consultar_no_banco_de_dados_uma_informação()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoIndefinida = Guid.Empty;
            var informaçãoQueNãoExiste = Guid.NewGuid();
            var informaçãoQueExiste = new Information().Create(false).Id;
            
            // Act, When

            var resultadoParaInformaçãoIndefinida = informaçãoIndefinida.GetInformation(false);
            var resultadoParaInformaçãoQueNãoExiste = informaçãoQueNãoExiste.GetInformation(false);
            var resultadoParaInformaçãoQueExiste = informaçãoQueExiste.GetInformation(false);

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().BeNull();
            resultadoParaInformaçãoQueNãoExiste.Should().BeNull();
            resultadoParaInformaçãoQueExiste.Should().NotBeNull().And
                .Subject.As<IInformation>().Id.Should().Be(informaçãoQueExiste);
        }

        [Fact]
        public void GetInformation_deve_consultar_no_banco_de_dados_uma_informação_com_opção_de_sincronizar_o_conteúdo_ou_não()
        {
            // Arrange, Given
            
            SetDataAccess();

            var conteúdoDaInformaçãoOriginal = this.Fixture<string>();
            var informaçãoOriginal = new Information
            {
                Content = conteúdoDaInformaçãoOriginal
            }.Create(false);

            var conteúdoDaInformaçãoClone = this.Fixture<string>();
            var informaçãoClone = new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginal.Id
            }.Create(false);
            
            // Act, When

            var conteúdoSemSincronizar = informaçãoClone.Id.GetInformation(false)?.Content;
            var conteúdoSincronizando = informaçãoClone.Id.GetInformation(true)?.Content;

            // Assert, Then

            conteúdoSemSincronizar.Should().Be(conteúdoDaInformaçãoClone);
            conteúdoSincronizando.Should().Be(conteúdoDaInformaçãoOriginal);
        }

        [Fact]
        public void Create_deve_criar_uma_informação()
        {
            // Arrange, Given

            SetDataAccess();
            var persistence = (IPersistenceProviderSqLite) _persistence;
            
            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformado = new Information{ Id = Guid.NewGuid() };
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Create(false);
            var resultadoParaInformaçãoComIdInformado = informaçãoComIdInformado.Create(false);
            Action criarDuplicado = () => informaçãoComIdInformado.Create(false);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().NotBeNull();
            resultadoParaInformaçãoComIdVazio.Id.Should().NotBeEmpty();
            resultadoParaInformaçãoComIdInformado.Should().NotBeNull();
            resultadoParaInformaçãoComIdInformado.Id.Should().NotBeEmpty().And
                .Subject.Should().Be(informaçãoComIdInformado.Id);
            criarDuplicado.Should().ThrowExactly<Microsoft.Data.Sqlite.SqliteException>()
                .Which.Message.Should().Be($"SQLite Error 19: 'UNIQUE constraint failed: {persistence.SqlNames.TableInformation}.{persistence.SqlNames.TableInformationColumnId}'.");
        }

        [Fact]
        public void Create_deve_criar_uma_informação_com_opção_de_sincronizar_o_conteúdo_ou_não()
        {
            // Arrange, Given
            
            SetDataAccess();

            var conteúdoDaInformaçãoOriginal = this.Fixture<string>();
            var informaçãoOriginal = new Information
            {
                Content = conteúdoDaInformaçãoOriginal
            }.Create(false);

            var conteúdoDaInformaçãoClone = this.Fixture<string>();
            
            // Act, When

            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginal.Id
            }.Create(false);
            var conteúdoOriginalSemSincronizar = informaçãoOriginal.Id.GetInformation(false).Content;
            
            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginal.Id
            }.Create(true);
            var conteúdoOriginalSincronizando = informaçãoOriginal.Id.GetInformation(false).Content;

            // Assert, Then

            conteúdoOriginalSemSincronizar.Should().Be(conteúdoDaInformaçãoOriginal);
            conteúdoOriginalSincronizando.Should().Be(conteúdoDaInformaçãoClone);
        }
        
        [Fact]
        public void Update_deve_atualizar_uma_informação()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Update(false);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.Update(false);
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.Update(false);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeTrue();
        }

        [Fact]
        public void Update_deve_atualizar_uma_informação_com_opção_de_sincronizar_o_conteúdo_ou_não()
        {
            // Arrange, Given
            
            SetDataAccess();

            var conteúdoDaInformaçãoOriginal = this.Fixture<string>();
            var informaçãoOriginal = new Information
            {
                Content = conteúdoDaInformaçãoOriginal
            }.Create(false);

            var conteúdoDaInformaçãoClone = this.Fixture<string>();
            
            // Act, When

            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginal.Id
            }.Create(false).Update(false);
            var conteúdoOriginalSemSincronizar = informaçãoOriginal.Id.GetInformation(false).Content;
            
            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginal.Id
            }.Create(false).Update(true);
            var conteúdoOriginalSincronizando = informaçãoOriginal.Id.GetInformation(false).Content;

            // Assert, Then

            conteúdoOriginalSemSincronizar.Should().Be(conteúdoDaInformaçãoOriginal);
            conteúdoOriginalSincronizando.Should().Be(conteúdoDaInformaçãoClone);
        }
        
        [Fact]
        public void UpdateOrCreate_deve_atualizar_uma_informação_mas_se_não_existir_deve_criar()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazioId = Guid.Empty;
            var informaçãoComIdVazio = new Information{ Id = informaçãoComIdVazioId };

            var informaçãoComIdInformadoQueNãoExisteId = Guid.NewGuid();
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = informaçãoComIdInformadoQueNãoExisteId };

            var informaçãoComIdInformadoQueExisteId = Guid.NewGuid();
            var informaçãoComIdInformadoQueExiste = new Information{ Id = informaçãoComIdInformadoQueExisteId }.Create();
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.UpdateOrCreate(false);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.UpdateOrCreate(false);
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.UpdateOrCreate(false);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().NotBeNull();
            resultadoParaInformaçãoComIdVazio.Id.Should().NotBeEmpty();
            resultadoParaInformaçãoComIdVazio.Id.Should().NotBe(informaçãoComIdVazioId);
            
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().NotBeNull();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Id.Should().NotBeEmpty();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Id.Should().Be(informaçãoComIdInformadoQueNãoExisteId);

            resultadoParaInformaçãoComIdInformadoQueExiste.Should().NotBeNull();
            resultadoParaInformaçãoComIdInformadoQueExiste.Id.Should().NotBeEmpty();
            resultadoParaInformaçãoComIdInformadoQueExiste.Id.Should().Be(informaçãoComIdInformadoQueExisteId);
        }

        [Fact]
        public void UpdateOrCreate_deve_atualizar_uma_informação_mas_se_não_existir_deve_criar_com_opção_de_sincronizar_o_conteúdo_ou_não()
        {
            // Arrange, Given
            
            SetDataAccess();

            var conteúdoDaInformaçãoOriginal = this.Fixture<string>();
            var informaçãoOriginalParaUpdate = new Information
            {
                Content = conteúdoDaInformaçãoOriginal
            }.Create(false);
            var informaçãoOriginalParaCreate = new Information
            {
                Content = conteúdoDaInformaçãoOriginal
            }.Create(false);

            var conteúdoDaInformaçãoClone = this.Fixture<string>();
            
            // Act, When

            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginalParaCreate.Id
            }.UpdateOrCreate(false);
            var conteúdoOriginalSemSincronizarForçandoCreate = informaçãoOriginalParaCreate.Id.GetInformation(false).Content;
            
            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginalParaCreate.Id
            }.UpdateOrCreate(true);
            var conteúdoOriginalSincronizandoForçandoCreate = informaçãoOriginalParaCreate.Id.GetInformation(false).Content;
            
            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginalParaUpdate.Id
            }.Create(false).UpdateOrCreate(false);
            var conteúdoOriginalSemSincronizarForçandoUpdate = informaçãoOriginalParaUpdate.Id.GetInformation(false).Content;
            
            new Information
            {
                Content = conteúdoDaInformaçãoClone, 
                ContentFromId = informaçãoOriginalParaUpdate.Id
            }.Create(false).UpdateOrCreate(true);
            var conteúdoOriginalSincronizandoForçandoUpdate = informaçãoOriginalParaUpdate.Id.GetInformation(false).Content;

            // Assert, Then

            conteúdoOriginalSemSincronizarForçandoCreate.Should().Be(conteúdoDaInformaçãoOriginal);
            conteúdoOriginalSincronizandoForçandoCreate.Should().Be(conteúdoDaInformaçãoClone);

            conteúdoOriginalSemSincronizarForçandoUpdate.Should().Be(conteúdoDaInformaçãoOriginal);
            conteúdoOriginalSincronizandoForçandoUpdate.Should().Be(conteúdoDaInformaçãoClone);
        }
        
        [Fact]
        public void Delete_deve_poder_apagar_uma_informação_sem_ser_recursivo()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();
            
            // Act, When

            // ReSharper disable once RedundantArgumentDefaultValue
            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Delete(false);
            // ReSharper disable once RedundantArgumentDefaultValue
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.Delete(false);
            // ReSharper disable once RedundantArgumentDefaultValue
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.Delete(false);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().Be(0);
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().Be(0);
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().Be(1);
        }
        
        [Fact]
        public void Delete_deve_poder_apagar_uma_informação_de_forma_recursiva()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();

            var cadeiaDeIds = new List<Guid>();
            Guid NovoId()
            {
                var id = Guid.NewGuid();
                cadeiaDeIds.Add(id);
                return id;
            }
            const int quantidadePorNível = 5;
            void Criar(int nível, Guid parentId)
            {
                if (nível <= 0) return;
                for (var i = 0; i < quantidadePorNível; i++)
                {
                    Criar(nível - 1, new Information {Id = NovoId(), ParentId = parentId}.Create(false).Id);
                }
            }
            Criar(3, new Information {Id = NovoId()}.Create(false).Id);
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Delete(true);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.Delete(true);
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.Delete(true);
            var resultadoParaInformaçõesEncadeadas = cadeiaDeIds[0].GetInformation(false).Delete(true);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().Be(0);
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().Be(0);
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().Be(1);
            resultadoParaInformaçõesEncadeadas.Should().Be(cadeiaDeIds.Count);
        }
        
        [Fact]
        public void HasChildren_verifica_se_a_informação_tem_informação_dependentes()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() }.Create();

            var filhos = this.FixtureMany<Information>();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = Guid.Empty;
                filho.ParentId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.Create();
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.HasChildren();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.HasChildren();
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.HasChildren();
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = informaçãoComIdInformadoQueExisteComFilhos.HasChildren();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().BeTrue();
        }
        
        [Fact]
        public void Children_deve_retornar_as_informações_imediatamente_dependentes()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() }.Create();

            var filhos = this.FixtureMany<Information>().ToList();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = Guid.Empty;
                filho.ParentId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.Create();
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Children().ToList();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.Children().ToList();
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.Children().ToList();
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = informaçãoComIdInformadoQueExisteComFilhos.Children().ToList();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().HaveCount(filhos.Count).And
                .Subject.Should().BeEquivalentTo(filhos.Select(a => a.Id));
        }
        
        [Fact]
        public void HasContentTo_verifica_se_o_conteúdo_é_clonado_por_outras_informações()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() }.Create();

            var filhos = this.FixtureMany<Information>();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.ParentId = Guid.Empty;
                filho.Create();
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.HasContentTo();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.HasContentTo();
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.HasContentTo();
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = informaçãoComIdInformadoQueExisteComFilhos.HasContentTo();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().BeTrue();
        }
        
        [Fact]
        public void ContentTo_retorna_as_informações_que_clonam_o_conteúdo()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();
            
            var informaçãoComIdInformadoQueExisteComFilhos = new Information{ Id = Guid.NewGuid() }.Create();

            var filhos = this.FixtureMany<Information>().ToList();
            foreach (var filho in filhos)
            {
                filho.ContentFromId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.ParentId = Guid.Empty;
                filho.Create();
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.ContentTo().ToList();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.ContentTo().ToList();
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.ContentTo().ToList();
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = informaçãoComIdInformadoQueExisteComFilhos.ContentTo().ToList();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().HaveCount(filhos.Count).And
                .Subject.Should().BeEquivalentTo(filhos.Select(a => a.Id));
        }

        [Fact]
        public void ContentFrom_retorna_a_informação_de_onde_o_conteúdo_é_clonado()
        {
            // Arrange, Given
            
            SetDataAccess();

            const int níveis = 5;
            var informações = new List<IInformation>();
            for (var i = 0; i < níveis; i++)
            {
                informações.Add(new Information
                {
                    Id = Guid.NewGuid(),
                    ContentFromId = informações.LastOrDefault()?.Id ?? Guid.Empty
                }.Create(false));
            }

            var informaçãoEmLoop = new Information {Id = Guid.NewGuid()};
            informaçãoEmLoop.ContentFromId = informaçãoEmLoop.Id;
            informaçãoEmLoop.Create();
            
            // Act, When

            var origemDoPrimeiro = informações.First().ContentFrom();
            var origemDoÚltimo = informações.Last().ContentFrom();
            var origemDoLoop = informaçãoEmLoop.ContentFrom();

            // Assert, Then

            origemDoPrimeiro.Should().Be(informações.First().Id);
            origemDoÚltimo.Should().Be(informações.First().Id);
            origemDoLoop.Should().BeEmpty();
        }

        [Fact]
        public void ContentLoad_deve_carregar_o_conteúdo_da_informação_de_origem()
        {
            // Arrange, Given
            
            SetDataAccess();

            const int níveis = 5;
            var informações = new List<IInformation>();
            for (var i = 0; i < níveis; i++)
            {
                informações.Add(new Information
                {
                    Id = Guid.NewGuid(),
                    Content = this.Fixture<string>(),
                    ContentFromId = informações.LastOrDefault()?.Id ?? Guid.Empty
                }.Create(false));
            }

            var informaçãoEmLoop = new Information
            {
                Id = Guid.NewGuid(),
                Content = this.Fixture<string>()
            };
            informaçãoEmLoop.ContentFromId = informaçãoEmLoop.Id;
            informaçãoEmLoop.Create();

            var informaçãoInexistente = new Information
            {
                Id = Guid.NewGuid(),
                Content = this.Fixture<string>()
            };
            
            // Act, When

            var carregamentoNoPrimeiro = informações.First().ContentLoad(true);
            var carregamentoNoÚltimo = informações.Last().ContentLoad(true);
            var carregamentoNoLoop = informaçãoEmLoop.ContentLoad(true);
            var carregamentoNoInexistente = informaçãoInexistente.ContentLoad(true);

            // Assert, Then

            carregamentoNoPrimeiro.Should().BeSameAs(informações.First());
            carregamentoNoPrimeiro.Content.Should().Be(informações.First().Content);

            carregamentoNoÚltimo.Should().BeSameAs(informações.Last());
            carregamentoNoÚltimo.Content.Should().Be(informações.First().Content);
            
            carregamentoNoLoop.Should().BeSameAs(informaçãoEmLoop);
            carregamentoNoLoop.Content.Should().Be(informaçãoEmLoop.Content);
            
            carregamentoNoInexistente.Should().BeSameAs(informaçãoInexistente);
            carregamentoNoInexistente.Content.Should().Be(informaçãoInexistente.Content);
        }

        [Fact]
        public void ContentLoad_deve_carregar_o_conteúdo_da_informação_de_origem_com_a_opção_antes_de_atualizar_as_informações_ou_não()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoOriginalVerdadeira = new Information
            {
                Content = this.Fixture<string>()
            }.Create(false);
            
            var informaçãoOriginalFalsa = new Information
            {
                Content = this.Fixture<string>()
            }.Create(false);
            
            var informaçãoClone = new Information
            {
                ContentFromId = informaçãoOriginalVerdadeira.Id
            }.Create(false);

            
            // Act, When

            informaçãoClone.ContentFromId = informaçãoOriginalFalsa.Id;
            
            var conteúdoSemConsultarBanco = informaçãoClone.ContentLoad(false).Content;
            var conteúdoConsultandoBanco = informaçãoClone.ContentLoad(true).Content;

            // Assert, Then

            conteúdoSemConsultarBanco.Should().Be(informaçãoOriginalFalsa.Content);
            conteúdoConsultandoBanco.Should().Be(informaçãoOriginalVerdadeira.Content);
        }

        [Fact]
        public void ContentSave_deve_atualizar_o_conteúdo_da_informação_de_origem()
        {
            // Arrange, Given
            
            SetDataAccess();

            const int níveis = 5;
            var informações = new List<IInformation>();
            for (var i = 0; i < níveis; i++)
            {
                informações.Add(new Information
                {
                    Id = Guid.NewGuid(),
                    Content = this.Fixture<string>(),
                    ContentFromId = informações.LastOrDefault()?.Id ?? Guid.Empty
                }.Create(false));
            }

            // Act, When

            var gravaçãoNoÚltimo = informações.Last().ContentSave(true);
            var conteúdoDoPrimeiro = informações.First().Get(false);

            // Assert, Then

            gravaçãoNoÚltimo.Should().BeSameAs(informações.Last());
            gravaçãoNoÚltimo.Content.Should().Be(conteúdoDoPrimeiro.Content);
        }

        [Fact]
        public void ContentSave_deve_atualizar_o_conteúdo_da_informação_de_origem_com_a_opção_antes_de_atualizar_as_informações_ou_não()
        {
            // Arrange, Given

            SetDataAccess();
            
            var conteúdoParaInformaçãoOriginalVerdadeira = this.Fixture<string>();
            var informaçãoOriginalVerdadeira = new Information().Create(false);
            
            var conteúdoParaInformaçãoOriginalFalsa = this.Fixture<string>();
            var informaçãoOriginalFalsa = new Information().Create(false);

            var informaçãoClone = new Information
            {
                ContentFromId = informaçãoOriginalVerdadeira.Id,
                Content = this.Fixture<string>()
            }.Create(false);

            void RedefinirValoresIniciais()
            {
                informaçãoOriginalVerdadeira.Content = conteúdoParaInformaçãoOriginalVerdadeira;
                informaçãoOriginalVerdadeira.Update(false);
                informaçãoOriginalFalsa.Content = conteúdoParaInformaçãoOriginalFalsa;
                informaçãoOriginalFalsa.Update(false);
            }

            // Act, When

            informaçãoClone.ContentFromId = informaçãoOriginalFalsa.Id;

            RedefinirValoresIniciais();
            informaçãoClone.ContentSave(false);
            var conteúdoSemConsultarBancoDaVerdadeira = informaçãoOriginalVerdadeira.Id.GetInformation(false).Content;
            var conteúdoSemConsultarBancoDaFalse = informaçãoOriginalFalsa.Id.GetInformation(false).Content;

            RedefinirValoresIniciais();
            informaçãoClone.ContentSave(true);
            var conteúdoConsultandoBancoDaVerdadeira = informaçãoOriginalVerdadeira.Id.GetInformation(false).Content;
            var conteúdoConsultandoBancoDaFalsa = informaçãoOriginalFalsa.Id.GetInformation(false).Content;

            // Assert, Then

            conteúdoSemConsultarBancoDaVerdadeira.Should().Be(conteúdoParaInformaçãoOriginalVerdadeira);
            conteúdoSemConsultarBancoDaFalse.Should().Be(informaçãoClone.Content);
            
            conteúdoConsultandoBancoDaVerdadeira.Should().Be(informaçãoClone.Content);
            conteúdoConsultandoBancoDaFalsa.Should().Be(conteúdoParaInformaçãoOriginalFalsa);
        }

        [Fact]
        public void ContentSave_não_deve_gerar_erro_se_não_localizar_o_clone()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoNãoExisteNemTemClone = new Information
            {
                Content = this.Fixture<string>()
            };

            var informaçãoExisteMasNãoÉClone = new Information
            {
                Content = this.Fixture<string>()
            }.Create();

            // Act, When

            var gravaçãoParaInformaçãoQueNãoExisteNemTemClone = informaçãoNãoExisteNemTemClone.ContentSave(true);
            var gravaçãoParaInformaçãoQueExisteMasNãoÉClone = informaçãoExisteMasNãoÉClone.ContentSave(true);

            // Assert, Then

            gravaçãoParaInformaçãoQueNãoExisteNemTemClone.Should().BeSameAs(informaçãoNãoExisteNemTemClone);
            gravaçãoParaInformaçãoQueExisteMasNãoÉClone.Should().BeSameAs(informaçãoExisteMasNãoÉClone);
        }
    }
}