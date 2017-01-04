namespace Parascan0
{
    partial class formClinicRemoveSlide
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formClinicRemoveSlide));
            this.pictureBoxSlidePlacement = new System.Windows.Forms.PictureBox();
            this.pictureBoxBack = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlidePlacement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxSlidePlacement
            // 
            this.pictureBoxSlidePlacement.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxSlidePlacement.Image")));
            this.pictureBoxSlidePlacement.Location = new System.Drawing.Point(0, -1);
            this.pictureBoxSlidePlacement.Name = "pictureBoxSlidePlacement";
            this.pictureBoxSlidePlacement.Size = new System.Drawing.Size(400, 420);
            this.pictureBoxSlidePlacement.TabIndex = 1;
            this.pictureBoxSlidePlacement.TabStop = false;
            // 
            // pictureBoxBack
            // 
            this.pictureBoxBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxBack.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxBack.Image")));
            this.pictureBoxBack.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxBack.InitialImage")));
            this.pictureBoxBack.Location = new System.Drawing.Point(135, 421);
            this.pictureBoxBack.Name = "pictureBoxBack";
            this.pictureBoxBack.Size = new System.Drawing.Size(138, 60);
            this.pictureBoxBack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBack.TabIndex = 51;
            this.pictureBoxBack.TabStop = false;
            this.pictureBoxBack.Click += new System.EventHandler(this.pictureBoxBack_Click);
            // 
            // formClinicRemoveSlide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 483);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxBack);
            this.Controls.Add(this.pictureBoxSlidePlacement);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formClinicRemoveSlide";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.formClinicRemoveSlide_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSlidePlacement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxSlidePlacement;
        private System.Windows.Forms.PictureBox pictureBoxBack;
    }
}