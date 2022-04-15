namespace MicroSiteMaker.Models;

public class PageByDatePage : IHtmlPageSource
{
    public bool IsSiteInformationPage => false;
    public bool IncludeInCategories => false;
    public string HtmlFileName { get; }
    public string MetaTagDescription { get; } = "";
    public List<string> InputFileLines { get; }
    public List<string> OutputLines { get; }
    public string Title { get; } = "Pages by Date";
    public DateTime FileDateTime { get; } = DateTime.Now;

    public PageByDatePage()
    {
        HtmlFileName = "pages-by-date.html";
        InputFileLines = new List<string>();
        OutputLines = new List<string>();
    }
}