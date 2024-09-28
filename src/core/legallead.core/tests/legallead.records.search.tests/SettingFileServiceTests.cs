using Bogus;
using Xunit;

namespace legallead.records.search.tests
{
    public class SettingFileServiceTests
    {
        private static readonly Faker faker = new();

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("missing")]
        [InlineData("caselayout", true)]
        [InlineData("collinCountyCaseType", true)]
        [InlineData("COLLINCountyMapping_1", true)]
        [InlineData("collinCountyUserMap", true)]
        [InlineData("courtAddress", true)]
        [InlineData("dentonCASECustomInstruction_1", true)]
        [InlineData("dentonCaseCustomInstruction", true)]
        [InlineData("dentonCountyCaseTYPE", true)]
        [InlineData("dentonDistrictCaseType", true)]
        [InlineData("denton-settings", true)]
        [InlineData("harrisCivilCaseType", true)]
        [InlineData("harrisCivilMapping", true)]
        [InlineData("harris-civil-settings", true)]
        [InlineData("settings", true)]
        [InlineData("tarrantCountyCaseType", true)]
        [InlineData("TarrantCountyCustomType", true)]
        [InlineData("tarrantCountyMapping_1", true)]
        [InlineData("tarrantCountyMapping_2", true)]
        [InlineData("tarrantCourtSearchDropDown", true)]
        [InlineData("WebDrivers", true)]
        public void SettingsCanEvaluateExist(string? name, bool expected = false)
        {
            const string ending = ".txt";
            if (name == null)
            {
                var test = SettingFileService.Exists(name);
                Assert.False(test);
                return;
            }
            string filename = faker.System.FileName(ending);
            string folder = Path.GetDirectoryName(filename) ?? string.Empty;
            string fullname = Path.Combine(folder, $"{name}{ending}");
            var actual = SettingFileService.Exists(fullname);
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("missing")]
        [InlineData("caselayout", true)]
        [InlineData("collinCountyCaseType", true)]
        [InlineData("COLLINCountyMapping_1", true)]
        [InlineData("collinCountyUserMap", true)]
        [InlineData("courtAddress", true)]
        [InlineData("dentonCASECustomInstruction_1", true)]
        [InlineData("dentonCaseCustomInstruction", true)]
        [InlineData("dentonCountyCaseTYPE", true)]
        [InlineData("dentonDistrictCaseType", true)]
        [InlineData("denton-settings", true)]
        [InlineData("harrisCivilCaseType", true)]
        [InlineData("harrisCivilMapping", true)]
        [InlineData("harris-civil-settings", true)]
        [InlineData("settings", true)]
        [InlineData("tarrantCountyCaseType", true)]
        [InlineData("TarrantCountyCustomType", true)]
        [InlineData("tarrantCountyMapping_1", true)]
        [InlineData("tarrantCountyMapping_2", true)]
        [InlineData("tarrantCourtSearchDropDown", true)]
        [InlineData("WebDrivers", true)]
        public void SettingsCanGetContentOrDefault(string? name, bool expected = false)
        {
            const string ending = ".txt";
            var fallback = faker.Random.AlphaNumeric(15);
            if (name == null)
            {
                var test = SettingFileService.GetContentOrDefault(name, fallback);
                Assert.Equal(fallback, test);
                return;
            }
            string filename = faker.System.FileName(ending);
            string folder = Path.GetDirectoryName(filename) ?? string.Empty;
            string fullname = Path.Combine(folder, $"{name}{ending}");
            var actual = SettingFileService.GetContentOrDefault(fullname, fallback);
            if (expected)
            {
                Assert.NotEqual(fallback, actual);
                Assert.False(string.IsNullOrWhiteSpace(actual));
                return;
            }
            Assert.Equal(fallback, actual);
        }
    }
}
