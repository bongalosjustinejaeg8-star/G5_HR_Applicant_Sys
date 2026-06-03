using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IInterviewEvaluationRepository
{
    Task<InterviewEvaluation?> GetByScheduleIdAsync(string scheduleId);
    Task<bool> CreateAsync(InterviewEvaluation evaluation);
    Task<bool> UpdateAsync(InterviewEvaluation evaluation);
}
