using Api;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests.Utils;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory<Program, DataContext>>, IAsyncLifetime
{
    protected readonly IServiceScope Scope;
    protected readonly DataContext Context;
    private readonly Func<Task> _resetDatabase;

    public BaseIntegrationTest(IntegrationTestWebAppFactory<Program, DataContext> factory)
    {
        Scope = factory.Services.CreateScope();

        Context = Scope.ServiceProvider
            .GetRequiredService<DataContext>();

        _resetDatabase = factory.ResetDatabaseAsync;
    }

    public async Task DisposeAsync()
    {
        await _resetDatabase();
        Scope.Dispose();
    }

    public Task InitializeAsync() => Task.CompletedTask;
}
