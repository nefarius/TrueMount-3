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
using TrueLib;

namespace TrueMount_3.Windows
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<EncryptedContainerFile> list = new List<EncryptedContainerFile>();
            EncryptedContainerFile ecf = new EncryptedContainerFile("C:\\lol.txt");
            list.Add(ecf);
            dataGridVolumes.ItemsSource = list;
        }
    }
}
