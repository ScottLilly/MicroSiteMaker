using System.Text.RegularExpressions;
using Markdig;
using MicroSiteMaker.Models;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
    private static bool s_follow = true;
    private static bool s_index = true;

    public static void SetFollow(bool follow)
    {
        s_follow = follow;
    }

    public static void SetIndex(bool index)
    {
        s_index = index;
    }

    public static void CreateOutputDirectoriesAndFiles(Website website)
    {
        FileService.PopulateWebsiteInputFiles(website);

        FileService.CreateOutputDirectories(website);

        FileService.CopyCssFilesToOutputDirectory(website);
        FileService.CopyImageFilesToOutputDirectory(website);

        CreateOutputPageHtml(website);
        FileService.WriteOutputFiles(website);
        FileService.CreateSitemapFile(website);
        FileService.CreateRobotsTextFile(website);
    }

    private static void CreateOutputPageHtml(Website website)
    {
        var templateLines = FileService.GetPageTemplateLines(website);

        foreach (IHtmlPageSource page in website.PagesAndCategoryPages)
        {
            foreach (string templateLine in templateLines)
            {
                if (templateLine.StartsWith("{{page-content}}"))
                {
                    foreach (string inputLine in page.InputFileLines)
                    {
                        page.OutputLines.Add(GetCleanedHtmlLine(website, page, inputLine));
                    }
                }
                else
                {
                    page.OutputLines.Add(GetCleanedHtmlLine(website, page, templateLine));
                }
            }

            // Build robots tag from parameters
            string followText = s_follow ? "follow" : "nofollow";
            string indexText = s_index ? "index" : "noindex";
            string robotsTag = $"<meta name=\"robots\" content=\"{followText}, {indexText}\">";

            // Check if OutputLines already has robots tag
            var robotsLine =
                page.OutputLines.FirstOrDefault(line => line.Contains("meta ") && line.Contains("robots"));

            // If so, replace. Otherwise, add it
            if(robotsLine == null)
            {
                var closeHeadLine =
                    page.OutputLines.First(line => line.Trim().StartsWith("</head"));
                var closeHeadIndex =
                    page.OutputLines.IndexOf(closeHeadLine);

                page.OutputLines.Insert(closeHeadIndex, robotsTag);
            }
            else
            {
                page.OutputLines[page.OutputLines.IndexOf(robotsLine)] = robotsTag;
            }

            if (!string.IsNullOrWhiteSpace(page.MetaTagDescription))
            {
                var closeHeadLine =
                    page.OutputLines.First(line => line.Trim().StartsWith("</head"));
                var closeHeadIndex =
                    page.OutputLines.IndexOf(closeHeadLine);

                page.OutputLines.Insert(closeHeadIndex, $"<meta name=\"description\" content=\"{page.MetaTagDescription}\">");
            }
        }
    }

    private static string GetCleanedHtmlLine(Website website, IHtmlPageSource page, string line)
    {
        var cleanedLine = line
            .Replace("{{website-name}}", website.Url)
            .Replace("{{page-name}}", page.Title)
            .Replace("{{stylesheet-name}}", website.CssFileName)
            .Replace("{{file-date}}", page.FileDateTime.ToString("dd MMMM yyyy"))
            .Replace("{{date-year}}", DateTime.Now.Year.ToString())
            .Replace("{{date-month}}", DateTime.Now.Month.ToString())
            .Replace("{{date-month-name}}", DateTime.Now.ToString("MMMM"))
            .Replace("{{date-date}}", DateTime.Now.Day.ToString())
            .Replace("{{date-dow}}", DateTime.Now.DayOfWeek.ToString());

        var htmlLine = Markdown.ToHtml(cleanedLine);

        htmlLine = MakeExternalLinksOpenInNewTab(htmlLine, website.Url);

        return htmlLine;
    }

    private static string MakeExternalLinksOpenInNewTab(string htmlLine, string websiteName)
    {
        string hrefPattern = @"(<a href.*?>)";

        Match regexMatch = Regex.Match(htmlLine, hrefPattern,
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            TimeSpan.FromSeconds(1));

        string outputLine = htmlLine;

        while (regexMatch.Success)
        {
            var hrefText = regexMatch.Groups[1].Value;

            if (hrefText.Contains("http", StringComparison.InvariantCultureIgnoreCase) &&
                !hrefText.Contains(websiteName, StringComparison.InvariantCultureIgnoreCase))
            {
                var newHrefText = hrefText;

                if (!newHrefText.Contains(" target=", StringComparison.InvariantCultureIgnoreCase))
                {
                    newHrefText = newHrefText.Replace(">", " target=\"_blank\">");
                }

                if (!newHrefText.Contains(" rel=", StringComparison.InvariantCultureIgnoreCase))
                {
                    newHrefText = newHrefText.Replace(">", " rel=\"nofollow\">");
                }

                outputLine = outputLine.Replace(hrefText, newHrefText);
            }

            regexMatch = regexMatch.NextMatch();
        }

        return outputLine;
    }
}