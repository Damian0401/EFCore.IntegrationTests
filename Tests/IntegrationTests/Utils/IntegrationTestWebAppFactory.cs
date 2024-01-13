using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api;
using Testcontainers.MsSql;
using Tests.Extensions;

namespace Tests.IntegrationTests.Utils;

public class IntegrationTestWebAppFactory<TProgram, TDbContext> 
    : WebApplicationFactory<TProgram>, IAsyncLifetime 
    where TProgram : class
    where TDbContext : DbContext
{
    private readonly MsSqlContainer _dbContainer;

    public IntegrationTestWebAppFactory()
    {
        _dbContainer = new MsSqlBuilder()
            .WithImage(Constants.SqlServerImage)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<DataContext>();

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(_dbContainer.GetConnectionString());
            });

            services.EnsureDbCreated<DataContext>();
        });
    }

    public async Task InitializeAsync() => await _dbContainer.StartAsync();

    public new async Task DisposeAsync() => await _dbContainer.DisposeAsync();
}
