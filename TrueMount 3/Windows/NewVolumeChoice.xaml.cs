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
    /// Interaction logic for NewVolumeChoice.xaml
    /// </summary>
    public partial class NewVolumeChoice : Window
    {
        public NewVolumeChoice()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonNewContainer_Click(object sender, RoutedEventArgs e)
        {
            new ContainerWizard(new EncryptedContainerFile(null)).Show();
            this.Close();
        }
    }
}
