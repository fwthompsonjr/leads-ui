using legallead.jdbc.models;

namespace legallead.jdbc.tests.models
{
    public class DbUploadRequestTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var sut = new DbUploadRequest();
            Assert.True(string.IsNullOrEmpty(sut.Id));
            Assert.Empty(sut.Contents);
        }
    }
}
