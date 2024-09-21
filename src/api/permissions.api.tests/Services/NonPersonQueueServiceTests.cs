using legallead.jdbc.entities;
using legallead.permissions.api.Services;

namespace permissions.api.tests.Services
{
    public class NonPersonQueueServiceTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ServiceCanConvertSampleData(int sourceId)
        {
            var expected = sourceId == 1 || sourceId == 2;
            var request = new QueueNonPersonBo
            {
                ExcelData = GetDenton(sourceId)
            };
            var response = NonPersonQueueService.GetPeople(request);
            Assert.Equal(expected, string.IsNullOrEmpty(response));
        }

        private static byte[]? GetDenton(int index = 0)
        {
            var source = index switch {
				0 => dentonSample,
				1 => null,
				2 => string.Empty,
				_ => dentonSample01
			};
            var content = source == null ? null : Convert.FromBase64String(source);
            return content;
        }
        private static readonly string dentonSample = Properties.Resources.denton_excel_file;
        private static readonly string dentonSample01 = Properties.Resources.denton_excel_file_01;
    }
}
