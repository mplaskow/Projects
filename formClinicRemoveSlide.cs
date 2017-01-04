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
    public partial class formClinicRemoveSlide : Form
    {
        public formClinicRemoveSlide()
        {
            InitializeComponent();
        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void formClinicRemoveSlide_Load(object sender, EventArgs e)
        {
            this.Activate();
            this.Focus();
        }
    }
}
