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

        CreateFile(webSite.InputTemplatesDirectory, "stylesheet.css", DefaultStyleSheetText());

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
        BuildPagesInOutputDirectory(webSite);
    }

    private static void CopyCssFilesToOutputDirectory(WebSite webSite)
    {
        foreach (FileInfo fileInfo in GetFilesOfExtension(webSite.InputTemplatesDirectory, "css"))
        {
            File.Copy(fileInfo.FullName, Path.Combine(webSite.OutputCssDirectory, fileInfo.Name));
        }
    }

    private static void BuildPagesInOutputDirectory(WebSite website)
    {
        foreach (FileInfo fileInfo in GetFilesOfExtension(website.InputPagesDirectory, "md"))
        {
            Console.WriteLine(fileInfo.FullName);
        }
    }

    private static List<FileInfo> GetFilesOfExtension(string path, string extension)
    {
        return Directory.GetFiles(path)
            .Select(f => new FileInfo(f))
            .Where(f => f.Extension.Replace(".", "")
                .Equals(extension.Replace(".", ""), StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private static string GetWebPageText(WebSite webSite)
    {
        StringBuilder text = new StringBuilder();

        text.AppendLine("<html>");

        text.AppendLine("<head>");
        text.AppendLine($"<title>{webSite.Name}</title>");
        text.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"stylesheet.css\" media=\"screen\">");
        text.AppendLine("</head>");

        text.AppendLine("<body>");
        text.AppendLine("</body>");

        text.AppendLine("</html>");

        return text.ToString();
    }

    private static IEnumerable<string> DefaultStyleSheetText()
    {
        return new List<string>
        {
            "font-family: Arial, Helvetica, sans-serif;",
            // Content width and alignment
            ".left, .right {",
            "   float: left;",
            "   width: 20%;",
            "}",
            ".main {",
            "   float: left;",
            "   width: 60%;",
            "}",
            // If the screen is less than 800 pixels wide, use 100% of it
            "@media screen and (max-width: 800px) {",
            "   .left, .main, .right {",
            "      width: 100%;",
            "   }",
            "}",
            // Text formatting
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

    private static void CreateFile(string path, string filename, IEnumerable<string> contents)
    {
        File.WriteAllLines(Path.Combine(path, filename), contents);
    }
}