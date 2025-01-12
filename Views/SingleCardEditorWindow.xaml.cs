// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Views/SingleCardEditorWindow.xaml.cs

using MemCard2025Creator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MemCard2025Creator.Views
{
    /// <summary>
    /// Interaction logic for SingleCardEditorWindow.xaml
    /// </summary>
    public partial class SingleCardEditorWindow : Window
    {
        public SingleCardEditorWindow(SingleCardEditorViewModel viewModel)
        {
            InitializeComponent();
            // Set DataContext to ViewModel passed in the constructor
            DataContext = viewModel;
        }
    }
}
