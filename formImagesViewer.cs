using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Parascan1.Data;

namespace Parascan0
{
    public partial class formImagesViewer : Form
    {
        private int RESULT_VALUE;
        private string FOLDER_PATH;

        public formImagesViewer(string folderPath, int resultValue)
        {
            InitializeComponent();
            this.BackColor = ObjectsStatic.StandardColorBackground;
            textBoxMessage.BackColor = ObjectsStatic.StandardColorBackground;
            RESULT_VALUE = resultValue;
            FOLDER_PATH = folderPath;
            textBoxMessage.Text = StringEnum.GetStringValue((FECALPARASITE_TYPE)resultValue) + " Images";
            PopulateImages();
        }

        private void PopulateImages()
        {
            const int IMAGE_COLUMN_MAXIMUM = 7;
            const int IMAGE_ROW_MAXIMUM = 2;
            string resultString = String.Empty;
            int imageWidth = (int)SharedValueController.IMAGE_RESULT_SIZE / 3;
            int imageHeight = (int)SharedValueController.IMAGE_RESULT_SIZE / 3;
            int pixelTop = 0;
            int pixelLeft = 0;
            int imageColumns = 0;
            panelImages.Controls.Clear();
            panelImages.Height = IMAGE_ROW_MAXIMUM * imageHeight;
            panelImages.Width = IMAGE_COLUMN_MAXIMUM * imageWidth;
            this.Width = panelImages.Width + 10;
            panelImages.Left = 0;
            panelImages.Top = textBoxMessage.Height + 10;

            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();

            string stringSQL = "Select Top " + Convert.ToString(IMAGE_COLUMN_MAXIMUM * 2) + " FileName From AnalysisRequestsResults Where CaseID = '" + ObjectsStatic.CASEID + "' and RequestID = " + ObjectsStatic.REQUESTID + " and ResultValue = " + Convert.ToString(RESULT_VALUE) + " Order By ConfidenceValue Desc";
            SqlDataReader sqlDataReader = dataObject.GetDataReaderFromQuery(stringSQL);
            while (sqlDataReader.Read())
            {
                if (pixelLeft > (panelImages.Width - imageWidth))
                {
                    pixelTop = pixelTop + imageHeight;
                    pixelLeft = 0;
                }
                PictureBox objectPictureBox = new PictureBox();
                objectPictureBox.Image = Image.FromFile(FOLDER_PATH + Convert.ToString(sqlDataReader[0]));
                objectPictureBox.Height = imageHeight;
                objectPictureBox.Width = imageWidth;
                objectPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                objectPictureBox.Left = pixelLeft;
                objectPictureBox.Top = pixelTop;
                objectPictureBox.Name = Convert.ToString(sqlDataReader[0]);
                objectPictureBox.Tag = Convert.ToString(sqlDataReader[0]);
                objectPictureBox.Click += new EventHandler(PictureBoxClick);
                panelImages.Controls.Add(objectPictureBox);
                pixelLeft = pixelLeft + objectPictureBox.Width;
                objectPictureBox.Visible = true;
                imageColumns++;

            }
            sqlDataReader.Close();

            dataObject.ObjectDispose();


        }


        private void PictureBoxClick(object sender, EventArgs e)
        {

            PictureBox pictureBoxClick = null;
            if (sender is PictureBox)
            {
                pictureBoxClick = sender as PictureBox;
                formImageViewer formImageViewer = new formImageViewer(FOLDER_PATH + pictureBoxClick.Tag);
                formImageViewer.ShowDialog();
                Application.DoEvents();
            }

        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
