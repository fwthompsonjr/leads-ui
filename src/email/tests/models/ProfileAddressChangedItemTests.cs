﻿using legallead.email.interfaces;
using legallead.email.models;

namespace legallead.email.tests.entities
{
    public class ProfileAddressChangedItemTests
    {
        [Fact]
        public void DtoCanBeGenerated()
        {
            var exception = Record.Exception(() =>
            {
                _ = ProfileMockInfrastructure.AddressChangeFaker.Generate();
            });
            Assert.Null(exception);
        }
        [Fact]
        public void DtoImplementsInterface()
        {
            var exception = Record.Exception(() =>
            {
                var test = ProfileMockInfrastructure.AddressChangeFaker.Generate();
                Assert.IsAssignableFrom<IProfileChangeItem>(test);
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DtoHasExpectedFieldDefined()
        {
            var sut = new ProfileAddressChangedItem();
            var test = ProfileMockInfrastructure.AddressChangeFaker.Generate();
            Assert.NotEqual(sut.FieldName, test.FieldName);
            Assert.NotEqual(sut.Description, test.Description);
        }
    }
}