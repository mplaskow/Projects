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
    public partial class formDataEntry : Form
    {
        public formDataEntry()
        {
            InitializeComponent();
            GetRoles();
            comboBoxValue5.SelectedIndex = 0;
        }

        private void formDataEntry_Load(object sender, EventArgs e)
        {
            this.BackColor = ObjectsStatic.StandardColorBackground;
            
        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBoxSave_Click(object sender, EventArgs e)
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            try
            {
                string stringSQL = "Insert Into AnalysisUsers (UserName, UserFirst, UserLast, UserPassword, UserEMail) Values ('" +
                    textBoxValue1.Text + "'" + ObjectsStatic.sp(textBoxValue2.Text) + ObjectsStatic.sp(textBoxValue3.Text) + ObjectsStatic.sp(textBoxValue4.Text) + ObjectsStatic.sp(textBoxValue6.Text) + ")";
                dataObject.ExecuteQuery(stringSQL);
                stringSQL = "Insert Into AnalysisUsersRolesAssign (UserName, RoleID) Values ('" + textBoxValue1.Text + "'," + comboBoxValue5.SelectedValue.ToString() + ")";
                dataObject.ExecuteQuery(stringSQL);
                labelMessage.Text = "User " + textBoxValue1.Text + " Created.";
            }
            catch 
            {
                labelMessage.Text = "Error Creating User " + textBoxValue1.Text;
            }
            
            dataObject.ObjectDispose();
            dataObject = null;
            GetUsers();
        }

        private void pictureBoxSearch_Click(object sender, EventArgs e)
        {
            GetUsers();
        }

        private void GetUsers()
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "SELECT AnalysisUsers.UserName, AnalysisUsers.UserLast AS Last, AnalysisUsers.UserFirst AS First, AnalysisRoles.RoleName " + 
            "FROM AnalysisUsers INNER JOIN AnalysisUsersRolesAssign ON AnalysisUsers.UserName = AnalysisUsersRolesAssign.UserName INNER JOIN " + 
            "AnalysisRoles ON AnalysisUsersRolesAssign.RoleID = AnalysisRoles.RoleID Where AnalysisUsers.UserName = '" + textBoxValue1.Text + "'";
            DataTable sqlTable = dataObject.GetDataTableFromQuery(stringSQL);
            dataGridView.AutoGenerateColumns = true;
            dataGridView.DataSource = sqlTable;
            dataGridView.Refresh();
            dataObject.ObjectDispose();
            dataObject = null;
        }

        private void GetRoles()
        {
            Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            string stringSQL = "SELECT RoleID, RoleName from AnalysisRoles Where RoleName like '%Vet%' Order by 2";
            DataTable dataTable = dataObject.GetDataTableFromQuery(stringSQL);
            comboBoxValue5.DisplayMember = "RoleName";
            comboBoxValue5.ValueMember = "RoleID";
            comboBoxValue5.DataSource = dataTable;
            comboBoxValue5.Refresh();
            dataObject.ObjectDispose();
            dataObject = null;
        }

        

 
    }
}
