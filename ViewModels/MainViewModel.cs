// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// ViewModels/MainViewModel.cs

using MemCard2025Creator.Commands;
using MemCard2025Creator.Views;
using MemCard2025Creator.ViewModels;
using MemCard2025Creator.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Media;
using System.Windows.Threading;
using System.Windows.Media;
using MemCard2025Creator.Utilities;
using System.Security;

namespace MemCard2025Creator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        private readonly CardManager _cardManager;


        public MainViewModel()
        {
            _cardManager = new CardManager();


            // Default to Home Mode
            CurrentView = new HomeView();
            ExportMemCardCommand = new RelayCommand(ExportMemCard);
            ExitCommand = new RelayCommand(ExitApplication);
            SingleCardCommand = new RelayCommand(OpenSingleCardView);
            BatchCardsCommand = new RelayCommand(OpenBatchCardsView);
            GenerateAudioFromFileCommand = new RelayCommand(OpenFile2AudioView);
            DeleteCategoryCommand = new RelayCommand(OpenDeleteCategoryView);

        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public ICommand ExportMemCardCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SingleCardCommand { get; }
        public ICommand BatchCardsCommand { get; }

        public ICommand GenerateAudioFromFileCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        // public object ZipFile { get; private set; }

        private void ExportMemCard(object parameter = null)
        {
            try
            {
                // Get the source and destination paths
                string sourceDirectory = Constants.Paths.Resource_Root;
                string destinationDirectory = Constants.Paths.Backup_Root;

                // Ensure the destination directory exists
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                // Generate the zip file name with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string zipFileName = $"MemCard-resource_{timestamp}.zip";
                string zipFilePath = Path.Combine(destinationDirectory, zipFileName);

                // Create the zip file
                ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath);

                // Notify the user
                MessageBox.Show($"Export completed successfully!\nBackup saved at:\n{zipFilePath}", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Handle errors and notify the user
                MessageBox.Show($"Export failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Create the destination directory
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If copying subdirectories, copy them and their contents to new location
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }

        private void ExitApplication(object parameter = null)
        {
            Application.Current.Shutdown();
        }

        private void OpenSingleCardView(object parameter)
        {
            CurrentView = new SingleCardView();
        }

        private void OpenBatchCardsView(object parameter)
        {
            CurrentView = new BatchCardView();
        }

        private void OpenFile2AudioView(object parameter)
        {
            CurrentView = new File2AudioView();
        }

        private void OpenDeleteCategoryView(object parameter)
        {
            CurrentView = new DeleteCategoryView();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
