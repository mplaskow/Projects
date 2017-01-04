namespace Parascan0
{
    partial class formClinicPlaceSlide
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formClinicPlaceSlide));
            this.pictureBoxSlidePlacement = new System.Windows.Forms.PictureBox();
            this.pictureBoxScan = new System.Windows.Forms.PictureBox();
            this.pictureBoxCancel = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlidePlacement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCancel)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxSlidePlacement
            // 
            this.pictureBoxSlidePlacement.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxSlidePlacement.Image")));
            this.pictureBoxSlidePlacement.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxSlidePlacement.InitialImage")));
            this.pictureBoxSlidePlacement.Location = new System.Drawing.Point(-5, -6);
            this.pictureBoxSlidePlacement.Name = "pictureBoxSlidePlacement";
            this.pictureBoxSlidePlacement.Size = new System.Drawing.Size(600, 420);
            this.pictureBoxSlidePlacement.TabIndex = 0;
            this.pictureBoxSlidePlacement.TabStop = false;
            // 
            // pictureBoxScan
            // 
            this.pictureBoxScan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxScan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxScan.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxScan.Image")));
            this.pictureBoxScan.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxScan.InitialImage")));
            this.pictureBoxScan.Location = new System.Drawing.Point(153, 420);
            this.pictureBoxScan.Name = "pictureBoxScan";
            this.pictureBoxScan.Size = new System.Drawing.Size(138, 60);
            this.pictureBoxScan.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxScan.TabIndex = 49;
            this.pictureBoxScan.TabStop = false;
            this.pictureBoxScan.Click += new System.EventHandler(this.pictureBoxScan_Click);
            // 
            // pictureBoxCancel
            // 
            this.pictureBoxCancel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxCancel.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCancel.Image")));
            this.pictureBoxCancel.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCancel.InitialImage")));
            this.pictureBoxCancel.Location = new System.Drawing.Point(299, 420);
            this.pictureBoxCancel.Name = "pictureBoxCancel";
            this.pictureBoxCancel.Size = new System.Drawing.Size(138, 60);
            this.pictureBoxCancel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCancel.TabIndex = 50;
            this.pictureBoxCancel.TabStop = false;
            this.pictureBoxCancel.Click += new System.EventHandler(this.pictureBoxCancel_Click);
            // 
            // formClinicPlaceSlide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 483);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxCancel);
            this.Controls.Add(this.pictureBoxScan);
            this.Controls.Add(this.pictureBoxSlidePlacement);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formClinicPlaceSlide";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlidePlacement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCancel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxSlidePlacement;
        private System.Windows.Forms.PictureBox pictureBoxScan;
        private System.Windows.Forms.PictureBox pictureBoxCancel;
    }
}