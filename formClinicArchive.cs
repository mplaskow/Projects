using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parascan0
{
    public partial class formClinicArchive : Form
    {
        public formClinicArchive()
        {
            InitializeComponent();
            this.BackColor = ObjectsStatic.StandardColorBackground;
            try
            {
                pictureBoxOrganizationLogo.Image = Image.FromFile(Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHIMAGES + "\\" + Properties.Settings.Default.STRING_ORGANIZATIONIMAGE);
            }
            catch { }
            pictureBoxLogo.Load(ObjectsStatic.IMAGEPATH_LOGO);
            pictureBoxLogo.Refresh();
            dateTimePickerStart.Value = System.DateTime.Now.AddMonths(-2);
        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
