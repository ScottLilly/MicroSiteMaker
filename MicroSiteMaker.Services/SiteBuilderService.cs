using System.Text;
using System.Text.RegularExpressions;
using MicroSiteMaker.Models;
using Microsoft.VisualBasic;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
    public static void BuildDirectories(string path)
    {
        Directory.CreateDirectory(path);

        Directory.CreateDirectory(Path.Combine(path, "Input"));
        Directory.CreateDirectory(Path.Combine(path, "Input/Pages"));
        Directory.CreateDirectory(Path.Combine(path, "Input/Templates"));
        Directory.CreateDirectory(Path.Combine(path, "Input/Images"));
        Directory.CreateDirectory(Path.Combine(path, "Input/Snippets"));

        Directory.CreateDirectory(Path.Combine(path, "Output"));
    }

    public static Dictionary<string, string> GetWebPageFiles(WebSite webSite)
    {
        Dictionary<string, string> files =
            new Dictionary<string, string>();

        foreach (WebPage webPage in webSite.Page)
        {
            files.Add(CleanedWebPageTitle(webPage, webSite.PageTitleSpaceReplacementCharacter),
                GetWebPageText(webSite, webPage));
        }

        files.Add("stylesheet.css", GetStyleSheet(webSite.StyleSheet));

        return files;
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

    private static string GetStyleSheet(StyleSheet styleSheet)
    {
        StringBuilder text = new StringBuilder();

        text.AppendLine(".left, .right {");
        text.AppendLine("   float: left;");
        text.AppendLine("   width: 20%;");
        text.AppendLine("}");
        text.AppendLine(".main {");
        text.AppendLine("   float: left;");
        text.AppendLine("   width: 60%;");
        text.AppendLine("}");
        // If the screen is less than 800 pixels wide, use 100% of it
        text.AppendLine("@media screen and (max-width: 800px) {");
        text.AppendLine("   .left, .main, .right {");
        text.AppendLine("      width: 100%;");
        text.AppendLine("   }");
        text.AppendLine("}");

        //text.AppendLine("div {");
        //text.AppendLine("   width: 500px;");
        //text.AppendLine("   margin: auto;");
        //text.AppendLine("}");
        text.AppendLine("h1 {");
        text.AppendLine("   text-align: center;");
        text.AppendLine("   color: #0000b8;");
        text.AppendLine("}");
        text.AppendLine("h2,");
        text.AppendLine("h3 {");
        text.AppendLine("   text-align: center;");
        text.AppendLine("   color: #0000b8;");
        text.AppendLine("   padding-top: 25px;");
        text.AppendLine("}");

        return text.ToString();
    }

    private static string CleanedWebPageTitle(WebPage webPage, string spaceReplacementCharacter)
    {
        return webPage.Title.ToLower().Replace(" ", spaceReplacementCharacter) + ".html";
    }
}