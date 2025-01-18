// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// ViewModels/DeleteCategoryViewModel.cs

using MemCard2025Creator.Commands;
using MemCard2025Creator.Utilities;
using MemCard2025Creator.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.IO;
using MemCard2025Creator.Models;
using MemCard2025Creator.Services;

namespace MemCard2025Creator.ViewModels
{
    public class DeleteCategoryViewModel : ViewModelBase
    {
        private string _selectedCategory;
        public ObservableCollection<string> ExistingCategories { get; private set; }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsDeleteCategoryEnabled));
            }
        }

        public bool IsDeleteCategoryEnabled => !string.IsNullOrEmpty(SelectedCategory);

        public ICommand DeleteCategoryCommand { get; }
        public ICommand CancelChangeCommand { get; }
        public ICommand GoHomeCommand { get; }

        public DeleteCategoryViewModel()
        {
            ExistingCategories = new ObservableCollection<string>();
            DeleteCategoryCommand = new RelayCommand(DeleteCategory, CanDeleteCategory);
            CancelChangeCommand = new RelayCommand(CancelChange);
            GoHomeCommand = new RelayCommand(GoHome);

            LoadExistingCategories();
        }

        private void LoadExistingCategories()
        {
            string filePath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, "existing-category.tmp");

            if (File.Exists(filePath))
            {
                var categories = File.ReadAllLines(filePath);
                ExistingCategories.Clear();
                foreach (var category in categories)
                {
                    ExistingCategories.Add(category);
                }
            }
        }

        private bool CanDeleteCategory(object parameter)
        {
            return IsDeleteCategoryEnabled;
        }

        private void DeleteCategory(object parameter)
        {
            if (MessageBox.Show($"Are you sure you want to delete the category '{SelectedCategory}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    DeleteMediaFiles();
                    UpdateCardListFile();

                    // Clear runtime folder
                    foreach (var file in Directory.GetFiles(Constants.Paths.RUNTIME_FOLDER))
                    {
                        File.Delete(file);
                    }
                    // Instantiate the AppInitializer
                    var appInitializer4 = new AppInitializer();
                    // Call InitialLoad to perform startup tasks
                    appInitializer4.InitialLoad();
                    MessageBox.Show("Category deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadExistingCategories(); // Refresh the list of categories
                    SelectedCategory = null; // Reset selection
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteMediaFiles()
        {
            string[] directories = { Constants.Paths.Image_Directory, Constants.Paths.Subtitle_Directory, Constants.Paths.Audio_Directory };

            foreach (var directory in directories)
            {
                if (Directory.Exists(directory))
                {
                    var filesToDelete = Directory.GetFiles(directory, $"{SelectedCategory}-*.*");
                    foreach (var file in filesToDelete)
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        private void UpdateCardListFile()
        {
            string cardListFile = Constants.Paths.CARD_LIST_FILE;

            if (File.Exists(cardListFile))
            {
                var updatedLines = File.ReadAllLines(cardListFile)
                                       .Where(line => !line.StartsWith($"{SelectedCategory}-"))
                                       .ToArray();
                var removeEmptyLines = updatedLines.Where(line2 => !string.IsNullOrWhiteSpace(line2)).ToArray();
                
                File.WriteAllLines(cardListFile, removeEmptyLines);
                
            }
        }

        private void CancelChange(object parameter)
        {
            Application.Current.MainWindow.DataContext = new MainViewModel();
        }

        private void GoHome(object parameter)
        {
            Application.Current.MainWindow.DataContext = new MainViewModel();
        }
    }
}




