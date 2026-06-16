using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Data.Repositories;


namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class HiringDecisionViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty] private ObservableCollection<dynamic> _applicants = new();
    [ObservableProperty] private object? _selectedApplicant;

    // ✅ MUST match RadioButton values in XAML
    [ObservableProperty] private string _decision = "Accept";

    [ObservableProperty] private string _remarks = string.Empty;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private bool _hasMessage = false;

    [ObservableProperty] private bool _canDecide = false;
    [ObservableProperty] private string _roleWarning = string.Empty;

    public HiringDecisionViewModel()
    {
        CheckRole();
        _ = LoadApplicantsAsync();
    }

    public HiringDecisionViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        CheckRole();
        _ = LoadApplicantsAsync();
    }

    private void CheckRole()
    {
        CanDecide = SessionManager.CurrentUserRole == UserRole.HRManager
                 || SessionManager.CurrentUserRole == UserRole.Admin;

        RoleWarning = CanDecide
            ? string.Empty
            : "⚠️ Access denied — only HR Manager or Admin can make final hiring decisions.";
    }

    private async Task LoadApplicantsAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var applicantRepo = new ApplicantRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);

            var apps = await appRepo.GetAllAsync();

            var eligible = apps.Where(a =>
                a.Status == ApplicationStatus.ForFinalReview ||
                a.Status == ApplicationStatus.Shortlisted ||
                a.Status == ApplicationStatus.ForInterview);

            Applicants.Clear();

            foreach (var app in eligible)
            {
                var applicant = await applicantRepo.GetByIdAsync(app.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(app.VacancyId);

                Applicants.Add(new
                {
                    ApplicationId = app.ApplicationId,
                    FullName = applicant?.FullName ?? "Unknown",
                    Position = vacancy?.PositionTitle ?? "Unknown",
                    Status = app.Status.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private async Task SubmitDecision()
    {
        if (SelectedApplicant == null)
        {
            Message = "⚠️ Please select an applicant first.";
            HasMessage = true;
            return;
        }

        if (!CanDecide)
        {
            Message = "⚠️ Access denied — only HR Manager or Admin can submit hiring decisions.";
            HasMessage = true;
            return;
        }

        // ✅ safety check (prevents invalid binding states)
        if (Decision != "Accept" && Decision != "Reject")
        {
            Message = "⚠️ Please select Accept or Reject before submitting.";
            HasMessage = true;
            return;
        }

        try
        {
            var appId = ((dynamic)SelectedApplicant).ApplicationId as string ?? string.Empty;

            var db = new DbContext(AppConfig.ConnectionString);
            var decisionRepo = new HiringDecisionRepository(db);
            var appRepo = new ApplicationRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);

            var app = await appRepo.GetByIdAsync(appId);
            if (app == null) return;

            var hiringDecision = new HiringDecision
            {
                ApplicationId = appId,
                DecidedBy = SessionManager.CurrentUserId,
                Decision = Decision,
                Remarks = Remarks,
                DecidedAt = DateTime.Now
            };

            await decisionRepo.CreateAsync(hiringDecision);

            // ✅ FIXED: must match Decision exactly
            var newStatus = Decision == "Accept"
                ? ApplicationStatus.Accepted
                : ApplicationStatus.Rejected;

            await appRepo.UpdateStatusAsync(appId, newStatus);

            await historyRepo.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = appId,
                ChangedBy = SessionManager.CurrentUserId,
                OldStatus = app.Status,
                NewStatus = newStatus,
                Remarks = $"Final decision: {Decision}. {Remarks}"
            });

            Message = $"✅ Decision submitted — applicant {Decision.ToLower()}.";
            HasMessage = true;
            Remarks = string.Empty;

            await LoadApplicantsAsync();
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            HasMessage = true;
        }
    }

    [RelayCommand]
    private void GoBack()
        => _mainViewModel?.NavigateToHRDashboard();
}