using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Server.Parser
{
    public partial class Parser
    {
        public bool ParseFile()
        {
            string filePath = @"test.icd";
            //читаем данные из файла
            XDocument doc = XDocument.Load(filePath);
            
            if (doc.Root != null)
            {
                if (!ParseDocument(doc))    //Парсим основные классы модели
                {
                    Logging.Log.Write("ParseDocunent: Finish whith status false", "Error   ");
                    return false;
                }

                if (!ModelFillDefultParam())    //Заполнение  модели параметрами по-умолчанию
                {
                    Logging.Log.Write("ModelFillDefultParam: Finish whith status false", "Error   ");
                    return false;
                }

                if (!FileParseToAttribute(doc)) //Заполняем объектную модель инициализированными параметрами 
                {
                    Logging.Log.Write("FileParseToAttribute: Finish whith status false", "Warning ");
                }

                CreateClasses();            //Создаем обновляймые классы 

                SaveFileConfig();           //Создаем из объектной модели - конфигурационную 
            }

            return true;
        }




        #region Создание обновляймых классов 
        private void CreateClasses()
        {
            foreach (var itemLd in ServerModel.Model.ListLD)
            {
                string pathNameLD = itemLd.NameLD;

                foreach (var itemLn in itemLd.ListLN)
                {
                    string pathNameLN = itemLn.NameLN;
                    //Если переменную класса нужно читать из памяти
                    var getDo = (from x in itemLn.ListDO
                                 where x.Type == "G"
                                 select x).ToList();

                    if (getDo.Count != 0)
                    {
                        GetDo(getDo, pathNameLD + "/" + pathNameLN);
                        continue;
                    }

                    //Если переменную класса нужно записать в память
                    var setDo = (from x in itemLn.ListDO
                                 where x.Type == "S"
                                 select x).ToList();

                    if (setDo.Count != 0)
                    {

                    }
                }
            }
        }



        private void GetDo(List<ServerModel.NodeDO> getDo, string path)
        {
            foreach (var itemDo in getDo)
            {
                //Проверка MV класса
                if (itemDo.TypeDO == "MV")
                {
                    string pathNameDo = path + "." + itemDo.NameDO;
                    var mv = new MvClass();

                    var siUnit = Convert.ToInt32((from y in (from x in itemDo.ListDA
                                                             where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
                                                             select x).ToList().Last().ListDA.ToList()
                                                  where y.NameDA.ToUpper() == "SIUnit".ToUpper()
                                                  select y).ToList().Last().Value);

                    var multiplier = Convert.ToInt32((from y in (from x in itemDo.ListDA
                                                                 where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
                                                                 select x).ToList().Last().ListDA.ToList()
                                                      where y.NameDA.ToUpper() == "Multiplier".ToUpper()
                                                      select y).ToList().Last().Value);

                    var scaleFactor = Convert.ToSingle((from y in (from x in itemDo.ListDA
                                                                  where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
                                                                  select x).ToList().Last().ListDA.ToList()
                                                       where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
                                                       select y).ToList().Last().Value.Replace('.',','));

                    var offset = Convert.ToSingle((from y in (from x in itemDo.ListDA
                                                             where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
                                                             select x).ToList().Last().ListDA.ToList()
                                                  where y.NameDA.ToUpper() == "Offset".ToUpper()
                                                  select y).ToList().Last().Value.Replace('.', ','));

                    if ((from y in (from x in itemDo.ListDA
                                    where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
                                    select x).ToList().Last().ListDA.ToList()
                         where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
                         select y).ToList().Last().Value == null)
                    {
                        scaleFactor = 1;
                    }

                    mv.ClassFill(siUnit, multiplier, scaleFactor, offset, itemDo.DescDO);

                    StructUpdateDataObj.DataObject dataObj = new StructUpdateDataObj.DataObject(pathNameDo, itemDo.Format,itemDo.Mask,itemDo.Addr,itemDo.TypeDO,mv);
                    StructUpdateDataObj.DataClassGet.Add(dataObj);
                    continue;
                }

                if (itemDo.TypeDO == "SPS")
                {
                    string pathNameDo = path + "." + itemDo.NameDO;
                    var sps = new SpsClass();

                    StructUpdateDataObj.DataObject dataObj = new StructUpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.TypeDO, sps);
                    StructUpdateDataObj.DataClassGet.Add(dataObj);
                    continue;
                }
            }
        }

        private void DefualtDo(List<ServerModel.NodeDO> defualtDo, string path)
        {
            foreach (var itemDo in defualtDo)
            {
                if (itemDo.NameDO == "Mod")
                {

                    
                    //DataObj.AddStructDefultDataObj();
                }
            }
        }
        #endregion

        #region Сохранение объектной модели в конфигурациионную модель для сервера
        private void SaveFileConfig()
        {
            string savePath = "test.cfg";
            FileStream fs = new FileStream(savePath, FileMode.Create);

            string str = $"MODEL({ServerModel.Model.NameModel}){{\n";
            var array = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(array, 0, array.Length);

            SaveLd(fs, ServerModel.Model.ListLD);

            str = "}\n";
            array = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(array, 0, array.Length);

            fs.Close();
        }

        private void SaveLd(FileStream fs, List<ServerModel.NodeLD> listLd)
        {
            foreach (var ld in listLd)
            {
                // Syntax: LD(<logical device name>){…}
                string str = $"LD({ld.NameLD}){{\n";
                var array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);

                SaveLn(fs, ld.ListLN);

                str = "}\n";
                array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);
            }
        }

        private void SaveLn(FileStream fs, List<ServerModel.NodeLN> listLn)
        {
            foreach (var ln in listLn)
            {
                // Syntax: LN(<logical node name>){…}
                string str = $"LN({ln.NameLN}){{\n";
                var array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);

                SaveDo(fs, ln.ListDO);

                str = "}\n";
                array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);
            }
        }

        private void SaveDo(FileStream fs, List<ServerModel.NodeDO> listDo)
        {
            foreach (var DO in listDo)
            {
                // Syntax: DO(<data object name> <nb of array elements>){…}
                string str = $"DO({DO.NameDO} {0}){{\n";
                var array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);

                SaveDa(fs, DO.ListDA);

                str = "}\n";
                array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);
            }
        }

        private void SaveDa(FileStream fs, List<ServerModel.NodeDA> listDa)
        {
            // DA(<data attribute name> <nb of array elements> <type> <FC> <trigger options> <sAddr>)[=value];
            // Constructed>
            // DA(<data attribute name> <nb of array elements> 27 <FC> <trigger options> <sAddr>){…}
            foreach (var da in listDa)
            {
                string str;
                byte[] array;
                if (da.ListDA.Count == 0)
                {
                    str = $"DA({da.NameDA} {0} {MapLibiecType(da.BTypeDA)} {MapLibiecFc(da.FCDA)} {0} {0})";
                    if (da.NameDA == "ctlModel")
                    {
                        str += " value = 0\n";
                    }
                    else
                    {
                        str += "\n";
                    }
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);
                }
                else
                {
                    str = $"DA({da.NameDA} {0} {MapLibiecType(da.BTypeDA)} {MapLibiecFc(da.FCDA)} {0} {0}){{\n";
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);

                    SaveDa(fs, da.ListDA);

                    str = "}\n";
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);
                }
            }
        }

        public int MapLibiecType(string dataType)
        {
            int type = 0;
            switch (dataType.ToUpper())
            {
                case "BOOLEAN":
                    type = (int)LibIecDataAttributeType.BOOLEAN;
                    break;
                case "INT8":
                    type = (int)LibIecDataAttributeType.INT8;
                    break;
                case "INT16":
                    type = (int)LibIecDataAttributeType.INT16;
                    break;
                case "INT32":
                    type = (int)LibIecDataAttributeType.INT32;
                    break;
                case "ENUM":
                    type = (int)LibIecDataAttributeType.INT32;
                    break;
                case "INT64":
                    type = (int)LibIecDataAttributeType.INT64;
                    break;
                case "INT128":
                    type = (int)LibIecDataAttributeType.INT128;
                    break;
                case "INT8U":
                    type = (int)LibIecDataAttributeType.INT8U;
                    break;
                case "INT16U":
                    type = (int)LibIecDataAttributeType.INT16U;
                    break;
                case "INT24U":
                    type = (int)LibIecDataAttributeType.INT24U;
                    break;
                case "INT32U":
                    type = (int)LibIecDataAttributeType.INT32U;
                    break;
                case "FLOAT32":
                    type = (int)LibIecDataAttributeType.FLOAT32;
                    break;
                case "FLOAT64":
                    type = (int)LibIecDataAttributeType.FLOAT64;
                    break;
                case "ENUMERATED":
                    type = (int)LibIecDataAttributeType.ENUMERATED;
                    break;
                case "OCTET_STRING_64":
                    type = (int)LibIecDataAttributeType.OCTET_STRING_64;
                    break;
                case "OCTET64":
                    type = (int)LibIecDataAttributeType.OCTET_STRING_64;
                    break;
                case "OCTET_STRING_6":
                    type = (int)LibIecDataAttributeType.OCTET_STRING_6;
                    break;
                case "OCTET6":
                    type = (int)LibIecDataAttributeType.OCTET_STRING_6;
                    break;
                case "OCTET_STRING_8":
                    type = (int)LibIecDataAttributeType.OCTET_STRING_8;
                    break;
                case "OCTET8":
                    type = (int)LibIecDataAttributeType.OCTET_STRING_8;
                    break;
                case "VISIBLE_STRING_32":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_32;
                    break;
                case "VISSTRING32":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_32;
                    break;
                case "VISIBLE_STRING_64":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_64;
                    break;
                case "VISSTRING64":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_64;
                    break;
                case "VISIBLE_STRING_65":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_65;
                    break;
                case "VISSTRING65":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_65;
                    break;
                case "VISIBLE_STRING_129":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_129;
                    break;
                case "VISSTRING129":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_129;
                    break;
                case "VISIBLE_STRING_255":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_255;
                    break;
                case "VISSTRING255":
                    type = (int)LibIecDataAttributeType.VISIBLE_STRING_255;
                    break;
                case "UNICODE_STRING_255":
                    type = (int)LibIecDataAttributeType.UNICODE_STRING_255;
                    break;
                case "UNICODE255":
                    type = (int)LibIecDataAttributeType.UNICODE_STRING_255;
                    break;
                case "TIMESTAMP":
                    type = (int)LibIecDataAttributeType.TIMESTAMP;
                    break;
                case "QUALITY":
                    type = (int)LibIecDataAttributeType.QUALITY;
                    break;
                case "CHECK":
                    type = (int)LibIecDataAttributeType.CHECK;
                    break;
                case "CODEDENUM":
                    type = (int)LibIecDataAttributeType.CODEDENUM;
                    break;
                case "GENERIC_BITSTRING":
                    type = (int)LibIecDataAttributeType.GENERIC_BITSTRING;
                    break;
                case "CONSTRUCTED":
                    type = (int)LibIecDataAttributeType.CONSTRUCTED;
                    break;
                case "STRUCT":
                    type = (int)LibIecDataAttributeType.CONSTRUCTED;
                    break;
                case "ENTRY_TIME":
                    type = (int)LibIecDataAttributeType.ENTRY_TIME;
                    break;
                case "PHYCOMADDR":
                    type = (int)LibIecDataAttributeType.PHYCOMADDR;
                    break;
            }
            return type;
        }

        int MapLibiecFc(string fc)
        {

            int fco;
            if (fc != null)
            {
                switch (fc.ToUpper())
                {
                    case "ST":
                        fco = (int)LibIecFunctionalConstraint.FC_ST;
                        break;
                    case "MX":
                        fco = (int)LibIecFunctionalConstraint.FC_MX;
                        break;
                    case "SP":
                        fco = (int)LibIecFunctionalConstraint.FC_SP;
                        break;
                    case "SV":
                        fco = (int)LibIecFunctionalConstraint.FC_SV;
                        break;
                    case "CF":
                        fco = (int)LibIecFunctionalConstraint.FC_CF;
                        break;
                    case "DC":
                        fco = (int)LibIecFunctionalConstraint.FC_DC;
                        break;
                    case "SG":
                        fco = (int)LibIecFunctionalConstraint.FC_SG;
                        break;
                    case "SE":
                        fco = (int)LibIecFunctionalConstraint.FC_SE;
                        break;
                    case "SR":
                        fco = (int)LibIecFunctionalConstraint.FC_SR;
                        break;
                    case "OR":
                        fco = (int)LibIecFunctionalConstraint.FC_OR;
                        break;
                    case "BL":
                        fco = (int)LibIecFunctionalConstraint.FC_BL;
                        break;
                    case "EX":
                        fco = (int)LibIecFunctionalConstraint.FC_EX;
                        break;
                    case "СО":
                        fco = (int)LibIecFunctionalConstraint.FC_CO;
                        break;
                    case "ALL":
                        fco = (int)LibIecFunctionalConstraint.FC_ALL;
                        break;
                    case "NONE":
                        fco = (int)LibIecFunctionalConstraint.FC_NONE;
                        break;
                    default:
                        fco = -1;
                        break;
                }
            }
            else
            {
                fco = -1;
            }

            return fco;
        }
        #endregion

        public void CoonvertStaticDataObj(int formatConvert, out string format)
        {
            format = "";

            if (formatConvert == 0)
            {
                format = "bool";
            }
            else if (formatConvert == 3)
            {
                format = "int";
            }
            else if (formatConvert == 10)
            {
                format = "float";
            }
            else if (formatConvert >= 13 && formatConvert <= 21)
            {
                format = "string";
            }
            else if (formatConvert == 22)
            {
                format = "datetime";
            }
            else if (formatConvert == 23)
            {
                format = "ushort";
            }
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    enum LibIecDataAttributeType
    {
        BOOLEAN = 0,/* int */
        INT8 = 1,   /* int8_t */
        INT16 = 2,  /* int16_t */
        INT32 = 3,  /* int32_t */
        INT64 = 4,  /* int64_t */
        INT128 = 5, /* no native mapping! */
        INT8U = 6,  /* uint8_t */
        INT16U = 7, /* uint16_t */
        INT24U = 8, /* uint32_t */
        INT32U = 9, /* uint32_t */
        FLOAT32 = 10, /* float */
        FLOAT64 = 11, /* double */
        ENUMERATED = 12,
        OCTET_STRING_64 = 13,
        OCTET_STRING_6 = 14,
        OCTET_STRING_8 = 15,
        VISIBLE_STRING_32 = 16,
        VISIBLE_STRING_64 = 17,
        VISIBLE_STRING_65 = 18,
        VISIBLE_STRING_129 = 19,
        VISIBLE_STRING_255 = 20,
        UNICODE_STRING_255 = 21,
        TIMESTAMP = 22,
        QUALITY = 23,
        CHECK = 24,
        CODEDENUM = 25,
        GENERIC_BITSTRING = 26,
        CONSTRUCTED = 27,
        ENTRY_TIME = 28,
        PHYCOMADDR = 29
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    enum LibIecFunctionalConstraint
    {
        /** Status information */
        FC_ST = 0,
        /** Measurands - analog values */
        FC_MX = 1,
        /** Setpoint */
        FC_SP = 2,
        /** Substitution */
        FC_SV = 3,
        /** Configuration */
        FC_CF = 4,
        /** Description */
        FC_DC = 5,
        /** Setting group */
        FC_SG = 6,
        /** Setting group editable */
        FC_SE = 7,
        /** Service response / Service tracking */
        FC_SR = 8,
        /** Operate received */
        FC_OR = 9,
        /** Blocking */
        FC_BL = 10,
        /** Extended definition */
        FC_EX = 11,
        /** Control */
        FC_CO = 12,
        FC_ALL = 99,
        FC_NONE = -1
    }
}