using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Parascan0
{
    public partial class formLogin : Form
    {
        public formLogin()
        {
            InitializeComponent();
            this.Visible = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "SELECT AnalysisRoles.RoleName, AnalysisUsers.UserName FROM AnalysisUsers INNER JOIN " +
                 "AnalysisUsersRolesAssign ON AnalysisUsers.UserName = AnalysisUsersRolesAssign.UserName INNER JOIN " +
                 "AnalysisRoles ON dbo.AnalysisUsersRolesAssign.RoleID = dbo.AnalysisRoles.RoleID where UPPER(AnalysisUsers.UserName) = '" + textBoxUserName.Text.ToUpper() + "'";
            try
            {
                SqlDataReader sqlDataReader = dataObject.GetDataReaderFromQuery(stringSQL);
                while (sqlDataReader.Read() == true)
                {
                    if (Convert.ToString(sqlDataReader["RoleName"]).ToUpper().IndexOf("DMIN") > 0)
                    {
                        ObjectsStatic.IsAdministrator = true;
                    }
                    ObjectsStatic.UserName = Convert.ToString(sqlDataReader["UserName"]);
                }
                if (ObjectsStatic.UserName == String.Empty)
                {
                    labelMessage.Visible = true;
                    labelMessage.Text = "Login Not Successful for User " + textBoxUserName.Text;
                }
                else
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    this.Close();
                }
                sqlDataReader.Dispose();
                sqlDataReader = null;
            }
            catch
            {
                labelMessage.Visible = true;
                labelMessage.Text = "Login Not Successful for User " + textBoxUserName.Text;

            }
            dataObject.ObjectDispose();
            dataObject = null;
            this.Close();
        }

        private void linkLabelPasswordForgot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string stringUserPassword = String.Empty;
            string stringUserEMail = String.Empty;

            try
            {

                if (MessageBox.Show("Send Password to User " + textBoxUserName.Text + "?", "Forgot Password", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
                    string stringSQL = "Select UserPassword, UserEMail from AnalysisUsers Where UserName = '" + textBoxUserName.Text + "'";
                    SqlDataReader sqlDataReader = dataObject.GetDataReaderFromQuery(stringSQL);
                    if (sqlDataReader.Read() == true)
                    {
                        stringUserPassword = Convert.ToString(sqlDataReader["UserPassword"]);
                        stringUserEMail = Convert.ToString(sqlDataReader["UserEMail"]);
                    }
                    if (stringUserPassword.Length < 4) { labelMessage.Text = "No Password found for User " + textBoxUserName.Text; return; }
                    HelperFunctions objectHelperFunctions = new HelperFunctions();
                    string stringEMailSubject = "Forgotten Password for " + textBoxUserName.Text;
                    string stringEMailBody = "Your Password for QMIRA Clinics is: " + stringUserPassword;
                    string[] stringEMailAttachments = new string[0];
                    objectHelperFunctions.SendEMail(stringEMailSubject, stringEMailBody, stringEMailAttachments, stringUserEMail);
                    labelMessage.Text = "Password sent to " + textBoxUserName.Text;
                    sqlDataReader.Dispose();
                    sqlDataReader = null;
                    dataObject.ObjectDispose();
                    dataObject = null;
                }
            }
            catch
            {
                labelMessage.Text = "Error Sending Password, Consult your System Administrator";
            }
        }

        private void linkLabelPasswordReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (MessageBox.Show("Change Password for User " + textBoxUserName.Text + "?", "Change Password", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                new formPassword(textBoxUserName.Text).ShowDialog();
                labelMessage.Text = ObjectsStatic.SelectedValue;
            }
        }

        private void formLogin_Activated(object sender, EventArgs e)
        {
            try
            {
                //formSplashScreen objectFormSplashScreen = new formSplashScreen();
                //objectFormSplashScreen.Dispose();
                //objectFormSplashScreen = null;
            }
            catch { }

            this.Visible = true;
            this.Focus();
            this.BackColor = ObjectsStatic.StandardColorBackground;
            linkLabelPasswordForgot.LinkColor = ObjectsStatic.StandardColorText;
            linkLabelPasswordReset.LinkColor = ObjectsStatic.StandardColorText;
            pictureBoxLogo.Load(ObjectsStatic.IMAGEPATH_LOGO);
            pictureBoxLogo.Refresh();

            this.AcceptButton = buttonOK;
            labelMessage.Visible = false;

            textBoxUserName.Text = "";
            textBoxUserName.Focus();
        }
    }
}
