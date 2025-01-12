// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Views/DeleteCategoryView.xaml.cs

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
using MemCard2025Creator.ViewModels;

namespace MemCard2025Creator.Views
{
    /// <summary>
    /// Interaction logic for DeleteCategoryView.xaml
    /// </summary>
    public partial class DeleteCategoryView : UserControl
    {
        public DeleteCategoryView()
        {
            InitializeComponent();
            // Set DataContext to ViewModel instance
            DataContext = new DeleteCategoryViewModel();
        }
    }
}
