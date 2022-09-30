using AutoMapper;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.CQRS.Note.Queries;
using Notes.Domain.Entities;

namespace Notes.Application.Profiles;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, GetNoteDto>().ReverseMap();
        CreateMap<CreateNoteCommand, Note>()
            .ForMember(prop => prop.CreationDate,  expression => expression.MapFrom(s => DateTime.UtcNow))
            .ForMember(prop => prop.LastTimeModified,  expression => expression.MapFrom(s => DateTime.UtcNow));
        CreateMap<UpdateNoteCommand, Note>()
            .ForMember(prop => prop.LastTimeModified,  expression => expression.MapFrom(s => DateTime.UtcNow));
        CreateMap<UpdateNoteForUserCommand, Note>()
            .ForMember(prop => prop.LastTimeModified,  expression => expression.MapFrom(s => DateTime.UtcNow));
    }
}