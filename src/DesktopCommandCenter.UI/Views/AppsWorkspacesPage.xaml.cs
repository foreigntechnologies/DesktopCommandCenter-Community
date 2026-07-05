using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DesktopCommandCenter.UI.Views
{
    public sealed partial class AppsWorkspacesPage : Page
    {
        private ObservableCollection<AppWorkspace> Workspaces { get; set; } = new ObservableCollection<AppWorkspace>();
        private AppWorkspace? _currentWorkspace;
        
        private string GetSavePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "DesktopCommandCenter");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            return Path.Combine(appFolder, "workspaces.json");
        }

        public AppsWorkspacesPage()
        {
            this.InitializeComponent();
            LoadData();
        }
        
        private void LoadData()
        {
            var path = GetSavePath();
            if (File.Exists(path))
            {
                try
                {
                    var json = File.ReadAllText(path);
                    var list = JsonSerializer.Deserialize<List<AppWorkspace>>(json);
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            Workspaces.Add(item);
                        }
                    }
                }
                catch { }
            }
            WorkspacesList.ItemsSource = Workspaces;
            
            if (Workspaces.Any())
            {
                WorkspacesList.SelectedIndex = 0;
            }
        }
        
        private void SaveData()
        {
            try
            {
                var path = GetSavePath();
                var json = JsonSerializer.Serialize(Workspaces.ToList());
                File.WriteAllText(path, json);
            }
            catch { }
        }

        private async void BtnNewWorkspace_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Novo Ambiente",
                PrimaryButtonText = "Criar",
                CloseButtonText = "Cancelar",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var inputTextBox = new TextBox { PlaceholderText = "Nome do Ambiente (ex: Web Dev)" };
            dialog.Content = inputTextBox;

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                var ws = new AppWorkspace { Id = Guid.NewGuid().ToString(), Name = inputTextBox.Text };
                Workspaces.Add(ws);
                SaveData();
                WorkspacesList.SelectedItem = ws;
            }
        }

        private void WorkspacesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkspacesList.SelectedItem is AppWorkspace ws)
            {
                _currentWorkspace = ws;
                WorkspaceDetailsArea.Visibility = Visibility.Visible;
                CurrentWorkspaceName.Text = ws.Name;
                CurrentWorkspaceAppCount.Text = $"{ws.Apps.Count} aplicativos vinculados";
                AppsGridView.ItemsSource = ws.Apps;
                
                if (ws.Apps.Count == 0)
                {
                    EmptyStateArea.Visibility = Visibility.Visible;
                    AppsGridView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    EmptyStateArea.Visibility = Visibility.Collapsed;
                    AppsGridView.Visibility = Visibility.Visible;
                }
            }
            else
            {
                _currentWorkspace = null;
                WorkspaceDetailsArea.Visibility = Visibility.Collapsed;
            }
        }

        private async void BtnAddApp_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWorkspace == null) return;

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            var window = (Microsoft.UI.Xaml.Application.Current as App)?.MainWindow;
            if (window != null)
            {
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
            }

            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".exe");
            picker.FileTypeFilter.Add(".bat");
            picker.FileTypeFilter.Add(".cmd");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var app = new WorkspaceApp 
                { 
                    Id = Guid.NewGuid().ToString(), 
                    Name = file.DisplayName, 
                    Path = file.Path 
                };
                
                _currentWorkspace.Apps.Add(app);
                SaveData();
                
                var temp = _currentWorkspace;
                WorkspacesList.SelectedItem = null;
                WorkspacesList.SelectedItem = temp;
            }
        }

        private void BtnLaunchAll_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWorkspace == null) return;
            
            foreach (var app in _currentWorkspace.Apps)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = app.Path,
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(app.Path)
                    });
                }
                catch { }
            }
        }

        private void BtnLaunchApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string id)
            {
                var app = _currentWorkspace?.Apps.FirstOrDefault(a => a.Id == id);
                if (app != null)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = app.Path,
                            UseShellExecute = true,
                            WorkingDirectory = Path.GetDirectoryName(app.Path)
                        });
                    }
                    catch { }
                }
            }
        }

        private void BtnRemoveApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string id)
            {
                var app = _currentWorkspace?.Apps.FirstOrDefault(a => a.Id == id);
                if (app != null)
                {
                    _currentWorkspace?.Apps.Remove(app);
                    SaveData();
                    
                    var temp = _currentWorkspace;
                    WorkspacesList.SelectedItem = null;
                    WorkspacesList.SelectedItem = temp;
                }
            }
        }
    }

    public class AppWorkspace
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ObservableCollection<WorkspaceApp> Apps { get; set; } = new ObservableCollection<WorkspaceApp>();
    }

    public class WorkspaceApp
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
