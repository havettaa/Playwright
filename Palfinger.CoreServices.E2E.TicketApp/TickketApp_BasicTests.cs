using Microsoft.Playwright;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Palfinger.CoreServices.E2E.Base.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Xunit;
using Xunit.Abstractions;

namespace Palfinger.CoreServices.E2E.TicketApp;

[Collection("UITests collection")]
public class TickketApp_BasicTests : UITestsBase, IClassFixture<FixtureServiceProvider>
{
    public TickketApp_BasicTests(FixtureServiceProvider serviceProvider, ITestOutputHelper output) : base(serviceProvider, output)
    {
    }

    [Fact(DisplayName = "01 Create Ticket")]
    public async Task TicketAddServiceCase()
    {
        await ExecuteAsync(GetTestName(), async (page) =>
        {
            await _url.NavigateTo(page);

            await page.WaitForSelectorAsync("input");

            await page.WaitForSelectorAsync("[data-cy='open_service_case']");
            await page.ClickAsync("[data-cy='open_service_case']");
        });
    }

    [Fact(DisplayName = "02 Filter Ticket List")]
    public async Task TicketFilterList()
    {
        await ExecuteAsync(GetTestName(), async (page) =>
        {
            await _url.NavigateTo(page);

            await page.WaitForSelectorAsync("input");
            await page.TypeAsync("input[id='UsernameOrEmail']", _credentials.UsernameGeneralAgent);
            await page.TypeAsync("input[id='Password']", _credentials.PasswordGeneralAgent);
            await page.ClickAsync("button[id='loginBtn']");

            await page.WaitForSelectorAsync("input[formcontrolname='search_text']");
            await page.WaitForTimeoutAsync(1500);
            await page.TypeAsync("[formcontrolname='search_text']", "-");
            await page.WaitForTimeoutAsync(1500);
            await page.TypeAsync("[formcontrolname='search_text']", $"-");

            await page.ClickAsync("[data-cy='freshDesk-button']");
            await page.WaitForTimeoutAsync(1500);
        });
    }
}
