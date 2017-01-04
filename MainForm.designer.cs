namespace Parascan0
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.PictureBox pictureBoxLogo;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelDisplay = new System.Windows.Forms.Panel();
            this.pictureBoxDisplay = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelFailed = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelFrameCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelCamera = new System.Windows.Forms.ToolStripStatusLabel();
            this.textBoxDistance = new System.Windows.Forms.TextBox();
            this.buttonScan = new System.Windows.Forms.Button();
            this.buttonEject = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonZDown = new System.Windows.Forms.Button();
            this.buttonZUp = new System.Windows.Forms.Button();
            this.buttonXRight = new System.Windows.Forms.Button();
            this.buttonXLeft = new System.Windows.Forms.Button();
            this.buttonYRight = new System.Windows.Forms.Button();
            this.buttonYLeft = new System.Windows.Forms.Button();
            this.comboBoxDistance = new System.Windows.Forms.ComboBox();
            this.buttonZAxis = new System.Windows.Forms.Button();
            this.buttonXAxis = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelParascan1 = new System.Windows.Forms.Label();
            this.buttonYAxis = new System.Windows.Forms.Button();
            this.buttonSnapshot = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonAutofocus = new System.Windows.Forms.Button();
            this.buttonWhiteBalance = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.port = new System.IO.Ports.SerialPort(this.components);
            pictureBoxLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(pictureBoxLogo)).BeginInit();
            this.panelDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.Image = global::Parascan0.Properties.Resources.imageQMIRALogo;
            pictureBoxLogo.Location = new System.Drawing.Point(65, 669);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new System.Drawing.Size(155, 38);
            pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBoxLogo.TabIndex = 37;
            pictureBoxLogo.TabStop = false;
            // 
            // panelDisplay
            // 
            this.panelDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDisplay.AutoScroll = true;
            this.panelDisplay.Controls.Add(this.pictureBoxDisplay);
            this.panelDisplay.Location = new System.Drawing.Point(271, 30);
            this.panelDisplay.Name = "panelDisplay";
            this.panelDisplay.Size = new System.Drawing.Size(818, 685);
            this.panelDisplay.TabIndex = 0;
            // 
            // pictureBoxDisplay
            // 
            this.pictureBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxDisplay.BackColor = System.Drawing.Color.Gainsboro;
            this.pictureBoxDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxDisplay.Image = global::Parascan0.Properties.Resources.uEyeLogo;
            this.pictureBoxDisplay.Location = new System.Drawing.Point(0, 3);
            this.pictureBoxDisplay.Name = "pictureBoxDisplay";
            this.pictureBoxDisplay.Size = new System.Drawing.Size(919, 800);
            this.pictureBoxDisplay.TabIndex = 1;
            this.pictureBoxDisplay.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFPS,
            this.toolStripStatusLabelFailed,
            this.toolStripStatusLabelFrameCount,
            this.toolStripStatusLabelCamera});
            this.statusStrip.Location = new System.Drawing.Point(0, 711);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip.Size = new System.Drawing.Size(1132, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // toolStripStatusLabelFPS
            // 
            this.toolStripStatusLabelFPS.Name = "toolStripStatusLabelFPS";
            this.toolStripStatusLabelFPS.Size = new System.Drawing.Size(29, 17);
            this.toolStripStatusLabelFPS.Text = ":FPS";
            this.toolStripStatusLabelFPS.Visible = false;
            // 
            // toolStripStatusLabelFailed
            // 
            this.toolStripStatusLabelFailed.Name = "toolStripStatusLabelFailed";
            this.toolStripStatusLabelFailed.Size = new System.Drawing.Size(41, 17);
            this.toolStripStatusLabelFailed.Text = ":Failed";
            this.toolStripStatusLabelFailed.Visible = false;
            // 
            // toolStripStatusLabelFrameCount
            // 
            this.toolStripStatusLabelFrameCount.Name = "toolStripStatusLabelFrameCount";
            this.toolStripStatusLabelFrameCount.Size = new System.Drawing.Size(48, 17);
            this.toolStripStatusLabelFrameCount.Text = ":Frames";
            this.toolStripStatusLabelFrameCount.Visible = false;
            // 
            // toolStripStatusLabelCamera
            // 
            this.toolStripStatusLabelCamera.Name = "toolStripStatusLabelCamera";
            this.toolStripStatusLabelCamera.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabelCamera.Size = new System.Drawing.Size(51, 17);
            this.toolStripStatusLabelCamera.Text = "Camera:";
            this.toolStripStatusLabelCamera.Visible = false;
            // 
            // textBoxDistance
            // 
            this.textBoxDistance.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxDistance.Location = new System.Drawing.Point(139, 349);
            this.textBoxDistance.Name = "textBoxDistance";
            this.textBoxDistance.Size = new System.Drawing.Size(115, 31);
            this.textBoxDistance.TabIndex = 48;
            this.textBoxDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonScan
            // 
            this.buttonScan.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonScan.Location = new System.Drawing.Point(55, 235);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(159, 51);
            this.buttonScan.TabIndex = 47;
            this.buttonScan.Text = "Scan";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // buttonEject
            // 
            this.buttonEject.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEject.Location = new System.Drawing.Point(2, 294);
            this.buttonEject.Name = "buttonEject";
            this.buttonEject.Size = new System.Drawing.Size(133, 42);
            this.buttonEject.TabIndex = 46;
            this.buttonEject.Text = "Eject";
            this.buttonEject.UseVisualStyleBackColor = true;
            this.buttonEject.Click += new System.EventHandler(this.buttonEject_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.Location = new System.Drawing.Point(61, 615);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(162, 48);
            this.buttonExit.TabIndex = 45;
            this.buttonExit.Text = "EXIT";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonZDown
            // 
            this.buttonZDown.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZDown.Location = new System.Drawing.Point(139, 484);
            this.buttonZDown.Name = "buttonZDown";
            this.buttonZDown.Size = new System.Drawing.Size(115, 36);
            this.buttonZDown.TabIndex = 44;
            this.buttonZDown.Text = "Z Down";
            this.buttonZDown.UseVisualStyleBackColor = true;
            this.buttonZDown.Click += new System.EventHandler(this.buttonZDown_Click);
            // 
            // buttonZUp
            // 
            this.buttonZUp.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZUp.Location = new System.Drawing.Point(18, 484);
            this.buttonZUp.Name = "buttonZUp";
            this.buttonZUp.Size = new System.Drawing.Size(115, 36);
            this.buttonZUp.TabIndex = 43;
            this.buttonZUp.Text = "Z Up";
            this.buttonZUp.UseVisualStyleBackColor = true;
            this.buttonZUp.Click += new System.EventHandler(this.buttonZUp_Click);
            // 
            // buttonXRight
            // 
            this.buttonXRight.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXRight.Location = new System.Drawing.Point(139, 439);
            this.buttonXRight.Name = "buttonXRight";
            this.buttonXRight.Size = new System.Drawing.Size(115, 36);
            this.buttonXRight.TabIndex = 42;
            this.buttonXRight.Text = "X Down";
            this.buttonXRight.UseVisualStyleBackColor = true;
            this.buttonXRight.Click += new System.EventHandler(this.buttonXRight_Click);
            // 
            // buttonXLeft
            // 
            this.buttonXLeft.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXLeft.Location = new System.Drawing.Point(18, 439);
            this.buttonXLeft.Name = "buttonXLeft";
            this.buttonXLeft.Size = new System.Drawing.Size(115, 36);
            this.buttonXLeft.TabIndex = 41;
            this.buttonXLeft.Text = "X Up";
            this.buttonXLeft.UseVisualStyleBackColor = true;
            this.buttonXLeft.Click += new System.EventHandler(this.buttonXLeft_Click);
            // 
            // buttonYRight
            // 
            this.buttonYRight.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonYRight.Location = new System.Drawing.Point(139, 397);
            this.buttonYRight.Name = "buttonYRight";
            this.buttonYRight.Size = new System.Drawing.Size(115, 36);
            this.buttonYRight.TabIndex = 40;
            this.buttonYRight.Text = "Y Right";
            this.buttonYRight.UseVisualStyleBackColor = true;
            this.buttonYRight.Click += new System.EventHandler(this.buttonYRight_Click);
            // 
            // buttonYLeft
            // 
            this.buttonYLeft.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonYLeft.Location = new System.Drawing.Point(18, 397);
            this.buttonYLeft.Name = "buttonYLeft";
            this.buttonYLeft.Size = new System.Drawing.Size(115, 36);
            this.buttonYLeft.TabIndex = 39;
            this.buttonYLeft.Text = "Y Left";
            this.buttonYLeft.UseVisualStyleBackColor = true;
            this.buttonYLeft.Click += new System.EventHandler(this.buttonYLeft_Click);
            // 
            // comboBoxDistance
            // 
            this.comboBoxDistance.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxDistance.FormattingEnabled = true;
            this.comboBoxDistance.Items.AddRange(new object[] {
            "5",
            "15",
            "30",
            "50",
            "100",
            "250",
            "500",
            "1000",
            "2000",
            "5000",
            "7500",
            "1000",
            "12500",
            "15000",
            "18000",
            "20000",
            "25000",
            "28000",
            "35000",
            "40000",
            "50000",
            "60000",
            "17500",
            "18000"});
            this.comboBoxDistance.Location = new System.Drawing.Point(18, 349);
            this.comboBoxDistance.Name = "comboBoxDistance";
            this.comboBoxDistance.Size = new System.Drawing.Size(115, 31);
            this.comboBoxDistance.TabIndex = 38;
            // 
            // buttonZAxis
            // 
            this.buttonZAxis.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZAxis.Location = new System.Drawing.Point(56, 178);
            this.buttonZAxis.Name = "buttonZAxis";
            this.buttonZAxis.Size = new System.Drawing.Size(159, 51);
            this.buttonZAxis.TabIndex = 36;
            this.buttonZAxis.Text = "Z Motion Test";
            this.buttonZAxis.UseVisualStyleBackColor = true;
            this.buttonZAxis.Click += new System.EventHandler(this.buttonZAxis_Click);
            // 
            // buttonXAxis
            // 
            this.buttonXAxis.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXAxis.Location = new System.Drawing.Point(55, 121);
            this.buttonXAxis.Name = "buttonXAxis";
            this.buttonXAxis.Size = new System.Drawing.Size(159, 51);
            this.buttonXAxis.TabIndex = 35;
            this.buttonXAxis.Text = "X Motion Test";
            this.buttonXAxis.UseVisualStyleBackColor = true;
            this.buttonXAxis.Click += new System.EventHandler(this.buttonXAxis_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(45, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 16);
            this.label1.TabIndex = 50;
            this.label1.Text = "DEVICE MOTION TESTING";
            // 
            // labelParascan1
            // 
            this.labelParascan1.AutoSize = true;
            this.labelParascan1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelParascan1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.labelParascan1.Location = new System.Drawing.Point(16, 2);
            this.labelParascan1.Name = "labelParascan1";
            this.labelParascan1.Size = new System.Drawing.Size(245, 42);
            this.labelParascan1.TabIndex = 49;
            this.labelParascan1.Text = "PARASCAN1";
            // 
            // buttonYAxis
            // 
            this.buttonYAxis.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonYAxis.Location = new System.Drawing.Point(55, 64);
            this.buttonYAxis.Name = "buttonYAxis";
            this.buttonYAxis.Size = new System.Drawing.Size(159, 51);
            this.buttonYAxis.TabIndex = 51;
            this.buttonYAxis.Text = "Y Motion Test";
            this.buttonYAxis.UseVisualStyleBackColor = true;
            this.buttonYAxis.Click += new System.EventHandler(this.buttonYAxis_Click);
            // 
            // buttonSnapshot
            // 
            this.buttonSnapshot.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSnapshot.Location = new System.Drawing.Point(61, 565);
            this.buttonSnapshot.Name = "buttonSnapshot";
            this.buttonSnapshot.Size = new System.Drawing.Size(162, 48);
            this.buttonSnapshot.TabIndex = 52;
            this.buttonSnapshot.Text = "Snapshot";
            this.buttonSnapshot.UseVisualStyleBackColor = true;
            this.buttonSnapshot.Click += new System.EventHandler(this.buttonSnapshot_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoad.Location = new System.Drawing.Point(136, 294);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(133, 42);
            this.buttonLoad.TabIndex = 53;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonAutofocus
            // 
            this.buttonAutofocus.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAutofocus.Location = new System.Drawing.Point(17, 524);
            this.buttonAutofocus.Name = "buttonAutofocus";
            this.buttonAutofocus.Size = new System.Drawing.Size(115, 36);
            this.buttonAutofocus.TabIndex = 54;
            this.buttonAutofocus.Text = "Autofocus";
            this.buttonAutofocus.UseVisualStyleBackColor = true;
            this.buttonAutofocus.Click += new System.EventHandler(this.buttonAutofocus_Click);
            // 
            // buttonWhiteBalance
            // 
            this.buttonWhiteBalance.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonWhiteBalance.Location = new System.Drawing.Point(139, 524);
            this.buttonWhiteBalance.Name = "buttonWhiteBalance";
            this.buttonWhiteBalance.Size = new System.Drawing.Size(115, 36);
            this.buttonWhiteBalance.TabIndex = 55;
            this.buttonWhiteBalance.Text = "White Bal";
            this.buttonWhiteBalance.UseVisualStyleBackColor = true;
            this.buttonWhiteBalance.Click += new System.EventHandler(this.buttonWhiteBalance_Click);
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.Red;
            this.labelMessage.Location = new System.Drawing.Point(639, 9);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(121, 18);
            this.labelMessage.TabIndex = 56;
            this.labelMessage.Text = "Processing...";
            // 
            // port
            // 
            this.port.PortName = "COM3";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SlateBlue;
            this.ClientSize = new System.Drawing.Size(1132, 733);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.buttonWhiteBalance);
            this.Controls.Add(this.buttonAutofocus);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSnapshot);
            this.Controls.Add(this.buttonYAxis);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelParascan1);
            this.Controls.Add(this.textBoxDistance);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.buttonEject);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonZDown);
            this.Controls.Add(this.buttonZUp);
            this.Controls.Add(this.buttonXRight);
            this.Controls.Add(this.buttonXLeft);
            this.Controls.Add(this.buttonYRight);
            this.Controls.Add(this.buttonYLeft);
            this.Controls.Add(this.comboBoxDistance);
            this.Controls.Add(pictureBoxLogo);
            this.Controls.Add(this.buttonZAxis);
            this.Controls.Add(this.buttonXAxis);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(pictureBoxLogo)).EndInit();
            this.panelDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelDisplay;
        private System.Windows.Forms.PictureBox pictureBoxDisplay;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFPS;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFailed;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFrameCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCamera;
        private System.Windows.Forms.TextBox textBoxDistance;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.Button buttonEject;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonZDown;
        private System.Windows.Forms.Button buttonZUp;
        private System.Windows.Forms.Button buttonXRight;
        private System.Windows.Forms.Button buttonXLeft;
        private System.Windows.Forms.Button buttonYRight;
        private System.Windows.Forms.Button buttonYLeft;
        private System.Windows.Forms.ComboBox comboBoxDistance;
        private System.Windows.Forms.Button buttonZAxis;
        private System.Windows.Forms.Button buttonXAxis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelParascan1;
        private System.Windows.Forms.Button buttonYAxis;
        private System.Windows.Forms.Button buttonSnapshot;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonAutofocus;
        private System.Windows.Forms.Button buttonWhiteBalance;
        private System.Windows.Forms.Label labelMessage;
        private System.IO.Ports.SerialPort port;
    }
}

