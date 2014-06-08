using System;
using System.Windows;
using AvalonWizard;
using Microsoft.Win32;
using TrueLib;
using System.IO;
using TrueLib.Remote;

namespace TrueMount_3.Windows
{
    /// <summary>
    /// Interaction logic for PasswordWizard.xaml
    /// </summary>
    public partial class PasswordWizard : Window
    {
        private KeyItem password = null;
        private string Protocol = string.Empty;

        public PasswordWizard(KeyItem password)
        {
            this.password = password;

            InitializeComponent();

            wizardPassword.DataContext = password;
        }

        private void buttonLocal_Click(object sender, RoutedEventArgs e)
        {
            wizardPassword.NextPage(wpLocal);
        }

        private void buttonRemote_Click(object sender, RoutedEventArgs e)
        {
            wizardPassword.NextPage(wpRemote);
        }

        private void wizardPassword_Finished(object sender, RoutedEventArgs e)
        {
            Wizard current = sender as Wizard;

            if (current.CurrentPage == wpLocal)
            {
                LogicalDisk pwDevice = new LogicalDisk();
                pwDevice.Denote(Path.GetPathRoot(textBoxLocalPath.Text));
            }

            if (current.CurrentPage == wpRemote)
            {
                switch (comboBoxProtocol.Text.ToLower())
                {
                    case "cifs":
                        break;
                    case "ftp":
                        break;
                    case "ftpes":
                        break;
                    case "http":
                        break;
                    case "https":
                        break;
                    case "sftp":
                        break;
                    case "webdav":
                        break;
                    default:
                        break;
                }
            }

            this.Close();
        }

        private void wizardPassword_Cancelled(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonOpenLocalFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog pwfile = new OpenFileDialog();

            if (pwfile.ShowDialog() == (Nullable<bool>)true)
            {
                textBoxLocalPath.Text = pwfile.FileName;
            }
        }
    }
}
