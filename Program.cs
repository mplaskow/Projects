using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ImagingController;
using System.IO.Ports;

namespace Parascan0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            

            int waitTime = Properties.Settings.Default.INTEGER_SPLASHSCREENTIME;
            Application.DoEvents();


            if (Properties.Settings.Default.BOOLEAN_ISLIVESCAN == true)
            {
                formSplashScreen objectSplashScreen = new formSplashScreen();
                objectSplashScreen.Show();
                Application.DoEvents();
                System.Threading.Thread.Sleep(waitTime);
                Application.DoEvents();
                objectSplashScreen.Hide();
            }

            try
            {
                SerialPort port = new SerialPort(Properties.Settings.Default.STRING_COMPORT, 9600);
                while (port.IsOpen == false)
                {
                    port.Open();
                    System.Threading.Thread.Sleep(500);
                }
                port.WriteLine("/1aM1L500V40000D500R\n\r");
                System.Threading.Thread.Sleep(500);
                port.WriteLine("/1aM1L500V40000P500R\n\r");
                System.Threading.Thread.Sleep(500);
                port.WriteLine("s2R\n\r");
                System.Threading.Thread.Sleep(500);
                port.Close();
                port = null;
            }
            catch { }

            
            if (Properties.Settings.Default.BOOLEAN_ISLIVESCAN == true)
            {
                Application.Run(new formInterface());
            }
            else if ((Properties.Settings.Default.BOOLEAN_ISTRAINING == true) && (System.Windows.Forms.MessageBox.Show("Test Run?", "", MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                Application.Run(new MainForm());
            }
            else
            {
                formSplashScreen objectSplashScreen = new formSplashScreen();
                objectSplashScreen.Show();
                Application.DoEvents();
                System.Threading.Thread.Sleep(waitTime);
                Application.DoEvents();
                objectSplashScreen.Hide();
                Application.Run(new formInterface());
            }
            
        }
    }
}