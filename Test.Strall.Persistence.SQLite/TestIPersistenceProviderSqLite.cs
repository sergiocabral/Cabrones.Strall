﻿using System;
using Cabrones.Test;
using Strall.Persistence.Sql;
using Xunit;

namespace Strall.Persistence.SqLite
{
    public class TestIPersistenceProviderSqLite
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IPersistenceProviderSqLite);

            // Assert, Then

            sut.AssertMyImplementations(typeof(IPersistenceProviderSql), typeof(IPersistenceProvider<ISqLiteConnectionInfo>), typeof(IDataAccess), typeof(IDisposable));
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(1);
            sut.AssertPublicPropertyPresence("SqliteConnection Connection { get; }");
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}