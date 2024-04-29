namespace legallead.email.utility
{
    public static class StringExtensions
    {
        private static readonly char[] separator = [' '];

        public static IEnumerable<string> SplitByLength(this string sentence, int length)
        {
            var charCount = 0;
            var list = sentence.Split(separator, StringSplitOptions.TrimEntries)
                .GroupBy(w =>
                {
                    int lineCount = charCount += w.Length + 1;
                    return lineCount / length;
                })
                .Select(g => string.Join(" ", g).TrimStart()).ToList();
            return list;
        }

        public static string RemoveLineEndings(this string content)
        {
            const string pipe = "|";
            const string doublepipe = "||";
            string[] lineendings = [Environment.NewLine, "\\r", "\\n"];
            if (string.IsNullOrWhiteSpace(content)) { return content; }
            foreach (var line in lineendings)
            {
                content = content.Replace(line, pipe);
            }
            if (!content.Contains(doublepipe)) { return content; }
            while (content.Contains(doublepipe)) { content = content.Replace(doublepipe, pipe); }
            return content;
        }
    }
}
