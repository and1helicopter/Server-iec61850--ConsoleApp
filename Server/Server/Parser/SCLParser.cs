using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Server.Parser
{
    public class SCLParser
    {
        public void ParseFile()
        {
            string filePath = @"Config.icd";
            //читаем данные из файла
            XDocument doc = XDocument.Load(filePath);
            
            if (doc.Root != null)
            {
                XNamespace xNamespace = doc.Root.Name.Namespace;

                XElement xIED = doc.Root.Element(xNamespace + "IED");

                StructModelObj.Model = new StructModelObj.NodeModel(xIED.Attribute("name").Value);

                IEnumerable<XElement> xLD = (from x in doc.Descendants()
                                             where x.Name.LocalName == "LDevice"
                                             select x).ToList();

                foreach (var LD in xLD)
                {
                    var xAttribute = LD.Attribute("inst");
                    if (xAttribute != null)
                    {
                        StructModelObj.Model.AddListLD(new StructModelObj.NodeLD(xAttribute.Value));

                        IEnumerable<XElement> xLN = LD.Elements().ToList();

                        foreach (var LN in xLN)
                        { 
                            var nameLN = LN.Attribute("lnClass").Value + LN.Attribute("inst").Value;
                            var lnClassLN = LN.Attribute("lnType").Value;
                            StructModelObj.Model.ListLD.Last().ListLN.Add(new StructModelObj.NodeLN(nameLN, lnClassLN));
                        }
                    }
                }
                
                ParseLn(doc);

                ParseDO(doc);

                ParseDA(doc);

                ParseEnum(doc);

                JoinLnToLd();

                SaveFileConfig();
            }
        }

        private void SaveFileConfig()
        {
            string savePath = "test.cfg";
            string str;
            byte[] array;
            FileStream fs = new FileStream(savePath, FileMode.Create);

            str = $"MODEL({StructModelObj.Model.NameModel}){{\n";
            array = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(array, 0, array.Length);

            foreach (var ld in StructModelObj.Model.ListLD)
            {
                // Syntax: LD(<logical device name>){…}
                str = $"LD({ld.NameModel}){{\n";
                array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);
                foreach (var ln in ld.ListLN)
                {
                    // Syntax: LN(<logical node name>){…}
                    str = $"LN({ln.NameLN}){{\n";
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);
                    foreach (var DO in ln.ListDO)
                    {
                        // Syntax: DO(<data object name> <nb of array elements>){…}
                        str = $"DO({DO.NameDO} {0}){{\n";
                        array = System.Text.Encoding.Default.GetBytes(str);
                        fs.Write(array, 0, array.Length);

                        SaveDA(fs, DO.ListDA);

                        str = "}\n";
                        array = System.Text.Encoding.Default.GetBytes(str);
                        fs.Write(array, 0, array.Length);
                    }

                    str = "}\n";
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);
                }

                str = "}\n";
                array = System.Text.Encoding.Default.GetBytes(str);
                fs.Write(array, 0, array.Length);
            }

            str = "}\n";
            array = System.Text.Encoding.Default.GetBytes(str);
            fs.Write(array, 0, array.Length);

            fs.Close();
        }

        private void SaveDA(FileStream fs,List<StructModelObj.NodeDA> listDa)
        {
            // DA(<data attribute name> <nb of array elements> <type> <FC> <trigger options> <sAddr>)[=value];
            // Constructed>
            // DA(<data attribute name> <nb of array elements> 27 <FC> <trigger options> <sAddr>){…}
            string str;
            byte[] array;

            foreach (var da in listDa)
            {
                if (da.ListDA.Count == 0)
                {
                    str = $"DA({da.NameDA} {0} {0} {0} {0} {0})\n";
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);
                }
                else
                {
                    str = $"DA({da.NameDA} {0} {0} {0} {0} {0}){{\n";
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);

                    SaveDA(fs, da.ListDA);

                    str = "}\n";
                    array = System.Text.Encoding.Default.GetBytes(str);
                    fs.Write(array, 0, array.Length);
                }
            }
        }

        private void JoinLnToLd()
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
                            foreach (var tempDo in tempLn.ListDO)
                            {
                                StructModelObj.NodeDO DO = new StructModelObj.NodeDO(tempDo.NameDO, tempDo.TypeDO);

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

                        DO.Last().ListDA.Add(da);

                        if (DO.Last().ListDA.Last().TypeDA != null && DO.Last().ListDA.Last().BTypeDA != "Enum (Integer)")
                        {
                            AddBda(DO.Last().ListDA.Last());
                        }
                    }
                    break;
                }
            }
        }


        private static void AddBda(StructModelObj.NodeDA listDa)
        {
            foreach (var listTempDa in StructModelObj.ListTempDA)
            {
                if (listDa.TypeDA == listTempDa.NameTypeDA)
                {
                    foreach (var tempDa in listTempDa.ListDA)
                    {
                        StructModelObj.NodeDA da = new StructModelObj.NodeDA(tempDa.NameDA, tempDa.FCDA, tempDa.BTypeDA, tempDa.TypeDA, tempDa.TrgOpsDA, tempDa.CountDA);

                        listDa.ListDA.Add(da);
                        if (listDa.ListDA.Last().TypeDA != null)
                        {
                            AddBda(listDa.ListDA.Last());
                        }
                    }
                }
            }
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
                    StructModelObj.ListTempLN.Add(new StructModelObj.NodeLN(xlnClass.Value, xid.Value));
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

                            StructModelObj.ListTempLN.Last().AddDOToLN(new StructModelObj.NodeDO(nameDo, typeDo));
                        }
                    }
                }
            }
        }

        private static void ParseDO(XDocument doc)
        {
            IEnumerable<XElement> xDO = (from x in doc.Descendants()
                                         where x.Name.LocalName == "DOType"
                                         select x).ToList();

            foreach (var DO in xDO)
            {
                var nameDO = DO.Attribute("id").Value;
                var typeDO = DO.Attribute("cdc").Value;

                StructModelObj.ListTempDO.Add(new StructModelObj.NodeDO(nameDO, typeDO));

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
                        : DA.Parent.Attribute("type") != null ? DA.Parent.Attribute("type").Value : null;
                    var trgOpsDA = TriggerOptions.NONE;
                    var countDA = DA.Attribute("count") != null ? DA.Attribute("count").Value : "0";

                    if (bTypeDA.Equals("Enum"))
                    {
                        bTypeDA = String.Concat(bTypeDA, " (Integer)");
                    }

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

                    if (bTypeBDA.Equals("Enum"))
                    {
                        bTypeBDA = String.Concat(bTypeBDA, " (Integer)");
                    }

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
    }
    
    public enum TriggerOptions
    {
        NONE = 0,
        DATA_CHANGED = 1,
        QUALITY_CHANGED = 2,
        DATA_UPDATE = 4,
        INTEGRITY = 8,
        GI = 16
    }
}