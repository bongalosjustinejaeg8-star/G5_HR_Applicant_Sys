namespace HRApplicantSystem.Shared.Helpers;

using System.Text.RegularExpressions;
public class Validation
{

    public bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public bool IsNotEmpty(string word)
    {
        return string.IsNullOrWhiteSpace(word);
    }

    public bool IsValidDate(DateTime date)
    {
        if (DateTime.Today < date)
        {
            return true;
        }
        return false;
    }

    public bool Is18Above(DateTime date)
    {
        if((DateTime.Today - date)>= 18)
    }


}