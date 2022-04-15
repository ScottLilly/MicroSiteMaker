using System.Text.RegularExpressions;
using MicroSiteMaker.Core;

namespace MicroSiteMaker.Models;

public class Page : IHtmlPageSource
{
    private readonly FileInfo _fileInfo;

    private string FileName => _fileInfo.Name;
    private string FileNameWithoutExtension =>
        Path.GetFileNameWithoutExtension(_fileInfo.Name);

    public string Title { get; private set; }
    public string MetaTagDescription { get; private set; } = "";
    public List<string> InputFileLines { get; } = new List<string>();
    public List<string> OutputLines { get; } = new List<string>();
    public List<string> Categories { get; } = new List<string>();

    public bool IncludeInCategories =>
        !Categories.Any(c => c.Matches(Constants.SpecialCategories.EXCLUDE));
    public DateTime FileDateTime => _fileInfo.CreationTime;
    public string HtmlFileName => FileName.ToHtmlFileName();

    public Page(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        Title = FileNameWithoutExtension.ToProperCase();

        LoadMarkdownLines(fileInfo);
    }

    #region Private methods

    private void LoadMarkdownLines(FileSystemInfo fileInfo)
    {
        foreach (string line in File.ReadAllLines(fileInfo.FullName))
        {
            if (LineContainsCategories(line) ||
                LineContainsMetaTagDescription(line) ||
                LineContainsTitleOverride(line))
            {
                continue;
            }

            InputFileLines.Add(line);
        }
    }

    private bool LineContainsCategories(string line)
    {
        MatchCollection categories =
            Regex.Matches(line, Constants.Regexes.CATEGORIES);

        if (categories.Count > 0)
        {
            foreach (Match match in categories)
            {
                if (match.Success && match.Groups.Count > 0)
                {
                    Categories.AddRange(match.Groups[1].Value.Split(","));

                    return true;
                }
            }
        }

        return false;
    }

    private bool LineContainsMetaTagDescription(string line)
    {
        MatchCollection metaTagDescription =
            Regex.Matches(line, Constants.Regexes.META_TAG_DESCRIPTION);

        if (metaTagDescription.Count > 0)
        {
            Match match = metaTagDescription.First();

            if (match.Success && match.Groups.Count > 0)
            {
                MetaTagDescription = match.Groups[1].Value;

                return true;
            }
        }

        return false;
    }

    private bool LineContainsTitleOverride(string line)
    {
        MatchCollection titleOverride =
            Regex.Matches(line, Constants.Regexes.TITLE_OVERRIDE);

        if (titleOverride.Count > 0)
        {
            Match match = titleOverride.First();

            if (match.Success && match.Groups.Count > 0)
            {
                Title = match.Groups[1].Value;

                return true;
            }
        }

        return false;
    }

    #endregion
}