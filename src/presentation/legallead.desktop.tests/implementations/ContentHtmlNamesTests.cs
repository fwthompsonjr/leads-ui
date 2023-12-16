using legallead.desktop.implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.tests.implementations
{
    public class ContentHtmlNamesTests
    {
        [Fact]
        public void ContentHtmlNamesCanBeCreated()
        {
            var sut = new ContentHtmlNames();
            Assert.NotNull(sut);
        }

        [Fact]
        public void ContentHtmlNamesContainsNames()
        {
            var sut = new ContentHtmlNames();
            Assert.NotNull(sut.Names);
            Assert.NotEmpty(sut.Names);
        }

        [Fact]
        public void ContentHtmlNamesContainsContentNames()
        {
            var sut = new ContentHtmlNames();
            Assert.NotNull(sut.ContentNames);
            Assert.NotEmpty(sut.ContentNames);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("missing", false)]
        [InlineData("introduction", true)]
        [InlineData("Introduction", true)]
        public void ContentHtmlNamesIsValid(string test, bool expected)
        {
            var sut = new ContentHtmlNames();
            var actual = sut.IsValid(test);
            Assert.Equal(expected, actual);
        }
    }
}