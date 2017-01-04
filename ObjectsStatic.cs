using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ImagingController;
using System.IO;
using Parascan1.Data;

namespace Parascan0
{
    public static class ObjectsStatic
    {

        #region "*** Declare Public Application Variables ***"

        public static string OrganizationName = "";
        public static string UserName = Properties.Settings.Default.STRING_DEFAULTUSERNAME;
        public static string SelectedValue;
        public static string CASEID = "001";
        public static string CASEFOLDER = "";
        public static string REQUESTID;
        public static string SPECIMENNUMBER;
        public static bool HasTapewormSegments = false;
        public static bool HasAdultWorms = false;
        public static string RESULTS_STRING = "";
        public static int Grams = 0;
        public static bool IsAdministrator = false;
        public static bool IsCaseReady = false;
        public static Color StandardColorBackground = Color.FromArgb(37, 64, 143);
        public static Color StandardColorText = Color.FromArgb(48, 179, 224);
        public const string STRING_EQUINE1 = "HOR";
        public const string STRING_EQUINE2 = "EQU";
        public static int INTEGER_REPETITIONS = Properties.Settings.Default.INTEGER_REPETITIONS;
        public static string IMAGEPATH_LOGO = Application.StartupPath + @"\" + Properties.Settings.Default.STRING_PATHIMAGES + @"\" + Properties.Settings.Default.STRING_LOGOIMAGE;
        public static int INDEX_MOTOR_Y = (int)MotorAxis.Y;
        public static int INDEX_MOTOR_X = (int)MotorAxis.X;
        public static int INDEX_MOTOR_Z = (int)MotorAxis.Z;
        public static FECALPARASITE_RESULT PARASITE_RESULT;
        public static int PARASITE_TYPE = 0;
        
        
        public enum MESSAGEBOX_TYPE
        {
            MESSAGEBOX_TAPEWORM = 0,
            MESSAGEBOX_ADULTWORMS = 1
        }


        #endregion

        #region "*** Public String Functions ***"
        
        public static string sp(string value)
        {
            return ",'" + value + "'";
        }

        public static string sp(int value)
        {
            return "," + Convert.ToString(value);
        }

        #endregion
        
        public static DialogResult MessageBoxResult(string userMessage, string userCaption, List<string> userButtons)
        {
            DialogResult returnValue;
            if (userButtons.Count == 3)
            {
                MessageBoxManager.Yes = userButtons[0];
                MessageBoxManager.No = userButtons[1];
                MessageBoxManager.Cancel = userButtons[2];
                MessageBoxManager.Register();
                returnValue = MessageBox.Show(userMessage, userCaption, MessageBoxButtons.YesNoCancel);
            }
            else
            {
                MessageBoxManager.OK = userButtons[0];
                MessageBoxManager.Cancel = userButtons[1];
                MessageBoxManager.Register();
                returnValue = MessageBox.Show(userMessage, userCaption, MessageBoxButtons.OKCancel);
            }
            MessageBoxManager.Unregister();
            return returnValue;
        }
        
        public static Focus[] GetFocusSettings()
        {
            return new FocusSettings(CameraType.uEye, ObjectiveType.TenX).Settings;
        }

        public static Movement[] GetMovementSettings()
        {
            return new MovementSettings(CameraType.uEye, ObjectiveType.TenX).Settings;
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        
    }

     
}
