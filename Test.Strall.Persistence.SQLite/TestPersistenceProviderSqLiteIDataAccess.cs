﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoFixture;
using Cabrones.Test;
using FluentAssertions;
using Strall.Exceptions;
using Xunit;

namespace Strall.Persistence.SQLite
{
    public class TestPersistenceProviderSqLiteIDataAccess: IDisposable
    {
        /// <summary>
        /// Sistema sob teste (SUT = System Under Test)
        /// </summary>
        private readonly IPersistenceProviderSqLite _sut;

        /// <summary>
        /// Arquivo do banco de dados para este teste.
        /// </summary>
        private const string Database = "TestPersistenceProviderSqLiteIDataAccess";

        /// <summary>
        /// Setup do teste.
        /// </summary>
        public TestPersistenceProviderSqLiteIDataAccess()
        {
            _sut = new PersistenceProviderSqLite();
            _sut.Open(new ConnectionInfo { Filename = Path.Combine(Environment.CurrentDirectory, Database) });
        }

        /// <summary>
        /// Liberação de recursos.
        /// </summary>
        public void Dispose() => _sut?.Close();

        [Fact]
        public void métodos_de_manipulação_de_dados_só_devem_funcionar_com_a_conexão_com_o_banco_de_dados_aberta()
        {
            // Arrange, Given

            var persistence = new PersistenceProviderSqLite() as IPersistenceProviderSqLite;
            
            // Act, When

            var métodos = new List<Action>
            {
                () => persistence.Exists(Guid.Empty),
                () => persistence.Get(Guid.Empty),
                () => persistence.Create(new InformationRaw()),
                () => persistence.Update(new InformationRaw()),
                () => persistence.Delete(Guid.Empty),
                () => persistence.HasChildren(Guid.Empty),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => persistence.Children(Guid.Empty).ToList(),
                () => persistence.HasClonesTo(Guid.Empty),
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                () => persistence.ClonesTo(Guid.Empty).ToList()
            };

            // Assert, Then

            foreach (var método in métodos)
            {
                método.Should().ThrowExactly<StrallConnectionIsCloseException>();
            }
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Exists()
        {
            // Arrange, Given

            var informaçãoIndefinida = new InformationRaw{ Id = Guid.Empty };
            
            var informaçãoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoQueExiste = new InformationRaw();
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

            var informaçãoIndefinida = new InformationRaw{ Id = Guid.Empty };
            
            var informaçãoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoQueExiste = new InformationRaw();
            informaçãoQueExiste.Id = _sut.Create(informaçãoQueExiste);
            
            // Act, When

            var resultadoParaInformaçãoIndefinida = _sut.Get(informaçãoIndefinida.Id);
            var resultadoParaInformaçãoQueNãoExiste = _sut.Get(informaçãoQueNãoExiste.Id);
            var resultadoParaInformaçãoQueExiste = _sut.Get(informaçãoQueExiste.Id);

            // Assert, Then

            resultadoParaInformaçãoIndefinida.Should().BeNull();
            resultadoParaInformaçãoQueNãoExiste.Should().BeNull();
            resultadoParaInformaçãoQueExiste.Should().NotBeNull().And
                .Subject.As<IInformationRaw>().Id.Should().Be(informaçãoQueExiste.Id);
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Create()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new InformationRaw{ Id = Guid.Empty };
            var informaçãoComIdInformado = new InformationRaw{ Id = Guid.NewGuid() };
            
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
            criarDuplicado.Should().ThrowExactly<Microsoft.Data.Sqlite.SqliteException>()
                .Which.Message.Should().Be($"SQLite Error 19: 'UNIQUE constraint failed: {_sut.SqlNames.TableInformation}.{_sut.SqlNames.TableInformationColumnId}'.");
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Update()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new InformationRaw{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new InformationRaw{ Id = Guid.NewGuid() };
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

            var informaçãoComIdVazio = new InformationRaw{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new InformationRaw{ Id = Guid.NewGuid() };
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

            var informaçãoComIdVazio = new InformationRaw{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.Fixture().CreateMany<InformationRaw>();
            foreach (var filho in filhos)
            {
                filho.ParentId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.CloneFromId = Guid.Empty;
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

            var informaçãoComIdVazio = new InformationRaw{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.Fixture().CreateMany<InformationRaw>().ToList();
            foreach (var filho in filhos)
            {
                filho.ParentId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.CloneFromId = Guid.Empty;
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

            var informaçãoComIdVazio = new InformationRaw{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.Fixture().CreateMany<InformationRaw>();
            foreach (var filho in filhos)
            {
                filho.CloneFromId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.ParentId = Guid.Empty;
                _sut.Create(filho);
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = _sut.HasClonesTo(informaçãoComIdVazio.Id);
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.HasClonesTo(informaçãoComIdInformadoQueNãoExiste.Id);
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.HasClonesTo(informaçãoComIdInformadoQueExiste.Id);
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = _sut.HasClonesTo(informaçãoComIdInformadoQueExisteComFilhos.Id);

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().BeFalse();
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().BeTrue();
        }
        
        [Fact]
        public void verifica_funcionamento_do_método_Clones()
        {
            // Arrange, Given

            var informaçãoComIdVazio = new InformationRaw{ Id = Guid.Empty };
            var informaçãoComIdInformadoQueNãoExiste = new InformationRaw{ Id = Guid.NewGuid() };
            
            var informaçãoComIdInformadoQueExiste = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExiste);
            
            var informaçãoComIdInformadoQueExisteComFilhos = new InformationRaw{ Id = Guid.NewGuid() };
            _sut.Create(informaçãoComIdInformadoQueExisteComFilhos);

            var filhos = this.Fixture().CreateMany<InformationRaw>().ToList();
            foreach (var filho in filhos)
            {
                filho.CloneFromId = informaçãoComIdInformadoQueExisteComFilhos.Id;
                filho.ParentId = Guid.Empty;
                _sut.Create(filho);
            }
            
            // Act, When

            var resultadoParaInformaçãoComIdVazio = _sut.ClonesTo(informaçãoComIdVazio.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueNãoExiste = _sut.ClonesTo(informaçãoComIdInformadoQueNãoExiste.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueExiste = _sut.ClonesTo(informaçãoComIdInformadoQueExiste.Id).ToList();
            var resultadoParaInformaçãoComIdInformadoQueExisteComFilhos = _sut.ClonesTo(informaçãoComIdInformadoQueExisteComFilhos.Id).ToList();

            // Assert, Then

            resultadoParaInformaçãoComIdVazio.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueNãoExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExiste.Should().HaveCount(0);
            resultadoParaInformaçãoComIdInformadoQueExisteComFilhos.Should().HaveCount(filhos.Count).And
                .Subject.Should().BeEquivalentTo(filhos.Select(a => a.Id));
        }
    }
}