﻿using CommunityToolkit.Mvvm.ComponentModel;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentPage;

        public MainWindowViewModel()
        {
            _currentPage = new LoginViewModel(this);
        }

        public void NavigateTo(ViewModelBase viewModel)
        {
            CurrentPage = viewModel;
        }
    }
}