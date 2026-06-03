using HRApplicantSystem.Data.Models;

namespace HRApplicantSystem.Data.Repositories;

public interface IInterviewScheduleRepository
{
    Task<IEnumerable<InterviewSchedule>> GetAllAsync();
    Task<InterviewSchedule?> GetByIdAsync(string id);
    Task<InterviewSchedule?> GetByApplicationIdAsync(string applicationId);
    Task<bool> CreateAsync(InterviewSchedule schedule);
    Task<bool> UpdateAsync(InterviewSchedule schedule);
    Task<bool> DeleteAsync(string id);
}
