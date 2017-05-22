using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

            if (xIed == null)
            {
                Logging.Log.Write("ParseDocunent: IED == null", "Error   ");
                return false;
            }

            if (xIed.Attribute("name") == null)
            {
                Logging.Log.Write("ParseDocunent: IED.name == null", "Error   ");
                return false;
            }

            ServerModel.Model = new ServerModel.NodeModel(xIed.Attribute("name")?.Value);

            IEnumerable<XElement> xLd = (from x in doc.Descendants()
                                         where x.Name.LocalName == "LDevice"
                                         select x).ToList();

            if (!xLd.Any())
            {
                Logging.Log.Write("ParseDocunent: LDevice == null", "Error   ");
                return false;
            }

            foreach (var ld in xLd)
            {
                if (ld.Attribute("inst") == null)
                {
                    Logging.Log.Write("ParseDocunent: LDevice.inst == null", "Error   ");
                    return false;
                }

                ServerModel.Model.ListLD.Add(new ServerModel.NodeLD(ld.Attribute("inst")?.Value));

                IEnumerable<XElement> xLn = ld.Elements().ToList();

                if (!xLn.Any())
                {
                    Logging.Log.Write("ParseDocunent: LN == null", "Error   ");
                    return false;
                }

                foreach (var ln in xLn)
                {
                    if (ln.Attribute("lnClass") != null && ln.Attribute("inst") != null)
                    {
                        string nameLN = ln.Attribute("prefix")?.Value + ln.Attribute("lnClass")?.Value + ln.Attribute("inst")?.Value;

                        if (ln.Attribute("lnType") == null)
                        {
                            Logging.Log.Write("ParseDocunent: LN.lnType == null", "Error   ");
                            return false;
                        }

                        ServerModel.Model.ListLD.Last().ListLN.Add(new ServerModel.NodeLN(nameLN, ln.Attribute("lnType")?.Value, ""));
                    }
                    else
                    {
                        Logging.Log.Write("ParseDocunent: LN.lnClass == null or LN.inst == null", "Error   ");
                        return false;
                    }
                }
            }

            if (!ParseLN(doc))
            {
                Logging.Log.Write("ParseDocunent.ParseLN: Finish with status false", "Error   ");
                return false;
            }
            if (!ParseDO(doc))
            {
                Logging.Log.Write("ParseDocunent.ParseDO: Finish with status false", "Error   ");
                return false;
            }
            if (!ParseDA(doc))
            {
                Logging.Log.Write("ParseDocunent.ParseDA: Finish with status false", "Error   ");
                return false;
            }
            if (!ParseEnum(doc))
            {
                Logging.Log.Write("ParseDocunent.ParseEnum: Finish with status false", "Warning ");
            }
            if (!JoinModel())                        //Создаем объектную модель 
            {
                Logging.Log.Write("ParseDocunent.JoinModel: Finish with status false", "Error   ");
                return false;
            }

            Logging.Log.Write("ParseDocunent: File parse success", "Success ");
            return true;
        }

        private bool ParseLN(XDocument doc)
        {
            IEnumerable<XElement> xLn = (from x in doc.Descendants()
                                         where x.Name.LocalName == "LNodeType"
                                         select x).ToList();

            if (!xLn.Any())
            {
                Logging.Log.Write("ParseDocunent.ParseLN: LNodeType == null", "Error   ");
                return false;
            }

            foreach (var ln in xLn)
            {
                if (ln.Attribute("lnClass") != null && ln.Attribute("id") != null)
                {
                    ServerModel.ListTempLN.Add(new ServerModel.NodeLN(ln.Attribute("lnClass")?.Value, ln.Attribute("id")?.Value, ln.Attribute("desc") != null ? ln.Attribute("desc")?.Value : ""));

                    IEnumerable<XElement> xDoElements = (from x in ln.Descendants()
                                                         where x.Name.LocalName == "DO"
                                                         select x).ToList();

                    if (!xDoElements.Any())
                    {
                        Logging.Log.Write("ParseDocunent.ParseLN: LNodeType == null", "Error   ");
                        return false;
                    }

                    foreach (var DO in xDoElements)
                    {
                        if (DO.Attribute("name") != null && DO.Attribute("type") != null)
                        {
                            ServerModel.ListTempLN.Last().ListDO.Add(new ServerModel.NodeDO(DO.Attribute("name")?.Value, DO.Attribute("type")?.Value, DO.Attribute("desc") != null ? DO.Attribute("desc")?.Value : ""));
                        }
                        else
                        {
                            Logging.Log.Write("ParseDocunent.ParseLN: DO.name == null or DO.type == null", "Error   ");
                            return false;
                        }
                    }
                }
                else
                {
                    Logging.Log.Write("ParseDocunent.ParseLN: LNodeType.lnClass == null or LNodeType.id == null", "Error   ");
                    return false;
                }
            }

            return true;
        }

        private bool ParseDO(XDocument doc)
        {
            IEnumerable<XElement> xDo = (from x in doc.Descendants()
                where x.Name.LocalName == "DOType"
                select x).ToList();

            if (!xDo.Any())
            {
                Logging.Log.Write("ParseDocunent.ParseDO: DOType == null", "Error   ");
                return false;
            }

            foreach (var DO in xDo)
            {
                if (DO.Attribute("id") == null || DO.Attribute("cdc") == null)
                {
                    Logging.Log.Write("ParseDocunent.ParseDO: DO.id == null or DO.cdc", "Error   ");
                    return false;
                }

                ServerModel.ListTempDO.Add(new ServerModel.NodeDO(DO.Attribute("id")?.Value, DO.Attribute("cdc")?.Value, DO.Attribute("desc") != null ? DO.Attribute("desc")?.Value : ""));

                IEnumerable<XElement> xDAElements = (from x in DO.Descendants()
                                                     where x.Name.LocalName == "DA"
                                                     select x).ToList();

                if (!xDAElements.Any())
                {
                    Logging.Log.Write("ParseDocunent.ParseDO: DA == null", "Error   ");
                    return false;
                }

                foreach (var da in xDAElements)
                {
                    if (da.Attribute("name") == null || da.Attribute("bType") == null || da.Attribute("fc") == null)
                    {
                        Logging.Log.Write("ParseDocunent.ParseDO: DA.name == null or DA.bType == null or DA.fc == null", "Warning ");
                        continue;
                    }

                    var nameDA = da.Attribute("name")?.Value;
                    var fcDA = da.Attribute("fc")?.Value;
                    var bTypeDA = da.Attribute("bType")?.Value;
                    var typeDA = da.Attribute("type") != null ? da.Attribute("type")?.Value : da.Parent.Attribute("type") != null ? da.Parent.Attribute("type")?.Value : null;
                    var trgOpsDA = TriggerOptions.NONE;
                    var countDA = da.Attribute("count") != null ? da.Attribute("count")?.Value : "0";

                    if ((da.Attribute("dchg")?.Value ?? "false").ToLower() == "true") trgOpsDA |= TriggerOptions.DATA_CHANGED;
                    if ((da.Attribute("qchg")?.Value ?? "false").ToLower() == "true") trgOpsDA |= TriggerOptions.QUALITY_CHANGED;
                    if ((da.Attribute("dupd")?.Value ?? "false").ToLower() == "true") trgOpsDA |= TriggerOptions.DATA_UPDATE;

                    ServerModel.ListTempDO.Last().ListDA.Add(new ServerModel.NodeDA(nameDA, fcDA, bTypeDA, typeDA, (byte)trgOpsDA, countDA));

                    if (da.Value != null)
                    {
                        ServerModel.ListTempDO.Last().ListDA.Last().Value = da.Value;
                    }
                }
            }

            return true;
        }

        private bool ParseDA(XDocument doc)
        {
            IEnumerable<XElement> xDA = (from x in doc.Descendants()
                                         where x.Name.LocalName == "DAType"
                                         select x).ToList();

            if (!xDA.Any())
            {
                Logging.Log.Write("ParseDocunent.ParseDA: DAType == null", "Error   ");
                return false;
            }

            foreach (var da in xDA)
            {
                IEnumerable<XElement> xDAElements = (from x in da.Descendants()
                                                     where x.Name.LocalName == "BDA"
                                                     select x).ToList();

                if (!xDAElements.Any())
                {
                    Logging.Log.Write("ParseDocunent.ParseDO: BDA == null", "Error   ");
                    return false;
                }

                ServerModel.ListTempDA.Add(new ServerModel.TempDA(da.Attribute("id")?.Value));

                foreach (var bda in xDAElements)
                {
                    if (bda.Attribute("name") == null || bda.Attribute("bType") == null )
                    {
                        Logging.Log.Write("ParseDocunent.ParseDO: DA.name == null or DA.bType == null", "Warning ");
                        continue;
                    }

                    var nameBda = bda.Attribute("name")?.Value;
                    var bTypeBda = bda.Attribute("bType")?.Value;
                    var typeBda = bda.Attribute("type") != null ? bda.Attribute("type")?.Value : null;
                    var trgOpsBda = TriggerOptions.NONE;
                    var countBda = bda.Attribute("count") != null ? bda.Attribute("count")?.Value : "0";

                    if ((bda.Attribute("dchg")?.Value ?? "false").ToLower() == "true") trgOpsBda |= TriggerOptions.DATA_CHANGED;
                    if ((bda.Attribute("qchg")?.Value ?? "false").ToLower() == "true") trgOpsBda |= TriggerOptions.QUALITY_CHANGED;
                    if ((bda.Attribute("dupd")?.Value ?? "false").ToLower() == "true") trgOpsBda |= TriggerOptions.DATA_UPDATE;

                    ServerModel.ListTempDA.Last().ListDA.Add(new ServerModel.NodeDA(nameBda, null, bTypeBda, typeBda, (byte)trgOpsBda, countBda));

                    if (bda.Value != "")
                    {
                        ServerModel.ListTempDO.Last().ListDA.Last().Value = bda.Value;
                    }
                }
            }

            return true;
        }

        private bool ParseEnum(XDocument doc)
        {
            IEnumerable<XElement> xEnum = (from x in doc.Descendants()
                                           where x.Name.LocalName == "EnumType"
                                           select x).ToList();

            if (!xEnum.Any())
            {
                Logging.Log.Write("ParseDocunent.ParseEnum: EnumTypeEnumType == null", "Warning ");
                return false;
            }

            foreach (var Enum in xEnum)
            {
                if (Enum.Attribute("id") == null)
                {
                    Logging.Log.Write("ParseDocunent.ParseEnum: EnumTypeEnumType.id == null", "Warning ");
                    continue;
                }

                var nameEnum = Enum.Attribute("id")?.Value;

                ServerModel.ListEnumType.Add(new ServerModel.EnumType(nameEnum));

                IEnumerable<XElement> xEnumVal = (from x in Enum.Descendants()
                                                  where x.Name.LocalName == "EnumVal"
                                                  select x).ToList();

                if (!xEnumVal.Any())
                {
                    Logging.Log.Write("ParseDocunent.ParseEnum: EnumVal == null", "Warning ");
                    return false;
                }

                foreach (var enumVal in xEnumVal)
                {
                    var nameEnumVal = Convert.ToInt32(enumVal.Attribute("ord")?.Value);
                    var valEnumVal = enumVal.Value;

                    ServerModel.ListEnumType.Last().ListEnumVal.Add(new ServerModel.EnumType.EnumVal(nameEnumVal, valEnumVal));
                }
            }

            return true;
        }
        #endregion

        #region Объединение основных классов в объектную модель
        private bool JoinModel()
        {
            return AddDo(ServerModel.Model);
        }

        private bool AddDo(ServerModel.NodeModel model)
        {
            foreach (var ld in model.ListLD)
            {
                foreach (var ln in ld.ListLN)
                {
                    foreach (var tempLn in ServerModel.ListTempLN)
                    {
                        if (ln.LnClassLN == tempLn.LnClassLN)
                        {
                            ln.DescLN = tempLn.DescLN;
                            foreach (var tempDo in tempLn.ListDO)
                            {
                                ServerModel.NodeDO DO = new ServerModel.NodeDO(tempDo.NameDO, tempDo.TypeDO, tempDo.DescDO);

                                ln.ListDO.Add(DO);
                                AddDa(ln.ListDO);
                            }
                            break;
                        }
                    }
                }
            }

            return true;
        }

        private void AddDa(List<ServerModel.NodeDO> DO)
        {
            foreach (var tempDo in ServerModel.ListTempDO)
            {
                if (DO.Last().TypeDO == tempDo.NameDO)
                {
                    foreach (var tempDa in tempDo.ListDA)
                    {
                        ServerModel.NodeDA da = new ServerModel.NodeDA(tempDa.NameDA, tempDa.FCDA, tempDa.BTypeDA, tempDa.TypeDA, tempDa.TrgOpsDA, tempDa.CountDA)
                        {
                            Value = tempDa.Value
                        };

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


        private void AddBda(ServerModel.NodeDA listDa, string fcDa)
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

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [Flags]
    enum TriggerOptions
    {
        NONE = 0,
        DATA_CHANGED = 1,
        QUALITY_CHANGED = 2,
        DATA_UPDATE = 4,
        INTEGRITY = 8,
        GI = 16
    }
}