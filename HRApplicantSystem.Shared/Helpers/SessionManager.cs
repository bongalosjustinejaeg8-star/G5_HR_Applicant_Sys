using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Shared.Helpers;


public static class SessionManager
{
    public static string? CurrentUserID { get; private set; }
    public static string? CurrentUserName { get; private set; }

    public static UserRole? CurrentUserRole { get; private set; }
    public static bool IsLoggedIn { get; private set; } = false;


    public static void Login(string name, string id, UserRole role)
    {
        CurrentUserID = id;
        CurrentUserName = name;
        CurrentUserRole = role;
        IsLoggedIn = true;


    }
    public static void Logout()
    {
        CurrentUserID = null;
        CurrentUserName = null;
        CurrentUserRole = UserRole.None;
        IsLoggedIn = false;
    }




}