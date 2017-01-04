using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Parascan1.Data;

namespace Parascan0
{
    public partial class formInterface : Form
    {

        #region "*** Declare Variables ***"

        private string REQUESTID = "";
        private string LOCATIONID = Properties.Settings.Default.STRING_LOCATIONID;
        private string ORGANIZATIONID = Properties.Settings.Default.STRING_ORGANIZATIONID;
        private string APPLICATION_PATH = Application.StartupPath;
        bool IS_NEW_CASE = false;
        private int INTEGER_SPACER = 20;
        private bool IsTraining = Properties.Settings.Default.BOOLEAN_ISTRAINING;
        string FOLDER_PATH = Properties.Settings.Default.STRING_PATHCASES + "\\" + ObjectsStatic.CASEID + "\\" + SharedValueController.SUBFOLDER_PROCESSED + "\\";
        
        private enum SCREEN_TYPE
        {
            HOME = 1,
            CASES = 2,
            SCAN = 3,
            RESULTS = 4
        }

        #endregion

        #region "*** Form Initialization Methods ***"

        public formInterface()
        {
            InitializeComponent();
        }

        private void formInterface_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'qMIRADataSet.vwCasesEscalated' table. You can move, or remove it, as needed.
            GetHomeScreen();
        }

        #endregion

        #region "*** Private Interface Methods ***"

        private void GetHomeScreen()
        {
            //Set Back Color for Form
            this.BackColor = ObjectsStatic.StandardColorBackground;
            textBoxLogo.BackColor = ObjectsStatic.StandardColorBackground;
            panelImages.BackColor = ObjectsStatic.StandardColorBackground;
            pictureBoxLogo.Load(ObjectsStatic.IMAGEPATH_LOGO);
            pictureBoxLogo.Refresh();

            this.Width = 1082;
            this.Height = 660;
            INTEGER_SPACER = 50;
            const int INTEGER_BUTTONSIZE = 300;
            ClearScreen();
            comboSpecies.SelectedIndex = 0;
            //pictureBoxCaseNew.Width = INTEGER_BUTTONSIZE;
            //pictureBoxCaseNew.Height = INTEGER_BUTTONSIZE;
            //pictureBoxCaseSearch.Width = INTEGER_BUTTONSIZE;
            //pictureBoxCaseSearch.Height = INTEGER_BUTTONSIZE;
            pictureBoxCaseNew.Width = INTEGER_BUTTONSIZE;
            pictureBoxCaseNew.Height = INTEGER_BUTTONSIZE;
            pictureBoxCaseSearch.Width = pictureBoxCaseNew.Width;
            pictureBoxCaseSearch.Height = pictureBoxCaseNew.Height;
            pictureBoxCaseNew.Left = Convert.ToInt16(this.Width / 2) - pictureBoxCaseNew.Width - Convert.ToInt16(INTEGER_SPACER / 2);
            pictureBoxCaseNew.Top = (this.Height / 2) - (pictureBoxCaseNew.Height / 2) - 60;
            pictureBoxCaseSearch.Left = pictureBoxCaseNew.Left + pictureBoxCaseNew.Width + INTEGER_SPACER;
            pictureBoxCaseSearch.Top = pictureBoxCaseNew.Top;
           

             pictureBoxLogo.Left = (this.Width / 2) - (pictureBoxLogo.Width / 2);
            pictureBoxLogo.Top = this.Height / 100;
            pictureBoxLogoParascan.Top = pictureBoxLogo.Top + pictureBoxLogo.Height - 5;
            pictureBoxLogoParascan.Left = pictureBoxLogo.Left + Convert.ToInt16(pictureBoxLogo.Width / 2) - Convert.ToInt16(pictureBoxLogoParascan.Width / 2);
            textBoxLogo.Top = pictureBoxLogoParascan.Top + pictureBoxLogoParascan.Height + 2;
            textBoxLogo.Left = pictureBoxLogo.Left + Convert.ToInt16(pictureBoxLogo.Width / 2) - Convert.ToInt16(textBoxLogo.Width / 2);
            pictureBoxLogo.Visible = true;
            pictureBoxLogoParascan.Visible = true;
            textBoxLogo.Visible = true;

            pictureBoxCaseNew.Visible = true;
            pictureBoxCaseSearch.Visible = true;
            dataGridView.Visible = false;
            this.ActiveControl = pictureBoxCaseNew;
            

            //pictureBoxReports.Visible = true;
            //if (ObjectsStatic.IsAdministrator == true)
           // {
                pictureBoxSetup.Left = Convert.ToInt16(pictureBoxCaseNew.Left / 2);
                pictureBoxSetup.Top = this.Height - pictureBoxSetup.Height - 200;
                pictureBoxSetup.Visible = true;
                pictureBoxHelp.Left = this.Width - pictureBoxSetup.Left - pictureBoxHelp.Width;
                pictureBoxHelp.Top = pictureBoxSetup.Top;
                pictureBoxHelp.Visible = true;
                pictureBoxLogoQMIRA.Top = pictureBoxHelp.Top + ((int)pictureBoxHelp.Height / 2) - ((int)pictureBoxLogoQMIRA.Height / 2);
                pictureBoxLogoQMIRA.Left = Convert.ToInt32(this.Width / 2) - (pictureBoxLogoQMIRA.Width / 2);
                pictureBoxLogo.Visible = true;
                
            //}

            labelEscalated.Left = 5;
            dataGridViewEscalated.Left = labelEscalated.Left + labelEscalated.Width + 5;
            dataGridViewEscalated.Width = this.Width - 25 - (labelEscalated.Left + labelEscalated.Width);
            dataGridViewEscalated.Top = pictureBoxCaseNew.Top + pictureBoxCaseNew.Height + 30;
            dataGridViewEscalated.Height = 125;
            labelEscalated.Top = dataGridViewEscalated.Top + (int)(dataGridViewEscalated.Height / 2) - (int)(labelEscalated.Height / 2);

            GetSecondOpinions();
        }

        private void GetSearchScreen(bool IsNewCase)
        {
            IS_NEW_CASE = IsNewCase;
            ClearScreen();
            ClearValues();
            panelCases.Left = 10;
            panelCases.Top = pictureBoxLogo.Top + pictureBoxLogo.Height;
            dataGridView.Left = panelCases.Left + panelCases.Width + 55;
            dataGridView.Top = panelCases.Top;
            pictureBoxScan.Left = dataGridView.Left + (dataGridView.Width / 2) - ((pictureBoxScan.Width + pictureBoxReports.Width + 20) / 2);
            pictureBoxScan.Top = dataGridView.Top + dataGridView.Height + 10;
            pictureBoxLogo.Left = panelCases.Left + (panelCases.Width / 2) - (pictureBoxLogo.Width / 2);
            pictureBoxLogo.Top = 10;
            labelMessage.Top = panelCases.Top + panelCases.Height - 3;
            labelMessage.Text = "";
            comboSpecies.SelectedIndex = 0;
            dateTimePicker.Value = System.DateTime.Now;
                   
            if (IsNewCase == true)
            {
                pictureBoxSave.Left = panelCases.Left;
                pictureBoxSave.Top = panelCases.Top + panelCases.Height + 20;
                pictureBoxBack.Left = pictureBoxSave.Left + pictureBoxSave.Width + 5;
                pictureBoxBack.Top = pictureBoxSave.Top;
                pictureBoxSave.Visible = true;
                pictureBoxSearch.Visible = false;
                pictureBoxScan.Enabled = false;
                pictureBoxScan.Visible = false;
                labelMessage.Visible = true;
                if (IsTraining == true)
                {
                    textCaseID.Text = string.Format("{0:D2}", System.DateTime.Now.Hour) + string.Format("{0:D2}", System.DateTime.Now.Minute) + string.Format("{0:D2}", System.DateTime.Now.Second);
                    ObjectsStatic.CASEID = textCaseID.Text;
                    textBarcode.Text = textCaseID.Text;
                    textBoxNamePet.Text = textCaseID.Text;
                    //comboGender.Text = "";
                    //comboStatus.Text = "";
                    textBoxDoctor.Text = "Doctor";
                    //comboTest.Text = "";
                    textBoxOwnerFirst.Text = "First";
                    textBoxOwnerLast.Text = textCaseID.Text;
                    //*** Begin Temp Code ***
                    if (IsTraining == true)
                    {
                        formTestResults objectFormTestResults = new formTestResults();
                        objectFormTestResults.ShowDialog();
                        textCaseID.Text = ObjectsStatic.CASEID;
                    }
                    comboSpecies.SelectedIndex = 1;
                    pictureBoxSave_Click(null, null);
                    pictureBoxScan_Click(null, null);
                    //*** End Temp Code ***
                }
                //comboStatus.SelectedIndex = 0;
            }
            else
            {
                pictureBoxSearch.Left = panelCases.Left;
                pictureBoxSearch.Top = panelCases.Top + panelCases.Height + 20;
                pictureBoxBack.Left = pictureBoxSearch.Left + pictureBoxSearch.Width + 5;
                pictureBoxBack.Top = pictureBoxSearch.Top;
                pictureBoxSave.Visible = false;
                pictureBoxSearch.Visible = true;
                pictureBoxScan.Visible = false;
                //labelMessage.Visible = false;
            }
            panelCases.Visible = true;
            dataGridView.Visible = true;
            pictureBoxBack.Visible = true;
                
            if (Properties.Settings.Default.BOOLEAN_ISLIVESCAN != false)
            {
                
                pictureBoxSearch_Click(null, null);
                pictureBoxLogo.Visible = true;
                textCaseID.Focus();
                //comboTest.Text = "Ova/Parasites";
            }
            
        }

        private void GetResultsScreen(bool IsNewCase)
        {
            if (IsNewCase == true)
            {
                formClinicRemoveSlide objectFormClinicRemoveSlide = new formClinicRemoveSlide();
                objectFormClinicRemoveSlide.ShowDialog();
                Application.DoEvents();
                objectFormClinicRemoveSlide.Dispose();
            }


            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
           
            string stringSQL = "Select Count(*) from AnalysisCases where CaseID = '" + ObjectsStatic.CASEID + "' and CaseCompleted > GETDATE() - 365";
           
            //*** Start Processing Executables ***
            if (IsTraining != true)
            {
                if (dataObject.GetDataScalarFromQuery(stringSQL) == "0")
                {
                    for (int processCount = 0; processCount < 3; processCount++)
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.STRING_PROCESSINGEXECUTABLE);
                        startInfo.Arguments = SharedValueController.STRING_ARGUMENT_REALTIME;
                        System.Diagnostics.Process.Start(startInfo);
                        System.Threading.Thread.Sleep(1000);
                        Application.DoEvents();
                    }
                }
            }
            //*** Clear Screen ***
            ClearScreen();
            pictureBoxLogo.Top = 20;
            pictureBoxLogo.Left = (this.Width / 2) - (pictureBoxLogo.Width / 2);
            pictureBoxLogo.Visible = true;
            pictureBoxLogo.Refresh();
            labelResults.Text = "Case #" + ObjectsStatic.CASEID + " Is Being Analyzed..";
            labelResults.Left = pictureBoxLogo.Left - (int)((labelResults.Width - pictureBoxLogo.Width) / 2);
            labelResults.Top = pictureBoxLogo.Top + pictureBoxLogo.Height;
            labelResults.Visible = true;
            progressBar1.Value = 0;
            progressBar1.Left = 100;
            progressBar1.Width = this.Width - 200;
            progressBar1.Visible = true;

           

            int completeAttempts = 0;
            
            if (IsTraining != true)
            {
                while (dataObject.GetDataScalarFromQuery(stringSQL) == "0")
                {
                    completeAttempts++;
                    labelResults.Text = "Case #" + ObjectsStatic.CASEID + " Is Being Analyzed";
                    if (completeAttempts == 1) { labelResults.Text = labelResults.Text + "."; }
                    else if (completeAttempts == 2) { labelResults.Text = labelResults.Text + ".."; }
                    else { labelResults.Text = labelResults.Text + "..."; completeAttempts = 0; }
                    labelResults.Refresh();
                    labelResults.Focus();
                    Application.DoEvents();
                    if (progressBar1.Value < progressBar1.Maximum) { progressBar1.Value = progressBar1.Value + 1; }
                    System.Threading.Thread.Sleep(4000);
                }
            }
            //*** Check for Tapeworm Segments and Adult Worms ***
            if (IsNewCase == true)
            {
                if (ObjectsStatic.HasTapewormSegments == true)
                {
                    try
                    {
                        DirectoryInfo imagesFolder = new DirectoryInfo(Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHIMAGES);
                        FileInfo[] imagesFiles = imagesFolder.GetFiles(SharedValueController.STRING_TAPEWORMFILE + "*.JPG");
                        for (int index = 0; index < imagesFiles.Length; index++)
                        {
                            imagesFiles[index].CopyTo(FOLDER_PATH + imagesFiles[index].Name);
                            stringSQL = "Insert Into AnalysisRequestsResults (RequestID,CaseID,ResultValue,FileName,ConfidenceValue) Values (" + ObjectsStatic.REQUESTID + st(ObjectsStatic.CASEID) + st((int)FECALPARASITE_TYPE.Dipylidian) + st(imagesFiles[index].Name) + st(index) + ")";
                            dataObject.ExecuteQuery(stringSQL);
                        }
                    }
                    catch { }
                }
            }


            dataObject.ObjectDispose();
            PopulateImages();
            EndAnalysisProcesses();
            //labelResults.Top = pictureBoxTips.Top + pictureBoxTips.Height + 20;
            progressBar1.Value = 0;
            progressBar1.Visible = false;
            pictureBoxReports.Height = 200;
            pictureBoxReports.Width = 200;
            pictureBoxReports.Top = labelResults.Top + labelResults.Height + 5;

            if (IsNewCase == false)
            {
                pictureBoxResultsEscalate.Top = pictureBoxReports.Top;
                pictureBoxResultsEscalate.Left = Convert.ToInt16(this.Width / 2) - pictureBoxResultsEscalate.Width - 25;
                pictureBoxReports.Left = pictureBoxResultsEscalate.Left + pictureBoxResultsEscalate.Width + 50;
                pictureBoxResultsEscalate.Visible = true;
            }
            else
            {
                pictureBoxReports.Left = Convert.ToInt16(this.Width / 2) - (pictureBoxReports.Width / 2);
            }

            panelImages.Top = pictureBoxReports.Top + pictureBoxReports.Height + 20;
            panelImages.Left = (int)((this.Width - panelImages.Width) / 2);
            //*** Draw Rectangle Around Results ***
            Pen objectPenWhite = new Pen(Color.White, 3);
            System.Drawing.Graphics graphics = this.CreateGraphics();
            graphics.DrawLine(objectPenWhite, panelImages.Left - 5, panelImages.Top - 5, panelImages.Left + panelImages.Width + 5, panelImages.Top - 5);
            graphics.DrawLine(objectPenWhite, panelImages.Left - 5, panelImages.Top + panelImages.Height + 10, panelImages.Left + panelImages.Width + 5, panelImages.Top + panelImages.Height + 10);
            graphics.Dispose();
            //System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(panelImages.Left - 5, panelImages.Top - 10, panelImages.Width + 10, panelImages.Height + 20);
            //graphics.DrawRectangle(objectPenWhite, rectangle);
           

            pictureBoxHelp.Top = panelImages.Top + panelImages.Height + 20;
            pictureBoxHelp.Left = pictureBoxLogoQMIRA.Left + pictureBoxLogoQMIRA.Width + 100;
            pictureBoxBack.Left = pictureBoxLogoQMIRA.Left - pictureBoxBack.Width - 100;
            pictureBoxBack.Top = pictureBoxHelp.Top;
            pictureBoxBack.Visible = true;
            pictureBoxLogoQMIRA.Top = pictureBoxHelp.Top + 10;
            pictureBoxLogoQMIRA.Visible = true;
            pictureBoxReports.Visible = true;
            //pictureBoxResultsEscalate.Visible = true;

            panelImages.Visible = true;

            IS_NEW_CASE = false;
            

        }

        private void GetSelectedSample(object sender, DataGridViewCellEventArgs e, bool IsEscalatedCase)
        {
            try
            {
                if (IsEscalatedCase == true)
                {
                    ObjectsStatic.CASEID = dataGridViewEscalated.Rows[e.RowIndex].Cells["CaseID"].Value.ToString();
                    ObjectsStatic.REQUESTID = dataGridViewEscalated.Rows[e.RowIndex].Cells["RequestID"].Value.ToString();
                }
                else
                {
                    ObjectsStatic.CASEID = dataGridView.Rows[e.RowIndex].Cells["CaseID"].Value.ToString();
                    ObjectsStatic.REQUESTID = dataGridView.Rows[e.RowIndex].Cells["RequestID"].Value.ToString();
                }
                string stringSQL = "Select Count(*) from AnalysisCases where CaseID = '" + ObjectsStatic.CASEID + "' and CaseCompleted IS NOT NULL";
                Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
                if (dataObject.GetDataScalarFromQuery(stringSQL) == "0")
                {
                    MessageBox.Show("There are No Results for This Patient");
                    return;
                }

                GetResultsScreen(false);
            }
            catch
            { }
        }



        private void ClearScreen()
        {
            pictureBoxCaseNew.Visible = false;
            pictureBoxCaseSearch.Visible = false;
            pictureBoxReports.Visible = false;
            pictureBoxResultsEscalate.Visible = false;
            pictureBoxSetup.Visible = false;
            //pictureBoxArchive.Visible = false;
            //pictureBoxResultsEscalate.Visible = false;
            panelCases.Visible = false;
            dataGridView.DataSource = null;
            dataGridView.Refresh();
            dataGridView.Visible = false;
            pictureBoxSave.Visible = false;
            pictureBoxSearch.Visible = false;
            pictureBoxScan.Visible = false;
            pictureBoxBack.Visible = false;
            labelResults.Visible = false;
            pictureBoxLogo.Visible = false;
            pictureBoxLogoParascan.Visible = false;
            textBoxLogo.Visible = false;
            labelEscalated.Visible = false;
            dataGridViewEscalated.Visible = false;
            textCaseID.Focus();
            labelMessage.Visible = false;
            panelImages.Visible = false;
            progressBar1.Visible = false;
            Pen objectPenWhite = new Pen(ObjectsStatic.StandardColorBackground, 3);
            System.Drawing.Graphics graphics = this.CreateGraphics();
            graphics.DrawLine(objectPenWhite, panelImages.Left - 5, panelImages.Top - 15, panelImages.Left + panelImages.Width + 5, panelImages.Top - 15);
            graphics.DrawLine(objectPenWhite, panelImages.Left - 5, panelImages.Top + panelImages.Height + 10, panelImages.Left + panelImages.Width + 5, panelImages.Top + panelImages.Height + 10);
            graphics.Dispose();
            
        }

        private void ClearValues()
        {
            textCaseID.Text = "";
            textBarcode.Text = "";
            textBoxNamePet.Text = "";
            comboSpecies.SelectedIndex = 0;
            comboSpecies.Text = Convert.ToString(comboSpecies.Items[0]);
            //comboGender.Text = "";
            //comboStatus.Text = "";
            textBoxDoctor.Text = "";
            //comboTest.Text = "";
            textBoxOwnerFirst.Text = "";
            textBoxOwnerLast.Text = "";
            labelMessage.Text = "";
            ObjectsStatic.HasTapewormSegments = false;
            ObjectsStatic.HasAdultWorms = false;
        }

        #endregion

        #region "*** Private Case and Search Methods ***"

        private void PopulateCase(object sender, DataGridViewCellEventArgs e)
        {
           Specimen objectSpecimen = new Specimen(dataGridView.Rows[e.RowIndex].Cells["CaseID"].Value.ToString(), Convert.ToDateTime(dataGridView.Rows[e.RowIndex].Cells["Date"].Value),
                dataGridView.Rows[e.RowIndex].Cells["OwnerFirst"].Value.ToString(), dataGridView.Rows[e.RowIndex].Cells["OwnerLast"].Value.ToString(),
               dataGridView.Rows[e.RowIndex].Cells["PetName"].Value.ToString(), dataGridView.Rows[e.RowIndex].Cells["Species"].Value.ToString(),
               dataGridView.Rows[e.RowIndex].Cells["Doctor"].Value.ToString(), dataGridView.Rows[e.RowIndex].Cells["RequestID"].Value.ToString());
            
            REQUESTID = dataGridView.Rows[e.RowIndex].Cells[dataGridView.Rows[e.RowIndex].Cells.Count - 1].Value.ToString();
            FOLDER_PATH = Properties.Settings.Default.STRING_PATHCASES + "\\" + ObjectsStatic.CASEID + "\\" + SharedValueController.SUBFOLDER_PROCESSED + "\\";
            DisplaySpecimen(objectSpecimen);

        }

        private void DisplaySpecimen(Specimen objectSpecimen)
        {
            textCaseID.Text = objectSpecimen.ID;
            textBarcode.Text = objectSpecimen.SpecimenNumber;
            //comboTest.Text = objectSpecimen.ConditionID;
            textBoxOwnerFirst.Text = objectSpecimen.First;
            textBoxOwnerLast.Text = objectSpecimen.Last;
            textBoxNamePet.Text = objectSpecimen.Nickname;
            comboSpecies.Text = objectSpecimen.Species;
            //comboBreed.Text = objectSpecimen.Breed;
            //comboGender.Text = objectSpecimen.Gender;
            //comboStatus.Text = objectSpecimen.Status;
            //dateTimePicker.Text = Convert.ToString(objectSpecimen.Date);
            textBoxDoctor.Text = objectSpecimen.Doctor;
            pictureBoxScan.Enabled = true;
            ObjectsStatic.CASEID = textCaseID.Text;
            ObjectsStatic.SPECIMENNUMBER = textBarcode.Text;
            ObjectsStatic.REQUESTID = objectSpecimen.RequestID;
            FOLDER_PATH = Properties.Settings.Default.STRING_PATHCASES + "\\" + ObjectsStatic.CASEID + "\\" + SharedValueController.SUBFOLDER_PROCESSED + "\\";
        }


        private void PopulateImages()
        {
            //*** Being Do Not Delete ***
            const int LABEL_RESULT_HEIGHT = 40;
            const int LABEL_RESULT_WIDTH = 400;
            const int BUTTON_RESULT_WIDTH = 100;
            const int BUTTON_RESULT_HEIGHT = 40;
            ObjectsStatic.RESULTS_STRING = String.Empty;

            Font LABEL_FONT_NEGATIVE = new Font(FontFamily.GenericSansSerif, 14f);
            Font LABEL_FONT_POSITIVE = new Font(FontFamily.GenericSansSerif, 16f, FontStyle.Bold);
            Font BUTTON_FONT = new Font(FontFamily.GenericSansSerif, 12f);

            int parasiteTypeCount = Enum.GetNames(typeof(FECALPARASITE_TYPE)).Length;
            int IMAGE_ROW_MAXIMUM = parasiteTypeCount - 1;
           

            panelImages.Controls.Clear();
            panelImages.Height = IMAGE_ROW_MAXIMUM * LABEL_RESULT_HEIGHT;
            panelImages.Width = LABEL_RESULT_WIDTH + BUTTON_RESULT_WIDTH + BUTTON_RESULT_WIDTH + 60;
            panelImages.Left = (this.Width / 2) - (panelImages.Width / 2);
            panelImages.Top = pictureBoxReports.Top + pictureBoxReports.Height + 40;
            
            
            ParameterController parameterController = new ParameterController();
            int[] parasiteCountMaximums = null;
            int[] parasiteCountMinimums = parameterController.GetParasiteRanges(ref parasiteCountMaximums);

            
            //*** End Do Not Delete ***

            try
            {
                //*** Check Parasite Results and Counts ***
                if (Properties.Settings.Default.BOOLEAN_ISENABLEDEDITRESULT == true)
                {
                    CheckParasiteReview(ObjectsStatic.CASEID, ObjectsStatic.REQUESTID);
                }
               
                Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
                StringBuilder stringBuilderResults = new StringBuilder();
                int pixelTop = 0;
                
                for (int resultValue = 1; resultValue < parasiteTypeCount; resultValue++)
                {
                    if (resultValue == (int)FECALPARASITE_TYPE.Strongyle) { continue; }
                    int pixelLeft = 20;

                    string stringSQL = "Select COUNT(FileName) as IMAGECOUNT From AnalysisRequestsResults Where CaseID = '" + ObjectsStatic.CASEID + "' and RequestID = " + ObjectsStatic.REQUESTID + " AND RESULTVALUE = " + Convert.ToString(resultValue);
                    int imageCount = Convert.ToInt32(dataObject.GetDataScalarFromQuery(stringSQL));
                    //*** Create Label for Results ***
                    Label objectLabel = new Label();
                    objectLabel.Top = pixelTop;
                    objectLabel.Left = 0;
                    objectLabel.Width = LABEL_RESULT_WIDTH;
                    objectLabel.Height = LABEL_RESULT_HEIGHT;
                    objectLabel.ForeColor = Color.White;
                    objectLabel.BackColor = Color.Transparent;
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
                        objectLabel.Visible = true;
                        panelImages.Controls.Add(objectLabel);
                   
                    }
                    else
                    {
                        objectLabel.Font = LABEL_FONT_POSITIVE;
                        objectLabel.Text = stringResult;
                        objectLabel.Visible = true;
                        panelImages.Controls.Add(objectLabel);
                        //*** Create Button to View Positive Images ***
                        Button objectButtonView = new Button();
                        pixelLeft = objectLabel.Width + 10;
                        objectButtonView.Left = pixelLeft;
                        objectButtonView.Top = objectLabel.Top;
                        objectButtonView.Width = BUTTON_RESULT_WIDTH;
                        objectButtonView.Height = BUTTON_RESULT_HEIGHT;
                        objectButtonView.Text = "Images...";
                        objectButtonView.TextAlign = ContentAlignment.MiddleCenter;
                        objectButtonView.ForeColor = Color.White;
                        objectButtonView.Tag = Convert.ToString(resultValue);
                        objectButtonView.Click += new EventHandler(PictureBoxClick);
                        objectButtonView.Visible = true;
                        panelImages.Controls.Add(objectButtonView);
                        //*** Create Button to View Parasite Data Sheet ***
                        Button objectButtonData = new Button();
                        pixelLeft = objectLabel.Width + (BUTTON_RESULT_WIDTH / 2);
                        objectButtonData.Left = objectButtonView.Left + objectButtonView.Width + 10;
                        objectButtonData.Top = objectLabel.Top;
                        objectButtonData.Width = BUTTON_RESULT_WIDTH;
                        objectButtonData.Height = BUTTON_RESULT_HEIGHT;
                        objectButtonData.Text = "Info...";
                        objectButtonData.TextAlign = ContentAlignment.MiddleCenter;
                        objectButtonData.ForeColor = Color.White;
                        objectButtonData.Visible = true;
                        panelImages.Controls.Add(objectButtonData);
                    }
                    stringBuilderResults.Append(stringResult).Append(", ");
                    pixelTop = pixelTop + LABEL_RESULT_HEIGHT;
                }
                
                dataObject.ObjectDispose();
                ObjectsStatic.RESULTS_STRING = stringBuilderResults.ToString();
                ObjectsStatic.RESULTS_STRING = ObjectsStatic.RESULTS_STRING.Substring(0, ObjectsStatic.RESULTS_STRING.Length - 2);
            
            }
            catch { }
                
            //*** Being Do Not Delete ***
             labelResults.Text = "Results for Case# " + ObjectsStatic.CASEID;
            labelResults.AutoSize = true;
            labelResults.Left = (this.Width / 2) - (labelResults.Width / 2);
            //*** End Do Not Delete ***
            parameterController = null;
        }

        
        #endregion

        #region "*** Private Form Event Methods ***"

        private void pictureBoxSearch_Click(object sender, EventArgs e)
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = GetSQLRequest();
            DataTable sqlTable = dataObject.GetDataTableFromQuery(stringSQL);
            dataGridView.AutoGenerateColumns = true;
            dataGridView.DataSource = sqlTable;
            dataGridView.Refresh();
            dataObject.ObjectDispose();
            dataObject = null;
            DataGridViewCellEventArgs eventArgs = new DataGridViewCellEventArgs(0,0);
            try
            {
                PopulateCase(sender, eventArgs);
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }

        }

        private void pictureBoxSave_Click(object sender, EventArgs e)
        {
            #region "*** Declare Variables ***"

            bool IsScanEnabled = true;
            textCaseID.Text = textCaseID.Text.ToUpper();
            if (textCaseID.Text.Trim().Length < 4) { textCaseID.Text = string.Format("{0:D2}", System.DateTime.Now.Hour) + string.Format("{0:D2}", System.DateTime.Now.Minute) + string.Format("{0:D2}", System.DateTime.Now.Second); }
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();

            #endregion

            #region "*** Check for Equine and Get Grams Value ***"

            if ((comboSpecies.Text.ToUpper().IndexOf(ObjectsStatic.STRING_EQUINE1) > -1) || (comboSpecies.Text.ToUpper().IndexOf(ObjectsStatic.STRING_EQUINE2) > -1))
            {
                List<string> messageBoxButtonStrings = new List<string>();
                messageBoxButtonStrings.Add("2 Grams");
                messageBoxButtonStrings.Add("5 Grams");
                DialogResult messageBoxResult = ObjectsStatic.MessageBoxResult("Grams of Feces?", "Equine Test", messageBoxButtonStrings);
                if (messageBoxResult == DialogResult.OK)
                {
                    ObjectsStatic.Grams = 2;
                }
                else
                {
                    ObjectsStatic.Grams = 5;
                }
            }

            #endregion

            #region "*** Insert Case and Request to Database ***"

            try
            {
                //*** Check for Duplicate Patient Sample for Patient ID and Date ***
                string stringSQL = "Select Count(*) From AnalysisRequests Where CaseID = '" + textCaseID.Text + "' And Convert(varchar(10),RequestDate,20) = '" + dateTimePicker.Text + "'";
                if (dataObject.GetDataScalarFromQuery(stringSQL) != "0")
                {
                    if (MessageBox.Show("There is a Sample for this Patient and Date. Scan Slide?", "Possible Duplicate Sample", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }

                }
                //*** Check for Duplicate Patient ID and Insert Patient ***
                stringSQL = "Select Count(*) From AnalysisCases Where CaseID = '" + textCaseID.Text + "' and OrganizationID = '" + Properties.Settings.Default.STRING_ORGANIZATIONID + "' and LocationID = '" + Properties.Settings.Default.STRING_LOCATIONID + "'";
                if (dataObject.GetDataScalarFromQuery(stringSQL) == "0")
                {
                    //stringSQL = "Insert Into AnalysisCases (CaseID, OrganizationID, LocationID, CaseTest, CaseFirst, CaseLast, CaseNickname, CaseSpecies, CaseBreed, CaseGender, CaseStatus, CaseDate) Values ('" +
                    //textCaseID.Text + "'" + sp(Properties.Settings.Default.STRING_ORGANIZATIONID) + sp(Properties.Settings.Default.STRING_LOCATIONID) + sp(comboTest.Text) + sp(textBoxOwnerFirst.Text) + sp(textBoxOwnerLast.Text) + sp(textBoxNamePet.Text) + sp(comboSpecies.Text) + sp(comboBreed.Text) + sp(comboGender.Text) + sp(comboStatus.Text) + sp(dateTimePicker.Text) + ")";
                    stringSQL = "Insert Into AnalysisCases (CaseID, OrganizationID, LocationID, CaseFirst, CaseLast, CaseNickname, CaseSpecies, CaseDate, CaseDoctor) Values ('" +
                    textCaseID.Text + "'" + ObjectsStatic.sp(Properties.Settings.Default.STRING_ORGANIZATIONID) + ObjectsStatic.sp(Properties.Settings.Default.STRING_LOCATIONID) + ObjectsStatic.sp(textBoxOwnerFirst.Text) + ObjectsStatic.sp(textBoxOwnerLast.Text) + ObjectsStatic.sp(textBoxNamePet.Text) + ObjectsStatic.sp(comboSpecies.Text) + ObjectsStatic.sp(dateTimePicker.Text) + ObjectsStatic.sp(textBoxDoctor.Text) + ")";
                    dataObject.ExecuteQuery(stringSQL);
                }
                //*** Insert Sample ***
                stringSQL = "Insert Into AnalysisRequests (OrganizationID,RequestDate,LocationID,UserName,StudyNumber,CaseID,SpecimenNumber,ConditionID,RequestTypeID,RequestGrams) Values ('" +
                         Properties.Settings.Default.STRING_ORGANIZATIONID + "'" + ObjectsStatic.sp(dateTimePicker.Text) + ObjectsStatic.sp(Properties.Settings.Default.STRING_LOCATIONID) + ObjectsStatic.sp(ObjectsStatic.UserName) + ObjectsStatic.sp("") + ObjectsStatic.sp(textCaseID.Text) + ObjectsStatic.sp(textBarcode.Text) + ObjectsStatic.sp(Properties.Settings.Default.STRING_CONDITIONID) + ObjectsStatic.sp("DIG") + ObjectsStatic.sp(ObjectsStatic.Grams) + ")";
                dataObject.ExecuteQuery(stringSQL);
                //*** Set RequestID and Display Save Message ***
                stringSQL = "Select Max(RequestID) From AnalysisRequests";
                REQUESTID = dataObject.GetDataScalarFromQuery(stringSQL);
                labelMessage.Text = "Patient ID#" + textCaseID.Text + ", Date " + dateTimePicker.Text + " Saved.";
                //*** Set RequestID and Display Save Message ***
                pictureBoxSearch_Click(null, null);
            }
            catch(Exception ex)
            {
                labelMessage.Text = "Error with Patient ID " + textCaseID.Text + ".";
                IsScanEnabled = false;
                string s = ex.ToString();
            }

            ObjectsStatic.CASEID = textCaseID.Text;
            ObjectsStatic.REQUESTID = REQUESTID;
            ObjectsStatic.SPECIMENNUMBER = textBarcode.Text;

            #endregion

            #region "*** Close Data Connection and Set Values ***"

            dataObject.ObjectDispose();
            dataObject = null;
            string saveCaseID = textCaseID.Text;
            //ClearValues();
            textCaseID.Text = saveCaseID;
            labelMessage.Left = panelCases.Left;
            labelMessage.Visible = true;
            //pictureBoxScan.Enabled = IsScanEnabled;
            //pictureBoxScan.Visible = IsScanEnabled;
            pictureBoxScan.Visible = false;
            pictureBoxScan_Click(null, null);
            
            #endregion

        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            GetSelectedSample(sender, e, false);
        }

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellEventArgs e)
        {
            GetSelectedSample(sender, e, false);
        }

        private void dataGridViewEscalated_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            GetSelectedSample(sender, e, true);
        }

        private void dataGridViewEscalated_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GetSelectedSample(sender, e, true);
        }

   
        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            if (IS_NEW_CASE == true)
            {
                if (MessageBox.Show("Are you Sure?  Any New Patient Data will be Lost.", "Data Will be Lost", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    GetHomeScreen();
                }
            }
            else
            {
                GetHomeScreen();
            }
        }

        private void pictureBoxScan_Click(object sender, EventArgs e)
        {
            int integerRepetition = 0;
            if (textCaseID.Text.Length < 3)
            {
                System.Windows.Forms.MessageBox.Show("Enter or Select Valid Case and Specimen");
                return;
            }

            if (IsTraining != true)
            {
                formMessageBox formMessageBox = new formMessageBox("Were any Tapeworm Segments Observed on or within the Stool/Fecal Sample?",ObjectsStatic.MESSAGEBOX_TYPE.MESSAGEBOX_TAPEWORM);
                formMessageBox.ShowDialog();
                formMessageBox = new formMessageBox("Were any Adult Worms Observed on or within the Stool/Fecal Sample?", ObjectsStatic.MESSAGEBOX_TYPE.MESSAGEBOX_ADULTWORMS);
                formMessageBox.ShowDialog();
            }

            while (integerRepetition < ObjectsStatic.INTEGER_REPETITIONS)
            {

                formClinicPlaceSlide objectFormClinicPlaceSlide = new formClinicPlaceSlide();
                objectFormClinicPlaceSlide.ShowDialog();
                Application.DoEvents();
                System.Threading.Thread.Sleep(1500);
                if (ObjectsStatic.IsCaseReady == true)
                {
                    formClinicScan formScan = new formClinicScan();
                    formScan.ShowDialog();
                    Application.DoEvents();
                    formScan = null;
                }
                else
                {
                    GetHomeScreen();
                    return;
                }
                Application.DoEvents();
                integerRepetition++;
            }
            //*** Begin Temp Code ***
            //if (ObjectsStatic.IsCaseReady == true) { GetResultsScreen(); }
            ClearScreen();
            dataGridView.Visible = false;
            panelCases.Visible = false;
            Application.DoEvents();
            if (IsTraining == true) 
            {   
                GetHomeScreen(); 
            }
            else 
            {
                GetResultsScreen(true); 
            }
            //*** End Temp Code ***
            
        }

        private void pictureBoxCaseNew_Click(object sender, EventArgs e)
        {
            GetSearchScreen(true);
        }

        private void pictureBoxCaseSearch_Click(object sender, EventArgs e)
        {
            GetSearchScreen(false);
        }

        private void pictureBoxReports_Click(object sender, EventArgs e)
        {
            new formReport().ShowDialog();
            GetHomeScreen();
        }

        private void pictureBoxResultsAccept_Click(object sender, EventArgs e)
        {
            GetHomeScreen();
        }

        private void pictureBoxResultsView_Click(object sender, EventArgs e)
        {
            formClinicMicroscope formMicroscope = new formClinicMicroscope(@"C:\Temp\QMira\Images\1111.JPG");
            formMicroscope.ShowDialog();
        }

        private void pictureBoxResultsEscalate_Click(object sender, EventArgs e)
        {
            const int INTEGER_SECONDOPINIONLIMIT = 5;
            string stringBody = "Ova & Parasites Report for PatientID: " + textCaseID.Text + ", For " + ObjectsStatic.OrganizationName + "." + Environment.NewLine + Environment.NewLine + 
                "PARASCAN Results are: " + ObjectsStatic.RESULTS_STRING + ".  " + Environment.NewLine + Environment.NewLine +
                "Attached are images of positive particles on the scan, you can view the attached file with any image viewer." + Environment.NewLine + Environment.NewLine;
            int parasiteTypeCount = Enum.GetNames(typeof(FECALPARASITE_TYPE)).Length;
               

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                HelperFunctions objectFunctions = new HelperFunctions();
                List<string> attachmentFilePaths = new List<string>();
                Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
                
                for (int resultValue = 1; resultValue < parasiteTypeCount; resultValue++)
                {
                     string stringSQL = "Select Top " + Convert.ToString(INTEGER_SECONDOPINIONLIMIT) + " FileName From AnalysisRequestsResults Where CaseID = '" + ObjectsStatic.CASEID + "' and RequestID = " + ObjectsStatic.REQUESTID + " and ResultValue = " + Convert.ToString(resultValue) + " Order By ConfidenceValue Desc";
                     SqlDataReader sqlDataReader = dataObject.GetDataReaderFromQuery(stringSQL);
                     while (sqlDataReader.Read())   
                     {
                        attachmentFilePaths.Add(FOLDER_PATH + Convert.ToString(sqlDataReader[0]));
                     }
                     sqlDataReader.Close();
                }

                
                objectFunctions.SendEMail("Ova & Parasite Second Opinion Request From " + ObjectsStatic.OrganizationName, stringBody, attachmentFilePaths.ToArray(), String.Empty);

                labelMessage.Top = pictureBoxLogo.Top + pictureBoxLogo.Height - 5;
                labelMessage.Visible = true;
                labelMessage.Text = "Second Opinion EMail Sent To " + ObjectsStatic.SelectedValue;
                labelMessage.Left = Convert.ToInt16(this.Width / 2) - Convert.ToInt16(labelMessage.Width / 2);
                labelMessage.Refresh();
                labelMessage.Focus();
                dataObject.ObjectDispose();
                Application.DoEvents();
            }
            catch
            {
                labelMessage.Text = "Second Opinion EMail to " + ObjectsStatic.SelectedValue + " Failed.";
                labelMessage.Refresh();
                labelMessage.Focus();
            }
            System.Threading.Thread.Sleep(5000);
            labelMessage.Visible = false;
            Cursor.Current = Cursors.Default;
        }

        private void comboSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            string stringSQL = "Select Breed From AnalysisBreeds where Species = '" + comboSpecies.Text + "' Order By Breed";
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            //comboBreed.DataSource = null;
            //comboBreed.Items.Clear();
            //comboBreed.Refresh();
            List<string> listBreeds = GetStringListFromDataReader(dataObject.GetDataReaderFromQuery(stringSQL));
            //comboBreed.DataSource = listBreeds;
            //comboBreed.Refresh();
            dataObject.ObjectDispose();
            dataObject = null;

        }

        private void pictureBoxSetup_Click(object sender, EventArgs e)
        {
            formLogin objectFormLogin = new formLogin();
            objectFormLogin.ShowDialog();
            Application.DoEvents();
            if (ObjectsStatic.IsAdministrator == true)
            {
                List<string> messageBoxButtonStrings = new List<string>();
                messageBoxButtonStrings.Add("Hospital Setup");
                messageBoxButtonStrings.Add("Archive Cases");
                DialogResult messageBoxResult = ObjectsStatic.MessageBoxResult("Choose Administrative Function", "Administrator", messageBoxButtonStrings);
                if (messageBoxResult == DialogResult.OK)
                {
                    new formSetup().ShowDialog();
                }
                else
                {
                    new formClinicArchive().ShowDialog();
                }
                
            }
        }

        private void pictureBoxArchive_Click(object sender, EventArgs e)
        {
            formLogin objectFormLogin = new formLogin();
            objectFormLogin.ShowDialog();
            Application.DoEvents();
            if (ObjectsStatic.IsAdministrator == true)
            {
                new formSetup().ShowDialog();
            }
        }

        private void pictureBoxHelp_Click(object sender, EventArgs e)
        {
            new formHelp().ShowDialog();
        }

        private void textCaseID_Leave(object sender, EventArgs e)
        {
            if (textCaseID.Text.Length > 4)
            {
                pictureBoxSearch_Click(null, null);
            }
        }

        private void PictureBoxClick(object sender,EventArgs e)
        {

            Button pictureBoxClick = null;
            if (sender is Button)
            {
                pictureBoxClick = sender as Button;
                formImagesViewer formImagesViewer = new formImagesViewer(FOLDER_PATH, Convert.ToInt16(pictureBoxClick.Tag));
                formImagesViewer.ShowDialog();
                Application.DoEvents();
            }
            
        }

        
 
        #endregion

        #region "*** Private Helper Methods ***"
        
        private void CheckParasiteReview(string CaseID, string RequestID)
        {
            ParameterController parameterController = new ParameterController();
          
            int[] parasiteCountMaximums = null;
            int[] parasiteCountMinimums = parameterController.GetParasiteRanges(ref parasiteCountMaximums);
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            
            try
            {
                for (int resultValue = 1; resultValue < parasiteCountMaximums.Length; resultValue++)
                {
                    string stringSQL = "Select Count(*) From AnalysisRequestsResults Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " and ResultValue = " + Convert.ToString(resultValue) + " And (IsReviewed IS NULL or IsReviewed <> 1)";
                    string imageCountString = dataObject.GetDataScalarFromQuery(stringSQL);
                    if (imageCountString == "0") { continue; }
                    int imageCount = Convert.ToInt32(imageCountString);
                     
                    if (SharedFunctionController.IsBetweenValues(imageCount,parasiteCountMinimums[resultValue],parasiteCountMaximums[resultValue]))
                    {
                        //*** Add Result Images to File Path List for Review ***
                        List<string> filePaths = new List<string>();
                        filePaths.Clear();
                        stringSQL = "Select Top 8 FileName From AnalysisRequestsResults Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " and ResultValue = " + Convert.ToString(resultValue) + " Order By ConfidenceValue Desc";
                        SqlDataReader sqlDataReader = dataObject.GetDataReaderFromQuery(stringSQL);
                        while (sqlDataReader.Read())
                        {
                            filePaths.Add(FOLDER_PATH + Convert.ToString(sqlDataReader[0]));
                        }
                        sqlDataReader.Close();

                        //*** Prompt User to Review Images ***
                        string stringMessage = "Is this Positive for " + StringEnum.GetStringValue((FECALPARASITE_TYPE)resultValue) + "?";
                        new formClinicEditResult(filePaths, stringMessage).ShowDialog();
                        Application.DoEvents();
                        if (ObjectsStatic.PARASITE_RESULT == FECALPARASITE_RESULT.No) 
                        {
                             stringSQL = "Delete From AnalysisRequestsResults Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " and ResultValue = " + Convert.ToString(resultValue);
                             dataObject.ExecuteQuery(stringSQL);
                        }
                        else if (ObjectsStatic.PARASITE_RESULT == FECALPARASITE_RESULT.Other)
                        {
                            stringSQL = "Update AnalysisRequestsResults Set ResultValue = " + Convert.ToString(ObjectsStatic.PARASITE_TYPE) + " IsReviewed = 1 Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " and ResultValue = " + Convert.ToString(resultValue);
                             dataObject.ExecuteQuery(stringSQL);
                        }
                        else if (ObjectsStatic.PARASITE_RESULT == FECALPARASITE_RESULT.SecondOpinion)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            HelperFunctions objectFunctions = new HelperFunctions();
                            try
                            {
                                stringSQL = "Delete From AnalysisRequestsResults Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " and ResultValue = " + Convert.ToString(resultValue);
                                dataObject.ExecuteQuery(stringSQL);
                                string[] emailAttachments = filePaths.ToArray();
                                objectFunctions.SendEMail("Ova & Parasite Second Opinion Request From " + ObjectsStatic.OrganizationName, stringMessage, emailAttachments, String.Empty);
                            }
                            catch 
                            {
                                string s = "";
                            }
                        }
                        else
                        {
                            stringSQL = "Update AnalysisRequestsResults Set IsReviewed = 1 Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " and ResultValue = " + Convert.ToString(resultValue);
                            dataObject.ExecuteQuery(stringSQL);
                        }
                   }
                   else if (imageCount < parasiteCountMinimums[resultValue])
                   {
                        stringSQL = "Delete From AnalysisRequestsResults Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " and ResultValue = " + Convert.ToString(resultValue);
                        dataObject.ExecuteQuery(stringSQL);
                   }
                }
               
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
            dataObject.ObjectDispose();
         }


        private string GetSQLRequest()
        {
            return "Select ac.CaseID, ar.RequestDate as Date, ac.CaseFirst as OwnerFirst, ac.CaseLast as OwnerLast, ac.CaseNickname as PetName, ac.CaseSpecies as Species, ac.CaseDoctor as Doctor, ar.RequestID From AnalysisCases ac inner join AnalysisRequests ar on ac.CaseID = ar.CaseID Where " +
                           "(Len(ac.CaseID) > 3 and ac.CaseID = '" + textCaseID.Text + "') or (Len(ac.CaseNickname) > 3 and ac.CaseNickname = '" + textBoxNamePet.Text + "')";
        }

        private void GetSecondOpinions()
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
              
            string stringSQL = "Select ac.CaseID, ar.RequestDate as Date, ac.CaseDoctor as Doctor, ac.CaseFirst as OwnerFirst, ac.CaseLast as OwnerLast, ac.CaseNickname as PetName, ac.CaseSpecies as Species, ar.RequestNotes as Notes, ar.RequestID From AnalysisCases ac inner join AnalysisRequests ar on ac.CaseID = ar.CaseID Where " +
                           "ac.OrganizationID = '" + Properties.Settings.Default.STRING_ORGANIZATIONID + "' and ac.LocationID = '" + Properties.Settings.Default.STRING_LOCATIONID + "' and ar.Completed = 0 and Len(ar.RequestNotes) > 7";
            
            DataTable sqlDataTable = dataObject.GetDataTableFromQuery(stringSQL);
            if (sqlDataTable.Rows.Count > 0)
            {
                labelEscalated.Visible = true;
                dataGridViewEscalated.Visible = true;
                dataGridViewEscalated.DataSource = sqlDataTable;
                dataGridViewEscalated.Refresh();
                dataGridViewEscalated.AutoResizeColumns();
            }
            dataObject.ObjectDispose();
            dataObject = null;

        }
                
        private List<string> GetStringListFromDataReader(SqlDataReader sqlDataReader)
        {
            List<string> list = new List<string>();

            while (sqlDataReader.Read())
            {
                list.Add(Convert.ToString(sqlDataReader[0]));
            }
            return list;
        }

        private void EndAnalysisProcesses()
        {
            string STRING_PROCESSNAME = SharedValueController.PROCESS_NAME;
            try
            {
                foreach (Process objectProcess in Process.GetProcesses())
                {
                    if (objectProcess.ProcessName.Trim().ToUpper().Contains(STRING_PROCESSNAME) == true)
                    {
                        objectProcess.Kill();
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
            catch { }
        }

        private string st(int value)
        {
            return "," + Convert.ToString(value);
        }

        private string st(double value)
        {
            value = Math.Round(value, 4);
            return "," + Convert.ToString(value);
        }

        private string st(string value)
        {
            return ",'" + value + "'";
        }

        private string st(bool value)
        {
            if (value == false)
            {
                return ",0";
            }
            else
            {
                return ",1";
            }
        } 

        #endregion

       

       

      
    }
}
