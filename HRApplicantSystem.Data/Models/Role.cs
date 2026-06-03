namespace HRApplicantSystem.Data.Models;

public class Role
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Permissions { get; set; }


}