using System.Collections.ObjectModel;

namespace MicroSiteMaker.Models;

public class WebSite
{
    public string Name { get; set; }

    public string BaseUrl { get; set; }
    public bool HttpsEnabled { get; set; }

    public string PageTitleSpaceReplacementCharacter { get; set; }

    public StyleSheet StyleSheet { get; set; }

    public ObservableCollection<WebPage> Page { get; } =
        new ObservableCollection<WebPage>();

    public WebSite()
    {
        PageTitleSpaceReplacementCharacter = "-";

        StyleSheet = new StyleSheet();
    }
}