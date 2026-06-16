namespace HRApplicantSystem.Shared.Helpers;

public class PasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}

class Program
{
    public static void Main()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("hr123");
        Console.WriteLine(hash);
    }
}

