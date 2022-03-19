using System.Text;
using MicroSiteMaker.Models;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
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

        text.AppendLine("div {");
        text.AppendLine("   width: 500px;");
        text.AppendLine("   margin: auto;");
        text.AppendLine("}");
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