using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

SetDefaultExpectTimeout(10_000);

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Firefox.LaunchAsync(new()
{
    Headless = false,
    SlowMo = 50,
});
string loginname = Environment.GetEnvironmentVariable("AUTOEXER_TEST_EMAIL")!;
string password = Environment.GetEnvironmentVariable("AUTOEXER_TEST_PASSWORD")!;

Console.WriteLine("Starting Test");
var page = await browser.NewPageAsync();
await page.GotoAsync("https://automationexercise.com/");
Console.WriteLine("Opening Website");
//await page.PauseAsync();
Console.WriteLine("Accessing Login Page");
await page.GetByText("Signup / Login").ClickAsync();
await page.WaitForURLAsync("**/login");
Console.WriteLine("Should see our login page now");
await page.Locator("[data-qa='login-email']").FillAsync(loginname);
await page.Locator("[data-qa='login-password']").FillAsync(password);
await page.Locator("[data-qa='login-button']").ClickAsync();
Console.WriteLine("Clicked to login");
Console.WriteLine("Back to Home");
await page.WaitForURLAsync("https://automationexercise.com/");
await Expect(page.GetByText("Logged in as Michael")).ToBeVisibleAsync();
Console.WriteLine("Login text verification and sending Screenshot.");
await page.ScreenshotAsync(new()
{
    Path = "screenshot.png",
});



