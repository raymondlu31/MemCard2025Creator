// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Models/Card.cs

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using MemCard2025Creator.Utilities;

namespace MemCard2025Creator.Models
{
    public class Card
    {

        // Category validation: Basic (A-Z), Reserved (0-9), or Custom (alphanumeric).
        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                if (IsValidCategory(value))
                {
                    _category = value;
                }
                else
                {
                    throw new ArgumentException("Invalid Category. Must be a single uppercase letter, a single digit, or an alphanumeric string.");
                }
            }
        }

        // 4-digit number as string in a Category from 0001 to 9999
        public string Sub_Number { get; set; } 
        // a string helps human recognize the card
        public string Card_Alias { get; set; } 

        // Id = Category + Sub_Number
        public string Card_Id => $"{Category}-{Sub_Number}";

        // 1 Card_Unic_Name = Card_Id + Card_Alias, 2 if Card_Alias is empty, Card_Unic_Name = Card_Id
        public string Card_Unic_Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Card_Alias))
                {
                    return Card_Id;
                }
                else
                {
                    return $"{Card_Id}-{Card_Alias}";
                }
            }
        }

        // ImagePath = MemCard2024Nov.Utilities.Constants.Paths.Image_Directory + Card_Unic_Name + ".JPG"
        public string Image_Path => $"{Constants.Paths.Image_Directory}{Card_Unic_Name}.JPG";
        public string Subtitle_Path => $"{Constants.Paths.Subtitle_Directory}{Card_Unic_Name}.txt";
        public string Audio_Path => $"{Constants.Paths.Audio_Directory}{Card_Unic_Name}.mp3";

        /// <summary>
        /// Validates the category.
        /// </summary>
        private static bool IsValidCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return false;

            // Basic Category: Single capital letter A-Z
            if (Regex.IsMatch(category, @"^[A-Z]$")) return true;

            // Reserved Category: Single digit 0-9
            if (Regex.IsMatch(category, @"^\d$")) return true;

            // Customized Category: Alphanumeric string (letters or numbers)
            if (Regex.IsMatch(category, @"^[A-Za-z0-9]+$")) return true;

            return false;
        }

        /// <summary>
        /// Creates a dummy card for DisplayMode initialization.
        /// </summary>
        public static Card DisplayModeInitCard()
        {
            return new Card
            {
                Category = "1",
                Sub_Number = "0001",
                Card_Alias = "DisplayInit"
            };
        }

        /// <summary>
        /// Creates a dummy card for ChallengeMode initialization.
        /// </summary>
        public static Card ChallengeModeInitCard()
        {
            return new Card
            {
                Category = "2",
                Sub_Number = "0001",
                Card_Alias = "ChallengeInit"
            };
        }

        /// <summary>
        /// Parse a Card object from a Card_Unic_Name string.
        /// </summary>
        public static Card ParseCardFromCardUnicName(string cardUnicName)
        {
            try
            {
                // Split the Card_Unic_Name using '-' as the delimiter
                var parts = cardUnicName.Split('-');
                if (parts.Length < 2)
                    throw new FormatException("Invalid Card_Unic_Name format.");

                // Validate Category
                var category = parts[0];
                if (!IsValidCategory(category))
                    throw new FormatException("Invalid Category format.");

                // Validate Sub_Number (4 digits)
                var subNumber = parts[1];
                if (!Regex.IsMatch(subNumber, @"^\d{4}$"))
                    throw new FormatException("Sub_Number must be a 4-digit number.");

                // Join the remaining parts as Card_Alias, if available
                var alias = parts.Length > 2 ? string.Join("-", parts.Skip(2)) : string.Empty;

                // Return a new Card object
                return new Card
                {
                    Category = category,
                    Sub_Number = subNumber,
                    Card_Alias = alias
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error parsing Card_Unic_Name '{cardUnicName}': {ex.Message}",
                    "Parse Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

    }
}