using System.Text.RegularExpressions;
using MicroSiteMaker.Core;

namespace MicroSiteMaker.Models;

public class Page
{
    private const string REGEX_CATEGORIES = @"\{\{Categories:(.*?)\}\}";

    private readonly FileInfo _fileInfo;

    public List<string> InputFileLines { get; } = new List<string>();
    public List<string> OutputLines { get; } = new List<string>();
    public List<string> Categories { get; } = new List<string>();

    public string FullName => _fileInfo.FullName;
    public string FileName => _fileInfo.Name;
    public string FileNameWithoutExtension =>
        Path.GetFileNameWithoutExtension(_fileInfo.Name);
    public string Title =>
        FileNameWithoutExtension.ToProperCase();

    public DateTime FileDateTime => _fileInfo.CreationTime;
    public string HtmlFileName =>
        FileName.ToLowerInvariant().Replace("  ", " ").Replace(" ", "-").Replace(".md", ".html");

    public Page(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;

        LoadMarkdownLines(fileInfo);
    }

    private void LoadMarkdownLines(FileSystemInfo fileInfo)
    {
        foreach (string line in File.ReadAllLines(fileInfo.FullName))
        {
            MatchCollection categories = Regex.Matches(line, REGEX_CATEGORIES);

            if (categories.Count == 0)
            {
                InputFileLines.Add(line);
            }
            else
            {
                foreach (Match match in categories)
                {
                    if (match.Success && match.Groups.Count > 0)
                    {
                        Categories.AddRange(match.Groups[1].Value.Split(","));
                    }
                }
            }
        }
    }
}