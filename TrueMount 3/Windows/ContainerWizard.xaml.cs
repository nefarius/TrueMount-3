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
using Microsoft.Win32;

namespace TrueMount_3.Windows
{
    /// <summary>
    /// Interaction logic for ContainerWizard.xaml
    /// </summary>
    public partial class ContainerWizard : Window
    {
        private EncryptedContainerFile container = null;

        public ContainerWizard(EncryptedContainerFile file)
        {
            InitializeComponent();

            container = file;

            textBoxFileName.DataContext = container;
            textBoxLabel.DataContext = container;
            textBoxTmp.DataContext = container;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog search = new OpenFileDialog();

            if (search.ShowDialog() == (Nullable<bool>)true)
            {
                textBoxFileName.Text = search.FileName;
            }
        }
    }
}
