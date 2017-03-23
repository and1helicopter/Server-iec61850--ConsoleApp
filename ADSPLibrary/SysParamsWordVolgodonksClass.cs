using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Word = Microsoft.Office.Interop.Word;


namespace ADSPLibrary
{
    public class SysParamsWordVolgodonksClass
    {
        private string applPath = "";
        
        private Word.Application wordapp;
     //   private Word.Documents worddocuments;
        private Word.Document worddocument;

        ushort[] SysParamsData = new ushort[288];


        public SysParamsWordVolgodonksClass(string newApplPath)
        {
            applPath = newApplPath;
        }

        private void OpenHexFile(string hexFileName)
        {
            int i1,index;
            string str;
            List<ushort> paramLine = new List<ushort>();

            try
            {
                StreamReader sr = new StreamReader(hexFileName);
                index = 0; 
                for (i1 = 0; i1 < 8*7; i1++)
                {
                    str = sr.ReadLine();
                    paramLine = AdvanceConvert.LineToUshorts(str);
                    if (paramLine.Count != 4)
                    {
                        throw new System.Exception();
                    }
                    for (int i2 = 0; i2 < 4; i2++)
                    {
                        SysParamsData[index++] = paramLine[i2];
                    }
                }
            }
            catch
            {
                throw new System.Exception();
            }
        }

        public void CreateWordDocument(string newWordDocFileName, string hexFileName)
        {
            try
            {
                OpenHexFile(hexFileName);
            }
            catch
            {
                throw new System.Exception("Ошибка при открытии HEX-файла!");
            }

            try
            {
                wordapp = new Word.Application();
                Object template = Type.Missing;
                Object newTemplate = false;
                Object documentType = Word.WdNewDocumentType.wdNewBlankDocument;
                Object visible = false;

                template = applPath + "Волгодонск.doc";
                worddocument =
                wordapp.Documents.Add(
                 ref template, ref newTemplate, ref documentType, ref visible);
            }
            catch
            {
                wordapp.Quit();
                wordapp = null;
                throw new System.Exception("Ошибка при запуске MS Word!");
            }
            finally { }

            #region WriteParamsToWordFile
            ushort param, param2 = 0, param3 = 0;

            //Уставка по умолчанию в РРВ
            param = SysParamsData[0x0595 - 0x0520];
            UpdateTextWord("{param1}",AdvanceConvert.HexToPercent(param)+ " %", worddocument);

            //Время НВ
            param = SysParamsData[0x05BB - 0x0520];
            UpdateTextWord("{param2}", AdvanceConvert.HexToUint1000(param) + " сек", worddocument);
               
            //Уставка включения форсировки
            param = SysParamsData[0x05CB - 0x0520];
            UpdateTextWord("{param3}", AdvanceConvert.HexToPercent(param,1) + " %", worddocument);

            //Уставка отключения форсировки
            param = SysParamsData[0x05CC - 0x0520];
            UpdateTextWord("{param4}", AdvanceConvert.HexToPercent(param,1) + " %", worddocument);

            //Задержка на отключение
            param = SysParamsData[0x05CA - 0x0520];
            UpdateTextWord("{param5}", AdvanceConvert.HexToUint((ushort)(param-1)) + " мс", worddocument);

            //VHZ-ограничение
            param = SysParamsData[0x05D1 - 0x0520];
            param2 = SysParamsData[0x05D2 - 0x0520];
            UpdateTextWord("{param6}", AdvanceConvert.LongHexToVHz(param,param2) + " Гц", worddocument);

            //Контроль изоляции
            param = SysParamsData[0x05AF - 0x0520];
            if (param != 0) { UpdateTextWord("{param7}", "Включено", worddocument); }
            else
            {
                UpdateTextWord("{param7}", "Отключено", worddocument); 
            }

            //Инвертирование токов
            param = SysParamsData[0x055C - 0x0520];
            if (param != 0) { UpdateTextWord("{param8}", "Не инверт.", worddocument); }
            else
            {
                UpdateTextWord("{param8}", "Инверт.", worddocument); 
            }

            //Регулятор If
            param  = SysParamsData[0x0589 - 0x0520];
            param2 = SysParamsData[0x058B - 0x0520];
            param3 = SysParamsData[0x058A - 0x0520];

            UpdateTextWord("{param9}", AdvanceConvert.HexToTi(param, true), worddocument);
            UpdateTextWord("{param10}", AdvanceConvert.HexToUint(param2) + " мс", worddocument);
            UpdateTextWord("{param11}", AdvanceConvert.HexTo8_8(param3,0) + " ", worddocument);

            //Регулятор U
            param = SysParamsData[0x0580 - 0x0520];
            param2 = SysParamsData[0x0582 - 0x0520];
            param3 = SysParamsData[0x0581 - 0x0520];

            UpdateTextWord("{param12}", AdvanceConvert.HexToTi(param, true), worddocument);
            UpdateTextWord("{param13}", AdvanceConvert.HexToUint(param2) + " мс", worddocument);
            UpdateTextWord("{param14}", AdvanceConvert.HexTo8_8(param3, 0) +  " ", worddocument);

            //ток форсировки
            param = SysParamsData[0x0576 - 0x0520];
            UpdateTextWord("{param15}", AdvanceConvert.HexToPercent(param,0) + " %", worddocument);

            //время форсировки
            param = SysParamsData[0x0573 - 0x0520];
            UpdateTextWord("{param16}", AdvanceConvert.HexToOverHeatTi(param) + " сек", worddocument);       
     
            //перегрузка по току
            param = SysParamsData[0x0570 - 0x0520];
            UpdateTextWord("{param17}", AdvanceConvert.HexToPercent(param, 0) + " %", worddocument);

            //Регулятор Перегрева
            param = SysParamsData[0x0577 - 0x0520];
            param2 = SysParamsData[0x0579 - 0x0520];
            param3 = SysParamsData[0x0578 - 0x0520];

            UpdateTextWord("{param18}", AdvanceConvert.HexToTi(param, true), worddocument);
            UpdateTextWord("{param19}", AdvanceConvert.HexToUint(param2) + " мс", worddocument);
            UpdateTextWord("{param20}", AdvanceConvert.HexTo8_8(param3, 0) + " ", worddocument);

            //Номинальн. UStat
            param = SysParamsData[0x0540 - 0x0520];
            UpdateTextWord("{param21}", AdvanceConvert.HexToInt(param) + " В", worddocument);

            //Номинальн. IStat
            param = SysParamsData[0x0541 - 0x0520];
            UpdateTextWord("{param22}", AdvanceConvert.HexToInt(param) + " A", worddocument);

            //Номинальн. Uf
            param = SysParamsData[0x0542 - 0x0520];
            UpdateTextWord("{param23}", AdvanceConvert.HexToInt10(param) + " В", worddocument);

            //Номинальн. Ktt
            param = SysParamsData[0x0544 - 0x0520];
            UpdateTextWord("{param24}", AdvanceConvert.HexToTT(param) + " А", worddocument);

            //Номинальн. If
            param = SysParamsData[0x0545 - 0x0520];
            param2 = SysParamsData[0x0546 - 0x0520];
            UpdateTextWord("{param25}", AdvanceConvert.CalcNomIf(param,param2) + " А", worddocument);

            //МТЗ трансформатора
            param = SysParamsData[0x05F8 - 0x0520];
            param2 = SysParamsData[0x05F9 - 0x0520];
            UpdateTextWord("{param26}", AdvanceConvert.HexToTransAlarm(param, true) + " А", worddocument);
            UpdateTextWord("{param27}", AdvanceConvert.HexToInt8(param2) + " мс", worddocument);

            //МТЗ VD
            param = SysParamsData[0x05A2 - 0x0520];
            param2 = SysParamsData[0x05B6 - 0x0520];
            UpdateTextWord("{param28}", AdvanceConvert.HexToPercent(param, 1) + " %", worddocument);
            UpdateTextWord("{param29}", AdvanceConvert.HexToInt8(param2) + " мс", worddocument);

            //Перенапряжение статора
            param = SysParamsData[0x05A0 - 0x0520];
            param2 = SysParamsData[0x05A1 - 0x0520];
            UpdateTextWord("{param30}", AdvanceConvert.HexToPercent(param, 1) + " %", worddocument);
            UpdateTextWord("{param31}", AdvanceConvert.HexToInt(param2) + " мс", worddocument);

            //Loss excit
            param = SysParamsData[0x05a8 - 0x0520];
            UpdateTextWord("{param32}", AdvanceConvert.HexToUint1000(param) + " сек", worddocument);

            //Сверхток1
            param = SysParamsData[0x05a5 - 0x0520];
            UpdateTextWord("{param33}", AdvanceConvert.HexToUint1000(param) + " сек", worddocument);

            //Асинхронный ход 
            param = SysParamsData[0x0591 - 0x0520];
            param2 = SysParamsData[0x0590 - 0x0520];
            UpdateTextWord("{param34}", AdvanceConvert.HexToUint(param) + "", worddocument);
            UpdateTextWord("{param35}", AdvanceConvert.HexToUint1000(param2) + " сек", worddocument);

            //Изоляция
            param = SysParamsData[0x05a9 - 0x0520];
            param2 = SysParamsData[0x05DD - 0x0520];
            UpdateTextWord("{param36}", AdvanceConvert.HexToUint(param) + "", worddocument);
            UpdateTextWord("{param37}", AdvanceConvert.HexToUint1000(param2) + " сек", worddocument);

            #endregion

            #region SaveWordFile
            Object fileName = newWordDocFileName;
            Object fileFormat = Word.WdSaveFormat.wdFormatDocument;
            Object lockComments = false;
            Object password = "";
            Object addToRecentFiles = false;
            Object writePassword = "";
            Object readOnlyRecommended = false;
            Object embedTrueTypeFonts = false;
            Object saveNativePictureFormat = false;
            Object saveFormsData = false;
            Object saveAsAOCELetter = Type.Missing;
            Object encoding = Type.Missing;
            Object insertLineBreaks = Type.Missing;
            Object allowSubstitutions = Type.Missing;
            Object lineEnding = Type.Missing;
            Object addBiDiMarks = Type.Missing;

            try
            {
                worddocument.SaveAs(ref fileName,
                ref fileFormat, ref lockComments,
                ref password, ref addToRecentFiles, ref writePassword,
                ref readOnlyRecommended, ref embedTrueTypeFonts,
                ref saveNativePictureFormat, ref saveFormsData,
                ref saveAsAOCELetter, ref encoding, ref insertLineBreaks,
                ref allowSubstitutions, ref lineEnding, ref addBiDiMarks);
            }
            catch
            {
                throw new System.Exception("Ошибка при сохранение документа MS Word!");
            }
            finally { wordapp.Quit(); }
            #endregion


        }

        private void UpdateTextWord(string oldText, string newText, Word.Document worddocumenttmp)
        {
            Word.Range wordrange = worddocumenttmp.Content;
            wordrange.Find.ClearFormatting();
            wordrange.Find.Execute(FindText: oldText, ReplaceWith: newText);
        }



    }
}
