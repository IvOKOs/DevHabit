using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using Testcontainers.MsSql;
using WireMock.Server;


namespace DevHabit.IntegrationTests.Infrastructure;

// api will run in memory using WebApplicationFactory to connect to our sql instance running in the container
public class DevHabitWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // db that we will connect to from out api project
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Sql12345!")
        .Build();
    private WireMockServer _wireMockServer;

    public WireMockServer GetWireMockServer() => _wireMockServer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Database", _msSqlContainer.GetConnectionString());
        builder.UseSetting("GitHub:BaseUrl", _wireMockServer.Urls[0]);
        builder.UseSetting("Encryption:Key", Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));
        Quartz.Logging.LogContext.SetCurrentLogProvider(NullLoggerFactory.Instance);
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        _wireMockServer = WireMockServer.Start();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
        _wireMockServer.Stop();
    }
}
