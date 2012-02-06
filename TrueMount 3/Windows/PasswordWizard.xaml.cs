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
    }
}
