using System.Security.Cryptography;
using System.Text;
public class PasswordService
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(passwordBytes);
        return Convert.ToBase64String(hashBytes);
    }
    public bool VerifyPassword(string password, string storedPasswordHash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == storedPasswordHash;
    }
}