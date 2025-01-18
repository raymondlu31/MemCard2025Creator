// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Views/ImageShaperWindow.xaml.cs
// Do Not Use ImageScaleTransform.ScaleX and ImageScaleTransform.ScaleY
// These properties belong to the ScaleTransform object.
// They are applied as a transform, which means they scale the rendered appearance of the image,
// not its actual properties like Width and Height.

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using MemCard2025Creator.Commands;
using MemCard2025Creator.Utilities;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Media3D;

namespace MemCard2025Creator.Views
{
    public partial class ImageShaperWindow : Window
    {
        private readonly string _selectedImagePath;
        private readonly string _newCardImagePath;
        private Point _lastMousePosition;
        private double InitScale;
        private bool _isDragging;
        private double _originalImageWidth;
        private double _originalImageHeight;
        private double _currentImageWidth;
        private double _currentImageHeight;

        public ImageSource ImageSource { get; private set; }

        public RelayCommand CutCommand => new RelayCommand(CutImage);

        public ImageShaperWindow(string selectedImagePath)
        {
            InitializeComponent();
            DataContext = this;

            _selectedImagePath = selectedImagePath;
            _newCardImagePath = System.IO.Path.Combine(Constants.Paths.RUNTIME_FOLDER, "CurrentSingleCardReshape.JPG");

            LoadImage();
            InitializeMaskPosition();
        }

        private void LoadImage()
        {
            Debug.WriteLine($"Selected image path: {_selectedImagePath}");
            if (!File.Exists(_selectedImagePath))
            {
                MessageBox.Show("The selected image file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedImagePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Fully load the image into memory
                bitmap.EndInit();

                // Calculate dimensions to fit within 800x800 while maintaining aspect ratio
                _originalImageWidth = bitmap.PixelWidth;
                _originalImageHeight = bitmap.PixelHeight;

                // Initial scaling to fit the image within 800px boundary
                InitScale = Math.Min(800 / _originalImageWidth, 800 / _originalImageHeight);

                // Apply initial scale to the image transform
                // _currentImageScale = InitScale;

                // Set the Zoom Slider to the initial scale
                ZoomSlider.Value = InitScale;

                // ImageScaleTransform.ScaleX = InitScale;
                // ImageScaleTransform.ScaleY = InitScale;

                

                // _currentImageWidth = _originalImageWidth;
                // _currentImageHeight = _originalImageHeight;

                _currentImageWidth = _originalImageWidth * InitScale;
                _currentImageHeight = _originalImageHeight * InitScale;

                // ZoomSlider.Value = 1.0;

                EditableImage.Width = _currentImageWidth;
                EditableImage.Height = _currentImageHeight;

                EditableImage.Source = bitmap;
                ImageSource = bitmap;

                // image always on top left of canvas
                Canvas.SetLeft(EditableImage, 25);
                Canvas.SetTop(EditableImage, 25);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void InitializeMaskPosition()
        {

            Canvas.SetLeft(MaskRectangle, 25);
            Canvas.SetTop(MaskRectangle, 5 + MaskHandleBar.Height);

            Canvas.SetLeft(MaskHandleBar, 25);
            Canvas.SetTop(MaskHandleBar, 5);
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (EditableImage != null)
            {

                // Zoom in or out based on the slider value (this applies additional zoom after initial scaling)
                double newScale = e.NewValue;

                // Apply the zoom scaling factor to the image
                // ImageScaleTransform.ScaleX = newScale;
                // ImageScaleTransform.ScaleY = newScale;

                // Optionally, update the image size
                _currentImageWidth = _originalImageWidth * newScale;
                _currentImageHeight = _originalImageHeight * newScale;
                EditableImage.Width = _currentImageWidth;
                EditableImage.Height = _currentImageHeight;
            }

        }


        private void MaskHandle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _lastMousePosition = e.GetPosition(MainCanvas);
            ((UIElement)sender).CaptureMouse();
        }

        private void MaskHandle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private void MaskHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            Point currentPosition = e.GetPosition(MainCanvas);
            double deltaX = currentPosition.X - _lastMousePosition.X;
            double deltaY = currentPosition.Y - _lastMousePosition.Y;

            // Get current position of the mask
            double currentX = Canvas.GetLeft(MaskRectangle);
            double currentY = Canvas.GetTop(MaskRectangle);

            // Update mask position
            Canvas.SetLeft(MaskRectangle, currentX + deltaX);
            Canvas.SetTop(MaskRectangle, currentY + deltaY);

            // Ensure the image remains at the top-left corner
            Canvas.SetLeft(EditableImage, 25);
            Canvas.SetTop(EditableImage, 25);

            // Update handle bar position to match the mask
            Canvas.SetLeft(MaskHandleBar, currentX + deltaX);
            Canvas.SetTop(MaskHandleBar, currentY + deltaY - MaskHandleBar.Height);

            _lastMousePosition = currentPosition;
        }

        private void CutImage(object parameter)
        {
            try
            {
                // Get the position and size of the mask
                double maskX = Canvas.GetLeft(MaskRectangle);
                double maskY = Canvas.GetTop(MaskRectangle);
                double maskWidth = MaskRectangle.Width;
                double maskHeight = MaskRectangle.Height;

                // Calculate the cropping rectangle, clamping values within the image bounds
                double imageLeft = Canvas.GetLeft(EditableImage);
                double imageTop = Canvas.GetTop(EditableImage);
                double imageWidth = EditableImage.ActualWidth;
                double imageHeight = EditableImage.ActualHeight;

                int cropX = (int)Math.Max(0, maskX - imageLeft);
                int cropY = (int)Math.Max(0, maskY - imageTop);
                int cropWidth = (int)Math.Min(maskWidth, imageWidth - cropX);
                int cropHeight = (int)Math.Min(maskHeight, imageHeight - cropY);

                // Ensure crop dimensions are valid
                if (cropWidth <= 0 || cropHeight <= 0)
                {
                    throw new InvalidOperationException("The mask area is invalid or outside the image bounds.");
                }

                // Create a new RenderTargetBitmap of the mask size
                var renderTarget = new RenderTargetBitmap(
                    (int)imageWidth, // Use the full image width
                    (int)imageHeight, // Use the full image height
                    96, 96,
                    PixelFormats.Pbgra32);

                // Draw the image onto the RenderTargetBitmap
                var drawingVisual = new DrawingVisual();
                using (var context = drawingVisual.RenderOpen())
                {
                    context.DrawImage(EditableImage.Source, new Rect(0, 0, imageWidth, imageHeight));
                }

                renderTarget.Render(drawingVisual);

                // Create the cropped bitmap
                var cropRect = new Int32Rect(cropX, cropY, cropWidth, cropHeight);
                var croppedBitmap = new CroppedBitmap(renderTarget, cropRect);

                // Save the cropped image
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));

                using (var fileStream = new FileStream(_newCardImagePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                MessageBox.Show($"Image saved to {_newCardImagePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
