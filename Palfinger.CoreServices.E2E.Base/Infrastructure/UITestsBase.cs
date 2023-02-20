using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Palfinger.CoreServices.E2E.Base.Extensions;
using Palfinger.CoreServices.E2E.Base.Options;
using Palfinger.CoreServices.E2E.Base.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Palfinger.CoreServices.E2E.Base.Infrastructure;

public abstract class UITestsBase : IAsyncLifetime
{
    [AllowNull] private FixtureServiceProvider _serviceProvider;
    [AllowNull] private IBrowser _browser;
    [AllowNull] protected UrlService _url;
    [AllowNull] protected E2ECredentials _credentials;

    protected ITestOutputHelper Output { get; }

    protected UITestsBase(FixtureServiceProvider serviceProvider, ITestOutputHelper output)
    {
        _serviceProvider = serviceProvider;

        Output = output;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.TestOutput(output)
            .CreateLogger();
    }

    public async Task InitializeAsync()
    {
        _browser = await _serviceProvider.GetRequiredServiceAsync<IBrowser>();
        _url = await _serviceProvider.GetRequiredServiceAsync<UrlService>();
        _credentials = (await _serviceProvider.GetRequiredServiceAsync<IOptions<E2ECredentials>>()).Value;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected async Task ExecuteAsync(
        string testName,
        Func<IPage, Task> testCodeCallback,
        string[]? assertedSnapshots = null)
    {
        var stateFileExists = File.Exists("state.json");
        var recordVideoDir = Path.Combine("..", "..");
        var browserContextOptions = new BrowserNewContextOptions
        {
            RecordVideoDir = recordVideoDir,
            StorageStatePath = stateFileExists ? "state.json" : null,
            ViewportSize = new ViewportSize() { Width = 1280, Height = 768 },
            RecordVideoSize = new RecordVideoSize() { Width = 1280, Height = 768 }
        };

        using var assertionScope = new AssertionScope(testName);
        var browserContext = await _browser.NewContextAsync(browserContextOptions);
        var page = await browserContext.NewPageAsync();

        page.ConfigureErrorLogging(Output);

        var removeVideo = false; // TODO put back true;
        try
        {
            try
            {
                await testCodeCallback(page);
            }
            finally
            {
                assertionScope.Dispose();
            }
        }
        catch
        {
            removeVideo = false;
            throw;
        }
        finally
        {
            await page.CloseAsync();
            await browserContext.CloseAsync();

            if (page.Video != null)
            {
                var videoPath = await page.Video.PathAsync();
                if (removeVideo)
                {
                    File.Delete(videoPath);
                }
                else
                {
                    var videoExtension = Path.GetExtension(videoPath);
                    var videoFileName = Path.Combine(browserContextOptions.RecordVideoDir, $"{testName}{videoExtension}");
                    if (File.Exists(videoFileName))
                        File.Delete(videoFileName);
                    File.Move(videoPath, videoFileName);
                }
            }

            if (assertedSnapshots != null)
            {
                assertedSnapshots
                    .Select(snapshot => assertionScope.Get<string>(snapshot))
                    .Where(path => !string.IsNullOrEmpty(path))
                    .ForEach(path =>
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    });
            }
        }
    }

    protected static string GetTestName(
     [CallerMemberName] string memberName = "",
     [CallerFilePath] string sourceFilePath = "",
     [CallerLineNumber] int sourceLineNumber = 0)
     => $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{memberName}";
}
