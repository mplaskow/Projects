using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Parascan0
{
    public partial class formHelp : Form
    {
        public formHelp()
        {
            InitializeComponent();
        }

        private void formHelp_Load(object sender, EventArgs e)
        {
            this.BackColor = ObjectsStatic.StandardColorBackground;
            pictureBoxLogo.Load(ObjectsStatic.IMAGEPATH_LOGO);
            pictureBoxLogo.Refresh();
            //textBoxLogo.BackColor = ObjectsStatic.StandardColorBackground;
            
        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxRemoteHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.STRING_HELPEXECUTABLE);
            System.Diagnostics.Process.Start(startInfo);
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }

        private void pictureBoxInstructions_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.STRING_PATHINSTRUCTIONSFILE);
            System.Diagnostics.Process.Start(startInfo);
            Application.DoEvents();
        }
    }
}
