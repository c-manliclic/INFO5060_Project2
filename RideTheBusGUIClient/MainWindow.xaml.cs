using System;
using System.Windows;
using System.ServiceModel;
using RideTheBusLibrary;


namespace RideTheBusGUIClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Black_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_RuleBook_Click(object sender, RoutedEventArgs e)
        {
            new RuleBook().ShowDialog();
        }
    }
}
