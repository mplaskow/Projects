namespace Parascan0
{
    partial class formClinicScan
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formClinicScan));
            this.panelDisplay = new System.Windows.Forms.Panel();
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressBarWait = new System.Windows.Forms.ProgressBar();
            this.pictureBoxTips = new System.Windows.Forms.PictureBox();
            this.labelTips = new System.Windows.Forms.Label();
            this.pictureBoxDisplay = new System.Windows.Forms.PictureBox();
            this.labelMessage = new System.Windows.Forms.Label();
            this.port = new System.IO.Ports.SerialPort(this.components);
            this.panelDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTips)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDisplay
            // 
            this.panelDisplay.Controls.Add(this.labelProgress);
            this.panelDisplay.Controls.Add(this.progressBarWait);
            this.panelDisplay.Controls.Add(this.pictureBoxTips);
            this.panelDisplay.Controls.Add(this.labelTips);
            this.panelDisplay.Controls.Add(this.pictureBoxDisplay);
            this.panelDisplay.Location = new System.Drawing.Point(0, 0);
            this.panelDisplay.Name = "panelDisplay";
            this.panelDisplay.Size = new System.Drawing.Size(968, 744);
            this.panelDisplay.TabIndex = 0;
            // 
            // labelProgress
            // 
            this.labelProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgress.ForeColor = System.Drawing.Color.White;
            this.labelProgress.Location = new System.Drawing.Point(10, 618);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(84, 30);
            this.labelProgress.TabIndex = 34;
            this.labelProgress.Text = "Progress:";
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressBarWait
            // 
            this.progressBarWait.Location = new System.Drawing.Point(100, 610);
            this.progressBarWait.Maximum = 50;
            this.progressBarWait.Name = "progressBarWait";
            this.progressBarWait.Size = new System.Drawing.Size(854, 45);
            this.progressBarWait.TabIndex = 33;
            // 
            // pictureBoxTips
            // 
            this.pictureBoxTips.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxTips.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxTips.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxTips.Image")));
            this.pictureBoxTips.Location = new System.Drawing.Point(503, 12);
            this.pictureBoxTips.Name = "pictureBoxTips";
            this.pictureBoxTips.Size = new System.Drawing.Size(220, 371);
            this.pictureBoxTips.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxTips.TabIndex = 32;
            this.pictureBoxTips.TabStop = false;
            // 
            // labelTips
            // 
            this.labelTips.Font = new System.Drawing.Font("Comic Sans MS", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTips.ForeColor = System.Drawing.Color.LightGray;
            this.labelTips.Location = new System.Drawing.Point(12, 9);
            this.labelTips.Name = "labelTips";
            this.labelTips.Size = new System.Drawing.Size(128, 30);
            this.labelTips.TabIndex = 31;
            this.labelTips.Text = "Scanning...";
            this.labelTips.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxDisplay
            // 
            this.pictureBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxDisplay.BackColor = System.Drawing.Color.Gainsboro;
            this.pictureBoxDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxDisplay.Image = global::Parascan0.Properties.Resources.uEyeLogo;
            this.pictureBoxDisplay.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxDisplay.Name = "pictureBoxDisplay";
            this.pictureBoxDisplay.Size = new System.Drawing.Size(968, 744);
            this.pictureBoxDisplay.TabIndex = 2;
            this.pictureBoxDisplay.TabStop = false;
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.Red;
            this.labelMessage.Location = new System.Drawing.Point(394, 751);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(121, 18);
            this.labelMessage.TabIndex = 35;
            this.labelMessage.Text = "Processing...";
            // 
            // formClinicScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 778);
            this.ControlBox = false;
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.panelDisplay);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "formClinicScan";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing...";
            this.Activated += new System.EventHandler(this.FormClinicScan_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formClinicScan_FormClosing);
            this.panelDisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTips)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelDisplay;
        private System.Windows.Forms.PictureBox pictureBoxDisplay;
        private System.Windows.Forms.Label labelTips;
        private System.Windows.Forms.PictureBox pictureBoxTips;
        private System.Windows.Forms.ProgressBar progressBarWait;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label labelMessage;
        private System.IO.Ports.SerialPort port;



    }
}

