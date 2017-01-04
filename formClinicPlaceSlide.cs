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
    public partial class formClinicPlaceSlide : Form
    {
        public formClinicPlaceSlide()
        {
            InitializeComponent();
            this.BackColor = ObjectsStatic.StandardColorBackground;
            
        }

        private void pictureBoxCancel_Click(object sender, EventArgs e)
        {
            ObjectsStatic.IsCaseReady = false;
            this.Close();
        }

        private void pictureBoxScan_Click(object sender, EventArgs e)
        {
            ObjectsStatic.IsCaseReady = true;
            this.Close();
        }
    }
}
