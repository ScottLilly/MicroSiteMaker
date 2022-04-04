using System.Drawing;
using System.Reflection;
using Markdig;
using MicroSiteMaker.Core;
using MicroSiteMaker.Models;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
    private static bool s_shouldCompressImages = true;
    private static long s_compressPercent = 80;

    public static void SetCompressImages(bool shouldCompress)
    {
        s_shouldCompressImages = shouldCompress;
    }

    public static void SetCompressPercent(long compressPercent)
    {
        s_compressPercent = compressPercent;
    }

    public static void CreateInputDirectoriesAndDefaultFiles(Website website)
    {
        Directory.CreateDirectory(website.ProjectDirectory);
        Directory.CreateDirectory(website.InputRootDirectory);
        Directory.CreateDirectory(website.InputPagesDirectory);
        Directory.CreateDirectory(website.InputTemplatesDirectory);
        Directory.CreateDirectory(website.InputImagesDirectory);

        CreateFile(website.InputTemplatesDirectory, "stylesheet.css", DefaultCssStylesheet());
        CreateFile(website.InputTemplatesDirectory, "page-template.html", DefaultWebPageTemplate());

        CreateFile(website.InputPagesDirectory, "about.md", DefaultAboutPageMarkdown());
        CreateFile(website.InputPagesDirectory, "privacy-policy.md", DefaultPrivacyPolicyPageMarkdown());
        CreateFile(website.InputPagesDirectory, "contact.md", DefaultContactPageMarkdown());
    }

    public static void CreateOutputDirectoriesAndFiles(Website website)
    {
        // Delete existing folders and files
        var rootDirectory = new DirectoryInfo(website.OutputRootDirectory);

        if (rootDirectory.Exists)
        {
            rootDirectory.Delete(true);
        }

        // Populate Website.Pages
        foreach (FileInfo fileInfo in GetFilesWithExtension(website.InputPagesDirectory, "md"))
        {
            website.Pages.Add(new Page(fileInfo));
        }

        // Create the new folders and files
        Directory.CreateDirectory(website.OutputRootDirectory);
        Directory.CreateDirectory(website.OutputImagesDirectory);
        Directory.CreateDirectory(website.OutputCssDirectory);

        CopyCssFilesToOutputDirectory(website);
        CopyImageFilesToOutputDirectory(website);
        CreateOutputPageHtml(website);
        WriteOutputFiles(website);
    }

    private static void CopyCssFilesToOutputDirectory(Website website)
    {
        foreach (FileInfo fileInfo in GetFilesWithExtension(website.InputTemplatesDirectory, "css"))
        {
            File.Copy(fileInfo.FullName, Path.Combine(website.OutputCssDirectory, fileInfo.Name));
        }
    }

    private static void CopyImageFilesToOutputDirectory(Website website)
    {
        foreach (FileInfo fileInfo in Directory.GetFiles(website.InputImagesDirectory)
                     .Select(f => new FileInfo(f)))
        {
            var outputFileName = Path.Combine(website.OutputImagesDirectory, fileInfo.Name);

            if (s_shouldCompressImages)
            {
                CompressAndCopyImage(fileInfo.FullName, s_compressPercent, outputFileName);
            }
            else
            {
                File.Copy(fileInfo.FullName, outputFileName);
            }
        }
    }

    private static void CreateOutputPageHtml(Website website)
    {
        var templateLines = GetPageTemplateLines(website);

        foreach (Page page in website.Pages)
        {
            foreach (string templateLine in templateLines)
            {
                if (templateLine.StartsWith("{{page-content}}"))
                {
                    foreach (string inputLine in page.InputFileLines)
                    {
                        page.OutputLines.Add(Markdown.ToHtml(ReplacedText(inputLine)));
                    }
                }
                else
                {
                    page.OutputLines.Add(
                        ReplacedText(templateLine)
                            .Replace("{{page-name}}", page.FileNameWithoutExtension));
                }
            }
        }
    }

    private static void WriteOutputFiles(Website website)
    {
        foreach (Page page in website.Pages)
        {
            var htmlFileName = MarkdownFileNameToHtmlFileName(page.FileName);

            CreateFile(website.OutputRootDirectory, $"{htmlFileName}", page.OutputLines);

            Console.WriteLine($"Created: {htmlFileName}");
        }

        Console.WriteLine($"Total HTML files created: {website.Pages.Count}");
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

    private static List<string> GetPageTemplateLines(Website website)
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