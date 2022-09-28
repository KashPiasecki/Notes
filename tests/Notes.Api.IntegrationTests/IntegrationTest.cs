using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Notes.Api.IntegrationTests.Utility;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Identity;
using Notes.Infrastructure.Persistence;

namespace Notes.Api.IntegrationTests;

public class IntegrationTest : IDisposable
{
    protected readonly HttpClient TestClient;
    private readonly IServiceProvider _serviceProvider;
    private const string TestUsername = "TestUser";
    private const string TestEmail = "test@example.com";
    private const string TestPassword = "Password123!@#";

    protected IntegrationTest()
    {
        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<DataContext>));
                    services.AddDbContext<DataContext>(options => { options.UseInMemoryDatabase("TestDb"); });
                });
            });
        _serviceProvider = appFactory.Services;
        TestClient = appFactory.CreateDefaultClient();
    }

    protected async Task AuthenticateAsync()
    {
        TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
    }

    private async Task<string> GetJwtAsync()
    {
        var response = 
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(TestUsername, TestEmail, TestPassword));
        var registrationResponse = await response.Content.ReadFromJsonAsync<AuthenticationSuccessResult>();
        return registrationResponse!.Token;
    }

    public void Dispose()
    {
        using var serviceScope = _serviceProvider.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.EnsureDeleted();
    }
}