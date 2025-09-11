using System.Security.Cryptography;
using System.Text;

namespace Trackster.Api.Core.Helpers;

public class PasswordHasher
{
    public static string HashPassword(string password, int iterations = 100_000, int keySize = 32)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltBytes = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ASPNETCORE_PASSWORD_SALT")!);

        using var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations, HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(keySize);

        return Convert.ToBase64String(hash);
    }
    
    public static bool VerifyPassword(string password, string storedHash, int iterations = 100_000, int keySize = 32)
    {
        string computedHash = HashPassword(password, iterations, keySize);
        
        return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(computedHash), Convert.FromBase64String(storedHash));
    }
}