using legallead.jdbc.attr;

namespace legallead.jdbc.tests.attr
{
    public class AttributeTests
    {
        [Theory]
        [InlineData("EMPTY")]
        [InlineData("ENCODED")]
        [InlineData("DECODED")]
        [InlineData("DOWNLOADED")]
        public void CountyFileStatusAttribute_ValidValues_ShouldPass(string status)
        {
            var attribute = new CountyFileStatusAttribute();
            var result = attribute.IsValid(status);
            Assert.True(result, $"Expected {status} to be valid.");
        }

        [Theory]
        [InlineData("INVALID")]
        [InlineData("ENCODING")]
        [InlineData("DECODE")]
        [InlineData(null)]
        [InlineData("")]
        public void CountyFileStatusAttribute_InvalidValues_ShouldFail(string status)
        {
            var attribute = new CountyFileStatusAttribute();
            var result = attribute.IsValid(status);
            Assert.False(result, $"Expected {status} to be invalid.");
        }

        [Theory]
        [InlineData("NONE")]
        [InlineData("EXL")]
        [InlineData("CSV")]
        [InlineData("JSON")]
        public void CountyFileTypeAttribute_ValidValues_ShouldPass(string type)
        {
            var attribute = new CountyFileTypeAttribute();
            var result = attribute.IsValid(type);
            Assert.True(result, $"Expected {type} to be valid.");
        }

        [Theory]
        [InlineData("XML")]
        [InlineData("TXT")]
        [InlineData("DOC")]
        [InlineData(null)]
        [InlineData("")]
        public void CountyFileTypeAttribute_InvalidValues_ShouldFail(string type)
        {
            var attribute = new CountyFileTypeAttribute();
            var result = attribute.IsValid(type);
            Assert.False(result, $"Expected {type} to be invalid.");
        }
    }
}
