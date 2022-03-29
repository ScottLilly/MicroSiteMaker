using System.Text;
using MicroSiteMaker.Models;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
    public static void CreateInputDirectoriesAndDefaultFiles(WebSite webSite)
    {
        Directory.CreateDirectory(webSite.ProjectDirectory);
        Directory.CreateDirectory(webSite.InputRootDirectory);
        Directory.CreateDirectory(webSite.InputPagesDirectory);
        Directory.CreateDirectory(webSite.InputTemplatesDirectory);
        Directory.CreateDirectory(webSite.InputImagesDirectory);

        CreateFile(webSite.InputTemplatesDirectory, "stylesheet.css", DefaultStyleSheet());
        CreateFile(webSite.InputTemplatesDirectory, "page-template.html", DefaultWebPageTemplate(webSite));

        CreateFile(webSite.InputPagesDirectory, "about.md", DefaultAboutPageMarkdown(webSite));
        CreateFile(webSite.InputPagesDirectory, "privacy-policy.md", DefaultPrivacyPolicyMarkdown(webSite));
        CreateFile(webSite.InputPagesDirectory, "contact.md", DefaultContactPageMarkdown(webSite));
    }

    public static void CreateOutputDirectoriesAndFiles(WebSite webSite)
    {
        Directory.CreateDirectory(webSite.OutputRootDirectory);
        Directory.CreateDirectory(webSite.OutputImagesDirectory);
        Directory.CreateDirectory(webSite.OutputCssDirectory);

        CopyCssFilesToOutputDirectory(webSite);
        CopyImageFilesToOutputDirectory(webSite);
        BuildPagesInOutputDirectory(webSite);
    }

    private static void CopyCssFilesToOutputDirectory(WebSite webSite)
    {
        foreach (FileInfo fileInfo in GetFilesOfExtension(webSite.InputTemplatesDirectory, "css"))
        {
            File.Copy(fileInfo.FullName, Path.Combine(webSite.OutputCssDirectory, fileInfo.Name));
        }
    }

    private static void CopyImageFilesToOutputDirectory(WebSite webSite)
    {
    }

    private static void BuildPagesInOutputDirectory(WebSite website)
    {
        var templateLines = GetPageTemplateLines(website);

        foreach (FileInfo fileInfo in GetFilesOfExtension(website.InputPagesDirectory, "md"))
        {
            var inputLines = File.ReadAllLines(fileInfo.FullName);
            var htmlLines = HtmlLinesFromMarkdownLines(inputLines);

            var outputLines = new List<string>();

            foreach (string templateLine in templateLines)
            {
                if (templateLine.StartsWith("{{page-content}}"))
                {
                    outputLines.AddRange(htmlLines);
                }
                else
                {
                    outputLines.Add(
                        templateLine
                            .Replace("{{page-name}}", fileInfo.Name));
                }
            }

            CreateFile(website.OutputRootDirectory,
                $"{MarkdownFileNameToHtmlFileName(fileInfo.Name)}", outputLines);
        }
    }

    private static List<string> GetPageTemplateLines(WebSite website)
    {
        return File.ReadAllLines(Path.Combine(website.InputTemplatesDirectory, website.TemplateFileName))
            .Select(line => line
                .Replace("{{website-name}}", website.Name)
                .Replace("{{stylesheet-name}}", website.CssFileName))
            .ToList();
    }

    private static List<string> HtmlLinesFromMarkdownLines(IEnumerable<string> markdownLines)
    {
        var htmlLines = new List<string>();

        foreach (string markdownFileLine in markdownLines)
        {
            var words = markdownFileLine.Split(" ");

            if (words.Length == 0)
            {
                htmlLines.Add("");
                continue;
            }

            var firstWord = words[0];
            var remainingWords = string.Join(" ", words.Skip(1));

            switch (firstWord)
            {
                case "#":
                    htmlLines.Add($"<H1>{remainingWords}</H1>");
                    break;
                case "##":
                    htmlLines.Add($"<H2>{remainingWords}</H2>");
                    break;
                case "###":
                    htmlLines.Add($"<H3>{remainingWords}</H3>");
                    break;
                default:
                    htmlLines.Add(markdownFileLine);
                    break;
            }
        }

        return htmlLines;
    }

    private static string MarkdownFileNameToHtmlFileName(string filename)
    {
        return filename.ToLowerInvariant().Replace("  ", " ").Replace(" ", "-").Replace(".md", ".html");
    }

    private static IEnumerable<string> DefaultStyleSheet()
    {
        return new List<string>
        {
            "font-family: Arial, Helvetica, sans-serif;",
            "h1 {",
            "   text-align: center;",
            "   color: #0000b8;",
            "}",
            "h2,",
            "h3 {",
            "   text-align: center;",
            "   color: #0000b8;",
            "   padding-top: 25px;",
            "}"
        };
    }

    private static IEnumerable<string> DefaultWebPageTemplate(WebSite webSite)
    {
        return new List<string>
        {
            "<html>",
            "<head>",
            "<title>{{website-name}} - {{page-name}}</title>",
            "<link rel=\"stylesheet\" type=\"text/css\" href=\"css\\{{stylesheet-name}}\" media=\"screen\">",
            "</head>",
            "<body style=\"width: 500px;margin: auto;\">",
            "{{page-content}}",
            "</body>",
            "</html>"
        };
    }

    private static IEnumerable<string> DefaultAboutPageMarkdown(WebSite webSite)
    {
        return new List<string>
        {
            $"## About {webSite.Name}",
            "",
            $"Welcome to {webSite.Name}"
        };
    }

    private static IEnumerable<string> DefaultPrivacyPolicyMarkdown(WebSite webSite)
    {
        return new List<string>
        {
            "## Privacy Policy",
            "",
            $"{webSite.Name} does not track any of your personal information."
        };
    }

    private static IEnumerable<string> DefaultContactPageMarkdown(WebSite webSite)
    {
        return new List<string>
        {
            "## Contact Information",
            "",
            $"Please contact {webSite.Name} here:"
        };
    }

    private static List<FileInfo> GetFilesOfExtension(string path, string extension)
    {
        return Directory.GetFiles(path)
            .Select(f => new FileInfo(f))
            .Where(f => f.Extension.Replace(".", "")
                .Equals(extension.Replace(".", ""), StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private static void CreateFile(string path, string filename, IEnumerable<string> contents)
    {
        File.WriteAllLines(Path.Combine(path, filename), contents);
    }
}