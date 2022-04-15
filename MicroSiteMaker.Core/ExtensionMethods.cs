namespace MicroSiteMaker.Core;

public static class ExtensionMethods
{
    private static readonly List<string> s_lowerCaseWords =
        new List<string>
        {
            "a",
            "an",
            "and",
            "as",
            "as long as",
            "at",
            "but",
            "by",
            "even if",
            "for",
            "from",
            "if",
            "if only",
            "in",
            "is",
            "into",
            "like",
            "near",
            "now that",
            "nor",
            "of",
            "off",
            "on",
            "on top of",
            "once",
            "onto",
            "or",
            "out of",
            "over",
            "past",
            "so",
            "so that",
            "than",
            "that",
            "the",
            "till",
            "to",
            "up",
            "upon",
            "with",
            "when",
            "yet"
        };

    public static string ToProperCase(this string text)
    {
        string cleanedText =
            text
                .Replace("-", " ")
                .Replace("_", " ");

        var rawWords =
            cleanedText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim().UpperCaseFirstChar())
                .ToList();

        var properCasedWords = new List<string>();

        if (rawWords.Any())
        {
            // First word is always upper-case
            properCasedWords.Add(rawWords.First().UpperCaseFirstChar());

            // Find correct casing for subsequent words
            foreach (string word in rawWords.Skip(1))
            {
                var matchingLowerCaseWord =
                    s_lowerCaseWords.FirstOrDefault(lcw =>
                        word.Equals(lcw, StringComparison.InvariantCultureIgnoreCase));

                properCasedWords.Add(matchingLowerCaseWord ?? word.UpperCaseFirstChar());
            }
        }

        return string.Join(' ', properCasedWords);
    }

    public static bool Matches(this string text, string matchingText)
    {
        return text.Equals(matchingText, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string ToHtmlFileName(this string filename)
    {
        return filename
            .ToLowerInvariant()
            .Replace("  ", " ")
            .Replace(" ", "-")
            .Replace(".md", ".html");
    }

    #region Private methods

    private static string UpperCaseFirstChar(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }

        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);

        return new string(a);
    }

    #endregion
}