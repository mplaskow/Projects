using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ImagingController;
using System.IO.Ports;
using AForge;
using AForge.Math.Geometry;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Parascan1.Data;


namespace Parascan0
{
    public partial class MainForm : Form
    {

        #region "*** Declare Camera Variables ***"

        // our camera class
        uEye.Camera m_Camera;
        uEye.Defines.DisplayRenderMode m_RenderMode; // render mode
        uEye.Defines.Status statusRet; //status message
        Int32 m_s32FrameCount;
        Boolean m_IsLive;
        private Timer m_UpdateTimer = new Timer();
        private string STRING_FOLDERPATH = "";
        Focus[] FocusSettings = ObjectsStatic.GetFocusSettings();
        private FocusPoint[] AutofocusPoints;
        Movement[] MovementSettings = ObjectsStatic.GetMovementSettings();
       
        #endregion

        #region "*** Class Level Motor Controller Variables ***"

        private int IMAGE_COLUMNS = Properties.Settings.Default.INTEGER_SLIDECOLUMNS;
        private int IMAGE_ROWS = Properties.Settings.Default.INTEGER_SLIDEROWS;
        private int AUTOFOCUS_COLUMNRANGE = Properties.Settings.Default.INTEGER_AUTOFOCUSCOLUMNS;
        private int AUTOFOCUS_ROWRANGE = Properties.Settings.Default.INTEGER_AUTOFOCUSROWS;
       
        int AUTOFOCUS_POINTS = Properties.Settings.Default.INTEGER_SLIDEFOCUSPOINTS;
        private int INDEX_MOTOR_PREVIOUS = -1;
        const int AUTOFOCUS_COLUMNS = 3;
        private int[] motorPositions = new int[4];
        Parascan1.Data.DataController dataObject = new Parascan1.Data.DataController();
            
 
        #endregion
       
        #region "*** Private Methods for Form Initialization, Activation and Closing Events ***"

        public MainForm()
        {
            InitializeComponent();
            this.BackColor = ObjectsStatic.StandardColorBackground;

            // Check Runtime Version
            Version verMin = new Version(3, 5);
            Boolean bOk = false;
            foreach (Version ver in InstalledDotNetVersions())
            {
                if (ver >= verMin)
                {
                    bOk = true;
                    break;
                }
            }

            if (!bOk)
            {
                this.Load += CloseOnStart;
            }

            pictureBoxDisplay.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBoxDisplay.Width = panelDisplay.Width;
            this.pictureBoxDisplay.Height = panelDisplay.Height;

            // initialize camera object
            // camera is not opened here
            m_Camera = new uEye.Camera();

            m_IsLive = false;
            m_RenderMode = uEye.Defines.DisplayRenderMode.FitToWindow;

            //dataObject.ExecuteQuery("truncate table _ControllerMovement");
            motorPositions[ObjectsStatic.INDEX_MOTOR_X] = MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementLoad;
            motorPositions[ObjectsStatic.INDEX_MOTOR_Y] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementLoad;
            motorPositions[ObjectsStatic.INDEX_MOTOR_Z] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementLoad;
           
     
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {

           
            uEye.Defines.Status statusRet;
            statusRet = initCamera();

            if (statusRet == uEye.Defines.Status.SUCCESS)
            {
                // start capture
                statusRet = m_Camera.Acquisition.Capture();
                if (statusRet != uEye.Defines.Status.SUCCESS)
                {
                    labelMessage.Text = "Starting live video failed";
                }
                else
                {
                    // everything is ok
                    m_IsLive = true;
                }
            }

            // cleanup on any camera error
            if (statusRet != uEye.Defines.Status.SUCCESS && m_Camera.IsOpened)
            {
                m_Camera.Exit();
            }

            Steppers_Initialize();

           // m_Camera.AutoFeatures.Software.WhiteBalance.SetEnable(true);
            //Application.DoEvents();
           // System.Threading.Thread.Sleep(4000);
            //m_Camera.Gain.Hardware.Scaled.SetBlue(1);
            

         }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // EnableMouse();
                m_Camera.Exit();
                port.Close();
            }
            catch { }
            dataObject.ObjectDispose();
            dataObject = null;
        }

        void CloseOnStart(object sender, EventArgs e)
        {
            labelMessage.Text = ".NET Runtime Version 3.5.0 is required";
            this.Close();
        }

        #endregion

        #region "*** Private Helper Methods for .NET Versions ***"

        public static System.Collections.ObjectModel.Collection<Version> InstalledDotNetVersions()
        {
            System.Collections.ObjectModel.Collection<Version> versions = new System.Collections.ObjectModel.Collection<Version>();
            Microsoft.Win32.RegistryKey NDPKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
            if (NDPKey != null)
            {
                string[] subkeys = NDPKey.GetSubKeyNames();
                foreach (string subkey in subkeys)
                {
                    GetDotNetVersion(NDPKey.OpenSubKey(subkey), subkey, versions);
                    GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Client"), subkey, versions);
                    GetDotNetVersion(NDPKey.OpenSubKey(subkey).OpenSubKey("Full"), subkey, versions);
                }
            }
            return versions;
        }

        private static void GetDotNetVersion(Microsoft.Win32.RegistryKey parentKey, string subVersionName, System.Collections.ObjectModel.Collection<Version> versions)
        {
            if (parentKey != null)
            {
                string installed = Convert.ToString(parentKey.GetValue("Install"));
                if (installed == "1")
                {
                    string version = Convert.ToString(parentKey.GetValue("Version"));
                    if (string.IsNullOrEmpty(version))
                    {
                        if (subVersionName.StartsWith("v"))
                            version = subVersionName.Substring(1);
                        else
                            version = subVersionName;
                    }

                    Version ver = new Version(version);

                    if (!versions.Contains(ver))
                        versions.Add(ver);
                }
            }
        }

        #endregion

        #region "*** Private Camera Event Methods ***"
        private uEye.Defines.Status initCamera()
        {
            statusRet = uEye.Defines.Status.NO_SUCCESS;
            
            statusRet = m_Camera.Init(0, pictureBoxDisplay.Handle);
                 
            if (statusRet != uEye.Defines.Status.SUCCESS)
            {
                labelMessage.Text = "Initializing the camera failed";
                return statusRet;
            }

                
            statusRet = m_Camera.Memory.Allocate();
            if (statusRet != uEye.Defines.Status.SUCCESS)
            {
                labelMessage.Text = "Allocating memory failed";
                return statusRet;
            }

            // set event
            m_Camera.EventFrame += onFrameEvent;

            // reset framecount
            m_s32FrameCount = 0;

            // start update timer for our statusbar
            m_UpdateTimer.Start();

            uEye.Types.SensorInfo sensorInfo;
            m_Camera.Information.GetSensorInfo(out sensorInfo);

            pictureBoxDisplay.SizeMode = PictureBoxSizeMode.Normal;
            toolStripStatusLabelCamera.Text = sensorInfo.SensorName;    
           
            //*** Set Camera Parameters ***
            uEye.Configuration.OpenMP.SetEnable(true);
            m_Camera.Timing.PixelClock.Set(96);
            m_Camera.Timing.Framerate.Set(5);
            statusRet = m_Camera.Timing.Exposure.Set(Properties.Settings.Default.DOUBLE_EXPOSURE);
            m_Camera.Gain.Hardware.Scaled.SetBlue(Properties.Settings.Default.INTEGER_BLUE);
            m_Camera.Gain.Hardware.Scaled.SetGreen(Properties.Settings.Default.INTEGER_GREEN);
            m_Camera.Gain.Hardware.Scaled.SetRed(Properties.Settings.Default.INTEGER_RED);
            m_Camera.EdgeEnhancement.Set(1);

                            
            return statusRet;
        }

        private void onFrameEvent(object sender, EventArgs e)
        {
            // convert sender object to our camera object
            uEye.Camera camera = sender as uEye.Camera;

            if (camera.IsOpened)
            {
                uEye.Defines.DisplayMode mode;
                camera.Display.Mode.Get(out mode);

                // only display in dib mode
                if (mode == uEye.Defines.DisplayMode.DiB)
                {
                    Int32 s32MemID;
                    camera.Memory.GetActive(out s32MemID);
                    camera.Memory.Lock(s32MemID);

                    camera.Memory.Unlock(s32MemID);
                    camera.Display.Render(s32MemID, m_RenderMode);
                }

                ++m_s32FrameCount;
            }
        }

        private void OnDisplayChanged(object sender, EventArgs e)
        {
            uEye.Defines.DisplayMode displayMode;
            m_Camera.Display.Mode.Get(out displayMode);

            // set scaling options
            if (displayMode != uEye.Defines.DisplayMode.DiB)
            {
                if (m_RenderMode == uEye.Defines.DisplayRenderMode.DownScale_1_2)
                {
                    m_RenderMode = uEye.Defines.DisplayRenderMode.Normal;

                    this.pictureBoxDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Left;

                    // get image size
                    System.Drawing.Rectangle rect;
                    m_Camera.Size.AOI.Get(out rect);

                    this.pictureBoxDisplay.Width = rect.Width;
                    this.pictureBoxDisplay.Height = rect.Height;
                }
                else
                {
                    m_Camera.DirectRenderer.SetScaling(m_RenderMode == uEye.Defines.DisplayRenderMode.FitToWindow);
                }
            }
            else
            {
                if (m_RenderMode != uEye.Defines.DisplayRenderMode.FitToWindow)
                {
                    this.pictureBoxDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Left;

                    // get image size
                    System.Drawing.Rectangle rect;
                    m_Camera.Size.AOI.Get(out rect);

                    if (m_RenderMode != uEye.Defines.DisplayRenderMode.Normal)
                    {

                        this.pictureBoxDisplay.Width = rect.Width / 2;
                        this.pictureBoxDisplay.Height = rect.Height / 2;
                    }
                    else
                    {
                        this.pictureBoxDisplay.Width = rect.Width;
                        this.pictureBoxDisplay.Height = rect.Height;
                    }
                }
            }

            
        }

        #endregion

        #region "*** Private Methods for Full Axis Movement ***"

        private void buttonXAxis_Click(object sender, EventArgs e)
        {
           // ControllerConnection.Instance.SendComand("/1aM2L75V30000fZ50000R");
            //for (int index = 0; index < 100; index++)
            //{
                //Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementSlide, 1500, false, false, false);
                //Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementSlide, 1500, false, false, false);
            //}
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "H", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementSlide, 1500, false, false, false);
        }

        private void buttonYAxis_Click(object sender, EventArgs e)
        {
            //ControllerConnection.Instance.SendComand("/1aM1L75V30000fZ400000R");
            //for (int index = 0; index < 100; index++)
            //{
                //Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide, 2500, false, false, false);
                //Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide, 2500, false, false, false);
            //}

        }

        private void buttonZAxis_Click(object sender, EventArgs e)
        {
            //ControllerConnection.Instance.SendComand("/1aM3L75V30000f1Z100000R");
            for (int index = 0; index < 100; index++)
            {
                Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "D", 15, 300, false, false, false);
                Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "P", 15, 300, false, false, false);
            }
       

        }

        #endregion

        #region "*** Private Methods for Exact Movement ***"

        private void buttonYLeft_Click(object sender, EventArgs e)
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "D", GetMovement(), 1000, false, false, false);
        }

        private void buttonYRight_Click(object sender, EventArgs e)
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", GetMovement(), 1000, false, false, false);
        }

        private void buttonXLeft_Click(object sender, EventArgs e)
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", GetMovement(), 1000, false, false, false);
        }

        private void buttonXRight_Click(object sender, EventArgs e)
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", GetMovement(), 1000, false, false, false);
        }

        private void buttonZUp_Click(object sender, EventArgs e)
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "D", GetMovement(), 1000, false, false, false);
        }

        private void buttonZDown_Click(object sender, EventArgs e)
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "P", GetMovement(), 1000, false, false, false);
        }

        #endregion

        #region "*** Private Methods for Form Events ***"

        private void Steppers_Initialize()
        {
            
            try
            {
                port.PortName = Properties.Settings.Default.STRING_COMPORT;
                port.BaudRate = 9600;
                int retryAttempts = 0;
                while ((port.IsOpen == false) && (retryAttempts < 10))
                {
                    port.Open();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch { }
            


            if (port.IsOpen == false)
            {
                //textWriterProcessed.WriteLine("Error Connecting to Port " + Properties.Settings.Default.STRING_COMPORT);
            }

        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            Steppers_Scan(true);
        }
       
        private void buttonSnapshot_Click(object sender, EventArgs e)
        {
            if (STRING_FOLDERPATH == "")
            {
                STRING_FOLDERPATH = GetCaseFolder() + "\\";
            }
            SetFormButtons(false);
            m_Camera.Image.Save(STRING_FOLDERPATH + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_X]) + "_" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_Y]) + "_" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_Z]) + ".JPG", System.Drawing.Imaging.ImageFormat.Jpeg, 70);
            SetFormButtons(true);
        }

        private void buttonEject_Click(object sender, EventArgs e)
        {
            SetFormButtons(false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "H", MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementSlide, MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "H", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementSlide, MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "H", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", 3500, 1500, false, false, false);
            Steppers_SaveSettings(ControllerLocation.Ejected);
            SetFormButtons(true);
       }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            
            SetFormButtons(false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementLoad, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementLoad, MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitLoad, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementLoad, MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementWaitLoad, false, false, false);

            motorPositions[ObjectsStatic.INDEX_MOTOR_X] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementLoad;
            motorPositions[ObjectsStatic.INDEX_MOTOR_Y] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementLoad;
            motorPositions[ObjectsStatic.INDEX_MOTOR_Z] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementLoad;

            bool validSlide = IsValidSlide();

            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", 500, 500, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", 500, 500, false, false, false);
           

            Steppers_SaveSettings(ControllerLocation.Loaded);
            SetFormButtons(true);
            
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            m_Camera.AutoFeatures.Software.WhiteBalance.SetEnable(false);
            m_Camera.Exit();
            m_Camera = null;
            Application.DoEvents();
            this.Close();
        }

        private void buttonAutofocus_Click(object sender, EventArgs e)
        {
            bool AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Coarse], false, false, false, ObjectsStatic.CASEID, 1, 1);
                        
            System.Threading.Thread.Sleep(500);

            AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Fine], false, false, false, ObjectsStatic.CASEID, 1, 1);
        }


        #endregion
        
        #region "*** Private Helper Methods ***"

        private int GetMovement()
        {
            int integerDistance = 0;
            try
            {
                integerDistance = Convert.ToInt32(comboBoxDistance.Text);
            }
            catch { }
            if (textBoxDistance.Text.Trim() != "") { integerDistance = Convert.ToInt32(textBoxDistance.Text); }
            return integerDistance;
        }

        private bool IsBetweenValues(int valueCurrent, int value1, int value2)
        {
            if ((valueCurrent >= value1) && (valueCurrent <= value2))
            {
                return true;
            }
            return false;
        }

        private void SetFormButtons(bool IsEnabled)
        {
            buttonAutofocus.Enabled = IsEnabled;
            buttonEject.Enabled = IsEnabled;
            buttonExit.Enabled = IsEnabled;
            buttonLoad.Enabled = IsEnabled;
            buttonScan.Enabled = IsEnabled;
            buttonSnapshot.Enabled = IsEnabled;
            buttonXAxis.Enabled = IsEnabled;
            buttonXLeft.Enabled = IsEnabled;
            buttonXRight.Enabled = IsEnabled;
            buttonYAxis.Enabled = IsEnabled;
            buttonYLeft.Enabled = IsEnabled;
            buttonYRight.Enabled = IsEnabled;
            buttonZAxis.Enabled = IsEnabled;
            buttonZDown.Enabled = IsEnabled;
            buttonZUp.Enabled = IsEnabled;
            Application.DoEvents();
        }

        private string GetCaseFolder()
        {
            DirectoryInfo objectDirectory = new DirectoryInfo(Properties.Settings.Default.STRING_PATHCASES + "\\" + ObjectsStatic.CASEID);
            if (objectDirectory.Exists == false) { objectDirectory.Create(); }
            return objectDirectory.FullName;
        }

        #endregion

        #region "*** Private Stepper Event Methods ***"

        private void Steppers_Scan(bool IsSaveMovement)
        {
            #region "*** Declare Variables ***"


            int AUTOFOCUS_ROWS = (int)Math.Round((double)IMAGE_ROWS / (double)AUTOFOCUS_ROWRANGE, 0);
            int AUTOFOCUS_COLUMNS = (int)Math.Round((double)IMAGE_COLUMNS / (double)AUTOFOCUS_COLUMNRANGE, 0);
            double AUTOFOCUS_RANGEY = (double)AUTOFOCUS_COLUMNRANGE / (double)2;
            double AUTOFOCUS_RANGEX = (double)AUTOFOCUS_ROWRANGE / (double)2;

            int AUTOFOCUS_MOVEMENTY = (int)(MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep * AUTOFOCUS_RANGEY);
            int AUTOFOCUS_MOVEMENTX = (int)(MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep * AUTOFOCUS_RANGEX);
            int AUTOFOCUS_WAITY = Convert.ToInt16(MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep * 1.25);
            int AUTOFOCUS_WAITX = Convert.ToInt16(MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitStep * 1.1);
            bool AUTOFOCUS_RESULT = true;
            string AUTOFOCUS_DIRECTION = "P";
            string stepperDirection = "P";
            int stepperMovementY = 0;
            int stepperWaitY = 0;
            int focusCount = 0;

            STRING_FOLDERPATH = GetCaseFolder() + "\\";
            ObjectsStatic.CASEFOLDER = STRING_FOLDERPATH;
            string stringCASEID = ObjectsStatic.CASEID;
            bool IsSavedFocus = Properties.Settings.Default.BOOLEAN_ISSAVEFOCUS;
            DateTime startTime = System.DateTime.Now;
            //(1000);

            int columnCurrent = 0;
            int rowCurrent = 0;
            Steppers_SaveSettings(ControllerLocation.Scanning);

            #endregion

            #region "*** Scan Slide ***"

            for (int rowAutofocus = 0; rowAutofocus < AUTOFOCUS_ROWS; rowAutofocus++)
            {
                for (int columnAutofocus = 0; columnAutofocus < AUTOFOCUS_COLUMNS; columnAutofocus++)
                {
                    //*** Set Autofocus Movement Direction and Distance ***
                    if (rowAutofocus % 2 == 1) { AUTOFOCUS_DIRECTION = "D"; stepperMovementY = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep * (AUTOFOCUS_COLUMNRANGE - 1); stepperWaitY = AUTOFOCUS_WAITY; }
                    else { AUTOFOCUS_DIRECTION = "P"; stepperMovementY = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep; stepperWaitY = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep; }
                    //*** Move to Autofocus Point for next Region ***
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, AUTOFOCUS_DIRECTION, AUTOFOCUS_MOVEMENTY, AUTOFOCUS_WAITY, false, IsSaveMovement, false);
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", AUTOFOCUS_MOVEMENTX, AUTOFOCUS_WAITX, false, IsSaveMovement, false);
                    //*** Perform Coars Autofocus in 1st and 3rd Rows ***
                    if (((rowAutofocus == 0) || (rowAutofocus == 2)) && (columnAutofocus == 0))
                    {
                        AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Coarse], false, false, IsSavedFocus, stringCASEID, rowAutofocus, columnAutofocus);
                        //*** If Coarse Autofocus unsuccessful Try Again ***
                        if (AUTOFOCUS_RESULT == false)
                        {
                            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep, false, IsSaveMovement, false);
                            AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Coarse], false, false, IsSavedFocus, stringCASEID, rowAutofocus, columnAutofocus);
                            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep, false, IsSaveMovement, false);
                        }
                    }
                    else { AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Medium], false, false, IsSavedFocus, stringCASEID, rowAutofocus, columnAutofocus); }
                    //*** Always perform Fine Autofocus and Move Back to Start Region Scan ***
                    AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Fine], false, false, IsSavedFocus, stringCASEID, rowAutofocus, columnAutofocus);
                    if (AUTOFOCUS_RESULT == false)
                    {
                        Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep, false, IsSaveMovement, false);
                        AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Fine], false, false, IsSavedFocus, stringCASEID, rowAutofocus, columnAutofocus);
                        Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep, false, IsSaveMovement, false);
                    }
                    //*** Temp Code ***
                    focusCount++;
                    //stringSQL = "INSERT INTO _ControllerMovement (CaseID,MovementIndex,MovementAxis,MovementDirection,PositionZ) VALUES (" +
                    //"'" + ObjectsStatic.CASEID + "'" + ObjectsStatic.sp(focusCount) + ObjectsStatic.sp(ObjectsStatic.INDEX_MOTOR_Z) + ObjectsStatic.sp(stepperDirection) + ObjectsStatic.sp(motorPositions[ObjectsStatic.INDEX_MOTOR_Z]) + ")";
                    //dataObject.ExecuteQuery(stringSQL);


                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "D", AUTOFOCUS_MOVEMENTY, AUTOFOCUS_WAITY, false, IsSaveMovement, false);
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", AUTOFOCUS_MOVEMENTX, AUTOFOCUS_WAITX, false, IsSaveMovement, false);
                    //*** Loop through Region and Capture Images ***
                    for (int row = 0; row < AUTOFOCUS_ROWRANGE; row++)
                    {
                        rowCurrent = (rowAutofocus * AUTOFOCUS_ROWRANGE) + row + 1;
                        for (int column = 0; column < AUTOFOCUS_COLUMNRANGE; column++)
                        {
                            if (row % 2 == 1) { stepperDirection = "D"; } else { stepperDirection = "P"; }
                            //*** Check for Direction and Set Current Column
                            if (AUTOFOCUS_DIRECTION == "P")
                            {
                                if (stepperDirection == "P")
                                { columnCurrent = (columnAutofocus * AUTOFOCUS_COLUMNRANGE) + column + 1; }
                                else { columnCurrent = ((columnAutofocus + 1) * AUTOFOCUS_COLUMNRANGE) - column; }
                            }
                            else
                            {
                                if (stepperDirection == "P") { columnCurrent = IMAGE_COLUMNS - ((columnAutofocus + 1) * AUTOFOCUS_COLUMNRANGE) + column + 1; }
                                else { columnCurrent = IMAGE_COLUMNS - (columnAutofocus * AUTOFOCUS_COLUMNRANGE) - column; }
                            }
                            //*** Save Image and Check for last Column in Region ***
                            string filePath = STRING_FOLDERPATH + ObjectsStatic.CASEID + "_" + string.Format("{0:D3}", rowCurrent) + "_" + string.Format("{0:D3}", columnCurrent) + ".JPG";
                            m_Camera.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg, 70);
                            FileInfo fileInfo = new FileInfo(filePath);
                            while (ObjectsStatic.IsFileLocked(fileInfo) == true) { }
                           Application.DoEvents();
                            if (column < (AUTOFOCUS_COLUMNRANGE - 1)) { Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, stepperDirection, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep, false, IsSaveMovement, false); }
                        }
                        //*** Move to next Row within Region ***
                       Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitStep, false, IsSaveMovement, false);
                    }
                    //*** Move to Next Autofocus Region on this Row ***
                    if (stepperMovementY > 0) { Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, AUTOFOCUS_DIRECTION, stepperMovementY, stepperWaitY, false, IsSaveMovement, false); }
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep * AUTOFOCUS_ROWRANGE, AUTOFOCUS_WAITX, false, IsSaveMovement, false);
                }
                //StartProcessingInstance();
                //*** Move to Next Autofocus Row and First Region ***
                Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep * AUTOFOCUS_ROWRANGE, AUTOFOCUS_WAITX, false, IsSaveMovement, false);
            }

            //*** Eject Slide ***
            buttonEject_Click(null, null);

            #endregion

            #region "*** Destroy Camera and Close Form ***"

            m_Camera.Exit();
            m_Camera = null;
            this.Close();

            #endregion
        }

        private void Steppers_Move(int INDEX_MOTOR, string stepperDirection, int stepperDistance, int stepperWait, bool IsSaveImage, bool IsSaveMovement, bool IsBacklash)
        {
            string stepperValues = "";
            if (INDEX_MOTOR != INDEX_MOTOR_PREVIOUS)
            {
                stepperValues = "aM" + Convert.ToString(INDEX_MOTOR + 1) + "L" + MovementSettings[INDEX_MOTOR].MovementAcceleration + "V" + MovementSettings[INDEX_MOTOR].MovementVelocity;
                INDEX_MOTOR_PREVIOUS = INDEX_MOTOR;
            }
            if (stepperDistance == 0) { return; }
            if (stepperDirection == "H") { stepperDirection = MovementSettings[INDEX_MOTOR].MovementDirectionHome; }

            string stepperCommand = "/1" + stepperValues + stepperDirection + Convert.ToString(stepperDistance) + "R";
            //ControllerConnection.Instance.SendComand(stepperCommand);
            port.WriteLine(stepperCommand + "\n\r");
            
            System.Threading.Thread.Sleep(stepperWait);
            if (stepperDirection == MovementSettings[INDEX_MOTOR].MovementDirectionPositive) { motorPositions[INDEX_MOTOR] = motorPositions[INDEX_MOTOR] + stepperDistance; }
            else { motorPositions[INDEX_MOTOR] = motorPositions[INDEX_MOTOR] - stepperDistance; }
            if (IsSaveImage == true) { Steppers_SaveImage(); }
            if (IsSaveMovement == true)
            {
                string stringSQL = "INSERT INTO _ControllerMovement (MovementAxis,MovementDirection,MovementDistance,PositionX,PositionY,PositionZ) VALUES (" +
                    Convert.ToString(INDEX_MOTOR) + ObjectsStatic.sp(stepperDirection) + ObjectsStatic.sp(stepperDistance) + ObjectsStatic.sp(motorPositions[ObjectsStatic.INDEX_MOTOR_X]) + ObjectsStatic.sp(motorPositions[ObjectsStatic.INDEX_MOTOR_Y]) + ObjectsStatic.sp(motorPositions[ObjectsStatic.INDEX_MOTOR_Z]) + ")";
                dataObject.ExecuteQuery(stringSQL);
            }
        }

        private void Steppers_SaveImage()
        {
            string fileName = "C:\\Temp\\Images\\000001\\" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_X]) + "_" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_Y]) + "_" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_Z]) + ".jpg";
            m_Camera.Image.Save(fileName,System.Drawing.Imaging.ImageFormat.Jpeg,70);
        }

        private void Steppers_SaveSettings(ControllerLocation controllerLocation)
        {
            File.WriteAllText(Application.StartupPath + "\\" + Properties.Settings.Default.STRING_FILECONFIG, Convert.ToString(Convert.ToInt16(controllerLocation)));
        }

        private bool Steppers_AutoFocus(Focus Focus, bool IsSavedImages, bool IsSavedMovement, bool IsSavedFocus, string CASEID, int autofocusRow, int autofocusColumn)
        {


            #region "*** Declare Variables ***"

            double focusScoreHighest = 0;
            double focusScoreCurrent = 0;
            const double FOCUS_SCORE_MINIMUM_FINE = 5.5;
            const double FOCUS_SCORE_MINIMUM_COARSE = 4.5;
            const int FOCUS_PIXELS_MINIMUM = 2400;
            int focusRangeMedian = motorPositions[ObjectsStatic.INDEX_MOTOR_Z];
            int focusPositionCurrent = motorPositions[ObjectsStatic.INDEX_MOTOR_Z];
            int focusPositionHighest = focusPositionCurrent + Focus.FocusRangeUp;
            int focusPositionHighestScore = focusPositionCurrent;
            int focusPositionLowest = focusPositionCurrent - Focus.FocusRangeDown;
            int focusPositionStart = focusPositionCurrent;
            int focusPositionDifference = focusRangeMedian;
            int focusWait = 0;
            int focusPixels = 0;
            int focusPixelsHighest = 0;
            string focusDirection = "P";

            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "D", Focus.FocusRangeUp, Focus.FocusWait * 2, IsSavedImages, IsSavedMovement, false);

            #endregion

            #region "*** Loop Through from Top Focus Point to Bottom ***"

            while (focusPositionCurrent >= focusPositionLowest)
            {
                Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "P", Focus.FocusStep, Focus.FocusWait, IsSavedImages, IsSavedMovement, false);

                focusPositionCurrent = motorPositions[ObjectsStatic.INDEX_MOTOR_Z];
                Bitmap bitmapFocus;

                Int32 s32MemId;
                m_Camera.Memory.GetActive(out s32MemId);
                m_Camera.Memory.Lock(s32MemId);
                m_Camera.Memory.ToBitmap(s32MemId, out bitmapFocus);
                if (Focus == FocusSettings[(int)FocusType.Fine]) { focusScoreCurrent = Autofocus.CalculateFocusValueGrayscaleFine(bitmapFocus, CASEID, autofocusRow, autofocusColumn, focusPositionCurrent, IsSavedFocus); }
                else { focusScoreCurrent = Autofocus.CalculateFocusValueGrayscale(bitmapFocus, ref focusPixels); }

                if (focusScoreCurrent > focusScoreHighest)
                {
                    focusScoreHighest = focusScoreCurrent;
                    focusPositionHighestScore = focusPositionCurrent;
                    focusPixelsHighest = focusPixels;
                }
                m_Camera.Memory.Unlock(s32MemId);
                bitmapFocus.Dispose();
                focusPositionCurrent = focusPositionCurrent - Focus.FocusStep;
            }

            #endregion

            #region "*** Check for Direction and Distance to Move Z Axis ***"

            if (focusPositionHighestScore > focusPositionCurrent)
            {
                focusPositionDifference = focusPositionHighestScore - focusPositionCurrent - Focus.FocusStep;
                focusDirection = "D";
            }
            else
            {
                focusPositionDifference = focusPositionCurrent - focusPositionHighestScore - Focus.FocusStep;
                focusDirection = "P";
            }

            if (focusPositionDifference <= 0) { return true; }

            #endregion

            #region "*** Check for Invalid Focus and Move Z Axis to best Focal Point ***"\

            //*** Check for Invalid Focus ***
            if (((Focus == FocusSettings[(int)FocusType.Fine]) && (focusScoreHighest < FOCUS_SCORE_MINIMUM_FINE)) || ((Focus == FocusSettings[(int)FocusType.Coarse]) && (focusScoreHighest < FOCUS_SCORE_MINIMUM_COARSE)))
            {
                Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "D", focusPositionDifference, Focus.FocusWait * 2, IsSavedImages, IsSavedMovement, false);
                return false;
            }


            //*** Begin Motherboard ***
            if (Focus == FocusSettings[(int)FocusType.Fine]) { focusWait = (int)(Focus.FocusWait * 2.5); } else { focusWait = Convert.ToInt16(Focus.FocusWait * 3); }
            //*** End Motherboard ***

            //*** Begin Laptop ***
            //if (Focus == FocusSettings[(int)FocusType.Fine]) { focusWait = (int)(Focus.FocusWait * 2); } else { focusWait = Convert.ToInt16(Focus.FocusWait * 2.5); }
            //*** End Laptop ***

            //*** Begin Tablet ***
            //if (Focus == FocusSettings[(int)FocusType.Fine]) { focusWait = (int)(Focus.FocusWait * 3); } else { focusWait = Convert.ToInt16(Focus.FocusWait * 4); }
            //if (Focus != FocusSettings[(int)FocusType.Fine])
            //{
            //    focusPositionDifference = focusPositionDifference + 120;
            //}
            //*** End Tablet ***

            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, focusDirection, focusPositionDifference, focusWait, IsSavedImages, IsSavedMovement, false);

            motorPositions[ObjectsStatic.INDEX_MOTOR_Z] = focusPositionHighestScore;

            return true;

            #endregion

        }


        #endregion

        private void buttonWhiteBalance_Click(object sender, EventArgs e)
        {
            m_Camera.AutoFeatures.Software.WhiteBalance.SetEnable(true);
            labelMessage.Text = "White Balance Enabled";
        }

        private bool IsValidSlide()
        {
            try { if (Properties.Settings.Default.BOOLEAN_ISVALIDATEDSLIDE != true) { return true; } }
            catch { return true; }

            try
            {

                System.Threading.Thread.Sleep(2000);

                bool IsLoaded = false;
                string stepperDirectionY = "D";
                int loadMovementsY = 0;
                int loadMovementsX = 1;
                Bitmap bitmapFocus;
                Int32 s32MemId;
                const int INTEGER_AREA_MINIMUM = 11000;
                const int INTEGER_AREA_MAXIMUM = 90000;

                const double DOUBLE_FULLNESS_MINIMUM = .55;

                m_Camera.Memory.GetActive(out s32MemId);
                m_Camera.Memory.Lock(s32MemId);
                m_Camera.Memory.ToBitmap(s32MemId, out bitmapFocus);
                ResizeBilinear objectResizeBilinear = new ResizeBilinear((int)(bitmapFocus.Width * .1), (int)(bitmapFocus.Height * .1));
                ExtractChannel objectExtractChannel = new ExtractChannel(RGB.B);

                while (IsLoaded == false)
                {

                    m_Camera.Memory.GetActive(out s32MemId);
                    m_Camera.Memory.Lock(s32MemId);
                    m_Camera.Memory.ToBitmap(s32MemId, out bitmapFocus);
                    Bitmap bitmapGrayscale = objectResizeBilinear.Apply(bitmapFocus);
                    bitmapGrayscale = objectExtractChannel.Apply(bitmapGrayscale);
                    bitmapGrayscale = new OtsuThreshold().Apply(bitmapGrayscale);
                    new Invert().ApplyInPlace(bitmapGrayscale);
                    AForge.Imaging.BlobCounter objectBlobCounter = new BlobCounter(bitmapGrayscale);
                    objectBlobCounter.ObjectsOrder = ObjectsOrder.Area;
                    objectBlobCounter.ProcessImage(bitmapGrayscale);
                    try
                    {
                        Blob blobFiduciary = objectBlobCounter.GetObjectsInformation()[0];
                        string fileName = string.Format("{0:D3}", loadMovementsX) + "_" + string.Format("{0:D3}", loadMovementsY) + "_" + Convert.ToString(Math.Round(blobFiduciary.Fullness, 2)).Replace(".", "_") + "_" + string.Format("{0:D4}", blobFiduciary.Area);
                        //bitmapFocus.Save("c:\\temp\\image_" + fileName + ".JPG");
                        //bitmapGrayscale.Save("c:\\temp\\image_" + fileName + "_Binary.JPG");
                        if ((blobFiduciary.Fullness >= DOUBLE_FULLNESS_MINIMUM) && (SharedFunctionController.IsBetweenValues(blobFiduciary.Area, INTEGER_AREA_MINIMUM, INTEGER_AREA_MAXIMUM)))
                        {
                            m_Camera.Memory.Unlock(s32MemId);
                            IsLoaded = true;
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //bitmapFocus.Save("c:\\temp\\image_" + string.Format("{0:D3}", loadMovementsX) + "_" + string.Format("{0:D3}", loadMovementsY) + ".JPG");
                        //bitmapGrayscale.Save("c:\\temp\\image_" + string.Format("{0:D3}", loadMovementsX) + "_" + string.Format("{0:D3}", loadMovementsY) + "_Binary.JPG");
                        //MessageBox.Show(ex.ToString());
                    }
                    m_Camera.Memory.Unlock(s32MemId);
                    bitmapFocus.Dispose();
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, stepperDirectionY, 350, 500, false, false, false);
                    loadMovementsY++;
                    if (loadMovementsY > 1)
                    {
                        loadMovementsX++;
                        Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", 450, 500, false, false, false);
                        loadMovementsY = 0;
                        if (stepperDirectionY == "D") { stepperDirectionY = "P"; } else { stepperDirectionY = "D"; }
                    }
                    if (loadMovementsX > 5)
                    {
                        MessageBox.Show("This is not a Valid QMIRA Slide!");
                        buttonEject_Click(null,null);
                        Application.DoEvents();
                        Application.Exit();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.ToString());
                return false;
            }
            MessageBox.Show("This is not a Valid QMIRA Slide!");
            buttonEject_Click(null, null);
            Application.DoEvents();
            Application.Exit();
            return false;
        }

        public static double GetQuadrilateralVariance(List<IntPoint> edgePoints)
        {
            double circleVariance = .001;
            SimpleShapeChecker objectShapeChecker = new SimpleShapeChecker();
            objectShapeChecker.RelativeDistortionLimit = (float)circleVariance;
            try
            {
                while ((objectShapeChecker.IsQuadrilateral(edgePoints) == false) && (circleVariance < 3))
                {
                    circleVariance = circleVariance + .001;
                    objectShapeChecker.RelativeDistortionLimit = (float)circleVariance;
                }
            }
            catch { circleVariance = .25; }
            return circleVariance;

        }
        
       

    }
}
