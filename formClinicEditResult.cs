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
    public partial class formClinicEditResult : Form
    {
        List<string> FILE_PATHS;
        string RESULT_MESSAGE = String.Empty;
        //int RESULT_SIZE = (int)(SharedValueController.IMAGE_RESULT_SIZE * .667);
        int RESULT_SIZE = 200;

        public formClinicEditResult(List<string> filePaths, string resultMessage)
        {
            InitializeComponent();
            FILE_PATHS = filePaths;
            RESULT_MESSAGE = resultMessage;
        }

        private void formClinicEditResult_Load(object sender, EventArgs e)
        {
            this.BackColor = ObjectsStatic.StandardColorBackground;
            panelImages.BackColor = ObjectsStatic.StandardColorBackground;
            textBoxMessage.BackColor = ObjectsStatic.StandardColorBackground;
            int imageRowLimit = (int)(panelImages.Width / RESULT_SIZE);
            int imageWidth = (int)(RESULT_SIZE);
            int imageHeight = (int)(RESULT_SIZE);
            textBoxMessage.Text = RESULT_MESSAGE;
            textBoxMessage.Left = panelImages.Left + (int)(panelImages.Width / 2) - (int)(textBoxMessage.Width / 2);
            int imageIndex = 0;
            string subfolderProcessed = SharedValueController.SUBFOLDER_PROCESSED.ToUpper();
            string subfolderRawImages = SharedValueController.SUBFOLDER_IMAGES.ToUpper();

            foreach (string FILE_PATH in FILE_PATHS)
            {
                try
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.Load(FILE_PATH);
                    pictureBox.Width = imageWidth;
                    pictureBox.Height = imageHeight;
                    pictureBox.Tag = FILE_PATH;
                    pictureBox.Click += new EventHandler(PictureBoxClick);
                    pictureBox.Refresh();
                    if (imageIndex == imageRowLimit)
                    {
                        pictureBox.Top = imageHeight;
                        imageIndex = 0;
                    }
                    int pictureBoxLeft = imageIndex * imageWidth;
                    pictureBox.Left = pictureBoxLeft;
                    panelImages.Controls.Add(pictureBox);
                    imageIndex++;
                }
                catch { }
            }
            panelImages.Refresh();

            //textBoxMessage.Top = pictureBoxResult.Top + (int)(pictureBoxResult.Height / 2) - (int)(textBoxMessage.Height / 2);
            pictureBoxYes.Focus();
        }

        private void pictureBoxYes_Click(object sender, EventArgs e)
        {
            ObjectsStatic.PARASITE_RESULT = FECALPARASITE_RESULT.Yes;
            this.Close();
        }

        private void pictureBoxNo_Click(object sender, EventArgs e)
        {
            ObjectsStatic.PARASITE_RESULT = FECALPARASITE_RESULT.No;
            this.Close();
        }

        private void pictureBoxEscalate_Click(object sender, EventArgs e)
        {
            ObjectsStatic.PARASITE_RESULT = FECALPARASITE_RESULT.SecondOpinion;
            this.Close();
        }

        private void pictureBoxOther_Click(object sender, EventArgs e)
        {
            new formClinicSelectResult().ShowDialog();
            ObjectsStatic.PARASITE_RESULT = FECALPARASITE_RESULT.Other;
            this.Close();
        }

        private void PictureBoxClick(object sender, EventArgs e)
        {

            PictureBox pictureBoxClick = null;
            if (sender is PictureBox)
            {
                pictureBoxClick = sender as PictureBox;
                formImageViewer formImageViewer = new formImageViewer(Convert.ToString(pictureBoxClick.Tag));
                formImageViewer.ShowDialog();
                Application.DoEvents();
            }

        }

       
    }
}
