using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ADSPLibrary
{
    public static class TextEventFile
    {

        static private string CalcMeasureParam(int paramNum, ushort paramValue)
        {
            string name, unitname, str;
            byte format;
            name = EventStructFormat.measureParams[paramNum].paramName + "=";
            unitname = " " + EventStructFormat.measureParams[paramNum].paramUnitName + " ";
            format = EventStructFormat.measureParams[paramNum].paramFormat;
            str = AdvanceConvert.HexToFormat(paramValue, format);
            return (name + str + unitname);
        }

        static public bool SaveEventLogToTextFile(ushort[,] eventLog, string filename)
        {
            int i;
            string eventCode;
            string eventType = StringsFromExMonApplForExcel.EmptyEventStr;
            string str;
            ushort data = 0;
            ushort useMask = 0xFFFF;
            StreamWriter sw;
            try
            { sw = File.CreateText(filename); }
            catch
            { return false; }

            for (i = 0; i < EventStructFormat.eventLogLength; i++)
            {
                sw.WriteLine("//------------------------------------------------------------------------------------------------//");
                sw.WriteLine(StringsFromExMonApplForExcel.EventStr+" N" + (i + 1).ToString());
                sw.WriteLine("//------------------------------------------------------------------------------------------------//");
                eventCode = "0x" + eventLog[i, 0].ToString("X4");
                eventType = StringsFromExMonApplForExcel.EmptyEventStr;

                foreach (string key in EventStructFormat.eventTypesHash.Keys)
                {
                    if (key == eventCode) { eventType = (string)EventStructFormat.eventTypesHash[key]; }
                }

                if (eventType == StringsFromExMonApplForExcel.EmptyEventStr)
                {
                    sw.WriteLine(StringsFromExMonApplForExcel.EventStr+": " + eventType);
                   // sw.WriteLine("___________________________________________________________________________________");
                    continue;
                }

                sw.WriteLine(StringsFromExMonApplForExcel.EventStr+": " + eventType);
                sw.WriteLine("Time: " + (eventLog[i, 6] & 0x3F).ToString("X2") + "/" + (eventLog[i, 7] & 0x1F).ToString("X2") + "/" + (eventLog[i, 8] & 0xFF).ToString("X2"));
                sw.WriteLine("Date: " + (eventLog[i, 4] & 0x3F).ToString("X2") + ":" + (eventLog[i, 3] & 0x7F).ToString("X2") + ":" + (eventLog[i, 2] & 0x7F).ToString("X2"));

                if (EventStructFormat.measureParams.Count != 0)
                {
                    sw.WriteLine("//--------------"+StringsFromExMonApplForExcel.MeasuresStrs +"-----------//");

                }


                for (int i1 = 0; i1 < EventStructFormat.measureParams.Count; i1++)
                {
                    data = eventLog[i, EventStructFormat.measureParams[i1].paramAddr];
                    str = CalcMeasureParam(i1, data);
                    sw.WriteLine(str);
                }


                if (EventStructFormat.digitUnits.Count != 0)
                {
                    sw.WriteLine("//--------------"+StringsFromExMonApplForExcel.DigInputsStr +"-----------//");

                }
                for (int i1 = 0; i1 < EventStructFormat.digitUnits.Count; i1++)
                {
                    data = eventLog[i, EventStructFormat.digitUnits[i1].EventPos];
                    useMask = EventStructFormat.digitUnits[i1].UseMask;
                    str = EventStructFormat.digitUnits[i1].DigitTitle;
                    sw.WriteLine("");
                    sw.WriteLine(str);
                    int i3 = 0;
                    for (int i2 = 0; i2 < 16; i2++)
                    {
                        str = EventStructFormat.digitUnits[i1].DigitStrings[i3++];
                        if ((data & 0x0001) != 0) { str = str + "=1\t\t\t\t"; }
                        else { str = str + "=0\t\t\t\t"; }
                        data = (ushort)(data >> 1);

                        if ((useMask & 0x0001) != 0) { sw.WriteLine(str); }
                        useMask = (ushort)(useMask >> 1);
                    }
                }

                #region CalcWarns
                List<string> paramsList = new List<string>();
                List<string> paramsList2 = new List<string>();
                paramsList.Clear();
                for (int i1 = 0; i1 < EventStructFormat.warningsList.Count; i1++)
                {
                    data = data = eventLog[i, EventStructFormat.warningsList[i1].EventPos];
                    paramsList2.Clear();
                    paramsList2 = EventStructFormat.warningsList[i1].RelevantList(data);

                    for (int i2 = 0; i2 < paramsList2.Count; i2++)
                    {
                        paramsList.Add(paramsList2[i2]);
                    }
                }

                if (paramsList.Count != 0)
                {
                    sw.WriteLine("");
                    sw.WriteLine("//--------------"+StringsFromExMonApplForExcel.WarnsStr +"-----------//");
                    for (int i1=0; i1<paramsList.Count; i1++)
                    {
                        sw.WriteLine(paramsList[i1]);
                    }

                }
                #endregion

                #region CalcAlarms
                paramsList.Clear();
                for (int i1 = 0; i1 < EventStructFormat.alarmList.Count; i1++)
                {
                    data = eventLog[i, EventStructFormat.alarmList[i1].EventPos];
                    paramsList2.Clear();
                    paramsList2 = EventStructFormat.alarmList[i1].RelevantList(data);

                    for (int i2 = 0; i2 < paramsList2.Count; i2++)
                    {
                        paramsList.Add(paramsList2[i2]);
                    }
                }

                if (paramsList.Count != 0)
                {
                    sw.WriteLine("");
                    sw.WriteLine("//--------------"+StringsFromExMonApplForExcel.AlarmsStr +"-----------//");
                    for (int i1 = 0; i1 < paramsList.Count; i1++)
                    {
                        sw.WriteLine(paramsList[i1]);
                    }

                }

                #endregion

               // sw.WriteLine("___________________________________________________________________________________");
            }

            sw.Close();



            return true;

        }


    }
}
