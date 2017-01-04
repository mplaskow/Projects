using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AForge.Math.Geometry;
using AForge.Imaging;
using AForge.Imaging.Filters;
using ImagingController;
using Parascan1.Data;


namespace Parascan0
{
    public partial class formClinicScan : Form, IMessageFilter
    {

        #region "*** Class Level Camera Variables ***"

        // our camera class
        uEye.Camera m_Camera;
        uEye.Defines.DisplayRenderMode m_RenderMode; // render mode
        uEye.Defines.Status statusRet; //status message
        Int32 m_s32FrameCount;
        Boolean m_IsLive;
        private Timer m_UpdateTimer = new Timer();
        private string APPLICATION_PATH = Application.StartupPath;
        private string STRING_FOLDERPATH = Properties.Settings.Default.STRING_PATHCASES;
        private string STRING_FILECONFIG = Properties.Settings.Default.STRING_FILECONFIG;
        Rectangle OldRect;
        Rectangle BoundRect;
        Focus[] FocusSettings = ObjectsStatic.GetFocusSettings();
        Movement[] MovementSettings = ObjectsStatic.GetMovementSettings();

       
        #endregion

        #region "*** Class Level Motor Controller Variables ***"

        private int IMAGE_COLUMNS = Properties.Settings.Default.INTEGER_SLIDECOLUMNS;
        private int IMAGE_ROWS = Properties.Settings.Default.INTEGER_SLIDEROWS;
        private int AUTOFOCUS_COLUMNRANGE = Properties.Settings.Default.INTEGER_AUTOFOCUSCOLUMNS;
        private int AUTOFOCUS_ROWRANGE = Properties.Settings.Default.INTEGER_AUTOFOCUSROWS;
        int AUTOFOCUS_POINTS = Properties.Settings.Default.INTEGER_SLIDEFOCUSPOINTS;
        private int INDEX_MOTOR_PREVIOUS;
        private int[] motorPositions = new int[4];
        DataController dataObject = new DataController();
        //private TextWriter textWriterProcessed = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Logs\\Controller.txt", true);
         
        #endregion

        #region "*** Private Methods for Form Initialization, Activation and Closing Events ***"
        
        public formClinicScan()
        {
            InitializeComponent();
            this.BackColor = ObjectsStatic.StandardColorBackground;
            this.Activate();
            this.Focus();
            this.BringToFront();
            Application.DoEvents();
            this.Text = "Testing Case " + ObjectsStatic.CASEID + "...";
            System.Threading.Thread.Sleep(500);
            
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
            
            // initialize camera object
            // camera is not opened here
            m_Camera = new uEye.Camera();

            m_IsLive = false;
            m_RenderMode = uEye.Defines.DisplayRenderMode.FitToWindow;
      
        }

        private void FormClinicScan_Activated(object sender, EventArgs e)
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

            Application.DoEvents();
           
            GetTipsInterface(Properties.Settings.Default.BOOLEAN_ISLIVESCAN);
            if (pictureBoxDisplay.Visible == false) { GetTipsNext(); }
            Steppers_Initialize();

            if (Steppers_ReadSettings() != ControllerLocation.Ejected) { Steppers_MoveEject(); System.Threading.Thread.Sleep(2000);  }
            if (Steppers_ReadSettings() != ControllerLocation.Loaded) { Steppers_MoveLoad(); }
            if (IsValidSlide() == true) 
            {
                //*** Run Scan ***
                Steppers_Scan(false);
                //EnableMouse();
            }
        }

        void CloseOnStart(object sender, EventArgs e)
        {
            labelMessage.Text = ".NET Runtime Version 3.5.0 is required";
            this.Close();
        }

        private void formClinicScan_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
               // EnableMouse();
                m_Camera.Exit();
                dataObject.ObjectDispose();
                dataObject = null;
            }
            catch { }
        }
         
        #endregion

        #region "*** Private Camera Event Methods ***"

        private uEye.Defines.Status initCamera()
        {

            statusRet = uEye.Defines.Status.NO_SUCCESS;

            statusRet = m_Camera.Init(1 | (Int32)uEye.Defines.DeviceEnumeration.UseDeviceID, pictureBoxDisplay.Handle);

            if (statusRet != uEye.Defines.Status.SUCCESS)
            {
                labelMessage.Text = "Initializing the camera failed";
                Application.DoEvents();
                this.Close();
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
            //}

            //*** Set Camera Parameters ***
            uEye.Configuration.OpenMP.SetEnable(true);
            m_Camera.Timing.PixelClock.Set(96);
            m_Camera.Timing.Framerate.Set(8);
            statusRet = m_Camera.Timing.Exposure.Set(Properties.Settings.Default.DOUBLE_EXPOSURE);
            m_Camera.Gain.Hardware.Scaled.SetBlue(Properties.Settings.Default.INTEGER_BLUE);
            m_Camera.Gain.Hardware.Scaled.SetGreen(Properties.Settings.Default.INTEGER_GREEN);
            m_Camera.Gain.Hardware.Scaled.SetRed(Properties.Settings.Default.INTEGER_RED);
            //m_Camera.EdgeEnhancement.Set(1);
            
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

        #region "*** Private Stepper Event Methods ***"

        private void Steppers_Initialize()
        {
            port.PortName = Properties.Settings.Default.STRING_COMPORT;
            port.BaudRate = 9600;
            int retryAttempts = 0;
            try
            {
                while ((port.IsOpen == false) && (retryAttempts < 10))
                {
                    port.Open();
                    System.Threading.Thread.Sleep(500);
                }
            }
            catch { }
            port.Close();

            try
            {
                while ((port.IsOpen == false) && (retryAttempts < 10))
                {
                    port.Open();
                    System.Threading.Thread.Sleep(500);
                }
            }
            catch { }


            if (port.IsOpen == false)
            {
                //textWriterProcessed.WriteLine("Error Connecting to Port " + Properties.Settings.Default.STRING_COMPORT);
            }

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
            if (stepperDirection == "H") { stepperDirection = MovementSettings[INDEX_MOTOR].MovementDirectionHome; stepperDistance = stepperDistance + 2000; }
            string stepperCommand = "/1" + stepperValues + stepperDirection + Convert.ToString(stepperDistance) + "R";
            //ControllerConnection.Instance.SendComand(stepperCommand);
            port.WriteLine(stepperCommand + "\n\r");
            //*** Temp Code ***
            //textWriterProcessed.WriteLine(stepperCommand);

            System.Threading.Thread.Sleep(stepperWait);
            if (stepperDirection == MovementSettings[INDEX_MOTOR].MovementDirectionPositive) { motorPositions[INDEX_MOTOR] = motorPositions[INDEX_MOTOR] + stepperDistance; }
            else { motorPositions[INDEX_MOTOR] = motorPositions[INDEX_MOTOR] - stepperDistance; }
            if (IsSaveImage == true) { Steppers_SaveImage(); }
        }
        
        private void Steppers_MoveLoad()
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementLoad, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementLoad, MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitLoad, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementLoad, MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementWaitLoad, false, false, false);

            motorPositions[ObjectsStatic.INDEX_MOTOR_X] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementLoad;
            motorPositions[ObjectsStatic.INDEX_MOTOR_Y] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementLoad;
            motorPositions[ObjectsStatic.INDEX_MOTOR_Z] = MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementSlide - MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementLoad;
            Steppers_SaveSettings(ControllerLocation.Loaded);
        }

        private void Steppers_MoveEject()
        {
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Z, "H", MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementSlide, MovementSettings[ObjectsStatic.INDEX_MOTOR_Z].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "H", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementSlide, MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "H", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementSlide, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitSlide, false, false, false);
            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", 3500, 1500, false, false, false);
            Steppers_SaveSettings(ControllerLocation.Ejected);
        }

        private void Steppers_Scan(bool IsSaveMovement)
        {

            #region "*** Declare Variables ***"

            int AUTOFOCUS_ROWS = (int)Math.Round((double)IMAGE_ROWS / (double)AUTOFOCUS_ROWRANGE, 0);
            int AUTOFOCUS_COLUMNS = (int)Math.Round((double)IMAGE_COLUMNS / (double)AUTOFOCUS_COLUMNRANGE, 0);
            double AUTOFOCUS_RANGEY = (double)AUTOFOCUS_COLUMNRANGE / (double)2;
            double AUTOFOCUS_RANGEX = (double)AUTOFOCUS_ROWRANGE / (double)2;
            int AUTOFOCUS_MOVEMENTY = (int)(MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep * AUTOFOCUS_RANGEY);
            int AUTOFOCUS_MOVEMENTX = (int)(MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep * AUTOFOCUS_RANGEX);
            int AUTOFOCUS_WAITY = Convert.ToInt16(MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep * 1.75);
            int AUTOFOCUS_WAITX = Convert.ToInt16(MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitStep * 1.1);
            bool AUTOFOCUS_RESULT = true;
            string AUTOFOCUS_DIRECTION = "P";
            string stepperDirection = "P";
            int stepperMovementY = 0;
            int stepperWaitY = 0;
            bool IsSavedFocus = Properties.Settings.Default.BOOLEAN_ISSAVEFOCUS;
            int focusCount = 0;
            string stringCASEID = ObjectsStatic.CASEID;
            

            STRING_FOLDERPATH = GetCaseFolder() + "\\";
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
                    if (rowAutofocus % 2 == 1) { AUTOFOCUS_DIRECTION = "D"; stepperMovementY = 0; }
                    else { AUTOFOCUS_DIRECTION = "P"; stepperMovementY = MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep * AUTOFOCUS_COLUMNRANGE; stepperWaitY = AUTOFOCUS_WAITY; }
                    //*** Move to Autofocus Point for next Region ***
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, AUTOFOCUS_DIRECTION, AUTOFOCUS_MOVEMENTY, AUTOFOCUS_WAITY, false, IsSaveMovement, false);
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", AUTOFOCUS_MOVEMENTX, AUTOFOCUS_WAITX, false, IsSaveMovement, false);
                    //*** Perform Coarse Autofocus in 1st and 3rd Rows ***
                    if (((rowAutofocus == 0) || (rowAutofocus == 2)) && (columnAutofocus == 0))
                    {
                        AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Coarse], false, false, IsSavedFocus, stringCASEID, rowAutofocus, columnAutofocus);
                        //*** If Coarse Autofocus unsuccessful Try Again ***
                        if (AUTOFOCUS_RESULT == false)
                        {
                            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", AUTOFOCUS_MOVEMENTY, AUTOFOCUS_WAITY, false, IsSaveMovement, false);
                            AUTOFOCUS_RESULT = Steppers_AutoFocus(FocusSettings[(int)FocusType.Coarse], false, false, IsSavedFocus, stringCASEID, rowAutofocus, columnAutofocus);
                            Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "D", AUTOFOCUS_MOVEMENTY, AUTOFOCUS_WAITY, false, IsSaveMovement, false);
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

                    if (pictureBoxDisplay.Visible == false) { GetTipsNext(); }
                    focusCount++;
                    //stringSQL = "INSERT INTO _ControllerMovement (CaseID,MovementIndex,MovementAxis,MovementDirection,PositionZ) VALUES (" +
                    //"'" + ObjectsStatic.CASEID + "'" + ObjectsStatic.sp(focusCount) + ObjectsStatic.sp(ObjectsStatic.INDEX_MOTOR_Z) + ObjectsStatic.sp(stepperDirection) + ObjectsStatic.sp(motorPositions[ObjectsStatic.INDEX_MOTOR_Z]) + ")";
                    //dataObject.ExecuteQuery(stringSQL);
                    //*** Increment Progress Bar and Move to Start of Autofocus Region ***
                    if (progressBarWait.Visible == true) { progressBarWait.Value = progressBarWait.Value + 15; }
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
                            if (progressBarWait.Visible == true) { progressBarWait.Value = progressBarWait.Value + 1; }
                            Application.DoEvents();

                            if (column < (AUTOFOCUS_COLUMNRANGE - 1)) { Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, stepperDirection, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep, false, IsSaveMovement, false); }
                        }
                        //*** Move to next Row within Region ***
                        if (pictureBoxDisplay.Visible == false) { GetTipsNext(); }
                        Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementWaitStep, false, IsSaveMovement, false);
                    }
                    //*** Move to Next Autofocus Region on this Row ***
                    if (stepperMovementY > 0) { Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, AUTOFOCUS_DIRECTION, stepperMovementY, stepperWaitY, false, IsSaveMovement, false); }
                    Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "D", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep * AUTOFOCUS_ROWRANGE, AUTOFOCUS_WAITX, false, IsSaveMovement, false);
                }
                //*** Start Processing Executable ***
                //if (rowAutofocus == 2) { StartProcessingInstance(); }
                //else if (rowAutofocus == 3) { StartProcessingInstance(); StartProcessingInstance(); }
                //*** Move to Next Autofocus Row and First Region ***
                if (AUTOFOCUS_DIRECTION == "P") { Steppers_Move(ObjectsStatic.INDEX_MOTOR_Y, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementStep, MovementSettings[ObjectsStatic.INDEX_MOTOR_Y].MovementWaitStep, false, IsSaveMovement, false); }
                Steppers_Move(ObjectsStatic.INDEX_MOTOR_X, "P", MovementSettings[ObjectsStatic.INDEX_MOTOR_X].MovementStep * AUTOFOCUS_ROWRANGE, AUTOFOCUS_WAITX, false, IsSaveMovement, false);
            }

            for (int index = 0; index < 3; index++) { StartProcessingInstance(); }
            
            //*** Eject Slide ***
            Steppers_MoveEject();

            #endregion

            #region "*** Destroy Camera and Close Form ***"

            m_Camera.Exit();
            m_Camera = null;
            port.Close();
            System.Threading.Thread.Sleep(500);
            this.Close();

            #endregion

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
            if (Focus == FocusSettings[(int)FocusType.Fine]) { focusWait = (int)(Focus.FocusWait * 3.5); } else { focusWait = Convert.ToInt16(Focus.FocusWait * 4.5); }
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

        private void Steppers_SaveImage()
        {
            string fileName = "C:\\Temp\\Images\\000001\\" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_X]) + "_" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_Y]) + "_" + string.Format("{0:D7}", motorPositions[ObjectsStatic.INDEX_MOTOR_Z]) + ".jpg";
            m_Camera.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg, 70);
        }
       
        private void Steppers_SaveSettings(ControllerLocation controllerLocation)
        {
            File.WriteAllText(APPLICATION_PATH + "\\" + Properties.Settings.Default.STRING_FILECONFIG, Convert.ToString(Convert.ToInt16(controllerLocation)));
        }

        private ControllerLocation Steppers_ReadSettings()
        {
            string stringLocation = File.ReadAllText(APPLICATION_PATH + "\\" + Properties.Settings.Default.STRING_FILECONFIG).Trim();
            if (Convert.ToInt16(stringLocation) == Convert.ToInt16(ControllerLocation.Ejected))
            { return ControllerLocation.Ejected; }
            else if (Convert.ToInt16(stringLocation) == Convert.ToInt16(ControllerLocation.Loaded))
            { return ControllerLocation.Loaded; }
            else
            { return ControllerLocation.Scanning;  }
        }
        
        #endregion

        #region "*** Private Helper Methods ***"

        private void GetTipsInterface(bool IsLiveScan)
        {

            if (IsLiveScan == true)
            {
                labelTips.Height = 440;
                labelTips.Width = Convert.ToInt16(this.Width * .5);
                pictureBoxTips.Height = labelTips.Height;
                pictureBoxTips.Width = Convert.ToInt16(labelTips.Width * .8);
                pictureBoxTips.Left = labelTips.Left + labelTips.Width;
                pictureBoxTips.Top = labelTips.Top;
                labelProgress.Left = labelTips.Left;
                progressBarWait.Top = pictureBoxTips.Top + pictureBoxTips.Height + 180;
                progressBarWait.Width = labelTips.Width + pictureBoxTips.Width - labelProgress.Width + 20;
                progressBarWait.Left = labelTips.Left + labelProgress.Width;
                progressBarWait.Maximum = (IMAGE_COLUMNS * IMAGE_ROWS) + (((int)(IMAGE_COLUMNS / AUTOFOCUS_COLUMNRANGE) * (int)(IMAGE_ROWS / AUTOFOCUS_ROWRANGE)) * 15) + 15;
                labelProgress.Top = progressBarWait.Top + Convert.ToInt16(progressBarWait.Height * .5) - Convert.ToInt16(labelProgress.Height * .5);
                pictureBoxDisplay.Visible = false;
            }
            Application.DoEvents();
            labelTips.Visible = IsLiveScan;
            labelProgress.Visible = IsLiveScan;
            progressBarWait.Visible = IsLiveScan;
            pictureBoxTips.Visible = IsLiveScan;
            labelTips.BringToFront();
            Application.DoEvents();
            pictureBoxDisplay.BringToFront();
            Application.DoEvents();
            System.Threading.Thread.Sleep(1000);

        }

        private void GetTipsNext()
        {
            ScreenTip objectTip = HelperClasses.GetNextTip(APPLICATION_PATH);
            labelTips.Text = objectTip.TextString;
            labelTips.Refresh();
            pictureBoxTips.Height = labelTips.Height;
            pictureBoxTips.Load(objectTip.ImageString);
            pictureBoxTips.Refresh();
            objectTip = null;
            Application.DoEvents();
        }

        private string GetCaseFolder()
        {
            DirectoryInfo objectDirectory = new DirectoryInfo(STRING_FOLDERPATH + "\\" + ObjectsStatic.CASEID);
            if (objectDirectory.Exists == false) { objectDirectory.Create(); }
            DirectoryInfo objectSubDirectory = new DirectoryInfo(STRING_FOLDERPATH + "\\" + ObjectsStatic.CASEID + "\\" + SharedValueController.SUBFOLDER_PROCESSED);
            if (objectSubDirectory.Exists == false) { objectSubDirectory.Create(); }
            objectSubDirectory = new DirectoryInfo(STRING_FOLDERPATH + "\\" + ObjectsStatic.CASEID + "\\" + SharedValueController.SUBFOLDER_IMAGES);
            if (objectSubDirectory.Exists == false) { objectSubDirectory.Create(); }
            objectSubDirectory = new DirectoryInfo(STRING_FOLDERPATH + "\\" + ObjectsStatic.CASEID + "\\" + SharedValueController.SUBFOLDER_GIARDIA);
            if (objectSubDirectory.Exists == false) { objectSubDirectory.Create(); }
            return objectDirectory.FullName;
        }

        private void StartProcessingInstance()
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(Properties.Settings.Default.STRING_PROCESSINGEXECUTABLE);
            System.Diagnostics.Process.Start(startInfo);
            System.Threading.Thread.Sleep(500);
            Application.DoEvents();
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x201 || m.Msg == 0x202 || m.Msg == 0x203) return true;
            if (m.Msg == 0x204 || m.Msg == 0x205 || m.Msg == 0x206) return true;
            return false;
        }

        public void EnableMouse()
        {
            Cursor.Clip = OldRect;
            Cursor.Show();
            Application.RemoveMessageFilter(this);
        }

        public void DisableMouse()
        {
            OldRect = Cursor.Clip;
            // Arbitrary location.
            BoundRect = new Rectangle(50, 50, 1, 1);
            Cursor.Clip = BoundRect;
            Cursor.Hide();
            Application.AddMessageFilter(this);
        }

        private bool IsBetweenValues(int valueCurrent, int value1, int value2)
        {
            if ((valueCurrent >= value1) && (valueCurrent <= value2))
            {
                return true;
            }
            return false;
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
                const int INTEGER_AREA_MAXIMUM = 110000;

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
                        Steppers_MoveEject();
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
            Steppers_MoveEject();
            Application.DoEvents();
            Application.Exit();
            return false;
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

       private void pictureBoxBack_Click(object sender, EventArgs e)
       {
           this.Close();
       }

       
       
      
    }
}

