using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Notes.Domain.Entities;

public class Note
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    public string UserId { get; init; }
    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; init; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; init; }
    public DateTime LastTimeModified { get; set; }
}