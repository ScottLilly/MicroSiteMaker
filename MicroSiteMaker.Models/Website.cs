using System.Collections.ObjectModel;

namespace MicroSiteMaker.Models;

public class WebSite
{
    public ReadOnlyDictionary<string, string> Parameters { get; private set; }

    public string Name =>
        Parameters.GetValueOrDefault("--site") ?? "";
    public string ProjectDirectory =>
        Parameters.GetValueOrDefault("--path") ?? "";

    public List<string> ErrorMessages { get; } =
        new List<string>();
    public bool HasErrors => ErrorMessages.Any();

    public string InputRootDirectory =>
        Path.Combine(ProjectDirectory, "input");
    public string InputPagesDirectory =>
        Path.Combine(InputRootDirectory, "pages");
    public string InputTemplatesDirectory =>
        Path.Combine(InputRootDirectory, "templates");
    public string InputImagesDirectory =>
        Path.Combine(InputRootDirectory, "images");

    public string OutputRootDirectory =>
        Path.Combine(ProjectDirectory, "output");
    public string OutputImagesDirectory =>
        Path.Combine(OutputRootDirectory, "images");
    public string OutputCssDirectory =>
        Path.Combine(OutputRootDirectory, "css");

    public string CssFileName =>
        Parameters.GetValueOrDefault("--stylesheet") ?? "stylesheet.css";

    public string TemplateFileName =>
            Parameters.GetValueOrDefault("--template") ?? "page-template.html";

    public WebSite(IDictionary<string, string> args)
    {
        AssignArguments(args);
    }

    private void AssignArguments(IDictionary<string, string> args)
    {
        Parameters = new ReadOnlyDictionary<string, string>(args);

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessages.Add("Parameter '--site' is required.");
        }

        if (string.IsNullOrWhiteSpace(ProjectDirectory))
        {
            ErrorMessages.Add("Parameter '--path' is required.");
        }
    }
}