# PlaywrightBDD

BDD-style UI and API test automation for .NET, built with [Playwright](https://playwright.dev/dotnet/), [Reqnroll](https://reqnroll.net/) (Gherkin), NUnit and Allure reporting.

## Tech Stack

| Area | Technology |
| --- | --- |
| Runtime | .NET 10 (`net10.0`) |
| Browser automation | Microsoft.Playwright 1.62.0 |
| BDD | Reqnroll 3.3.4 + Reqnroll.NUnit |
| Test runner | NUnit 4.3.2 |
| Reporting | Allure (Allure.NUnit / Allure.Reqnroll) |
| CI/CD | Azure Pipelines |

## Project Structure

```
PlaywrightBDD.slnx
azure-pipelines.yml
PlaywrightBDD.Framework/        # Reusable framework (Reqnroll binding assembly)
├── Drivers/PlaywrightDriver.cs # Playwright browser/context/page lifecycle
├── Hooks/TestHooks.cs          # Reqnroll hooks (setup/teardown, screenshots)
└── Support/TestSettings.cs     # Strongly-typed settings
PlaywrightBDD.Tests/            # Test project
├── Features/
│   ├── UI/BasicNavigation.feature
│   └── API/PublishBook.feature
├── StepDefinitions/{UI,API}/   # Step bindings
├── Pages/HomePage.cs           # Page objects
├── appsettings.json            # Run settings
├── allureConfig.json           # Allure configuration
└── reqnroll.json               # Registers the Framework as a binding assembly
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PowerShell (`pwsh`) to install Playwright browsers
- (Optional) [Allure CLI](https://allurereport.org/docs/install/) to view reports locally

## Getting Started

```bash
git clone <repo-url>
cd PlaywrightBDD
dotnet build

# Install Playwright browsers (one-time)
pwsh PlaywrightBDD.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
```

## Configuration

Settings live in [PlaywrightBDD.Tests/appsettings.json](PlaywrightBDD.Tests/appsettings.json) and can be overridden with environment variables.

| Setting | Default | Description |
| --- | --- | --- |
| `BaseUrl` | `https://playwright.dev` | Application under test |
| `Browser` | `chromium` | Browser to launch |
| `Headless` | `false` | Run without a visible browser |
| `SlowMoMilliseconds` | `0` | Slow down each action |
| `DefaultTimeoutMilliseconds` | `30000` | Default Playwright timeout |
| `ScreenshotsOnFailure` | `true` | Capture screenshots on failing scenarios |
| `ScreenshotsDirectory` | `Screenshots` | Screenshot output folder |
| `VideoRecording` | `false` | Record videos |
| `EnableTracing` | `false` | Enable Playwright tracing |
| `ViewportWidth` / `ViewportHeight` | `1920` / `1080` | Browser viewport |

## Running Tests

```bash
# All tests
dotnet test

# Filter by feature/category
dotnet test --filter "TestCategory=smoke"
```

## Included Features

- **UI – Playwright.dev Navigation**: verifies the home page title and navigation to the installation guide.
- **API – Book Registration**: obtains a token, then publishes a book entry (scenario outline with examples).

## Allure Reporting

Results are written to `allure-results` in the test output folder `PlaywrightBDD.Tests/bin/Debug/net10.0/allure-results`.
To open reporst on your local machine use following commands after running your tests
```bash
allure generate bin/Debug/net10.0/allure-results -o allure-report --clean
allure open allure-report
```

## CI/CD

[azure-pipelines.yml](azure-pipelines.yml)

Installs .NET 10, restores, builds (Release), installs Chromium, runs tests, and publishes TRX results, the Allure report and raw results, screenshots, and videos as artifacts.
Add following variables in your pipeline
```bash
BROWSER=Chromium
HEADLESS=true
VIDEO_RECORDING=true
```

## Adding a New Test

1. Add a `.feature` file under `PlaywrightBDD.Tests/Features/`.
2. Implement the steps in `PlaywrightBDD.Tests/StepDefinitions/`.
3. Add page objects to `PlaywrightBDD.Tests/Pages/` as needed.
4. Run `dotnet test`.
