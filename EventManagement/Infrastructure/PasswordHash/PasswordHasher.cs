using System;
using System.Security.Cryptography;
using System.Text;

namespace EventManagement.Infrastructure.PasswordHash;

public class PasswordHasher : IPasswordHasher
{
    const int Iterations = 1000;
    const int KeySize = 128 / 8; // 128 bits
    private static readonly byte[] Salt = {
        0xA, 0x20, 0xB6, 0x3,
        0x4, 0x15, 0x43, 0x3F,
        0x6F, 0xF, 0x38, 0x4D,
        0x81, 0x20, 0xC, 0x1D
    };

    public string HashPassword(string password)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            Salt,
            Iterations,
            HashAlgorithmName.SHA384,
            KeySize);
        return Convert.ToHexString(hash);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(providedPassword, Salt, Iterations, HashAlgorithmName.SHA384, KeySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hashedPassword));
    }
}