// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Services/FileService.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MemCard2025Creator.Models;
using MemCard2025Creator.Utilities;

namespace MemCard2025Creator.Services
{
    public class FileService
    {
        private readonly string _baseDirectory;
        private readonly string _runtimeDirectory;
        private readonly string _cardListPath;

        // public List<Card> CurrentCardList { get; private set; }
        public List<Card> runtimeCardList { get; private set; }

        public FileService()
        {
            _baseDirectory = Constants.Paths.BASE_DIRECTORY;
            _runtimeDirectory = Constants.Paths.RUNTIME_FOLDER;
            _cardListPath = Constants.Paths.CARD_LIST_FILE;
        }

        public void ClearRuntimeDirectory()
        {
            if (Directory.Exists(_runtimeDirectory))
            {
                Directory.GetFiles(_runtimeDirectory).ToList().ForEach(File.Delete);
            }
            else
            {
                Directory.CreateDirectory(_runtimeDirectory);
            }
        }

        /// <summary>
        /// Writes the CurrentCardList to runtime files, grouped by category.
        /// </summary>
        public void Write_CurrentCardList_ToRuntimeFiles(List<Card> runtime_Card_List)
        {
            runtimeCardList = runtime_Card_List;

            try
            {
                // Ensure the runtime folder exists
                if (!Directory.Exists(Constants.Paths.RUNTIME_FOLDER))
                {
                    Directory.CreateDirectory(Constants.Paths.RUNTIME_FOLDER);
                }

                // Group cards by category and write each group to a separate file
                var groupedCards = runtimeCardList.GroupBy(card => card.Category);

                foreach (var group in groupedCards)
                {
                    var filePath = Path.Combine(Constants.Paths.RUNTIME_FOLDER, $"currentSequence-category-{group.Key}.tmp");
                    File.WriteAllLines(filePath, group.Select(card => card.Card_Unic_Name));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing runtime files: {ex.Message}", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads cards from runtime files into CurrentCardList.
        /// </summary>
        public List<Card> Load_CurrentCardList_FromRuntimeFiles()
        {
            runtimeCardList = new List<Card>();
            try
            {
                var cardList = new List<Card>();

                foreach (var filePath in Directory.GetFiles(Constants.Paths.RUNTIME_FOLDER, "currentSequence-category-*.tmp"))
                {
                    var cardLines = File.ReadAllLines(filePath).ToList();

                    // Convert each line to a Card object
                    var cards = cardLines
                        .Select(Card.ParseCardFromCardUnicName)
                        .Where(card => card != null) // Exclude invalid cards
                        .ToList();

                    cardList.AddRange(cards);
                }

                runtimeCardList = cardList;
                // ShowCurrentCardList(); // Optional: For debugging
                // string cardListString = string.Join("\n", CurrentCardList.Select(card => card.Card_Unic_Name));
                // MessageBox.Show(cardListString, "---- Current Card List ----");

                return runtimeCardList;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading runtime files: {ex.Message}", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                runtimeCardList = new List<Card>(); // Reset to an empty list on error
                return runtimeCardList;
            }
            
        }

        public void WriteInitRuntimeFiles(IEnumerable<Card> cards)
        {
            // Group cards by category
            var cardsByCategory = cards.GroupBy(c => c.Category);

            // Determine the next category alphabetically
            var existingCategories = cardsByCategory.Select(g => g.Key).OrderBy(c => c).ToList();
            var nextCategory = GetNextCategory(existingCategories.LastOrDefault());

            // Write currentSequence-category-<Category>.tmp files
            foreach (var group in cardsByCategory)
            {
                var category = group.Key;
                var filePath2 = Path.Combine(_runtimeDirectory, $"currentSequence-category-{category}.tmp");

                // Write all card IDs in the current Sequence category to the file
                File.WriteAllLines(filePath2, group.Select(c => c.Card_Unic_Name));
            }

            // Write current-category-<Category>.tmp files
            foreach (var group in cardsByCategory)
            {
                var category = group.Key;
                var filePath = Path.Combine(_runtimeDirectory, $"current-category-{category}.tmp");
                

                // Write all card IDs in the current category to the file
                File.WriteAllLines(filePath, group.Select(c => c.Card_Unic_Name));

                // Write the next card ID for each category
                var nextCardFile = Path.Combine(_runtimeDirectory, $"next-card-{category}.tmp");
                var nextCardId = $"{category}-{(group.Max(c => int.Parse(c.Sub_Number)) + 1):D4}";
                File.WriteAllText(nextCardFile, nextCardId);
            }

            // Write next-category.tmp
            WriteNextCategoryFile(nextCategory);
            // File.WriteAllText(Path.Combine(_runtimeDirectory, "next-category.tmp"), nextCategory);

            // Write existing-category.tmp
            var existingCategoryFilePath = Path.Combine(_runtimeDirectory, "existing-category.tmp");
            File.WriteAllLines(existingCategoryFilePath, existingCategories);
        }

        public void WriteNextCategoryFile(string nextCategory)
        {
            // Ensure runtime directory exists
            if (!Directory.Exists(_runtimeDirectory))
            {
                Directory.CreateDirectory(_runtimeDirectory);
            }

            // Write the next category to a file
            File.WriteAllText(Path.Combine(_runtimeDirectory, "next-category.tmp"), nextCategory);
        }


        /// <summary>
        /// GetNextCategory determines the next Basic Category.
        /// </summary>
        private string GetNextCategory(string lastCategory)
        {
            // Handle empty or null last category
            if (string.IsNullOrEmpty(lastCategory)) return "A"; // Default to "A" if no categories exist

            // Handle basic category transition
            if (char.IsLetter(lastCategory[0]) && lastCategory.Length == 1)
            {
                if (lastCategory[0] == 'Z')
                {
                    // Show a message when "Z" is the last category
                    MessageBox.Show("No more Basic Category available, please create Customised Category.",
                                    "Basic Category Limit Reached",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    return null; // Returning null to indicate no further categories available
                }

                // Move to the next Basic Category
                char nextChar = (char)(lastCategory[0] + 1);
                return nextChar.ToString();
            }
            // Fallback: Return null if the input doesn't match expected conditions
            return null;
        }

        public (List<string> errors, List<string> warnings) CheckCardElements(IEnumerable<Card> cards)
        {
            var errors = new List<string>();
            var warnings = new List<string>();

            foreach (var card in cards)
            {
                if (!FileExists(card.Image_Path))
                {
                    errors.Add($"Error: Missing image for {card.Card_Unic_Name}");
                }
                if (!FileExists(card.Subtitle_Path))
                {
                    warnings.Add($"Warning: Missing subtitle for {card.Card_Unic_Name}");
                }
                if (!FileExists(card.Audio_Path))
                {
                    warnings.Add($"Warning: Missing audio for {card.Card_Unic_Name}");
                }
            }

            return (errors, warnings);
        }

        public string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(_baseDirectory, "..", "..", relativePath);
        }

        public bool FileExists(string relativePath)
        {
            string fullPath = GetAbsolutePath(relativePath);
            return File.Exists(fullPath);
        }

        public string[] ReadAllLines(string relativePath)
        {
            string fullPath = GetAbsolutePath(relativePath);
            return File.ReadAllLines(fullPath);
        }
    }
}

