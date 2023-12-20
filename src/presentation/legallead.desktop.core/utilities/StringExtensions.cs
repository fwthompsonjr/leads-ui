using System.Globalization;

namespace legallead.desktop.utilities
{
    internal static class StringExtensions
    {
        public static string ToTitleCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            return textInfo.ToTitleCase(s.ToLower());
        }

        private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
    }
}