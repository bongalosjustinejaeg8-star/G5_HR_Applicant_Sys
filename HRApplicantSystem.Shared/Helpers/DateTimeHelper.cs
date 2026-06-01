namespace HRApplicantSystem.Shared.Helpers;

public class DateTimeHelper
{
    public string FormatDate(DateTime date)
    {
        return date.ToString("dddd, dd MMMM yyyy");

    }

    public string FromatDateTime(DateTime date)
    {
        return date.ToString("MMMM d, yyyy h:mm tt");

    }

    public DateTime GetCurrentTimestamp()
    {
        return DateTime.Now;
    }

}