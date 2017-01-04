using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parascan1.Data;

namespace Parascan0
{
    public partial class formImageViewer : Form
    {
        private string filePath;

        public formImageViewer(string FILEPATH)
        {
            InitializeComponent();
            filePath = FILEPATH;
            pictureBoxViewer.Width = (int)(SharedValueController.IMAGE_RESULT_SIZE * 1.5);
            pictureBoxViewer.Height = pictureBoxViewer.Width;
            pictureBoxViewer.Left = (this.Width / 2) - (pictureBoxViewer.Width / 2);
        }

        private void formImageViewer_Load(object sender, EventArgs e)
        {
            this.BackColor = ObjectsStatic.StandardColorBackground;
            this.Activate();
            pictureBoxViewer.Load(filePath);
            pictureBoxViewer.Refresh();
        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
