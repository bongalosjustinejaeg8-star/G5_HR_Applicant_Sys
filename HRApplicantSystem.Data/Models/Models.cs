using HRApplicantSystem.Shared.Enums;




namespace HRApplicantSystem.Data.Models;

public class Applicant
{

    public string ApplicantId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactNo { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string WorkExperience { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
}

public class ApplicantAccount
{
    public string AccountId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

}

public class ApplicantDocument
{
    public string DocumentId { get; set; } = string.Empty;
    public string ApplicantId { get; set; } = string.Empty;
    public string RequirementTypeId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; } = DocumentStatus.Missing;
    public string? Remarks { get; set; }
    public DateTime SubmittedAt { get; set; }

    // ✨ UI ONLY (not stored in DB)
    public string FileName =>
        string.IsNullOrWhiteSpace(FilePath)
            ? ""
            : Path.GetFileName(FilePath);

    public string DocumentTypeName =>
        RequirementTypeId switch
        {
            "79263f6b-68c0-11f1-baa4-f894c21dce1e" => "Resume",
            "79264256-68c0-11f1-baa4-f894c21dce1e" => "ID",
            "7926434c-68c0-11f1-baa4-f894c21dce1e" => "Transcript",
            "79264402-68c0-11f1-baa4-f894c21dce1e" => "Certificate",
            "7926447c-68c0-11f1-baa4-f894c21dce1e" => "Other",
            _ => "Unknown"
        };
}
public class RequirementTypeOption
{
    public string Id { get; set; } = "";   // KEEP STRING, NOT GUID
    public string Name { get; set; } = "";
}
public class Application
{
    public string ApplicationId { get; set; } = string.Empty;
    public string ApplicantId { get; set; } = string.Empty;
    public string VacancyId { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
    public DateTime SubmittedAt { get; set; }
    public bool IsLocked { get; set; } = false;

}

public class ApplicationStatusHistory
{
    public string HistoryId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string? ChangedBy { get; set; }
    public ApplicationStatus? OldStatus { get; set; }
    public ApplicationStatus NewStatus { get; set; } = ApplicationStatus.Draft;
    public DateTime ChangedAt { get; set; }
    public string? Remarks { get; set; }

}

public class AuditTrail
{
    public string AuditId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TableAffected { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public DateTime PerformedAt { get; set; }

}

public class Department
{
    public string DepartmentId { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

}

public class HiringDecision
{
    public string DecisionId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string DecidedBy { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public DateTime DecidedAt { get; set; }
}

public class InterviewEvaluation
{
    public string EvalId { get; set; } = string.Empty;
    public string ScheduleId { get; set; } = string.Empty;
    public string EvaluatedBy { get; set; } = string.Empty;
    public int? Score { get; set; }

    public string Remarks { get; set; } = string.Empty;
    public string? Recommendation { get; set; }

    public string PassFail { get; set; } = string.Empty;
}

public class InterviewSchedule
{
    public string ScheduleId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string InterviewerId { get; set; } = string.Empty;
    public DateTime InterviewDate { get; set; }
    public InterviewMode Mode { get; set; } = InterviewMode.Onsite;
    public string Location { get; set; } = string.Empty;
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;

}

public class JobVacancy
{
    public string VacancyId { get; set; } = string.Empty;
    public string DepartmentId { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public VacancyStatus Status { get; set; } = VacancyStatus.Open;
    public DateTime CreatedAt { get; set; }
}

public class Position
{
    public string PositionId { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

public class RequirementType
{
    public string RequirementTypeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

}
public class Role
{
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string? Permissions { get; set; }


}
public class ScreeningResult
{
    public string ScreeningId { get; set; } = string.Empty;
    public string ApplicationId { get; set; } = string.Empty;
    public string ScreenedBy { get; set; } = string.Empty;
    public ScreeningResults Result { get; set; } = ScreeningResults.NotQualified;
    public string Remarks { get; set; } = string.Empty;
    public DateTime ScreenedAt { get; set; }

}

public class User
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

}
