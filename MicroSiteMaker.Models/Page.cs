using System.Text.RegularExpressions;
using MicroSiteMaker.Core;

namespace MicroSiteMaker.Models;

public class Page : IHtmlPageSource
{
    private const string REGEX_CATEGORIES = @"{{Categories:(.*?)}}";
    private const string REGEX_META_TAG_DESCRIPTION = @"{{Meta-Description:(.*?)}}";
    private const string REGEX_TITLE_OVERRIDE = @"{{Title:(.*?)}}";

    private readonly FileInfo _fileInfo;

    public bool IncludeInCategories =>
        !Categories.Any(c => c.Equals("Exclude", StringComparison.InvariantCultureIgnoreCase));
    public List<string> InputFileLines { get; } = new List<string>();
    public List<string> OutputLines { get; } = new List<string>();
    public List<string> Categories { get; } = new List<string>();

    public string FileName => _fileInfo.Name;
    public string FileNameWithoutExtension =>
        Path.GetFileNameWithoutExtension(_fileInfo.Name);
    public string Title { get; private set; }
    public string MetaTagDescription { get; private set; }

    public DateTime FileDateTime => _fileInfo.CreationTime;
    public string HtmlFileName =>
        FileName.ToLowerInvariant().Replace("  ", " ").Replace(" ", "-").Replace(".md", ".html");

    public Page(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        Title = FileNameWithoutExtension.ToProperCase();

        LoadMarkdownLines(fileInfo);
    }

    private void LoadMarkdownLines(FileSystemInfo fileInfo)
    {
        foreach (string line in File.ReadAllLines(fileInfo.FullName))
        {
            MatchCollection categories = Regex.Matches(line, REGEX_CATEGORIES);
            MatchCollection metaTagDescription = Regex.Matches(line, REGEX_META_TAG_DESCRIPTION);
            MatchCollection titleOverride = Regex.Matches(line, REGEX_TITLE_OVERRIDE);

            // Handle Categories line
            if (categories.Count > 0)
            {
                foreach (Match match in categories)
                {
                    if (match.Success && match.Groups.Count > 0)
                    {
                        Categories.AddRange(match.Groups[1].Value.Split(","));
                    }
                }

                continue;
            }

            // Handle meta tag description line
            if (metaTagDescription.Count > 0)
            {
                Match match = metaTagDescription.First();
                if (match.Success && match.Groups.Count > 0)
                {
                    MetaTagDescription = match.Groups[1].Value;
                }

                continue;
            }

            // Handle Title override line
            if (titleOverride.Count > 0)
            {
                Match match = titleOverride.First();
                if (match.Success && match.Groups.Count > 0)
                {
                    Title = match.Groups[1].Value;
                }

                continue;
            }

            // Handle non-special lines
            InputFileLines.Add(line);
        }
    }
}