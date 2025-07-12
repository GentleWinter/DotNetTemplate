namespace Template.Domain.DTO;

public class UpdatePasswordDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Code { get; set; }
}