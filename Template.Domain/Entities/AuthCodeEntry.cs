namespace Template.Domain.Entities;

public record AuthCodeEntry(string Code, DateTime ExpiresAt);