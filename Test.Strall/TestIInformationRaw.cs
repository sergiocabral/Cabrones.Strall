﻿using System;
using Cabrones.Test;
using Xunit;

namespace Strall
{
    public class TestIInformationRaw
    {
        [Fact]
        public void verificações_declarativas()
        {
            // Arrange, Given
            // Act, When

            var sut = typeof(IInformationRaw);

            // Assert, Then

            sut.AssertMyImplementations(typeof(ICloneable));
            sut.AssertMyOwnImplementations();
            sut.AssertMyOwnPublicPropertiesCount(16);
            sut.AssertPublicPropertyPresence("Guid Id { get; set; }");
            sut.AssertPublicPropertyPresence("String Description { get; set; }");
            sut.AssertPublicPropertyPresence("String Content { get; set; }");
            sut.AssertPublicPropertyPresence("String ContentType { get; set; }");
            sut.AssertPublicPropertyPresence("Guid ParentId { get; set; }");
            sut.AssertPublicPropertyPresence("String ParentRelation { get; set; }");
            sut.AssertPublicPropertyPresence("Guid CloneFromId { get; set; }");
            sut.AssertPublicPropertyPresence("Int32 SiblingOrder { get; set; }");
            sut.AssertMyOwnPublicMethodsCount(0);
        }
    }
}