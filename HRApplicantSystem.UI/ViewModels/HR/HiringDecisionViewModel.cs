using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Services.Implementations;
using HRApplicantSystem.Services.Interfaces;
using HRApplicantSystem.Shared.Enums;
using HRApplicantSystem.Shared.Helpers;

namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class HiringDecisionViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;
    private readonly IHiringDecisionService? _hiringDecisionService;

    [ObservableProperty] private ObservableCollection<dynamic> _applicants = new();
    [ObservableProperty] private object? _selectedApplicant;
    [ObservableProperty] private string _decision = "Accepted";
    [ObservableProperty] private string _remarks = string.Empty;
    [ObservableProperty] private string _message = string.Empty;
    [ObservableProperty] private bool _hasMessage = false;
    [ObservableProperty] private bool _canDecide = false;
    [ObservableProperty] private string _roleWarning = string.Empty;

    public HiringDecisionViewModel()
    {
        CanDecide = SessionManager.CurrentUserRole == UserRole.HRManager || SessionManager.CurrentUserRole == UserRole.Admin;
        RoleWarning = CanDecide ? string.Empty : "⚠️ Only HR Manager or Admin can make hiring decisions.";
        _ = LoadApplicantsAsync();
    }

    public HiringDecisionViewModel(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        CanDecide = SessionManager.CurrentUserRole == UserRole.HRManager || SessionManager.CurrentUserRole == UserRole.Admin;
        RoleWarning = CanDecide ? string.Empty : "⚠️ Only HR Manager or Admin can make hiring decisions.";
        _ = LoadApplicantsAsync();
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
            var eligible = apps.Where(a => a.Status == ApplicationStatus.ForFinalReview || a.Status == ApplicationStatus.ForInterview || a.Status == ApplicationStatus.Shortlisted);
            Applicants.Clear();
            foreach (var app in eligible)
            {
                var applicant = await applicantRepo.GetByIdAsync(app.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(app.VacancyId);
                Applicants.Add(new
                {
                    ApplicationId = app.ApplicationId,
                    FirstName = applicant?.FullName ?? "Unknown",
                    Position = vacancy?.PositionTitle ?? "Unknown",
                    Status = app.Status.ToString()
                });
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    [RelayCommand]
    private async Task SubmitDecision()
    {
        if (SelectedApplicant == null || !CanDecide) return;
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var decisionRepo = new HiringDecisionRepository(db);
            var appRepo = new ApplicationRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);

            var appId = ((dynamic)SelectedApplicant).ApplicationId as string ?? string.Empty;
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

            var newStatus = Decision == "Accepted" ? ApplicationStatus.Accepted : ApplicationStatus.Rejected;
            await appRepo.UpdateStatusAsync(appId, newStatus);
            await historyRepo.CreateAsync(new ApplicationStatusHistory
            {
                ApplicationId = appId,
                ChangedBy = SessionManager.CurrentUserId,
                OldStatus = app.Status,
                NewStatus = newStatus,
                Remarks = Remarks
            });

            Message = $"Decision submitted: {Decision}";
            HasMessage = true;
            await LoadApplicantsAsync();
        }
        catch (Exception ex) { Message = ex.Message; HasMessage = true; }
    }

    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToHRDashboard();
}