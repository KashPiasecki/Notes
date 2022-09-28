using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using AutoFixture;
using FluentAssertions;
using Notes.Api.IntegrationTests.Utility;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Identity;
using NUnit.Framework;

namespace Notes.Api.IntegrationTests.Controllers;

public class IdentityControllerTests : IntegrationTest
{
    private IFixture? _fixture;

    [SetUp]
    public void SetUp() => _fixture = new Fixture();
    
    [TearDown]
    public void TearDown() => Dispose();
    
    [Test]
    public async Task Register_WithProperValues_CreatesUser()
    {
        // Arrange
        var testUsername = _fixture.Create<Guid>().ToString();
        var testEmail = _fixture.Create<MailAddress>().ToString();
        var testPassword = $"Password{testUsername}";

        // Act
        var response = 
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));
        var registrationResponse = await response.Content.ReadFromJsonAsync<AuthenticationSuccessResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        registrationResponse?.Token.Should().NotBeEmpty();
        registrationResponse?.RefreshToken.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task Register_WithImproperValues_ReturnsBadRequest()
    {
        // Arrange
        var testUsername = _fixture.Create<Guid>().ToString();
        var testEmail = _fixture.Create<MailAddress>().ToString();
        var testPassword = _fixture.Create<int>().ToString();

        // Act
        var response = 
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));
        var registrationResponse = await response.Content.ReadFromJsonAsync<AuthenticationFailedResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        registrationResponse?.Errors.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task Login_WithProperCredentials_ReturnsToken()
    {
        // Arrange
        var testUsername = _fixture.Create<Guid>().ToString();
        var testEmail = _fixture.Create<MailAddress>().ToString();
        var testPassword = $"Password{testUsername}";

        // Act
        await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));
        var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new LoginUserCommand(testEmail, testPassword));
        var loginResponse = await response.Content.ReadFromJsonAsync<AuthenticationSuccessResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        loginResponse?.Token.Should().NotBeEmpty();
        loginResponse?.RefreshToken.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task Login_WithImproperCredentials_ReturnsBadRequest()
    {
        // Arrange
        var testEmail = _fixture.Create<MailAddress>().ToString();
        var testPassword = _fixture.Create<string>();

        // Act
        var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new LoginUserCommand(testEmail, testPassword));
        var loginResponse = await response.Content.ReadFromJsonAsync<AuthenticationFailedResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        loginResponse?.Errors.Should().NotBeEmpty();
    }
    
    // Inefficient test which needs to wait for token refresh time to die
    // [Test]
    // public async Task RefreshToken_WithProperToken_ReturnsNewToken()
    // {
    //     // Arrange
    //     var testUsername = _fixture.Create<Guid>().ToString();
    //     var testEmail = _fixture.Create<MailAddress>().ToString();
    //     var testPassword = $"Password{testUsername}";
    //
    //     // Act
    //     var registerResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));
    //     var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthenticationSuccessResult>();
    //     await Task.Delay(TimeSpan.FromMinutes(6));
    //     var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.RefreshToken, new RefreshTokenCommand(registerResult!.Token, registerResult.RefreshToken));
    //     var refreshTokenResult = await response.Content.ReadFromJsonAsync<AuthenticationSuccessResult>();
    //
    //     // Assert
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);
    //     refreshTokenResult?.Token.Should().NotBeEmpty();
    //     refreshTokenResult?.RefreshToken.Should().NotBeEmpty();
    // }
    
    [Test]
    public async Task RefreshToken_WithImproperToken_ReturnsBadRequest()
    {
        // Arrange
        var testUsername = _fixture.Create<Guid>().ToString();
        var testEmail = _fixture.Create<MailAddress>().ToString();
        var testPassword = $"Password{testUsername}";

        // Act
        var registerResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthenticationSuccessResult>();
        var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.RefreshToken, new RefreshTokenCommand(registerResult!.Token, registerResult!.RefreshToken));
        var refreshTokenResult = await response.Content.ReadFromJsonAsync<AuthenticationFailedResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        refreshTokenResult?.Errors.Should().NotBeEmpty();
    }
}