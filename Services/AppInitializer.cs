// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Services/AppInitializer.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using MemCard2025Creator.Models;
using MemCard2025Creator.Utilities;

namespace MemCard2025Creator.Services
{
    public class AppInitializer
    {
        // private readonly string _baseDirectory;
        // private readonly string _runtimeDirectory;
        private readonly string _cardListPath;
        private readonly FileService _fileService;

        public AppInitializer()
        {
            // _baseDirectory = Constants.Paths.BASE_DIRECTORY;
            // _runtimeDirectory = Constants.Paths.RUNTIME_FOLDER;
            _cardListPath = Constants.Paths.CARD_LIST_FILE;
            _fileService = new FileService(); // Initialize FileService
        }

        public void InitialLoad()
        {
            // 1. Read card-list.txt
            var cardLines = _fileService.ReadAllLines(_cardListPath);
            // var cards = Card.ParseCardFromCardUnicName(cardLines);

            // Process each line and filter out null results
            var cards = cardLines.Select(line => Card.ParseCardFromCardUnicName(line))
                               .Where(card => card != null)
                               .ToList();

            // 2. Clear runtime directory
            _fileService.ClearRuntimeDirectory();

            // 3. Write runtime files based on card-list.txt
            _fileService.WriteInitRuntimeFiles(cards);

            // 4. Scan media directory to check card elements exist
            var (errors, warnings) = _fileService.CheckCardElements(cards);

            // 5. Show consistency summary
            ShowConsistencySummary(errors, warnings);
        }


        private void ShowConsistencySummary(List<string> errors, List<string> warnings)
        {
            string message;

            if (errors.Any())
            {
                // Show prefix for errors
                message = "Please fix errors:\n" + string.Join("\n", errors) + "\n" + string.Join("\n", warnings);
            }
            else if (warnings.Any())
            {
                // Show prefix for warnings only
                message = "Please note:\n" + string.Join("\n", warnings);
            }
            else
            {
                // No errors or warnings
                message = "Consistency check passed.";
            }

            // Display the message to the user
            MessageBox.Show(message, "Consistency Summary", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}

