// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// ViewModels/File2AudioViewModel.cs

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
using System.Text.RegularExpressions;

namespace MemCard2025Creator.ViewModels
{
    public class File2AudioViewModel : ViewModelBase
    {
        private string _readingFilePath;
        private string category_name;


        public string ReadingFilePath
        {
            get => _readingFilePath;
            set
            {
                _readingFilePath = value;
                RaisePropertyChanged();
                UpdateOutputDirectory();
            }
        }

        public string OutputDirectory;

        public ICommand SelectReadingFileCommand { get; }
        public ICommand GenerateMediaCommand { get; }

        public ICommand CancelCommand { get; }
        public ICommand GoHomeCommand { get; }

        public File2AudioViewModel()
        {
            SelectReadingFileCommand = new RelayCommand(SelectReadingFile);
            GenerateMediaCommand = new RelayCommand(GenerateMedia, CanGenerateMedia);
            CancelCommand = new RelayCommand(CancelChange);
            GoHomeCommand = new RelayCommand(GoHome);
        }

        private void SelectReadingFile(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ReadingFilePath = openFileDialog.FileName;
            }
        }

        private void UpdateOutputDirectory()
        {
            if (!string.IsNullOrEmpty(ReadingFilePath))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ReadingFilePath);
                // use the the first word in file name as the category name
                category_name = Regex.Match(fileNameWithoutExtension, @"[A-Za-z]+").Value;
                OutputDirectory = Path.Combine(Constants.Paths.RUNTIME_FOLDER, $"GeneratedFrom-{fileNameWithoutExtension}");
            }
        }

        private bool CanGenerateMedia(object parameter)
        {
            return !string.IsNullOrEmpty(ReadingFilePath) && Directory.Exists(Constants.Paths.RUNTIME_FOLDER);
        }

        private void GenerateMedia(object parameter)
        {
            try
            {
                if (string.IsNullOrEmpty(ReadingFilePath))
                {
                    MessageBox.Show("No reading file selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!File.Exists(ReadingFilePath))
                {
                    MessageBox.Show("The selected reading file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Directory.CreateDirectory(OutputDirectory);

                string[] lines = File.ReadAllLines(ReadingFilePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    string subNumber = (i+1).ToString("D4");
                    
                    string subtitleText = lines[i];

                    // get the first word of the subtitleText as CardAlias
                    // use regular expression to get the first word [A-Za-z]+
                    string alias = Regex.Match(subtitleText, @"[A-Za-z]+").Value;

                    string subtitleFileName = Path.Combine(OutputDirectory, $"{category_name}-{subNumber}-{alias}.txt");
                    string audioFileName = Path.Combine(OutputDirectory, $"{category_name}-{subNumber}-{alias}.mp3");

                    // Save subtitle text
                    File.WriteAllText(subtitleFileName, subtitleText);

                    // Generate audio (placeholder logic - replace with actual TTS generation code)
                    GenerateAudioFromLine(subtitleText, audioFileName);
                }

                MessageBox.Show($"Audio files generated successfully in {OutputDirectory}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateAudioFromLine(string text, string audioFilePath)
        {
            if (!string.IsNullOrWhiteSpace(text))
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
                        synthesizer.Speak(text);
                    }

                    // MessageBox.Show($"Audio file generated and saved to {audioFilePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating audio: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

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


