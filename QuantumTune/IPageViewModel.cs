using System.ComponentModel;

public interface IPageViewModel : INotifyPropertyChanged
{
    bool IsLoading { get; set; }
    Task LoadAsync();
    void OnNavigatedTo();
}