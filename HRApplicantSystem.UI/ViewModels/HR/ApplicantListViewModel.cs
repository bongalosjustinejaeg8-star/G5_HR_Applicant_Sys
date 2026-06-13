using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRApplicantSystem.Data;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Enums;

namespace HRApplicantSystem.UI.ViewModels.HR;

public partial class ApplicantListViewModel : ViewModelBase
{
    private readonly MainWindowViewModel? _mainViewModel;

    [ObservableProperty] private ObservableCollection<dynamic> _applicants = new();
    [ObservableProperty] private object? _selectedApplicant;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _hasSelected = false;

    public ApplicantListViewModel() { _ = LoadApplicantsAsync(); }
    public ApplicantListViewModel(MainWindowViewModel mainViewModel) { _mainViewModel = mainViewModel; _ = LoadApplicantsAsync(); }

    partial void OnSelectedApplicantChanged(dynamic? value) => HasSelected = value != null;

    private async Task LoadApplicantsAsync()
    {
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var applicantRepo = new ApplicantRepository(db);
            var vacancyRepo = new JobVacancyRepository(db);

            var apps = await appRepo.GetAllAsync();
            Applicants.Clear();
            foreach (var app in apps)
            {
                var applicant = await applicantRepo.GetByIdAsync(app.ApplicantId);
                var vacancy = await vacancyRepo.GetByIdAsync(app.VacancyId);
                Applicants.Add(new
                {
                    ApplicationId = app.ApplicationId,
                    FirstName = applicant?.FullName ?? "Unknown",
                    Email = string.Empty,
                    Position = vacancy?.PositionTitle ?? "Unknown",
                    Status = app.Status.ToString(),
                    IsLocked = app.IsLocked
                });
            }
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    [RelayCommand]
    private async Task StartReview()
    {
        if (SelectedApplicant == null) return;
        try
        {
            var db = new DbContext(AppConfig.ConnectionString);
            var appRepo = new ApplicationRepository(db);
            var historyRepo = new ApplicationStatusHistoryRepository(db);
            var appId = ((dynamic)SelectedApplicant).ApplicationId as string ?? string.Empty;
            await appRepo.LockAsync(appId);
            await appRepo.UpdateStatusAsync(appId, ApplicationStatus.UnderReview);
            await historyRepo.CreateAsync(new HRApplicantSystem.Data.Models.ApplicationStatusHistory
            {
                ApplicationId = appId,
                ChangedBy = HRApplicantSystem.Shared.Helpers.SessionManager.CurrentUserId,
                OldStatus = ApplicationStatus.Submitted,
                NewStatus = ApplicationStatus.UnderReview,
                Remarks = "HR started review"
            });
            await LoadApplicantsAsync();
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
    }

    [RelayCommand] private void GoBack() => _mainViewModel?.NavigateToHRDashboard();
}