using System.Text;
using MicroSiteMaker.Models;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
    public static Dictionary<string, string> GetWebPageFiles(WebSite webSite)
    {
        return webSite.Page.ToDictionary(webPage =>
            $"{CleanedWebPageTitle(webPage, webSite.PageTitleSpaceReplacementCharacter)}",
            webPage => GetWebPageText(webSite, webPage));
    }

    private static string GetWebPageText(WebSite webSite, WebPage webPage)
    {
        StringBuilder text = new StringBuilder();

        text.AppendLine("<html>");
        text.AppendLine("<head>");
        text.Append("<title>");
        text.Append(webSite.Name);
        text.AppendLine("</title>");
        text.AppendLine("<style>");
        text.AppendLine("div {");
        text.AppendLine("width: 500px;");
        text.AppendLine("margin: auto;");
        text.AppendLine("}");
        text.AppendLine("h1 {");
        text.AppendLine("text-align: center;");
        text.AppendLine("color: #0000b8;");
        text.AppendLine("}");
        text.AppendLine("h2,");
        text.AppendLine("h3 {");
        text.AppendLine("text-align: center;");
        text.AppendLine("color: #0000b8;");
        text.AppendLine("padding-top: 25px;");
        text.AppendLine("}");
        text.AppendLine("</style>");
        text.AppendLine("</head>");



        text.AppendLine("</html>");

        return text.ToString();
    }

    private static string CleanedWebPageTitle(WebPage webPage, string spaceReplacementCharacter)
    {
        return webPage.Title.ToLower().Replace(" ", spaceReplacementCharacter) + ".html";
    }
}