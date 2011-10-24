using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dolinay;
using System.IO;
using TrueLib;
using System.Globalization;

namespace TrueTest
{
    public partial class TestApp : Form
    {
        public TestApp()
        {
            InitializeComponent();
        }

        private void TestApp_Load(object sender, EventArgs e)
        {
            DriveDetector dd = new DriveDetector();
            dd.DeviceArrived += new DriveDetectorEventHandler(dd_DeviceArrived);

            Configuration cfg = Configuration.Local;

            //logBox.AppendText(cfg.FirstStart.ToString());

            if (cfg.FirstStart)
                cfg.FirstStart = !cfg.FirstStart;

            cfg.Language = new CultureInfo("en-US");
            Configuration.Local = cfg;

            Password pw1 = new Password("static://localhost/mypw()lul6&");
            Console.WriteLine(pw1.CachedFileName);
        }

        void dd_DeviceArrived(object sender, DriveDetectorEventArgs e)
        {
            logBox.AppendText(e.Drive + Environment.NewLine);

            TriggerDevice td = new TriggerDevice();
            if (td.IsOnline)
                MessageBox.Show("Online");

            string file = string.Format("{0}tm3identity", e.Drive);
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(System.Guid.NewGuid().ToString());
            }
            File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.Hidden | FileAttributes.System);
        }
    }
}
