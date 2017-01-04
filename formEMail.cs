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
    public partial class formEMail : Form
    {
        public formEMail()
        {
            InitializeComponent();
            GetUsers(); 
            this.BackColor = ObjectsStatic.StandardColorBackground;
            this.AcceptButton = buttonOK;
        }

        private void GetUsers()
        {
            comboBoxOrganizationUsers.Items.Clear();
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "Select UserName, UserLast as Last, UserFirst as First, UserEMail as Email From AnalysisUsers Order by 2,3";
            SqlDataReader dataReader = dataObject.GetDataReaderFromQuery(stringSQL);
            while (dataReader.Read() == true)
            {
                string stringValue = Convert.ToString(dataReader[0]) + ", " + Convert.ToString(dataReader[1]) + ", " + Convert.ToString(dataReader[2] + ", " + Convert.ToString(dataReader[3]));
                comboBoxOrganizationUsers.Items.Add(stringValue);
            }
            dataReader.Close();
            dataObject.ObjectDispose();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            ObjectsStatic.SelectedValue = String.Empty;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string stringValue = comboBoxOrganizationUsers.Text;
            int integerCharacter = stringValue.LastIndexOf(',');
            stringValue = stringValue.Substring(integerCharacter + 1, stringValue.Length - integerCharacter - 1);
            ObjectsStatic.SelectedValue = stringValue.Trim();
            this.Close();
        }

    }
}
