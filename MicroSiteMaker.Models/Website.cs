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
        Path.Combine(ProjectDirectory, "Input");
    public string InputPagesDirectory =>
        Path.Combine(InputRootDirectory, "Pages");
    public string InputTemplatesDirectory =>
        Path.Combine(InputRootDirectory, "Templates");
    public string InputImagesDirectory =>
        Path.Combine(InputRootDirectory, "Images");

    public string OutputRootDirectory =>
        Path.Combine(ProjectDirectory, "Output");
    public string OutputImagesDirectory =>
        Path.Combine(OutputRootDirectory, "Images");
    public string OutputCssDirectory =>
        Path.Combine(OutputRootDirectory, "CSS");

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