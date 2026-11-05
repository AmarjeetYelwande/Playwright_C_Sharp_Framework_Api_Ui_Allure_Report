using Microsoft.Extensions.Configuration;

namespace PlaywrightBDD.Framework.Support;
public class TestSettings
{
    public string BaseUrlApi = "https://conduit-api.bondaracademy.com";
    public string BaseUrl { get; set; } = "https://playwright.dev";
    public string Browser { get; set; } = "chromium";
    public bool Headless { get; set; } = true;
    public int SlowMoMilliseconds { get; set; }
    public int DefaultTimeoutMilliseconds { get; set; } = 30000;
    public bool ScreenshotsOnFailure { get; set; } = true;
    public string ScreenshotsDirectory { get; set; } = "Screenshots";
    public int ViewportWidth { get; set; } = 1920;
    public int ViewportHeight { get; set; } = 1080;
    public bool VideoRecording { get; set; }
    public bool EnableTracing { get; set; }
    public string TracingDirectory { get; set; } = "Tracing";
    
    private static TestSettings? _instance;
    public static TestSettings Load()
    {
        if (_instance is not null)
        {
            return _instance;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        _instance = configuration.Get<TestSettings>() ?? new TestSettings();
        return _instance;
    }
}