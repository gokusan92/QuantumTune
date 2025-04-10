using WindowsOptimizer.ViewModels;

namespace WindowsOptimizer.Models;

public class StartupItemModel : ViewModelBase
{
    private string _command;
    private string _impact;
    private bool _isEnabled;
    private bool _isEssential;
    private string _location;
    private string _name;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Command
    {
        get => _command;
        set => SetProperty(ref _command, value);
    }

    public string Location
    {
        get => _location;
        set => SetProperty(ref _location, value);
    }

    public string Impact
    {
        get => _impact;
        set => SetProperty(ref _impact, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public bool IsEssential
    {
        get => _isEssential;
        set => SetProperty(ref _isEssential, value);
    }
}