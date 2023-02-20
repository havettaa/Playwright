using Microsoft.Playwright;
using System.IO;
using System.Threading.Tasks;

namespace Palfinger.CoreServices.E2E.Base.Services;

public class UrlService
{
    private readonly string _baseUrl;

    public UrlService(string baseUrl)
    {
        _baseUrl = baseUrl;
    }

    public async Task NavigateTo(IPage page, string urlExtension = "")
    {
        await page.GotoAsync(Path.Combine(_baseUrl, urlExtension), new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    }
}