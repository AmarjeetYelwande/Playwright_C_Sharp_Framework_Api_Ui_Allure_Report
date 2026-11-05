using NUnit.Framework;
using PlaywrightBDD.Framework.Drivers;
using Reqnroll;

namespace PlaywrightBDD.Framework.Base;

[Binding]
public class CommonSteps(PlaywrightUiDriver uiDriver)
{
    [Given("I open url located at {string}")]
    public Task GivenIOpenUrlLocatedAtString(string url) => uiDriver.Page.GotoAsync(url);

    [Then("The page title should contain {string}")]
    public async Task ThenThePageTitleShouldContainString(string expectedPageTitle)
    {
        var actualPageTitle = await uiDriver.Page.TitleAsync();
        Assert.That(actualPageTitle, Does.Contain(expectedPageTitle));
    }
    
}