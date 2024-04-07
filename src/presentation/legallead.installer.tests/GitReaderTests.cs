using Bogus;
using legallead.installer.Classes;
using legallead.installer.Models;
using System.Diagnostics;

namespace legallead.installer.tests
{
    public class GitReaderTests
    {
        private static readonly Faker<ReleaseAssetModel> assetFaker =
            new Faker<ReleaseAssetModel>()
            .RuleFor(x => x.AssetId, y => y.Random.Int(100, 2000))
            .RuleFor(x => x.RepositoryId, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.Name, y => Path.GetFileNameWithoutExtension(y.System.CommonFileName()))
            .RuleFor(x => x.DownloadUrl, y => y.Internet.Url())
            .RuleFor(x => x.Version, y => y.System.Semver());

        private static readonly Faker<ReleaseModel> modelFaker =
            new Faker<ReleaseModel>()
            .RuleFor(x => x.Name, y => y.System.Version().ToString())
            .RuleFor(x => x.Id, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.RepositoryId, y => y.Random.Long(1, 1000))
            .RuleFor(x => x.PublishDate, y => y.Date.Recent())
            .FinishWith((x, m) =>
            {
                var nbr = x.Random.Int(2, 5);
                m.Assets = assetFaker.Generate(nbr);
            });

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(true, false, false)]
        public void ClientCanFindAsset(bool isBlankVersion, bool isBlankApp, bool isNotFound)
        {
            var releases = modelFaker.Generate(10);
            var list = releases.SelectMany(x => x.Assets).ToList();
            var selection = new Faker().PickRandom(list);
            var changeApp = new Faker().Random.Bool();
            var version = selection.Version;
            var name = selection.Name;
            if (isBlankVersion) version = string.Empty;
            if (isBlankApp) name = string.Empty;
            if (isNotFound && changeApp) name = "not-matched";
            if (isNotFound && !changeApp) version = "not-matched";
            var client = new GitReader();
            var actual = client.FindAsset(releases, version, name);
            if (isBlankVersion || isBlankApp || isNotFound)
                Assert.Null(actual);
            else
                Assert.NotNull(actual);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData("not-available", false)]
        [InlineData("legallead.desktop-windows", true)]
        [InlineData("  legallead.desktop-windows", true)]
        [InlineData("legallead.desktop-windows  ", true)]
        [InlineData("  legallead.desktop-windows  ", true)]
        [InlineData("legallead.installer", true)]
        [InlineData("LEGALLEAD.INSTALLER", true)]
        [InlineData("Legallead.Installer", true)]
        public void ClientCanVerify(string name, bool expected)
        {
            var client = new GitReader();
            var isLocated = client.VerifyPackageName(name);
            Assert.Equal(expected, isLocated);
        }
    }
}