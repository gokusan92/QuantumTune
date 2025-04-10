using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace WindowsOptimizer.Helpers;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<object, bool> _canExecute;
    private readonly Func<object, Task> _execute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<object, Task> execute, Func<object, bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return !_isExecuting && (_canExecute == null || _canExecute(parameter));
    }

    public async void Execute(object parameter)
    {
        if (!CanExecute(parameter)) return;

        try
        {
            _isExecuting = true;
            CommandManager.InvalidateRequerySuggested();
            await _execute(parameter);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as appropriate for your application
            Debug.WriteLine($"Command execution failed: {ex.Message}");
            MessageBox.Show($"Operation failed: {ex.Message}", "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _isExecuting = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}