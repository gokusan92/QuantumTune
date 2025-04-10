using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using WindowsOptimizer.Helpers;
using WindowsOptimizer.Models;
using WindowsOptimizer.Services;

namespace WindowsOptimizer.ViewModels
{
    public class StartupItemsViewModel : ViewModelBase, IPageViewModel
    {
        private readonly StartupService _startupService;
        private bool _isLoading;
        private bool _isOptimizing;
        private ObservableCollection<StartupItemModel> _startupItems;

        public StartupItemsViewModel(StartupService startupService)
        {
            _startupService = startupService;
            _startupItems = [];
            _isLoading = false;
            _isOptimizing = false;

            LoadStartupItemsCommand = new AsyncRelayCommand(ExecuteLoadStartupItemsAsync);
            ToggleStartupItemCommand = new AsyncRelayCommand(ExecuteToggleStartupItemAsync);
            OptimizeAllCommand = new AsyncRelayCommand(ExecuteOptimizeAllAsync);
        }

        public ObservableCollection<StartupItemModel> StartupItems
        {
            get => _startupItems;
            private set => SetProperty(ref _startupItems, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsOptimizing
        {
            get => _isOptimizing;
            private set => SetProperty(ref _isOptimizing, value);
        }

        public ICommand LoadStartupItemsCommand { get; }
        public ICommand ToggleStartupItemCommand { get; }
        public ICommand OptimizeAllCommand { get; }

        public async void OnNavigatedTo()
        {
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            await ExecuteLoadStartupItemsAsync(null);
        }

        private async Task ExecuteLoadStartupItemsAsync(object? parameter)
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;

                var items = new List<StartupItemModel>();

                // Fallback: Get from registry directly and startup folders
                try
                {
                    // Fetch the startup items from registry and startup folders concurrently
                    var registryItemsTask = GetStartupItemsFromRegistry();
                    var folderItemsTask = GetStartupItemsFromStartupFolders();

                    // Await both tasks
                    await Task.WhenAll(registryItemsTask, folderItemsTask);

                    // Add the results of both tasks to the list
                    items.AddRange(registryItemsTask.Result);
                    items.AddRange(folderItemsTask.Result);

                    // Service fetch could be added here for online functionality
                    var serviceItems = await _startupService.GetStartupItemsAsync();
                    if (serviceItems.Count > 0)
                    {
                        items.AddRange(serviceItems);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Service/Registry/Folder load failed: {ex.Message}");
                }

                // Update the ObservableCollection with all the startup items
                StartupItems = new ObservableCollection<StartupItemModel>(items);
            }
            finally
            {
                IsLoading = false;
            }
        }


        private async Task<List<StartupItemModel>> GetStartupItemsFromRegistry()
        {
            var items = new List<StartupItemModel>();

            try
            {
                // 1. Get items from HKCU\Run
                items.AddRange(await Task.Run(() => GetStartupItemsFromRegistryKey(Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "HKCU\\Run")));

                // 2. Get items from HKLM\Run
                items.AddRange(await Task.Run(() => GetStartupItemsFromRegistryKey(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "HKLM\\Run")));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading registry startup items: {ex.Message}");
            }

            return items;
        }

        private List<StartupItemModel> GetStartupItemsFromRegistryKey(RegistryKey registryKey, string keyPath, string location)
        {
            var items = new List<StartupItemModel>();

            using var key = registryKey.OpenSubKey(keyPath, false);
            if (key == null) return items;
            items.AddRange(from name in key.GetValueNames()
            let command = key.GetValue(name)?.ToString()
            where !string.IsNullOrEmpty(command)
            select new StartupItemModel
            {
                Name = name,
                Command = command,
                Location = location,
                IsEnabled = true,
                IsEssential = IsEssentialItem(name),
                Impact = DetermineImpact(name, command)
            });

            return items;
        }

        private async Task<List<StartupItemModel>> GetStartupItemsFromStartupFolders()
        {
            var items = new List<StartupItemModel>();

            try
            {
                string userStartupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string allUsersStartupFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);

                items.AddRange(await Task.Run(() => AddStartupFolderItems(userStartupFolder, "User Startup Folder")));
                items.AddRange(await Task.Run(() => AddStartupFolderItems(allUsersStartupFolder, "All Users Startup Folder")));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading startup folder items: {ex.Message}");
            }

            return items;
        }

        private List<StartupItemModel> AddStartupFolderItems(string folderPath, string location)
        {
            var items = new List<StartupItemModel>();

            if (!Directory.Exists(folderPath)) return items;

            foreach (var file in Directory.GetFiles(folderPath))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var extension = Path.GetExtension(file).ToLower();

                // Only include .exe and .lnk files
                if (extension == ".exe" || extension == ".lnk")
                {
                    items.Add(new StartupItemModel
                    {
                        Name = fileName,
                        Command = file,
                        Location = location,
                        IsEnabled = true,
                        IsEssential = false,
                        Impact = "Medium Impact"
                    });
                }
            }

            return items;
        }

        private static bool IsEssentialItem(string name)
        {
            string[] essentialItems = {
                "Windows Security",
                "Windows Defender",
                "Microsoft Security Client",
                "Explorer",
                "Shell Hardware Detection",
                "Security Center"
            };

            return essentialItems.Any(item => name.Contains(item, StringComparison.OrdinalIgnoreCase));
        }

        private static string DetermineImpact(string name, string command)
        {
            if (IsEssentialItem(name)) return "Essential";

            var nameLower = name.ToLower();
            var commandLower = command.ToLower();

            string[] highImpactKeywords =
            [
                "steam", "spotify", "adobe", "dropbox", "onedrive", "google",
                "discord", "teams", "skype", "zoom"
            ];

            return highImpactKeywords.Any(keyword => nameLower.Contains(keyword) || commandLower.Contains(keyword)) ? "High Impact" : "Medium Impact";
        }

        private async Task ExecuteToggleStartupItemAsync(object parameter)
        {
            if (IsOptimizing) return;

            if (parameter is StartupItemModel item)
            {
                try
                {
                    IsOptimizing = true;
                    var newEnabled = !item.IsEnabled;

                    var success = await _startupService.SetStartupItemEnabledAsync(item, newEnabled);

                    if (success)
                    {
                        item.IsEnabled = newEnabled;
                        OnPropertyChanged(nameof(StartupItems));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error toggling startup item: {ex.Message}");
                }
                finally
                {
                    IsOptimizing = false;
                }
            }
        }

        private async Task ExecuteOptimizeAllAsync(object parameter)
        {
            if (IsOptimizing) return;

            try
            {
                IsOptimizing = true;

                foreach (var item in StartupItems)
                {
                    if (item.IsEssential || !item.IsEnabled ||
                        (item.Impact != "High Impact" && item.Impact != "Medium Impact")) continue;
                    var success = await _startupService.SetStartupItemEnabledAsync(item, false);
                    if (success)
                    {
                        item.IsEnabled = false;
                    }
                }

                OnPropertyChanged(nameof(StartupItems));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error optimizing all: {ex.Message}");
            }
            finally
            {
                IsOptimizing = false;
            }
        }
    }
}
