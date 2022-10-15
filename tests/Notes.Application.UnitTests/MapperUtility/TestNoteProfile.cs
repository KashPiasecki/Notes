using AutoMapper;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.CQRS.Note.Queries;
using Notes.Domain.Entities;

namespace Notes.Application.UnitTests.MapperUtility;

public class TestNoteProfile : Profile
{
    public TestNoteProfile()
    {
        CreateMap<Note, GetNoteDto>();
        CreateMap<CreateNoteCommand, Note>();
        CreateMap<UpdateNoteCommand, Note>();
        CreateMap<UpdateNoteForUserCommand, Note>();
    }
}