using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Notes.Api.IntegrationTests.Utility;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Identity;
using Notes.Infrastructure.Persistence;
using NUnit.Framework;

namespace Notes.Api.IntegrationTests;

public class IntegrationTest
{
    protected HttpClient TestClient;
    protected Fixture Fixture;
    private IServiceProvider _serviceProvider;

    [SetUp]
    public void SetUp()
    {
        Fixture = new Fixture();
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

    [TearDown]
    public void TearDown(){
        using var serviceScope = _serviceProvider.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        context.Database.EnsureDeleted();
    }

    protected async Task<AuthenticationSuccessResult> AuthenticateAsync()
    {
        var registrationResponse = await GetJwtAsync();
        TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", registrationResponse.Token);
        return registrationResponse;
    }

    private async Task<AuthenticationSuccessResult> GetJwtAsync()
    {
        var testUsername = Fixture.Create<string>();
        var testEmail = Fixture.Create<MailAddress>().ToString();
        var testPassword = Fixture.CreateValidPassword();
        var response =
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));
        var registrationResponse = await response.Content.ReadFromJsonAsync<AuthenticationSuccessResult>();
        return registrationResponse!;
    }
}