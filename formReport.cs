using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using Parascan1.Data;

namespace Parascan0
{
    public partial class formReport : Form
    {
        bool IsNotesChanged = false;
        private System.IO.Stream streamToPrint;
        string streamType;
        string FOLDER_PATH = Properties.Settings.Default.STRING_PATHCASES + "\\" + ObjectsStatic.CASEID + "\\" + SharedValueController.SUBFOLDER_PROCESSED + "\\";
        int imageWidth = SharedValueController.IMAGE_RESULT_SIZE;
        int imageHeight = SharedValueController.IMAGE_RESULT_SIZE;

        public formReport()
        {
            InitializeComponent();
         }

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool BitBlt
        (
            IntPtr hdcDest, // handle to destination DC
            int nXDest, // x-coord of destination upper-left corner
            int nYDest, // y-coord of destination upper-left corner
            int nWidth, // width of destination rectangle
            int nHeight, // height of destination rectangle
            IntPtr hdcSrc, // handle to source DC
            int nXSrc, // x-coordinate of source upper-left corner
            int nYSrc, // y-coordinate of source upper-left corner
            System.Int32 dwRop // raster operation code
        );

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (IsNotesChanged == true)
            {
                if (System.Windows.Forms.MessageBox.Show("Save Hospital Notes?", "Save Notes", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    UpdateClinicNotes();
                }
            }
            UpdateCaseCompleted();
            this.Close();

        }

        private void formReport_Load(object sender, EventArgs e)
        {
            //Set Back Color for Form
            labelReport.BackColor = ObjectsStatic.StandardColorBackground;
            //panelImages.Width = imageWidth * 4;
            //panelImages.Height = imageHeight * 3;
            PopulateReport();
        }

        #region "*** Private Methods to Populate Values ***"

        private void PopulateReport()
        {
            //*** Populate Organization Information from Database ***
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "SELECT OrganizationID, OrganizationName, OrganizationContact, OrganizationAddress1, OrganizationCity, OrganizationState, OrganizationZipCode, OrganizationEMail, OrganizationPhone " +
                "FROM AnalysisOrganizations Where OrganizationID = '" + Properties.Settings.Default.STRING_ORGANIZATIONID + "'";
            SqlDataReader dataReader = dataObject.GetDataReaderFromQuery(stringSQL);
            if (dataReader.Read() == true)
            {
                textBoxOrganizationName.Text = Convert.ToString(dataReader["OrganizationName"]);
                labelOrganizationContact.Text = Convert.ToString(dataReader["OrganizationContact"]);
                labelOrganizationAddress.Text = Convert.ToString(dataReader["OrganizationAddress1"]);
                labelOrganizationCity.Text = Convert.ToString(dataReader["OrganizationCity"]) + ", " + Convert.ToString(dataReader["OrganizationState"]) + " " + Convert.ToString(dataReader["OrganizationZipCode"]);
                labelOrganizationEMail.Text = Convert.ToString(dataReader["OrganizationEMail"]);
                labelOrganizationPhone.Text = Convert.ToString(dataReader["OrganizationPhone"]);
            }
            dataReader.Close();
            pictureBoxOrganizationLogo.Image = Image.FromFile(Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHIMAGES + "\\" + Properties.Settings.Default.STRING_ORGANIZATIONIMAGE);
            pictureBoxOrganizationLogo.Refresh();
            //*** Populate Case and Specimen Information from Database ***
            stringSQL = "SELECT AnalysisCases.CaseID, AnalysisCases.CaseFirst, AnalysisCases.CaseLast, AnalysisCases.CaseNickname, AnalysisCases.CaseDate, AnalysisCases.CaseGender, AnalysisCases.CaseSpecies, " +
            "AnalysisCases.CaseBreed, AnalysisRequests.RequestNotes, AnalysisRequests.SpecimenNumber, AnalysisRequests.RequestClinicNotes " +
            "FROM AnalysisCases INNER JOIN AnalysisRequests ON AnalysisCases.CaseID = AnalysisRequests.CaseID AND AnalysisCases.OrganizationID = AnalysisRequests.OrganizationID " +
            "WHERE AnalysisCases.CaseID = '" + ObjectsStatic.CASEID + "' and AnalysisRequests.SpecimenNumber = '" + ObjectsStatic.SPECIMENNUMBER + "'";
            dataReader = dataObject.GetDataReaderFromQuery(stringSQL);
            if (dataReader.Read() == true)
            {
                textBoxCaseID.Text = Convert.ToString(dataReader["CaseID"]);
                labelReport.Text = "Veterinary Parasite Report for " + Convert.ToString(dataReader["CaseNickname"]) + " " + Convert.ToString(dataReader["CaseLast"]);
                textBoxSpecimenNumber.Text = Convert.ToString(dataReader["SpecimenNumber"]);
                textBoxCaseOwner.Text = Convert.ToString(dataReader["CaseFirst"]) + " " + Convert.ToString(dataReader["CaseLast"]);
                textBoxCaseGender.Text = Convert.ToString(dataReader["CaseGender"]);
                labelRequestDate.Text = "Date: " + Convert.ToDateTime(dataReader["CaseDate"]).ToLongDateString();
                textBoxCaseSpecies.Text = Convert.ToString(dataReader["CaseSpecies"]);
                textBoxCaseBreed.Text = Convert.ToString(dataReader["CaseBreed"]);
                //textBoxRequestNotes.Text = GetRequestNotes(Convert.ToString(dataReader["RequestNotes"]));
                textBoxRequestClinicNotes.Text = Convert.ToString(dataReader["RequestClinicNotes"]);
            }
            dataObject.ObjectDispose();
            dataObject = null;
            PopulateImages();
            buttonPrint.Focus();
        }

        private void PopulateImages()
        {
            //*** Being Do Not Delete ***
            const int LABEL_RESULT_HEIGHT = 60;
            int IMAGE_COLUMN_MAXIMUM = panelImagesResults.Width / LABEL_RESULT_HEIGHT;
            const int LABEL_RESULT_WIDTH = 300;
           
            Font LABEL_FONT_NEGATIVE = new Font(FontFamily.GenericSansSerif, 12f);
            Font LABEL_FONT_POSITIVE = new Font(FontFamily.GenericSansSerif, 12f, FontStyle.Bold | FontStyle.Italic);
            
            int parasiteTypeCount = Enum.GetNames(typeof(FECALPARASITE_TYPE)).Length;
            int IMAGE_ROW_MAXIMUM = parasiteTypeCount - 1;


            panelImages.Controls.Clear();
            panelImages.Height = IMAGE_ROW_MAXIMUM * LABEL_RESULT_HEIGHT;
            panelImagesResults.Height = panelImages.Height;
            panelImages.Width = LABEL_RESULT_WIDTH;
            panelImages.Left = labelRequestNotes.Left;
            panelImagesResults.Left = panelImages.Left + panelImages.Width;
           
            ParameterController parameterController = new ParameterController();
            int[] parasiteCountMaximums = null;
            int[] parasiteCountMinimums = parameterController.GetParasiteRanges(ref parasiteCountMaximums);


            //*** End Do Not Delete ***

            try
            {
                Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();

                int pixelTop = 0;

                for (int resultValue = 1; resultValue < parasiteTypeCount; resultValue++)
                {
                    if (resultValue == (int)FECALPARASITE_TYPE.Strongyle) { continue; }

                    bool IsPositive = false;

                    #region "*** Select Count of Images From Results and Populate Report Labels ***"
                    
                    string stringSQL = "Select COUNT(FileName) as IMAGECOUNT From AnalysisRequestsResults Where CaseID = '" + ObjectsStatic.CASEID + "' and RequestID = " + ObjectsStatic.REQUESTID + " AND RESULTVALUE = " + Convert.ToString(resultValue);
                    int imageCount = Convert.ToInt32(dataObject.GetDataScalarFromQuery(stringSQL));
                    //*** Create Label for Results ***
                    Label objectLabel = new Label();
                    objectLabel.Top = pixelTop;
                    objectLabel.Left = 0;
                    objectLabel.Width = LABEL_RESULT_WIDTH;
                    objectLabel.Height = LABEL_RESULT_HEIGHT;
                    objectLabel.ForeColor = Color.Black;
                    objectLabel.TextAlign = ContentAlignment.MiddleLeft;

                    string stringResult = String.Empty;
                    if (imageCount >= parasiteCountMinimums[resultValue])
                    {
                        stringResult = SharedFunctionController.GetParasiteResults(resultValue, imageCount);
                    }

                    if (stringResult == String.Empty)
                    {
                        stringResult = StringEnum.GetStringValue((FECALPARASITE_TYPE)resultValue) + ": Negative";
                        objectLabel.Font = LABEL_FONT_NEGATIVE;
                        objectLabel.Text = stringResult;
                    }
                    else
                    {
                        objectLabel.Font = LABEL_FONT_POSITIVE;
                        objectLabel.Text = stringResult;
                        IsPositive = true;
                       
                    }
                    objectLabel.Visible = true;
                    panelImages.Controls.Add(objectLabel);

                    

                    #endregion

                    #region "*** Select Images From Results and Populate Report Images ***"

                    if (IsPositive == true)
                    {

                        int pixelLeft = 0;
                        stringSQL = "Select Top " + Convert.ToString(IMAGE_COLUMN_MAXIMUM) + " FileName From AnalysisRequestsResults Where CaseID = '" + ObjectsStatic.CASEID + "' and RequestID = " + ObjectsStatic.REQUESTID + " AND RESULTVALUE = " + Convert.ToString(resultValue) + " Order By ConfidenceValue Desc";

                        SqlDataReader sqlDataReader = dataObject.GetDataReaderFromQuery(stringSQL);
                        while (sqlDataReader.Read())
                        {
                            PictureBox objectPictureBox = new PictureBox();
                            objectPictureBox.Image = Image.FromFile(FOLDER_PATH + Convert.ToString(sqlDataReader[0]));
                            objectPictureBox.Height = LABEL_RESULT_HEIGHT;
                            objectPictureBox.Width = LABEL_RESULT_HEIGHT;
                            objectPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                            objectPictureBox.Left = pixelLeft;
                            objectPictureBox.Top = pixelTop;
                            panelImagesResults.Controls.Add(objectPictureBox);
                            pixelLeft = pixelLeft + objectPictureBox.Width;
                            objectPictureBox.Visible = true;
                        }
                        sqlDataReader.Close();

                    }

                    #endregion


                    pixelTop = pixelTop + LABEL_RESULT_HEIGHT;
                }

                dataObject.ObjectDispose();
            }
            catch { }

            //*** Being Do Not Delete ***

            //labelResults.Text = "Results for Case# " + ObjectsStatic.CASEID;
            //labelResults.AutoSize = true;
            //labelResults.Left = (this.Width / 2) - (labelResults.Width / 2);
            //*** End Do Not Delete ***
            parameterController = null;
        }

        #endregion
    
        private string GetRequestNotes(string stringRequestNotes)
        {
            StringBuilder returnValue = new StringBuilder();
            int positiveCount = 0;
            returnValue.Append(Environment.NewLine).Append(Environment.NewLine).Append(stringRequestNotes.Trim());
            if (positiveCount > 0)
            {  return "The Patient is Positive for: " + returnValue.ToString() + "."; }
            else
            { return "The Patient is Negative."; }
            
        }

        private void SendEMail()
        {
            ToggleButtonsForPrint(false);
            PrintReport(false);
            string stringFileNameReport = FOLDER_PATH + "Report.png";

            HelperFunctions objectFunctions = new HelperFunctions();

            string stringBody = "";
            //string stringBody = "Ova & Parasites Report for PatientID: " + textBoxCaseID.Text + ", For " + ObjectsStatic.OrganizationName +
               // "QMIRA Results are: " + textBoxRequestNotes.Text.Trim() + ".  " + Environment.NewLine + Environment.NewLine +
              //  "Attached are images of positive particles on the scan, you can view the attached file with any image viewer." + Environment.NewLine + Environment.NewLine;
            

            string[] emailAttachments = new string[1] { stringFileNameReport };
            
            objectFunctions.SendEMail("Patient Ova & Parasites Report From " + ObjectsStatic.OrganizationName, stringBody, emailAttachments, String.Empty);
                Cursor.Current = Cursors.Default;
            ToggleButtonsForPrint(true);
        }

        private void buttonEMail_Click(object sender, EventArgs e)
        {
            SendEMail();
            buttonClose.Visible = true;
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            textBoxRequestClinicNotes.Focus();
            Application.DoEvents();
            ToggleButtonsForPrint(false);
            PrintReport(true);
            ToggleButtonsForPrint(true);
            buttonClose.Visible = true;
        }

        private void ToggleButtonsForPrint(bool IsVisible)
        {
            buttonClose.Visible = IsVisible;
            buttonEMail.Visible = IsVisible;
            buttonPrint.Visible = IsVisible;
            //buttonExport.Visible = IsVisible;
        }

        private void UpdateClinicNotes()
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "Update AnalysisRequests Set RequestClinicNotes = '" + textBoxRequestClinicNotes.Text.Trim() + "' " +
                "Where CaseID = '" + ObjectsStatic.CASEID + "' and RequestID = " + ObjectsStatic.REQUESTID;
            dataObject.ExecuteQuery(stringSQL);
            dataObject.ObjectDispose();
            dataObject = null;
        }

        private void UpdateCaseCompleted()
        {
           // Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            //*** Temp Code ***

            //string stringSQL = "Update AnalysisRequests Set RequestDateCompleted = GetDate(), Completed = 1 " +
            //    "Where CaseID = '" + ObjectsStatic.CASEID + "' and RequestID = " + ObjectsStatic.REQUESTID;
            //dataObject.ExecuteQuery(stringSQL);
            //dataObject.ObjectDispose();
            //dataObject = null;

        }

        private void buttonExport_Click(object sender, EventArgs e)
        {

        }

        private void textBoxRequestClinicNotes_TextChanged(object sender, EventArgs e)
        {
            IsNotesChanged = true;
        }

        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Image image = System.Drawing.Image.FromStream(this.streamToPrint);
            e.PageSettings.Margins.Left = 10;
            e.PageSettings.Margins.Right = 10;
            Margins margins = new Margins(0, 0, 0, 0);
            printDocument1.PrinterSettings.DefaultPageSettings.Margins = margins;
        
            int x = e.MarginBounds.X;
            int y = e.MarginBounds.Y;
            int width = image.Width;
            int height = image.Height;
            if ((width / e.MarginBounds.Width) > (height / e.MarginBounds.Height))
            {
                width = e.MarginBounds.Width;
                height = image.Height * e.MarginBounds.Width / image.Width;
            }
            else
            {
                height = e.MarginBounds.Height;
                width = image.Width * e.MarginBounds.Height / image.Height;
            }
            System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(x, y, width, height);
            e.Graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
        }

        public void StartPrint(Stream streamToPrint, string streamType)
        {
            this.printDocument1.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
            this.printDocument1.DefaultPageSettings.Landscape = false;

            this.streamToPrint = streamToPrint;
            this.streamType = streamType;
            System.Windows.Forms.PrintDialog PrintDialog1 = new PrintDialog();
            PrintDialog1.AllowSomePages = true;
            PrintDialog1.ShowHelp = true;
            PrintDialog1.Document = this.printDocument1;

            DialogResult result = PrintDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.printDocument1.Print();
                //docToPrint.Print();
            }

        }

        private void PrintReport(bool IsPrinted)
        {
           // textBoxRequestNotes.Visible = true;

            
            Graphics g1 = this.CreateGraphics();
            Image MyImage = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height, g1);
            Graphics g2 = Graphics.FromImage(MyImage);
            IntPtr dc1 = g1.GetHdc();
            IntPtr dc2 = g2.GetHdc();
            BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);
            g1.ReleaseHdc(dc1);
            g2.ReleaseHdc(dc2);

            Font fontOrganization = new Font("Arial", 16, FontStyle.Italic);
           // Brush brushOrganization = new Brush(ObjectsStatic.StandardColorBackground);
            int pixelLeft = this.ClientRectangle.Width / 2;

            try
            {
                MyImage.Save(FOLDER_PATH + "Report.png", ImageFormat.Png);
            }
            catch { }
            
            if ((IsPrinted == true) && (Properties.Settings.Default.BOOLEAN_ISPRINTINGENABLED == true))
            {
                FileStream fileStream = new FileStream(FOLDER_PATH + "Report.png", FileMode.Open, FileAccess.Read);
                StartPrint(fileStream, "Image");
                fileStream.Close();
            }

        }


    }
}
