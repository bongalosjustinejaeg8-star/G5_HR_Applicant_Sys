using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HRApplicantSystem.UI.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _email = string.Empty;
    private string _password = string.Empty;

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}