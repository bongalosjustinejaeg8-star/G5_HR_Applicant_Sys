using HRApplicantSystem.Shared.Helpers;
namespace HRApplicantSystem.Shared;

public class Class1
{
    public void Main()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("admin123");
        Console.WriteLine(hash);
    }

}
