namespace HRApplicantSystem.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage;

        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public MainWindowViewModel()
        {
            CurrentPage = new LoginViewModel();
        }
    }
}