using MicroSiteMaker.Core;

namespace MicroSiteMaker.Models;

public class CategoryPage : IHtmlPageSource
{
    public bool IncludeInCategories => false;
    public string CategoryName { get; }
    public string HtmlFileName { get; }
    public List<string> InputFileLines { get; }
    public List<string> OutputLines { get; }
    public string Title { get; }
    public string MetaTagDescription { get; } = "";
    public DateTime FileDateTime { get; } = DateTime.Now;

    public CategoryPage(string categoryName)
    {
        CategoryName = categoryName;
        Title = $"Category: {CategoryName.Replace("-", "").ToProperCase()}";
        HtmlFileName = $"category-{PageName(categoryName)}.html".Replace("--", "-");
        InputFileLines = new List<string> {$"# Category: {categoryName}"};
        OutputLines = new List<string>();
    }

    private static string PageName(string categoryName) =>
        categoryName.ToLowerInvariant().Replace("  ", " ").Replace(" ", "-");
}