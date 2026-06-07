using Avalonia;
using Avalonia.Controls;
using HRApplicantSystem.UI.ViewModels;

namespace HRApplicantSystem.UI.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();
        }
    }
}