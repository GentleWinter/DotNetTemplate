using Template.Domain.Entities;

namespace Template.Domain.DTO;

public class ClientDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required AuthInfo AuthInfo { get; set; }
    public Roles Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }    
}