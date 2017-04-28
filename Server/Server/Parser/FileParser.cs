using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Server.Parser
{
    public class FileParser
    {
        public void ParseFile()
        {
            string filePath = @"test.icd";
            //читаем данные из файла
            XDocument doc = XDocument.Load(filePath);
            
            if (doc.Root != null)
            {
                ParseDocument(doc);         //Парсим основные классы модели
                JoinModel();                //Создаем пустую объектную модель 
                ParseDefultParam(doc);      //Заполняем объектную модель инициализированными параметрами 

                CreateClasses();            //Создаем обновляймые классы 

                SaveFileConfig();           //Создаем из объектной модели - конфигурационную 
            }
        }

        #region Парсер файла на основные клсассы 
        private void ParseDocument(XDocument doc)
        {
            XNamespace xNamespace = doc.Root.Name.Namespace;

            XElement xIed = doc.Root.Element(xNamespace + "IED");

            if (xIed != null)
            {
                var xAttribute = xIed.Attribute("name");
                if (xAttribute != null)
                    StructModelObj.Model = new StructModelObj.NodeModel(xAttribute.Value);
            }

            IEnumerable<XElement> xLd = (from x in doc.Descendants()
                where x.Name.LocalName == "LDevice"
                select x).ToList();

            foreach (var ld in xLd)
            {
                var xAttribute = ld.Attribute("inst");
                if (xAttribute != null)
                {
                    StructModelObj.Model.AddListLD(new StructModelObj.NodeLD(xAttribute.Value));

                    IEnumerable<XElement> xLn = ld.Elements().ToList();

                    foreach (var ln in xLn)
                    {
                        var xAttributelnClass = ln.Attribute("lnClass");
                        var xAttributeinst = ln.Attribute("inst");
                        if (xAttributelnClass != null && xAttributeinst != null)
                        {
                            var xAttributeprefix = ln.Attribute("prefix");
                            string nameLn;
                            if (xAttributeprefix != null)
                            {
                                nameLn = xAttributeprefix.Value + xAttributelnClass.Value + xAttributeinst.Value;
                            }
                            else
                            {
                                nameLn = xAttributelnClass.Value + xAttributeinst.Value;
                            }
                            var xAttributelnType = ln.Attribute("lnType");
                            if (xAttributelnType != null)
                            {
                                var lnClassLn = xAttributelnType.Value;
                                string xdescStr = "";
                                StructModelObj.Model.ListLD.Last().ListLN.Add(new StructModelObj.NodeLN(nameLn, lnClassLn, xdescStr));
                            }
                        }
                    }
                }
            }

            ParseLn(doc);
            ParseDo(doc);
            ParseDA(doc);
            ParseEnum(doc);
        }

        private static void ParseLn(XDocument doc)
        {
            IEnumerable<XElement> xLn = (from x in doc.Descendants()
                where x.Name.LocalName == "LNodeType"
                select x).ToList();

            foreach (var ln in xLn)
            {
                var xlnClass = ln.Attribute("lnClass");
                var xid = ln.Attribute("id");
                if (xlnClass != null && xid != null)
                {
                    string xdescStr = "";
                    if (ln.Attribute("desc") != null)
                    {
                        xdescStr = ln.Attribute("desc").Value;
                    }
                    StructModelObj.ListTempLN.Add(new StructModelObj.NodeLN(xlnClass.Value, xid.Value, xdescStr));
                    IEnumerable<XElement> xDoElements = (from x in ln.Descendants()
                        where x.Name.LocalName == "DO"
                        select x).ToList();
                    foreach (var DO in xDoElements)
                    {
                        var xNameDo = DO.Attribute("name");
                        var xTypeDo = DO.Attribute("type");
                        if (xNameDo != null && xTypeDo != null)
                        {
                            var nameDo = xNameDo.Value;
                            var typeDo = xTypeDo.Value;
                            xdescStr = "";
                            if (DO.Attribute("desc") != null)
                            {
                                xdescStr = DO.Attribute("desc").Value;
                            }

                            StructModelObj.ListTempLN.Last().AddDOToLN(new StructModelObj.NodeDO(nameDo, typeDo, xdescStr));
                        }
                    }
                }
            }
        }

        private static void ParseDo(XDocument doc)
        {
            IEnumerable<XElement> xDo = (from x in doc.Descendants()
                where x.Name.LocalName == "DOType"
                select x).ToList();

            foreach (var DO in xDo)
            {
                var nameDO = DO.Attribute("id").Value;
                var typeDO = DO.Attribute("cdc").Value;
                string xdescStr = "";
                if (DO.Attribute("desc") != null)
                {
                    xdescStr = DO.Attribute("desc").Value;
                }

                StructModelObj.ListTempDO.Add(new StructModelObj.NodeDO(nameDO, typeDO, xdescStr));

                IEnumerable<XElement> xDAElements = (from x in DO.Descendants()
                    where x.Name.LocalName == "DA"
                    select x).ToList();

                foreach (var DA in xDAElements)
                {
                    var nameDA = DA.Attribute("name").Value;
                    var fcDA = DA.Attribute("fc") != null
                        ? DA.Attribute("fc").Value
                        : DA.Parent.Attribute("fc").Value;
                    var bTypeDA = DA.Attribute("bType").Value;
                    var typeDA = DA.Attribute("type") != null
                        ? DA.Attribute("type").Value
                        : DA.Parent.Attribute("type") != null
                            ? DA.Parent.Attribute("type").Value
                            : null;
                    var trgOpsDA = TriggerOptions.NONE;
                    var countDA = DA.Attribute("count") != null ? DA.Attribute("count").Value : "0";

                    //if (bTypeDA.Equals("Enum"))
                    //{
                    //    bTypeDA = String.Concat(bTypeDA, " (Integer)");
                    //}

                    var dchgDA = DA.Attribute("dchg");
                    if ((dchgDA != null ? dchgDA.Value : "false").ToLower() == "true")
                        trgOpsDA |= TriggerOptions.DATA_CHANGED;

                    var qchgDA = DA.Attribute("qchg");
                    if ((qchgDA != null ? qchgDA.Value : "false").ToLower() == "true")
                        trgOpsDA |= TriggerOptions.QUALITY_CHANGED;

                    var dupdDA = DA.Attribute("dupd");
                    if ((dupdDA != null ? dupdDA.Value : "false").ToLower() == "true")
                        trgOpsDA |= TriggerOptions.DATA_UPDATE;

                    StructModelObj.ListTempDO.Last()
                        .AddDAToDA(new StructModelObj.NodeDA(nameDA, fcDA, bTypeDA, typeDA, (byte)trgOpsDA, countDA));

                    if (DA.Value != "")
                    {
                        StructModelObj.ListTempDO.Last().ListDA.Last().Value = DA.Value;
                    }
                }
            }
        }

        private static void ParseDA(XDocument doc)
        {
            IEnumerable<XElement> xDA = (from x in doc.Descendants()
                where x.Name.LocalName == "DAType"
                select x).ToList();

            foreach (var DA in xDA)
            {
                var nameDA = DA.Attribute("id").Value;

                StructModelObj.ListTempDA.Add(new StructModelObj.TypeDA(nameDA));

                IEnumerable<XElement> xDAElements = (from x in DA.Descendants()
                    where x.Name.LocalName == "BDA"
                    select x).ToList();

                foreach (var BDA in xDAElements)
                {
                    var nameBDA = BDA.Attribute("name").Value;
                    var fcBDA = BDA.Attribute("fc") != null ? BDA.Attribute("fc").Value : null;
                    var bTypeBDA = BDA.Attribute("bType").Value;
                    var typeBDA = BDA.Attribute("type") != null ? BDA.Attribute("type").Value : null;
                    var trgOpsBDA = TriggerOptions.NONE;
                    var countBDA = BDA.Attribute("count") != null ? BDA.Attribute("count").Value : "0";

                    //if (bTypeBDA.Equals("Enum"))
                    //{
                    //    bTypeBDA = String.Concat(bTypeBDA, " (Integer)");
                    //}

                    var dchgBDA = BDA.Attribute("dchg");
                    if ((dchgBDA != null ? dchgBDA.Value : "false").ToLower() == "true")
                        trgOpsBDA |= TriggerOptions.DATA_CHANGED;

                    var qchgBDA = BDA.Attribute("qchg");
                    if ((qchgBDA != null ? qchgBDA.Value : "false").ToLower() == "true")
                        trgOpsBDA |= TriggerOptions.QUALITY_CHANGED;

                    var dupdBDA = BDA.Attribute("dupd");
                    if ((dupdBDA != null ? dupdBDA.Value : "false").ToLower() == "true")
                        trgOpsBDA |= TriggerOptions.DATA_UPDATE;

                    StructModelObj.ListTempDA.Last().ListDA.Add(new StructModelObj.NodeDA(nameBDA, fcBDA, bTypeBDA, typeBDA, (byte)trgOpsBDA, countBDA));

                    if (BDA.Value != "")
                    {
                        StructModelObj.ListTempDO.Last().ListDA.Last().Value = BDA.Value;
                    }
                }
            }
        }

        private static void ParseEnum(XDocument doc)
        {
            IEnumerable<XElement> xEnum = (from x in doc.Descendants()
                where x.Name.LocalName == "EnumType"
                select x).ToList();

            foreach (var Enum in xEnum)
            {
                var nameEnum = Enum.Attribute("id").Value;

                StructModelObj.ListEnumType.Add(new StructModelObj.EnumType(nameEnum));

                IEnumerable<XElement> xEnumVal = (from x in Enum.Descendants()
                    where x.Name.LocalName == "EnumVal"
                    select x).ToList();

                foreach (var EnumVal in xEnumVal)
                {
                    var nameEnumVal = Convert.ToInt32(EnumVal.Attribute("ord").Value);
                    var valEnumVal = EnumVal.Value != null ? EnumVal.Value : "";

                    StructModelObj.ListEnumType.Last().ListEnumVal.Add(new StructModelObj.EnumType.EnumVal(nameEnumVal, valEnumVal));
                }
            }
        }
        #endregion

        #region Объединение основных классов в объектную модель
        private void JoinModel()
        {
            AddDo(StructModelObj.Model);
        }

        private void AddDo(StructModelObj.NodeModel model)
        {
            foreach (var ld in model.ListLD)
            {
                foreach (var ln in ld.ListLN)
                {
                    foreach (var tempLn in StructModelObj.ListTempLN)
                    {
                        if (ln.LnClassLN == tempLn.LnClassLN)
                        {
                            string desc = tempLn.DescLN;
                            ln.DescLN = desc;
                            foreach (var tempDo in tempLn.ListDO)
                            {
                                desc = tempDo.DescDO;
                                StructModelObj.NodeDO DO = new StructModelObj.NodeDO(tempDo.NameDO, tempDo.TypeDO, desc);

                                ln.ListDO.Add(DO);
                                AddDa(ln.ListDO);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void AddDa(List<StructModelObj.NodeDO> DO)
        {
            foreach (var tempDo in StructModelObj.ListTempDO)
            {
                if (DO.Last().TypeDO == tempDo.TypeDO)
                {
                    foreach (var tempDa in tempDo.ListDA)
                    {
                        StructModelObj.NodeDA da = new StructModelObj.NodeDA(tempDa.NameDA, tempDa.FCDA, tempDa.BTypeDA, tempDa.TypeDA, tempDa.TrgOpsDA, tempDa.CountDA);
                        da.Value = tempDa.Value;

                        DO.Last().ListDA.Add(da);

                        if (DO.Last().ListDA.Last().TypeDA != null && DO.Last().ListDA.Last().BTypeDA != "Enum")
                        {
                            AddBda(DO.Last().ListDA.Last(), da.FCDA);
                        }
                    }
                    break;
                }
            }
        }


        private static void AddBda(StructModelObj.NodeDA listDa, string fcDa)
        {
            foreach (var listTempDa in StructModelObj.ListTempDA)
            {
                if (listDa.TypeDA == listTempDa.NameTypeDA)
                {
                    foreach (var tempDa in listTempDa.ListDA)
                    {
                        StructModelObj.NodeDA da = new StructModelObj.NodeDA(tempDa.NameDA, tempDa.FCDA ?? fcDa, tempDa.BTypeDA, tempDa.TypeDA, tempDa.TrgOpsDA, tempDa.CountDA);

                        listDa.ListDA.Add(da);
                        if (listDa.ListDA.Last().TypeDA != null)
                        {
                            AddBda(listDa.ListDA.Last(), da.FCDA);
                        }
                    }
                }
            }
        }
        #endregion

        #region Заполнение объектной модели параметрами 
        private void ParseDefultParam(XDocument doc)
        {
            var ied = (from x in doc.Descendants()
                where x.Name.LocalName == "IED"
                select x).Attributes("name").ToList().Last().Value;

            IEnumerable<XElement> xLd = (from x in doc.Descendants()
                where x.Name.LocalName == "LDevice"
                select x).ToList();

            foreach (var lditem in xLd)
            {
                var xAttributeinst = lditem.Attribute("inst");
                if (xAttributeinst != null)
                {
                    var ld = xAttributeinst.Value;

                    IEnumerable<XElement> xLn = lditem.Elements().ToList();

                    foreach (var lnitem in xLn)
                    {
                        var xAttributelnClass = lnitem.Attribute("lnClass");
                        var xAttributeinbbst = lnitem.Attribute("inst");

                        var xAttributeprefix = lnitem.Attribute("prefix");

                        if (xAttributelnClass != null && xAttributeinbbst != null)
                        {
                            string ln;
                            if (xAttributeprefix != null)
                            {
                                ln = xAttributeprefix.Value + xAttributelnClass.Value + xAttributeinbbst.Value;
                            }
                            else
                            {
                                ln = xAttributelnClass.Value + xAttributeinbbst.Value;
                            }

                            IEnumerable<XElement> xDoi = lnitem.Elements().ToList();

                            foreach (var doiitem in xDoi)
                            {
                                var xAttributeDoi = doiitem.Attribute("name");
                                if (xAttributeDoi != null)
                                {
                                    var doi = xAttributeDoi.Value;

                                    var type = (from x in doiitem.Descendants()
                                        where x.Name.LocalName == "private"
                                        select x).ToList();

                                    string[] typeDO = { "D" };

                                    if (type.Count != 0)
                                    {
                                        typeDO = type[0].Value.Split(';');
                                    }

                                    IEnumerable<XElement> xDai = doiitem.Elements().ToList();

                                    foreach (var daiitem in xDai)
                                    {
                                        //Рассматриваем DA верхнего уровня 
                                        if (daiitem.Attribute("name") != null)
                                        {
                                            if ((from x in daiitem.Descendants()
                                                    where x.Name.LocalName == "DAI"
                                                    select x).ToList().Count == 0)
                                            {
                                                //Если нет вложений типа DA
                                                var dai = daiitem.Attribute("name").Value;
                                                var value = daiitem.Value;
                                                ParseFillModel(ied, ld, ln, doi, dai, typeDO, value);
                                                //     StructDefultDataObj.AddStructDefultDataObj(ied, ld, ln, doi, dai, value);
                                            }
                                            else
                                            {
                                                //Если есть вложения типа DA
                                                var dai = daiitem.Attribute("name").Value;
                                                ParseDefultParamBDA(daiitem, ied, ld, ln, doi, typeDO, dai);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseDefultParamBDA(XElement bdai, string ied, string ld, string ln, string doi, string[] typeDO, string dai)
        {
            IEnumerable<XElement> xDai = bdai.Elements().ToList();

            foreach (var daiitem in xDai)
            {
                //Рассматриваем DA верхнего уровня 
                if (daiitem.Attribute("name") != null)
                {
                    if ((from x in daiitem.Descendants()
                            where x.Name.LocalName == "DAI"
                            select x).ToList().Count == 0)
                    {
                        //Если нет вложений типа DA
                        var daitemp = dai + "." + daiitem.Attribute("name").Value;
                        var value = daiitem.Value;
                        ParseFillModel(ied, ld, ln, doi, daitemp, typeDO, value);
                        //      StructDefultDataObj.AddStructDefultDataObj(ied, ld, ln, doi, daitemp, value);
                    }
                    else
                    {
                        //Если есть вложения типа DA
                        var daitemp = dai + "." + daiitem.Attribute("name").Value;
                        ParseDefultParamBDA(daiitem, ied, ld, ln, doi, typeDO, daitemp);
                    }
                }
            }
        }

        private void ParseFillModel(string ied, string ld, string ln, string doi, string daitemp, string[] typeDO, string value)
        {
            var LN = (from x in StructModelObj.Model.ListLD
                where x.NameLD == ld
                select x.ListLN).ToList().Last().ToList();


            var DO = (from x in LN
                where x.NameLN == ln
                select x.ListDO).ToList().Last().ToList();

            var tempDO = (from x in DO
                where x.NameDO == doi
                select x).ToList().Last();

            if (typeDO.ToList().Count == 3)
            {
                string[] typeTempDO = typeDO[0].Split(':');
                tempDO.InfoData(typeTempDO[0], typeTempDO[1], typeDO[1], typeDO[2]);
            }
            else
            {
                tempDO.InfoData(null, "0", "0", typeDO[0]);
            }

            var DA = (from x in DO
                where x.NameDO == doi
                select x.ListDA).ToList().Last().ToList();

            string[] str = daitemp.Split('.');
            var list = new List<string>(str);

            ParseFillModelBDA(list, DA, value);
        }

        private void ParseFillModelBDA(List<string> list, List<StructModelObj.NodeDA> DAI, string value)
        {
            if (list.Count == 1)
            {
                var DA = (from x in DAI
                    where x.NameDA == list[0]
                    select x).ToList().Last();

                DA.Value = value;
            }
            else
            {
                if (list.Count == 0) { return; }

                var DA = (from x in DAI
                    where x.NameDA == list[0]
                    select x.ListDA).ToList().Last().ToList();

                list.RemoveAt(0);
                ParseFillModelBDA(list, DA, value);
            }
        }
        #endregion

        #region Создание обновляймых классов 
        private void CreateClasses()
        {
            foreach (var itemLd in StructModelObj.Model.ListLD)
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

                        continue;
                    }

                    var defualtDo = (from x in itemLn.ListDO
                                     where x.Type == "D"
                                     select x).ToList();


                    if (defualtDo.Count != 0)
                    {

                        
                    }
                }
            }
        }

        private void GetDo(List<StructModelObj.NodeDO> getDo, string path)
        {
            foreach (var itemDo in getDo)
            {
                if (itemDo.TypeDO == "MV")
                {
                    string pathNameDO = path + "." + itemDo.NameDO;
                    var mv = new MvClass();

                    var siUnit = Convert.ToInt32((from y in (from x in itemDo.ListDA
                                                             where x.NameDA.ToUpper() == "Unit".ToUpper()
                                                             select x).ToList().Last().ListDA.ToList()
                                                  where y.NameDA.ToUpper() == "SIUnit".ToUpper()
                                                  select y).ToList().Last().Value);

                    var multiplier = Convert.ToInt32((from y in (from x in itemDo.ListDA
                                                                 where x.NameDA.ToUpper() == "Unit".ToUpper()
                                                                 select x).ToList().Last().ListDA.ToList()
                                                      where y.NameDA.ToUpper() == "Multiplier".ToUpper()
                                                      select y).ToList().Last().Value);

                    var scaleFactor = Convert.ToInt32((from y in (from x in itemDo.ListDA
                                                                  where x.NameDA.ToUpper() == "sVC".ToUpper()
                                                                  select x).ToList().Last().ListDA.ToList()
                                                       where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
                                                       select y).ToList().Last().Value);

                    var offset = Convert.ToInt32((from y in (from x in itemDo.ListDA
                                                             where x.NameDA.ToUpper() == "sVC".ToUpper()
                                                             select x).ToList().Last().ListDA.ToList()
                                                  where y.NameDA.ToUpper() == "Offset".ToUpper()
                                                  select y).ToList().Last().Value);

                    mv.MvClassFill(siUnit, multiplier, scaleFactor, offset, itemDo.DescDO);

                    StructUpdateDataObj.DataObject dataObj = new StructUpdateDataObj.DataObject(pathNameDO, itemDo.Format,itemDo.Mask,itemDo.Addr,itemDo.TypeDO,mv);
                    StructUpdateDataObj.DataClassGet.Add(dataObj);



                }

            }
        }

        #endregion

        #region Сохранение объектной модели в конфигурациионную модель для сервера
        private void SaveFileConfig()
        {
            string savePath = "test.cfg";
            FileStream fs = new FileStream(savePath, FileMode.Create);

            string str = $"MODEL({StructModelObj.Model.NameModel}){{\n";
            var array = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(array, 0, array.Length);

            SaveLd(fs, StructModelObj.Model.ListLD);

            str = "}\n";
            array = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(array, 0, array.Length);

            fs.Close();
        }

        private void SaveLd(FileStream fs, List<StructModelObj.NodeLD> listLd)
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

        private void SaveLn(FileStream fs, List<StructModelObj.NodeLN> listLn)
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

        private void SaveDo(FileStream fs, List<StructModelObj.NodeDO> listDo)
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

        private void SaveDa(FileStream fs, List<StructModelObj.NodeDA> listDa)
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
                    case "CO":
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

        public void UpdateStaticDataObj(StructDefultDataObj.DefultDataObj itemDefultDataObj, out string format, out string value, out string path)
        {
            var oo = (from x in StructModelObj.Model.ListLD
                where x.NameLD == itemDefultDataObj.LDevice
                select x).ToList();

            var oo1 = (from x in oo[0].ListLN
                where x.NameLN == itemDefultDataObj.LN
                select x).ToList();

            var oo2 = (from x in oo1[0].ListDO
                where x.NameDO == itemDefultDataObj.DOI
                select x).ToList();

            if (itemDefultDataObj.DAI.Contains("."))
            {
                //Если DA вложеные 
                string[] str = itemDefultDataObj.DAI.Split('.');
                var oo3 = (from x in oo2[0].ListDA
                    where x.NameDA == str[0]
                    select x).ToList();
                var list = new List<string>(str);
                list.RemoveAt(0);
                str = list.ToArray();
                UpdateStaticDataObjBDA(oo3, itemDefultDataObj, str, out format, out value, out path);
            }
            else
            {
                //Если DA не вложеные
                var oo3 = (from x in oo2[0].ListDA
                    where x.NameDA == itemDefultDataObj.DAI
                    select x).ToList();

                LoopUpdateStaticDataObj(oo3, itemDefultDataObj, out format, out value, out path);
            }
        }

        private void UpdateStaticDataObjBDA(List<StructModelObj.NodeDA> da, StructDefultDataObj.DefultDataObj itemDefultDataObj, string[] str, out string format, out string value, out string path)
        {
            if (str.Length != 1)
            {
                //Если DA вложеные 
                var oo3 = (from x in da[0].ListDA
                    where x.NameDA == str[0]
                    select x).ToList();
                var list = new List<string>(str);
                list.RemoveAt(0);
                str = list.ToArray();
                UpdateStaticDataObjBDA(oo3, itemDefultDataObj, str, out format, out value, out path);
            }
            else
            {
                //Если DA не вложеные
                var oo3 = (from x in da[0].ListDA
                    where x.NameDA == str[0]
                    select x).ToList();

                LoopUpdateStaticDataObj(oo3, itemDefultDataObj, out format, out value, out path);
            }
        }


        private void LoopUpdateStaticDataObj(List<StructModelObj.NodeDA> oo3, StructDefultDataObj.DefultDataObj itemDefultDataObj,  out string format, out string value, out string path)
        {
            oo3[0].Value = itemDefultDataObj.Value;
            var oo4 = MapLibiecType(oo3[0].BTypeDA);
            CoonvertStaticDataObj(oo4, out format);

            value = itemDefultDataObj.Value;
            path = itemDefultDataObj.LDevice + "/" + itemDefultDataObj.LN + "." + itemDefultDataObj.DOI + "." + itemDefultDataObj.DAI;

            //if (oo3[0].BTypeDA == "Enum")
            //{
            //    foreach (var itemEnumType in StructModelObj.ListEnumType)
            //    {
            //        if (oo3[0].TypeDA == itemEnumType.NameEnumType)
            //        {
            //            foreach (var itemEnumVal in itemEnumType.ListEnumVal)
            //            {
            //                if (itemEnumVal.ValEnumVal == itemDefultDataObj.Value)
            //                {
            //                    value = Convert.ToString(itemEnumVal.OrdEnumVal);
            //                    path = itemDefultDataObj.LDevice + "/" + itemDefultDataObj.LN + "." + itemDefultDataObj.DOI + "." + itemDefultDataObj.DAI;
            //                    break;
            //                }
            //            }
            //            break;
            //        }
            //    }
            //}
        }

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
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    enum TriggerOptions
    {
        NONE = 0,
        DATA_CHANGED = 1,
        QUALITY_CHANGED = 2,
        DATA_UPDATE = 4,
        INTEGRITY = 8,
        GI = 16
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