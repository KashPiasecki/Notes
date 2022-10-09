using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using AutoFixture;
using FluentAssertions;
using Notes.Api.IntegrationTests.Utility;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts;
using Notes.Domain.Contracts.Identity;
using NUnit.Framework;
using ApiRoutes = Notes.Api.IntegrationTests.Utility.ApiRoutes;

namespace Notes.Api.IntegrationTests.Controllers;

public class IdentityControllerTests : IntegrationTest
{
    [Test]
    public async Task Register_WithProperValues_CreatesUser()
    {
        // Arrange - Act
        var response = await AuthenticateAsync();

        // Assert
        response.Token.Should().NotBeEmpty();
        response.RefreshToken.Should().NotBeEmpty();
    }

    [TestCase("short")]
    [TestCase("tooLongUserName305798325702357")]
    public async Task Register_WithImproperUserName_ReturnsUnprocessableEntity(string userName)
    {
        // Arrange
        var testEmail = Fixture.Create<string>();
        var testPassword = Fixture.CreateValidPassword();

        // Act
        var response = 
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(userName, testEmail, testPassword));
        var registrationResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        registrationResponse?.Errors.Should().NotBeEmpty();
    }
    
    [Test]
    public async Task Register_WithImproperEmail_ReturnsUnprocessableEntity()
    {
        // Arrange
        var testUsername = Fixture.Create<string>();
        var testEmail = Fixture.Create<string>();
        var testPassword = Fixture.CreateValidPassword();

        // Act
        var response = 
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));
        var registrationResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        registrationResponse?.Errors.Should().NotBeEmpty();
    }

    [TestCase("short")]
    [TestCase("TooLongPassword532932598723507320957239572395")]
    [TestCase("nouppercasepassword01")]
    [TestCase("NOLOWERCASEPASSWORD02")]
    [TestCase("NoNumbersPassword")]
    public async Task Register_WithImproperPassword_ReturnsUnprocessableEntity(string password)
    {
        // Arrange
        var testUsername = Fixture.Create<string>();
        var testEmail = Fixture.Create<MailAddress>().ToString();

        // Act
        var response = 
            await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, password));
        var registrationResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        registrationResponse?.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task Login_WithProperCredentials_ReturnsToken()
    {
        // Arrange
        var testUsername = Fixture.Create<string>();
        var testEmail = Fixture.Create<MailAddress>().ToString();
        var testPassword = Fixture.CreateValidPassword();
        await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new RegisterUserCommand(testUsername, testEmail, testPassword));

        // Act
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
        var testEmail = Fixture.Create<MailAddress>().ToString();
        var testPassword = Fixture.CreateValidPassword();

        // Act
        var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new LoginUserCommand(testEmail, testPassword));
        var loginResponse = await response.Content.ReadFromJsonAsync<AuthenticationFailedResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        loginResponse?.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task RefreshToken_WithImproperToken_ReturnsBadRequest()
    {
        // Arrange - Act
        var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.RefreshToken, new RefreshTokenCommand(Fixture.Create<string>(), Fixture.Create<string>()));
        var refreshTokenResult = await response.Content.ReadFromJsonAsync<AuthenticationFailedResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        refreshTokenResult?.Errors.Should().NotBeEmpty();
    }
}