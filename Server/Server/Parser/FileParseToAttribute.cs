using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Server.DataClasses;

namespace Server.Parser
{
    public partial class Parser
    {
        #region Заполнение объектной модели параметрами из файла
        private static bool FileParseToAttribute(XDocument doc)
        {
            if (doc.Root == null)
            {
                Log.Log.Write("FileParseToAttribute: doc.Root == null", "Warning ");
                return false;
            }

            IEnumerable<XElement> xLd = (from x in doc.Descendants()
                                         where x.Name.LocalName == "LDevice"
                                         select x).ToList();

            if (!xLd.Any())
            {
                Log.Log.Write("FileParseToAttribute: LDevice == null", "Warning ");
                return false;
            }

            foreach (var lditem in xLd)
            {
                if (lditem.Attribute("inst") == null)
                {
                    Log.Log.Write("FileParseToAttribute: LDevice.inst == null", "Warning ");
                    continue;
                }

                var ld = lditem.Attribute("inst")?.Value;

                IEnumerable<XElement> xLn = lditem.Elements().ToList();

                if (!xLn.Any())
                {
                    Log.Log.Write("FileParseToAttribute: LN == null", "Warning ");
                    return false;
                }

                foreach (var lnitem in xLn)
                {
                    if (lnitem.Attribute("lnClass") == null || lnitem.Attribute("inst") == null)
                    {
                        Log.Log.Write("FileParseToAttribute: LN.lnClass == null or LN.inst == null", "Warning ");
                        continue;
                    }
                    
                    string ln = lnitem.Attribute("prefix")?.Value + lnitem.Attribute("lnClass")?.Value + lnitem.Attribute("inst")?.Value;

                    IEnumerable<XElement> xDoi = lnitem.Elements().ToList();

                    foreach (var doiitem in xDoi)
                    {
                        if (doiitem.Attribute("name") == null)
                        {
                            Log.Log.Write("FileParseToAttribute: DO.name == null", "Warning ");
                            continue;
                        }
                        var doi = doiitem.Attribute("name")?.Value;

                        //Проверяю на собственный формат 
                        var type = (from x in doiitem.Descendants()
                                    where x.Name.LocalName == "private"
                                    select x).ToList();
                       
                        if (type.Count != 0) //Проверяю на собственный формат 
                        {
                            //Отметим объекты которые изменяются
                            try
                            {
                                var tempDo = (from z in (from y in (from x in ServerModel.Model.ListLD
                                                                    where x.NameLD.ToUpper() == ld?.ToUpper()
                                                                    select x).ToList().First().ListLN
                                                         where y.NameLN.ToUpper() == ln.ToUpper()
                                                         select y).ToList().First().ListDO
                                              where z.NameDO.ToUpper() == doi?.ToUpper()
                                              select z).ToList().First();

                                tempDo.Type = type.First().Value;
                            }
                            catch
                            {
                                Log.Log.Write("FileParseToAttribute: private DA not found", "Warning ");
                                continue;
                            }
                        }

                        ParseDefultParamBda(doiitem, ld, ln, doi, null);
                    }
                }
            }

            Log.Log.Write("FileParseToAttribute: File parse success", "Success ");
            return true;
        }

        private static void ParseDefultParamBda(XElement bdai, string ld, string ln, string doi, string dai)
        {
            IEnumerable<XElement> xDai = bdai.Elements().ToList();

            if (!xDai.Any())
            {
                Log.Log.Write("FileParseToAttribute: DA == null", "Warning ");
                return;
            }

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
                        var daitemp = dai == null ? daiitem.Attribute("name")?.Value : dai + "." + daiitem.Attribute("name")?.Value;
                        var value = daiitem.Value;
                        ParseFillModel(ld, ln, doi, daitemp, value);
                    }
                    else
                    {
                        //Если есть вложения типа DA
                        var daitemp = dai == null ? daiitem.Attribute("name")?.Value : dai + "." + daiitem.Attribute("name")?.Value;
                        ParseDefultParamBda(daiitem,  ld, ln, doi, daitemp);
                    }
                }
            }
        }

        private static void ParseFillModel( string ld, string ln, string doi, string daitemp, string value)
        {
            try
            {
                var Do = (from x in (from x in ServerModel.Model.ListLD
                        where x.NameLD == ld
                        select x.ListLN).ToList().Last().ToList()
                    where x.NameLN == ln
                    select x.ListDO).ToList().Last().ToList();

                var da = (from x in Do
                    where x.NameDO == doi
                    select x.ListDA).ToList().Last().ToList();

                var list = new List<string>(daitemp.Split('.'));

                string path = ld + "/" + ln + "." + doi;

                if (list.Count > 0)
                {
                    ParseFillModelBDA(list, da, value, path);
                }
            }
            catch
            {
                // ignored
            }
        }

        private static void ParseFillModelBDA(List<string> list, List<ServerModel.NodeDA> dai, string value, string path)
        {
            if (list.Count == 1)
            {
                var da = (from x in dai
                          where x.NameDA == list[0]
                          select x).ToList().Last();

                string btype = "string";

                //С учетом формата

                if (da.BTypeDA.ToUpper() == "INT32" || da.BTypeDA.ToUpper() == "INT")
                {
                    btype = "int";

                    try
                    {
                        da.Value = Convert.ToInt32(value).ToString();
                    }
                    catch
                    {
                        Log.Log.Write("FileParseToAttribute.ParseFillModelBDA: DA.BTypeDA.INT32", "Warning ");
                        return;
                    }
                }
                else if (da.BTypeDA.ToUpper() == "BOOL" || da.BTypeDA.ToUpper() == "BOOLEAN")
                {
                    btype = "bool";

                    try
                    {
                        da.Value = Convert.ToBoolean(value).ToString();
                    }
                    catch
                    {
                        Log.Log.Write("FileParseToAttribute.ParseFillModelBDA: DA.BTypeDA.BOOLEAN", "Warning ");
                        return;
                    }
                }
                else if (da.BTypeDA.ToUpper() == "ENUM" || da.BTypeDA.ToUpper() == "ENUMERATED")
                {
                    btype = "int";

                    try
                    {
                        da.Value = Convert.ToInt32(value).ToString();
                    }
                    catch
                    {
                        //Для ENUMERATED типов 
                        if (da.TypeDA.ToUpper() == "SIUnit".ToUpper())
                        {
                            try
                            {
                                da.Value = (from x in (from x in ServerModel.ListEnumType
                                                       where x.NameEnumType.ToUpper() == "SIUNIT".ToUpper()
                                                       select x.ListEnumVal).ToList().First()
                                            where x.ValEnumVal.ToUpper() == value.ToUpper()
                                            select x.OrdEnumVal).ToList().First().ToString();
                            }
                            catch
                            {
                                Log.Log.Write("FileParseToAttribute.ParseFillModelBDA: DA.ENUM.SIUnit", "Warning ");
                                return;
                            }

                        }

                        else if (da.TypeDA.ToUpper() == "multiplier".ToUpper())
                        {
                            try
                            {
                                da.Value = (from x in (from x in ServerModel.ListEnumType
                                                       where x.NameEnumType.ToUpper() == "multiplier".ToUpper()
                                                       select x.ListEnumVal).ToList().First()
                                            where x.ValEnumVal.ToUpper() == value.ToUpper()
                                            select x.OrdEnumVal).ToList().First().ToString();
                            }
                            catch
                            {
                                Log.Log.Write("FileParseToAttribute.ParseFillModelBDA: DA.ENUM.multiplier", "Warning ");
                                return;
                            }
                        }

                        else if (da.TypeDA.ToUpper() == "CtlModels".ToUpper())
                        {
                            try
                            {
                                da.Value = (from x in (from x in ServerModel.ListEnumType
                                                       where x.NameEnumType.ToUpper() == "CtlModels".ToUpper()
                                                       select x.ListEnumVal).ToList().First()
                                            where x.ValEnumVal.ToUpper().Replace('-', '_') == value.ToUpper().Replace('-', '_')
                                            select x.OrdEnumVal).ToList().First().ToString();
                            }
                            catch
                            {
                                Log.Log.Write("FileParseToAttribute.ParseFillModelBDA: DA.ENUM.CtlModels", "Warning ");
                                return;
                            }
                        }
                    }
                }
                else if(da.BTypeDA.ToUpper() == "FLOAT" || da.BTypeDA.ToUpper() == "FLOAT32")
                {
                    btype = "float";

                    try
                    {
                        da.Value = Convert.ToSingle(value.Replace('.',',')).ToString(CultureInfo.CurrentCulture);
                    }
                    catch
                    {
                        Log.Log.Write("FileParseToAttribute.ParseFillModelBDA: DA.BTypeDA.FLOAT", "Warning ");
                        return;
                    }
                }
                else
                {
                    da.Value = value;
                }

                var item = (from x in DataObj.StructDataObj
                            where x.Path == path + "." + list[0]
                            select x).ToList();

                if (item.Count == 0)
                {
                    DataObj.StructDataObj.Add(new DataObj.DefultDataObj(path + "." + list[0], btype, da.Value));
                }
                else
                {
                    item.First().Value = da.Value;
                    item.First().Type = btype;
                }
            }
            else
            {
                if (list.Count == 0)
                {

                    return;
                }

                var da = (from x in dai
                    where x.NameDA == list[0]
                    select x.ListDA).ToList().Last().ToList();

                path += "." + list[0];
                list.RemoveAt(0);
                ParseFillModelBDA(list, da, value, path);
            }
        }
        #endregion
    }
}