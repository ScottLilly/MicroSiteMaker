using MicroSiteMaker.Models;
using MicroSiteMaker.Services;

namespace MicroSiteMaker.ViewModels;

public class MicroSiteViewModel
{
    public WebSite Site { get; set; }

    public MicroSiteViewModel()
    {
        Site = new WebSite();
    }

    public Dictionary<string, string> GetWebPages()
    {
        return SiteBuilderService.GetWebPageFiles(Site);
    }
}