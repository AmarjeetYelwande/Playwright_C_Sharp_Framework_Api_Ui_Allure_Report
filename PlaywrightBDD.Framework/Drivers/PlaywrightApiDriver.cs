using PlaywrightBDD.Framework.Support;

namespace PlaywrightBDD.Framework.Drivers;

public class PlaywrightApiDriver(TestSettings settings) : IAsyncDisposable
{
    private IPlaywright Playwright { get; set; } = null!;
    public IAPIRequestContext Api { get; set; } = null!;
    private IBrowserContext Context { get; set; } = null!;
    
    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Api = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = settings.BaseUrlApi,
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Accept"] = "application/json"
            }
        });
    }
    
    public async ValueTask DisposeAsync()
    {
        await Api.DisposeAsync();
        Playwright?.Dispose();
    }
}