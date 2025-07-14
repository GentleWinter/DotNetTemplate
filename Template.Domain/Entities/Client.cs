using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required Plan Plan { get; set; }
    public Roles Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [NotMapped]
    public AuthInfo AuthInfo
    {
        get => new AuthInfo { Email = Email, Password = Password };
        set
        {
            Email = value?.Email ?? "";
            Password = value?.Password ?? "";
        }
    }
}