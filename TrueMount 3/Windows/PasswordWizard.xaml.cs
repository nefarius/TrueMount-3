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
using System.Threading;

namespace TrueMount_3.Windows
{
    /// <summary>
    /// Interaction logic for PasswordWizard.xaml
    /// </summary>
    public partial class PasswordWizard : Window
    {
        private Password password = null;

        public PasswordWizard(Password password)
        {
            this.password = password;

            InitializeComponent();
        }

        private void buttonLocal_Click(object sender, RoutedEventArgs e)
        {
            wizardPassword.NextPage(wpLocal);
        }

        private void buttonRemote_Click(object sender, RoutedEventArgs e)
        {
            wizardPassword.NextPage(wpRemote);
        }
    }
}
