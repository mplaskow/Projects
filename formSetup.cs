using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Parascan1.Data;
using System.Data.SqlClient;

namespace Parascan0
{
    public partial class formSetup : Form
    {

        #region "*** Initialization and Load Events ***"

        public formSetup()
        {
            InitializeComponent();
            GetOrganization();
            GetUsers();
            GetStates();
            this.BackColor = ObjectsStatic.StandardColorBackground;
            try
            {
                pictureBoxOrganizationLogo.Image = Image.FromFile(Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHIMAGES + "\\" + Properties.Settings.Default.STRING_ORGANIZATIONIMAGE);
            }
            catch { }
            pictureBoxLogo.Load(ObjectsStatic.IMAGEPATH_LOGO);
            pictureBoxLogo.Refresh();
        }

        #endregion

        #region "*** Private Methods for Form Events ***"

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxNewUser_Click(object sender, EventArgs e)
        {
            new formDataEntry().ShowDialog();
            GetUsers();
        }

        private void pictureBoxLogoFromLibrary_Click(object sender, EventArgs e)
        {
            pictureBoxOrganizationLogo.Image = null;
            ObjectsStatic.OrganizationName = textBoxOrganizationName.Text;
            formImageSelection formIcon = new formImageSelection();
            formIcon.ShowDialog();
            pictureBoxOrganizationLogo.Image = Image.FromFile(Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHIMAGES + "\\" + Properties.Settings.Default.STRING_ORGANIZATIONIMAGE);
            pictureBoxOrganizationLogo.Refresh();
        }

        private void pictureBoxLogoFromFile_Click(object sender, EventArgs e)
        {
            openFileDialogOrganizationLogo.InitialDirectory = "c:\\temp\\QMIRA";
            openFileDialogOrganizationLogo.ShowDialog();
            try
            {
                pictureBoxOrganizationLogo.Image = Image.FromFile(openFileDialogOrganizationLogo.FileName);
                pictureBoxOrganizationLogo.Image.Save(Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHIMAGES + "\\" + Properties.Settings.Default.STRING_ORGANIZATIONIMAGE);
            }
            catch
            {
                MessageBox.Show("Invalid Image");
            }
        }

        private void pictureBoxSave_Click(object sender, EventArgs e)
        {
            SaveOrganization();
            this.Close();
        }

        #endregion

        #region "*** Private Methods to Save Values ***"

        private void SaveOrganization()
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "UPDATE AnalysisOrganizations SET OrganizationName = '" + textBoxOrganizationName.Text.Trim() + "', " +
                 "OrganizationAccount = '" + textBoxOrganizationAccount.Text.Trim() + "', " +
                "OrganizationContact = '" + textBoxOrganizationContact.Text.Trim() + "', " +
                "OrganizationAddress1 = '" + textBoxOrganizationAddress.Text.Trim() + "', " +
                "OrganizationCity = '" + textBoxOrganizationCity.Text.Trim() + "', " +
                "OrganizationState = '" + comboBoxOrganizationState.Text + "', " +
                "OrganizationZipCode = '" + textBoxOrganizationZipCode.Text.Trim() + "', " +
                "OrganizationEMail = '" + textBoxOrganizationEMail.Text.Trim() + "', " +
                "OrganizationPhone = '" + textBoxOrganizationPhone.Text.Trim() + "' " +
                "WHERE OrganizationID = '" + Properties.Settings.Default.STRING_ORGANIZATIONID + "'";
            dataObject.ExecuteQuery(stringSQL);
            dataObject.ObjectDispose();
            dataObject = null;
        }

        #endregion

        #region "*** Private Methods to Populate Values ***"

        private void GetOrganization()
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "SELECT OrganizationID, OrganizationName, OrganizationAccount, OrganizationContact, OrganizationAddress1, OrganizationCity, OrganizationState, OrganizationZipCode, OrganizationEMail, OrganizationPhone " +
                "FROM AnalysisOrganizations Where OrganizationID = '" + Properties.Settings.Default.STRING_ORGANIZATIONID + "'";
            SqlDataReader dataReader = dataObject.GetDataReaderFromQuery(stringSQL);
            if (dataReader.Read() == true)
            {
                textBoxOrganizationName.Text = Convert.ToString(dataReader["OrganizationName"]);
                textBoxOrganizationAccount.Text = Convert.ToString(dataReader["OrganizationAccount"]);
                textBoxOrganizationContact.Text = Convert.ToString(dataReader["OrganizationContact"]);
                textBoxOrganizationAddress.Text = Convert.ToString(dataReader["OrganizationAddress1"]);
                textBoxOrganizationCity.Text = Convert.ToString(dataReader["OrganizationCity"]);
                comboBoxOrganizationState.Text = Convert.ToString(dataReader["OrganizationState"]);
                textBoxOrganizationZipCode.Text = Convert.ToString(dataReader["OrganizationZipCode"]);
                textBoxOrganizationEMail.Text = Convert.ToString(dataReader["OrganizationEMail"]);
                textBoxOrganizationPhone.Text = Convert.ToString(dataReader["OrganizationPhone"]);
            }
            dataReader.Close();
            dataObject.ObjectDispose();
            dataObject = null;
        }

        private void GetUsers()
        {
            comboBoxOrganizationUsers.Items.Clear();
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "Select UserName, UserLast as Last, UserFirst as First, UserEMail as Email From AnalysisUsers Order by 2,3";
            SqlDataReader dataReader = dataObject.GetDataReaderFromQuery(stringSQL);
            while (dataReader.Read() == true)
            {
                string stringValue = Convert.ToString(dataReader[1]) + ", " + Convert.ToString(dataReader[0]);
                comboBoxOrganizationUsers.Items.Add(stringValue);
            }
            dataReader.Close();
            dataObject.ObjectDispose();
            dataObject = null;
        }

        private void GetStates()
        {
            #region "*** Populate Combo Box with States ***"
            comboBoxOrganizationState.Items.Clear();
            comboBoxOrganizationState.Refresh();
            comboBoxOrganizationState.Items.Add("AL");
            comboBoxOrganizationState.Items.Add("AK");
            comboBoxOrganizationState.Items.Add("AZ");
            comboBoxOrganizationState.Items.Add("AR");
            comboBoxOrganizationState.Items.Add("CA");
            comboBoxOrganizationState.Items.Add("CO");
            comboBoxOrganizationState.Items.Add("CT");
            comboBoxOrganizationState.Items.Add("DE");
            comboBoxOrganizationState.Items.Add("FL");
            comboBoxOrganizationState.Items.Add("GA");
            comboBoxOrganizationState.Items.Add("HI");
            comboBoxOrganizationState.Items.Add("ID");
            comboBoxOrganizationState.Items.Add("IL");
            comboBoxOrganizationState.Items.Add("IN");
            comboBoxOrganizationState.Items.Add("IA");
            comboBoxOrganizationState.Items.Add("KS");
            comboBoxOrganizationState.Items.Add("KY");
            comboBoxOrganizationState.Items.Add("LA");
            comboBoxOrganizationState.Items.Add("ME");
            comboBoxOrganizationState.Items.Add("MD");
            comboBoxOrganizationState.Items.Add("MA");
            comboBoxOrganizationState.Items.Add("MI");
            comboBoxOrganizationState.Items.Add("MN");
            comboBoxOrganizationState.Items.Add("MS");
            comboBoxOrganizationState.Items.Add("MO");
            comboBoxOrganizationState.Items.Add("MT");
            comboBoxOrganizationState.Items.Add("NE");
            comboBoxOrganizationState.Items.Add("NV");
            comboBoxOrganizationState.Items.Add("NH");
            comboBoxOrganizationState.Items.Add("NJ");
            comboBoxOrganizationState.Items.Add("NM");
            comboBoxOrganizationState.Items.Add("NY");
            comboBoxOrganizationState.Items.Add("NC");
            comboBoxOrganizationState.Items.Add("ND");
            comboBoxOrganizationState.Items.Add("OH");
            comboBoxOrganizationState.Items.Add("OK");
            comboBoxOrganizationState.Items.Add("OR");
            comboBoxOrganizationState.Items.Add("PA");
            comboBoxOrganizationState.Items.Add("RI");
            comboBoxOrganizationState.Items.Add("SC");
            comboBoxOrganizationState.Items.Add("SD");
            comboBoxOrganizationState.Items.Add("TN");
            comboBoxOrganizationState.Items.Add("TX");
            comboBoxOrganizationState.Items.Add("UT");
            comboBoxOrganizationState.Items.Add("VT");
            comboBoxOrganizationState.Items.Add("VA");
            comboBoxOrganizationState.Items.Add("WA");
            comboBoxOrganizationState.Items.Add("WV");
            comboBoxOrganizationState.Items.Add("WI");
            comboBoxOrganizationState.Items.Add("WY");
            #endregion
        }

        #endregion

    }
}
