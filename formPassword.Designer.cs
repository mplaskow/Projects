namespace Parascan0
{
    partial class formPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formPassword));
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.labelPasswordVerify = new System.Windows.Forms.Label();
            this.labelPasswordNew = new System.Windows.Forms.Label();
            this.textBoxPasswordNew = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.textBoxPasswordVerify = new System.Windows.Forms.TextBox();
            this.textBoxPasswordCurrent = new System.Windows.Forms.TextBox();
            this.labelPasswordCurrent = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(153, 12);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(200, 58);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 31;
            this.pictureBoxLogo.TabStop = false;
            // 
            // labelPasswordVerify
            // 
            this.labelPasswordVerify.AutoSize = true;
            this.labelPasswordVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPasswordVerify.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelPasswordVerify.Location = new System.Drawing.Point(74, 149);
            this.labelPasswordVerify.Name = "labelPasswordVerify";
            this.labelPasswordVerify.Size = new System.Drawing.Size(149, 24);
            this.labelPasswordVerify.TabIndex = 38;
            this.labelPasswordVerify.Text = "Verfiy Password:";
            this.labelPasswordVerify.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelPasswordNew
            // 
            this.labelPasswordNew.AutoSize = true;
            this.labelPasswordNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPasswordNew.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelPasswordNew.Location = new System.Drawing.Point(82, 117);
            this.labelPasswordNew.Name = "labelPasswordNew";
            this.labelPasswordNew.Size = new System.Drawing.Size(141, 24);
            this.labelPasswordNew.TabIndex = 36;
            this.labelPasswordNew.Text = "New Password:";
            this.labelPasswordNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxPasswordNew
            // 
            this.textBoxPasswordNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPasswordNew.Location = new System.Drawing.Point(229, 114);
            this.textBoxPasswordNew.Name = "textBoxPasswordNew";
            this.textBoxPasswordNew.PasswordChar = '*';
            this.textBoxPasswordNew.Size = new System.Drawing.Size(186, 29);
            this.textBoxPasswordNew.TabIndex = 2;
            // 
            // buttonOK
            // 
            this.buttonOK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonOK.BackgroundImage")));
            this.buttonOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOK.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonOK.Location = new System.Drawing.Point(143, 195);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(0);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonOK.Size = new System.Drawing.Size(121, 56);
            this.buttonOK.TabIndex = 53;
            this.buttonOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessage.ForeColor = System.Drawing.Color.Red;
            this.labelMessage.Location = new System.Drawing.Point(12, 264);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(74, 20);
            this.labelMessage.TabIndex = 55;
            this.labelMessage.Text = "Message";
            this.labelMessage.Visible = false;
            // 
            // textBoxPasswordVerify
            // 
            this.textBoxPasswordVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPasswordVerify.Location = new System.Drawing.Point(229, 149);
            this.textBoxPasswordVerify.Name = "textBoxPasswordVerify";
            this.textBoxPasswordVerify.PasswordChar = '*';
            this.textBoxPasswordVerify.Size = new System.Drawing.Size(186, 29);
            this.textBoxPasswordVerify.TabIndex = 3;
            // 
            // textBoxPasswordCurrent
            // 
            this.textBoxPasswordCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPasswordCurrent.Location = new System.Drawing.Point(229, 79);
            this.textBoxPasswordCurrent.Name = "textBoxPasswordCurrent";
            this.textBoxPasswordCurrent.PasswordChar = '*';
            this.textBoxPasswordCurrent.Size = new System.Drawing.Size(186, 29);
            this.textBoxPasswordCurrent.TabIndex = 1;
            // 
            // labelPasswordCurrent
            // 
            this.labelPasswordCurrent.AutoSize = true;
            this.labelPasswordCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPasswordCurrent.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelPasswordCurrent.Location = new System.Drawing.Point(59, 82);
            this.labelPasswordCurrent.Name = "labelPasswordCurrent";
            this.labelPasswordCurrent.Size = new System.Drawing.Size(164, 24);
            this.labelPasswordCurrent.TabIndex = 56;
            this.labelPasswordCurrent.Text = "Current Password:";
            this.labelPasswordCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonClose
            // 
            this.buttonClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonClose.BackgroundImage")));
            this.buttonClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.buttonClose.Location = new System.Drawing.Point(274, 195);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(0);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonClose.Size = new System.Drawing.Size(121, 56);
            this.buttonClose.TabIndex = 57;
            this.buttonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // formPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MidnightBlue;
            this.ClientSize = new System.Drawing.Size(536, 293);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.labelPasswordCurrent);
            this.Controls.Add(this.textBoxPasswordCurrent);
            this.Controls.Add(this.textBoxPasswordVerify);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelPasswordVerify);
            this.Controls.Add(this.labelPasswordNew);
            this.Controls.Add(this.textBoxPasswordNew);
            this.Controls.Add(this.pictureBoxLogo);
            this.Name = "formPassword";
            this.Text = "formPassword";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label labelPasswordVerify;
        private System.Windows.Forms.Label labelPasswordNew;
        private System.Windows.Forms.TextBox textBoxPasswordNew;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.TextBox textBoxPasswordVerify;
        private System.Windows.Forms.TextBox textBoxPasswordCurrent;
        private System.Windows.Forms.Label labelPasswordCurrent;
        private System.Windows.Forms.Button buttonClose;
    }
}