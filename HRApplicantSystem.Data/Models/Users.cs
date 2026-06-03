namespace HRApplicantSystem.Data.Models;

public class User
{
    public string UserID { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

}