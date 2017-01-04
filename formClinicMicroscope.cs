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
    public partial class formClinicMicroscope : Form
    {
        string pageURL;
        private List<string> iconFilePath = new List<string>();
        private string iconFileName = String.Empty;
        

        public formClinicMicroscope(string URL)
        {
            InitializeComponent();
            //NYAC35805167_MIX_TOX3
            pageURL = "http://localhost/ahtpathologyassistant/FormAnalysisRequestViewerMicroscope.aspx?REQUESTID=39167&ORGANIZATIONID=WHT&LOCATIONID=43085&CASEID=1111&SPECIMENID=001&PROCESSED=IMAGESANALYSIS/WHT/43085/1111/001/1111_PROCESSED.JPG&RESULTS=TOXO4";
            pageURL = URL;
        }

        private void formClinicMicroscope_Load(object sender, EventArgs e)
        {
            imageBoxMicroscope.Image = Image.FromFile(pageURL);
            //string stringIconPath = Application.StartupPath + "\\" + Properties.Settings.Default.STRING_PATHICONS;
            //DirectoryInfo objectDirectory = new DirectoryInfo(stringIconPath);
            //FileInfo[] objectFiles = objectDirectory.GetFiles();
            //Image objectImage;

            //this.BackColor = ObjectsStatic.StandardColorBackground;
            //listViewResults.BackColor = ObjectsStatic.StandardColorBackground;
         
            #region "*** Load Image List and List View ***"

            //foreach (FileInfo objectFile in objectFiles)
            //{
                //try
                //{
                //    FileInfo objectFile = new FileInfo(@"C:\Temp\QMira\Images\1111_Results.JPG");
                //    objectImage = Image.FromFile(objectFile.FullName);
                //    this.imageListResults.Images.Add(objectImage);
                //    iconFilePath.Add(objectFile.FullName);
                //}
                //catch { }
            //}
            //this.listViewResults.View = View.LargeIcon;
            //this.imageListResults.ImageSize = new Size(imageBoxMicroscope.Width, 86);
            //this.listViewResults.LargeImageList = this.imageListResults;
            
            //for (int index = 0; index < this.imageListResults.Images.Count; index++)
            //{
            //    ListViewItem item = new ListViewItem();
            //    item.ImageIndex = index;
            //    item.Name = iconFilePath[index];
            //    this.listViewResults.Items.Add(item);
            //}
            //this.listViewResults.Refresh();


            #endregion

        }

        private void formClinicMicroscope_Activated(object sender, EventArgs e)
        {
            imageBoxMicroscope.ZoomToFit();
            imageBoxMicroscope.Focus();
            imageBoxMicroscope.Zoom = imageBoxMicroscope.Zoom + 1;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    
    }
}
