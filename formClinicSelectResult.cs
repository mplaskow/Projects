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
    public partial class formClinicSelectResult : Form
    {
        public formClinicSelectResult()
        {
            InitializeComponent();
        }

        private void formClinicSelecteResult_Load(object sender, EventArgs e)
        {
            this.BackColor = ObjectsStatic.StandardColorBackground;
            PopulateResults();
        }

        private void PopulateResults()
        {
            const int PIXEL_SPACING = 15;
            int resultCount = FECALPARASITE_TYPE.GetNames(typeof(FECALPARASITE_TYPE)).Length;
            int pixelTop = labelMessage.Top + labelMessage.Height + PIXEL_SPACING;

            
            for (int indexResult = 1; indexResult < resultCount; indexResult++)
            {
                RadioButton objectRadioButton = new RadioButton();
                objectRadioButton.Top = pixelTop;
                objectRadioButton.Left = labelMessage.Left;
                objectRadioButton.ForeColor = Color.White;
                objectRadioButton.Font = new Font("Verdana", 16);
                objectRadioButton.Tag = Convert.ToString(indexResult);
                objectRadioButton.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
                objectRadioButton.Text = StringEnum.GetStringValue((FECALPARASITE_TYPE)indexResult);
                objectRadioButton.AutoSize = true;
                objectRadioButton.TextAlign = ContentAlignment.MiddleLeft;
                objectRadioButton.Visible = true;
                this.Controls.Add(objectRadioButton);
                pixelTop = pixelTop + objectRadioButton.Height + PIXEL_SPACING;
            }
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = (RadioButton)sender;
            ObjectsStatic.PARASITE_TYPE = Convert.ToInt32(r.Tag);
            this.Close();
        }
    }
}
