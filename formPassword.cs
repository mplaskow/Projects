using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Parascan0
{
    public partial class formPassword : Form
    {
        string UserName;
        public formPassword(string stringUserName)
        {
            UserName = stringUserName;
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "Select UserPassword from AnalysisUsers Where UserName = '" + UserName + "'";
            string stringPassword = dataObject.GetDataScalarFromQuery(stringSQL);
            if (stringPassword.Length < 4)
            {
                ObjectsStatic.SelectedValue = "User Name " + UserName + " is not Found";
                this.Close();
            }
            else if (stringPassword != textBoxPasswordCurrent.Text)
            {
                labelMessage.Text = "Current Password Incorrect";
            }
            else if (textBoxPasswordNew.Text != textBoxPasswordVerify.Text)
            {
                labelMessage.Text = "Password Verification Incorrect";
            }
            else
            {
                stringSQL = "Update AnalysisUsers Set UserPassword = '" + textBoxPasswordNew.Text + "' Where UserName = '" + UserName + "'";
                dataObject.ExecuteQuery(stringSQL);
                ObjectsStatic.SelectedValue = "Password Updated for " + UserName;
                this.Close();
            }
            dataObject.ObjectDispose();
            dataObject = null;
           
        }
    }
}
