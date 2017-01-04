namespace Parascan0
{
    partial class formReport
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
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.pictureBoxOrganizationLogo = new System.Windows.Forms.PictureBox();
            this.textBoxOrganizationName = new System.Windows.Forms.TextBox();
            this.labelOrganizationAddress = new System.Windows.Forms.Label();
            this.labelOrganizationPhone = new System.Windows.Forms.Label();
            this.labelOrganizationContact = new System.Windows.Forms.Label();
            this.labelOrganizationEMail = new System.Windows.Forms.Label();
            this.labelOrganizationCity = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelRequestDate = new System.Windows.Forms.Label();
            this.labelReport = new System.Windows.Forms.Label();
            this.labelCaseID = new System.Windows.Forms.Label();
            this.labelBarcode = new System.Windows.Forms.Label();
            this.labelOwner = new System.Windows.Forms.Label();
            this.labelGender = new System.Windows.Forms.Label();
            this.labelSpecies = new System.Windows.Forms.Label();
            this.labelBreed = new System.Windows.Forms.Label();
            this.labelRequestNotes = new System.Windows.Forms.Label();
            this.textBoxRequestClinicNotes = new System.Windows.Forms.TextBox();
            this.labelRequestClinicNotes = new System.Windows.Forms.Label();
            this.textBoxCaseID = new System.Windows.Forms.TextBox();
            this.textBoxCaseGender = new System.Windows.Forms.TextBox();
            this.textBoxSpecimenNumber = new System.Windows.Forms.TextBox();
            this.textBoxCaseSpecies = new System.Windows.Forms.TextBox();
            this.textBoxCaseOwner = new System.Windows.Forms.TextBox();
            this.textBoxCaseBreed = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxImage = new System.Windows.Forms.PictureBox();
            this.buttonEMail = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.panelImages = new System.Windows.Forms.Panel();
            this.panelImagesResults = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrganizationLogo)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClose.Location = new System.Drawing.Point(363, 785);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(121, 32);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Exit";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Visible = false;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPrint.Location = new System.Drawing.Point(90, 785);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(121, 32);
            this.buttonPrint.TabIndex = 2;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // pictureBoxOrganizationLogo
            // 
            this.pictureBoxOrganizationLogo.Image = global::Parascan0.Properties.Resources.imageQMIRALogo;
            this.pictureBoxOrganizationLogo.Location = new System.Drawing.Point(36, 11);
            this.pictureBoxOrganizationLogo.Name = "pictureBoxOrganizationLogo";
            this.pictureBoxOrganizationLogo.Size = new System.Drawing.Size(87, 83);
            this.pictureBoxOrganizationLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxOrganizationLogo.TabIndex = 34;
            this.pictureBoxOrganizationLogo.TabStop = false;
            // 
            // textBoxOrganizationName
            // 
            this.textBoxOrganizationName.BackColor = System.Drawing.Color.White;
            this.textBoxOrganizationName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxOrganizationName.Enabled = false;
            this.textBoxOrganizationName.Font = new System.Drawing.Font("Comic Sans MS", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOrganizationName.ForeColor = System.Drawing.Color.Navy;
            this.textBoxOrganizationName.Location = new System.Drawing.Point(119, -2);
            this.textBoxOrganizationName.Name = "textBoxOrganizationName";
            this.textBoxOrganizationName.Size = new System.Drawing.Size(347, 38);
            this.textBoxOrganizationName.TabIndex = 35;
            this.textBoxOrganizationName.Text = "EVERGREEN";
            this.textBoxOrganizationName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelOrganizationAddress
            // 
            this.labelOrganizationAddress.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOrganizationAddress.ForeColor = System.Drawing.Color.Black;
            this.labelOrganizationAddress.Location = new System.Drawing.Point(443, 18);
            this.labelOrganizationAddress.Name = "labelOrganizationAddress";
            this.labelOrganizationAddress.Size = new System.Drawing.Size(200, 16);
            this.labelOrganizationAddress.TabIndex = 36;
            this.labelOrganizationAddress.Text = "555 Main Street";
            this.labelOrganizationAddress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOrganizationPhone
            // 
            this.labelOrganizationPhone.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOrganizationPhone.ForeColor = System.Drawing.Color.Black;
            this.labelOrganizationPhone.Location = new System.Drawing.Point(456, 52);
            this.labelOrganizationPhone.Name = "labelOrganizationPhone";
            this.labelOrganizationPhone.Size = new System.Drawing.Size(200, 16);
            this.labelOrganizationPhone.TabIndex = 37;
            this.labelOrganizationPhone.Text = "(614) 582-4184";
            this.labelOrganizationPhone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOrganizationContact
            // 
            this.labelOrganizationContact.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOrganizationContact.ForeColor = System.Drawing.Color.Black;
            this.labelOrganizationContact.Location = new System.Drawing.Point(456, 68);
            this.labelOrganizationContact.Name = "labelOrganizationContact";
            this.labelOrganizationContact.Size = new System.Drawing.Size(200, 16);
            this.labelOrganizationContact.TabIndex = 38;
            this.labelOrganizationContact.Text = "Mark Plaskow";
            this.labelOrganizationContact.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOrganizationEMail
            // 
            this.labelOrganizationEMail.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOrganizationEMail.ForeColor = System.Drawing.Color.Black;
            this.labelOrganizationEMail.Location = new System.Drawing.Point(456, 84);
            this.labelOrganizationEMail.Name = "labelOrganizationEMail";
            this.labelOrganizationEMail.Size = new System.Drawing.Size(200, 16);
            this.labelOrganizationEMail.TabIndex = 39;
            this.labelOrganizationEMail.Text = "meplaskow@qmira.com";
            this.labelOrganizationEMail.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOrganizationCity
            // 
            this.labelOrganizationCity.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOrganizationCity.ForeColor = System.Drawing.Color.Black;
            this.labelOrganizationCity.Location = new System.Drawing.Point(456, 36);
            this.labelOrganizationCity.Name = "labelOrganizationCity";
            this.labelOrganizationCity.Size = new System.Drawing.Size(200, 16);
            this.labelOrganizationCity.TabIndex = 40;
            this.labelOrganizationCity.Text = "Fishers, IN 11111";
            this.labelOrganizationCity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.labelRequestDate);
            this.panel1.Controls.Add(this.textBoxOrganizationName);
            this.panel1.Controls.Add(this.labelOrganizationAddress);
            this.panel1.Location = new System.Drawing.Point(18, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(644, 103);
            this.panel1.TabIndex = 41;
            // 
            // labelRequestDate
            // 
            this.labelRequestDate.AutoSize = true;
            this.labelRequestDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRequestDate.Location = new System.Drawing.Point(184, 78);
            this.labelRequestDate.Name = "labelRequestDate";
            this.labelRequestDate.Size = new System.Drawing.Size(43, 16);
            this.labelRequestDate.TabIndex = 62;
            this.labelRequestDate.Text = "Date: ";
            // 
            // labelReport
            // 
            this.labelReport.BackColor = System.Drawing.Color.Navy;
            this.labelReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelReport.ForeColor = System.Drawing.Color.White;
            this.labelReport.Location = new System.Drawing.Point(18, 104);
            this.labelReport.Name = "labelReport";
            this.labelReport.Size = new System.Drawing.Size(644, 28);
            this.labelReport.TabIndex = 42;
            this.labelReport.Text = "Veterinary Parasite Report for Buddy Smith";
            this.labelReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelCaseID
            // 
            this.labelCaseID.AutoSize = true;
            this.labelCaseID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCaseID.ForeColor = System.Drawing.Color.Navy;
            this.labelCaseID.Location = new System.Drawing.Point(-1, 135);
            this.labelCaseID.Name = "labelCaseID";
            this.labelCaseID.Size = new System.Drawing.Size(84, 20);
            this.labelCaseID.TabIndex = 43;
            this.labelCaseID.Text = "Patient ID:";
            this.labelCaseID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelBarcode
            // 
            this.labelBarcode.AutoSize = true;
            this.labelBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBarcode.ForeColor = System.Drawing.Color.Navy;
            this.labelBarcode.Location = new System.Drawing.Point(283, 135);
            this.labelBarcode.Name = "labelBarcode";
            this.labelBarcode.Size = new System.Drawing.Size(99, 20);
            this.labelBarcode.TabIndex = 44;
            this.labelBarcode.Text = "Accession #:";
            this.labelBarcode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelOwner
            // 
            this.labelOwner.AutoSize = true;
            this.labelOwner.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelOwner.ForeColor = System.Drawing.Color.Navy;
            this.labelOwner.Location = new System.Drawing.Point(479, 136);
            this.labelOwner.Name = "labelOwner";
            this.labelOwner.Size = new System.Drawing.Size(59, 20);
            this.labelOwner.TabIndex = 45;
            this.labelOwner.Text = "Owner:";
            this.labelOwner.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelGender
            // 
            this.labelGender.AutoSize = true;
            this.labelGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGender.ForeColor = System.Drawing.Color.Navy;
            this.labelGender.Location = new System.Drawing.Point(44, 158);
            this.labelGender.Name = "labelGender";
            this.labelGender.Size = new System.Drawing.Size(40, 20);
            this.labelGender.TabIndex = 46;
            this.labelGender.Text = "Sex:";
            this.labelGender.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSpecies
            // 
            this.labelSpecies.AutoSize = true;
            this.labelSpecies.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSpecies.ForeColor = System.Drawing.Color.Navy;
            this.labelSpecies.Location = new System.Drawing.Point(312, 159);
            this.labelSpecies.Name = "labelSpecies";
            this.labelSpecies.Size = new System.Drawing.Size(70, 20);
            this.labelSpecies.TabIndex = 47;
            this.labelSpecies.Text = "Species:";
            this.labelSpecies.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelBreed
            // 
            this.labelBreed.AutoSize = true;
            this.labelBreed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBreed.ForeColor = System.Drawing.Color.Navy;
            this.labelBreed.Location = new System.Drawing.Point(483, 159);
            this.labelBreed.Name = "labelBreed";
            this.labelBreed.Size = new System.Drawing.Size(56, 20);
            this.labelBreed.TabIndex = 48;
            this.labelBreed.Text = "Breed:";
            this.labelBreed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelRequestNotes
            // 
            this.labelRequestNotes.AutoSize = true;
            this.labelRequestNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRequestNotes.ForeColor = System.Drawing.Color.Navy;
            this.labelRequestNotes.Location = new System.Drawing.Point(6, 184);
            this.labelRequestNotes.Name = "labelRequestNotes";
            this.labelRequestNotes.Size = new System.Drawing.Size(83, 16);
            this.labelRequestNotes.TabIndex = 49;
            this.labelRequestNotes.Text = "Test Results";
            this.labelRequestNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxRequestClinicNotes
            // 
            this.textBoxRequestClinicNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxRequestClinicNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRequestClinicNotes.Location = new System.Drawing.Point(3, 761);
            this.textBoxRequestClinicNotes.Name = "textBoxRequestClinicNotes";
            this.textBoxRequestClinicNotes.Size = new System.Drawing.Size(667, 21);
            this.textBoxRequestClinicNotes.TabIndex = 52;
            this.textBoxRequestClinicNotes.TextChanged += new System.EventHandler(this.textBoxRequestClinicNotes_TextChanged);
            // 
            // labelRequestClinicNotes
            // 
            this.labelRequestClinicNotes.AutoSize = true;
            this.labelRequestClinicNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRequestClinicNotes.ForeColor = System.Drawing.Color.Navy;
            this.labelRequestClinicNotes.Location = new System.Drawing.Point(-1, 738);
            this.labelRequestClinicNotes.Name = "labelRequestClinicNotes";
            this.labelRequestClinicNotes.Size = new System.Drawing.Size(193, 20);
            this.labelRequestClinicNotes.TabIndex = 51;
            this.labelRequestClinicNotes.Text = "Clinical Notes / Comments";
            this.labelRequestClinicNotes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxCaseID
            // 
            this.textBoxCaseID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCaseID.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCaseID.Location = new System.Drawing.Point(78, 135);
            this.textBoxCaseID.Name = "textBoxCaseID";
            this.textBoxCaseID.Size = new System.Drawing.Size(208, 19);
            this.textBoxCaseID.TabIndex = 53;
            this.textBoxCaseID.Text = "1111111";
            // 
            // textBoxCaseGender
            // 
            this.textBoxCaseGender.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCaseGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCaseGender.Location = new System.Drawing.Point(79, 158);
            this.textBoxCaseGender.Multiline = true;
            this.textBoxCaseGender.Name = "textBoxCaseGender";
            this.textBoxCaseGender.Size = new System.Drawing.Size(89, 20);
            this.textBoxCaseGender.TabIndex = 54;
            this.textBoxCaseGender.Text = "Female";
            // 
            // textBoxSpecimenNumber
            // 
            this.textBoxSpecimenNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSpecimenNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSpecimenNumber.Location = new System.Drawing.Point(382, 135);
            this.textBoxSpecimenNumber.Name = "textBoxSpecimenNumber";
            this.textBoxSpecimenNumber.Size = new System.Drawing.Size(101, 19);
            this.textBoxSpecimenNumber.TabIndex = 55;
            this.textBoxSpecimenNumber.Text = "NYAC888855";
            // 
            // textBoxCaseSpecies
            // 
            this.textBoxCaseSpecies.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCaseSpecies.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCaseSpecies.Location = new System.Drawing.Point(382, 159);
            this.textBoxCaseSpecies.Multiline = true;
            this.textBoxCaseSpecies.Name = "textBoxCaseSpecies";
            this.textBoxCaseSpecies.Size = new System.Drawing.Size(101, 20);
            this.textBoxCaseSpecies.TabIndex = 56;
            this.textBoxCaseSpecies.Text = "Canine";
            // 
            // textBoxCaseOwner
            // 
            this.textBoxCaseOwner.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCaseOwner.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCaseOwner.Location = new System.Drawing.Point(535, 136);
            this.textBoxCaseOwner.Name = "textBoxCaseOwner";
            this.textBoxCaseOwner.Size = new System.Drawing.Size(152, 19);
            this.textBoxCaseOwner.TabIndex = 57;
            this.textBoxCaseOwner.Text = "John Doe";
            // 
            // textBoxCaseBreed
            // 
            this.textBoxCaseBreed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCaseBreed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCaseBreed.Location = new System.Drawing.Point(535, 159);
            this.textBoxCaseBreed.Multiline = true;
            this.textBoxCaseBreed.Name = "textBoxCaseBreed";
            this.textBoxCaseBreed.Size = new System.Drawing.Size(152, 25);
            this.textBoxCaseBreed.TabIndex = 58;
            this.textBoxCaseBreed.Text = "Golden Retriever";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(498, 795);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "Powered By: ";
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.Image = global::Parascan0.Properties.Resources.imageQMIRALogo;
            this.pictureBoxImage.Location = new System.Drawing.Point(566, 789);
            this.pictureBoxImage.Name = "pictureBoxImage";
            this.pictureBoxImage.Size = new System.Drawing.Size(104, 28);
            this.pictureBoxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxImage.TabIndex = 61;
            this.pictureBoxImage.TabStop = false;
            // 
            // buttonEMail
            // 
            this.buttonEMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEMail.Location = new System.Drawing.Point(229, 785);
            this.buttonEMail.Name = "buttonEMail";
            this.buttonEMail.Size = new System.Drawing.Size(121, 32);
            this.buttonEMail.TabIndex = 62;
            this.buttonEMail.Text = "EMail";
            this.buttonEMail.UseVisualStyleBackColor = true;
            this.buttonEMail.Click += new System.EventHandler(this.buttonEMail_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // panelImages
            // 
            this.panelImages.BackColor = System.Drawing.Color.White;
            this.panelImages.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelImages.Location = new System.Drawing.Point(1, 200);
            this.panelImages.Name = "panelImages";
            this.panelImages.Size = new System.Drawing.Size(277, 540);
            this.panelImages.TabIndex = 63;
            // 
            // panelImagesResults
            // 
            this.panelImagesResults.BackColor = System.Drawing.Color.White;
            this.panelImagesResults.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelImagesResults.Location = new System.Drawing.Point(289, 200);
            this.panelImagesResults.Name = "panelImagesResults";
            this.panelImagesResults.Size = new System.Drawing.Size(300, 540);
            this.panelImagesResults.TabIndex = 64;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(410, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 16);
            this.label2.TabIndex = 65;
            this.label2.Text = "Images";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // formReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(684, 823);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panelImagesResults);
            this.Controls.Add(this.panelImages);
            this.Controls.Add(this.buttonEMail);
            this.Controls.Add(this.pictureBoxImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxCaseBreed);
            this.Controls.Add(this.textBoxCaseOwner);
            this.Controls.Add(this.textBoxCaseSpecies);
            this.Controls.Add(this.textBoxSpecimenNumber);
            this.Controls.Add(this.textBoxCaseGender);
            this.Controls.Add(this.textBoxCaseID);
            this.Controls.Add(this.textBoxRequestClinicNotes);
            this.Controls.Add(this.labelRequestClinicNotes);
            this.Controls.Add(this.labelRequestNotes);
            this.Controls.Add(this.labelBreed);
            this.Controls.Add(this.labelSpecies);
            this.Controls.Add(this.labelGender);
            this.Controls.Add(this.labelOwner);
            this.Controls.Add(this.labelBarcode);
            this.Controls.Add(this.labelCaseID);
            this.Controls.Add(this.labelReport);
            this.Controls.Add(this.labelOrganizationCity);
            this.Controls.Add(this.labelOrganizationEMail);
            this.Controls.Add(this.labelOrganizationContact);
            this.Controls.Add(this.labelOrganizationPhone);
            this.Controls.Add(this.pictureBoxOrganizationLogo);
            this.Controls.Add(this.buttonPrint);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.panel1);
            this.Name = "formReport";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.formReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrganizationLogo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.PictureBox pictureBoxOrganizationLogo;
        private System.Windows.Forms.TextBox textBoxOrganizationName;
        private System.Windows.Forms.Label labelOrganizationAddress;
        private System.Windows.Forms.Label labelOrganizationPhone;
        private System.Windows.Forms.Label labelOrganizationContact;
        private System.Windows.Forms.Label labelOrganizationEMail;
        private System.Windows.Forms.Label labelOrganizationCity;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelReport;
        private System.Windows.Forms.Label labelCaseID;
        private System.Windows.Forms.Label labelBarcode;
        private System.Windows.Forms.Label labelOwner;
        private System.Windows.Forms.Label labelGender;
        private System.Windows.Forms.Label labelSpecies;
        private System.Windows.Forms.Label labelBreed;
        private System.Windows.Forms.Label labelRequestNotes;
        private System.Windows.Forms.TextBox textBoxRequestClinicNotes;
        private System.Windows.Forms.Label labelRequestClinicNotes;
        private System.Windows.Forms.TextBox textBoxCaseID;
        private System.Windows.Forms.TextBox textBoxCaseGender;
        private System.Windows.Forms.TextBox textBoxSpecimenNumber;
        private System.Windows.Forms.TextBox textBoxCaseSpecies;
        private System.Windows.Forms.TextBox textBoxCaseOwner;
        private System.Windows.Forms.TextBox textBoxCaseBreed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxImage;
        private System.Windows.Forms.Label labelRequestDate;
        private System.Windows.Forms.Button buttonEMail;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Panel panelImages;
        private System.Windows.Forms.Panel panelImagesResults;
        private System.Windows.Forms.Label label2;
    }
}