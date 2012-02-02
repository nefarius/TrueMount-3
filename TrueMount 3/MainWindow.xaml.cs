using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrueMount_3.Windows;

namespace TrueMount_3
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

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuSettings_Click(object sender, RoutedEventArgs e)
        {
            WelcomeWizard ww = new WelcomeWizard();
            ww.ShowDialog();
        }
    }
}
