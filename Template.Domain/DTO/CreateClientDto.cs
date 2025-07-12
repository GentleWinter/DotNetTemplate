using Template.Domain.Entities;

namespace Template.Domain.DTO;

public class CreateClientDto
{
    public required string Name { get; set; }
    public required AuthInfo AuthInfo { get; set; }
    public Roles Role { get; set; }
    public bool IsActive { get; set; }
}