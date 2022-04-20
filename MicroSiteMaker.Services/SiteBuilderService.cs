using System.Text.RegularExpressions;
using Markdig;
using MicroSiteMaker.Core;
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
        website.PopulateMenuLines();

        var templateLines = FileService.GetPageTemplateLines(website);

        foreach (IHtmlPageSource page in website.AllPages)
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
                else if (templateLine.StartsWith("{{menu}}"))
                {
                    foreach (string menuLine in website.MenuLines)
                    {
                        page.OutputLines.Add(GetCleanedHtmlLine(website, page, menuLine));
                    }
                }
                else
                {
                    page.OutputLines.Add(GetCleanedHtmlLine(website, page, templateLine));
                }
            }

            AddRobotsMetaTagToOutputLines(page);
            AddDescriptionMetaTagToOutputLines(page);
        }
    }

    private static void AddRobotsMetaTagToOutputLines(IHtmlPageSource page)
    {
        // Build robots tag from parameters
        string followText = s_follow ? "follow" : "nofollow";
        string indexText = s_index ? "index" : "noindex";
        string robotsTag = $"<meta name=\"robots\" content=\"{followText}, {indexText}\">";

        // Check if OutputLines already has robots tag
        var robotsLine =
            page.OutputLines.FirstOrDefault(line => line.Contains("meta ") && line.Contains("robots"));

        // If robots line does not exist, add it. Otherwise, replace the existing one.
        if (robotsLine == null)
        {
            page.OutputLines.Insert(IndexOfClosingHeadTagInOutputLines(page), robotsTag);
        }
        else
        {
            page.OutputLines[page.OutputLines.IndexOf(robotsLine)] = robotsTag;
        }
    }

    private static void AddDescriptionMetaTagToOutputLines(IHtmlPageSource page)
    {
        if (string.IsNullOrWhiteSpace(page.MetaTagDescription))
        {
            return;
        }

        page.OutputLines.Insert(IndexOfClosingHeadTagInOutputLines(page),
            $"<meta name=\"description\" content=\"{page.MetaTagDescription}\">");
    }

    private static int IndexOfClosingHeadTagInOutputLines(IHtmlPageSource page)
    {
        return page.OutputLines.IndexOf(page.OutputLines.First(line => line.Trim().StartsWith("</head")));
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

        return MakeExternalLinksOpenInNewTab(htmlLine, website.Url);
    }

    private static string MakeExternalLinksOpenInNewTab(string htmlLine, string websiteName)
    {
        Match regexMatch =
            Regex.Match(htmlLine, Constants.Regexes.A_HREF,
                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                TimeSpan.FromSeconds(1));

        string cleanedLine = htmlLine;

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

                cleanedLine = cleanedLine.Replace(hrefText, newHrefText);
            }

            regexMatch = regexMatch.NextMatch();
        }

        return cleanedLine;
    }
}