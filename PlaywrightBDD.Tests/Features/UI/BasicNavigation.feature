Feature: Playwright.dev Navigation
  As a visitor to the Playwright documentation site
  I want to navigate and search the site
  So that I can find information about Playwright

  Scenario: Home page loads with correct title
    Given I open url located at "https://playwright.dev"
    Then The page title should contain "Playwright"

  Scenario: Get started link leads to the installation guide
    Given I open url located at "https://playwright.dev"
    When I click the "Get started" link
    Then the page heading "Installation" should be visible
