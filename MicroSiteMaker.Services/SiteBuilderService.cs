using System.Drawing;
using System.Reflection;
using Markdig;
using MicroSiteMaker.Core;
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

        CreateFile(webSite.InputTemplatesDirectory, "stylesheet.css", DefaultCssStylesheet());
        CreateFile(webSite.InputTemplatesDirectory, "page-template.html", DefaultWebPageTemplate());

        CreateFile(webSite.InputPagesDirectory, "about.md", DefaultAboutPageMarkdown());
        CreateFile(webSite.InputPagesDirectory, "privacy-policy.md", DefaultPrivacyPolicyPageMarkdown());
        CreateFile(webSite.InputPagesDirectory, "contact.md", DefaultContactPageMarkdown());
    }

    public static void CreateOutputDirectoriesAndFiles(WebSite webSite)
    {
        // Delete existing folders and files
        var rootDirectory = new DirectoryInfo(webSite.OutputRootDirectory);

        if (rootDirectory.Exists)
        {
            rootDirectory.Delete(true);
        }

        // Create the new folders and files
        Directory.CreateDirectory(webSite.OutputRootDirectory);
        Directory.CreateDirectory(webSite.OutputImagesDirectory);
        Directory.CreateDirectory(webSite.OutputCssDirectory);

        CopyCssFilesToOutputDirectory(webSite);
        CopyImageFilesToOutputDirectory(webSite);
        BuildPagesInOutputDirectory(webSite);
    }

    private static void CopyCssFilesToOutputDirectory(WebSite webSite)
    {
        foreach (FileInfo fileInfo in GetFilesWithExtension(webSite.InputTemplatesDirectory, "css"))
        {
            File.Copy(fileInfo.FullName, Path.Combine(webSite.OutputCssDirectory, fileInfo.Name));
        }
    }

    private static void CopyImageFilesToOutputDirectory(WebSite webSite)
    {
        foreach (FileInfo fileInfo in Directory.GetFiles(webSite.InputImagesDirectory)
                     .Select(f => new FileInfo(f)))
        {
            CompressAndCopyImage(fileInfo.FullName, 75, Path.Combine(webSite.OutputImagesDirectory, fileInfo.Name));
        }
    }

    private static void BuildPagesInOutputDirectory(WebSite website)
    {
        var templateLines = GetPageTemplateLines(website);

        foreach (FileInfo fileInfo in GetFilesWithExtension(website.InputPagesDirectory, "md"))
        {
            var outputLines = new List<string>();

            foreach (string templateLine in templateLines)
            {
                if (templateLine.StartsWith("{{page-content}}"))
                {
                    var inputLines = File.ReadAllLines(fileInfo.FullName);

                    foreach (string inputLine in inputLines)
                    {
                        outputLines.Add(Markdown.ToHtml(ReplacedText(inputLine)));
                    }
                }
                else
                {
                    outputLines.Add(
                        ReplacedText(templateLine)
                            .Replace("{{page-name}}", Path.GetFileNameWithoutExtension(fileInfo.Name)));
                }
            }

            CreateFile(website.OutputRootDirectory,
                $"{MarkdownFileNameToHtmlFileName(fileInfo.Name)}", outputLines);
        }
    }

    private static string ReplacedText(string rawText)
    {
        return rawText
            .Replace("{{date-year}}", DateTime.Now.Year.ToString())
            .Replace("{{date-month}}", DateTime.Now.Month.ToString())
            .Replace("{{date-month-name}}", DateTime.Now.ToString("MMMM"))
            .Replace("{{date-date}}", DateTime.Now.Day.ToString())
            .Replace("{{date-dow}}", DateTime.Now.DayOfWeek.ToString());
    }

    private static void CompressAndCopyImage(string originalFilePath, long quality, string outputFilePath) =>
        Image.FromFile(originalFilePath).SaveJpeg(outputFilePath, quality);

    private static List<string> GetPageTemplateLines(WebSite website)
    {
        return File.ReadAllLines(Path.Combine(website.InputTemplatesDirectory, website.TemplateFileName))
            .Select(line => line
                .Replace("{{website-name}}", website.Name)
                .Replace("{{stylesheet-name}}", website.CssFileName))
            .ToList();
    }

    private static string MarkdownFileNameToHtmlFileName(string filename)
    {
        return filename.ToLowerInvariant().Replace("  ", " ").Replace(" ", "-").Replace(".md", ".html");
    }

    private static List<FileInfo> GetFilesWithExtension(string path, string extension)
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

    private static IEnumerable<string> GetLinesFromResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        using StreamReader reader = new StreamReader(stream);

        return reader.ReadToEnd().Split(Environment.NewLine).ToList();
    }

    #region Read resource files for default files

    private static IEnumerable<string> DefaultCssStylesheet() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultCssStylesheet.txt");

    private static IEnumerable<string> DefaultWebPageTemplate() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultWebPageTemplate.txt");

    private static IEnumerable<string> DefaultAboutPageMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultAboutPageMarkdown.txt");

    private static IEnumerable<string> DefaultPrivacyPolicyPageMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultPrivacyPolicyPageMarkdown.txt");

    private static IEnumerable<string> DefaultContactPageMarkdown() =>
        GetLinesFromResource("MicroSiteMaker.Services.Resources.DefaultContactPageMarkdown.txt");

    #endregion
}