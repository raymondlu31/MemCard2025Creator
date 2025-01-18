// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Utilities/MessagesHelper.cs

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MemCard2025Creator.Utilities
{
    public static class MessageHelper
    {
        /// <summary>
        /// Displays a large message in a custom pop-up window with an "OK" button.
        /// </summary>
        /// <param name="message">The message content to display.</param>
        /// <param name="fontSize">The font size of the message text.</param>
        /// <returns>A MessageBoxResult indicating whether "OK" was clicked.</returns>
        public static MessageBoxResult ChallengeResultWindow(string message, double fontSize = 16)
        {
            Window window = new Window
            {
                Title = "Challenge Completed",
                Width = 500,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = Brushes.White,
                ResizeMode = ResizeMode.NoResize
            };

            // Create main StackPanel
            StackPanel mainStackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            // Create ScrollViewer for the message
            ScrollViewer scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                MaxHeight = 350, // Leave space for the OK button
                Margin = new Thickness(10)
            };

            // Create TextBlock for the message
            TextBlock textBlock = new TextBlock
            {
                Text = message,
                FontSize = fontSize,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10)
            };

            // Create OK Button
            Button okButton = new Button
            {
                Content = "OK",
                Width = 100,
                Height = 40,
                Margin = new Thickness(20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add click handler to close the window and set result
            okButton.Click += (sender, e) =>
            {
                window.DialogResult = true;
                window.Close();
            };

            // Assemble the controls
            scrollViewer.Content = textBlock;
            mainStackPanel.Children.Add(scrollViewer);
            mainStackPanel.Children.Add(okButton);

            // Set the window's content
            window.Content = mainStackPanel;

            // Show dialog and return appropriate result
            bool? dialogResult = window.ShowDialog();
            return dialogResult == true ? MessageBoxResult.OK : MessageBoxResult.None;
        }
    }
}