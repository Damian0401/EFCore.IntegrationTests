using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Api;
using Testcontainers.MsSql;
using Tests.Extensions;
using Microsoft.Data.SqlClient;
using Respawn;

namespace Tests.IntegrationTests.Utils;

public class IntegrationTestWebAppFactory<TProgram, TDbContext> 
    : WebApplicationFactory<TProgram>, IAsyncLifetime 
    where TProgram : class
    where TDbContext : DbContext
{
    private readonly MsSqlContainer _dbContainer;
    private SqlConnection _connection = default!;
    private Respawner _respawner = default!;

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

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _connection = new SqlConnection(_dbContainer.GetConnectionString());

        // Need to perform migrations before creating the respawner
        _ = CreateClient();
        
        await InitializeRespawnerAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
        await _connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer
        });
    }

    public new async Task DisposeAsync() => await _dbContainer.DisposeAsync();

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_connection);
}
