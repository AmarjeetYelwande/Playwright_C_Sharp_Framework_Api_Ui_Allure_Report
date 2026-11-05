using Allure.Net.Commons;
using NUnit.Framework;
using PlaywrightBDD.Framework.Base;
using Reqnroll.BoDi;
using Reqnroll;
using PlaywrightBDD.Framework.Drivers;
using PlaywrightBDD.Framework.Support;

namespace PlaywrightBDD.Framework.Hooks;

[Binding]
public class TestHooks(ScenarioContext scenarioContext, PlaywrightUiDriver uiDriver, PlaywrightApiDriver apiDriver)
{
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        var settings = TestSettings.Load();
        if (settings.ScreenshotsOnFailure && !Directory.Exists(settings.ScreenshotsDirectory))
        {
            Directory.CreateDirectory(settings.ScreenshotsDirectory);
        }
    }
    
    [BeforeScenario(Order = 0)]
    public static void RegisterSettings(IObjectContainer objectContainer)
    {
        objectContainer.RegisterInstanceAs(TestSettings.Load());
    }

    [BeforeScenario(Order = 1)]
    public async Task BeforeScenario()
    {
        await uiDriver.InitializeAsync();
        await apiDriver.InitializeAsync();
    }

    [AfterStep]
    public async Task AfterStep()
    {
        var settings = TestSettings.Load();
        if (!settings.ScreenshotsOnFailure)
        {
            return;
        }

        if (scenarioContext.TestError is null)
        {
            return;
        }

        var fileName = $"{scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png"
            .Replace(' ', '_');
        var filePath = Path.Combine(settings.ScreenshotsDirectory, fileName);

        var screenshot = await uiDriver.CaptureScreenshotAsync();
        await File.WriteAllBytesAsync(filePath, screenshot);
        TestContext.AddTestAttachment(filePath, "Failure screenshot");
        AllureApi.AddAttachment("Failure screenshot", "image/png", screenshot, ".png");
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        await uiDriver.DisposeAsync();
    }
}
