using MicroSiteMaker.Models;
using MicroSiteMaker.ViewModels;
using Xunit;

namespace Test.MicroSiteMaker.ViewModels;

public class TestMicroSiteViewModel
{
    [Fact]
    public void Test_Instantiate()
    {
        var viewModel = new MicroSiteViewModel();

        Assert.NotNull(viewModel);
    }

    [Fact]
    public void Test_Populate()
    {
        var viewModel = new MicroSiteViewModel();
        viewModel.Site.Name = "My Test Website";
        viewModel.Site.BaseUrl = "mytestwebsite.com";
        viewModel.Site.HttpsEnabled = true;

        viewModel.Site.Page.Add(new WebPage {Title = "Privacy Policy"});

        var pages = viewModel.GetWebPages();

        Assert.Equal(2, pages.Count);
    }

}