using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parascan0
{
    public partial class formTestResults : Form
    {
        private int EVENT_COUNT = 0; 

        public formTestResults()
        {
            InitializeComponent();
        }

        private void formTestResults_Load(object sender, EventArgs e)
        {
            PopulateForm();
        }

        private void PopulateForm()
        {
            string[] stringParasites = new string[9] { "TOX", "HOOK", "TRICH", "CCI", "CCE", "GIAR", "STRON", "TAEN", "DIPYL" };
            const int PIXEL_HEIGHT = 25;
            const int PIXEL_WIDTH = 90;
            const int PIXEL_SPACER = 15;
            Font fontRadioButton = new Font("Arial Narrow", 14);
            int pixelLeft = 0;
           
            for (int indexResult = 0; indexResult < 1; indexResult++)
            {
                Label newLabel = new Label();
                newLabel.Left = 10;
                newLabel.Top =  buttonEnter.Top + buttonEnter.Height + PIXEL_HEIGHT + (indexResult * (5 * PIXEL_HEIGHT));
                newLabel.Font = new Font("Verdana", 16);
                string stringResultIndex = Convert.ToString(indexResult + 1);
                newLabel.Name = "labelResult" + stringResultIndex;
                newLabel.Text = "Result " + stringResultIndex + ":";
                newLabel.AutoSize = true;
                Controls.Add(newLabel);
                pixelLeft = newLabel.Left + newLabel.Width + 10;
                foreach (string stringParasite in stringParasites)
                {
                    CheckBox checkBoxResult = null;
                    for (int indexDensity = 0; indexDensity < 4; indexDensity++)
                    {
                        checkBoxResult = new CheckBox();
                        checkBoxResult.Left = pixelLeft;
                        checkBoxResult.Width = PIXEL_WIDTH;
                        checkBoxResult.Top = newLabel.Top + (indexDensity * PIXEL_HEIGHT);
                        if (indexDensity > 0) { checkBoxResult.Top = checkBoxResult.Top + PIXEL_SPACER;  }
                        checkBoxResult.Text = stringParasite + Convert.ToString(indexDensity + 1);
                        checkBoxResult.Font = fontRadioButton;
                        checkBoxResult.Name = "checkBoxResult" + stringParasite + Convert.ToString(indexDensity + 1) + "_" + stringResultIndex;
                        Controls.Add(checkBoxResult);
                    }
                    pixelLeft = pixelLeft + checkBoxResult.Width;
                }
           }
        }

        #region "*** Private Methods for Form Events ***"

        private void buttonNEGATIVE_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
        #endregion

        private void buttonEnter_Click(object sender, EventArgs e)
        {
            string resultString = "";
            foreach (Control c in this.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox objectCheckBox = (CheckBox)c;
                    if (objectCheckBox.Checked == true)
                    {
                        resultString = resultString + "_" + objectCheckBox.Text;
                    }
                }
            }
            if (resultString == "") { MessageBox.Show("No Result Selected"); return; }
            if (System.Windows.Forms.MessageBox.Show(resultString + "?","Result Correct?",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ObjectsStatic.CASEID = ObjectsStatic.CASEID + resultString;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.GetTotalMemory(true);
                this.Close();
            }
        }

       
       
     
        
    }
}
