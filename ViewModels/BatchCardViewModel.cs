// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// ViewModels/BatchCardViewModel.cs

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
using Microsoft.Win32;
using System.Windows.Controls;
using System.Speech.Synthesis;
using MemCard2025Creator.Services;

namespace MemCard2025Creator.ViewModels
{

    public class BatchCardViewModel : ViewModelBase
    {
        private string _customizedCategory;
        private int _numberOfImagesSelected;
        private readonly string _existingCategoriesFile = Path.Combine(Constants.Paths.RUNTIME_FOLDER, "existing-category.tmp");
        private readonly string BatchCard_ImageDirectory = Constants.Paths.Image_Directory;
        private readonly string BatchCard_SubtitleDirectory = Constants.Paths.Subtitle_Directory;
        private readonly string BatchCard_AudioDirectory = Constants.Paths.Audio_Directory;

        public BatchCardViewModel()
        {
            ValidateCategoryCommand = new RelayCommand(DoValidateCategory);
            RefreshCardCommand = new RelayCommand(RefreshCategories);
            SelectBatchImagesCommand = new RelayCommand(SelectBatchImages);
            GenerateBatchCardsCommand = new RelayCommand(GenerateBatchCards, CanGenerateBatchCards);
            CancelCardCommand = new RelayCommand(CancelBatchCards);
            GoHomeCommand = new RelayCommand(GoHome);

            SelectedImagePaths = new ObservableCollection<string>();
            LoadExistingCategories();

        }

        private bool CanGenerateBatchCards(object parameter)
        {
            return SelectedImagePaths.Count > 0;
        }

        public string CustomizedCategory
        {
            get => _customizedCategory;
            set
            {
                _customizedCategory = value;
                RaisePropertyChanged();
            }
        }

        public int NumberOfImagesSelected
        {
            get => _numberOfImagesSelected;
            set
            {
                _numberOfImagesSelected = value;
                RaisePropertyChanged(nameof(NumberOfImagesSelected));
            }
        }

        public ObservableCollection<string> SelectedImagePaths { get; }

        public ICommand ValidateCategoryCommand { get; }
        public ICommand RefreshCardCommand { get; }
        public ICommand SelectBatchImagesCommand { get; }
        public ICommand GenerateBatchCardsCommand { get; }
        public ICommand CancelCardCommand { get; }
        public ICommand GoHomeCommand { get; }

        private List<string> ExistingCategories { get; set; }

        private void LoadExistingCategories()
        {
            if (File.Exists(_existingCategoriesFile))
            {
                ExistingCategories = File.ReadAllLines(_existingCategoriesFile).ToList();
            }
            else
            {
                ExistingCategories = new List<string>();
            }
        }

        private void DoValidateCategory(object parameter)
        {
            if (parameter is string category)
            {
                if (ValidateCategory(category))
                {
                    MessageBox.Show("Category name is valid.", "Validation Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Invalid category input.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public bool ValidateCategory(string Customized_Category)
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

            if (ExistingCategories.Contains(Customized_Category))
            {
                MessageBox.Show("The category name is existing. Please input a new one for the batch.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;  // Exit early if validation fails
            }

            _customizedCategory = Customized_Category;
            RaisePropertyChanged();

            return true;  // Validation passed
        }

        private void RefreshCategories(object parameter)
        {
            LoadExistingCategories();
            System.Windows.MessageBox.Show("Categories refreshed successfully.", "Refresh Success", System.Windows.MessageBoxButton.OK);
        }

        private void SelectBatchImages(object parameter)
        {
            // Validate the category before proceeding
            if (!ValidateCategory(_customizedCategory))
            {
                MessageBox.Show("Invalid category input.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Exit if the category validation fails
            }

            // Open file dialog to select images
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files (*.JPG;*.png)|*.JPG;*.png|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImagePaths.Clear();
                foreach (var file in openFileDialog.FileNames)
                {
                    SelectedImagePaths.Add(file);
                }

                NumberOfImagesSelected = SelectedImagePaths.Count;
            }
        }

        private void GenerateBatchCards(object parameter)
        {
            if (SelectedImagePaths.Count == 0)
            {
                System.Windows.MessageBox.Show("No images selected. Please select images to generate cards.", "Error", System.Windows.MessageBoxButton.OK);
                return;
            }

            string category = CustomizedCategory;
            string nextCardFilePath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, $"BatchCard-next-{category}.tmp");

            int nextSubNumber = File.Exists(nextCardFilePath)
                ? int.Parse(File.ReadAllText(nextCardFilePath))
                : 1;

            foreach (var imagePath in SelectedImagePaths)
            {
                string originalFileName = Path.GetFileNameWithoutExtension(imagePath);
                string subNumber = nextSubNumber.ToString("D4");
                string uniqueName = $"{category}-{subNumber}-{originalFileName}";
                string subtitle = originalFileName;

                // Copy image
                string outputImagePath = Path.Combine(BatchCard_ImageDirectory, $"{uniqueName}.JPG");
                Directory.CreateDirectory(Path.GetDirectoryName(outputImagePath));
                // File.Copy(imagePath, outputImagePath, true);
                // Replace the existing image copy code with:
                CardMediaHelper.AdjustAndSaveImage(imagePath, outputImagePath);

                // Create subtitle file
                string subtitleFilePath = Path.Combine(BatchCard_SubtitleDirectory, $"{uniqueName}.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(subtitleFilePath));
                File.WriteAllText(subtitleFilePath, subtitle);

                // Generate audio file
                string audioFilePath = Path.Combine(BatchCard_AudioDirectory, $"{uniqueName}.mp3");
                // string audioFilePath = Path.Combine(BatchCard_AudioDirectory, $"{uniqueName}.wav");
                Directory.CreateDirectory(Path.GetDirectoryName(audioFilePath));
                GenerateAudio(subtitle, audioFilePath);

                // Update card-list.txt
                var cardListPath = Path.Combine(Constants.Paths.CONFIG_FOLDER, "card-list.txt");
                File.AppendAllText(cardListPath, Environment.NewLine + uniqueName);

                nextSubNumber++;
            }

            

            File.WriteAllText(nextCardFilePath, nextSubNumber.ToString());


            // remove empty lines from card-list.txt
            string cardListFile = Constants.Paths.CARD_LIST_FILE;

            if (File.Exists(cardListFile))
            {
                var removeEmptyLines = File.ReadAllLines(cardListFile)
                                       .Where(line =>
                                           !string.IsNullOrWhiteSpace(line)) // Exclude empty or whitespace lines
                                       .ToArray();

                File.WriteAllLines(cardListFile, removeEmptyLines);
            }

            System.Windows.MessageBox.Show("Batch cards generated successfully.", "Success", System.Windows.MessageBoxButton.OK);

            // Clear runtime folder
            foreach (var file in Directory.GetFiles(Constants.Paths.RUNTIME_FOLDER))
            {
                File.Delete(file);
            }
            // Instantiate the AppInitializer
            var appInitializer3 = new AppInitializer();
            // Call InitialLoad to perform startup tasks
            appInitializer3.InitialLoad();
        }

        private void GenerateAudio(string subtitle, string audioFilePath)
        {
            if (!string.IsNullOrWhiteSpace(subtitle))
            {
                try
                {

                    using (var synthesizer = new SpeechSynthesizer())
                    {
                        synthesizer.Rate = -5; // from -10 to 10, the less the slower
                        synthesizer.Volume = 100; // Max volume

                        // Choose the voice (optional)
                        synthesizer.SelectVoiceByHints(VoiceGender.Female);

                        synthesizer.SetOutputToWaveFile(audioFilePath);
                        synthesizer.Speak(subtitle);
                    }

                    MessageBox.Show($"Audio file generated and saved to {audioFilePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating audio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            // Placeholder for actual TTS implementation
            // File.WriteAllText(audioFilePath, $"Audio content for: {subtitle}");
        }

        private void CancelBatchCards(object parameter)
        {
            Application.Current.MainWindow.DataContext = new MainViewModel();
        }

        private void GoHome(object parameter)
        {
            Application.Current.MainWindow.DataContext = new MainViewModel();
        }

    }

}

