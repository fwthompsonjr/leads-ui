using Bogus;
using legallead.installer.Classes;
using legallead.installer.Interfaces;
using legallead.installer.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace legallead.installer.tests
{
    public class LeadAppInstallerTests
    {
        [Fact]
        public void SutCanCreate()
        {
            var exception = Record.Exception(() =>
            {
                _ = GetInstance();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void SutHasFileManager()
        {
            var instance = GetInstance();
            Assert.NotNull(instance.FileManager);
        }

        [Fact]
        public void SutParentHasFileManager()
        {
            var fileManager = InstallerInstance.GetOperation();
            var instance = new LeadAppInstaller(fileManager);
            Assert.NotNull(instance.FileManager);
        }

        [Fact]
        public void SutHasParentFolder()
        {
            var instance = GetInstance();
            Assert.False(string.IsNullOrEmpty(instance.ParentFolder));
        }

        [Fact]
        public void SutParentHasParentFolder()
        {
            var fileManager = InstallerInstance.GetOperation();
            var instance = new LeadAppInstaller(fileManager);
            Assert.False(string.IsNullOrEmpty(instance.ParentFolder));
        }

        [Fact]
        public void SutHasSubFolder()
        {
            var instance = GetInstance();
            Assert.False(string.IsNullOrEmpty(instance.SubFolder));
        }

        [Fact]
        public void SutParentHasSubFolder()
        {
            var fileManager = InstallerInstance.GetOperation();
            var instance = new LeadAppInstaller(fileManager);
            Assert.False(string.IsNullOrEmpty(instance.SubFolder));
        }

        [Fact]
        public void InstallWithoutDataShouldReturnNull()
        {
            var instance = GetInstance();
            var payload = new InstallParameter
            {
                Data = null
            };
            var actual = instance.Install(payload.Model, payload.Data);
            Assert.Null(actual);
        }

        [Fact]
        public void InstallWithoutNameShouldReturnNull()
        {
            var instance = GetInstance();
            var payload = new InstallParameter();
            payload.Model.Name = string.Empty;
            var actual = instance.Install(payload.Model, payload.Data);
            Assert.Null(actual);
        }

        [Fact]
        public void InstallWithoutVersionShouldReturnNull()
        {
            var instance = GetInstance();
            var payload = new InstallParameter();
            payload.Model.Version = string.Empty;
            var actual = instance.Install(payload.Model, payload.Data);
            Assert.Null(actual);
        }

        [Fact]
        public void InstallNormalPathway()
        {
            var instance = GetInstance();
            var payload = new InstallParameter();
            var subfolder = instance.SubFolder;
            var targetDir = Path.Combine(subfolder, payload.Model.Name);
            var installDir = Path.Combine(targetDir, payload.Model.Version);
            var mock = instance.MockFileManager;

            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == subfolder)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == targetDir)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == installDir)))
                .Returns(false)
                .Returns(true);
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == subfolder)));
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == targetDir)));
            mock.Setup(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b)));
            var actual = instance.Install(payload.Model, payload.Data);
            Assert.Equal(installDir, actual);
            mock.Verify(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b)), Times.Never);
        }

        [Fact]
        public void InstallShouldDeleteTargetIfExists()
        {
            var instance = GetInstance();
            var payload = new InstallParameter();
            var subfolder = instance.SubFolder;
            var targetDir = Path.Combine(subfolder, payload.Model.Name);
            var installDir = Path.Combine(targetDir, payload.Model.Version);
            var mock = instance.MockFileManager;

            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == subfolder)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == targetDir)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == installDir)))
                .Returns(true)
                .Returns(true);
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == subfolder)));
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == targetDir)));
            mock.Setup(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b)));
            var actual = instance.Install(payload.Model, payload.Data);
            Assert.Equal(installDir, actual);
            mock.Verify(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b)), Times.Once);
        }


        [Fact]
        public void InstallShouldReturnEmptyIfTargetDoesNotExist()
        {
            var instance = GetInstance();
            var payload = new InstallParameter();
            var subfolder = instance.SubFolder;
            var targetDir = Path.Combine(subfolder, payload.Model.Name);
            var installDir = Path.Combine(targetDir, payload.Model.Version);
            var mock = instance.MockFileManager;

            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == subfolder)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == targetDir)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == installDir)))
                .Returns(true)
                .Returns(false);
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == subfolder)));
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == targetDir)));
            mock.Setup(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b)));
            var actual = instance.Install(payload.Model, payload.Data);
            Assert.Equal("", actual);
            mock.Verify(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b)), Times.Once);
        }

        [Fact]
        public void InstallShouldReturnEmptyOnException()
        {
            var instance = GetInstance();
            var exception = new Faker().System.Exception();
            var payload = new InstallParameter();
            var subfolder = instance.SubFolder;
            var targetDir = Path.Combine(subfolder, payload.Model.Name);
            var installDir = Path.Combine(targetDir, payload.Model.Version);
            var mock = instance.MockFileManager;

            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == subfolder)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == targetDir)))
                .Returns(false)
                .Returns(true);
            mock.SetupSequence(s => s.DirectoryExists(It.Is<string>(n => n == installDir)))
                .Returns(true)
                .Returns(false);
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == subfolder)));
            mock.Setup(s => s.CreateDirectory(It.Is<string>(n => n == targetDir)));
            mock.Setup(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b))).Throws(exception);
            var actual = instance.Install(payload.Model, payload.Data);
            Assert.Equal("", actual);
            mock.Verify(s => s.DeleteDirectory(It.Is<string>(n => n == installDir), It.Is<bool>(b => b)), Times.Once);
        }

        private static InstallerInstance GetInstance()
        {
            var manager = InstallerInstance.GetOperation();
            var instance = new InstallerInstance(manager);
            return instance;
        }

        private sealed class InstallerInstance : LeadAppInstaller
        {
            private const string _subDir = "_ll-testing";
            private readonly ILeadFileOperation fileService;
            private readonly Mock<ILeadFileOperation> fileServiceMq;
            private readonly string parentFolder;
            private readonly string _applicationsDir;

            public InstallerInstance(ILeadFileOperation fileOperation) : base(fileOperation) 
            {
                fileServiceMq = new Mock<ILeadFileOperation>();
                fileService = fileServiceMq.Object;
                parentFolder = Path.GetTempPath();
                _applicationsDir = Path.Combine(parentFolder, _subDir);
            }
            public override string ParentFolder => parentFolder;
            public override string SubFolder => _applicationsDir;
            public override ILeadFileOperation FileManager => fileService;
            public Mock<ILeadFileOperation> MockFileManager => fileServiceMq;

            public static ILeadFileOperation GetOperation()
            {
                return new Mock<ILeadFileOperation>().Object;
            }
        }

        private sealed class InstallParameter
        {
            private readonly static Faker faker = new();
            public InstallParameter()
            {
                Model = new()
                {
                    AssetId = faker.Random.Int(100, 2000),
                    RepositoryId = faker.Random.Long(200000, 400000),
                    Name = Path.GetFileNameWithoutExtension(faker.System.CommonFileName()),
                    DownloadUrl = faker.Internet.Url(),
                    Version = faker.System.Semver()
                };
                var count = faker.Random.Int(200, 2000);
                Data = faker.System.Random.Bytes(count);
            }
            public ReleaseAssetModel Model { get; set; }
            public object? Data { get; set; }
        }
    }
}
