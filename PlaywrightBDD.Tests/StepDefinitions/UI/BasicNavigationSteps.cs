using PlaywrightBDD.Framework.Base;

namespace PlaywrightBDD.Tests.StepDefinitions.UI;

[Binding]
public class BasicNavigationSteps(PlaywrightUiDriver uiDriver)
{
    private HomePage HomePage => field ??= new HomePage(uiDriver.Page);

    [When(@"I click the ""Get started"" link")]
    public async Task WhenIClickTheGetStartedLink()
    {
        await HomePage.ClickGetStartedAsync();
    }
    
    [When(@"I search for ""(.*)""")]
    public async Task WhenISearchFor(string term)
    {
        await HomePage.SearchForAsync(term);
    }

    [Then(@"the page heading ""(.*)"" should be visible")]
    public async Task ThenThePageHeadingShouldBeVisible(string headingText)
    {
        var isVisible = await HomePage.IsHeadingVisibleAsync(headingText);
        Assert.That(isVisible, Is.True, $"Expected heading '{headingText}' to be visible.");
    }
}