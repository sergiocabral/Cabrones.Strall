﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Strall.Persistence.SqLite;
using Xunit;

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
                () => informacao.Get(),
                () => Guid.Empty.GetInformation(),
                () => informacao.Create(),
                () => informacao.Update(),
                () => informacao.Delete(),
                () => informacao.HasContentTo(),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => informacao.ContentTo().ToList(),
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

            var resultadoParaInformaçãoIndefinida = informaçãoIndefinida.Get();
            var resultadoParaInformaçãoQueNãoExiste = informaçãoQueNãoExiste.Get();
            var resultadoParaInformaçãoQueExiste = informaçãoQueExiste.Get();

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().NotBeNull();
            resultadoParaInformaçãoIndefinida.Id.Should().BeEmpty();
            resultadoParaInformaçãoQueNãoExiste.Should().NotBeNull();
            resultadoParaInformaçãoQueNãoExiste.Id.Should().BeEmpty();
            resultadoParaInformaçãoQueExiste.Should().NotBeNull().And
                .Subject.As<IInformation>().Id.Should().Be(informaçãoQueExiste.Id);
        }

        [Fact]
        public void GetInformation_deve_consultar_no_banco_de_dados_uma_informação()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoIndefinida = Guid.Empty;
            var informaçãoQueNãoExiste = Guid.NewGuid();
            var informaçãoQueExiste = new Information().Create().Id;
            
            // Act, When

            var resultadoParaInformaçãoIndefinida = informaçãoIndefinida.GetInformation();
            var resultadoParaInformaçãoQueNãoExiste = informaçãoQueNãoExiste.GetInformation();
            var resultadoParaInformaçãoQueExiste = informaçãoQueExiste.GetInformation();

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().BeNull();
            resultadoParaInformaçãoQueNãoExiste.Should().BeNull();
            resultadoParaInformaçãoQueExiste.Should().NotBeNull().And
                .Subject.As<IInformation>().Id.Should().Be(informaçãoQueExiste);
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

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Create();
            var resultadoParaInformaçãoComIdInformado = informaçãoComIdInformado.Create();
            Action criarDuplicado = () => informaçãoComIdInformado.Create();

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
        public void Update_deve_atualizar_uma_informação()
        {
            // Arrange, Given
            
            SetDataAccess();

            var informaçãoComIdVazio = new Information{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new Information{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new Information{ Id = Guid.NewGuid() }.Create();
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Update();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.Update();
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.Update();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeTrue();
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

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.UpdateOrCreate();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.UpdateOrCreate();
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.UpdateOrCreate();

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
                    Criar(nível - 1, new Information {Id = NovoId(), ParentId = parentId}.Create().Id);
                }
            }
            Criar(3, new Information {Id = NovoId()}.Create().Id);
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = informaçãoComIdVazio.Delete(true);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = informaçãoComIdInformadoQueNãoExiste.Delete(true);
            var resultadoParaInformaçãoComIdInformadoQueExiste = informaçãoComIdInformadoQueExiste.Delete(true);
            var resultadoParaInformaçõesEncadeadas = cadeiaDeIds[0].GetInformation().Delete(true);

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
                }.Create());
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
                }.Create());
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

            var origemDoPrimeiro = informações.First().ContentLoad();
            var origemDoÚltimo = informações.Last().ContentLoad();
            var origemDoLoop = informaçãoEmLoop.ContentLoad();
            var origemDoInexistente = informaçãoInexistente.ContentLoad();

            // Assert, Then

            origemDoPrimeiro.Should().Be(informações.First().Content);
            origemDoÚltimo.Should().Be(informações.First().Content);
            origemDoLoop.Should().Be(informaçãoEmLoop.Content);
            origemDoInexistente.Should().Be(informaçãoInexistente.Content);
        }
        
    }
}