using System.ComponentModel.DataAnnotations.Schema;

namespace Notes.Domain.Entities;

public class Note
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; init; }
    public DateTime LastTimeModified { get; set; }
}