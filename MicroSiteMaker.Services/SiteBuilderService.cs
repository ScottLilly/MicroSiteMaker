using System.Text;
using MicroSiteMaker.Models;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
    public static void CreateDirectoriesAndDefaultFiles(WebSite webSite)
    {
        Directory.CreateDirectory(webSite.ProjectDirectory);

        string inputRootDirectory = Path.Combine(webSite.ProjectDirectory, "Input");
        string inputPagesDirectory = Path.Combine(inputRootDirectory, "Pages");
        string inputTemplatesDirectory = Path.Combine(inputRootDirectory, "Templates");

        Directory.CreateDirectory(inputRootDirectory);
        Directory.CreateDirectory(inputPagesDirectory);
        Directory.CreateDirectory(inputTemplatesDirectory);
        Directory.CreateDirectory(Path.Combine(inputRootDirectory, "Images"));

        string outputRootDirectory = Path.Combine(webSite.ProjectDirectory, "Output");
        Directory.CreateDirectory(outputRootDirectory);
        Directory.CreateDirectory(Path.Combine(outputRootDirectory, "Images"));
        Directory.CreateDirectory(Path.Combine(outputRootDirectory, "CSS"));

        CreateFile(inputTemplatesDirectory, "stylesheet.css", GetStyleSheetText());

        CreateFile(inputPagesDirectory, "about.md", GetAboutPageMarkdown(webSite));
        CreateFile(inputPagesDirectory, "privacy-policy.md", GetPrivacyPolicyMarkdown(webSite));
        CreateFile(inputPagesDirectory, "contact.md", GetContactPageMarkdown(webSite));
    }

    private static string GetWebPageText(WebSite webSite, WebPage webPage)
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

    private static IEnumerable<string> GetStyleSheetText()
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

    private static IEnumerable<string> GetAboutPageMarkdown(WebSite webSite)
    {
        return new List<string>
        {
            $"## About {webSite.Name}",
            "",
            $"Welcome to {webSite.Name}"
        };
    }

    private static IEnumerable<string> GetPrivacyPolicyMarkdown(WebSite webSite)
    {
        return new List<string>
        {
            "## Privacy Policy",
            "",
            $"{webSite.Name} does not track any of your personal information."
        };
    }

    private static IEnumerable<string> GetContactPageMarkdown(WebSite webSite)
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