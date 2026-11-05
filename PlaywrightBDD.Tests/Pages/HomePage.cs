namespace PlaywrightBDD.Tests.Pages;

public class HomePage
{
    private readonly IPage _page;
    public HomePage(IPage page)
    {
        _page = page;
    }

    private ILocator GetStartedLink => _page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
    private ILocator SearchButton => _page.GetByRole(AriaRole.Button, new() { Name = "Search" });
    private ILocator SearchInput => _page.GetByPlaceholder("Search docs");
    public async Task ClickGetStartedAsync()
    {
        await GetStartedLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task SearchForAsync(string term)
    {
        await SearchButton.ClickAsync();
        await SearchInput.FillAsync(term);
        await SearchInput.PressAsync("Enter");
    }

    public async Task<bool> IsHeadingVisibleAsync(string headingText)
    {
        var heading = _page.GetByRole(AriaRole.Heading, new() { Name = headingText });
        try
        {
            await heading.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }
}