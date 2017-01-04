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
    public partial class formMessageBox : Form
    {
        private ObjectsStatic.MESSAGEBOX_TYPE messageBoxType;

        public formMessageBox(string MESSAGE, ObjectsStatic.MESSAGEBOX_TYPE MESSAGE_BOX_TYPE)
        {
            InitializeComponent();
            messageBoxType = MESSAGE_BOX_TYPE;
            textBoxMessage.Text = MESSAGE;
        }

        private void formMessageBox_Load(object sender, EventArgs e)
        {
            this.BackColor = ObjectsStatic.StandardColorBackground;
            textBoxMessage.BackColor = ObjectsStatic.StandardColorBackground;
        }

        private void pictureBoxYes_Click(object sender, EventArgs e)
        {
            SetReturnValue(true);
        }

        private void pictureBoxNo_Click(object sender, EventArgs e)
        {
            SetReturnValue(false);
        }

        private void SetReturnValue(bool RETURNVALUE)
        {
            if (messageBoxType == ObjectsStatic.MESSAGEBOX_TYPE.MESSAGEBOX_TAPEWORM)
            {
                ObjectsStatic.HasTapewormSegments = RETURNVALUE;
            }
            else if (messageBoxType == ObjectsStatic.MESSAGEBOX_TYPE.MESSAGEBOX_ADULTWORMS)
            {
                ObjectsStatic.HasAdultWorms = RETURNVALUE;
            }
            this.Close();
        }
    }
}
