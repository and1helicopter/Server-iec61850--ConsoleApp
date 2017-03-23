using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;


namespace ADSPLibrary
{
    static public class StringsFromExMonApplForExcel
    {
        public static string ErrorCreatingFileStr = "Error while creating event log file!";
        public static string EmptyEventStr = "No data";
        public static string EventStr = "Event";
        public static string MeasuresStrs = "Measure parameters";
        public static string DigInputsStr = "Digital inputs";
        public static string DigOutputsStr = "Digital outputs";
        public static string NumberFormatLocalStr = "DD.MM.YY";
        public static string AlarmsStr = "Alarms";
        public static string WarnsStr = "Warns";


        private static void LoadParam(XmlDocument Doc, string AttribName, string ParamName, out String Value)
        {
            XmlNodeList xmls;
            XmlNode xmlNode;

            xmls = Doc.GetElementsByTagName(ParamName);

            if (xmls.Count != 1)
            {
                Value = "";
                return;
            }

            xmlNode = xmls[0];
            try
            {
                Value = Convert.ToString(xmlNode.Attributes[AttribName].Value);
            }
            catch
            {
                Value = "";
            }


        }

        public static void LoadStringsFromXML(string FileName)
        {
            XmlNodeList xmls;
            XmlNode xmlNode;
            int Num = 1;
            string attribName = "Text";

            var doc = new XmlDocument();
            try { doc.Load(FileName); }
            catch
            {
                return;
            }


            xmls = doc.GetElementsByTagName("Language");

            if (xmls.Count != 1)
            {
                return;
            }

            xmlNode = xmls[0];
            try
            {
                Num = Convert.ToInt32(xmlNode.Attributes["Num"].Value);
            }
            catch
            {

            }
            attribName = attribName + Num.ToString();

            LoadParam(doc, attribName, "ErrorCreatingFileStr", out ErrorCreatingFileStr);
            LoadParam(doc, attribName, "EmptyEventStr", out EmptyEventStr);
            LoadParam(doc, attribName, "EventStr", out EventStr);
            LoadParam(doc, attribName, "MeasuresStrs", out MeasuresStrs);
            LoadParam(doc, attribName, "DigInputsStr", out DigInputsStr);
            LoadParam(doc, attribName, "DigOutputsStr", out DigOutputsStr);
            LoadParam(doc, attribName, "NumberFormatLocalStr", out NumberFormatLocalStr);
            LoadParam(doc, attribName, "AlarmsStr", out AlarmsStr);
            LoadParam(doc, attribName, "WarnsStr", out WarnsStr);
        }
    }


    
    static public class ExcelEventLine
    {
        static int nowEventNum = 0;

        static public int NowEventNum { get { return nowEventNum; } set { } }

        public static event EventHandler CreateFileStepFinished;

        static int excelFilePos = 1;

        public static int ExcelFilePos { get { return excelFilePos; } set { } }

        static void WriteExcelLine(int excelIndex, Excel.Workbook eventExcelWorkBook, int eventNum, ushort[,] eventLog)
        {
            try
            {
                ushort u = eventLog[eventNum, EventStructFormat.eventLogCountBlocks * 8 - 1];
                
            }
            catch
            {
                throw new Exception(StringsFromExMonApplForExcel.ErrorCreatingFileStr);
            }

            string eventCode;
            string eventType = StringsFromExMonApplForExcel.EmptyEventStr;
            string str;
            Excel.Range excelcells;
            Excel.Sheets excelsheets;
            Excel.Worksheet excelworksheet;
            excelsheets = eventExcelWorkBook.Worksheets;

            int startIndex = excelIndex+1;
            int i;
            int maxLineCount;
            int lineCount = 0;
            int lineCount1 = 0;
            int lineCount2 = 0;

            excelworksheet = (Excel.Worksheet)excelsheets.get_Item(1);

            #region EventHeader
            excelcells = excelworksheet.get_Range("A" + excelIndex.ToString(), "G" + excelIndex.ToString());
            excelcells.Merge(Type.Missing);
            excelcells.Font.Size = 15;
            excelcells.Font.Italic = true;
            excelcells.Font.Bold = true;

            eventCode = "0x" + eventLog[eventNum,0].ToString("X4");
            eventType = StringsFromExMonApplForExcel.EmptyEventStr; ;

            foreach (string key in EventStructFormat.eventTypesHash.Keys)
            {
                if (key == eventCode) { eventType = (string)EventStructFormat.eventTypesHash[key]; }
            }

            if (eventType == StringsFromExMonApplForExcel.EmptyEventStr)
            {
                excelcells.Value = StringsFromExMonApplForExcel.EventStr+" N" + (eventNum + 1).ToString() + ". " + StringsFromExMonApplForExcel.EmptyEventStr;
                excelFilePos = excelIndex+1;
                return;
            }

            excelcells.Value = StringsFromExMonApplForExcel.EventStr + " N" + (eventNum + 1).ToString() + ". " + eventType;
            excelIndex++;

            str = (eventLog[eventNum, 6] & 0x3F).ToString("X2") + "." + (eventLog[eventNum, 7] & 0x1F).ToString("X2") + "." + (eventLog[eventNum, 8] & 0xFF).ToString("X2") + "  " +
                  (eventLog[eventNum, 4] & 0x3F).ToString("X2") + ":" + (eventLog[eventNum, 3] & 0x7F).ToString("X2") + ":" + (eventLog[eventNum, 2] & 0x7F).ToString("X2");

            excelcells = excelworksheet.get_Range("A" + excelIndex.ToString(), "A" + excelIndex.ToString());
            excelcells.Merge(Type.Missing);
            excelcells.ClearFormats();
            //excelcells.NumberFormat = "общий";
            excelcells.NumberFormatLocal = StringsFromExMonApplForExcel.NumberFormatLocalStr;
            excelcells.Font.Size = 15;
            excelcells.Font.Italic = true;
            excelcells.Font.Bold = true;
            excelcells.Value = str;


            excelIndex++;
            excelcells = excelworksheet.get_Range("A" + excelIndex.ToString(), "C" + excelIndex.ToString());
            excelcells.Merge(Type.Missing);

            excelcells.Value = StringsFromExMonApplForExcel.MeasuresStrs;
            excelcells.Font.Size = 12;
            excelcells.Font.Italic = false;
            excelcells.Font.Bold = true;

            excelcells = excelworksheet.get_Range("D" + excelIndex.ToString(), "D" + excelIndex.ToString());
            excelcells.Merge(Type.Missing);
            excelcells.Value = StringsFromExMonApplForExcel.DigInputsStr;//"Дискретные входы";
            excelcells.Font.Size = 12;
            excelcells.Font.Italic = false;
            excelcells.Font.Bold = true;

            excelcells = excelworksheet.get_Range("E" + excelIndex.ToString(), "E" + excelIndex.ToString());
            excelcells.Merge(Type.Missing);
            excelcells.Value = StringsFromExMonApplForExcel.DigOutputsStr;//"Дискретные выходы";
            excelcells.Font.Size = 12;
            excelcells.Font.Italic = false;
            excelcells.Font.Bold = true;
            
            #endregion

            #region MeasureParams
            for (i = 0; i < (EventStructFormat.measureParams.Count); i++)
            {
                string name, unitname;
                byte format;
                ushort data;
                name = EventStructFormat.measureParams[i].paramName;
                unitname = " " + EventStructFormat.measureParams[i].paramUnitName;
                format = EventStructFormat.measureParams[i].paramFormat;
                data = eventLog[eventNum, EventStructFormat.measureParams[i].paramAddr];
                str = AdvanceConvert.HexToFormat(data, format);

                excelcells = excelworksheet.get_Range("A" + (excelIndex + i + 1).ToString(), "A" + (excelIndex + i + 1).ToString().ToString());
                excelcells.Value = name;
                excelcells = excelworksheet.get_Range("b" + (excelIndex + i + 1).ToString(), "b" + (excelIndex + i + 1).ToString().ToString());
                excelcells.Value = str;
                excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;
                excelcells = excelworksheet.get_Range("c" + (excelIndex + i + 1).ToString(), "c" + (excelIndex + i + 1).ToString().ToString());
                excelcells.Value = unitname;

                

            }
            maxLineCount = EventStructFormat.measureParams.Count;


            #endregion

            
            #region Digits
            for (i = 0; i < EventStructFormat.digitUnits.Count; i++)
            {
                string digitStr;

                ushort data;
                
                ushort useMask = 0xFFFF;
                data = eventLog[eventNum, EventStructFormat.digitUnits[i].EventPos];
                useMask = EventStructFormat.digitUnits[i].UseMask;

                if (EventStructFormat.digitUnits[i].PlateType == 0) { digitStr = "D"; lineCount = lineCount1; }
                else { digitStr = "E"; lineCount = lineCount2; }

                int i3 = 0;
                for (int i2 = 0; i2 < 16; i2++)
                {
                    str = EventStructFormat.digitUnits[i].DigitStrings[i3++];
                    if ((useMask & 0x0001) != 0)
                    {
                        excelcells =
                        excelworksheet.get_Range(digitStr + (excelIndex  + lineCount + 1).ToString(),
                                                 digitStr + (excelIndex  + lineCount + 1).ToString());
                        excelcells.Value = str;

                        if ((data & 0x0001) != 0) { excelcells.Interior.ColorIndex = 4; }
                        lineCount++;
                    }
                    useMask = (ushort)(useMask >> 1);
                    data = (ushort)(data >> 1);
                }

                if (EventStructFormat.digitUnits[i].PlateType == 0) { lineCount1 = lineCount; }
                else { lineCount2 = lineCount; }

            }
            #endregion
            if (lineCount1 > maxLineCount) { maxLineCount = lineCount1; }
            if (lineCount2 > maxLineCount) { maxLineCount = lineCount2; }

            #region Warnings

            List<string> paramsList = new List<string>();
            List<string> paramsList2 = new List<string>();
            paramsList.Clear();
            for (int i1 = 0; i1 < EventStructFormat.warningsList.Count; i1++)
            {
                ushort data = eventLog[eventNum, EventStructFormat.warningsList[i1].EventPos];
                paramsList2.Clear();
                paramsList2 = EventStructFormat.warningsList[i1].RelevantList(data);

                for (int i2 = 0; i2 < paramsList2.Count; i2++)
                {
                    paramsList.Add(paramsList2[i2]);
                }
            }

            str = "F";
            if (paramsList.Count != 0)
            {
                excelcells = excelworksheet.get_Range("F" + excelIndex.ToString(), "F" + excelIndex.ToString());
                excelcells.Merge(Type.Missing);
                excelcells.Value = StringsFromExMonApplForExcel.WarnsStr;
                excelcells.Font.Size = 12;
                excelcells.Font.Italic = false;
                excelcells.Font.Bold = true;

                lineCount = 0;
                for (int i2 = 0; i2 < paramsList.Count; i2++)
                {
                    excelcells =
                    excelworksheet.get_Range(str + (excelIndex + i2 + 1).ToString(),
                                             str + (excelIndex + i2 + 1).ToString());
                    excelcells.Value = paramsList[i2];
                    excelcells.Interior.ColorIndex = 6;
                }

                str = "G";
            }


            #endregion

            #region Alarms

            paramsList = new List<string>();
            paramsList2 = new List<string>();
            paramsList.Clear();
            for (int i1 = 0; i1 < EventStructFormat.alarmList.Count; i1++)
            {
                ushort data = eventLog[eventNum, EventStructFormat.alarmList[i1].EventPos];
                paramsList2.Clear();
                paramsList2 = EventStructFormat.alarmList[i1].RelevantList(data);

                for (int i2 = 0; i2 < paramsList2.Count; i2++)
                {
                    paramsList.Add(paramsList2[i2]);
                }
            }


            if (paramsList.Count != 0)
            {
                excelcells = excelworksheet.get_Range(str + excelIndex.ToString(), str + excelIndex.ToString());
                excelcells.Merge(Type.Missing);
                excelcells.Value = StringsFromExMonApplForExcel.AlarmsStr;
                excelcells.Font.Size = 12;
                excelcells.Font.Italic = false;
                excelcells.Font.Bold = true;

                lineCount = 0;
                for (int i2 = 0; i2 < paramsList.Count; i2++)
                {
                    excelcells =
                    excelworksheet.get_Range(str + (excelIndex + i2 + 1).ToString(),
                                             str + (excelIndex + i2 + 1).ToString());
                    excelcells.Value = paramsList[i2];
                    excelcells.Interior.ColorIndex = 3;
                }
            }


            #endregion

            #region TotalFrame

            excelIndex = excelIndex + maxLineCount + 1;
            excelcells = excelworksheet.get_Range("A" + excelIndex.ToString(), str + excelIndex.ToString());
            excelIndex++;
            excelcells.Merge(Type.Missing);

            excelcells = excelworksheet.get_Range("A" + (startIndex + 1).ToString(), str + (excelIndex - 2).ToString());
            excelcells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThick;
            excelcells.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThick;
            excelcells.Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThick;
            excelcells.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThick;

            str = (startIndex + 1).ToString() + ":" + (excelIndex - 2).ToString();
            excelcells = (Excel.Range)excelworksheet.Rows[str, Type.Missing];
            excelcells.Group(Type.Missing);
           

            #endregion

            excelFilePos = excelIndex;
            nowEventNum = eventNum+1;
            if (CreateFileStepFinished != null) CreateFileStepFinished(null, new EventArgs());

            excelcells = null;
            excelsheets = null;
            excelworksheet = null;
        }
       
        public static void CreateExcelEventFile(ushort[,] eventLog, string fileName)
        {   
            Excel.Application excelapp;
            Excel.Worksheet excelworksheet;
            Excel.Workbook excelappworkbook;

            excelapp = new Excel.Application();
            excelapp.Visible = false;
           // excelapp.Interactive = false;
            excelapp.SheetsInNewWorkbook = 1;
            excelapp.Workbooks.Add(Type.Missing);

            excelworksheet = (Excel.Worksheet)excelapp.Workbooks[1].Worksheets.get_Item(1);
            
            excelworksheet.Columns[1].ColumnWidth = 30;
            excelworksheet.Columns[2].ColumnWidth = 10;
            excelworksheet.Columns[3].ColumnWidth = 7;
            excelworksheet.Columns[4].ColumnWidth = 40;
            excelworksheet.Columns[5].ColumnWidth = 40;
            excelworksheet.Columns[6].ColumnWidth = 40;
            excelworksheet.Columns[7].ColumnWidth = 40;

            excelappworkbook = excelapp.Workbooks[1];

            int i = 0;
            int excelIndex = 1;

       
            while (i < EventStructFormat.eventLogLength)
            {
                //eventLog[i, 0] = 0xAE01;
                WriteExcelLine(excelIndex, excelappworkbook, i, eventLog);
                excelIndex = excelFilePos;
                i++;
            }

            excelappworkbook = excelapp.Workbooks[1];
            try
            {
                excelappworkbook = excelapp.Workbooks[1];
                excelappworkbook.Saved = true;
                excelapp.DisplayAlerts = false;
                excelapp.DefaultSaveFormat = Excel.XlFileFormat.xlExcel7;
                excelappworkbook.SaveAs(fileName,  //object Filename
                   Excel.XlFileFormat.xlExcel7,                        //object FileFormat
                   Type.Missing,                       //object Password 
                   Type.Missing,                       //object WriteResPassword  
                   Type.Missing,                       //object ReadOnlyRecommended
                   Type.Missing,                       //object CreateBackup
                   Excel.XlSaveAsAccessMode.xlNoChange,//XlSaveAsAccessMode AccessMode
                   Type.Missing,                       //object ConflictResolution
                   Type.Missing,                       //object AddToMru 
                   Type.Missing,                       //object TextCodepage
                   Type.Missing,                       //object TextVisualLayout
                   Type.Missing);                      //object Local


                excelapp.Quit();
            }
            catch
            {
                excelapp.Quit();
            }

        /*    excelapp = null;
            excelappworkbook = null;
            excelworksheet = null;
            GC.Collect();

            Process[] procs;
            procs = Process.GetProcessesByName("EXCEL");
            foreach (Process proc in procs)
            {
                proc.Kill();
            }


            Process proc1;

            proc1 = new Process();
            proc1.StartInfo.FileName = @fileName;
            proc1.Start();*/
        }   
    }
}
