using legallead.desktop.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.tests.utilities
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("   ", "   ")]
        [InlineData("war and peace", "War And Peace")]
        [InlineData("WAR AND PEACE", "War And Peace")]
        [InlineData("WAR and PEACE", "War And Peace")]
        [InlineData("War and peace", "War And Peace")]
        [InlineData("War and PeaCE", "War And Peace")]
        public void TitleCaseTests(string input, string expected)
        {
            var actual = input.ToTitleCase();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("   ", "   ")]
        [InlineData("fruit-and-vegetables", "fruit-and-vegetables")]
        [InlineData("fruit/and/vegetables", "fruit/and/vegetables")]
        [InlineData("fruit/and/vegetables/", "fruit/and/vegetables")]
        [InlineData("fruit/and/vegetables////", "fruit/and/vegetables")]
        public void TrimEndTests(string input, string expected)
        {
            var actual = input.TrimSlash();
            Assert.Equal(expected, actual);
        }
    }
}