using MicroSiteMaker.Core;

namespace MicroSiteMaker.Models;

public class Website
{
    public Dictionary<string, string> Parameters { get; private set; } =
        new Dictionary<string, string>();
    public List<string> MenuLines { get; } =
        new List<string>();

    public List<IHtmlPageSource> Pages { get; }
    public List<IHtmlPageSource> CategoryPages { get; }
    public IHtmlPageSource PagesByDatePage { get; }

    public List<IHtmlPageSource> AllPages =>
        Pages.Concat(CategoryPages.Concat(new List<IHtmlPageSource> {PagesByDatePage})).ToList();

    public string Url =>
        Parameters.GetValueOrDefault(Constants.Parameters.URL) ?? "";
    public string ProjectDirectory =>
        Parameters.GetValueOrDefault(Constants.Parameters.PATH) ?? "";

    public List<string> ErrorMessages { get; } =
        new List<string>();
    public bool HasErrors => ErrorMessages.Any();

    public string InputRootDirectory =>
        Path.Combine(ProjectDirectory, Constants.Directories.Input.ROOT);
    public string InputPagesDirectory =>
        Path.Combine(InputRootDirectory, Constants.Directories.Input.PAGES);
    public string InputTemplatesDirectory =>
        Path.Combine(InputRootDirectory, Constants.Directories.Input.TEMPLATES);
    public string InputImagesDirectory =>
        Path.Combine(InputRootDirectory, Constants.Directories.Input.IMAGES);

    public string OutputRootDirectory =>
        Path.Combine(ProjectDirectory, Constants.Directories.Output.ROOT);
    public string OutputImagesDirectory =>
        Path.Combine(OutputRootDirectory, Constants.Directories.Output.IMAGES);
    public string OutputCssDirectory =>
        Path.Combine(OutputRootDirectory, Constants.Directories.Output.CSS);

    public string CssFileName =>
        Parameters.GetValueOrDefault(Constants.Parameters.STYLESHEET) ?? "stylesheet.css";

    public string TemplateFileName =>
            Parameters.GetValueOrDefault(Constants.Parameters.TEMPLATE) ?? "page-template.html";

    public Website(IDictionary<string, string> args)
    {
        AssignArguments(args);

        Pages = new List<IHtmlPageSource>();
        CategoryPages = new List<IHtmlPageSource>
        {
            new CategoryPage(Constants.SpecialCategories.UNCATEGORIZED)
        };

        PagesByDatePage = new PageByDatePage();
        PagesByDatePage.InputFileLines.Add("# Pages by Date");
    }

    public void PopulateMenuLines()
    {
        MenuLines.Clear();

        MenuLines.Add("<div class=\"menu\">");
        MenuLines.Add("<ul>");

        MenuLines.Add("<li class=\"dropdown\">");
        MenuLines.Add("<a class=\"dropbtn\" href=\"./about.html\">Home</a>");
        MenuLines.Add("</li>");

        MenuLines.Add("<li class=\"dropdown\">");
        MenuLines.Add("<a class=\"dropbtn\">Pages by Category</a>");
        MenuLines.Add("<div class=\"dropdown-content\">");

        // Add all Category pages, except UNCATEGORIZED
        foreach (var categoryPage in CategoryPages
                     .Where(cp => !cp.Title.Contains(Constants.SpecialCategories.UNCATEGORIZED, StringComparison.InvariantCultureIgnoreCase))
                     .OrderBy(cp=> cp.Title))
        {
            MenuLines.Add($"<a href=\"./{categoryPage.HtmlFileName}\">{categoryPage.Title.Replace("Category: ", "").Trim()}</a>");
        }

        // Add UNCATEGORIZED CategoryPage
        foreach (var categoryPage in CategoryPages
                     .Where(cp => cp.Title.Contains(Constants.SpecialCategories.UNCATEGORIZED, StringComparison.InvariantCultureIgnoreCase))
                     .OrderBy(cp => cp.Title))
        {
            MenuLines.Add($"<a href=\"./{categoryPage.HtmlFileName}\">{categoryPage.Title.Replace("Category: ", "").Trim()}</a>");
        }

        MenuLines.Add("</div>");
        MenuLines.Add("</li>");

        MenuLines.Add("<li class=\"dropdown\">");
        MenuLines.Add("<a class=\"dropbtn\" href=\"./pages-by-date.html\">Pages by Date</a>");
        MenuLines.Add("</li>");

        MenuLines.Add("<li class=\"dropdown\">");
        MenuLines.Add("<a class=\"dropbtn\">Site Information</a>");
        MenuLines.Add("<div class=\"dropdown-content\">");
        MenuLines.Add("<a href=\"./contact.html\">Contact</a>");
        MenuLines.Add("<a href=\"./privacy-policy.html\">Privacy Policy</a>");
        MenuLines.Add("</div>");
        MenuLines.Add("</li>");

        MenuLines.Add("</ul>");
        MenuLines.Add("</div>");
    }

    private void AssignArguments(IDictionary<string, string> args)
    {
        Parameters = new Dictionary<string, string>(args);

        if (string.IsNullOrWhiteSpace(Url))
        {
            ErrorMessages.Add($"Parameter '{Constants.Parameters.URL}' is required.");
        }

        if (string.IsNullOrWhiteSpace(ProjectDirectory))
        {
            ErrorMessages.Add($"Parameter '{Constants.Parameters.PATH}' is required.");
        }
    }
}