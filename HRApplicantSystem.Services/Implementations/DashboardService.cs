using System.Linq;
using System.Threading.Tasks;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IInterviewScheduleRepository _interviewRepository;

    public DashboardService(
        IApplicationRepository applicationRepository,
        IInterviewScheduleRepository interviewRepository)
    {
        _applicationRepository = applicationRepository;
        _interviewRepository = interviewRepository;
    }

    public async Task<dynamic?> GetApplicantDashboardDataAsync(string applicantId)
    {
        var applications = await _applicationRepository.GetByApplicantIdAsync(applicantId);
        var latest = applications.OrderByDescending(a => a.SubmittedAt).FirstOrDefault();

        var interviews = await _interviewRepository.GetAllAsync();
        var upcoming = interviews
            .Where(i => i.InterviewDate > DateTime.Now
                     && i.Status == InterviewStatus.Scheduled)
            .ToList();

        return new
        {
            CurrentStatus = latest?.Status.ToString() ?? "No application yet",
            SubmittedDate = latest?.SubmittedAt.ToString("MMMM d, yyyy") ?? string.Empty,
            UpcomingInterviews = upcoming
        };
    }

    public async Task<dynamic?> GetHRDashboardDataAsync()
    {
        var all = await _applicationRepository.GetAllAsync();
        var list = all.ToList();

        return new
        {
            TotalApplications = list.Count,
            Pending = list.Count(a => a.Status == ApplicationStatus.Submitted),
            UnderReview = list.Count(a => a.Status == ApplicationStatus.UnderReview),
            Accepted = list.Count(a => a.Status == ApplicationStatus.Accepted),
            Rejected = list.Count(a => a.Status == ApplicationStatus.Rejected)
        };
    }
}