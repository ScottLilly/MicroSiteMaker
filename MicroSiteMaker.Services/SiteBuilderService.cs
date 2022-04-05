using System.Text.RegularExpressions;
using Markdig;
using MicroSiteMaker.Models;

namespace MicroSiteMaker.Services;

public static class SiteBuilderService
{
    public static void CreateOutputDirectoriesAndFiles(Website website)
    {
        FileService.PopulateWebsiteInputFiles(website);

        FileService.CreateOutputDirectories(website);

        FileService.CopyCssFilesToOutputDirectory(website);
        FileService.CopyImageFilesToOutputDirectory(website);

        CreateOutputPageHtml(website);
        FileService.WriteOutputFiles(website);
    }

    private static void CreateOutputPageHtml(Website website)
    {
        var templateLines = FileService.GetPageTemplateLines(website);

        foreach (Page page in website.Pages)
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
        }
    }

    private static string GetCleanedHtmlLine(Website website, Page page, string line)
    {
        var cleanedLine = line
            .Replace("{{website-name}}", website.Name)
            .Replace("{{page-name}}", page.FileNameWithoutExtension)
            .Replace("{{stylesheet-name}}", website.CssFileName)
            .Replace("{{date-year}}", DateTime.Now.Year.ToString())
            .Replace("{{date-month}}", DateTime.Now.Month.ToString())
            .Replace("{{date-month-name}}", DateTime.Now.ToString("MMMM"))
            .Replace("{{date-date}}", DateTime.Now.Day.ToString())
            .Replace("{{date-dow}}", DateTime.Now.DayOfWeek.ToString());

        cleanedLine = MakeExternalLinksOpenInNewTab(cleanedLine, website.Name);

        return Markdown.ToHtml(cleanedLine);
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

            if (!hrefText.Contains(websiteName, StringComparison.InvariantCultureIgnoreCase) &&
                !hrefText.Contains(" target=", StringComparison.InvariantCultureIgnoreCase))
            {
                outputLine =
                    outputLine.Replace(hrefText, hrefText.Replace(">", " target=\"_blank\">"));
            }

            regexMatch = regexMatch.NextMatch();
        }

        return outputLine;
    }
}