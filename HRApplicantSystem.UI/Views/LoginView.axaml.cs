using Avalonia.Controls;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = new LoginViewModel();
    }
}