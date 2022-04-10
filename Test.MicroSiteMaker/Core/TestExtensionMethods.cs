using MicroSiteMaker.Core;
using Xunit;

namespace Test.MicroSiteMaker.Core;

public class TestExtensionMethods
{
    [Fact]
    public void Test_ProperCasePageTitle()
    {
        Assert.Equal("Page Not Found", "page-not-found".ToProperCase());
        Assert.Equal("Page Not Found", "page - not - found".ToProperCase());
        Assert.Equal("This is a Page Title", "this-is-a-page-title".ToProperCase());
        Assert.Equal("Born in the USA", "born in the USA".ToProperCase());
        Assert.Equal("A Star is Born", "a star is born".ToProperCase());
    }
}