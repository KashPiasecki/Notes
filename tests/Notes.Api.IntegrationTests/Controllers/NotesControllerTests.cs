using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Notes.Api.IntegrationTests.Utility;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.CQRS.Note.Commands.Delete;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.CQRS.Note.Queries;
using NUnit.Framework;

namespace Notes.Api.IntegrationTests.Controllers;

public class NotesControllerTests : IntegrationTest
{
    [TestCase(ApiRoutes.Notes.Get)]
    [TestCase(ApiRoutes.Notes.User.Get)]
    public async Task AnyEndpoint_WithoutAuthentication_ReturnsUnauthorized(string uri)
    {
        // Arrange
        // Act
        var response = await TestClient.GetAsync(uri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task GetAll_EmptyDb_ReturnsEmptyList()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await TestClient.GetAsync(ApiRoutes.Notes.User.Get);
        var notes = await response.Content.ReadFromJsonAsync<List<GetNoteDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        notes.Should().BeEmpty();
    }


    [Test]
    public async Task GetAll_WithSomeData_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();
        var createNoteCommand = Fixture.Create<CreateNoteCommand>();

        // Act
        var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Notes.Post, createNoteCommand);
        var postResult = await postResponse.Content.ReadFromJsonAsync<GetNoteDto>();
        var response = await TestClient.GetAsync(ApiRoutes.Notes.User.Get);
        var notes = await response.Content.ReadFromJsonAsync<List<GetNoteDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var note = notes?.SingleOrDefault(x => x.Id == postResult?.Id);
        note?.Title.Should().Be(createNoteCommand.Title);
        note?.Content.Should().Be(createNoteCommand.Content);
        notes.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetById_WithValidId_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();
        var createNoteCommand = Fixture.Create<CreateNoteCommand>();

        // Act
        var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Notes.Post, createNoteCommand);
        var postResult = await postResponse.Content.ReadFromJsonAsync<GetNoteDto>();
        var response = await TestClient.GetAsync(ApiRoutes.Notes.GetById.Replace("<id>", postResult?.Id.ToString()));
        var note = await response.Content.ReadFromJsonAsync<GetNoteDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        note?.Title.Should().Be(createNoteCommand.Title);
        note?.Content.Should().Be(createNoteCommand.Content);
        note?.Id.Should().Be(postResult.Id);
    }

    [Test]
    public async Task GetById_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        var fixture = new Fixture();

        // Act
        var response = await TestClient.GetAsync(ApiRoutes.Notes.GetById.Replace("<id>", fixture.Create<string>()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Post_WithValidValues_ReturnsCreated()
    {
        // Arrange
        await AuthenticateAsync();
        var createNoteCommand = Fixture.Create<CreateNoteCommand>();

        // Act
        var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Notes.Post, createNoteCommand);
        var postResult = await postResponse.Content.ReadFromJsonAsync<GetNoteDto>();

        // Assert
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        postResult?.Title.Should().Be(createNoteCommand.Title);
        postResult?.Content.Should().Be(createNoteCommand.Content);
    }

    [Test]
    public async Task Post_WithInvalidValues_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        var createNoteCommand = Fixture.Build<CreateNoteCommand>()
            .With(x => x.Title, string.Empty)
            .With(x => x.Content, string.Empty)
            .Create();

        // Act
        var response = await TestClient.PostAsJsonAsync(ApiRoutes.Notes.Post, createNoteCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Put_WithValidValues_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();
        var createNoteCommand = Fixture.Create<CreateNoteCommand>();

        // Act
        var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Notes.Post, createNoteCommand);
        var postResult = await postResponse.Content.ReadFromJsonAsync<GetNoteDto>();
        var updateNoteCommand = Fixture.Build<UpdateNoteCommand>().With(x => x.Id, postResult?.Id).Create();
        var response = await TestClient.PutAsJsonAsync(ApiRoutes.Notes.User.Update, updateNoteCommand);
        var note = await response.Content.ReadFromJsonAsync<GetNoteDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        note?.Title.Should().Be(updateNoteCommand.Title);
        note?.Content.Should().Be(updateNoteCommand.Content);
        note?.Id.Should().Be(updateNoteCommand.Id);
    }

    [Test]
    public async Task Put_WithInvalidValues_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();
        var updateNoteCommand = Fixture.Create<UpdateNoteCommand>();

        // Act
        var response = await TestClient.PutAsJsonAsync(ApiRoutes.Notes.User.Update, updateNoteCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Delete_WithValidValues_ReturnsNoContent()
    {
        // Arrange
        await AuthenticateAsync();
        var createNoteCommand = Fixture.Create<CreateNoteCommand>();

        // Act
        var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.Notes.Post, createNoteCommand);
        var postResult = await postResponse.Content.ReadFromJsonAsync<GetNoteDto>();
        var deleteNoteCommand = Fixture.Build<DeleteNoteCommand>()
            .With(x => x.Id, postResult?.Id)
            .Create();
        var response = await TestClient.DeleteWithJsonAsync(ApiRoutes.Notes.User.Delete, deleteNoteCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WithInvalidValues_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await TestClient.DeleteAsync(ApiRoutes.Notes.Delete.Replace("<id>", Fixture.Create<string>()));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}