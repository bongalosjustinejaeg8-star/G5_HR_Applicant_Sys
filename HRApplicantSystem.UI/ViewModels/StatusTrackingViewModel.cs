using CommunityToolkit.Mvvm.ComponentModel;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class StatusTrackingViewModel : ViewModelBase
    {
        [ObservableProperty]
        private int _currentStep = 3; 

        [ObservableProperty]
        private string _interviewSchedule = "June 15, 2026 - 2:00 PM via Microsoft Teams";

        [ObservableProperty]
        private string _hrRemarks = "Your initial technical evaluation looks promising! We have scheduled your technical interview. Please review C# OOP concepts and basic database design before the call.";

        [ObservableProperty]
        private string _finalResult = "Pending Interview Evaluation";
    }
}