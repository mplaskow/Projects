namespace Parascan0
{
    partial class formTestResults
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
            this.buttonNEGATIVE = new System.Windows.Forms.Button();
            this.buttonEnter = new System.Windows.Forms.Button();
            this.labelResults = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonNEGATIVE
            // 
            this.buttonNEGATIVE.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNEGATIVE.ForeColor = System.Drawing.Color.Green;
            this.buttonNEGATIVE.Location = new System.Drawing.Point(12, 12);
            this.buttonNEGATIVE.Name = "buttonNEGATIVE";
            this.buttonNEGATIVE.Size = new System.Drawing.Size(923, 59);
            this.buttonNEGATIVE.TabIndex = 4;
            this.buttonNEGATIVE.Text = "NEGATIVE";
            this.buttonNEGATIVE.UseVisualStyleBackColor = true;
            this.buttonNEGATIVE.Click += new System.EventHandler(this.buttonNEGATIVE_Click);
            // 
            // buttonEnter
            // 
            this.buttonEnter.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEnter.ForeColor = System.Drawing.Color.Maroon;
            this.buttonEnter.Location = new System.Drawing.Point(358, 94);
            this.buttonEnter.Name = "buttonEnter";
            this.buttonEnter.Size = new System.Drawing.Size(577, 59);
            this.buttonEnter.TabIndex = 12;
            this.buttonEnter.Text = "ENTER POSITIVE";
            this.buttonEnter.UseVisualStyleBackColor = true;
            this.buttonEnter.Click += new System.EventHandler(this.buttonEnter_Click);
            // 
            // labelResults
            // 
            this.labelResults.AutoSize = true;
            this.labelResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResults.ForeColor = System.Drawing.Color.Red;
            this.labelResults.Location = new System.Drawing.Point(22, 114);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(309, 24);
            this.labelResults.TabIndex = 13;
            this.labelResults.Text = "SELECT RESULTS BELOW THEN:";
            // 
            // formTestResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 451);
            this.ControlBox = false;
            this.Controls.Add(this.labelResults);
            this.Controls.Add(this.buttonEnter);
            this.Controls.Add(this.buttonNEGATIVE);
            this.Name = "formTestResults";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.formTestResults_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonNEGATIVE;
        private System.Windows.Forms.Button buttonEnter;
        private System.Windows.Forms.Label labelResults;
    }
}