namespace HRApplicantSystem.Shared.Helpers;

using System.Text.RegularExpressions;
public class ValidationHelper
{

    public bool IsEmailValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    public bool IsNotEmpty(string word)
    {
        return !string.IsNullOrWhiteSpace(word);
    }

    public bool IsValidDate(DateTime date)
    {
        return (DateTime.Today < date);
    }


}

