using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;

namespace ADSPLibrary
{
    public class MeasureParam
    {
        public string paramName = " ";
        public string paramShortName = " ";
        public string paramUnitName = " ";
        public ushort paramAddr = 0;
        public byte paramFormat = 0;
        public byte paramSubFormat = 0;
    }
    public static class EventStructFormat
    {
        public static string SystemName{ get; set; }

        static string configFileName = "SysType.xml";
        public static List<MeasureParam> measureParams = new List<MeasureParam>();
        public static List<DigitUnit> digitUnits = new List<DigitUnit>();

        public static List<WordUnit> warningsList = new List<WordUnit>();
        public static List<WordUnit> alarmList = new List<WordUnit>();
        public static List<WordUnit> readyList = new List<WordUnit>();
        
        public static Hashtable eventTypesHash = new Hashtable();
        public static ushort loadEventAddr = 0x3FFF;
        public static ushort startEventLogAddr = 0x3FFF;
        public static ushort eventLogCountBlocks = 0;
        public static int eventLogLength = 0;

        public static int MeasureLabelWidth = 200;
        public static int DigitLabelWidth = 200;
        public static int DigitLabelWidth2 = 90;

        public static ushort loadMeasureAddr = 0x200;

        public static ushort sysParamStartAddr = 0x0520;
        public static ushort sysParamBlockCount = 16;

        public static bool EnaScope        { get; set; }
        public static bool EnaWordDocument { get; set; }
        public static bool EnaDirectForm    { get; set; }
        public static bool EnaEventLog      { get; set; }
        public static bool EnaJogForm       { get; set; }
        public static bool EnaMeasureForm   { get; set; }
        public static int  NumMeasureForm   { get; set; }
        public static bool EnaParamsForm    { get; set; }
        public static int  NumParamsForm    { get; set; }
        public static bool DigitMasks       { get; set; }


        private static void LoadWordUnits(int listNum)
        {
            string blockName = "";
            List<WordUnit> unitsList;

            XmlNodeList xmls;
            XmlNode xmlline;
            int wordCount = 0;
            int i;

            switch (listNum)
            {
                case 1: { blockName = "WarningStrings"; unitsList = warningsList;} break;
                case 2: { blockName = "AlarmStrings"; unitsList = alarmList; } break;
                default: { blockName = "ReadyStrings"; unitsList = readyList; } break;
            }

            unitsList.Clear();

            var doc = new XmlDocument();
            doc.Load(configFileName);
            xmls = doc.GetElementsByTagName(blockName);
           
            if (xmls.Count != 1)
            {
                throw new Exception("Ошибки в файле конфигурации!");
            }
            
            xmlline = xmls[0];
            wordCount = Convert.ToInt32(xmlline.Attributes["CountWord"].Value);

            for (i = 0; i < wordCount; i++)
            {
                unitsList.Add(new WordUnit(configFileName, blockName, i));
            }
        }

        public static void LoadMeasureParams()
        {
            XmlNodeList xmls;
            XmlNode xmlNode;
            int paramCount = 0;
            int i;
            MeasureParam measureParam;
            measureParams.Clear();

            var doc = new XmlDocument();
            doc.Load(configFileName);

            xmls = doc.GetElementsByTagName("MeasureParams");
            if (xmls.Count != 1)
            {
                throw new Exception("Ошибки в файле конфигурации!");
            }

            xmlNode = xmls[0];
            paramCount = Convert.ToInt32(xmlNode.Attributes["Count"].Value);
            MeasureLabelWidth = Convert.ToInt32(xmlNode.Attributes["MaxWidth"].Value);

            for (i = 0; i < paramCount; i++)
            {
                xmls = doc.GetElementsByTagName("MeasureParam" + (i + 1).ToString());
                xmlNode = xmls[0];
                measureParam = new MeasureParam();
                measureParam.paramName = Convert.ToString(xmlNode.Attributes["Name"].Value);
                measureParam.paramShortName = Convert.ToString(xmlNode.Attributes["ShortName"].Value);
                measureParam.paramUnitName = Convert.ToString(xmlNode.Attributes["UnitName"].Value);
                measureParam.paramAddr = Convert.ToUInt16(xmlNode.Attributes["Addr"].Value);
                measureParam.paramFormat = Convert.ToByte(xmlNode.Attributes["Format"].Value);
                measureParam.paramSubFormat = Convert.ToByte(xmlNode.Attributes["SubFormat"].Value);
                measureParams.Add(measureParam);
            }
        }

        public static void LoadEventTypes()
        {
            XmlNodeList xmls;
            XmlNode xmlNode;
            int paramCount = 0;
            int i;
            eventTypesHash.Clear();
 
            var doc = new XmlDocument();
            doc.Load(configFileName);

            
            xmls = doc.GetElementsByTagName("EventLogSetup");
            if (xmls.Count != 1)
            {
                throw new Exception("Ошибки в файле конфигурации!");
            }
         
            xmlNode = xmls[0];
            paramCount = Convert.ToInt32(xmlNode.Attributes["Count"].Value);
            loadEventAddr = Convert.ToUInt16(xmlNode.Attributes["Addr1"].Value);
            loadMeasureAddr = Convert.ToUInt16(xmlNode.Attributes["Addr3"].Value);
            startEventLogAddr = Convert.ToUInt16(xmlNode.Attributes["Addr2"].Value);
            eventLogCountBlocks = Convert.ToUInt16(xmlNode.Attributes["BlockCount"].Value);
            eventLogLength = Convert.ToInt32(xmlNode.Attributes["LogLength"].Value);



            for (i = 0; i < paramCount; i++)
            {
               xmls = doc.GetElementsByTagName("EventType" + (i + 1).ToString());
               if (xmls.Count != 1)
               {
                   throw new Exception("Ошибки в файле конфигурации!");
               }
               xmlNode = xmls[0];
               eventTypesHash.Add(Convert.ToString(xmlNode.Attributes["Code"].Value), Convert.ToString(xmlNode.Attributes["Name"].Value));
            }

        }
       
        static void LoadDigitUnits()
        {
            XmlNodeList xmls;
            XmlNode xmlNode;
            int paramCount = 0;
            int i;

            var doc = new XmlDocument();
            doc.Load(configFileName);



            xmls = doc.GetElementsByTagName("Digits");
            if (xmls.Count != 1)
            {
                throw new Exception("Ошибки в файле конфигурации!");
            }

            xmlNode = xmls[0];
            paramCount = Convert.ToInt32(xmlNode.Attributes["Count"].Value);
            DigitLabelWidth = Convert.ToInt32(xmlNode.Attributes["MaxWidth"].Value);
            DigitLabelWidth2 = Convert.ToInt32(xmlNode.Attributes["MaxWidth2"].Value);


            for (i = 0; i < paramCount; i++)
            {
                digitUnits.Add(new DigitUnit(configFileName,i+1));
            }

        }

        static void LoadSystemParams()
        {
            XmlNodeList xmls;
            XmlNode xmlNode;

            var doc = new XmlDocument();
            doc.Load(configFileName);



            xmls = doc.GetElementsByTagName("SysParamSetup");
            if (xmls.Count != 1)
            {
                throw new Exception("Ошибки в файле конфигурации!");
            }

            xmlNode = xmls[0];
            sysParamStartAddr = Convert.ToUInt16(xmlNode.Attributes["Addr"].Value);
            sysParamBlockCount = Convert.ToUInt16(xmlNode.Attributes["Count"].Value);

        }

        static void LoadMainMenusConfig()
        {
        }

        static void LoadSystemName()
        {
            XmlNodeList xmls;
            XmlNode xmlNode;

            var doc = new XmlDocument();
            doc.Load(configFileName);



            xmls = doc.GetElementsByTagName("SystemInfo");
            if (xmls.Count != 1)
            {
                SystemName = configFileName;
                return;
            }

            try
            {
                xmlNode = xmls[0];
                SystemName = Convert.ToString(xmlNode.Attributes["Name"].Value);
            }
            catch
            {
                SystemName = configFileName;
            }
        }

        public static void LoadButtons()
        {
            XmlNodeList xmls;
            XmlNode xmlNode;

            var doc = new XmlDocument();
            try
            {
                doc.Load(configFileName);
            }
            catch
            {
                EnaDirectForm = false;
                EnaEventLog = false;
                EnaJogForm = false;
                
                EnaMeasureForm = false;
                EnaDirectForm = false;
                EnaWordDocument = false;
                EnaScope = false;
                NumMeasureForm = 0;

                EnaParamsForm = false;
                NumParamsForm = 0;

                DigitMasks = false;
                return;
            }


            xmls = doc.GetElementsByTagName("DirectEna");
            if (xmls.Count == 1)
            {
                EnaDirectForm = true;
            }
            else
            {
                EnaDirectForm = false;
            }

            xmls = doc.GetElementsByTagName("EventLogEna");
            if (xmls.Count == 1)
            {
                EnaEventLog = true;
            }
            else
            {
                EnaEventLog = false;
            }

            xmls = doc.GetElementsByTagName("JogEna");
            if (xmls.Count == 1)
            {
                EnaJogForm = true;
            }
            else
            {
                EnaJogForm = false;
            }

            xmls = doc.GetElementsByTagName("MeasureEna");
            if (xmls.Count == 1)
            {
                EnaMeasureForm = true;

                try
                {
                    xmlNode = xmls[0];
                    NumMeasureForm = Convert.ToInt32(xmlNode.Attributes["Number"].Value);
                }
                catch
                {
                    EnaMeasureForm = false;
                    NumMeasureForm = 0;
                }

            }
            else
            {
                EnaMeasureForm = false;
                NumMeasureForm = 0;
            }


            xmls = doc.GetElementsByTagName("SystemParamEna");
            if (xmls.Count == 1)
            {
                EnaParamsForm = true;

                try
                {
                    xmlNode = xmls[0];
                    NumParamsForm = Convert.ToInt32(xmlNode.Attributes["Number"].Value);
                }
                catch
                {
                    EnaParamsForm = false;
                    NumParamsForm = 0;
                }

            }
            else
            {
                EnaParamsForm = false;
                NumParamsForm = 0;
            }

            xmls = doc.GetElementsByTagName("DigitMasksEna");
            if (xmls.Count == 1)
            {
                DigitMasks = true;
            }
            else
            {
                DigitMasks = false;
            }


            xmls = doc.GetElementsByTagName("WordDocEna");
            if (xmls.Count == 1)
            {
                EnaWordDocument = true;
            }
            else
            {
                EnaWordDocument = false;
            }

            xmls = doc.GetElementsByTagName("ScopeEna");
            if (xmls.Count == 1)
            {
                EnaScope = true;
            }
            else
            {
                EnaScope = false;
            }
        }

        public static void LoadEventStruct(string newConfigFileName)
        {
            measureParams.Clear();
            digitUnits.Clear();
            warningsList.Clear();
            alarmList.Clear();
            readyList.Clear();
            eventTypesHash.Clear();

            configFileName = newConfigFileName;
            LoadEventTypes();
            LoadMeasureParams();
            LoadDigitUnits();
            LoadWordUnits(0);
            LoadWordUnits(1);
            LoadWordUnits(2);
            LoadSystemParams();
            LoadMainMenusConfig();
            LoadSystemName();
            LoadButtons();
        }

        
        

    }
}
