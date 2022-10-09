namespace Notes.Application.Filtering;

public class NoteFilter
{
    public string Title { get; init; }
    public string Content { get; init; }

    public NoteFilter(NoteFilterQuery noteFilterQuery)
    {
        Title = noteFilterQuery.Title;
        Content = noteFilterQuery.Content;
    }
}