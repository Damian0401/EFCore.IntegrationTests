using Api;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests.Utils;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory<Program, DataContext>>, IDisposable
{
    protected readonly IServiceScope Scope;
    protected readonly DataContext Context;

    public BaseIntegrationTest(IntegrationTestWebAppFactory<Program, DataContext> factory)
    {
        Scope = factory.Services.CreateScope();

        Context = Scope.ServiceProvider
            .GetRequiredService<DataContext>();
    }

    public virtual void Dispose()
    {
        Scope.Dispose();
    }
}
