namespace Template.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required AuthInfo AuthInfo { get; set; }
    public required Plan Plan { get; set; }
    public Roles Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}