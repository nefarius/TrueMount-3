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
using System.ComponentModel;

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

#if DEBUG
            container.FileName = "D:\\test.txt";
            container.Label = "Secret porn collection";
            container.MountOptions.Removable = true;
#endif

            wizardContainerFile.DataContext = container;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog search = new OpenFileDialog();

            if (search.ShowDialog() == (Nullable<bool>)true)
            {
                container.FileName = search.FileName;
                BindingOperations.GetBindingExpressionBase(textBoxFileName, 
                    TextBox.TextProperty).UpdateTarget(); 
            }
        }

        private void wizardContainerFile_Finished(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
