namespace legallead.resources.tests
{
    public class ResourceTextTest
    {
        [Fact]
        public void ResourceProvidesText()
        {
            var text = ResourceText.Text();
            Assert.False(string.IsNullOrWhiteSpace(text));
        }
    }
}