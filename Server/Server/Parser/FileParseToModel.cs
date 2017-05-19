using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Server.Parser
{
    public partial class Parser
    {
        #region Парсер файла на основные клсассы 
        private bool ParseDocument(XDocument doc)
        {
            if (doc.Root == null)
            {
                Logging.Log.Write("ParseDocunent: doc.Root == null", "Error   ");
                return false;
            }



            XNamespace xNamespace = doc.Root.Name.Namespace;

            XElement xIed = doc.Root.Element(xNamespace + "IED");

            if (xIed != null)
            {
                var xAttribute = xIed.Attribute("name");
                if (xAttribute != null)
                    ServerModel.Model = new ServerModel.NodeModel(xAttribute.Value);
            }

            IEnumerable<XElement> xLd = (from x in doc.Descendants()
                where x.Name.LocalName == "LDevice"
                select x).ToList();

            foreach (var ld in xLd)
            {
                var xAttribute = ld.Attribute("inst");
                if (xAttribute != null)
                {
                    ServerModel.Model.ListLD.Add(new ServerModel.NodeLD(xAttribute.Value));

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
                                ServerModel.Model.ListLD.Last().ListLN.Add(new ServerModel.NodeLN(nameLn, lnClassLn, xdescStr));
                            }
                        }
                    }
                }
            }

            ParseLn(doc);
            ParseDo(doc);
            ParseDA(doc);
            ParseEnum(doc);

            return true;
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
                    ServerModel.ListTempLN.Add(new ServerModel.NodeLN(xlnClass.Value, xid.Value, xdescStr));
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

                            ServerModel.ListTempLN.Last().ListDO.Add(new ServerModel.NodeDO(nameDo, typeDo, xdescStr));
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

                ServerModel.ListTempDO.Add(new ServerModel.NodeDO(nameDO, typeDO, xdescStr));

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

                    ServerModel.ListTempDO.Last().ListDA.Add(new ServerModel.NodeDA(nameDA, fcDA, bTypeDA, typeDA, (byte)trgOpsDA, countDA));

                    if (DA.Value != "")
                    {
                        ServerModel.ListTempDO.Last().ListDA.Last().Value = DA.Value;
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

                ServerModel.ListTempDA.Add(new ServerModel.TempDA(nameDA));

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

                    ServerModel.ListTempDA.Last().ListDA.Add(new ServerModel.NodeDA(nameBDA, fcBDA, bTypeBDA, typeBDA, (byte)trgOpsBDA, countBDA));

                    if (BDA.Value != "")
                    {
                        ServerModel.ListTempDO.Last().ListDA.Last().Value = BDA.Value;
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

                ServerModel.ListEnumType.Add(new ServerModel.EnumType(nameEnum));

                IEnumerable<XElement> xEnumVal = (from x in Enum.Descendants()
                    where x.Name.LocalName == "EnumVal"
                    select x).ToList();

                foreach (var EnumVal in xEnumVal)
                {
                    var nameEnumVal = Convert.ToInt32(EnumVal.Attribute("ord").Value);
                    var valEnumVal = EnumVal.Value != null ? EnumVal.Value : "";

                    ServerModel.ListEnumType.Last().ListEnumVal.Add(new ServerModel.EnumType.EnumVal(nameEnumVal, valEnumVal));
                }
            }
        }
        #endregion

        #region Объединение основных классов в объектную модель
        private void JoinModel()
        {
            AddDo(ServerModel.Model);
        }

        private void AddDo(ServerModel.NodeModel model)
        {
            foreach (var ld in model.ListLD)
            {
                foreach (var ln in ld.ListLN)
                {
                    foreach (var tempLn in ServerModel.ListTempLN)
                    {
                        if (ln.LnClassLN == tempLn.LnClassLN)
                        {
                            string desc = tempLn.DescLN;
                            ln.DescLN = desc;
                            foreach (var tempDo in tempLn.ListDO)
                            {
                                desc = tempDo.DescDO;
                                ServerModel.NodeDO DO = new ServerModel.NodeDO(tempDo.NameDO, tempDo.TypeDO, desc);

                                ln.ListDO.Add(DO);
                                AddDa(ln.ListDO);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void AddDa(List<ServerModel.NodeDO> DO)
        {
            foreach (var tempDo in ServerModel.ListTempDO)
            {
                if (DO.Last().TypeDO == tempDo.NameDO)
                {
                    foreach (var tempDa in tempDo.ListDA)
                    {
                        ServerModel.NodeDA da = new ServerModel.NodeDA(tempDa.NameDA, tempDa.FCDA, tempDa.BTypeDA, tempDa.TypeDA, tempDa.TrgOpsDA, tempDa.CountDA);
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


        private static void AddBda(ServerModel.NodeDA listDa, string fcDa)
        {
            foreach (var listTempDa in ServerModel.ListTempDA)
            {
                if (listDa.TypeDA == listTempDa.NameTypeDA)
                {
                    foreach (var tempDa in listTempDa.ListDA)
                    {
                        ServerModel.NodeDA da = new ServerModel.NodeDA(tempDa.NameDA, tempDa.FCDA ?? fcDa, tempDa.BTypeDA, tempDa.TypeDA, tempDa.TrgOpsDA, tempDa.CountDA);

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

    }
}