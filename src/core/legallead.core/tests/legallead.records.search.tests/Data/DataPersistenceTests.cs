using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using legallead.records.search.Db;

namespace legallead.records.search.UnitTests.Data
{
    [TestClass]
    public class DataPersistenceTests
    {
        private class TmpData
        {
            public int Id { get; set; } 
            public string Name { get; set; } = string.Empty;
            public DateTime CreateDate { get; set; }

            public string FileName { get; set; } = string.Empty;
        }

        private Faker<TmpData>? Faker;

        [TestInitialize]
        public void Setup()
        {
            Faker ??= new Faker<TmpData>()
                    .RuleFor(f => f.Id, r => r.IndexFaker)
                    .RuleFor(f => f.Name, r => r.Person.FullName)
                    .RuleFor(f => f.CreateDate, r => r.Date.Recent(7))
                    .RuleFor(f => f.FileName, r => r.System.FileName("temp"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            var path = DataPersistence.DataFolder;
            try
            {
                if (!Directory.Exists(path))
                {
                    return;
                }

                var files = new DirectoryInfo(path).GetFiles("*.temp").ToList();
                if (!files.Any())
                {
                    return;
                }

                files.ForEach(f => File.Delete(f.FullName));
            }
            catch (Exception)
            {
                // no action in cleanup text error
            }
        }

        [TestMethod]
        public void CanGet_Folder()
        {
            var folder = DataPersistence.DataFolder;
            Directory.Exists(folder).ShouldBeTrue();
        }

        [TestMethod]
        public void Faker_GeneratesUniqueIndex()
        {
            if (Faker == null) return;
            var obj = Faker.Generate(2);
            obj[0].Id.ShouldNotBeSameAs(obj[1].Id);
            obj[0].Id = obj[1].Id;
            obj[0].Id.ShouldBeEquivalentTo(obj[1].Id);
        }

        [TestMethod]
        public void Faker_GeneratesUniqueDate()
        {
            if (Faker == null) return;
            var obj = Faker.Generate(2);
            obj[0].CreateDate.ShouldNotBeSameAs(obj[1].CreateDate);
            obj[0].CreateDate = obj[1].CreateDate;
            obj[0].CreateDate.ShouldBeEquivalentTo(obj[1].CreateDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_RequiresAFileName()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            DataPersistence.Save(string.Empty, obj.FileName);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Save_RequiresData()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, null);
        }

        [TestMethod]
        public void Save_WillSave_NewData()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Save_WillNot_Overwrite()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeTrue();
            DataPersistence.Save(obj.FileName, obj);
        }

        [TestMethod]
        public void Save_FileExists_True()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeTrue();
            DataPersistence.FileExists(obj.FileName).ShouldBeTrue();
        }

        [TestMethod]
        public void Save_FileExists_False()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            var expected = Path.Combine(DataPersistence.DataFolder, obj.FileName);
            File.Exists(expected).ShouldBeFalse();
            DataPersistence.FileExists(obj.FileName).ShouldBeFalse();
        }

        [TestMethod]
        public void GetContent_NoFileExists_IsNull()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            var fname = DataPersistence.GetContent<TmpData>(obj.FileName);
            Assert.AreEqual(0, fname.Id);
        }

        [TestMethod]
        public void GetContent_FileExists()
        {
            if (Faker == null) return;
            var obj = Faker.Generate();
            DataPersistence.Save(obj.FileName, obj);
            var actual = DataPersistence.GetContent<TmpData>(obj.FileName);
            actual.Id.ShouldBe(obj.Id);
            actual.Name.ShouldBe(obj.Name);
            actual.CreateDate.ShouldBe(obj.CreateDate);
            actual.FileName.ShouldBe(obj.FileName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileExists_Expects_FileName()
        {
            _ = DataPersistence.FileExists(string.Empty);
        }
    }
}
