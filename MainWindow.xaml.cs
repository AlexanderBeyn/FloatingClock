using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FloatingClock
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

        private void Handle_LeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Handle_RightButtonDown(Object sender, MouseButtonEventArgs e)
        {
            ContextMenu.IsOpen = true;
        }

        private void Handle_About(Object sender, RoutedEventArgs e) {
            var about = new AboutBox();
            about.ShowDialog();
        }

        private void Handle_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Handle_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (FindName("Clock") as Clock).Stop();
            Properties.Settings.Default.Save();
        }

        private void Handle_ContentRendered(object sender, EventArgs e)
        {
            (FindName("Clock") as Clock).Start();
        }
    }

}
