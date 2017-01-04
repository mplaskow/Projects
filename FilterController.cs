using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Parascan1.Data;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using QMIRATraining;
using ParasiteDetection;

namespace Parascan1.Processing
{
    class FilterController
    {

        #region "*** Declare Class Level Variables ***"

        ParasiteType objectSpecies = null;
        DataController dataController;
        
        DataTable dataTableAnalysisParasiteFecals;
        DataTable dataTableAnalysisParasiteFecalsShapesInterior;
        DataTable dataTableAnalysisParasiteFecalsShapesAnalysis;
        DataTable dataTableAnalysisParasiteFecalsShapesCoccidia;
        double DOUBLE_WIDTHPERCENT = SharedValueController.DOUBLE_WIDTHPERCENT;
        int BITMAP_WIDTH = 0;
        int BITMAP_HEIGHT = 0;
        int IMAGE_RESULT_SIZE = SharedValueController.IMAGE_RESULT_SIZE;
        int IMAGE_RESULT_HALF = (int)(SharedValueController.IMAGE_RESULT_SIZE / 2);
        
        //*** Temp Code ***
        //int perimeterPointsWidth = 0;
        //int perimeterPointsHeight = 0;

        #endregion

        public void ProcessFilters()
        {

            #region "*** Declare Variables for Image Constants and Settings ***"

            string CaseID = "";
            string RequestID = "";
            TrainingModel[] parasiteTrainingModels = GetTrainingModels();
            const int CONSTANT_PIXELSIZE_PARASITEMINIMUM_RESIZED_SEPARATE = 11;
            const int CONSTANT_PIXELSIZE_PARASITEMINIMUM_RESIZED_COMBINED = 12;
            const int CONSTANT_PIXELSIZE_PARASITEMAXIMUM_RESIZED_SEPARATE = 80;
            const int CONSTANT_PIXELSIZE_PARASITEMAXIMUM_RESIZED_COMBINED = 80;
            const int CONSTANT_BITMAPQUALITY = 70;
            System.Drawing.Imaging.ImageCodecInfo jpgEncoder = SharedFunctionController.GetEncoder(System.Drawing.Imaging.ImageFormat.Jpeg);
            System.Drawing.Imaging.EncoderParameters bitmapEncoderParameters = SharedFunctionController.GetEncoderParametersJPGFromQuality(CONSTANT_BITMAPQUALITY);
            string fileName = "";
            int focusInteger = 0;
            double thresholdScore = 0;
            int pixelBlobCenterX = 0;
            int pixelBlobCenterY = 0;
                                                   
            int cellTypeStart = Properties.Settings.Default.INTEGER_CELLTYPE_START;
            int cellTypeEnd = Properties.Settings.Default.INTEGER_CELLTYPE_END;
            bool IsTraining = Properties.Settings.Default.BOOL_ISTRAINING;
            bool IsPositiveTraining = false;
            bool IsNegativeTraining = false;
            bool IsTraining_Database = Properties.Settings.Default.BOOL_ISTRAININGDATABASE;
            bool IsSavedImages = Properties.Settings.Default.BOOL_ISSAVEDIMAGES;
            bool IsKeptPositives = Properties.Settings.Default.BOOL_ISKEPTPOSITIVES;
            bool IsKeptPositivesOnly = Properties.Settings.Default.BOOL_ISKEPTPOSITIVESONLY;
            Pen objectPenBlack = new Pen(Color.Black);
            Pen objectPen = new Pen(Color.Blue, 2);
            Font objectFont = new Font("Arial", 10, FontStyle.Regular);
           
            #endregion

            #region "*** Declare Variables for Image Giardia Constants and Settings ***"

            const int CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_RESIZED_SEPARATE = 3;
            const int CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_RESIZED_COMBINED = 5;
            const int CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_BUBBLE_SEPARATE = 240;
            const int CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_SEPARATE = 15;
            const int CONSTANT_PIXELSIZE_PARASITEMAXIMUM_GIARDIA_SEPARATE = 43;
            const int CONSTANT_PIXELSIZE_COLORHEIGHT_GIARDIA = 12;
            List<SpeciesResult> speciesResultsGiardia = new List<SpeciesResult>();
            

            //*** Begin Set Image Values, Blob Limit and Threshold ***
            const int CONSTANT_GIARDIA_BLOBCOUNT = 490;
            TrainingSetController trainingController = new TrainingSetController(17, 27);
            
            //*** End Set Image Values, Blob Limit and Threshold ***
          
            #endregion
            
            #region "*** Declare Variables for Image Filtering ***"
            
            dataController = new DataController(Properties.Settings.Default.KEY_DATABASECONNECTION_SQLSERVER);

            DirectoryInfo directoryInfoRoot = new DirectoryInfo(Properties.Settings.Default.STRING_PATHCASES);
            string directoryImages = Properties.Settings.Default.STRING_PATHIMAGES;
            DirectoryInfo[] directoriesCases = directoryInfoRoot.GetDirectories();
            Parasite[] Parasites = SharedFunctionController.GetParasites(Properties.Settings.Default.BOOL_ISFILTEREDPARASITES);
            ImageStatistics objectImageStatisticsColor = null;
            
            #endregion

            #region "*** Begin Loop through Case Folders and Filter Images ***"

            foreach (DirectoryInfo caseFolder in directoriesCases)
            {
                FileInfo[] caseFiles = caseFolder.GetFiles("*.jpg");
                List<string> caseSQL = new List<string>();
                int indexFile = 0;
                
                if (caseFiles.Length < 60) { continue; }
                    
                try
                {

                    #region "*** Initialize Case and File Values ***"

                    CaseID = caseFolder.Name.ToUpper();
                    RequestID = String.Empty;
                    int indexRegion = 0;
                    int GIARDIA_BLOB_COUNT = 0;
                    string stringSQL = "Select Count(*) from AnalysisCases Where CaseID = '" + CaseID + "' and CaseCompleted Is Not Null";
                    if (dataController.GetDataScalarFromQuery(stringSQL) != "0") { continue; }
                    
                    stringSQL = "Select Count(*) from AnalysisCases Where CaseID = '" + CaseID + "'";
                    WriteToLog(CaseID + " Started");
                    int limitPositives = Properties.Settings.Default.INTEGER_LIMITPOSITIVES;
                    int[] cellCount = new int[Enum.GetNames(typeof(FECALPARASITE_TYPE)).Length];
                    List<SpeciesResult> SpeciesResults = new List<SpeciesResult>(); 
                    // Sort by file name ascending 
                    Array.Sort(caseFiles, delegate(FileInfo f2, FileInfo f1) { return f2.Name.CompareTo(f1.Name); });

                    string stringTableSuffix = SharedFunctionController.GetTableSuffixFromFileName(CaseID);

                    
                    if (IsTraining_Database == true)
                    {
                        dataTableAnalysisParasiteFecals = GetDataTable_AnalysisParasiteFecals(stringTableSuffix);
                        dataTableAnalysisParasiteFecalsShapesInterior = GetDataTable_AnalysisParasiteFecalsShapesInterior(stringTableSuffix);
                        dataTableAnalysisParasiteFecalsShapesAnalysis = GetDataTable_AnalysisParasiteFecalsShapesAnalysis(stringTableSuffix);
                        dataTableAnalysisParasiteFecalsShapesCoccidia = GetDataTable_AnalysisParasiteFecalsShapesCoccidia(stringTableSuffix);
                    }

                    if (dataController.GetDataScalarFromQuery(stringSQL) == "0")
                    {
                            stringSQL = "Insert Into AnalysisCases (CaseID, OrganizationID,LocationID) Values ('" + CaseID + "'" + st(SharedValueController.STRING_DEFAULTORGANIZATIONID) + st(Properties.Settings.Default.STRING_LOCATIONID) + ")";
                            dataController.ExecuteQuery(stringSQL);
                            stringSQL = "Insert Into AnalysisRequests (OrganizationID,RequestDate,LocationID,UserName,StudyNumber,CaseID,SpecimenNumber,ConditionID,RequestTypeID,RequestGrams) Values ('" +
                              SharedValueController.STRING_DEFAULTORGANIZATIONID + "',GetDate()" + st(SharedValueController.STRING_DEFAULTLOCATIONID) + st(SharedValueController.STRING_DEFAULTUSERNAME) + st("") + st(CaseID) + st("") + st(SharedValueController.STRING_DEFAULTCONDITIONID) + st("DIG") + st(0) + ")";
                            dataController.ExecuteQuery(stringSQL);
                    }
                    else if (IsTraining_Database == true)
                    {
                        continue;
                    }

                    stringSQL = "Select Max(RequestID) from AnalysisRequests Where CaseID = '" + CaseID + "'";
                    RequestID = dataController.GetDataScalarFromQuery(stringSQL);


                    #endregion

                    //*** Begin Loop Through Case Files and Process ***
                    for (indexFile = 0; indexFile < caseFiles.Length; indexFile++)
                    {
                        #region "*** Set Case File and Check if Exists and Processed ***"

                        FileInfo caseFile = null;
                        caseFile = caseFiles[indexFile];
                        bool IsPositiveImage = false;
                        double rotateAngle = 0;

                        if (caseFile.Exists == false) { continue; }
                        if ((IsTraining == false) && (IsTraining_Database == false))
                        {
                            if (SharedFunctionController.IsFileProcessed(caseFolder.FullName + "\\" + SharedValueController.STRING_IMAGESPROCESSEDFILENAME, caseFile.Name) == true)  { continue; }
                            SharedFunctionController.WriteFileProcessed(caseFolder.FullName + "\\" + SharedValueController.STRING_IMAGESPROCESSEDFILENAME, caseFile.Name);
                        }

                        indexRegion++;

                        #endregion

                        #region "*** Create Bitmap from File and Filter ***"

                        //CodeTimer.CodeTime.Start("Create Image Bitmap, Grayscale, Resize, Binary");

                        //WriteToLog("Image " + caseFile.Name + " Started");
                        Bitmap objectBitmap = (Bitmap)System.Drawing.Image.FromFile(caseFile.FullName);
                        BITMAP_WIDTH = objectBitmap.Width;
                        BITMAP_HEIGHT = objectBitmap.Height;
                        Graphics objectBitmapGraphics = Graphics.FromImage(objectBitmap);
                        ContrastStretch objectContrastStretch = new ContrastStretch();
                        Bitmap objectBitmapContrast = objectContrastStretch.Apply(objectBitmap);
                        //*** Resize for Processing ***
                        Bitmap objectBitmapContrastResize = new ResizeBilinear((int)(objectBitmap.Width * .25), (int)(objectBitmap.Height * .25)).Apply(objectBitmapContrast);
                        //*** Filter to Grayscale Resized Image ***
                        Bitmap objectBitmapGrayscale = new ExtractChannel(RGB.R).Apply(objectBitmapContrastResize);
                        //*** Filter to Binary Resized Image ***
                        Bitmap objectBitmapBinary = new BradleyLocalThresholding().Apply(objectBitmapGrayscale);
                        new Invert().ApplyInPlace(objectBitmapBinary);
                        IntPoint[] perimeterPoints = null; double axisMajor = 0; double axisMinor = 0; double fullness = 0; double focusContrast = 0; List<IntPoint> edgePoints = null; Bitmap objectBitmapParasiteBinary = null; double circleVariance; Rectangle objectRectangleParasiteNew;
                        Bitmap objectBitmapParasite = null;
                        Bitmap objectBitmapParasiteRotated = null;
                        Bitmap objectBitmapParasiteGrayscale = null;
                        int cellType = (int)FECALPARASITE_TYPE.Giardia;
                        int[] colorEdge = new int[4]; int[] edgeWidth = new int[4];

                        //CodeTimer.CodeTime.End("Create Image Bitmap, Grayscale, Resize, Binary");

               
                        #endregion

                        #region "*** Begin Giardia Test ***"

                        //#region "*** Set Giardia Values, Row and Column for Current Image ***"

                        //objectSpecies = Parasites[cellType].ParasiteTypes[0];
                        //int indexPointColor = (objectSpecies.SpeciesHeight / 2) + 1;
                        //string STRING_ROWS_GIARDIA = SharedValueController.ROWS_GIARDIA;
                        //string STRING_COLUMNS_GIARDIA = SharedValueController.COLUMNS_GIARDIA;
                        //string stringRow = "|" + Convert.ToString(SharedFunctionController.GetRowFromImage(indexFile, Properties.Settings.Default.INTEGER_IMAGES_COLUMNS)) + "|";
                        //string stringColumn = "|" + Convert.ToString(SharedFunctionController.GetColumnFromImage(indexFile, Properties.Settings.Default.INTEGER_IMAGES_COLUMNS)) + "|";
                        //List<Blob>[] giardiaResultBlobs = new List<Blob>[9];
                        //for (int indexResults = 0; indexResults < 9; indexResults++)
                        //{
                        //    giardiaResultBlobs[indexResults] = new List<Blob>();
                        //}

                        //#endregion

                        //if ((STRING_ROWS_GIARDIA.IndexOf(stringRow) > -1) && (STRING_COLUMNS_GIARDIA.IndexOf(stringColumn) > -1) && (GIARDIA_BLOB_COUNT <= CONSTANT_GIARDIA_BLOBCOUNT))
                        //{
                        //    #region "*** Check for Blobs too Large for Giardia ***"

                        //    //stringSQL = "Select GiardiaBlobCount From AnalysisRequests Where CaseID = '" + CaseID + "' and RequestID = " + RequestID;
                        //    //try { GIARDIA_BLOB_COUNT = Convert.ToInt32(dataController.GetDataScalarFromQuery(stringSQL)); }
                        //    //catch { GIARDIA_BLOB_COUNT = 0; }

                        //    //if (GIARDIA_BLOB_COUNT > CONSTANT_GIARDIA_BLOBCOUNT) { continue; }
                        //    Bitmap objectBitmapBinaryGiardia = (Bitmap)objectBitmapBinary.Clone();
                        //    new BlobsFiltering(600, 600, 10000, 10000, true).ApplyInPlace(objectBitmapBinaryGiardia);
                        //    BlobCounter blobCounterGiardia = new AForge.Imaging.BlobCounter(objectBitmapBinaryGiardia);
                        //    //blobCounterGiardia.ObjectsOrder = ObjectsOrder.Size;
                        //    blobCounterGiardia.ProcessImage(objectBitmapBinaryGiardia);
                        //    Blob[] blobsGiardia = blobCounterGiardia.GetObjectsInformation();
                        //    if (blobsGiardia.Length > 0) { continue; }
                            
                        //    blobsGiardia = null;

                        //    #endregion

                        //    #region "*** Filter for Giardia Blobs ***"

                        //    //*** Filter out Blobs too Small for Giardia in Resized Image ***
                        //    //int pixelBitmapBinaryGiardiaLeft = Convert.ToInt32(CONSTANT_PIXELLEFT_GIARDIA * objectBitmapBinary.Width);
                        //    //objectRectangleParasiteNew = new Rectangle(pixelBitmapBinaryGiardiaLeft, 0, objectBitmapBinary.Width - pixelBitmapBinaryGiardiaLeft, objectBitmapBinary.Height);
                        //    //objectBitmapBinaryGiardia = new Crop(objectRectangleParasiteNew).Apply(objectBitmapBinary);
                        //    objectBitmapBinaryGiardia = (Bitmap)objectBitmapBinary.Clone();

                        //    new BlobsFiltering(CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_RESIZED_SEPARATE, CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_RESIZED_SEPARATE, 10000, 10000, false).ApplyInPlace(objectBitmapBinaryGiardia);
                        //    new BlobsFiltering(CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_RESIZED_COMBINED, CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_RESIZED_COMBINED, 10000, 10000, true).ApplyInPlace(objectBitmapBinaryGiardia);
                        //    //objectBitmapBinaryGiardia = new ResizeBilinear(objectBitmapBinaryGiardia.Width * 4, objectBitmapBinaryGiardia.Height * 4).Apply(objectBitmapBinaryGiardia);
                        //    //pixelBitmapBinaryGiardiaLeft = objectBitmap.Width - objectBitmapBinaryGiardia.Width;
                        //    objectBitmapBinaryGiardia = new ResizeBilinear(objectBitmap.Width, objectBitmap.Height).Apply(objectBitmapBinaryGiardia);

                        //    #endregion

                        //    #region "*** Remove Bubbles from Resized Image ***"

                        //    //*** Remove Bubbles from Resized Image ***
                        //    List<Blob> blobsGiardiaBubbles = new List<Blob>();
                        //    blobCounterGiardia = new AForge.Imaging.BlobCounter(objectBitmapBinaryGiardia);
                        //    blobCounterGiardia.ObjectsOrder = ObjectsOrder.Size;
                        //    blobCounterGiardia.ProcessImage(objectBitmapBinaryGiardia);
                        //    blobsGiardia = blobCounterGiardia.GetObjectsInformation();
                        //    foreach (Blob blobGiardia in blobsGiardia)
                        //    {
                        //        Rectangle rectangleBlobGiardia = blobGiardia.Rectangle;
                        //        if ((rectangleBlobGiardia.Width > CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_BUBBLE_SEPARATE) && (rectangleBlobGiardia.Height > CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_BUBBLE_SEPARATE))
                        //        {
                        //            blobsGiardiaBubbles.Add(blobGiardia);
                        //        }
                        //    }

                        //    #endregion

                        //    #region "*** Filter Blobs in Resized Image for Giardia ***"

                        //    new BlobsFiltering(CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_SEPARATE, CONSTANT_PIXELSIZE_PARASITEMINIMUM_GIARDIA_SEPARATE, CONSTANT_PIXELSIZE_PARASITEMAXIMUM_GIARDIA_SEPARATE, CONSTANT_PIXELSIZE_PARASITEMAXIMUM_GIARDIA_SEPARATE, false).ApplyInPlace(objectBitmapBinaryGiardia);
                        //    blobCounterGiardia = new AForge.Imaging.BlobCounter(objectBitmapBinaryGiardia);
                        //    blobCounterGiardia.ObjectsOrder = ObjectsOrder.YX;
                        //    blobCounterGiardia.ProcessImage(objectBitmapBinaryGiardia);
                        //    blobsGiardia = blobCounterGiardia.GetObjectsInformation();
                        //    int blobCountGiardia = blobsGiardia.Length;
                        //    if (blobCountGiardia > 350) { continue; }
                        //    List<SpeciesResult> giardiaImageResults = new List<SpeciesResult>();

                        //    #endregion

                        //    #region "*** Process Giardia Blobs ***"

                        //    for (int indexGiardia = 0; indexGiardia < blobCountGiardia; indexGiardia++)
                        //    {

                        //        try
                        //        {

                        //            #region "*** Declare Variables and Check Giardia Blob Location ***"

                        //            Bitmap objectBitmapParasiteGiardia = null;
                        //            IsPositiveTraining = false;
                        //            IsNegativeTraining = false;
                        //            if (cellCount[cellType] >= CONSTANT_GIARDIA_BLOBCOUNT) { indexGiardia = blobCountGiardia; continue; }
                        //            Blob blobGiardia = blobsGiardia[indexGiardia];
                        //            //if ((blobGiardia.Rectangle.Left < 400) || (blobGiardia.Rectangle.Left > 4512) || (blobGiardia.Rectangle.Top < 400) || (blobGiardia.Rectangle.Top > 3284)) { continue; }
                        //            string IDParasiteGiardia = caseFolder.Name + "_" + string.Format("{0:D3}", indexRegion) + "_" + string.Format("{0:D5}", blobGiardia.Rectangle.Left) + "_" + string.Format("{0:D5}", blobGiardia.Rectangle.Top);

                        //            #endregion

                        //            #region "*** Check if Object Inside Bubble ***"

                        //            //*** Check if Object Inside Bubble ***
                        //            bool IsBubble = false;
                        //            foreach (Blob blobGiardiaBubble in blobsGiardiaBubbles)
                        //            {
                        //                if (SharedFunctionController.IsPointInRectangle(blobGiardia.CenterOfGravity, blobGiardiaBubble.Rectangle) == true)
                        //                {
                        //                    IsBubble = true;
                        //                    break;
                        //                }
                        //            }
                        //            if (IsBubble == true)
                        //            {
                        //                continue;
                        //            }

                        //            #endregion

                        //            #region "*** Check Giardia Width, Height, Ratio, Fullness ***

                        //            bool IsPositiveGiardia = IsPossibleParasiteCheckSize(blobGiardia);
                        //            if (IsPositiveGiardia == true)
                        //            {
                        //                colorEdge = new int[4] { 0, 0, 0, 0 };
                        //                edgeWidth = new int[4] { 0, 0, 0, 0 };
                        //                colorEdge[0]++;
                        //                GIARDIA_BLOB_COUNT++;
                        //                if (GIARDIA_BLOB_COUNT > CONSTANT_GIARDIA_BLOBCOUNT) { indexGiardia = blobCountGiardia; indexGiardia = blobCountGiardia; continue; }
                        //            }

                        //            #endregion

                        //            #region "*** If Training Check for Known Positive or Negative Giardia ***"

                        //            if ((IsKeptPositives == true) && (IsTraining_Database == true) && (GetDatabaseIsParasitePositive(IDParasiteGiardia) == true))
                        //            {
                        //                IsPositiveTraining = true;
                        //            }
                        //            if ((IsKeptPositives == true) && (IsTraining_Database == true) && (GetDatabaseIsParasiteNegative(IDParasiteGiardia) == true))
                        //            {
                        //                IsNegativeTraining = true; IsPositiveGiardia = false;
                        //            }
                        //            if ((IsKeptPositivesOnly == true) && (IsTraining_Database == true) && (IsPositiveTraining == false) && (stringTableSuffix == "")) { IsPositiveGiardia = false; }

                        //            //if ((IDParasiteGiardia == "003424_GIAR3_038_01085_03333") || (IDParasiteGiardia == "003424_GIAR3_025_03053_01129") || (IDParasiteGiardia == "003424_GIAR3_033_03997_02785"))
                        //            //{
                        //            //    System.Diagnostics.Debug.Assert(true);
                        //            //}


                        //            #endregion

                        //            #region "*** Check Giardia Shape, AxisMajor, AxisMinor, Fullness, FocusContrast, Get Rotated Bitmaps, Get Perimeter Points ***

                        //            if ((IsPositiveGiardia == true) || (IsPositiveTraining == true))
                        //            {
                        //                colorEdge[1]++;
                        //                //*** Crop and Capture Giardia Image ***
                        //                objectRectangleParasiteNew = SharedFunctionController.GetValidRectangle(SharedFunctionController.GetNewRectangle(blobGiardia, .4), objectBitmapBinaryGiardia.Width, objectBitmapBinaryGiardia.Height);
                        //                Crop objectCropGiardia = new Crop(objectRectangleParasiteNew);
                        //                objectBitmapParasiteGiardia = objectCropGiardia.Apply(objectBitmap);
                        //                objectBitmapParasiteRotated = (Bitmap)objectBitmapParasiteGiardia.Clone();
                        //                IsPositiveGiardia = IsPossibleParasiteCheckShape(IDParasiteGiardia, cellType, ref perimeterPoints, ref objectBitmapParasiteRotated, ref axisMajor, ref axisMinor, ref fullness, ref focusContrast, ref edgePoints, ref objectBitmapParasiteBinary, CaseID, ref rotateAngle);
                        //                //if (IsPositiveGiardia == false) { IsPositiveTraining = false; }
                        //            }

                        //            //*** Check Giardia Shape Score ***
                        //            if ((IsPositiveGiardia == true) || (IsPositiveTraining == true))
                        //            {
                        //                colorEdge[2]++;
                        //                bool IsShapeThreshold = trainingController.IsPositiveFromBitmap(parasiteTrainingModels[cellType], parasiteTrainingModels[cellType].TrainingSetString[0], parasiteTrainingModels[cellType].TrainingSetThreshold, objectBitmapParasiteRotated, ref thresholdScore);
                        //                edgeWidth[0] = (int)thresholdScore;
                        //            }

                        //            #endregion

                        //            #region "*** If Possible Giardia Positive Save Result to List ***"

                        //            if ((IsPositiveGiardia == true) || (IsPositiveTraining == true))
                        //            {
                        //                cellCount[cellType]++;
                        //                IsPositiveImage = true;

                        //                //*** Add Raw Bitmap to List to get Mean Prediction ***
                        //                objectRectangleParasiteNew = SharedFunctionController.GetValidRectangle(SharedFunctionController.GetNewRectangle(blobGiardia, .1), objectBitmap.Width, objectBitmap.Height);
                        //                Crop objectCrop = new Crop(objectRectangleParasiteNew);
                        //                objectBitmapParasiteGiardia = objectCrop.Apply(objectBitmap);
                        //                fileName = string.Format("{0:D2}", cellType) + "_" + string.Format("{0:D3}", (int)focusContrast) + "_" + string.Format("{0:D3}", indexRegion) + "_" + string.Format("{0:D5}", blobGiardia.Rectangle.Left) + "_" + string.Format("{0:D5}", blobGiardia.Rectangle.Top) + ".JPG";
                        //                string filePath = caseFolder.FullName + "\\" + SharedValueController.SUBFOLDER_PROCESSED + "\\" + fileName;
                        //                objectImageStatisticsColor = GetColorImageStatisticsFromBitmapGiardia(objectBitmapParasiteRotated, CONSTANT_PIXELSIZE_COLORHEIGHT_GIARDIA, perimeterPoints[perimeterPoints.Length - indexPointColor - 1], perimeterPoints[indexPointColor]);
                                
                        //                if (IsTraining_Database == true)
                        //                {
                        //                    filePath = GetFileSavePath(directoryImages, CaseID, IDParasiteGiardia, stringTableSuffix, true, IsPositiveTraining, IsNegativeTraining);
                        //                }
                        //                else
                        //                {
                        //                    filePath = caseFolder.FullName + "\\" + SharedValueController.SUBFOLDER_GIARDIA + "\\" + IDParasiteGiardia;
                        //                }
                        //                //giardiaImageResults.Add(new SpeciesResult(CaseID, indexRegion, cellType, IDParasiteGiardia, cellType, blobGiardia.Rectangle, blobGiardia.Fullness, fullness, focusContrast, axisMinor, axisMajor, SharedFunctionController.GetCircleVariance(edgePoints), blobCountGiardia, objectImageStatisticsColor, colorEdge, edgeWidth, objectBitmapParasiteRotated, filePath));
                        //                giardiaImageResults.Add(new SpeciesResult(CaseID, RequestID, indexRegion, cellType, IDParasiteGiardia, cellType, blobGiardia.Rectangle, blobGiardia.Fullness, fullness, focusContrast, axisMinor, axisMajor, SharedFunctionController.GetCircleVariance(edgePoints), blobCountGiardia, objectImageStatisticsColor, colorEdge, edgeWidth, objectBitmapParasiteGiardia, filePath));
                                            
                        //            }

                        //            #endregion
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            //WriteToLog(" Case: " + CaseID + ", Function IsPossibleParasiteCheckShape, " + ex.ToString());
                        //            string s = ex.ToString();
                        //        }
                        //    }
                        //    #region "*** Save Entire Image Results ***"

                        //   // stringSQL = "Update AnalysisRequests Set GiardiaBlobCount = " + Convert.ToString(GIARDIA_BLOB_COUNT) + " Where CaseID = '" + CaseID + "' and RequestID = " + RequestID;
                        //    //dataController.ExecuteQuery(stringSQL);


                        //    if (IsPositiveImage == true)
                        //    {
                        //        double blobCountLeft = 1;
                        //        double blobCountRight = 1;
                        //        double blobCountTop = 1;
                        //        double blobCountBottom = 1;

                        //        if (giardiaImageResults.Count > 5)
                        //        {
                        //            foreach (SpeciesResult giardiaResult in giardiaImageResults)
                        //            {
                        //                if (giardiaResult.objectRectangleBlob.Left < 2450) { blobCountLeft++; }
                        //                else { blobCountRight++; }
                        //                if (giardiaResult.objectRectangleBlob.Top < 1835) { blobCountTop++; }
                        //                else { blobCountBottom++; }
                        //            }
                        //        }

                        //        #region "*** If Ratio of Giardia on Left, Right, Top, Bottom of Image Valid then Save Results ***"

                        //        if ((SharedFunctionController.IsBetweenValues(blobCountLeft / blobCountRight, .33, 3)) && (SharedFunctionController.IsBetweenValues(blobCountTop / blobCountBottom, .33, 3)))
                        //        {
                        //            foreach (SpeciesResult giardiaResult in giardiaImageResults)
                        //            {
                        //                Bitmap objectBitmapParasiteGiardia = giardiaResult.BitmapResult;
                        //                objectImageStatisticsColor = giardiaResult.objectStatisticsParasite;

                        //                if (IsTraining_Database == true)
                        //                {
                        //                    dataTableAnalysisParasiteFecals.Rows.Add(GetDataRow_AnalysisParasiteFecals(giardiaResult.CaseID, giardiaResult.RequestID, giardiaResult.indexRegion, giardiaResult.cellType, giardiaResult.IDParasite, giardiaResult.indexParasiteType, giardiaResult.objectRectangleBlob, giardiaResult.blobFullness, giardiaResult.fullness, giardiaResult.focusContrast, giardiaResult.axisMinor, giardiaResult.axisMajor, giardiaResult.circleVariance, giardiaResult.blobCountInterior, giardiaResult.objectStatisticsParasite, giardiaResult.colorEdge, giardiaResult.edgeWidth));
                        //                    Rectangle objectRectangleParasite = giardiaResult.objectRectangleBlob;
                        //                    objectBitmapGraphics.DrawEllipse(new Pen(Color.Blue, 4), objectRectangleParasite.X - 35, objectRectangleParasite.Y - 35, objectRectangleParasite.Width + 70, objectRectangleParasite.Height + 70);
                        //                }
                        //                else
                        //                {
                        //                    stringSQL = "Insert Into _AnalysisParasiteFecals (CaseID, RequestID, Region, ResultValue, IDParasite, BlobTop, BlobLeft, BlobFullness, Fullness, FocusContrast, AxisMajor, CircleVariance, " +
                        //                        "MeanRed, MeanGreen, MeanBlue, DeviationRed, DeviationGreen, DeviationBlue, Red11, Green11, Blue11, EdgeWidth1, EdgeWidth2) Values ('" +
                        //                        CaseID + "'" + st(Convert.ToInt32(RequestID)) + st(indexRegion) + st((int)FECALPARASITE_TYPE.Giardia) + st(giardiaResult.IDParasite) + st(giardiaResult.objectRectangleBlob.Top) + st(giardiaResult.objectRectangleBlob.Left) +
                        //                        st(giardiaResult.blobFullness) + st(giardiaResult.fullness) + st(giardiaResult.focusContrast) + st(giardiaResult.axisMajor) + st(giardiaResult.circleVariance) +
                        //                        st(objectImageStatisticsColor.Red.Mean) + st(objectImageStatisticsColor.Green.Mean) + st(objectImageStatisticsColor.Blue.Mean) + st(objectImageStatisticsColor.Red.StdDev) + st(objectImageStatisticsColor.Green.StdDev) + st(objectImageStatisticsColor.Blue.StdDev) +
                        //                        st(objectImageStatisticsColor.Red.GetRange(.78).Min) + st(objectImageStatisticsColor.Green.GetRange(.78).Min) + st(objectImageStatisticsColor.Blue.GetRange(.78).Min) + st(giardiaResult.edgeWidth[0]) + st(giardiaResult.blobCountInterior) + ")";
                        //                    dataController.ExecuteQuery(stringSQL);
                        //                }
                        //                objectBitmapParasiteGiardia.Save(giardiaResult.filePath + ".JPG", jpgEncoder, bitmapEncoderParameters);
                        //                objectBitmapParasiteGiardia.Dispose();
                        //            }
                        //        }
                                
                        //        #endregion

                        //        //objectBitmap.Save("c:\\temp\\images\\" + caseFolder.Name + "_" + string.Format("{0:D3}", indexRegion) + ".jpg", jpgEncoder, bitmapEncoderParameters);
                        //        //objectBitmapBinaryGiardia.Save("c:\\temp\\images\\" + caseFolder.Name + "_" + string.Format("{0:D3}", indexRegion) + "_Binary.jpg",jpgEncoder,bitmapEncoderParameters);

                        //    }

                        //    #endregion

                        //    #region "*** Dispose of Variables ***"

                        //    giardiaImageResults.Clear();
                        //    objectBitmapBinaryGiardia.Dispose();
                        //    objectBitmapBinaryGiardia = null;

                        //    #endregion

                        //    #endregion
                        //}
                        
                        #endregion "*** End Giardia Test ***"

                        #region "*** Process Binary Resized Image ***

                        new BlobsFiltering(CONSTANT_PIXELSIZE_PARASITEMINIMUM_RESIZED_SEPARATE, CONSTANT_PIXELSIZE_PARASITEMINIMUM_RESIZED_SEPARATE, CONSTANT_PIXELSIZE_PARASITEMAXIMUM_RESIZED_SEPARATE, CONSTANT_PIXELSIZE_PARASITEMAXIMUM_RESIZED_SEPARATE, false).ApplyInPlace(objectBitmapBinary);
                        new BlobsFiltering(CONSTANT_PIXELSIZE_PARASITEMINIMUM_RESIZED_COMBINED, CONSTANT_PIXELSIZE_PARASITEMINIMUM_RESIZED_COMBINED, CONSTANT_PIXELSIZE_PARASITEMAXIMUM_RESIZED_COMBINED, CONSTANT_PIXELSIZE_PARASITEMAXIMUM_RESIZED_COMBINED, true).ApplyInPlace(objectBitmapBinary);
                        objectBitmapBinary = new ResizeBilinear(objectBitmap.Width, objectBitmap.Height).Apply(objectBitmapBinary);
                             
                        #endregion

                        #region "*** Destroy Objects ***"

                        BlobCounter blobCounter = new AForge.Imaging.BlobCounter(objectBitmapBinary);
                        blobCounter.ObjectsOrder = ObjectsOrder.YX;
                        blobCounter.ProcessImage(objectBitmapBinary);
                        Blob[] blobs = blobCounter.GetObjectsInformation();
                        int blobCount = blobs.Length;
                        objectBitmapContrast.Dispose();
                        objectBitmapContrast = null;
                        objectBitmapContrastResize.Dispose();
                        objectBitmapContrastResize = null;
                        objectBitmapGrayscale.Dispose();
                        objectBitmapGrayscale = null;
                        objectContrastStretch = null;
                        GC.Collect();
                        GC.GetTotalMemory(true);
                        GC.WaitForPendingFinalizers();
                        //WriteToLog("Filters for " + caseFile.Name + " Completed");
                            
                        //*** Temp Code ***

                        #endregion

                        //*** Begin Loop Through Parasite Types and Filtering ***
                        #region "*** Begin Loop Through Blobs and Filter ***"

                        for (int index = 0; index < blobCount; index++)
                        {
                            try
                            {

                                #region "*** Set Blob, Rectangle, Center Point ***"

                                Blob blob = blobs[index];
                                IntPoint pointCenter = (IntPoint)blob.CenterOfGravity;
                                cellType = cellTypeStart;
                                string IDParasite = caseFolder.Name + "_" + string.Format("{0:D3}", indexRegion) + "_" + string.Format("{0:D5}", blob.Rectangle.Left) + "_" + string.Format("{0:D5}", blob.Rectangle.Top);
                                string IDParasite_Alternate = caseFile.Name + "_" + string.Format("{0:D5}", blob.Rectangle.Left) + "_" + string.Format("{0:D5}", blob.Rectangle.Top);
                                bool IsPositive = false;
                                IsPositiveTraining = false;
                                IsNegativeTraining = false;
                                int blobCountInterior = 0;

                                //*** Temp Code ***
                                //if (IDParasite.IndexOf("04893_00885") > -1)
                                //{
                                //   System.Diagnostics.Debug.Assert(true);
                                //}

                                #endregion

                                while ((IsPositive == false) && (cellType <= cellTypeEnd))
                                {
                                    IsPositive = false;
                                    colorEdge = new int[4]; edgeWidth = new int[4];
                                    if (cellCount[cellType] >= limitPositives) { cellType++; continue; }
                                    if (((cellType == (int)FECALPARASITE_TYPE.Eimeria) && (cellCount[(int)FECALPARASITE_TYPE.Isospora] > 5)) || ((cellType == (int)FECALPARASITE_TYPE.Isospora) && (cellCount[(int)FECALPARASITE_TYPE.Eimeria] > 5))) { cellType++; continue; }
                                    try { if (Parasites[cellType].ParasiteTypes.Length == 0) { cellType++;  continue; } }
                                    catch { cellType++; continue; }
                                                        
                                    //*** Begin Loop through Types for Current Parasite ***
                                    for (int indexParasiteType = 0; indexParasiteType < Parasites[cellType].ParasiteTypes.Length; indexParasiteType++)
                                    {

                                        #region "*** Set Parasite Type ***"

                                        //*** End Temp Code
                                        objectSpecies = Parasites[cellType].ParasiteTypes[indexParasiteType];
                                            
                                        #endregion

                                        #region "*** Check Width, Height, Ratio, Fullness ***

                                        //CodeTimer.CodeTime.Start("Parasite Size");

                                        IsPositive = IsPossibleParasiteCheckSize(blob);

                                        //*** Check for Positive Training ***
                                        
                                        

                                        //CodeTimer.CodeTime.End("Parasite Size");

                                        #endregion

                                        #region "*** Check Shape, AxisMajor, AxisMinor, Fullness, FocusContrast, Get Rotated Bitmaps, Get Perimeter Points ***

                                        //CodeTimer.CodeTime.Start("Parasite Shape");

                                        if ((IsKeptPositives == true) && (IsTraining_Database == true) && (GetDatabaseIsParasitePositive(IDParasite) == true))
                                        {
                                            IsPositiveTraining = true;
                                        }

                                        if ((IsPositive == true) || (IsPositiveTraining == true))
                                        {
                                            //*** Temp Code ***
                                            edgeWidth[1] = 1;
                                            objectRectangleParasiteNew = SharedFunctionController.GetValidRectangle(SharedFunctionController.GetNewRectangle(blob, .4), objectBitmap.Width, objectBitmap.Height);
                                            Crop objectCrop = new Crop(objectRectangleParasiteNew);
                                            objectBitmapParasite = objectCrop.Apply(objectBitmap);
                                            objectBitmapParasiteRotated = (Bitmap)objectBitmapParasite.Clone();
                                            IsPositive = IsPossibleParasiteCheckShape(IDParasite, cellType, ref perimeterPoints, ref objectBitmapParasiteRotated, ref axisMajor, ref axisMinor, ref fullness, ref focusContrast, ref edgePoints, ref objectBitmapParasiteBinary, CaseID, ref rotateAngle);
                                        }

                                        //CodeTimer.CodeTime.End("Parasite Shape");


                                        #endregion

                                        #region "*** Check Interior for Parasite Type ***

                                        //CodeTimer.CodeTime.Start("Parasite Interior");

                                        if ((IsPositive == true) || (IsPositiveTraining == true))
                                        {
                                            //*** Temp Code ***
                                            edgeWidth[1] = 2;
                                            Bitmap objectBitmapParasiteOriginal = (Bitmap)objectBitmapParasiteRotated.Clone();
                                            if ((FECALPARASITE_TYPE)cellType == FECALPARASITE_TYPE.Isospora)
                                            {
                                                IsPositive = IsPossibleParasiteCheckInterior(IDParasite, cellType, objectBitmapParasiteOriginal, perimeterPoints, ref blobCountInterior, ref colorEdge, ref edgeWidth, stringTableSuffix, IsTraining, ref objectBitmapParasiteBinary, FECALPARASITECONDITION_GRAYSCALE_CONVERSION.Blue);
                                                if (IsPositive == false) { IsPositive = IsPossibleParasiteCheckInterior(IDParasite, cellType, objectBitmapParasiteOriginal, perimeterPoints, ref blobCountInterior, ref colorEdge, ref edgeWidth, stringTableSuffix, IsTraining, ref objectBitmapParasiteBinary, FECALPARASITECONDITION_GRAYSCALE_CONVERSION.Grayscale); }
                                            }
                                            else
                                            {
                                                IsPositive = IsPossibleParasiteCheckInterior(IDParasite, cellType, objectBitmapParasiteOriginal, perimeterPoints, ref blobCountInterior, ref colorEdge, ref edgeWidth, stringTableSuffix, IsTraining, ref objectBitmapParasiteBinary, FECALPARASITECONDITION_GRAYSCALE_CONVERSION.Grayscale);
                                            }
                                            objectBitmapParasiteOriginal.Dispose(); objectBitmapParasiteOriginal = null;
                                        }

                                        //CodeTimer.CodeTime.End("Parasite Interior");


                                        #endregion

                                        #region "*** Check Shape Patterns for Parasite Type ***"

                                        //CodeTimer.CodeTime.Start("Parasite Pixel Patterns");


                                        if ((IsPositive == true) || (IsPositiveTraining == true))
                                        {
                                            try
                                            {
                                                //*** Temp Code ***
                                                edgeWidth[1] = 3;
                                                if (parasiteTrainingModels[cellType].TrainingWeightsPositive.Length > 0)
                                                {
                                                    IsPositive = trainingController.IsPositiveFromBitmap(parasiteTrainingModels[cellType], parasiteTrainingModels[cellType].TrainingSetString[0], parasiteTrainingModels[cellType].TrainingSetThreshold, objectBitmapParasiteRotated, ref thresholdScore);
                                                    edgeWidth[0] = (int)thresholdScore;
                                                    //*** Temp Code ***
                                                    //IsPositive = true;
                                                }
                                            }
                                            catch { }
                                        }

                                        //CodeTimer.CodeTime.End("Parasite Pixel Patterns");

                                        //CodeTimer.CodeTime.Start("Parasite Shape Patterns");


                                        if ((IsPositive == true) || (IsPositiveTraining == true))
                                        {
                                            //*** Temp Code ***
                                            edgeWidth[1] = 4;
                                            IsPositive = IsPossibleParasiteCheckShapePatterns(IDParasite, perimeterPoints, cellType, stringTableSuffix, CaseID, IsTraining);
                                        }

                                        //CodeTimer.CodeTime.End("Parasite Shape Patterns");


                                        #endregion

                                        #region "*** Check Color for Parasite Type ***"

                                        //CodeTimer.CodeTime.Start("Parasite Color");
                                        

                                        if ((IsPositive == true) || (IsPositiveTraining == true))
                                        {
                                            //*** Temp Code ***
                                            edgeWidth[1] = 5;
                                            if ((objectSpecies.SpeciesConditions_Color.Length > 0) && (objectBitmapParasiteRotated != null))
                                            {
                                                objectImageStatisticsColor = SharedFunctionController.GetColorImageStatisticsFromBitmap(objectBitmapParasiteRotated, DOUBLE_WIDTHPERCENT);
                                                IsPositive = IsPossibleParasiteCheckColor(objectImageStatisticsColor);
                                            }
                                        }

                                        //CodeTimer.CodeTime.End("Parasite Color");


                                        #endregion

                                        #region "*** Check For Saving Positive Parasites ***"

                                        

                                        if ((IsPositive == true) && (IsKeptPositives == true) && (IsTraining_Database == true) && (IsPositiveTraining == false) && (GetDatabaseIsParasiteNegative(IDParasite) == true)) 
                                        { 
                                            IsNegativeTraining = true; 
                                        }

                                        if ((IsKeptPositivesOnly == true) && (IsTraining_Database == true) && (IsPositiveTraining == false) && (stringTableSuffix == ""))
                                        {   
                                            IsPositive = false; 
                                        }

                                        #endregion

                                        #region "*** If Possible Positive Save Result ***"

                                        //CodeTimer.CodeTime.Start("Parasite Positive");


                                        if ((IsPositive == true) || (IsPositiveTraining == true))
                                        {

                                            cellCount[cellType]++; IsPositiveImage = true; colorEdge[0] = (int)thresholdScore;

                                            #region "*** If Live Scan Save Values for Positive and Bitmap ***"

                                            if (IsTraining_Database == false)
                                            {
                                                focusInteger = 70 - (int)focusContrast;
                                                fileName = string.Format("{0:D2}", cellType) + "_" + string.Format("{0:D3}", indexRegion) + "_" + string.Format("{0:D5}", blob.Rectangle.Left) + "_" + string.Format("{0:D5}", blob.Rectangle.Top) + ".JPG";
                                                    
                                                //*** Save Bitmap to Processed Folder for Case ***
                                                try
                                                {
                                                    string filePath = caseFolder.FullName + "\\" + SharedValueController.SUBFOLDER_PROCESSED + "\\" + fileName;
                                                    pixelBlobCenterX = blob.Rectangle.Left + (int)(blob.Rectangle.Width / 2);
                                                    pixelBlobCenterY = blob.Rectangle.Top + (int)(blob.Rectangle.Height / 2);
                                                    if ((cellType != (int)FECALPARASITE_TYPE.Eimeria) || (cellType != (int)FECALPARASITE_TYPE.Isospora))
                                                    {
                                                        objectRectangleParasiteNew = new Rectangle(pixelBlobCenterX - IMAGE_RESULT_HALF, pixelBlobCenterY - IMAGE_RESULT_HALF, IMAGE_RESULT_SIZE, IMAGE_RESULT_SIZE);
                                                    }
                                                    else
                                                    {
                                                        int imageResultHalf = (int)(IMAGE_RESULT_HALF * .666);
                                                        objectRectangleParasiteNew = new Rectangle(pixelBlobCenterX - imageResultHalf, pixelBlobCenterY - imageResultHalf, imageResultHalf * 2, imageResultHalf * 2);
                                                    }
                                                     
                                                    Rectangle objectRectangleParasite = blob.Rectangle;
                                                    objectBitmapGraphics.DrawEllipse(new Pen(Color.Blue, 4), objectRectangleParasite.X - 30, objectRectangleParasite.Y - 30, objectRectangleParasite.Width + 60, objectRectangleParasite.Height + 60);
                                                    int parasiteHeight = (int)Math.Round((double)objectRectangleParasite.Height * .45, 0);
                                                    int parasiteWidth = (int)Math.Round((double)objectRectangleParasite.Width * .45, 0);
                                                    string parasiteString = Convert.ToString(parasiteWidth) + "x" + Convert.ToString(parasiteHeight) + " um";
                                                    objectBitmapGraphics.DrawString(parasiteString, objectFont, SystemBrushes.WindowText, pixelBlobCenterX - 30, objectRectangleParasite.Y + objectRectangleParasite.Height + 2);
                                   
                                                   // objectRectangleParasiteNew = new Rectangle(pixelBlobCenterX - IMAGE_RESULT_SIZE, pixelBlobCenterY - IMAGE_RESULT_SIZE, IMAGE_RESULT_SIZE * 2, IMAGE_RESULT_SIZE * 2);
                                                    objectBitmapParasite = new Crop(objectRectangleParasiteNew).Apply(objectBitmap);
                                                    objectBitmapParasite = SharedFunctionController.GetBitmapWithoutBlack(objectBitmapParasite);
                                                    objectBitmapParasite = SharedFunctionController.GetBitmapAdjustedColor(objectBitmapParasite);
                                                    new Sharpen().ApplyInPlace(objectBitmapParasite);
                                                    objectBitmapParasite.Save(filePath);
                                                    InsertDatabaseResult(CaseID, RequestID, cellType, fileName, focusContrast, thresholdScore, parasiteTrainingModels[cellType].TrainingSetThreshold);
                                                    
                                                }
                                                catch { }

                                            }
                                           

                                            #endregion

                                            #region "*** If Training then Save Record and Image ***"

                                            if (IsTraining == true)
                                            {
                                                try
                                                {
                                                    objectImageStatisticsColor = SharedFunctionController.GetColorImageStatisticsFromBitmap(objectBitmapParasiteRotated, DOUBLE_WIDTHPERCENT); 
                                                    circleVariance = SharedFunctionController.GetCircleVariance(edgePoints);
                                                    dataTableAnalysisParasiteFecals.Rows.Add(GetDataRow_AnalysisParasiteFecals(CaseID, RequestID, indexRegion, cellType, IDParasite, indexParasiteType, blob.Rectangle, blob.Fullness, fullness, focusContrast, axisMinor, axisMajor, circleVariance, blobCountInterior, objectImageStatisticsColor,colorEdge,edgeWidth));
                                                    if (IsSavedImages == true)
                                                    {
                                                        string filePath = GetFileSavePath(directoryImages, CaseID, IDParasite, stringTableSuffix, false, IsPositiveTraining, IsNegativeTraining);
                                                        try 
                                                        {
                                                            SharedFunctionController.GetBitmapAdjustedColor(objectBitmapParasite);
                                                            //objectBitmapParasiteRotated.Save(filePath + ".jpg",jpgEncoder,bitmapEncoderParameters);
                                                            objectBitmapParasite.Save(filePath + ".jpg",jpgEncoder,bitmapEncoderParameters);
                                                            //objectBitmapParasiteBinary.Save(filePath + "_Binary.jpg",jpgEncoder,bitmapEncoderParameters); 
                                                            //*** Begin Temp Code ***
                                                                
                                                            //*** End Temp Code ***
                                                        }
                                                        catch { }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    WriteToLog(" Case: " + CaseID + ", Parasite " + IDParasite + ", " + ex.ToString());
                                                }
                                            }

                                            //*** Highlight Parasite ***
                                            //Rectangle objectRectangleParasite = blob.Rectangle;
                                            //objectBitmapGraphics.DrawEllipse(new Pen(Color.Blue, 4), objectRectangleParasite.X - 25, objectRectangleParasite.Y - 25, objectRectangleParasite.Width + 50, objectRectangleParasite.Height + 50);
                                         
                                            #endregion

                                            
                                        }
                                        #endregion
                                    }
                                    //CodeTimer.CodeTime.End("Parasite Positive");

                                    //*** End Loop through Types for Current Parasite ***
                                    cellType++;
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteToLog(" Case: " + CaseID + ", " + ex.ToString());
                            }
                        }
                        #endregion "*** End Loop Through Blobs and Filter ***"
                        //*** End Loop Through Parasite Types and Filtering ***

                        #region "*** Display Bitmap for Debugging ***"

                        //if (IsPositiveImage == true)
                        //{
                        //    objectBitmap = SharedFunctionController.GetBitmapAdjustedColor(objectBitmap);
                        //    objectBitmap.Save("c:\\temp\\images\\" + CaseID + "_" + string.Format("{0:D3}", indexRegion) + ".jpg",jpgEncoder,bitmapEncoderParameters);
                        // }
                            
                        #endregion

                        #region "*** Dispose of Variables ***"

                        try
                        {
                            objectBitmap.Dispose();
                            objectBitmap = null;
                            objectBitmapBinary.Dispose();
                            objectBitmapBinary = null;
                            objectBitmapParasite.Dispose();
                            objectBitmapParasite = null;
                            objectBitmapParasiteRotated.Dispose();
                            objectBitmapParasiteRotated = null;
                            objectBitmapParasiteGrayscale.Dispose();
                            objectBitmapParasiteGrayscale = null;
                        }
                        catch { }
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.GetTotalMemory(true);
                        //WriteToLog("Image " + caseFile.Name + " Ended");
                            

                        #endregion
                       
                    }

                    #region "*** Write to  Database ***"

                    UpdateDatabase(CaseID, caseSQL, IsTraining_Database);
                   
                    #endregion

                }
                catch (Exception ex)
                {
                    WriteToLog(ex.ToString());
                }
                //*** End Loop Through Case Files and Process ***

                if ((IsTraining == false) && (IsTraining_Database == false))
                {
                    DateTime dateTimeEnd = System.DateTime.Now.AddSeconds(120);

                    bool IsCaseCompleted = false;
                    while ((IsCaseCompleted == false) && (System.DateTime.Now < dateTimeEnd))
                    {
                        IsCaseCompleted = true;
                        foreach (FileInfo caseFile in caseFiles)
                        {
                            if (SharedFunctionController.IsFileProcessed(caseFolder.FullName + "\\" + SharedValueController.STRING_IMAGESPROCESSEDFILENAME, caseFile.Name) == false)
                            {
                                IsCaseCompleted = false;
                            }
                        }
                    }

                    string stringSQL = "Select * from _AnalysisParasiteFecals Where CaseID = '" + CaseID + "' and RequestID = " + RequestID + " AND ResultValue = " + Convert.ToString((int)FECALPARASITE_TYPE.Giardia);
                    SqlDataReader sqlReader = dataController.GetDataReaderFromQuery(stringSQL);
                    string[] stringSQLStatements = IsCasePositiveGiardia(CaseID, RequestID, caseFolder.FullName + "\\" + SharedValueController.SUBFOLDER_PROCESSED + "\\", caseFolder.FullName + "\\" + SharedValueController.SUBFOLDER_GIARDIA + "\\", SharedValueController.CASE_GIARDIA_PREDICTION_MINIMUM, sqlReader);
                    sqlReader = null;

                    try
                    {
                        for (int index = 0; index < stringSQLStatements.Length; index++)
                        {
                            //dataController.ExecuteQuery(stringSQLStatements[index]);
                        }
                    }
                    catch { }

                    dataController.ExecuteQuery("Update AnalysisCases Set CaseCompleted = GetDate() Where CaseID = '" + CaseID + "'");
                    //dataController.ExecuteQuery("Delete from _AnalysisParasiteFecals Where CaseID = '" + CaseID + "' and RequestID = " + RequestID);
                    //Environment.Exit(0);
                }
                else
                {
                    dataController.ExecuteQuery("Update AnalysisCases Set CaseCompleted = GetDate() Where CaseID = '" + CaseID + "'");
                }
                
            }

            #region "*** Dispose of Database Values When Completed ***"

            try
            {
                dataController.ObjectDispose();
                dataController = null;
                dataTableAnalysisParasiteFecals.Dispose();
                dataTableAnalysisParasiteFecalsShapesInterior.Dispose();
                dataTableAnalysisParasiteFecalsShapesAnalysis.Dispose();
            }
            catch { }

            //CodeTimer.CodeTime.Report();

            #endregion

            #endregion "*** End Loop through Case Folders and Filter Images ***"
        }

        #region "*** Private Boolean Filter Methods ***"

        private bool IsPossibleParasiteCheckSize(Blob blobParasite)
        {
            //*** Check Shape Ratio Width to Height ***
            Rectangle objectRectangleParasite = blobParasite.Rectangle;
            if (SharedFunctionController.IsBetweenValues(objectRectangleParasite.Width, objectSpecies.ShapeBlobWidthMinimum, objectSpecies.ShapeBlobWidthMaximum) == false) { return false; }
            if (SharedFunctionController.IsBetweenValues(objectRectangleParasite.Height, objectSpecies.ShapeBlobHeightMinimum, objectSpecies.ShapeBlobHeightMaximum) == false) { return false; }
            if (SharedFunctionController.IsBetweenValues((double)((double)objectRectangleParasite.Width / (double)objectRectangleParasite.Height), objectSpecies.ShapeBlobRatioMinimum, objectSpecies.ShapeBlobRatioMaximum) == false) { return false; }
            if (SharedFunctionController.IsBetweenValues(blobParasite.Fullness, objectSpecies.ShapeBlobFullnessMinimum, objectSpecies.ShapeBlobFullnessMaximum) == false) { return false; }
            return true;
        }

        private bool IsPossibleParasiteCheckShape(string IDParasite, int cellType, ref IntPoint[] perimeterPoints, ref Bitmap objectBitmapParasiteRotated, ref double axisMajor, ref double axisMinor, ref double fullness, ref double focusContrast, ref List<IntPoint> edgePoints, ref Bitmap objectBitmapParasiteBinary, string CaseID, ref double rotateAngle)
        {
            bool UseMaximumChange = true;
            const double ROTATE_ANGLE_ADJUSTMENT = 7.5;
            if (((FECALPARASITE_TYPE)cellType == FECALPARASITE_TYPE.Giardia)) { UseMaximumChange = false; }
            IntPoint pointCenter = new IntPoint();
            Bitmap objectBitmapParasiteContrast = new ContrastStretch().Apply(objectBitmapParasiteRotated);
            try
            {
                //*** Get Blob from Parasite Bitmap ***
                double[] blobsCircleVariance = null;
                int pixelOffsetColumn = 0; int pixelOffsetRow = 0;
                Blob blobParasite = SharedFunctionController.GetBlobFromBitmap(objectBitmapParasiteContrast, ref edgePoints, ref objectBitmapParasiteBinary, ref fullness, ref pixelOffsetColumn, ref pixelOffsetRow, objectSpecies.GrayscalePercent, objectSpecies.SpeciesChannelColor);
                if (blobParasite == null) { return false; }
                pointCenter = (IntPoint)blobParasite.CenterOfGravity;
                Crop objectCropParasite = new Crop(blobParasite.Rectangle);
                objectBitmapParasiteContrast = objectCropParasite.Apply(objectBitmapParasiteContrast);
                objectBitmapParasiteRotated = objectCropParasite.Apply(objectBitmapParasiteRotated);
                Bitmap objectBitmapParasiteGrayscale = new Grayscale(0.2125, 0.7154, 0.0721).Apply(objectBitmapParasiteContrast);
                //*** Get Focus Score and Invert Parasite ***
                focusContrast = SharedFunctionController.GetFocusValue(objectBitmapParasiteGrayscale);


                #region "*** Get Blob from Parasite Bitmap Binary ***"

                if (((FECALPARASITE_TYPE)cellType != FECALPARASITE_TYPE.Giardia))
                {
                    blobParasite = SharedFunctionController.GetBlobsFromBitmapBinary(ref objectBitmapParasiteBinary, ref edgePoints, ref blobsCircleVariance, 1, 1, true, false, true, true)[0];
                }
                else
                {
                    blobParasite = SharedFunctionController.GetBlobsFromBitmapBinary(ref objectBitmapParasiteBinary, ref edgePoints, ref blobsCircleVariance, 0, 0, true, false, true, true)[0];
                }
                objectBitmapParasiteBinary = new Crop(blobParasite.Rectangle).Apply(objectBitmapParasiteBinary);
                objectBitmapParasiteRotated = new Crop(blobParasite.Rectangle).Apply(objectBitmapParasiteRotated);
                //*** Rotate Binary Image First Time ***
                double doubleAngle = 0 - SharedFunctionController.GetShapeAxis(SharedFunctionController.GetPerimeterPointsFromEdges(edgePoints, blobParasite.Rectangle.Height, blobParasite.Rectangle.Width, UseMaximumChange), ref axisMajor, ref axisMinor, false, objectSpecies.IsMajorAxisUsed);
                //*** Rotate Images First Time ****
                if (((FECALPARASITE_TYPE)cellType != FECALPARASITE_TYPE.Giardia))
                {
                    objectBitmapParasiteBinary = SharedFunctionController.rotateImage(objectBitmapParasiteBinary, (float)doubleAngle);
                    objectBitmapParasiteRotated = SharedFunctionController.rotateImage(objectBitmapParasiteRotated, (float)doubleAngle);
                }
                else
                {
                    objectBitmapParasiteBinary = SharedFunctionController.rotateImage2(objectBitmapParasiteBinary, (int)(doubleAngle), true);
                    objectBitmapParasiteRotated = SharedFunctionController.rotateImage2(objectBitmapParasiteRotated, (int)(doubleAngle), false);
                }
                perimeterPoints = SharedFunctionController.GetPerimeterPointsFromBitmap(objectBitmapParasiteBinary, ref pointCenter, UseMaximumChange);
                //*** Rotate Binary Image Second Time ***
                if (((FECALPARASITE_TYPE)cellType != FECALPARASITE_TYPE.Giardia))
                {
                    doubleAngle = 0 - SharedFunctionController.GetShapeAxis(perimeterPoints, ref axisMajor, ref axisMinor, true, SharedFunctionController.GetAlignmentFromBitmapBinary(objectBitmapParasiteBinary)) + ROTATE_ANGLE_ADJUSTMENT;
                }
                else
                {
                    //doubleAngle = 0 - SharedFunctionController.GetShapeAxisConcave(perimeterPoints, ref axisMajor, ref axisMinor);
                    doubleAngle = 0 - SharedFunctionController.GetShapeAxis(perimeterPoints, ref axisMajor, ref axisMinor, true, SharedFunctionController.GetAlignmentFromBitmapBinary(objectBitmapParasiteBinary)) + ROTATE_ANGLE_ADJUSTMENT;
                }
                if (((FECALPARASITE_TYPE)cellType != FECALPARASITE_TYPE.Giardia))
                {
                    objectBitmapParasiteBinary = SharedFunctionController.rotateImage(objectBitmapParasiteBinary, (float)doubleAngle);
                    objectBitmapParasiteRotated = SharedFunctionController.rotateImage(objectBitmapParasiteRotated, (float)doubleAngle);
                }
                else
                {
                    objectBitmapParasiteBinary = SharedFunctionController.rotateImage2(objectBitmapParasiteBinary, (int)(doubleAngle), true);
                    objectBitmapParasiteRotated = SharedFunctionController.rotateImage2(objectBitmapParasiteRotated, (int)(doubleAngle), false);
                  }

                try
                {
                    objectBitmapParasiteBinary = new ExtractChannel(RGB.R).Apply(objectBitmapParasiteBinary);
                    objectBitmapParasiteBinary = new OtsuThreshold().Apply(objectBitmapParasiteBinary);
                }
                catch { }

                if (((FECALPARASITE_TYPE)cellType != FECALPARASITE_TYPE.Giardia))
                {
                    blobParasite = SharedFunctionController.GetBlobsFromBitmapBinary(ref objectBitmapParasiteBinary, ref edgePoints, ref blobsCircleVariance, 1, 1, true, false, true, true)[0];
                }
                else
                {
                    blobParasite = SharedFunctionController.GetBlobsFromBitmapBinary(ref objectBitmapParasiteBinary, ref edgePoints, ref blobsCircleVariance, 0, 0, true, false, true, true)[0];
                }

                #endregion

                objectBitmapParasiteRotated = new Crop(blobParasite.Rectangle).Apply(objectBitmapParasiteRotated);
                ColorFiltering objectColorFiltering = new ColorFiltering(new IntRange(0, 2), new IntRange(0, 2), new IntRange(0, 2));
                objectColorFiltering.FillColor = new RGB(255, 255, 255);
                objectColorFiltering.FillOutsideRange = false;
                objectColorFiltering.ApplyInPlace(objectBitmapParasiteRotated);

                //*** Get Axis and Focal Points ***
                doubleAngle = 0 - SharedFunctionController.GetShapeAxis(perimeterPoints, ref axisMajor, ref axisMinor, true, objectSpecies.IsMajorAxisUsed);

                if (((FECALPARASITE_TYPE)cellType == FECALPARASITE_TYPE.Giardia) && (IsReversedGiardia(perimeterPoints) == true))
                //if (((FECALPARASITE_TYPE)cellType == FECALPARASITE_TYPE.Giardia) && (IsReversedGiardia(objectBitmapParasiteBinary) == true))
                {
                    //new Mirror(false, true).ApplyInPlace(objectBitmapParasiteBinary);
                    objectBitmapParasiteBinary.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    objectBitmapParasiteRotated.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }

                
               
            }
            catch (Exception ex)
            {
                WriteToLog(" Case: " + CaseID + ", Function IsPossibleParasiteCheckShape, " + ex.ToString());
                return false;
            }

            //*** Check for Valid Values and Return ***
            double axisRatio = axisMinor / axisMajor;
            bool returnValue = true;
            if (SharedFunctionController.IsBetweenValues(focusContrast, objectSpecies.ShapeFocusContrastMinimum, objectSpecies.ShapeFocusContrastMaximum) == false)
            {
                returnValue = false;
            }
            if (SharedFunctionController.IsBetweenValues(axisRatio, objectSpecies.ShapeAxisRatioMinimum, objectSpecies.ShapeAxisRatioMaximum) == false)
            {
                returnValue = false;
            }
            if (SharedFunctionController.IsBetweenValues(axisMajor, objectSpecies.ShapeAxisMajorMinimum, objectSpecies.ShapeAxisMajorMaximum) == false)
            {
                returnValue = false;
            }
            if (SharedFunctionController.IsBetweenValues(axisMinor, objectSpecies.ShapeAxisMinorMinimum, objectSpecies.ShapeAxisMinorMaximum) == false)
            {
                returnValue = false;
            }
            if (SharedFunctionController.IsBetweenValues(fullness, objectSpecies.ShapeFullnessMinimum, objectSpecies.ShapeFullnessMaximum) == false)
            {
                returnValue = false;
            }
            if (returnValue == true)
            {
                if (SharedFunctionController.IsMatchCheckCircleVariance(edgePoints, objectSpecies.ShapeCircleVarianceMinimum, objectSpecies.ShapeCircleVarianceMaximum) == false)
                {
                    returnValue = false;
                }
            }

            objectBitmapParasiteContrast.Dispose();
            if (returnValue == false) { return false; }

            objectBitmapParasiteBinary = new ResizeBilinear(objectSpecies.SpeciesWidth, objectSpecies.SpeciesHeight).Apply(objectBitmapParasiteBinary);
            try { new OtsuThreshold().ApplyInPlace(objectBitmapParasiteBinary); }
            catch { objectBitmapParasiteBinary = new ExtractChannel(RGB.R).Apply(objectBitmapParasiteBinary); objectBitmapParasiteBinary = new OtsuThreshold().Apply(objectBitmapParasiteBinary); }

            objectBitmapParasiteRotated = new ResizeBilinear(objectSpecies.SpeciesWidth, objectSpecies.SpeciesHeight).Apply(objectBitmapParasiteRotated);

            perimeterPoints = SharedFunctionController.GetPerimeterPointsFromBitmap(objectBitmapParasiteBinary, ref pointCenter, UseMaximumChange);
            return true;
        }

        private bool IsPossibleParasiteCheckInterior(string IDParasite, int cellType, Bitmap objectBitmapParasiteOriginal, IntPoint[] perimeterPoints, ref int blobCountInterior, ref int[] colorEdge, ref int[] edgeWidth, string stringTableSuffix, bool IsTraining, ref Bitmap objectBitmapBinaryParasite, FECALPARASITECONDITION_GRAYSCALE_CONVERSION grayscaleType)
        {

            #region "*** Declare Variables ***"

            ImageStatistics objectStatistics = null;
            FECALPARASITE_TYPE objectParasiteType = (FECALPARASITE_TYPE)cellType;
            bool IsPositiveParasite = true;
            bool IsPossibleParasiteCoccidia = false;
            List<IntPoint> edgePointsOuter = new List<IntPoint>();
            int integerPointCenter = Convert.ToInt32(objectBitmapParasiteOriginal.Width / 2);
            int integerThreshold = 0;
            blobCountInterior = 0;
            int integerPointBottom = objectBitmapParasiteOriginal.Height - 3;
            double blobDistancePrevious = 0; double blobAreaRatioPrevious = 0; double blobMinimumDistancePrevious = 0; double blobMaximumDistancePrevious = 0;
            
            System.Drawing.Point[] drawingPoints = null;
            double[] distancesEdge = new double[2];
            double grayMean = 0;
            blobCountInterior = 0;
            
            #endregion

            #region "*** Create Grayscale Bitmap from Original and Get Image Statistics ***"

            Bitmap objectBitmapGrayscaleParasite;
            if ((objectParasiteType == FECALPARASITE_TYPE.Isospora) && (grayscaleType == FECALPARASITECONDITION_GRAYSCALE_CONVERSION.Blue)) { objectBitmapGrayscaleParasite = new ExtractChannel(RGB.B).Apply(objectBitmapParasiteOriginal); }
            else { objectBitmapGrayscaleParasite = new Grayscale(0.2125, 0.7154, 0.0721).Apply(objectBitmapParasiteOriginal); }
            objectStatistics = new ImageStatistics(objectBitmapGrayscaleParasite);
            int rectangleCenterY = (int)(objectBitmapGrayscaleParasite.Height / 2);
            int rectangleCenterYOffset = (int)(objectBitmapGrayscaleParasite.Height / 12);
            
            #endregion

            #region "*** Check for Parasite Type and Set Interior Values ***"

            if ((objectParasiteType == FECALPARASITE_TYPE.Eimeria) || (objectParasiteType == FECALPARASITE_TYPE.Isospora))
            {
                #region "*** Set Interior Values for Isospora ***"

                integerThreshold = objectStatistics.Gray.GetRange(.15).Max;
                objectBitmapBinaryParasite = new Threshold(integerThreshold).Apply(objectBitmapGrayscaleParasite);
                new BlobsFiltering(10, 10, 91, 101, true).ApplyInPlace(objectBitmapBinaryParasite);
                new Opening().ApplyInPlace(objectBitmapBinaryParasite);
                
                #endregion
            }
            else
            {
                #region "*** Set Threshold and Create Binary Bitmap for other Parasites ***"

                SimplePosterization objectPosterization = new SimplePosterization();
                objectPosterization.PosterizationInterval = 16;
                objectPosterization.ApplyInPlace(objectBitmapGrayscaleParasite);
                //colorEdge[0] = SharedFunctionController.GetColorChangesEdge(objectBitmapGrayscaleParasite, 0, rectangleCenterY - rectangleCenterYOffset, 1, ref edgeWidth[0], ref colorEdge[2]);
                //colorEdge[1] = SharedFunctionController.GetColorChangesEdge(objectBitmapGrayscaleParasite, 0, rectangleCenterY + rectangleCenterYOffset, 1, ref edgeWidth[1], ref colorEdge[3]);
                AForge.Math.Histogram histogramGray = new ImageStatistics(objectBitmapGrayscaleParasite).Gray;
                //integerThreshold = histogramGray.Median + 1;
                //grayMean = histogramGray.Mean;
                //objectBitmapBinaryParasite = new Threshold(integerThreshold).Apply(objectBitmapGrayscaleParasite);
                objectBitmapBinaryParasite = new OtsuThreshold().Apply(objectBitmapGrayscaleParasite);
                new BlobsFiltering((int)(objectBitmapBinaryParasite.Width / 25), (int)(objectBitmapBinaryParasite.Height / 25), 1000000, 100000, false).ApplyInPlace(objectBitmapBinaryParasite);
               
                #endregion
            }

            #endregion

            #region "*** Declare Interior Processing Variables ***"

            
            AForge.Imaging.BlobCounter blobCounterParasite = new AForge.Imaging.BlobCounter(objectBitmapBinaryParasite);
            blobCounterParasite.ObjectsOrder = ObjectsOrder.Area;
            blobCounterParasite.ProcessImage(objectBitmapBinaryParasite);
            Blob[] blobsParasiteArray = blobCounterParasite.GetObjectsInformation();
            List<Blob> blobsCoccidia = new List<Blob>();
            int imageBinaryWidth = objectBitmapBinaryParasite.Width; int imageBinaryHeight = objectBitmapBinaryParasite.Height; int imageCenterX = (int)(objectBitmapBinaryParasite.Width / 2); int imageCenterY = (int)(objectBitmapBinaryParasite.Height / 2);
            int blobsParasiteCount = blobsParasiteArray.Length;
            bool IsCompletedInterior = false;
            int blobsParasiteBubblesCount = 0;
            List<IntPoint> edgePointsParasite = new List<IntPoint>();
            
            #endregion

            #region "*** Declare Variables for Coccidia ***"

            const int INTEGER_BLOBCOUNT_COCCIDIA = 3;
            int[] blobsArea = new int[INTEGER_BLOBCOUNT_COCCIDIA];
            double[] blobsFullness = new double[INTEGER_BLOBCOUNT_COCCIDIA];
            double[] blobsDistanceCenter = new double[INTEGER_BLOBCOUNT_COCCIDIA];
            int[] blobsOffsetX = new int[INTEGER_BLOBCOUNT_COCCIDIA];
            int[] blobsOffsetY = new int[INTEGER_BLOBCOUNT_COCCIDIA];
            int[] blobsX = new int[INTEGER_BLOBCOUNT_COCCIDIA];
            int[] blobsY = new int[INTEGER_BLOBCOUNT_COCCIDIA];
            int[] blobsWidth = new int[INTEGER_BLOBCOUNT_COCCIDIA];
            int[] blobsHeight = new int[INTEGER_BLOBCOUNT_COCCIDIA];
            double[] blobsCircleVariance = new double[INTEGER_BLOBCOUNT_COCCIDIA];
            List<IntPoint>[] blobsEdgePoints = new List<IntPoint>[INTEGER_BLOBCOUNT_COCCIDIA];
            List<Blob> blobsParasite = new List<Blob>();
            
            #endregion
            
            #region "*** Processing for Parasite Interior ***"

            //*** Begin Loop through Blobs for Binary Image ***

            while (IsCompletedInterior == false)
            {
                #region "*** If no Blobs Found, Change Grayscale and Binary Values to Check for Interior Bubble ***"

                if (blobsParasiteArray.Length == 0)
                {
                    blobsParasiteArray = GetBlobsInteriorFromBitmapGrayscale(objectBitmapParasiteOriginal, objectBitmapGrayscaleParasite, ref objectBitmapBinaryParasite, ref blobCounterParasite, (int)grayMean, 1);
                    blobsParasite.Clear();
                }

                //*** Create List of Valid Interior Blobs ***
                for (int index = 0; index < blobsParasiteArray.Length; index++)
                {
                    Blob blobParasiteCurrent = blobsParasiteArray[index];
                    //if ((objectParasiteType != FECALPARASITE_TYPE.Isospora) || (SharedFunctionController.IsRectangleTouchingEllipse(blobParasiteCurrent.Rectangle, imageBinaryWidth, imageBinaryHeight, ELLIPSE_WIDTH) == false))
                    //{
                        if (SharedFunctionController.IsRectangleTouchingEdge(blobParasiteCurrent.Rectangle, imageBinaryWidth, imageBinaryHeight) == false)
                        {
                            blobsParasite.Add(blobParasiteCurrent);
                        }
                    //}
                }

                #endregion

                foreach (Blob blobParasite in blobsParasite)
                {

                    #region "*** Declare and Increment Variables for Interior Blob ***"

                    blobCountInterior++;
                    double blobSizeRatio = (double)blobParasite.Rectangle.Width / (double)blobParasite.Rectangle.Height;
                    double blobDistanceCenter = SharedFunctionController.GetDistanceBetweenPoints(blobParasite.CenterOfGravity.X, blobParasite.CenterOfGravity.Y, imageCenterX, imageCenterY);
                    int blobOffsetX = (int)Math.Abs(blobParasite.CenterOfGravity.X - imageCenterX);
                    int blobOffsetY = (int)Math.Abs(blobParasite.CenterOfGravity.Y - imageCenterY);
                    int blobOffsetXPrevious = 0;
                    int blobOffsetYPrevious = 0;

                    #endregion

                    #region "*** Check for Eimeria and Isospora and Filter ***"

                    if (((objectParasiteType == FECALPARASITE_TYPE.Eimeria) || (objectParasiteType == FECALPARASITE_TYPE.Isospora)) && (blobCountInterior < 4))
                    {
                        #region "*** Set Coccidia Interior Values ***"

                        blobsCoccidia.Add(blobParasite);
                        blobsDistanceCenter[blobCountInterior - 1] = blobDistanceCenter;
                        blobsOffsetX[blobCountInterior - 1] = blobOffsetX;  blobsOffsetY[blobCountInterior - 1] = blobOffsetY;
                        blobsEdgePoints[blobCountInterior - 1] = blobCounterParasite.GetBlobsEdgePoints(blobParasite);
                        if (IsTraining == true)
                        {
                            blobsCircleVariance[blobCountInterior - 1] = SharedFunctionController.GetCircleVariance(blobCounterParasite.GetBlobsEdgePoints(blobParasite));  
                        }
                        if ((blobCountInterior == 3) || ((blobCountInterior == 2) && (blobsParasite.Count == 2)))
                        {
                            double distance21 = SharedFunctionController.GetDistanceBetweenPoints(blobsCoccidia[1].CenterOfGravity.X, blobsCoccidia[1].CenterOfGravity.Y, blobsCoccidia[0].CenterOfGravity.X, blobsCoccidia[0].CenterOfGravity.Y);
                            double distance21Minimum = 0; double distance21Maximum = SharedFunctionController.GetDistanceBetweenBlobsMaximum(blobCounterParasite.GetBlobsEdgePoints(blobsCoccidia[1]), blobCounterParasite.GetBlobsEdgePoints(blobsCoccidia[0]), ref distance21Minimum, ref distancesEdge, drawingPoints);  
                            double distance31 = 0; double distance32 = 0; double distance31Minimum = 0; double distance32Minimum = 0; double distance31Maximum = 0; double distance32Maximum = 0; 
                            try
                            {
                                distance31 = SharedFunctionController.GetDistanceBetweenPoints(blobsCoccidia[2].CenterOfGravity.X, blobsCoccidia[2].CenterOfGravity.Y, blobsCoccidia[0].CenterOfGravity.X, blobsCoccidia[0].CenterOfGravity.Y);
                                distance32 = SharedFunctionController.GetDistanceBetweenPoints(blobsCoccidia[2].CenterOfGravity.X, blobsCoccidia[2].CenterOfGravity.Y, blobsCoccidia[1].CenterOfGravity.X, blobsCoccidia[1].CenterOfGravity.Y);
                                distance31Maximum = SharedFunctionController.GetDistanceBetweenBlobsMaximum(blobCounterParasite.GetBlobsEdgePoints(blobsCoccidia[2]), blobCounterParasite.GetBlobsEdgePoints(blobsCoccidia[0]), ref distance31Minimum, ref distancesEdge, drawingPoints);
                                distance32Maximum = SharedFunctionController.GetDistanceBetweenBlobsMaximum(blobCounterParasite.GetBlobsEdgePoints(blobsCoccidia[2]), blobCounterParasite.GetBlobsEdgePoints(blobsCoccidia[1]), ref distance32Minimum, ref distancesEdge, drawingPoints);
                            }
                            catch (Exception ex)
                            {
                                string s = ex.ToString();
                            }
                            //*** Check for Posssible Coccidia ***
                            IsPossibleParasiteCoccidia = IsPossibleParasiteCheckInteriorDetailsCoccidia(IDParasite, blobsParasite.Count, blobsDistanceCenter, blobsOffsetX, blobsOffsetY, blobsEdgePoints, distance21, distance21Minimum, distance21Maximum, distance31, distance31Minimum, distance31Maximum, distance32, distance32Minimum, distance32Maximum, blobsCoccidia, objectParasiteType);
                            if (IsTraining == true) 
                            {  
                                dataTableAnalysisParasiteFecalsShapesCoccidia.Rows.Add(GetDataRow_AnalysisParasiteFecalsShapesCoccidia(IDParasite, blobsParasite.Count, blobsDistanceCenter, blobsOffsetX, blobsOffsetY, blobsCircleVariance, distance21, distance21Minimum, distance21Maximum, distance31, distance31Minimum, distance31Maximum, distance32, distance32Minimum, distance32Maximum, blobsCoccidia));
                                if ((objectParasiteType == FECALPARASITE_TYPE.Isospora) && (IsPossibleParasiteCoccidia == true)) 
                                {
                                    int indexIsospora = 0;
                                    if (grayscaleType == FECALPARASITECONDITION_GRAYSCALE_CONVERSION.Blue) { indexIsospora = 2; }
                                    colorEdge[indexIsospora] = (int)SharedFunctionController.GetDistanceBetweenPoints(Math.Round(((blobsCoccidia[0].CenterOfGravity.X + blobsCoccidia[1].CenterOfGravity.X) / 2), 0), Math.Round(((blobsCoccidia[0].CenterOfGravity.Y + blobsCoccidia[1].CenterOfGravity.Y) / 2), 0), imageCenterX, imageCenterY);
                                    if (blobsCoccidia[0].Rectangle.Y < blobsCoccidia[1].Rectangle.Y)
                                    {
                                        colorEdge[indexIsospora + 1] = (int)SharedFunctionController.GetDistanceBetweenPoints(Math.Round(((double)(blobsCoccidia[0].Rectangle.X + blobsCoccidia[1].Rectangle.X + blobsCoccidia[1].Rectangle.Width) / 2), 0), Math.Round(((double)(blobsCoccidia[0].Rectangle.Y + blobsCoccidia[1].Rectangle.Y + blobsCoccidia[1].Rectangle.Height) / 2), 0), imageCenterX, imageCenterY); 
                                    }
                                    else
                                    {
                                        colorEdge[indexIsospora + 1] = (int)SharedFunctionController.GetDistanceBetweenPoints(Math.Round(((double)(blobsCoccidia[1].Rectangle.X + blobsCoccidia[0].Rectangle.X + blobsCoccidia[0].Rectangle.Width) / 2), 0), Math.Round(((double)(blobsCoccidia[1].Rectangle.Y + blobsCoccidia[0].Rectangle.Y + blobsCoccidia[0].Rectangle.Height) / 2), 0), imageCenterX, imageCenterY); 
                                    }
                                    
                                //    Bitmap objectBitmapBinaryNucleus = (Bitmap)objectBitmapBinaryParasite.Clone();
                                //    colorEdge = SharedFunctionController.GetNucleusFromBitmapBinary(IDParasite, objectBitmapBinaryNucleus, colorEdge);
                                //    objectBitmapBinaryNucleus.Dispose(); objectBitmapBinaryNucleus = null;
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion

                    #region "*** Set Interior Values and Check for Positive Parasite ***"

                    //*** Begin Check Parasite Type and Interior Values ***
                    int blobArea = blobParasite.Area;
                    if (blobParasite.Rectangle.Height > blobParasite.Rectangle.Width) { blobSizeRatio = (double)blobParasite.Rectangle.Height / (double)blobParasite.Rectangle.Width; }
                    try
                    {
                        
                        bool IsUsedCircleVariance = false;
                        if (IsPossibleParasiteCheckInteriorDetails(blobCountInterior, blobDistanceCenter, blobArea, blobParasite.Fullness, blobCounterParasite.GetBlobsEdgePoints(blobParasite), IsUsedCircleVariance) == false) { IsPositiveParasite = false; blobsParasiteBubblesCount++; }
                        
                    }
                    catch (Exception ex)
                    {
                        string s = ex.ToString();
                    }
                    //*** End Check Parasite Type and Interior Values ***
                    if (IsTraining == true)
                    {
                        double blobCircleVariance = SharedFunctionController.GetCircleVariance(blobCounterParasite.GetBlobsEdgePoints(blobParasite));
                        dataTableAnalysisParasiteFecalsShapesInterior.Rows.Add(GetDataRow_AnalysisParasiteFecalsShapesInterior(IDParasite, blobCountInterior, blobsParasite.Count, blobParasite.Area, blobParasite.Fullness, blobParasite.Rectangle, blobParasite.CenterOfGravity.X, blobParasite.CenterOfGravity.Y, blobSizeRatio, blobDistanceCenter, blobDistancePrevious, blobAreaRatioPrevious, blobMinimumDistancePrevious, blobMaximumDistancePrevious, blobCircleVariance, blobOffsetX, blobOffsetY, distancesEdge[0], distancesEdge[1], blobOffsetXPrevious, blobOffsetYPrevious, grayscaleType));
                    }

                    #endregion

                    #region "*** Check for Coccidia and Negative Result ***"

                    if (IsPositiveParasite == false) { break; }

                    #endregion
 
                }

                IsCompletedInterior = true;
            }

            //*** End Loop through Blobs for Binary Image ***

            #endregion

            #region "*** Check Interior Details for Parasite Types ***"

            if ((objectParasiteType == FECALPARASITE_TYPE.Eimeria) && (IsPossibleParasiteCoccidia == false)) { IsPositiveParasite = false; }
            if ((objectParasiteType == FECALPARASITE_TYPE.Isospora) && (IsPossibleParasiteCoccidia == false)) { IsPositiveParasite = false; }
            if ((objectParasiteType == FECALPARASITE_TYPE.Toxocara) && (IsPositiveParasite == true)) { IsPositiveParasite = IsPossibleParasiteCheckInteriorDetailsToxocara(IDParasite, objectBitmapParasiteOriginal, IsTraining); }
          
            #endregion

            #region "*** Destroy Objects and Return Result ***"

            try
            {
                objectStatistics = null;
                //objectBitmapBinaryParasite.Dispose();
                objectBitmapGrayscaleParasite.Dispose();

            }
            catch { }

            return IsPositiveParasite;
           
            #endregion

        }

        private bool IsPossibleParasiteCheckColor(ImageStatistics objectImageStatisticsColor)
        {
            double valueColor = 0;
                   
            foreach (SpeciesCondition_Color speciesConditionColor in objectSpecies.SpeciesConditions_Color)
            {
                if (speciesConditionColor.ColorStatistic == FECALPARASITECONDITION_COLOR_STATISTIC.Mean)
                {
                    if (speciesConditionColor.ColorCalculation == FECALPARASITECONDITION_COLOR_CALCULATION.Ratio)
                    {
                        if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Red) { valueColor = objectImageStatisticsColor.Red.Mean / (objectImageStatisticsColor.Green.Mean + objectImageStatisticsColor.Blue.Mean); }
                        else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Green) { valueColor = objectImageStatisticsColor.Green.Mean / (objectImageStatisticsColor.Red.Mean + objectImageStatisticsColor.Blue.Mean); }
                        else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Blue) { valueColor = objectImageStatisticsColor.Blue.Mean / (objectImageStatisticsColor.Red.Mean + objectImageStatisticsColor.Green.Mean); }
                    }
                    else //if (speciesConditionColor.ColorCalculation == FECALPARASITECONDITION_COLOR_CALCULATION.Total)
                    {
                        valueColor = objectImageStatisticsColor.Red.Mean + objectImageStatisticsColor.Green.Mean + objectImageStatisticsColor.Blue.Mean;
                    }
                }
                else if (speciesConditionColor.ColorStatistic == FECALPARASITECONDITION_COLOR_STATISTIC.Deviation)
                {
                    if (speciesConditionColor.ColorCalculation == FECALPARASITECONDITION_COLOR_CALCULATION.Ratio)
                    {
                        if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Red) { valueColor = objectImageStatisticsColor.Red.StdDev / (objectImageStatisticsColor.Green.StdDev + objectImageStatisticsColor.Blue.StdDev); }
                        else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Green) { valueColor = objectImageStatisticsColor.Green.StdDev / (objectImageStatisticsColor.Red.StdDev + objectImageStatisticsColor.Blue.StdDev); }
                        else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Blue) { valueColor = objectImageStatisticsColor.Blue.StdDev / (objectImageStatisticsColor.Red.StdDev + objectImageStatisticsColor.Green.StdDev); }
                    }
                    else //if (speciesConditionColor.ColorCalculation == FECALPARASITECONDITION_COLOR_CALCULATION.Total)
                    {
                        valueColor = objectImageStatisticsColor.Red.StdDev + objectImageStatisticsColor.Green.StdDev + objectImageStatisticsColor.Blue.StdDev;
                    }
                }
                else if (speciesConditionColor.ColorStatistic == FECALPARASITECONDITION_COLOR_STATISTIC.ColorPercent)
                {
                    if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Red) { valueColor = SharedFunctionController.GetColorFromPercent(objectImageStatisticsColor.Red, speciesConditionColor.ColorValueRange); }
                    else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Green) { valueColor = SharedFunctionController.GetColorFromPercent(objectImageStatisticsColor.Green, speciesConditionColor.ColorValueRange); }
                    else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Blue) { valueColor = SharedFunctionController.GetColorFromPercent(objectImageStatisticsColor.Blue, speciesConditionColor.ColorValueRange); }
                }
                else
                {
                    double valueColorRed = SharedFunctionController.GetColorFromPercent(objectImageStatisticsColor.Red, speciesConditionColor.ColorValueRange);
                    double valueColorGreen = SharedFunctionController.GetColorFromPercent(objectImageStatisticsColor.Green, speciesConditionColor.ColorValueRange);
                    double valueColorBlue = SharedFunctionController.GetColorFromPercent(objectImageStatisticsColor.Blue, speciesConditionColor.ColorValueRange);
                    if (speciesConditionColor.ColorCalculation == FECALPARASITECONDITION_COLOR_CALCULATION.Ratio)
                    {
                        if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Red) { valueColor = valueColorRed / (valueColorGreen + valueColorBlue); }
                        else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Green) { valueColor = valueColorGreen / (valueColorRed + valueColorBlue); }
                        else if (speciesConditionColor.Color == FECALPARASITECONDITION_COLOR.Blue) { valueColor = valueColorBlue / (valueColorRed + valueColorGreen); }
                    }
                    else //if (speciesConditionColor.ColorCalculation == FECALPARASITECONDITION_COLOR_CALCULATION.Total)
                    {
                        valueColor = valueColorRed + valueColorGreen + valueColorBlue;
                    }
                }
                if (SharedFunctionController.IsBetweenValues(valueColor, speciesConditionColor.ColorValueMinimum, speciesConditionColor.ColorValueMaximum) == false) 
                { 
                    return false; 
                }
            }
            
            return true;
        }

        private Blob[] GetBlobsInteriorFromBitmapGrayscale(Bitmap objectBitmapParasiteOriginal, Bitmap objectBitmapGrayscaleParasite, ref Bitmap objectBitmapBinaryParasite, ref AForge.Imaging.BlobCounter blobCounterParasite, int grayMean, int blobsAttemptCount)
        {
            int erosionCount = 0;
            int integerTreshold = 0;

            if (blobsAttemptCount == 0) { integerTreshold = (int)grayMean; }
            else if (blobsAttemptCount == 1) { integerTreshold = 41; erosionCount = 3; }
            else if (blobsAttemptCount == 2) { integerTreshold = 57; erosionCount = 2; }
            else if (blobsAttemptCount == 3) { objectBitmapGrayscaleParasite = new ExtractChannel(RGB.B).Apply(objectBitmapParasiteOriginal);  integerTreshold = 41; erosionCount = 3; }
            else { objectBitmapGrayscaleParasite = new ExtractChannel(RGB.B).Apply(objectBitmapParasiteOriginal); integerTreshold = 57; erosionCount = 2; } 


            objectBitmapBinaryParasite = new Threshold(integerTreshold).Apply(objectBitmapGrayscaleParasite);
            new BlobsFiltering((int)(objectBitmapBinaryParasite.Width / 25), (int)(objectBitmapBinaryParasite.Height / 25), 1000000, 100000, false).ApplyInPlace(objectBitmapBinaryParasite);
            for (int counter = 0; counter < erosionCount; counter++)
            {
                new Erosion().ApplyInPlace(objectBitmapBinaryParasite);
            }
            blobCounterParasite = new AForge.Imaging.BlobCounter(objectBitmapBinaryParasite);
            blobCounterParasite.ObjectsOrder = ObjectsOrder.Area;
            blobCounterParasite.ProcessImage(objectBitmapBinaryParasite);
            Blob[] blobsParasite = blobCounterParasite.GetObjectsInformation();
            return blobsParasite;
        }

        private bool IsPossibleParasiteCheckInteriorDetails(int blobIndex, double blobDistanceCenter, int blobArea, double blobFullness, List<IntPoint> edgePointsParasite, bool IsUsedCircleVariance)
        {
            try
            {
                foreach (SpeciesInterior interior in objectSpecies.SpeciesInterior)
                {
                    if ((SharedFunctionController.IsBetweenValues(blobIndex,interior.ShapeBlobInteriorIndexMinimum,interior.ShapeBlobInteriorIndexMaximum) == true) && 
                        (SharedFunctionController.IsBetweenValues(blobDistanceCenter, interior.ShapeBlobInteriorDistanceMinimum, interior.ShapeBlobInteriorDistanceMaximum) == true) && 
                        (SharedFunctionController.IsBetweenValues(blobArea, interior.ShapeBlobInteriorAreaMinimum, interior.ShapeBlobInteriorAreaMaximum) == true) &&
                        (SharedFunctionController.IsBetweenValues(blobFullness, interior.ShapeBlobInteriorFullnessMinimum, interior.ShapeBlobInteriorFullnessMaximum) == true))
                    {
                        if ((interior.IsUsedCircleVariance == false) || (SharedFunctionController.IsMatchCheckCircleVariance(edgePointsParasite, interior.ShapeBlobInteriorCircleVarianceMinimum, interior.ShapeBlobInteriorCircleVarianceMaximum) == true))
                        {
                            return false;
                        }
                    }
                }
            }
            catch { } //Species Interior not Defined
            return true;
        }

        private bool IsPossibleParasiteCheckInteriorDetailsCoccidia(string IDParasite, int blobCount, double[] blobsDistanceCenter, int[] blobsOffsetX, int[] blobsOffsetY, List<IntPoint>[] blobsEdgePoints, double distance21, double distance21Minimum, double distance21Maximum, double distance31, double distance31Minimum, double distance31Maximum, double distance32, double distance32Minimum, double distance32Maximum, List<Blob> blobsCoccidia, FECALPARASITE_TYPE objectParasiteType)
        {
            int blobIndexPositive = 0;

            try
            {
                #region "*** Begin Eimeria Check ***"

                if (objectParasiteType == FECALPARASITE_TYPE.Eimeria)
                {
                    if (SharedFunctionController.IsBetweenValues(blobsCoccidia[0].Area, 120, 1260))
                    {
                        blobIndexPositive = 1;
                    }

                    if (blobIndexPositive == 1)
                    {
                        if (SharedFunctionController.IsBetweenValues(blobsCoccidia[1].Area, 55, 1150))
                        {
                           return true;
                        }
                    }
                }

                #endregion "*** End Eimeria Check ***"

                #region "*** Begin Isospora Check ***"

                else if (objectParasiteType == FECALPARASITE_TYPE.Isospora)
                {

                    //blobarea1 between 160 and 1375 
                    //and blobarea2 between 150 and 900 
                    //and (BlobXCenter1 + blobxcenter2) / 2 between 27 and 46 
                    //and (BlobyCenter1 + blobycenter2) / 2 between 44 and 57 
                    //and BlobOffsetX21 < 18 
                    //and bloboffsety21 < 18
                    //and BlobAreaRatio21 > .045  
                    //and BlobMaximumDistance21 between 65 and 150 
                    //and BlobDistance21 between 35 and 145 
                    //and blobfullness1 between .1 and .75
                    //and blobfullness2 between .1 and .7

                    //*** Apply Blob1 Criteria ***
                    //blobarea1 between 185 and 1375 and blobfullness2 between .1 and .9
                    if (SharedFunctionController.IsBetweenValues(blobsCoccidia[0].Area, 185, 1375) && (SharedFunctionController.IsBetweenValues(blobsCoccidia[0].Fullness, .1, .9)))
                    {
                        blobIndexPositive = 1;
                    }
                    ////*** Apply Blob2 Criteria
                    if (blobIndexPositive == 1)
                    {
                        //and blobarea2 between 114 and 900 and blobfullness2 between .1 and .85
                        if (SharedFunctionController.IsBetweenValues(blobsCoccidia[1].Area, 114, 900) && (SharedFunctionController.IsBetweenValues(blobsCoccidia[1].Fullness, .1, .85)))
                        {
                            int centerPointX = (int)((blobsCoccidia[1].CenterOfGravity.X + blobsCoccidia[0].CenterOfGravity.X) / 2);
                            int centerPointY = (int)((blobsCoccidia[1].CenterOfGravity.Y + blobsCoccidia[0].CenterOfGravity.Y) / 2);
                            //and BlobAreaRatio21 > .20 and BlobOffsetX21 < 27 and Bloboffsety21 < 39 
                            //and BlobDistance21 between 26 and 145 and BlobMaximumDistance21 between 42 and 150 
                            //and (BlobXCenter1 + blobxcenter2) / 2 between 15 and 50 and (BlobyCenter1 + blobycenter2) / 2 between 39 and 72
                            if (((double)blobsCoccidia[1].Area / (double)blobsCoccidia[0].Area > .2) && (SharedFunctionController.IsBetweenValues(distance21Maximum, 42, 145)) && (SharedFunctionController.IsBetweenValues(distance21, 26, 145))
                                && (SharedFunctionController.IsBetweenValues(centerPointX, 15, 50)) && (SharedFunctionController.IsBetweenValues(centerPointY, 39, 72)))
                            {
                                return true;
                            }
                        }
                     }
                   
                }

                #endregion "*** End Isospora Check ***"
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }

            return false;

        }

        private bool IsPossibleParasiteCheckInteriorDetailsToxocara(string IDParasite, Bitmap objectBitmapParasiteOriginal, bool IsTraining)
        {
            const int INTEGER_THRESHOLD_TOXOCARA = 56;

            Bitmap objectBitmapGrayscaleParasite = new ExtractChannel(RGB.B).Apply(objectBitmapParasiteOriginal);
            ImageStatistics objectStatistics = new ImageStatistics(objectBitmapGrayscaleParasite);
            SimplePosterization objectPosterization = new SimplePosterization();
            objectPosterization.PosterizationInterval = 16;
            objectPosterization.ApplyInPlace(objectBitmapGrayscaleParasite);
            Bitmap objectBitmapBinaryParasite = new Threshold(INTEGER_THRESHOLD_TOXOCARA).Apply(objectBitmapGrayscaleParasite);
            new BlobsFiltering((int)(objectBitmapBinaryParasite.Width / 10), (int)(objectBitmapBinaryParasite.Height / 10), 1000000, 100000, false).ApplyInPlace(objectBitmapBinaryParasite);
            //*** Set Image Size Variables ***
            int imageBinaryWidth = objectBitmapBinaryParasite.Width; int imageBinaryHeight = objectBitmapBinaryParasite.Height; int imageCenterX = (int)(objectBitmapBinaryParasite.Width / 2); int imageCenterY = (int)(objectBitmapBinaryParasite.Height / 2);


            AForge.Imaging.BlobCounter blobCounterParasite = new AForge.Imaging.BlobCounter(objectBitmapBinaryParasite);
            blobCounterParasite.ObjectsOrder = ObjectsOrder.Area;
            blobCounterParasite.ProcessImage(objectBitmapBinaryParasite);
            Blob[] blobsParasiteArray = blobCounterParasite.GetObjectsInformation();

            for (int index = 0; index < blobsParasiteArray.Length; index++)
            {
                Blob blobParasiteCurrent = blobsParasiteArray[index];
                if (SharedFunctionController.IsRectangleTouchingEdge(blobParasiteCurrent.Rectangle, imageBinaryWidth, imageBinaryHeight) == false)
                {
                    double blobDistanceCenter = SharedFunctionController.GetDistanceBetweenPoints(blobParasiteCurrent.CenterOfGravity.X, blobParasiteCurrent.CenterOfGravity.Y, imageCenterX, imageCenterY);
                    double blobCircleVariance = SharedFunctionController.GetCircleVariance(blobCounterParasite.GetBlobsEdgePoints(blobParasiteCurrent));
                    if ((blobParasiteCurrent.Area > 2900) && (blobDistanceCenter < 72) && (SharedFunctionController.IsMatchCheckCircleVariance(blobCounterParasite.GetBlobsEdgePoints(blobParasiteCurrent),0,.105)))
                    {
                        if (IsTraining == true)
                        {
                            dataTableAnalysisParasiteFecalsShapesInterior.Rows.Add(GetDataRow_AnalysisParasiteFecalsShapesInterior(IDParasite, -1, -1, blobParasiteCurrent.Area, blobParasiteCurrent.Fullness, blobParasiteCurrent.Rectangle, blobParasiteCurrent.CenterOfGravity.X, blobParasiteCurrent.CenterOfGravity.Y, 0, blobDistanceCenter, 0, 0, 0, 0, blobCircleVariance, 0, 0, 0, 0, 0, 0, FECALPARASITECONDITION_GRAYSCALE_CONVERSION.Grayscale));
                        }
                        return false;
                    }
                }
            }

            return true;

        }

        private bool IsPossibleParasiteCheckShapePatterns(string IDParasite, IntPoint[] perimeterPoints, int cellType, string stringTableSuffix, string CaseID, bool IsTraining)
        {

            #region "*** Declare Variables ***

            //WriteToLog("Shape Patterns for " + IDParasite + " Started");
     
            int indexEnd = perimeterPoints.Length - 1;
            double pixelDistance = 0;
            double pixelDistancePrevious = 0;
            double pixelDistanceDifferencePrevious = 0;
            double pixelDistanceDifferenceOpposite = 0;
            double pixelDistanceAverage = 0;
            double pixelDistanceDifference = 0;
            double pixelDistanceOpposite = 0;
            double pixelDistanceRatio = 0;
            double pixelDistanceRatioTotal = 0;
            int indexMedian = 0;
            int indexReference = 0;
            int integerQuadrant = 0;
            int indexOpposite = 0;
            IntPoint[] referencePoints;
            if (((FECALPARASITE_TYPE)cellType != FECALPARASITE_TYPE.Giardia)) { referencePoints = GetReferencePoints(perimeterPoints, ref indexMedian); }
            else { referencePoints = GetReferencePointsGiardia(perimeterPoints, ref indexMedian); }
            int indexQuadrant = (int)(indexMedian / 2);
            int[] anomalyCounts = new int[objectSpecies.SpeciesConditions.Length];

            #endregion

            #region "*** Loop Through Points ***"
           
            for (int index = 0; index <= indexEnd; index++)
            {
                try
                {
                    indexOpposite = indexEnd - index;
                    if (index <= indexMedian) { indexReference = index; }
                    else { indexReference = indexEnd - index; }
                    //*** Begin Temp Code ***
                    if (objectSpecies.SpeciesPoints[index] == null) { objectSpecies.SpeciesPoints[index] = new SpeciesPoint(index, 0); }
                    //*** End Temp Code ***
                    //*** Set Pixel Distance from Center Point ***
                    pixelDistance = SharedFunctionController.GetDistanceBetweenPoints(perimeterPoints[index].X, perimeterPoints[index].Y, referencePoints[indexReference].X, referencePoints[indexReference].Y);
                    pixelDistanceAverage = objectSpecies.SpeciesPoints[index].DistanceAverage;
                    pixelDistanceDifference = Math.Abs(pixelDistance - pixelDistanceAverage);
                    pixelDistanceDifferencePrevious = Math.Abs(pixelDistanceDifference - pixelDistancePrevious);
                    pixelDistanceOpposite = SharedFunctionController.GetDistanceBetweenPoints(perimeterPoints[indexOpposite].X, perimeterPoints[indexOpposite].Y, referencePoints[indexReference].X, referencePoints[indexReference].Y);
                    pixelDistanceDifferenceOpposite = Math.Abs(pixelDistanceOpposite - pixelDistanceAverage);
                    pixelDistanceDifferenceOpposite = Math.Abs(Math.Round(pixelDistanceDifference - pixelDistanceDifferenceOpposite, 4));

                    //*** Check Pixel Distance for Anomaly ***
                    if (pixelDistance > 0) { pixelDistanceRatio = (pixelDistanceDifference / pixelDistance); }
                    else { pixelDistanceRatio = 0; }
                    pixelDistanceRatioTotal = pixelDistanceRatioTotal + pixelDistanceRatio;
                    integerQuadrant = Convert.ToInt16(index / indexQuadrant);
                    //*** Get Anomaly Counts for Condition Type and Values ***
                    GetAnomalyCounts(ref anomalyCounts, pixelDistanceDifference, pixelDistanceDifferenceOpposite, pixelDistanceDifferencePrevious, pixelDistanceRatio, integerQuadrant);
                    ////*** Check for Training and Update Database ***
                    if (IsTraining == true)
                    {
                        if (index == 0) { pixelDistancePrevious = pixelDistance; }
                        dataTableAnalysisParasiteFecalsShapesAnalysis.Rows.Add(GetDataRow_AnalysisParasiteFecalsShapesAnalysis(IDParasite, CaseID, cellType, index, indexOpposite, pixelDistanceAverage, pixelDistance, pixelDistanceDifference, pixelDistanceRatio, pixelDistanceOpposite, pixelDistanceOpposite, pixelDistanceDifferenceOpposite, 0, pixelDistanceDifferencePrevious, integerQuadrant, perimeterPoints[index].X, perimeterPoints[index].Y, 0));
                        //GetDatabasePoint(stringTableSuffix, IDParasite, CaseID, cellType, index, indexOpposite, pixelDistanceAverage, pixelDistance, pixelDistanceDifference, pixelDistanceDifference / pixelDistance, pixelDistanceOpposite, integerQuadrant, referencePoints[indexReference].X, perimeterPoints[index].X, perimeterPoints[index].Y, pixelDistanceDifferencePrevious, pixelDistanceDifferenceOpposite);
                        //string stringSQL = "INSERT INTO _AnalysisParasiteFecalsShapesAnalysis" + stringTableSuffix + " (IDParasite,CaseID,ResultValue,PointIndex,PointIndexOpposite,DistanceAverage,Distance,DistanceDifference,DistanceRatio,DistanceOpposite,Quadrant,PointX,PointY,DistanceDifferencePrevious,DistanceDifferenceOpposite) VALUES ('" +
                        //IDParasite + "'" + st(stringCaseID) + st(cellType) + st(indexPoint) + st(indexPointOpposite) + st(pixelDistanceAverage) + st(pixelDistance) + st(pixelDistanceDifference) + st(pixelDistanceRatio) + st(pixelDistanceOpposite) + st(quadrant) + st(pointX) + st(pointY) + st(pixelDistancePrevious) + st(pixelDistanceDifferenceOpposite) + ")";
                        //stringSQL = stringSQL.Replace("Infinity", "0");
                    }
                    pixelDistancePrevious = pixelDistanceDifference;
                }
                catch (Exception ex)
                {
                    WriteToLog(" Case: " + CaseID + ", Function IsPossibleParasiteCheckShapePatterns, " + ex.ToString());
                }
            }
   
            //WriteToLog("Shape Patterns Anomalies for " + IDParasite + " Started");
           
            for (int index = 0; index < anomalyCounts.Length; index++)
            {
                if (anomalyCounts[index] > objectSpecies.SpeciesConditions[index].PointCount) { return false; }
            }

            //WriteToLog("Shape Patterns for " + IDParasite + " Ended");
           
            return true;
            
            #endregion

        }

        private void GetAnomalyCounts(ref int[] anomalyCounts, double pixelDistanceDifference, double pixelDistanceDifferenceOpposite, double pixelDistanceDifferencePrevious, double pixelDistanceRatio, int Quadrant)
        {

            int index = 0;
            foreach (SpeciesCondition speciesCondition in objectSpecies.SpeciesConditions)
            {
                if (speciesCondition.CONDITIONTYPE == FECALPARASITECONDITION_TYPE.DistanceDifferenceOpposite)
                {
                    if ((pixelDistanceDifferenceOpposite > speciesCondition.MinimumDistance) && (IsMatchQuadrant(speciesCondition, Quadrant))) { anomalyCounts[index]++; }
                }
                else if (speciesCondition.CONDITIONTYPE == FECALPARASITECONDITION_TYPE.DistanceDifferencePrevious)
                {
                    if ((pixelDistanceDifferencePrevious > speciesCondition.MinimumDistance) && (IsMatchQuadrant(speciesCondition, Quadrant))) { anomalyCounts[index]++; }
                }
                else if (speciesCondition.CONDITIONTYPE == FECALPARASITECONDITION_TYPE.DistanceDifferenceRatio)
                {
                    if ((pixelDistanceRatio > speciesCondition.MinimumDistance) && (IsMatchQuadrant(speciesCondition, Quadrant))) { anomalyCounts[index]++; }
                }
                else
                {
                    if ((pixelDistanceDifference > speciesCondition.MinimumDistance) && (IsMatchQuadrant(speciesCondition, Quadrant)))
                    {
                        anomalyCounts[index]++;
                        //if (index == 0) { IsAnomaly1 = true; }
                    }
                }
                index++;
            }
        }

        private bool IsMatchQuadrant(SpeciesCondition speciesCondition, int quadrant)
        {
            if (speciesCondition.Quadrant < 0) { return true; }
            if (speciesCondition.Quadrant == quadrant) { return true; }
            else { return false; }
        }

        #endregion

        #region "*** Private Boolean GiardiaFilter Methods ***"

        private ImageStatistics GetColorImageStatisticsFromBitmapGiardia(Bitmap colorBitmap, int colorHeight, IntPoint pointLeftEdge, IntPoint pointRightEdge)
        {
            //Rectangle objectRectangleGiardiaColor = new Rectangle(pointLeftEdge.X, pointLeftEdge.Y - (colorHeight / 2), pointRightEdge.X - pointLeftEdge.X, colorHeight);
            Rectangle objectRectangleGiardiaColor = new Rectangle(0, 0, colorBitmap.Width, colorBitmap.Height);
            Bitmap colorBitmapCenter = new Crop(objectRectangleGiardiaColor).Apply(colorBitmap);
            ImageStatistics objectStatisticsParasite = new ImageStatistics(colorBitmapCenter);
            return objectStatisticsParasite;
        }

        private bool IsReversedGiardia(IntPoint[] perimeterPoints)
        {
            int indexMedian = (int)(perimeterPoints.Length / 2);
            int pointsTotal = perimeterPoints.Length - 1;
            IntPoint pointFrom = new IntPoint((int)System.Math.Round(((double)(perimeterPoints[0].X + perimeterPoints[pointsTotal].X) / 2), 0), perimeterPoints[0].Y);
            IntPoint pointTo = new IntPoint((int)System.Math.Round(((double)(perimeterPoints[indexMedian].X + perimeterPoints[indexMedian + 1].X) / 2), 0), perimeterPoints[indexMedian].Y);
            double distanceLeft = 0; double distanceRight = 0;
            for (int index = 0; index <= indexMedian; index++)
            {
                int indexOpposite = pointsTotal - index;
                IntPoint pointLine = SharedFunctionController.GetLinePoint(pointFrom, pointTo, index);
                int integerMultiplier = 0;
                if (SharedFunctionController.IsBetweenValues(index, (int)indexMedian * .3333, (int)indexMedian * .6666) == true) { integerMultiplier = 2; }
                else { integerMultiplier = 1; }
                distanceRight = distanceRight + ((perimeterPoints[index].X - pointLine.X) * integerMultiplier);
                distanceLeft = distanceLeft + ((pointLine.X - perimeterPoints[indexOpposite].X) * integerMultiplier);
            }
            if (distanceRight >= distanceLeft) { return false; }
            return true;
        }

        private bool IsReversedGiardia(Bitmap objectBitmapBinaryParasiteGiardia)
        {
            int rectangleWidth = (int)objectBitmapBinaryParasiteGiardia.Width / 2;
            int bitmapHeight = objectBitmapBinaryParasiteGiardia.Height;
            Rectangle objectRectangleGiardia = new Rectangle(0, 0, rectangleWidth, bitmapHeight);
            Bitmap objectBitmapGiardiaHalf = new Crop(objectRectangleGiardia).Apply(objectBitmapBinaryParasiteGiardia);
            ImageStatistics objectImageStatisticsGiardia = new ImageStatistics(objectBitmapGiardiaHalf);
            int pixelsLeft = objectImageStatisticsGiardia.PixelsCountWithoutBlack;
            objectRectangleGiardia = new Rectangle(rectangleWidth + 1, 0, rectangleWidth + 1, bitmapHeight);
            objectBitmapGiardiaHalf = new Crop(objectRectangleGiardia).Apply(objectBitmapBinaryParasiteGiardia);
            objectImageStatisticsGiardia = new ImageStatistics(objectBitmapGiardiaHalf);
            int pixelsRight = objectImageStatisticsGiardia.PixelsCountWithoutBlack;
            if (pixelsLeft > pixelsRight) { return true; }
            return false;
        }

        private ImageInfo GetImageInfoFromSpeciesResult(SpeciesResult giardiaResult)
        {
            ImageStatistics objectImageStatistics = giardiaResult.objectStatisticsParasite;
            ImageInfo imageInfoReturn = new ImageInfo();
            imageInfoReturn.AxisMajor = giardiaResult.axisMajor;
            imageInfoReturn.BlobCount = giardiaResult.blobCountInterior;
            imageInfoReturn.BlobFullness = giardiaResult.blobFullness;
            imageInfoReturn.BlobLeft = giardiaResult.objectRectangleBlob.Left;
            imageInfoReturn.BlobTop = giardiaResult.objectRectangleBlob.Top;
            imageInfoReturn.CircleVariance = giardiaResult.circleVariance;
            imageInfoReturn.FocusContrast = giardiaResult.focusContrast;
            imageInfoReturn.Fullness = giardiaResult.fullness;
            imageInfoReturn.image = giardiaResult.BitmapResult;
            imageInfoReturn.ShapeThreshold = giardiaResult.edgeWidth[0];
            imageInfoReturn.MeanRed = objectImageStatistics.Red.Mean;
            imageInfoReturn.MeanGreen = objectImageStatistics.Green.Mean;
            imageInfoReturn.MeanBlue = objectImageStatistics.Blue.Mean;
            imageInfoReturn.DeviationRed = objectImageStatistics.Red.StdDev;
            imageInfoReturn.DeviationGreen = objectImageStatistics.Green.StdDev;
            imageInfoReturn.DeviationBlue = objectImageStatistics.Blue.StdDev;
            imageInfoReturn.Red11 = objectImageStatistics.Red.GetRange(.78).Min;
            imageInfoReturn.Green11 = objectImageStatistics.Green.GetRange(.78).Min;
            imageInfoReturn.Blue11 = objectImageStatistics.Blue.GetRange(.78).Min;
            return imageInfoReturn;           
        }

        private string[] IsCasePositiveGiardia(string CaseID, string RequestID, string folderPathProcessed, string folderPathGiardia, double PredictionThreshold, SqlDataReader sqlReader)
        {
            string[] returnString = new string[1];
            const int IMAGE_COUNT = 14;
            ParasiteDetector parasiteDetector = new ParasiteDetector(AppDomain.CurrentDomain.BaseDirectory + "\\GiardiaDetection\\TrainedModels");
            List<ImageInfo> imageInfoList = new List<ImageInfo>();
            while (sqlReader.Read())
            {
                ImageInfo imageInfoReturn = new ImageInfo();
                imageInfoReturn.name = Convert.ToString(sqlReader["IDParasite"] + ".JPG");;
                imageInfoReturn.image = new Bitmap(folderPathGiardia + imageInfoReturn.name);
                imageInfoReturn.AxisMajor = Convert.ToDouble(sqlReader["AxisMajor"]);
                imageInfoReturn.BlobCount = Convert.ToInt16(sqlReader["EdgeWidth2"]);
                imageInfoReturn.BlobFullness = Convert.ToDouble(sqlReader["BlobFullness"]);
                imageInfoReturn.BlobLeft = Convert.ToInt16(sqlReader["BlobLeft"]);
                imageInfoReturn.BlobTop = Convert.ToInt16(sqlReader["BlobTop"]);
                imageInfoReturn.CircleVariance = Convert.ToDouble(sqlReader["CircleVariance"]);
                imageInfoReturn.FocusContrast = Convert.ToDouble(sqlReader["FocusContrast"]);
                imageInfoReturn.Fullness = Convert.ToDouble(sqlReader["Fullness"]);
                imageInfoReturn.ShapeThreshold = Convert.ToDouble(sqlReader["EdgeWidth1"]);
                imageInfoReturn.MeanRed = Convert.ToDouble(sqlReader["MeanRed"]);
                imageInfoReturn.MeanGreen = Convert.ToDouble(sqlReader["MeanGreen"]);
                imageInfoReturn.MeanBlue = Convert.ToDouble(sqlReader["MeanBlue"]);
                imageInfoReturn.DeviationRed = Convert.ToDouble(sqlReader["DeviationRed"]);
                imageInfoReturn.DeviationGreen = Convert.ToDouble(sqlReader["DeviationGreen"]);
                imageInfoReturn.DeviationBlue = Convert.ToDouble(sqlReader["DeviationBlue"]);
                imageInfoReturn.Red11 = Convert.ToDouble(sqlReader["Red11"]);
                imageInfoReturn.Green11 = Convert.ToDouble(sqlReader["Green11"]);
                imageInfoReturn.Blue11 = Convert.ToDouble(sqlReader["Blue11"]);
                imageInfoList.Add(imageInfoReturn);
            }
            sqlReader.Close();
            double casePrediction = parasiteDetector.GetCaseProbabilityPrediction(imageInfoList);
            
            if (casePrediction >= PredictionThreshold)
            {

                imageInfoList.Sort(delegate(ImageInfo r1, ImageInfo r2)
                {
                    return r2.FocusContrast.CompareTo(r1.FocusContrast);
                });

                for (int giardiaCount = 0; giardiaCount < IMAGE_COUNT; giardiaCount++)
                {
                    ImageInfo imageInfo = imageInfoList[giardiaCount];
                    InsertDatabaseResult(CaseID, RequestID, (int)FECALPARASITE_TYPE.Giardia, imageInfo.name, imageInfo.FocusContrast, imageInfo.ShapeThreshold, 1);
                    Bitmap objectBitmapGiardia = GetGiardiaFromBitmap(imageInfo.image);
                    objectBitmapGiardia.Save(folderPathProcessed + imageInfo.name);
                }
            }
            returnString[0] = "Update _AnalysisParasiteFecals Set ConfidenceScore = " + Convert.ToString(casePrediction) + " Where CaseID = '" + CaseID + "' and RequestID = " + RequestID;

            return returnString;
           
        }

        #endregion

        #region "*** Private Shape Helper Methods ***"

        private IntPoint[] GetReferencePoints(IntPoint[] perimeterPoints, ref int indexMedian)
        {
            int indexEnd = perimeterPoints.Length - 1;
            indexMedian = (int)(indexEnd / 2);
            int indexMiddle = ((int)indexMedian + 1) / 2;
            IntPoint[] returnPoints = new IntPoint[indexMedian + 1];

            try
            {
                IntPoint pointTop = new IntPoint((int)((perimeterPoints[0].X + perimeterPoints[indexEnd].X) / 2), 0);
                int pointXChange = 2 * ((int)((perimeterPoints[indexEnd - indexMiddle].X + perimeterPoints[indexMiddle].X + 1) / 2) - pointTop.X);
                IntPoint pointBottom = new IntPoint(pointTop.X + pointXChange, indexMedian);
                double pixelDistance = SharedFunctionController.GetDistanceBetweenPoints(pointTop.X, pointTop.Y, pointBottom.X, pointBottom.Y);
                pixelDistance = pixelDistance / indexMedian;
                for (int index = 0; index <= indexMedian; index++)
                {
                    returnPoints[index] = SharedFunctionController.GetLinePoint(pointTop, pointBottom, (index * pixelDistance));
                }
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }
            return returnPoints;
        }

        private IntPoint[] GetReferencePointsGiardia(IntPoint[] perimeterPoints, ref int indexMedian)
        {
            const int POINT_SECTIONS = 3;
            int indexEnd = perimeterPoints.Length - 1;
            indexMedian = (int)(indexEnd / 2);
            int indexSection = ((int)indexMedian + 1) / POINT_SECTIONS;
            IntPoint[] returnPoints = new IntPoint[indexMedian + 1];
            IntPoint[] pointsTop = new IntPoint[POINT_SECTIONS];
            IntPoint[] pointsBottom = new IntPoint[POINT_SECTIONS];
            double[] pointsDistance = new double[POINT_SECTIONS];
            int indexTop = 0;

            try
            {

                for (int index = 0; index < POINT_SECTIONS; index++)
                {
                    indexTop = index * indexSection;
                    pointsTop[index] = new IntPoint((int)((perimeterPoints[indexEnd - indexTop].X + perimeterPoints[indexTop].X) / 2), indexTop);
                    pointsBottom[index] = new IntPoint((int)((perimeterPoints[indexEnd - (indexTop + indexSection - 1)].X + perimeterPoints[indexTop + indexSection - 1].X) / 2), pointsTop[index].Y + indexSection - 1);
                    pointsDistance[index] = SharedFunctionController.GetDistanceBetweenPoints(pointsTop[index].X, pointsTop[index].Y, pointsBottom[index].X, pointsBottom[index].Y) / (indexSection - 1);
                }


                for (int index = 0; index <= indexMedian; index++)
                {
                    int currentSection = Convert.ToInt32(index / indexSection);
                    int indexDistance = (index - (currentSection * indexSection));

                    returnPoints[index] = SharedFunctionController.GetLinePoint(pointsTop[currentSection], pointsBottom[currentSection], indexDistance * pointsDistance[currentSection]);
                }

            }
            catch (Exception ex)
            {
                string s = ex.ToString();
            }

            return returnPoints;
        }

        private int GetWidthFromPerimeterPoints(IntPoint[] perimeterEdgePoints, int pixelHeight)
        {
            int totalPoints = (pixelHeight * 2) - 1;
            int pixelWidthHighest = 0;
            for (int index = 0; index < pixelHeight; index++)
            {
                int pixelWidth = Math.Abs(perimeterEdgePoints[index].X - perimeterEdgePoints[totalPoints - index].X);
                if (pixelWidth > pixelWidthHighest)
                {
                    pixelWidthHighest = pixelWidth;
                }
            }
            return pixelWidthHighest;
        }
        
        #endregion

        #region "*** Private IO Helper Methods ***"

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private void WriteToLog(string message)
        {
            try
            {
                message = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + ": " + message;
                TextWriter textWriterProcessed = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + SharedValueController.STRING_LOGFILENAME, true);
                textWriterProcessed.WriteLine(message);
                textWriterProcessed.Close();
                textWriterProcessed.Dispose();
            }
            catch { }
        }

        #endregion

        #region "*** Private Database Helper Methods ***"

        private bool GetDatabaseIsParasitePositive(string IDParasite)
        {
            string stringSQL = "Select Count(*) From " + SharedValueController.STRING_TABLE_PARASITEPOSITIVE + " Where IDParasite = '" + IDParasite + "'";
            if (Convert.ToInt32(dataController.GetDataScalarFromQuery(stringSQL)) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool GetDatabaseIsParasiteNegative(string IDParasite)
        {
            string stringSQL = "Select Count(*) From " + SharedValueController.STRING_TABLE_PARASITENEGATIVE + " Where IDParasite = '" + IDParasite + "'";
            if (Convert.ToInt32(dataController.GetDataScalarFromQuery(stringSQL)) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private DataTable GetDataTable_AnalysisParasiteFecals(string stringTableSuffix)
        {
            #region "*** Create Data Table for _AnalysisParasiteFecals ***"

            //Select '            returnTable.Columns.Add("' + name + '",' + CASE WHEN system_type_id = 167 THEN 'typeof(string)' 
            //WHEN system_type_id = 62 THEN 'typeof(double)' WHEN system_type_id in (52,56) THEN 'typeof(Int32)' END + ');' from sys.columns where object_id in 
            //(select object_id from sys.tables where name = '_AnalysisParasiteFecals') and column_id > 1 order by column_id
            DataTable returnTable = new DataTable(SharedValueController.STRING_TABLE_PARASITEMAIN + stringTableSuffix);

            returnTable.Columns.Add("ID", typeof(Int32));
            returnTable.Columns.Add("CaseID", typeof(string));
            returnTable.Columns.Add("RequestID", typeof(Int32));
            returnTable.Columns.Add("Region", typeof(Int32));
            returnTable.Columns.Add("ResultValue", typeof(Int32));
            returnTable.Columns.Add("IDParasite", typeof(string));
            returnTable.Columns.Add("Species", typeof(Int32));
            returnTable.Columns.Add("BlobTop", typeof(Int32));
            returnTable.Columns.Add("BlobLeft", typeof(Int32));
            returnTable.Columns.Add("BlobWidth", typeof(Int32));
            returnTable.Columns.Add("BlobHeight", typeof(Int32));
            returnTable.Columns.Add("BlobFullness", typeof(double));
            returnTable.Columns.Add("Fullness", typeof(double));
            returnTable.Columns.Add("FocusContrast", typeof(double));
            returnTable.Columns.Add("AxisMajor", typeof(double));
            returnTable.Columns.Add("AxisMinor", typeof(double));
            returnTable.Columns.Add("AxisRatio", typeof(double));
            returnTable.Columns.Add("CircleVariance", typeof(double));
            returnTable.Columns.Add("MeanRed", typeof(double));
            returnTable.Columns.Add("MeanGreen", typeof(double));
            returnTable.Columns.Add("MeanBlue", typeof(double));
            returnTable.Columns.Add("DeviationRed", typeof(double));
            returnTable.Columns.Add("DeviationGreen", typeof(double));
            returnTable.Columns.Add("DeviationBlue", typeof(double));
            returnTable.Columns.Add("Red02", typeof(double));
            returnTable.Columns.Add("Green02", typeof(double));
            returnTable.Columns.Add("Blue02", typeof(double));
            returnTable.Columns.Add("Red98", typeof(double));
            returnTable.Columns.Add("Green98", typeof(double));
            returnTable.Columns.Add("Blue98", typeof(double));
            returnTable.Columns.Add("Red04", typeof(double));
            returnTable.Columns.Add("Green04", typeof(double));
            returnTable.Columns.Add("Blue04", typeof(double));
            returnTable.Columns.Add("Red96", typeof(double));
            returnTable.Columns.Add("Green96", typeof(double));
            returnTable.Columns.Add("Blue96", typeof(double));
            returnTable.Columns.Add("Red11", typeof(double));
            returnTable.Columns.Add("Green11", typeof(double));
            returnTable.Columns.Add("Blue11", typeof(double));
            returnTable.Columns.Add("Red89", typeof(double));
            returnTable.Columns.Add("Green89", typeof(double));
            returnTable.Columns.Add("Blue89", typeof(double));
            returnTable.Columns.Add("Red18", typeof(double));
            returnTable.Columns.Add("Green18", typeof(double));
            returnTable.Columns.Add("Blue18", typeof(double));
            returnTable.Columns.Add("Red82", typeof(double));
            returnTable.Columns.Add("Green82", typeof(double));
            returnTable.Columns.Add("Blue82", typeof(double));
            returnTable.Columns.Add("Red25", typeof(double));
            returnTable.Columns.Add("Green25", typeof(double));
            returnTable.Columns.Add("Blue25", typeof(double));
            returnTable.Columns.Add("Red75", typeof(double));
            returnTable.Columns.Add("Green75", typeof(double));
            returnTable.Columns.Add("Blue75", typeof(double));
            returnTable.Columns.Add("Red32", typeof(double));
            returnTable.Columns.Add("Green32", typeof(double));
            returnTable.Columns.Add("Blue32", typeof(double));
            returnTable.Columns.Add("Red68", typeof(double));
            returnTable.Columns.Add("Green68", typeof(double));
            returnTable.Columns.Add("Blue68", typeof(double));
            returnTable.Columns.Add("Red39", typeof(double));
            returnTable.Columns.Add("Green39", typeof(double));
            returnTable.Columns.Add("Blue39", typeof(double));
            returnTable.Columns.Add("Red61", typeof(double));
            returnTable.Columns.Add("Green61", typeof(double));
            returnTable.Columns.Add("Blue61", typeof(double));
            returnTable.Columns.Add("Red46", typeof(double));
            returnTable.Columns.Add("Green46", typeof(double));
            returnTable.Columns.Add("Blue46", typeof(double));
            returnTable.Columns.Add("Red54", typeof(double));
            returnTable.Columns.Add("Green54", typeof(double));
            returnTable.Columns.Add("Blue54", typeof(double));
            returnTable.Columns.Add("BlobCount", typeof(Int32));
            returnTable.Columns.Add("ColorEdge1", typeof(Int32));
            returnTable.Columns.Add("ColorEdge2", typeof(Int32));
            returnTable.Columns.Add("ColorEdge3", typeof(Int32));
            returnTable.Columns.Add("ColorEdge4", typeof(Int32));
            returnTable.Columns.Add("EdgeWidth1", typeof(Int32));
            returnTable.Columns.Add("EdgeWidth2", typeof(Int32));
            returnTable.Columns.Add("EdgeWidth3", typeof(Int32));
            returnTable.Columns.Add("EdgeWidth4", typeof(Int32));

            return returnTable;

            #endregion
        }

        private DataTable GetDataTable_AnalysisParasiteFecalsShapesInterior(string stringTableSuffix)
        {
            #region "*** Create Data Table for _AnalysisParasiteFecalsInterior ***"

            //Select '            returnTable.Columns.Add("' + name + '",' + CASE WHEN system_type_id = 167 THEN 'typeof(string)' 
            //WHEN system_type_id = 62 THEN 'typeof(double)' WHEN system_type_id in (52,56) THEN 'typeof(Int32)' END + ');' from sys.columns where object_id in 
            //(select object_id from sys.tables where name = '_AnalysisParasiteFecalsShapesInterior') order by column_id

            DataTable returnTable = new DataTable(SharedValueController.STRING_TABLE_PARASITEINTERIOR + stringTableSuffix);

            returnTable.Columns.Add("IDParasite", typeof(string));
            returnTable.Columns.Add("BlobIndex", typeof(Int32));
            returnTable.Columns.Add("BlobCount", typeof(Int32));
            returnTable.Columns.Add("BlobArea", typeof(Int32));
            returnTable.Columns.Add("BlobFullness", typeof(double));
            returnTable.Columns.Add("BlobX", typeof(Int32));
            returnTable.Columns.Add("BlobY", typeof(Int32));
            returnTable.Columns.Add("BlobWidth", typeof(Int32));
            returnTable.Columns.Add("BlobHeight", typeof(Int32));
            returnTable.Columns.Add("BlobCenterX", typeof(double));
            returnTable.Columns.Add("BlobCenterY", typeof(double));
            returnTable.Columns.Add("BlobSizeRatio", typeof(double));
            returnTable.Columns.Add("BlobDistanceCenter", typeof(double));
            returnTable.Columns.Add("BlobDistancePrevious", typeof(double));
            returnTable.Columns.Add("BlobAreaRatioPrevious", typeof(double));
            returnTable.Columns.Add("BlobMinimumDistancePrevious", typeof(double));
            returnTable.Columns.Add("BlobMaximumDistancePrevious", typeof(double));
            returnTable.Columns.Add("BlobOffsetX", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetY", typeof(Int32));
            returnTable.Columns.Add("BlobCircleVariance", typeof(double));
            returnTable.Columns.Add("BlobDistanceEdge", typeof(double));
            returnTable.Columns.Add("BlobDistanceEdgePrevious", typeof(double));
            returnTable.Columns.Add("BlobOffsetXPrevious", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetYPrevious", typeof(Int32));
            

            return returnTable;

            #endregion
        }

        private DataTable GetDataTable_AnalysisParasiteFecalsShapesAnalysis(string stringTableSuffix)
        {
            #region "*** Create Data Table for _AnalysisParasiteFecalsShapesAnalysis ***"

            //Select '            returnTable.Columns.Add("' + name + '",' + CASE WHEN system_type_id = 167 THEN 'typeof(string)' 
            //WHEN system_type_id = 62 THEN 'typeof(double)' WHEN system_type_id in (52,56,127) THEN 'typeof(Int32)' END + ');', * from sys.columns where object_id in 
            //(select object_id from sys.tables where name = '_AnalysisParasiteFecalsShapesAnalysis') order by column_id

            DataTable returnTable = new DataTable(SharedValueController.STRING_TABLE_PARASITESHAPES + stringTableSuffix);

            returnTable.Columns.Add("ID", typeof(Int32));
            returnTable.Columns.Add("IDParasite", typeof(string));
            returnTable.Columns.Add("CaseID", typeof(string));
            returnTable.Columns.Add("ResultValue", typeof(Int32));
            returnTable.Columns.Add("PointIndex", typeof(Int32));
            returnTable.Columns.Add("PointIndexOpposite", typeof(Int32));
            returnTable.Columns.Add("DistanceAverage", typeof(double));
            returnTable.Columns.Add("Distance", typeof(double));
            returnTable.Columns.Add("DistanceDifference", typeof(double));
            returnTable.Columns.Add("DistanceRatio", typeof(double));
            returnTable.Columns.Add("DistanceAverageOpposite", typeof(double));
            returnTable.Columns.Add("DistanceOpposite", typeof(double));
            returnTable.Columns.Add("DistanceDifferenceOpposite", typeof(double));
            returnTable.Columns.Add("DistanceRatioOpposite", typeof(double));
            returnTable.Columns.Add("DistanceDifferencePrevious", typeof(double));
            returnTable.Columns.Add("Quadrant", typeof(Int32));
            returnTable.Columns.Add("PointX", typeof(Int32));
            returnTable.Columns.Add("PointY", typeof(Int32));
            returnTable.Columns.Add("PointXAverage", typeof(double));

            return returnTable;

            #endregion
        }

        private DataTable GetDataTable_AnalysisParasiteFecalsShapesCoccidia(string stringTableSuffix)
        {
            #region "*** Create Data Table for _AnalysisParasiteFecalsCoccidia ***"

            //Select '            returnTable.Columns.Add("' + name + '",' + CASE WHEN system_type_id = 167 THEN 'typeof(string)' 
            //WHEN system_type_id = 62 THEN 'typeof(double)' WHEN system_type_id in (52,56) THEN 'typeof(Int32)' END + ');' from sys.columns where object_id in 
            //(select object_id from sys.tables where name = '_AnalysisParasiteFecalsShapesCoccidia') order by column_id

            DataTable returnTable = new DataTable(SharedValueController.STRING_TABLE_PARASITECOCCIDIA + stringTableSuffix);

            returnTable.Columns.Add("IDParasite", typeof(string));
            returnTable.Columns.Add("BlobCount", typeof(Int32));
            returnTable.Columns.Add("BlobX1", typeof(Int32));
            returnTable.Columns.Add("BlobY1", typeof(Int32));
            returnTable.Columns.Add("BlobXCenter1", typeof(Int32));
            returnTable.Columns.Add("BlobYCenter1", typeof(Int32));
            returnTable.Columns.Add("BlobWidth1", typeof(Int32));
            returnTable.Columns.Add("BlobHeight1", typeof(Int32));
            returnTable.Columns.Add("BlobArea1", typeof(Int32));
            returnTable.Columns.Add("BlobFullness1", typeof(double));
            returnTable.Columns.Add("BlobDistanceCenter1", typeof(double));
            returnTable.Columns.Add("BlobOffsetX1", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetY1", typeof(Int32));
            returnTable.Columns.Add("BlobCircleVariance1", typeof(double));
            returnTable.Columns.Add("BlobX2", typeof(Int32));
            returnTable.Columns.Add("BlobY2", typeof(Int32));
            returnTable.Columns.Add("BlobXCenter2", typeof(Int32));
            returnTable.Columns.Add("BlobYCenter2", typeof(Int32));
            returnTable.Columns.Add("BlobWidth2", typeof(Int32));
            returnTable.Columns.Add("BlobHeight2", typeof(Int32));
            returnTable.Columns.Add("BlobArea2", typeof(Int32));
            returnTable.Columns.Add("BlobFullness2", typeof(double));
            returnTable.Columns.Add("BlobDistanceCenter2", typeof(double));
            returnTable.Columns.Add("BlobOffsetX2", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetY2", typeof(Int32));
            returnTable.Columns.Add("BlobCircleVariance2", typeof(double));
            returnTable.Columns.Add("BlobAreaRatio21", typeof(double));
            returnTable.Columns.Add("BlobDistance21", typeof(double));
            returnTable.Columns.Add("BlobMinimumDistance21", typeof(double));
            returnTable.Columns.Add("BlobMaximumDistance21", typeof(double));
            returnTable.Columns.Add("BlobOffsetX21", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetY21", typeof(Int32));
            returnTable.Columns.Add("BlobX3", typeof(Int32));
            returnTable.Columns.Add("BlobY3", typeof(Int32));
            returnTable.Columns.Add("BlobXCenter3", typeof(Int32));
            returnTable.Columns.Add("BlobYCenter3", typeof(Int32));
            returnTable.Columns.Add("BlobWidth3", typeof(Int32));
            returnTable.Columns.Add("BlobHeight3", typeof(Int32));
            returnTable.Columns.Add("BlobArea3", typeof(Int32));
            returnTable.Columns.Add("BlobFullness3", typeof(double));
            returnTable.Columns.Add("BlobDistanceCenter3", typeof(double));
            returnTable.Columns.Add("BlobOffsetX3", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetY3", typeof(Int32));
            returnTable.Columns.Add("BlobCircleVariance3", typeof(double));
            returnTable.Columns.Add("BlobAreaRatio31", typeof(double));
            returnTable.Columns.Add("BlobDistance31", typeof(double));
            returnTable.Columns.Add("BlobMinimumDistance31", typeof(double));
            returnTable.Columns.Add("BlobMaximumDistance31", typeof(double));
            returnTable.Columns.Add("BlobOffsetX31", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetY31", typeof(Int32));
            returnTable.Columns.Add("BlobAreaRatio32", typeof(double));
            returnTable.Columns.Add("BlobDistance32", typeof(double));
            returnTable.Columns.Add("BlobMinimumDistance32", typeof(double));
            returnTable.Columns.Add("BlobMaximumDistance32", typeof(double));
            returnTable.Columns.Add("BlobOffsetX32", typeof(Int32));
            returnTable.Columns.Add("BlobOffsetY32", typeof(Int32));

            return returnTable;

            #endregion
        }

        private DataRow GetDataRow_AnalysisParasiteFecals(string CaseID, string RequestID, int indexRegion, int cellType, string IDParasite, int indexParasiteType, Rectangle objectRectangleBlob, double blobFullness, double fullness, double focusContrast, double axisMinor, double axisMajor, double circleVariance, int blobCountInterior, ImageStatistics objectStatisticsParasite, int[] colorEdge, int[] edgeWidth)
        {

            #region "*** Populate _AnalysisParasiteFecals Data ***"
            
            //returnTable.Columns.Add("ID", typeof(Int32));
            //returnTable.Columns.Add("CaseID", typeof(string));
            //returnTable.Columns.Add("RequestID", typeof(Int32));
            //returnTable.Columns.Add("Region", typeof(Int32));
            //returnTable.Columns.Add("ResultValue", typeof(Int32));
            //returnTable.Columns.Add("IDParasite", typeof(string));
            //returnTable.Columns.Add("Species", typeof(Int32));
            //returnTable.Columns.Add("BlobTop", typeof(Int32));
            //returnTable.Columns.Add("BlobLeft", typeof(Int32));
            //returnTable.Columns.Add("BlobWidth", typeof(Int32));
           // returnTable.Columns.Add("BlobHeight", typeof(Int32));
            //returnTable.Columns.Add("BlobFullness", typeof(double));
            //returnTable.Columns.Add("Fullness", typeof(double));
            //returnTable.Columns.Add("FocusContrast", typeof(double));
            //returnTable.Columns.Add("AxisMajor", typeof(double));
            //returnTable.Columns.Add("AxisMinor", typeof(double));
            //returnTable.Columns.Add("AxisRatio", typeof(double));
            //returnTable.Columns.Add("CircleVariance", typeof(double));


            DataRow returnRow = dataTableAnalysisParasiteFecals.NewRow();
            if ((axisMinor == null) || (axisMinor == Double.NaN)) { axisMinor = 0; }
            if ((axisMajor == 0) || (axisMajor == null) || (axisMajor == Double.NaN)) { axisMajor = 1; }
            double axisRatio;
            try { axisRatio = Math.Round(axisMinor, 2) / Math.Round(axisMajor, 2); }
            catch { axisRatio = 0; }
            if ((axisRatio == null) || (axisRatio == Double.NaN)) { axisRatio = 0; }

            returnRow[1] = CaseID;
            returnRow[2] = Convert.ToInt32(RequestID);
            returnRow[3] = indexRegion;
            returnRow[4] = cellType;
            returnRow[5] = IDParasite;
            returnRow[6] = indexParasiteType;
            returnRow[7] = objectRectangleBlob.Top;
            returnRow[8] = objectRectangleBlob.Left;
            returnRow[9] = objectRectangleBlob.Width;
            returnRow[10] = objectRectangleBlob.Height;
            returnRow[11] = blobFullness;
            returnRow[12] = fullness;
            returnRow[13] = focusContrast;
            returnRow[14] = Math.Round(axisMajor, 2);
            returnRow[15] = Math.Round(axisMinor, 2);
            returnRow[16] = axisRatio;
            returnRow[17] = circleVariance;

            #endregion

            #region "*** Populate _AnalysisParasiteFecals Color ***"

            double doublePixelRange = .96;
            int indexColumn = 23;

            returnRow[18] = Math.Round(objectStatisticsParasite.Red.Mean,4);
            returnRow[19] = Math.Round(objectStatisticsParasite.Green.Mean, 4);
            returnRow[20] = Math.Round(objectStatisticsParasite.Blue.Mean, 4);
            returnRow[21] = Math.Round(objectStatisticsParasite.Red.StdDev, 4);
            returnRow[22] = Math.Round(objectStatisticsParasite.Green.StdDev, 4);
            returnRow[23] = Math.Round(objectStatisticsParasite.Blue.StdDev, 4);
            
            while (doublePixelRange > 0)
            {
                indexColumn++; returnRow[indexColumn] = objectStatisticsParasite.Red.GetRange(doublePixelRange).Min;
                indexColumn++; returnRow[indexColumn] = objectStatisticsParasite.Green.GetRange(doublePixelRange).Min;
                indexColumn++; returnRow[indexColumn] = objectStatisticsParasite.Blue.GetRange(doublePixelRange).Min;
                indexColumn++; returnRow[indexColumn] = objectStatisticsParasite.Red.GetRange(doublePixelRange).Max;
                indexColumn++; returnRow[indexColumn] = objectStatisticsParasite.Green.GetRange(doublePixelRange).Max;
                indexColumn++; returnRow[indexColumn] = objectStatisticsParasite.Blue.GetRange(doublePixelRange).Max;
                if (doublePixelRange == .96) { doublePixelRange = doublePixelRange - .04; }
                else
                { doublePixelRange = doublePixelRange - .14; }
            }

            returnRow[dataTableAnalysisParasiteFecals.Columns.Count - 9] = blobCountInterior;

            for (int indexColorEdge = 0; indexColorEdge < 4; indexColorEdge++)
            {
                returnRow[dataTableAnalysisParasiteFecals.Columns.Count - (8 - indexColorEdge)] = colorEdge[indexColorEdge];
                returnRow[dataTableAnalysisParasiteFecals.Columns.Count - (4 - indexColorEdge)] = edgeWidth[indexColorEdge];
            }

            return returnRow;

            #endregion

        }

        private DataRow GetDataRow_AnalysisParasiteFecalsShapesInterior(string IDParasite, int blobIndex, int blobCount, int blobArea, double blobFullness, Rectangle objectRectangleBlob, double blobCenterX, double blobCenterY, double blobSizeRatio, double blobDistanceCenter, double blobDistancePrevious, double blobAreaRatioPrevious, double blobMinimumDistancePrevious, double blobMaximumDistancePrevious, double distanceCircleVariance, int blobOffsetX, int blobOffsetY, double blobDistanceEdge, double blobDistanceEdgePrevious, int blobOffsetXPrevious, int blobOffsetYPrevious, FECALPARASITECONDITION_GRAYSCALE_CONVERSION grayscaleType)
        {

            #region "*** Populate _AnalysisParasiteFecals Data ***"

            if (grayscaleType == FECALPARASITECONDITION_GRAYSCALE_CONVERSION.Blue) { blobOffsetXPrevious = 2; }
            else { blobOffsetXPrevious = 1; }

            DataRow returnRow = dataTableAnalysisParasiteFecalsShapesInterior.NewRow();

            returnRow[0] = IDParasite;
            returnRow[1] = blobIndex;
            returnRow[2] = blobCount;
            returnRow[3] = blobArea;
            returnRow[4] = blobFullness;
            returnRow[5] = objectRectangleBlob.X;
            returnRow[6] = objectRectangleBlob.Y;
            returnRow[7] = objectRectangleBlob.Width;
            returnRow[8] = objectRectangleBlob.Height;
            returnRow[9] = blobCenterX;
            returnRow[10] = blobCenterY;
            returnRow[11] = blobSizeRatio;
            returnRow[12] = blobDistanceCenter;
            returnRow[13] = blobDistancePrevious;
            returnRow[14] = blobAreaRatioPrevious;
            returnRow[15] = blobMinimumDistancePrevious;
            returnRow[16] = blobMaximumDistancePrevious;
            returnRow[17] = blobOffsetX;
            returnRow[18] = blobOffsetY;
            returnRow[19] = distanceCircleVariance;
            returnRow[20] = blobDistanceEdge;
            returnRow[21] = blobDistanceEdgePrevious;
            returnRow[22] = blobOffsetXPrevious;
            returnRow[23] = blobOffsetYPrevious;

            return returnRow;

            #endregion

        }

        private DataRow GetDataRow_AnalysisParasiteFecalsShapesAnalysis(string IDParasite, string CaseID, int cellType, int pointIndex, int pointIndexOpposite, double distanceAverage, double distance, double distanceDifference, double distanceRatio, double distanceAverageOpposite, double distanceOpposite, double distanceDifferenceOpposite, double distanceRatioOpposite, double distanceDifferencePrevious, int quadrant, int pointX, int pointY, double pointXAverage)
        {
            #region "*** Populate _AnalysisParasiteFecals Data ***"

            DataRow returnRow = dataTableAnalysisParasiteFecalsShapesAnalysis.NewRow();

            //returnTable.Columns.Add("ID", typeof(Int32));
            //returnTable.Columns.Add("IDParasite", typeof(string));
            //returnTable.Columns.Add("CaseID", typeof(string));
            //returnTable.Columns.Add("ResultValue", typeof(Int32));
            //returnTable.Columns.Add("PointIndex", typeof(Int32));
            //returnTable.Columns.Add("PointIndexOpposite", typeof(Int32));
            //returnTable.Columns.Add("DistanceAverage", typeof(double));
            //returnTable.Columns.Add("Distance", typeof(double));
            //returnTable.Columns.Add("DistanceDifference", typeof(double));
            //returnTable.Columns.Add("DistanceRatio", typeof(double));
            //returnTable.Columns.Add("DistanceAverageOpposite", typeof(double));
            //returnTable.Columns.Add("DistanceOpposite", typeof(double));
            //returnTable.Columns.Add("DistanceDifferenceOpposite", typeof(double));
            //returnTable.Columns.Add("DistanceRatioOpposite", typeof(double));
            //returnTable.Columns.Add("DistanceDifferencePrevious", typeof(double));
            //returnTable.Columns.Add("Quadrant", typeof(Int32));
            //returnTable.Columns.Add("PointX", typeof(Int32));
            //returnTable.Columns.Add("PointY", typeof(Int32));
            //returnTable.Columns.Add("PointXAverage", typeof(double));
            if ((distanceRatio == null) || (distanceRatio == Double.NaN)) { distanceRatio = 0; }

            returnRow[1] = IDParasite;
            returnRow[2] = CaseID;
            returnRow[3] = cellType;
            returnRow[4] = pointIndex;
            returnRow[5] = pointIndexOpposite;
            returnRow[6] = distanceAverage;
            returnRow[7] = distance;
            returnRow[8] = Math.Round(distanceDifference,4);
            returnRow[9] = Math.Round(distanceRatio, 4);
            returnRow[10] = distanceAverageOpposite;
            returnRow[11] = Math.Round(distanceOpposite, 4);
            returnRow[12] = distanceDifferenceOpposite;
            returnRow[13] = distanceRatioOpposite;
            returnRow[14] = Math.Round(distanceDifferencePrevious, 4);
            returnRow[15] = quadrant;
            returnRow[16] = pointX;
            returnRow[17] = pointY;
            returnRow[18] = pointXAverage;
              
            return returnRow;

            #endregion
        }

        private DataRow GetDataRow_AnalysisParasiteFecalsShapesCoccidia(string IDParasite, int blobCount, double[] blobsDistanceCenter, int[] blobsOffsetX, int[] blobsOffsetY, double[] blobsCircleVariance, double distance21, double distance21Minimum, double distance21Maximum, double distance31, double distance31Minimum, double distance31Maximum, double distance32, double distance32Minimum, double distance32Maximum, List<Blob> blobsCoccidia)
        {
            #region "*** Populate _AnalysisParasiteFecals Data ***"

            if (blobsCoccidia.Count == 2) { blobsCoccidia.Add(blobsCoccidia[0]); }

            DataRow returnRow = dataTableAnalysisParasiteFecalsShapesCoccidia.NewRow();
             
            //*** First Blob for Coccidia ***
            returnRow[0] = IDParasite;
            returnRow[1] = blobCount;
            returnRow[2] = blobsCoccidia[0].Rectangle.X;
            returnRow[3] = blobsCoccidia[0].Rectangle.Y;
            returnRow[4] = (int)blobsCoccidia[0].CenterOfGravity.X;
            returnRow[5] = (int)blobsCoccidia[0].CenterOfGravity.Y;
            returnRow[6] = blobsCoccidia[0].Rectangle.Width;
            returnRow[7] = blobsCoccidia[0].Rectangle.Height;
            returnRow[8] = blobsCoccidia[0].Area;
            returnRow[9] = blobsCoccidia[0].Fullness;
            returnRow[10] = blobsDistanceCenter[0];
            returnRow[11] = blobsOffsetX[0];
            returnRow[12] = blobsOffsetY[0];
            returnRow[13] = blobsCircleVariance[0];
            //*** Second Blob for Coccidia ***
            returnRow[14] = blobsCoccidia[1].Rectangle.X;
            returnRow[15] = blobsCoccidia[1].Rectangle.Y;
            returnRow[16] = (int)blobsCoccidia[1].CenterOfGravity.X;
            returnRow[17] = (int)blobsCoccidia[1].CenterOfGravity.Y;
            returnRow[18] = blobsCoccidia[1].Rectangle.Width;
            returnRow[19] = blobsCoccidia[1].Rectangle.Height;
            returnRow[20] = blobsCoccidia[1].Area;
            returnRow[21] = blobsCoccidia[1].Fullness;
            returnRow[22] = blobsDistanceCenter[1];
            returnRow[23] = blobsOffsetX[1];
            returnRow[24] = blobsOffsetY[1];
            returnRow[25] = blobsCircleVariance[1];
            //*** Compare Blobs 2 and 1 ***
            returnRow[26] = (double)blobsCoccidia[1].Area / (double)blobsCoccidia[0].Area;
            returnRow[27] = distance21;
            returnRow[28] = distance21Minimum;
            returnRow[29] = distance21Maximum;
            returnRow[30] = Math.Abs(blobsOffsetX[1] - blobsOffsetX[0]);
            returnRow[31] = Math.Abs(blobsOffsetY[1] - blobsOffsetY[0]);
            //*** Third Blob for Coccidia ***
            returnRow[32] = blobsCoccidia[2].Rectangle.X;
            returnRow[33] = blobsCoccidia[2].Rectangle.Y;
            returnRow[34] = (int)blobsCoccidia[2].CenterOfGravity.X;
            returnRow[35] = (int)blobsCoccidia[2].CenterOfGravity.Y;
            returnRow[36] = blobsCoccidia[2].Rectangle.Width;
            returnRow[37] = blobsCoccidia[2].Rectangle.Height;
            returnRow[38] = blobsCoccidia[2].Area;
            returnRow[39] = blobsCoccidia[2].Fullness;
            returnRow[40] = blobsDistanceCenter[2];
            returnRow[41] = blobsOffsetX[2];
            returnRow[42] = blobsOffsetY[2];
            returnRow[43] = blobsCircleVariance[2];
            //*** Compare Blobs 3 and 1 ***
            returnRow[44] = (double)blobsCoccidia[2].Area / (double)blobsCoccidia[0].Area;
            returnRow[45] = distance31;
            returnRow[46] = distance31Minimum;
            returnRow[47] = distance31Maximum;
            returnRow[48] = Math.Abs(blobsOffsetX[2] - blobsOffsetX[0]);
            returnRow[49] = Math.Abs(blobsOffsetY[2] - blobsOffsetY[0]);
            //*** Compare Blobs 3 and 2 ***
            returnRow[50] = (double)blobsCoccidia[2].Area / (double)blobsCoccidia[1].Area;
            returnRow[51] = distance32;
            returnRow[52] = distance32Minimum;
            returnRow[53] = distance32Maximum;
            returnRow[54] = Math.Abs(blobsOffsetX[2] - blobsOffsetX[1]);
            returnRow[55] = Math.Abs(blobsOffsetY[2] - blobsOffsetY[1]);

            return returnRow;

            #endregion
        }

        private void UpdateDatabase(string CaseID, List<string> caseSQL, bool IsTraining_Database)
        {
            #region "*** Update DataTables to Database ***"

            if (IsTraining_Database == false) { return; }
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(dataController.GetDataConnection());
            sqlBulkCopy.BulkCopyTimeout = 0;
            sqlBulkCopy.BatchSize = 125000;
            sqlBulkCopy.DestinationTableName = dataTableAnalysisParasiteFecals.TableName;
            dataTableAnalysisParasiteFecals.AcceptChanges();
            sqlBulkCopy.WriteToServer(dataTableAnalysisParasiteFecals);
            //dataController.ExecuteQuery("Update " + dataTableAnalysisParasiteFecals.TableName + " Set AxisRatio = Round(AxisMinor / AxisMajor,3) Where CaseID = '" + CaseID + "' and AxisMajor > 0");
            sqlBulkCopy.DestinationTableName = dataTableAnalysisParasiteFecalsShapesInterior.TableName;
            dataTableAnalysisParasiteFecalsShapesInterior.AcceptChanges();
            sqlBulkCopy.WriteToServer(dataTableAnalysisParasiteFecalsShapesInterior);
            sqlBulkCopy.DestinationTableName = dataTableAnalysisParasiteFecalsShapesCoccidia.TableName;
            dataTableAnalysisParasiteFecalsShapesCoccidia.AcceptChanges();
            sqlBulkCopy.WriteToServer(dataTableAnalysisParasiteFecalsShapesCoccidia);
            sqlBulkCopy.DestinationTableName = dataTableAnalysisParasiteFecalsShapesAnalysis.TableName;
            dataTableAnalysisParasiteFecalsShapesAnalysis.AcceptChanges();
            int insertAttempts = 0;
            do
            {
                sqlBulkCopy.WriteToServer(dataTableAnalysisParasiteFecalsShapesAnalysis);
                sqlBulkCopy.BatchSize = 100000;
                insertAttempts++;
            }
            while ((Convert.ToInt32(dataController.GetDataScalarFromQuery("Select Count(*) from " + dataTableAnalysisParasiteFecalsShapesAnalysis.TableName + " Where CaseID = '" + CaseID + "'")) < dataTableAnalysisParasiteFecalsShapesAnalysis.Rows.Count) && (insertAttempts > 3));

            try
            {
                dataTableAnalysisParasiteFecals.Clear(); dataTableAnalysisParasiteFecalsShapesInterior.Clear(); dataTableAnalysisParasiteFecalsShapesAnalysis.Clear();
                dataTableAnalysisParasiteFecals.AcceptChanges(); dataTableAnalysisParasiteFecalsShapesInterior.AcceptChanges(); dataTableAnalysisParasiteFecalsShapesAnalysis.AcceptChanges();
            }
            catch
            {
                dataTableAnalysisParasiteFecals = null; dataTableAnalysisParasiteFecalsShapesInterior = null; dataTableAnalysisParasiteFecalsShapesAnalysis = null;
            }
            sqlBulkCopy.Close();
            try
            {
                foreach(string stringSQL in caseSQL)
                {
                    dataController.ExecuteQuery(stringSQL);
                    System.Threading.Thread.Sleep(250);
                }
            }
            catch { }

            #endregion
        }

        private void InsertDatabaseResult(string CaseID, string RequestID, int ResultValue, string fileName, double focusContrast, double thresholdScore, double thresholdScoreMinimum)
        {
            double confidenceValue = (focusContrast / objectSpecies.ShapeFocusContrastMinimum) + (thresholdScore / thresholdScoreMinimum);
            int ReportOrder = GetReportOrderFromParasiteType(ResultValue);
            string stringSQL = "Insert Into AnalysisRequestsResults (RequestID,CaseID,ResultValue,ReportOrder,FileName,ConfidenceValue) Values (" + RequestID + st(CaseID) + st(ResultValue) + st(ReportOrder) + st(fileName) + st(confidenceValue) + ")";
            dataController.ExecuteQuery(stringSQL);
        }

        #endregion

        #region "*** Private String Helper Methods ***"

        private string GetFileSavePath(string directoryImages, string CaseID, string IDParasite, string stringTableSuffix, bool IsSavedCase, bool IsKnownPositive, bool IsKnownNegative)
        {
            string filePath = directoryImages + "\\POSITIVES\\" + IDParasite;
            string folderPath = directoryImages + "\\POSITIVES\\" + CaseID;
            if (stringTableSuffix != "")
            {
                folderPath = directoryImages + "\\NEGATIVES\\" + CaseID;
                filePath = directoryImages + "\\NEGATIVES\\" + IDParasite;
            }
            
            if (IsSavedCase == true)
            {
                DirectoryInfo objectDirectoryInfo = new DirectoryInfo(folderPath);
                if (objectDirectoryInfo.Exists == false) { objectDirectoryInfo.Create(); }
                filePath = objectDirectoryInfo.FullName + "\\" + IDParasite;
                objectDirectoryInfo = null;
            }
            else if (IsKnownNegative == true)
            {
                filePath = directoryImages + "\\NEGATIVES\\" + IDParasite;
            }
            else if (IsKnownPositive == false)
            {
                filePath = directoryImages + "\\" + IDParasite;
            }
            
            return filePath;
        }

        private string GetStringPercent(double doublePercent, bool IsMaximum)
        {

            if (IsMaximum == false)
            {
                doublePercent = .50 - (doublePercent / 2);
            }
            else
            {
                doublePercent = .50 + (doublePercent / 2);
            }
            doublePercent = Math.Round(doublePercent, 2);
            string returnValue = Convert.ToString(doublePercent).Replace(".", "").Substring(1, 2);
            return returnValue;
        }

        private string st(int value)
        {
            return "," + Convert.ToString(value);
        }

        private string st(double value)
        {
            value = Math.Round(value, 4);
            return "," + Convert.ToString(value);
        }

        private string st(string value)
        {
            return ",'" + value + "'";
        }

        private string st(bool value)
        {
            if (value == false)
            {
                return ",0";
            }
            else
            {
                return ",1";
            }
        }

        private string mr(int value)
        {
            return "_" + Convert.ToString(value);
        }

        private string mr(double value)
        {
            return "_" + Convert.ToString(Math.Round(value, 0));
        }

        private List<string> GetDebugFileStrings()
        {
            List<string> returnStrings = new List<string>();
            returnStrings.Add("062717_TOX1_009_006");
            returnStrings.Add("070854_TOX2_004_010");
            returnStrings.Add("041034_TOX1_003_003");
            returnStrings.Add("040311_TOX2_CCI2_CCE4_003_005");
            returnStrings.Add("023423_TOX3_009_012");
            returnStrings.Add("005115_TOX3_LT_010_006");
            returnStrings.Add("005115_TOX3_LT_010_007");
            returnStrings.Add("005115_TOX3_LT_010_009");
            returnStrings.Add("005115_TOX3_LT_010_012");
            returnStrings.Add("005115_TOX3_LT_011_007");
            returnStrings.Add("005115_TOX3_LT_011_010");
            returnStrings.Add("005115_TOX3_LT_011_012");
            returnStrings.Add("005115_TOX3_LT_012_003");
            returnStrings.Add("005115_TOX3_LT_012_004");
            returnStrings.Add("003404_TOX1_GIAR4_005_001");
            returnStrings.Add("070854_TOX2_004_010");
            returnStrings.Add("070854_TOX2_008_008");
            returnStrings.Add("070854_TOX2_008_010");
            returnStrings.Add("070854_TOX2_014_009");
            return returnStrings;

        }

        #endregion

        #region "*** Private Parasite Helper Methods ***"

        private TrainingModel[] GetTrainingModels()
        {
            return TrainingModelController.GetTrainingModelsParasites();
        }

        private int GetReportOrderFromParasiteType(int resultValue)
        {
            if (resultValue == (int)FECALPARASITE_TYPE.Eimeria)
            {
                return (int)FECALPARASITE_REPORT.Eimeria;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Isospora)
            {
                return (int)FECALPARASITE_REPORT.Isospora;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Hookworm)
            {
                return (int)FECALPARASITE_REPORT.Hookworm;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Trichuris)
            {
                return (int)FECALPARASITE_REPORT.Trichuris;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Toxocara)
            {
                return (int)FECALPARASITE_REPORT.Toxocara;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Giardia)
            {
                return (int)FECALPARASITE_REPORT.Giardia;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Taenia)
            {
                return (int)FECALPARASITE_REPORT.Taenia;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Dipylidian)
            {
                return (int)FECALPARASITE_REPORT.Dipylidian;
            }
            else if (resultValue == (int)FECALPARASITE_TYPE.Strongyle)
            {
                return (int)FECALPARASITE_REPORT.Hookworm;
            }
            else
            {
                return 15;
            }


        }

        private Bitmap GetGiardiaFromBitmap(Bitmap bitmapIn)
        {
            int[] VALUES_GRAYSCALE = new int[9] { 104, 120, 136, 152, 168, 184, 200, 216, 232 };

            Random randomNumber = new Random((int)System.DateTime.Now.Ticks);
            int giardiaImageNumber = randomNumber.Next(1,7);
            Bitmap objectBitmapContrastStretch = new ContrastStretch().Apply(bitmapIn);
            Bitmap objectBitmapGrayscale = new ExtractChannel(RGB.R).Apply(objectBitmapContrastStretch);
            ImageStatistics objectImageStatistics = new ImageStatistics(objectBitmapGrayscale);
            int threshold = objectImageStatistics.Gray.GetRange(.6).Min;
            int maximumWhite = objectImageStatistics.Gray.Max;
            new Threshold(threshold).ApplyInPlace(objectBitmapGrayscale);
            new Invert().ApplyInPlace(objectBitmapGrayscale);
            AForge.Imaging.BlobCounter objectBlobCounter = new BlobCounter(objectBitmapGrayscale);
            objectBlobCounter.ObjectsOrder = ObjectsOrder.Size;
            Blob blob = objectBlobCounter.GetObjectsInformation()[0];
            List<IntPoint> edgePoints = objectBlobCounter.GetBlobsEdgePoints(blob);
            edgePoints.Sort(delegate(IntPoint r1, IntPoint r2)
            {
                return r1.X.CompareTo(r2.X) != 0 ? r1.X.CompareTo(r2.X) : r1.Y.CompareTo(r2.Y);
            });
            IntPoint pointTopLeft = edgePoints[0];
            IntPoint pointBottomRight = edgePoints[edgePoints.Count - 1];
            double dx = pointBottomRight.X - pointTopLeft.X;  //centers - a collection of 2 centers of an Image no.0 is the top left center of cube, no.1 is  the top right
            double dy = pointTopLeft.Y - pointBottomRight.Y;
            double ang = 0 - (Math.Atan2(dx, dy) * (180 / Math.PI) - 90);                      

            string giardiaImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\Images\\" + SharedValueController.STRING_FILENAME_GIARDIA + "_" + String.Format("{0:D2}",giardiaImageNumber) + ".JPG";
            
            Bitmap objectBitmapGiardia = new Bitmap(giardiaImagePath);
            objectBitmapGiardia = SharedFunctionController.rotateImageWithCrop(objectBitmapGiardia, ang, VALUES_GRAYSCALE[VALUES_GRAYSCALE.Length - 1] + 1);
            objectBitmapGiardia = new ResizeBilinear(bitmapIn.Width, bitmapIn.Height).Apply(objectBitmapGiardia);
            SimplePosterization objectPosterization = new SimplePosterization();
            objectPosterization.PosterizationInterval = 16;
            objectPosterization.ApplyInPlace(objectBitmapGiardia);

            objectImageStatistics = new ImageStatistics(bitmapIn);
            int valueRed = objectImageStatistics.Red.Min;
            int valueGreen = objectImageStatistics.Green.Min;
            int valueBlue = objectImageStatistics.Blue.Min;
            //int valueRedStep = ((SharedFunctionController.GetColorFromPercent(objectImageStatistics.Red,.93) - valueRed) / 8);
            //int valueGreenStep = ((SharedFunctionController.GetColorFromPercent(objectImageStatistics.Green,.93) - valueGreen) / 8);
            //int valueBlueStep = ((SharedFunctionController.GetColorFromPercent(objectImageStatistics.Blue,.93) - valueBlue) / 8);

            ColorFiltering colorFilter;
            
            for (int indexColor = 0; indexColor < VALUES_GRAYSCALE.Length; indexColor++)
            {
                IntRange grayscaleRange = new IntRange(VALUES_GRAYSCALE[indexColor], VALUES_GRAYSCALE[indexColor]);
                colorFilter = new ColorFiltering(grayscaleRange, grayscaleRange, grayscaleRange);
                colorFilter.FillOutsideRange = false;
                colorFilter.FillColor = new RGB((byte)valueRed, (byte)valueGreen, (byte)valueBlue);
                colorFilter.ApplyInPlace(objectBitmapGiardia);
                //valueRed = valueRed + valueRedStep;
                //valueGreen = valueGreen + valueGreenStep;
                //valueBlue = valueBlue + valueBlueStep;
                valueRed = (int)(valueRed * 1.08); if (valueRed > 237) { valueRed = 237; }
                valueGreen = (int)(valueGreen * 1.08); if (valueGreen > 237) { valueGreen = 237; }
                valueBlue = (int)(valueBlue * 1.08); if (valueBlue > 237) { valueGreen = 237; }
            }

            bitmapIn = new Bitmap(330, 330);
            using (Graphics gfx = Graphics.FromImage(bitmapIn))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                gfx.FillRectangle(brush, 0, 0, bitmapIn.Width, bitmapIn.Height);
                Rectangle sourceRectangle = new Rectangle(0, 0, objectBitmapGiardia.Width, objectBitmapGiardia.Height);
                Rectangle targetRectangle = new Rectangle((bitmapIn.Width / 2) - (sourceRectangle.Width / 2), (bitmapIn.Height / 2) - (sourceRectangle.Height / 2), sourceRectangle.Width, sourceRectangle.Height);
                gfx.DrawImage(objectBitmapGiardia, targetRectangle, sourceRectangle, GraphicsUnit.Pixel);
                gfx.DrawEllipse(new Pen(Color.Blue, 4), targetRectangle.X - 30, targetRectangle.Y - 30, targetRectangle.Width + 60, targetRectangle.Height + 60);
                int parasiteHeight = (int)Math.Round((double)targetRectangle.Height * .45, 0);
                int parasiteWidth = (int)Math.Round((double)targetRectangle.Width * .45, 0);
                string parasiteString = Convert.ToString(parasiteWidth) + "x" + Convert.ToString(parasiteHeight) + " um";
                gfx.DrawString(parasiteString, new Font("Arial", 10, FontStyle.Regular), SystemBrushes.WindowText, (bitmapIn.Width / 2) - 30, targetRectangle.Y + targetRectangle.Height + 2);

            }

            IntRange whiteRange = new IntRange(235, 255);
            colorFilter = new ColorFiltering(whiteRange, whiteRange, whiteRange);
            colorFilter.FillOutsideRange = false;
            colorFilter.FillColor = new RGB(255, 255, 255);
            colorFilter.ApplyInPlace(bitmapIn);

            return bitmapIn;
           
           
        }


        #endregion



    }
}
