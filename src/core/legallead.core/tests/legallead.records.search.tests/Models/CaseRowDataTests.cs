using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using legallead.records.search.Models;

namespace legallead.records.search.UnitTests.Models
{

    [TestClass]
    public class CaseRowDataTests
    {
        private Faker<CaseDataAddress>? AddressFaker;
        private Faker<CaseRowData>? DtoFaker;

        [TestInitialize]
        public void Setup()
        {
            AddressFaker ??= new Faker<CaseDataAddress>()
                    .RuleFor(f => f.Case, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Role, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Party, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Attorney, r => r.Random.AlphaNumeric(15));
            DtoFaker ??= new Faker<CaseRowData>()
                    .RuleFor(f => f.RowId, r => r.IndexFaker)
                    .RuleFor(f => f.Case, r => r.Random.AlphaNumeric(15))
                    .RuleFor(f => f.Court, r => r.Random.AlphaNumeric(3))
                    .RuleFor(f => f.FileDate, r => r.Random.AlphaNumeric(8))
                    .RuleFor(f => f.Status, r => r.Random.AlphaNumeric(8))
                    .RuleFor(f => f.TypeDesc, r => r.Random.AlphaNumeric(20))
                    .RuleFor(f => f.Subtype, r => r.Random.AlphaNumeric(4))
                    .RuleFor(f => f.Style, r => r.Random.AlphaNumeric(40))
                    .RuleFor(f => f.CaseDataAddresses, r =>
                    {
                        var ii = r.Random.Int(0, 5);
                        return AddressFaker.Generate(ii);
                    });
        }

        [TestMethod]
        public void CanConstruct()
        {
            var obj = new CaseRowData();
            Assert.IsNotNull(obj);
        }

        [TestMethod]
        public void CanInit()
        {
            if (DtoFaker == null) {  return; }
            var obj = DtoFaker.Generate();
            Assert.IsNotNull(obj);
        }
        [TestMethod]
        public void CanSet_Case()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Case = src.Case;
            obj.Case.ShouldBe(src.Case);
        }

        [TestMethod]
        public void CanSet_RowId()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.RowId = src.RowId;
            obj.RowId.ShouldBe(src.RowId);
        }

        [TestMethod]
        public void CanSet_Court()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Court = src.Court;
            obj.Court.ShouldBe(src.Court);
        }

        [TestMethod]
        public void CanSet_FileDate()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.FileDate = src.FileDate;
            obj.FileDate.ShouldBe(src.FileDate);
        }

        [TestMethod]
        public void CanSet_Status()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Status = src.Status;
            obj.Status.ShouldBe(src.Status);
        }

        [TestMethod]
        public void CanSet_TypeDesc()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.TypeDesc = src.TypeDesc;
            obj.TypeDesc.ShouldBe(src.TypeDesc);
        }

        [TestMethod]
        public void CanSet_Subtype()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Subtype = src.Subtype;
            obj.Subtype.ShouldBe(src.Subtype);
        }

        [TestMethod]
        public void CanSet_Style()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.Style = src.Style;
            obj.Style.ShouldBe(src.Style);
        }

        [TestMethod]
        public void CanSet_CaseDataAddresses()
        {
            if (DtoFaker == null) { return; }
            var list = DtoFaker.Generate(2);
            var obj = list[0];
            var src = list[1];
            obj.CaseDataAddresses = src.CaseDataAddresses;
            obj.CaseDataAddresses.ShouldBe(src.CaseDataAddresses);
        }
    }

}
