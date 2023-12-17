using CefSharp.Wpf;
using CefSharp;
using legallead.desktop.implementations;
using legallead.desktop.utilities;
using System.Text;
using System;
using System.Windows;

namespace legallead.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentHandler.LoadLocal("introduction", content1);
        }
    }
}