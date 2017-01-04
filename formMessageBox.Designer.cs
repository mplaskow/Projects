namespace Parascan0
{
    partial class formMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMessageBox));
            this.pictureBoxYes = new System.Windows.Forms.PictureBox();
            this.pictureBoxNo = new System.Windows.Forms.PictureBox();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxYes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxYes
            // 
            this.pictureBoxYes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxYes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxYes.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxYes.Image")));
            this.pictureBoxYes.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxYes.InitialImage")));
            this.pictureBoxYes.Location = new System.Drawing.Point(72, 181);
            this.pictureBoxYes.Name = "pictureBoxYes";
            this.pictureBoxYes.Size = new System.Drawing.Size(164, 68);
            this.pictureBoxYes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxYes.TabIndex = 52;
            this.pictureBoxYes.TabStop = false;
            this.pictureBoxYes.Click += new System.EventHandler(this.pictureBoxYes_Click);
            // 
            // pictureBoxNo
            // 
            this.pictureBoxNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxNo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxNo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxNo.Image")));
            this.pictureBoxNo.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxNo.InitialImage")));
            this.pictureBoxNo.Location = new System.Drawing.Point(273, 181);
            this.pictureBoxNo.Name = "pictureBoxNo";
            this.pictureBoxNo.Size = new System.Drawing.Size(164, 68);
            this.pictureBoxNo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxNo.TabIndex = 53;
            this.pictureBoxNo.TabStop = false;
            this.pictureBoxNo.Click += new System.EventHandler(this.pictureBoxNo_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.BackColor = System.Drawing.Color.MidnightBlue;
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxMessage.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxMessage.ForeColor = System.Drawing.Color.White;
            this.textBoxMessage.Location = new System.Drawing.Point(12, 12);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(491, 163);
            this.textBoxMessage.TabIndex = 54;
            this.textBoxMessage.TabStop = false;
            this.textBoxMessage.Text = "Were any Tapeworm Segments Observed on or within the Stool/Fecal Sample?";
            this.textBoxMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // formMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(64)))), ((int)(((byte)(163)))));
            this.ClientSize = new System.Drawing.Size(512, 261);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.pictureBoxNo);
            this.Controls.Add(this.pictureBoxYes);
            this.Name = "formMessageBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.formMessageBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxYes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxYes;
        private System.Windows.Forms.PictureBox pictureBoxNo;
        private System.Windows.Forms.TextBox textBoxMessage;
    }
}