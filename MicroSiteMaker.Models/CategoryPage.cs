using MicroSiteMaker.Core;

namespace MicroSiteMaker.Models;

public class CategoryPage : IHtmlPageSource
{
    public string CategoryName { get; }
    public string HtmlFileName { get; }
    public List<string> InputFileLines { get; }
    public List<string> OutputLines { get; }
    public string Title { get; }
    public DateTime FileDateTime { get; } = DateTime.MinValue;

    public CategoryPage(string categoryName)
    {
        CategoryName = categoryName;
        Title = $"Category: {CategoryName.Replace("-", "").ToProperCase()}";
        HtmlFileName = $"category-{PageName(categoryName)}.html".Replace("--", "-");
        InputFileLines = new List<string>();
        OutputLines = new List<string>();
    }

    private string PageName(string categoryName) =>
        categoryName.ToLowerInvariant().Replace("  ", " ").Replace(" ", "-");
}