// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// ViewModels/SingleCardEditorViewModel.cs

using MemCard2025Creator.Commands;
using MemCard2025Creator.Utilities;
using MemCard2025Creator.Views;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Speech.Synthesis;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MemCard2025Creator.Models;
using MemCard2025Creator.Services;
using MemCard2025Creator.ViewModels;

namespace MemCard2025Creator.ViewModels
{

    public class SingleCardEditorViewModel : ViewModelBase
    {
        public string ImagePath { get; set; }
        public string Subtitle { get; set; }
        public string CardUniqueName { get; }

        public string CardRuntime_ImagePath { get; private set; }
        public string CardRuntime_SubtitlePath { get; private set; }
        public string CardRuntime_AudioPath { get; private set; }

        public string CardSave_ImagePath { get; private set; }
        public string CardSave_SubtitlePath { get; private set; }
        public string CardSave_AudioPath { get; private set; }

        public ICommand SelectImageCommand { get; }
        public ICommand ImageShaperCommand { get; }
        public ICommand GenerateAudioCommand { get; }
        public ICommand SaveCardCommand { get; }

        public ICommand CancelCardCommand { get; }

        public SingleCardEditorViewModel(string cardUniqueName)
        {
            CardUniqueName = cardUniqueName;

            // Commands
            SelectImageCommand = new RelayCommand(SelectImage);
            GenerateAudioCommand = new RelayCommand(GenerateAudio);
            SaveCardCommand = new RelayCommand(SaveCard);
            CancelCardCommand = new RelayCommand(CancelCard);
            ImageShaperCommand = new RelayCommand(OpenImageShaper);

            // Paths for runtime files
            CardRuntime_ImagePath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, "CurrentSingleCardReshape.JPG");
            CardRuntime_SubtitlePath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, "CurrentSingleCardSubtitle.txt");
            CardRuntime_AudioPath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, "CurrentSingleCardAudio.mp3");

            // Paths for saving files
            CardSave_ImagePath = Path.Combine(Constants.Paths.Image_Directory, CardUniqueName + ".JPG");
            CardSave_SubtitlePath = Path.Combine(Constants.Paths.Subtitle_Directory, CardUniqueName + ".txt");
            CardSave_AudioPath = Path.Combine(Constants.Paths.Audio_Directory, CardUniqueName + ".mp3");

        }

        private void SelectImage(object parameter)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select Image",
                Filter = "Image Files (*.png;*.JPG)|*.png;*.JPG"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                RaisePropertyChanged(nameof(ImagePath));
            }
        }

        private void OpenImageShaper(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(ImagePath))
            {
                var imageShaperWindow = new ImageShaperWindow(ImagePath);
                imageShaperWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an image first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void GenerateAudio(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(Subtitle))
            {
                try
                {

                    using (var synthesizer = new SpeechSynthesizer())
                    {
                        synthesizer.Rate = -5; // from -10 to 10, the less the slower
                        synthesizer.Volume = 100; // Max volume

                        // Choose the voice (optional)
                        synthesizer.SelectVoiceByHints(VoiceGender.Female);

                        synthesizer.SetOutputToWaveFile(CardRuntime_AudioPath);
                        synthesizer.Speak(Subtitle);
                    }

                    MessageBox.Show($"Audio file generated and saved to {CardRuntime_AudioPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating audio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                SaveSubtitle(Subtitle);
            }
        }

        private void SaveSubtitle(string subtitle)
        {
            if (!string.IsNullOrWhiteSpace(subtitle))
            {

                File.WriteAllText(CardRuntime_SubtitlePath, subtitle);
                MessageBox.Show($"Subtitle saved to {CardRuntime_SubtitlePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void SaveCard(object parameter)
        {
            // move file CardRuntime_ImagePath to CardSave_ImagePath and rename it
            // move file CardRuntime_SubtitlePath to CardSave_SubtitlePath and rename it
            // move file CardRuntime_AudioPath to CardSave_AudioPath and rename it
            // Update card-list.txt: append CardUniqueName to the end of the file
            // clear and re-create files in runtime folder, the procedure is as app initial start
            try
            {
                // Ensure directories exist
                Directory.CreateDirectory(Path.GetDirectoryName(CardSave_ImagePath));
                Directory.CreateDirectory(Path.GetDirectoryName(CardSave_SubtitlePath));
                Directory.CreateDirectory(Path.GetDirectoryName(CardSave_AudioPath));

                // Move runtime files to save locations
                if (File.Exists(CardRuntime_ImagePath)) File.Move(CardRuntime_ImagePath, CardSave_ImagePath);
                if (File.Exists(CardRuntime_SubtitlePath)) File.Move(CardRuntime_SubtitlePath, CardSave_SubtitlePath);
                if (File.Exists(CardRuntime_AudioPath)) File.Move(CardRuntime_AudioPath, CardSave_AudioPath);

                // Update card-list.txt
                var cardListPath = Path.Combine(Constants.Paths.CONFIG_FOLDER, "card-list.txt");
                File.AppendAllText(cardListPath, Environment.NewLine + CardUniqueName);

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


                // Clear runtime folder
                foreach (var file in Directory.GetFiles(Constants.Paths.RUNTIME_FOLDER))
                {
                    File.Delete(file);
                }

                MessageBox.Show("Card saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving card: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Instantiate the AppInitializer
            var appInitializer2 = new AppInitializer();
            // Call InitialLoad to perform startup tasks
            appInitializer2.InitialLoad();

            // close the SingleCardEditorWindow.xaml window
            Application.Current.Windows.OfType<SingleCardEditorWindow>().FirstOrDefault()?.Close();

        }

        private void CancelCard(object parameter)
        {
            // close the SingleCardEditorWindow.xaml window
            Application.Current.Windows.OfType<SingleCardEditorWindow>().FirstOrDefault()?.Close();
        }
    }

}

