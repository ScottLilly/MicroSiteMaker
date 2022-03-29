using System.Collections.ObjectModel;

namespace MicroSiteMaker.Models;

public class WebSite
{
    public ReadOnlyDictionary<string, string> Parameters { get; private set; }

    public string Name => Parameters.GetValueOrDefault("--site") ?? "";
    public string ProjectDirectory => Parameters.GetValueOrDefault("--path") ?? "";

    public StyleSheet StyleSheet { get; set; }

    public ObservableCollection<WebPage> Pages { get; } =
        new ObservableCollection<WebPage>();

    public bool HasErrors => ErrorMessages.Any();
    public List<string> ErrorMessages { get; } =
        new List<string>();

    public WebSite(IDictionary<string, string> args)
    {
        StyleSheet = new StyleSheet();

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
            ErrorMessages.Add("Path is required for '--path' parameter.");
        }
    }
}