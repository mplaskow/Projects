namespace Parascan0
{

    partial class formClinicMicroscope
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formClinicMicroscope));
            this.buttonClose = new System.Windows.Forms.Button();
            this.imageBoxMicroscope = new Cyotek.Windows.Forms.ImageBox();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.imageListResults = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Arial Black", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.ForeColor = System.Drawing.Color.Red;
            this.buttonClose.Location = new System.Drawing.Point(548, 791);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(156, 40);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // imageBoxMicroscope
            // 
            this.imageBoxMicroscope.Location = new System.Drawing.Point(-2, 94);
            this.imageBoxMicroscope.Name = "imageBoxMicroscope";
            this.imageBoxMicroscope.Size = new System.Drawing.Size(1399, 701);
            this.imageBoxMicroscope.TabIndex = 2;
            // 
            // listViewResults
            // 
            this.listViewResults.Location = new System.Drawing.Point(-2, 2);
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(1367, 86);
            this.listViewResults.TabIndex = 3;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            // 
            // imageListResults
            // 
            this.imageListResults.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListResults.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListResults.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1399, 86);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // formClinicMicroscope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1366, 746);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listViewResults);
            this.Controls.Add(this.imageBoxMicroscope);
            this.Controls.Add(this.buttonClose);
            this.Name = "formClinicMicroscope";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "formClinicMicroscope";
            this.Load += new System.EventHandler(this.formClinicMicroscope_Load);
            this.Activated += new System.EventHandler(this.formClinicMicroscope_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private Cyotek.Windows.Forms.ImageBox imageBoxMicroscope;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.ImageList imageListResults;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}