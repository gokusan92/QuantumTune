namespace WindowsOptimizer.ViewModels;

public class CleanupItemViewModel : ViewModelBase
{
    private string _description = string.Empty;
    private bool _isSelected = true;
    private string _name = string.Empty;
    private string _size = string.Empty;
    private ulong _sizeInBytes;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public ulong SizeInBytes
    {
        get => _sizeInBytes;
        set => SetProperty(ref _sizeInBytes, value);
    }
}