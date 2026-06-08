using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using HRApplicantSystem.UI.ViewModels;
using HRApplicantSystem.UI.Views;

namespace HRApplicantSystem.UI;



public partial class App : Application
{
    public override void OnFrameworkInitializationCompleted()
    {
        AppConfig.Load(); // ← add this line!

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}