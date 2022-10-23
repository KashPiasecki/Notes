using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.AspNetCore.Identity;
using Notes.Application.CQRS.Filtering;
using Notes.Domain.Contracts.Filters;
using Notes.Domain.Entities;
using Notes.Infrastructure.Persistence;
using Notes.Infrastructure.Repositories;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Collections;
using TddXt.AnyRoot.Invokable;
using TddXt.AnyRoot.Strings;

namespace Notes.Infrastructure.UnitTests.Repositories;

public class NoteRepositoryTests
{
    [Test]
    public async Task GetByIdAsync_ExistingNote_ReturnsIt()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        var note = noteList.First();
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);

        // Act
        var result = await noteRepository.GetByIdAsync(note.Id, Any.CancellationToken());

        // Assert
        result.Should().Be(note);
    }

    [Test]
    public async Task GetByIdAsync_NonExistingNote_ReturnsNull()
    {
        // Arrange
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();

        var noteRepository = new NoteRepository(mockedDbContext);

        // Act
        var result = await noteRepository.GetByIdAsync(Any.Guid(), Any.CancellationToken());

        // Assert
        result!.Should().BeNull();
    }
    
    [Test]
    public async Task GetByIdForUserAsync_ExistingNote_ReturnsIt()
    {
        // Arrange
        var user = Any.Instance<IdentityUser>();
        var noteList = Any.List<Note>(3);
        var note = Any.Instance<Note>().WithProperty(x => x.User, user);
        noteList.Add(note);
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);

        // Act
        var result = await noteRepository.GetByIdForUserAsync(user.Id, note.Id, Any.CancellationToken());

        // Assert
        result.Should().Be(note);
    }

    [Test]
    public async Task GetByIdForUserAsync_NonExistingNote_ReturnsNull()
    {
        // Arrange
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();

        var noteRepository = new NoteRepository(mockedDbContext);

        // Act
        var result = await noteRepository.GetByIdForUserAsync(Any.String(), Any.Guid(), Any.CancellationToken());

        // Assert
        result!.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_Called_ReturnsNotes()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, string.Empty);

        // Act
        var result = (await noteRepository.GetAllAsync(defaultPaginationFilter, noteFilterQuery, Any.CancellationToken())).ToList();

        // Assert
        result.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, string.Empty);

        // Act
        var result = (await noteRepository.GetAllAsync(defaultPaginationFilter, noteFilterQuery, Any.CancellationToken())).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetAllAsync_FilterByTitle_ReturnsListWithMatchingEntitiesIrrelevantCase()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>().WithProperty(x => x.Title, "test123"),
                Any.Instance<Note>().WithProperty(x => x.Title, "Test12"),
                Any.Instance<Note>().WithProperty(x => x.Title, "teST1")
            });
        {
        }
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, "test");

        // Act
        var result = (await noteRepository.GetAllAsync(defaultPaginationFilter, noteFilterQuery, Any.CancellationToken())).ToList();

        // Assert
        result.Count.Should().Be(3);
    }

    [Test]
    public async Task GetAllAsync_FilterByContent_ReturnsListWithMatchingEntitiesIrrelevantCase()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>().WithProperty(x => x.Content, "test123"),
                Any.Instance<Note>().WithProperty(x => x.Content, "Test12"),
                Any.Instance<Note>().WithProperty(x => x.Content, "teST1")
            });
        {
        }
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Title, string.Empty)
            .WithProperty(x => x.Content, "test");

        // Act
        var result = (await noteRepository.GetAllAsync(defaultPaginationFilter, noteFilterQuery, Any.CancellationToken())).ToList();

        // Assert
        result.Count.Should().Be(3);
    }

    [Test]
    public async Task CountAsync_Called_Counts()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, string.Empty);

        // Act
        var result = await noteRepository.CountAsync(noteFilterQuery, Any.CancellationToken());

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task CountAsync_FilterByTitle_CountsWithFilterIrrelevantCase()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>().WithProperty(x => x.Title, "test123"),
                Any.Instance<Note>().WithProperty(x => x.Title, "Test12"),
                Any.Instance<Note>().WithProperty(x => x.Title, "teST1")
            });
        {
        }
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, "test");

        // Act
        var result = await noteRepository.CountAsync(noteFilterQuery, Any.CancellationToken());

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task CountAsync_FilterByContent_CountsWithFilterIrrelevantCase()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>().WithProperty(x => x.Content, "test123"),
                Any.Instance<Note>().WithProperty(x => x.Content, "Test12"),
                Any.Instance<Note>().WithProperty(x => x.Content, "teST1")
            });
        {
        }
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Title, string.Empty)
            .WithProperty(x => x.Content, "test");

        // Act
        var result = await noteRepository.CountAsync(noteFilterQuery, Any.CancellationToken());

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task Remove_Called_Removes()
    {
        // Arrange
        var noteList = Any.List<Note>(3);
        var note = noteList.First();
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);

        // Act
        noteRepository.Remove(note);
        await mockedDbContext.SaveChangesAsync();
        var result = await noteRepository.GetByIdAsync(note.Id, Any.CancellationToken());

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllForUserAsync_Called_ReturnsNotesForUser()
    {
        // Arrange
        var userId = Any.String();
        var noteList = Any.List<Note>();
        var user = Any.Instance<IdentityUser>()
            .WithProperty(x => x.Id, userId);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>().WithProperty(x => x.User, user),
                Any.Instance<Note>().WithProperty(x => x.User, user),
                Any.Instance<Note>().WithProperty(x => x.User, user)
            });
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, string.Empty);

        // Act
        var result = (await noteRepository.GetAllForUserAsync(userId, defaultPaginationFilter, noteFilterQuery, Any.CancellationToken()))
            .ToList();

        // Assert
        result.Count.Should().Be(3);
    }


    [Test]
    public async Task GetAllForUserAsync_EmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, string.Empty);

        // Act
        var result =
            (await noteRepository.GetAllForUserAsync(Any.String(), defaultPaginationFilter, noteFilterQuery, Any.CancellationToken())).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetAllForUserAsync_FilterByTitle_ReturnsListForUserWithMatchingEntitiesIrrelevantCase()
    {
        // Arrange
        var userId = Any.String();
        var noteList = Any.List<Note>();
        var user = Any.Instance<IdentityUser>()
            .WithProperty(x => x.Id, userId);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>()
                    .WithProperty(x => x.Title, "test123")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Title, "Test12")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Title, "teST1")
                    .WithProperty(x => x.User, user)
            });
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, "test");

        // Act
        var result = (await noteRepository.GetAllForUserAsync(userId, defaultPaginationFilter, noteFilterQuery, Any.CancellationToken())).ToList();

        // Assert
        result.Count.Should().Be(3);
    }

    [Test]
    public async Task GetAllForUserAsync_FilterByContent_ReturnsListForUserWithMatchingEntitiesIrrelevantCase()
    {
        // Arrange
        var userId = Any.String();
        var noteList = Any.List<Note>();
        var user = Any.Instance<IdentityUser>()
            .WithProperty(x => x.Id, userId);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>()
                    .WithProperty(x => x.Content, "test123")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Content, "Test12")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Content, "teST1")
                    .WithProperty(x => x.User, user)
            });
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var defaultPaginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Title, string.Empty)
            .WithProperty(x => x.Content, "test");

        // Act
        var result = (await noteRepository.GetAllForUserAsync(userId, defaultPaginationFilter, noteFilterQuery, Any.CancellationToken())).ToList();

        // Assert
        result.Count.Should().Be(3);
    }

    [Test]
    public async Task CountForUserAsync_Called_Counts()
    {
        // Arrange
        var userId = Any.String();
        var noteList = Any.List<Note>();
        var user = Any.Instance<IdentityUser>()
            .WithProperty(x => x.Id, userId);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>().WithProperty(x => x.User, user),
                Any.Instance<Note>().WithProperty(x => x.User, user),
                Any.Instance<Note>().WithProperty(x => x.User, user)
            });
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, string.Empty);

        // Act
        var result = await noteRepository.CountForUserAsync(userId, noteFilterQuery, Any.CancellationToken());

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task CountForUserAsync_FilterByTitle_CountsForUserWithFilterIrrelevantCase()
    {
        // Arrange
        var userId = Any.String();
        var noteList = Any.List<Note>();
        var user = Any.Instance<IdentityUser>()
            .WithProperty(x => x.Id, userId);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>()
                    .WithProperty(x => x.Title, "test123")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Title, "Test12")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Title, "teST1")
                    .WithProperty(x => x.User, user)
            });
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Content, string.Empty)
            .WithProperty(x => x.Title, "test");

        // Act
        var result = await noteRepository.CountForUserAsync(userId, noteFilterQuery, Any.CancellationToken());

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task CountForUserAsync_FilterByContent_CountsForUserWithFilterIrrelevantCase()
    {
        // Arrange
        var userId = Any.String();
        var noteList = Any.List<Note>();
        var user = Any.Instance<IdentityUser>()
            .WithProperty(x => x.Id, userId);
        noteList.AddRange(
            new List<Note>
            {
                Any.Instance<Note>()
                    .WithProperty(x => x.Content, "test123")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Content, "Test12")
                    .WithProperty(x => x.User, user),
                Any.Instance<Note>()
                    .WithProperty(x => x.Content, "teST1")
                    .WithProperty(x => x.User, user)
            });
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.Notes.AddRange(noteList);
        await mockedDbContext.SaveChangesAsync();

        var noteRepository = new NoteRepository(mockedDbContext);
        var noteFilterQuery = Any.Instance<NoteFilterQuery>()
            .WithProperty(x => x.Title, string.Empty)
            .WithProperty(x => x.Content, "test");

        // Act
        var result = await noteRepository.CountForUserAsync(userId, noteFilterQuery, Any.CancellationToken());

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public async Task AddAsync_Called_ReturnsCreatedNote()
    {
        // Arrange
        var note = Any.Instance<Note>();
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();

        var noteRepository = new NoteRepository(mockedDbContext);

        // Act
        var result = await noteRepository.AddAsync(note, Any.CancellationToken());

        // Assert
        result.Should().NotBeNull();
    }
    
    [Test]
    public async Task AddAsync_Called_AddsCreatedNote()
    {
        // Arrange
        var note = Any.Instance<Note>();
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();

        var noteRepository = new NoteRepository(mockedDbContext);

        // Act
        await noteRepository.AddAsync(note, Any.CancellationToken());
        await mockedDbContext.SaveChangesAsync();
        var result = await noteRepository.GetByIdAsync(note.Id, Any.CancellationToken());

        // Assert
        result.Should().Be(note);
    }
}