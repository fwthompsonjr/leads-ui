using Bogus;
using legallead.installer.Classes;
using legallead.installer.Models;

namespace legallead.installer.tests
{
    public class GitReaderTests
    {
        private static readonly string[] Repos = ["leads-ui", "leads-reader"];
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
            .RuleFor(x => x.RepositoryName, y =>
            {
                var dc = y.Random.Double();
                var id = dc > 0.75d ? 1 : 0;
                return Repos[id];
            })
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
            var exception = Record.Exception(() =>
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
                _ = client.FindAsset(releases, version, name);
            });
            Assert.Null(exception);
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
        [InlineData("legallead.reader.service", true)]
        [InlineData("  legallead.reader.service", true)]
        [InlineData("legallead.reader.service  ", true)]
        [InlineData("  legallead.reader.service  ", true)]
        public void ClientCanVerify(string name, bool expected)
        {
            var client = new GitReader();
            var isLocated = client.VerifyPackageName(name);
            Assert.Equal(expected, isLocated);
        }
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        public void ReaderCanFilterReleases(int releases, int alternates)
        {
            var problems = Record.Exception(() =>
            {
                var client = new MockGitReader();
                var list1 = releases switch
                {
                    0 => null,
                    _ => modelFaker.Generate(releases)
                };
                var list2 = alternates switch
                {
                    0 => null,
                    _ => modelFaker.Generate(alternates)
                };
                var response = client.FilterReleases(list1, list2);
                var sum = releases + alternates;
                if (sum == 0) Assert.Null(response);
                else
                {
                    Assert.Equal(sum, response?.Count);
                }
            });
            Assert.Null(problems);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(15)]
        public void ReaderCanFilterAssets(int releases)
        {
            var problems = Record.Exception(() =>
            {
                var client = new MockGitReader();
                _ = client.AllowShortcuts;
                var list = releases switch
                {
                    0 => null,
                    _ => modelFaker.Generate(releases)
                };
                _ = client.FilterAssets(list);
            });
            Assert.Null(problems);
        }
        private sealed class MockGitReader : GitReader
        {
            public List<ReleaseModel>? FilterReleases(
                List<ReleaseModel>? releases,
                List<ReleaseModel>? alternates)
            {
                return GetReleases(releases, alternates);
            }
            public List<ReleaseAssetModel>? FilterAssets(
                List<ReleaseModel>? releases
            )
            {
                return GetAssets(releases);
            }
        }


    }
}