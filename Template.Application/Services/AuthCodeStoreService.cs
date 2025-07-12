using System.Collections.Concurrent;
using System.Security.Cryptography;
using Template.Domain.Entities;

namespace Template.Application.Services;

public class AuthCodeStoreService
{
    private static readonly ConcurrentDictionary<string, AuthCodeEntry> _store = new();

    public static void Save(string email, string code, TimeSpan ttl)
    {
        _store[email] = new AuthCodeEntry(code, DateTime.UtcNow.Add(ttl));
    }

    public static bool Validate(string email, string code)
    {
        if (_store.TryGetValue(email, out var entry))
        {
            if (DateTime.UtcNow <= entry.ExpiresAt && entry.Code == code)
            {
                _store.TryRemove(email, out _);
                return true;
            }
        }

        return false;
    }
    
    public static string GenerateNumericAuthCode(int length = 6)
    {
        if (length <= 0 || length > 10)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be between 1 and 10.");

        int min = (int)Math.Pow(10, length - 1);
        int max = (int)Math.Pow(10, length) - 1;

        return RandomNumberGenerator.GetInt32(min, max + 1).ToString();
    }
}