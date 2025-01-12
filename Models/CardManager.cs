// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Models/CardManager.cs

using MemCard2025Creator.Utilities;
using MemCard2025Creator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace MemCard2025Creator.Models
{
    public class CardManager
    {
        private readonly FileService _fileService;

        // Store the current list of Card objects
        public List<Card> CurrentCardList { get; private set; } = new List<Card>();

        public CardManager()
        {
            _fileService = new FileService();
        }

        public List<Card> UpdateCardList()
        {
            CurrentCardList = _fileService.Load_CurrentCardList_FromRuntimeFiles();
            return CurrentCardList;
        }

        /// <summary>
        /// Load the default sequence of cards from the card list file.
        /// </summary>
        public void LoadDefaultSequence()
        {
            if (File.Exists(Constants.Paths.CARD_LIST_FILE))
            {
                var cardLines = _fileService.ReadAllLines(Constants.Paths.CARD_LIST_FILE);

                CurrentCardList = cardLines
                    .Select(Card.ParseCardFromCardUnicName) // Convert each line into a Card object
                    .Where(card => card != null) // Filter out any parsing errors
                    .ToList();

                _fileService.Write_CurrentCardList_ToRuntimeFiles(CurrentCardList); // Save to runtime files

                ShowCurrentCardList(); // Optional: For debugging
            }
            else
            {
                MessageBox.Show("Error: card-list.txt not found.", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Reads and shuffles each category's cards, updating CurrentCardList.
        /// </summary>
        public void ShuffleCards()
        {
            var shuffledList = new List<Card>();

            // Read each category file in runtime folder, shuffle, and add to shuffled list
            foreach (var filePath in Directory.GetFiles(Constants.Paths.RUNTIME_FOLDER, "current-category-*.tmp"))
            {
                var cardLines = File.ReadAllLines(filePath).ToList();

                // Convert each line to a Card object
                var cards = cardLines
                    .Select(line => Card.ParseCardFromCardUnicName(line))
                    .Where(card => card != null) // Exclude invalid cards
                    .ToList();

                // Shuffle the cards
                var random = new Random();
                cards = cards.OrderBy(_ => random.Next()).ToList();
                // cardLines = cardLines.OrderBy(x => random.Next()).ToList();

                // Add shuffled lines to the final list
                shuffledList.AddRange(cards);
            }

            // Update the current card list with the shuffled result
            CurrentCardList = shuffledList;

            _fileService.Write_CurrentCardList_ToRuntimeFiles(CurrentCardList); // Save to runtime files

            // Show the current shuffled list in a popup for verification
            ShowCurrentCardList();
        }



        // Get the image path for a given card
        public string GetCard_ImagePath(Card card)
        {
            return card.Image_Path;
        }

        // Get the audio path for a given card
        public string GetCard_AudioPath(Card card)
        {
            return card.Audio_Path;
        }

        // Get the subtitle path for a given card
        public string GetCard_SubtitlePath(Card card)
        {
            return card.Subtitle_Path;
        }

        /// <summary>
        /// Displays the current card list in a popup window (for debugging).
        /// </summary>
        private void ShowCurrentCardList()
        {
            string cardListString = string.Join("\n", CurrentCardList.Select(card => card.Card_Unic_Name));
            MessageBox.Show(cardListString, "---- Current Card List ----");
        }

        

    }
}



