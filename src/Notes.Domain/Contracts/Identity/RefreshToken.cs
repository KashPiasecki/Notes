using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Notes.Domain.Contracts.Identity;

public class RefreshToken
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Token { get; init; }
    public string JwtId { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime ExpireDate { get; init; }
    public bool Used { get; set; }
    public bool Invalidated { get; set; }
    public string UserId { get; init; }
    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; init; }
    
}