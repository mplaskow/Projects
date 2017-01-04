using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Parascan0
{
    public partial class formImageSelection : Form
    {
        private List<string> iconFilePath = new List<string>();
        private string iconFileName = String.Empty;
        private int bitmapWidth;
        private int bitmapHeight;
        public Bitmap OrganizationIcon;

        public formImageSelection()
        {
            InitializeComponent();
        }

        private void formImageSelection_Load(object sender, EventArgs e)
        {

            #region "*** Set Values for Image Folder and Variables ***"

            string stringIconPath = Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHICONS;
            DirectoryInfo objectDirectory = new DirectoryInfo(stringIconPath);
            FileInfo[] objectFiles = objectDirectory.GetFiles();
            Image objectImage;

            this.BackColor = ObjectsStatic.StandardColorBackground;
            listViewIcons.BackColor = ObjectsStatic.StandardColorBackground;

            textBoxOrganizationName.Text = ObjectsStatic.OrganizationName;

            #endregion

            #region "*** Load Image List and List View ***"

            foreach (FileInfo objectFile in objectFiles)
            {
                try
                {
                    objectImage = Image.FromFile(objectFile.FullName);
                    this.imageListIcons.Images.Add(objectImage);
                    iconFilePath.Add(objectFile.FullName);
                    bitmapWidth = objectImage.Width;
                    bitmapHeight = objectImage.Height;
                }
                catch { }
            }
            this.listViewIcons.View = View.LargeIcon;
            this.imageListIcons.ImageSize = new Size(125, 125);
            this.listViewIcons.LargeImageList = this.imageListIcons;

            for (int index = 0; index < this.imageListIcons.Images.Count; index++)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = index;
                item.Name = iconFilePath[index];
                this.listViewIcons.Items.Add(item);
            }

            #endregion

            #region "*** Load Installed Fonts ***"
            
            System.Drawing.Text.InstalledFontCollection fontCollection = new System.Drawing.Text.InstalledFontCollection();
            foreach (System.Drawing.FontFamily fontFamily in fontCollection.Families)
            {
                comboBoxOrganizationFont.Items.Add(fontFamily.Name);
            }

            #endregion

        }

        private void pictureBoxBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listViewIcons_SelectedIndexChanged(object sender, EventArgs e)
        {
            iconFileName = listViewIcons.SelectedItems[0].Name;
        }

        private void pictureBoxSave_Click(object sender, EventArgs e)
        {
            string stringOrganizationName = textBoxOrganizationName.Text.Trim();
            const int iconPadding = 25;
            if (iconFileName.Length < 7) { MessageBox.Show("No Image Selected", "Select Hospital Icon");  }
            if ((stringOrganizationName.Length > 0) && (comboBoxOrganizationFont.Text.Length > 3))
            {
                Font organizationFont = new Font(comboBoxOrganizationFont.Text, 14, FontStyle.Regular);

                int pixelOffset = TextRenderer.MeasureText(stringOrganizationName,organizationFont).Width;
                if (pixelOffset > bitmapWidth) { pixelOffset = bitmapWidth; }
                OrganizationIcon = new Bitmap(bitmapWidth, bitmapHeight + iconPadding);
                Graphics objectGraphics = Graphics.FromImage(OrganizationIcon);
                objectGraphics.Clear(Color.White);
                objectGraphics.DrawImage(Image.FromFile(iconFileName), 0, 0);
                objectGraphics.DrawString(stringOrganizationName, organizationFont, SystemBrushes.WindowText, Convert.ToInt16(bitmapWidth * .5) - Convert.ToInt16(pixelOffset * .5), bitmapHeight - 1);
                string stringOrganizationImagePath = Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHIMAGES + "\\" + Properties.Settings.Default.STRING_ORGANIZATIONIMAGE;

                OrganizationIcon.Save(stringOrganizationImagePath);
                objectGraphics.Dispose();
            }
            this.Close();
        }


    

        
    }
}
