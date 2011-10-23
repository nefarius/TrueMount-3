using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace TrueLib
{
    public class Program
    {
        public string Executable { get; set; }
        public string ArgumentLine { get; set; }
        public bool HideWindow { get; set; }
        public TimeSpan Delay { get; set; }
        public bool Background { get; set; }

        public Program()
        {
            HideWindow = false;
            Delay = new TimeSpan(0, 0, 0);
            Background = false;
        }

        public Program(string executable) : this() { this.Executable = executable; }

        public void Launch()
        {
            Process p = new Process();
            p.StartInfo.FileName = Executable;
            p.StartInfo.Arguments = ArgumentLine;
            p.StartInfo.WindowStyle = (HideWindow) ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal;

            Thread.Sleep(Delay);
            p.Start();

            if (!Background)
                p.WaitForExit();
        }
    }
}
