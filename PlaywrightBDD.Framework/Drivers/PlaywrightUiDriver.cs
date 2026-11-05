using PlaywrightBDD.Framework.Support;
using Reqnroll;

namespace PlaywrightBDD.Framework.Drivers;

public class PlaywrightUiDriver(TestSettings settings, ScenarioContext scenarioContext) : IAsyncDisposable
{
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    public IBrowserContext Context { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;
    
    private string _videoRecordDirectory = "";

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await LaunchBrowserAsync();
        
        var videoRecording = Environment.GetEnvironmentVariable("VIDEO_RECORDING") ?? (settings.VideoRecording).ToString();
        _videoRecordDirectory = bool.Parse(videoRecording.ToLower()) ? "Videos" : "";
        
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = _videoRecordDirectory,
                ViewportSize = new ViewportSize
                {
                    Width = settings.ViewportWidth,
                    Height = settings.ViewportHeight
                }
            });
        Context.SetDefaultTimeout(settings.DefaultTimeoutMilliseconds);
        if (settings.EnableTracing)
        {
            await StartTracing();
        }
        Page = await Context.NewPageAsync();
    }
    private async Task StartTracing()
    {
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots =  true,
            Sources = true
        });
    }
    private Task<IBrowser> LaunchBrowserAsync()
    {
        var isHeadless = Environment.GetEnvironmentVariable("HEADLESS") ?? (settings.Headless).ToString();
        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = bool.Parse(isHeadless.ToLower()),
            SlowMo = settings.SlowMoMilliseconds
        };
        
        var browser = Environment.GetEnvironmentVariable("BROWSER") ?? settings.Browser;
        return browser.ToLower() switch
        {
            "firefox" => Playwright.Firefox.LaunchAsync(launchOptions),
            "webkit" => Playwright.Webkit.LaunchAsync(launchOptions),
            _ => Playwright.Chromium.LaunchAsync(launchOptions)
        };
    }
    public async Task<byte[]> CaptureScreenshotAsync()
    {
        return await Page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
    }
    public async ValueTask DisposeAsync()
    {
        var fileName = $"{scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.zip"
            .Replace(' ', '_');
        var filePath = Path.Combine(settings.TracingDirectory, fileName);

        if (settings.EnableTracing)
        {
            await Context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = filePath
            });
        }

        await Context.CloseAsync();
        await Browser.CloseAsync();
        Playwright?.Dispose();
    }
}