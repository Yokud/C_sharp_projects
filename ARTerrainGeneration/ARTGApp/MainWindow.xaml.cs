﻿using System;
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
using HeightMapLib;

namespace ARTGApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TerrainModel model = new TerrainModel(1000, 1000);

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            MainFrame.Children.Add(model);
        }
    }
}
