using System.Diagnostics;
using System.Windows.Input;
using WindowsOptimizer.Helpers;

namespace WindowsOptimizer.ViewModels;

public class SupportViewModel : ViewModelBase
{
    public SupportViewModel()
    {
        // Initialize commands
        OpenUserGuideCommand = new RelayCommand(ExecuteOpenUserGuide);
        ContactSupportCommand = new RelayCommand(ExecuteContactSupport);
        VisitForumCommand = new RelayCommand(ExecuteVisitForum);
    }

    public ICommand OpenUserGuideCommand { get; }
    public ICommand ContactSupportCommand { get; }
    public ICommand VisitForumCommand { get; }

    private void ExecuteOpenUserGuide(object parameter)
    {
        OpenUrl("https://www.example.com/userguide");
    }

    private void ExecuteContactSupport(object parameter)
    {
        OpenUrl("https://www.example.com/support");
    }

    private void ExecuteVisitForum(object parameter)
    {
        OpenUrl("https://www.example.com/forum");
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error opening URL: {ex.Message}");
        }
    }
}