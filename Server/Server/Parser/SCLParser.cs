﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Server.Parser
{
    public class SCLParser
    {
        public static void ParseFile()
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
                
                ParseLN(doc);

                ParseDO(doc);

                ParseDA(doc);

                ParseEnum(doc);

                JoinToModel();
            }
        }

        private static void JoinToModel()
        {
            JoinLNtoLD();
        }

        private static void JoinLNtoLD()
        {
            foreach (var LD in StructModelObj.Model.ListLD)
            {
                AddDO(LD.ListLN);
            }
        }

        private static void AddDO(List<StructModelObj.NodeLN> ListLN)
        {
            foreach (var LN in ListLN)
            {
                foreach (var TempLN in StructModelObj.ListTempLN)
                {
                    if (LN.LnClassLN == TempLN.LnClassLN)
                    {
                        foreach (var TempDO in TempLN.ListDO)
                        {
                            LN.ListDO.Add(TempDO);
                            AddDA(LN);
                        }
                    }
                }
            }
        }

        private static void AddDA(StructModelObj.NodeLN LN)
        {
            foreach (var tempDO in StructModelObj.ListTempDO)
            {
                if (LN.ListDO.Last().TypeDO == tempDO.TypeDO)
                {
                    foreach (var TempDA in tempDO.ListDA)
                    {
                        LN.ListDO.Last().ListDA.Add(TempDA);
                        /*
                            AddBDA(LN.ListDO.Last().ListDA);
                        */
                    }
                }
            }
        }

        private static void AddBDA(List<StructModelObj.NodeDA> listDa)
        {
            

        }

        private static void ParseLN(XDocument doc)
        {
            IEnumerable<XElement> xLN = (from x in doc.Descendants()
                                         where x.Name.LocalName == "LNodeType"
                                         select x).ToList();

            foreach (var LN in xLN)
            {
                var xlnClass = LN.Attribute("lnClass");
                var xid = LN.Attribute("id"); 
                if (xlnClass != null && xid != null)
                {
                    StructModelObj.ListTempLN.Add(new StructModelObj.NodeLN(xlnClass.Value, xid.Value));
                    IEnumerable<XElement> xDOElements = (from x in LN.Descendants()
                                                         where x.Name.LocalName == "DO"
                                                         select x).ToList();
                    foreach (var DO in xDOElements)
                    {
                        var nameDO = DO.Attribute("name").Value;
                        var typeDO = DO.Attribute("type").Value;

                        StructModelObj.ListTempLN.Last().AddDOToLN(new StructModelObj.NodeDO(nameDO, typeDO));
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