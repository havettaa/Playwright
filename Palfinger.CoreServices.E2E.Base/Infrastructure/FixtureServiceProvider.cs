using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Palfinger.CoreServices.E2E.Base.Options;
using Palfinger.CoreServices.E2E.Base.Services;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Palfinger.CoreServices.E2E.Base.Infrastructure;

public class FixtureServiceProvider : IAsyncDisposable
{
    private string _testName;
    [AllowNull] private IPlaywright _playwright;
    [AllowNull] private ServiceProvider _serviceProvider;

    public FixtureServiceProvider()
    {
        _testName = "";
    }

    public async Task<T> GetRequiredServiceAsync<T>() where T : notnull
    {
        if (_serviceProvider == null)
        {
            await SetupAsync();
        }

        return _serviceProvider!.GetRequiredService<T>();
    }

    //[MemberNotNull(nameof(_serviceProvider))]
    private async Task SetupAsync()
    {
        var targetEnv = Environment.GetEnvironmentVariable("TARGETENV");

        var path = Path.GetDirectoryName(typeof(FixtureServiceProvider).Assembly.Location);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(path ?? "/")
            .AddJsonFile($"appsettings.{targetEnv}.json")
            .AddAzureKeyVault(new Uri($"https://kv-coreservices-{targetEnv}.vault.azure.net"), new DefaultAzureCredential())
            .Build();

        _playwright = await Playwright.CreateAsync();
        var browser = await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Convert.ToBoolean(configuration["Headless"]),
        });

        var baseUrl = configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl not set in appsetting.json");
        
        var services = new ServiceCollection();
        services.AddSingleton(browser);
        services.AddScoped(_ => new UrlService(baseUrl));
        services.AddOptions<E2ECredentials>()
            .Bind(configuration.GetSection(nameof(E2ECredentials)))
            .ValidateDataAnnotations();

        _serviceProvider = services.BuildServiceProvider();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_serviceProvider is not null)
        {
            var browser = await GetRequiredServiceAsync<IBrowser>();
            await browser.DisposeAsync();
            await _serviceProvider.DisposeAsync().ConfigureAwait(false);
            _playwright.Dispose();
        }

        _serviceProvider = null;
    }
}
