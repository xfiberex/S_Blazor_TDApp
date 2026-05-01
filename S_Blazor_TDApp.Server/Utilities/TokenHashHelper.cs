using System.Security.Cryptography;
using System.Text;

namespace S_Blazor_TDApp.Server.Utilities;

public static class TokenHashHelper
{
    public static string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);
        return Convert.ToHexString(hashBytes);
    }

    public static bool Matches(string token, string? storedValue)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(storedValue))
        {
            return false;
        }

        return FixedTimeEquals(token, storedValue) || FixedTimeEquals(HashToken(token), storedValue);
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(left),
            Encoding.UTF8.GetBytes(right));
    }
}