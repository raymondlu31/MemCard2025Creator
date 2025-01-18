// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// ViewModels/SingleCardViewModel.cs

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

namespace MemCard2025Creator.ViewModels
{

    public class SingleCardViewModel : ViewModelBase
    {
        private bool _isENBCategory;
        private bool _isCustomizedCategory;
        private string _customizedCategory;
        private string _selectedCategory;
        private string _cardAlias;
        private string _cardUnicName;

        public ObservableCollection<string> ENBCategories { get; set; }
        public ObservableCollection<string> Existing_CardUniqueName { get; set; }

        public bool IsENBCategory
        {
            get => _isENBCategory;
            set { _isENBCategory = value; RaisePropertyChanged(); }
        }

        public bool IsCustomizedCategory
        {
            get => _isCustomizedCategory;
            set { _isCustomizedCategory = value; RaisePropertyChanged(); }
        }

        public string CustomizedCategory
        {
            get => _customizedCategory;
            set
            {
                _customizedCategory = value;
                UpdateCardUniqueName();
                RaisePropertyChanged();
            }
        }

        public bool ValidateCustomizedCategory(string Customized_Category)
        {
            // Validate input
            if (Customized_Category.Length <= 1)
            {
                MessageBox.Show("The category name must be at least 2 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;  // Exit early if validation fails
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(Customized_Category, @"^[A-Za-z0-9]+$"))
            {
                MessageBox.Show("The category name accepts letters and numbers only.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;  // Exit early if validation fails
            }

            if (ENBCategories.Contains(Customized_Category))
            {
                MessageBox.Show("Please select the category from the dropdown list above, or input a new one.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;  // Exit early if validation fails
            }

            _customizedCategory = Customized_Category;
            UpdateCardUniqueName();
            RaisePropertyChanged();

            return true;  // Validation passed
        }

        public bool ValidateCardUniqueName(string Unique_Name)
        {
            // Validate CardUniqueName


            if (Existing_CardUniqueName.Contains(Unique_Name))
            {
                MessageBox.Show("The card unique name is duplicate, please refresh card name and try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;  // Exit early if validation fails
            }
            UpdateCardUniqueName();
            RaisePropertyChanged();

            return true;  // Validation passed
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                UpdateCardUniqueName();
                RaisePropertyChanged();
            }
        }

        public string CardAlias
        {
            get => _cardAlias;
            set
            {
                _cardAlias = value;
                UpdateCardUniqueName();
                RaisePropertyChanged();
            }
        }

        public string Card_Unic_Name
        {
            get => _cardUnicName;
            private set { _cardUnicName = value; RaisePropertyChanged(); }
        }

        public ICommand RefreshCardCommand { get; }

        public ICommand EditCardCommand { get; }
        
        public ICommand CancelCardCommand { get; }
        public ICommand GoHomeCommand { get; }


        public SingleCardViewModel()
        {
            ENBCategories = new ObservableCollection<string>(LoadENBCategories());
            Existing_CardUniqueName = new ObservableCollection<string>(LoadExistingCardUniqueName());
            IsENBCategory = true;
            EditCardCommand = new RelayCommand(OpenSingleCardEditor, CanEditCard);
            RefreshCardCommand = new RelayCommand(RefreshCardUniqueName);
            CancelCardCommand = new RelayCommand(CancelCard);
            GoHomeCommand = new RelayCommand(GoHome);

        }

        public void RefreshCardUniqueName(object parameter)
        {
            UpdateCardUniqueName();
            RaisePropertyChanged();
        }

        private List<string> LoadENBCategories()
        {
            var ENBCategories = new List<string>();

            try
            {
                // Path to the existing and next category files
                var existingCategoryPath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, "existing-category.tmp");
                var nextCategoryPath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, "next-category.tmp");

                // Load categories from existing-category.tmp (one category per line)
                if (File.Exists(existingCategoryPath))
                {
                    var existingCategories = File.ReadAllLines(existingCategoryPath)
                                                  .Where(line => !string.IsNullOrWhiteSpace(line))
                                                  .Select(line => line.Trim());
                    ENBCategories.AddRange(existingCategories);
                }

                // Load the next category from next-category.tmp (only one line)
                if (File.Exists(nextCategoryPath))
                {
                    var nextCategory = File.ReadAllText(nextCategoryPath).Trim();
                    if (!string.IsNullOrWhiteSpace(nextCategory))
                    {
                        ENBCategories.Add(nextCategory);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return ENBCategories;
        }

        private List<string> LoadExistingCardUniqueName()
        {
            var Existing_CardUniqueName = new List<string>();

            try
            {

                // Load categories from existing-category.tmp (one category per line)
                if (File.Exists(Constants.Paths.CARD_LIST_FILE))
                {
                    var existingCardUniqueName = File.ReadAllLines(Constants.Paths.CARD_LIST_FILE)
                                                  .Where(line => !string.IsNullOrWhiteSpace(line))
                                                  .Select(line => line.Trim());
                    Existing_CardUniqueName.AddRange(existingCardUniqueName);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return Existing_CardUniqueName;
        }

        private void UpdateCardUniqueName()
        {
            string category = IsENBCategory ? SelectedCategory : CustomizedCategory;
            string subNumber = "0001"; // Default Sub_number
            if (IsENBCategory && !string.IsNullOrEmpty(category))
            {
                // Dynamically construct the file path for next-card-<category>.tmp
                var nextCardFileName = $"next-card-{category}.tmp";
                // Read sub-number from next-card-<category>.tmp
                var nextCard = Path.Combine(Constants.Paths.RUNTIME_FOLDER, nextCardFileName);
                var nextCardId = File.ReadAllText(nextCard).Trim();
                // nextCardId = <category>-<subNumber>
                subNumber = nextCardId.Split('-').LastOrDefault();
            }
            Card_Unic_Name = !string.IsNullOrEmpty(CardAlias) ? $"{category}-{subNumber}-{CardAlias}" : $"{category}-{subNumber}";
        }

        private bool CanEditCard(object parameter) => !string.IsNullOrEmpty(Card_Unic_Name);

        private void OpenSingleCardEditor(object parameter)
        {
            // Validate the customized category before proceeding
            if (!string.IsNullOrEmpty(CustomizedCategory) && IsCustomizedCategory)
            {
                // Run the validation method
                if (!ValidateCustomizedCategory(CustomizedCategory))
                {
                    if (!ValidateCardUniqueName(Card_Unic_Name))
                    {
                        // If validation fails, do not open the editor
                        return;  // Exit early without opening the editor
                    }
                    // If validation fails, do not open the editor
                    return;  // Exit early without opening the editor
                }
            }

            // Open the Single Card Editor window
            var editorWindow = new SingleCardEditorWindow(new SingleCardEditorViewModel(Card_Unic_Name));
            editorWindow.ShowDialog(); // This should open the window modally
        }

        private void CancelCard(object parameter)
        {
            Application.Current.MainWindow.DataContext = new MainViewModel();
        }

        private void GoHome(object parameter)
        {
            Application.Current.MainWindow.DataContext = new MainViewModel();
        }
    }

}

