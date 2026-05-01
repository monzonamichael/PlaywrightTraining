using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

[TestFixture]
public class LoginTests
{
    string loginname = Environment.GetEnvironmentVariable("AUTOEXER_TEST_EMAIL")!;
    string loginpassword = Environment.GetEnvironmentVariable("AUTOEXER_TEST_PASSWORD")!;


    //This section and OneTimeSetup for debugging only, not best practice
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;

    [OneTimeSetUp]
    public async Task GlobalSetUp()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Firefox.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 50
        });
    }

    private async Task Login()
    {
        await _page.Locator("[data-qa='login-email']").FillAsync(loginname);
        await _page.Locator("[data-qa='login-password']").FillAsync(loginpassword);
        await _page.Locator("[data-qa='login-button']").ClickAsync();
        await _page.WaitForURLAsync("**");
    }

    //Always do Setup - Setup fires before each test to execute
    [SetUp]
    public async Task SetUp()
    {
        _page = await _browser!.NewPageAsync();
        //Goal: Click to our login page
        Console.WriteLine("This Setup runs per task");
        await _page.GotoAsync("https://automationexercise.com/");
        await _page.Locator("a[href='/login']").ClickAsync();
        await _page.WaitForURLAsync("**/login");
    }
    
    //Individual tests go here
    [Test]
    public async Task LoginAuthSuccess()
    {
        await Login();
        await Expect(_page.GetByText("Logged in as Michael")).ToBeVisibleAsync();
        Console.WriteLine("Finished checking positive case");
        await _page!.WaitForTimeoutAsync(3000);
    }
    [Test]
    public async Task LoginBadCredential()
    {
        await _page.Locator("[data-qa='login-email']").FillAsync(loginname);
        await _page.Locator("[data-qa='login-password']").FillAsync("badpassword");
        await _page.Locator("[data-qa='login-button']").ClickAsync();
        await Expect(_page.GetByText("Your email or password is incorrect")).ToBeVisibleAsync();
        Console.WriteLine("Got bad password test done");
        await _page!.WaitForTimeoutAsync(3000);
    }

    [Test]
    public async Task LogOutSuccess()
    {
        await Login();
        await _page.Locator("a[href='/logout']").ClickAsync();
        await _page.WaitForURLAsync("**/login");
        await Expect(_page.GetByText("Signup / Login")).ToBeVisibleAsync();
        Console.WriteLine("End logout test");
    }

    //Teardown fires after every test is completed
    [TearDown]
     public async Task TearDown()
    {
        await _page!.CloseAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        await _browser!.CloseAsync();
        _playwright!.Dispose();
    }
}