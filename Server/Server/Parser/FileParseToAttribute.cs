using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Server.DataClasses;
using Server.Update;

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
					if (lnitem.Attribute("type")?.Value.ToUpper() == "EnergocomplektBitArray".ToUpper())
					{
						try
						{
							var nameBitArray = lnitem.Value.Split(';')[0];
							var addrBitArray = Convert.ToUInt16(lnitem.Value.Split(';')[1]);

							UpdateDataObj.BitArray.Add(new UpdateDataObj.BitArrayObj(nameBitArray, 0));
							UpdateDataObj.ClassGetObjects.Add(new UpdateDataObj.GetObject(addrBitArray, 2, true)
							{
								BitArray = UpdateDataObj.BitArray.Last()
							});

							continue;
						}
						catch
						{
							Log.Log.Write("FileParseToAttribute: LN.type == EnergocomplektBitArray", "Warning ");
							continue;
						}
					}
					if (lnitem.Attribute("lnClass") == null || lnitem.Attribute("inst") == null)
					{
						Log.Log.Write("FileParseToAttribute: LN.lnClass == null or LN.inst == null", "Warning ");
						continue;
					}

					string ln = lnitem.Attribute("prefix")?.Value + lnitem.Attribute("lnClass")?.Value + lnitem.Attribute("inst")?.Value;

					#region DOI
					IEnumerable<XElement> xDoi = lnitem.Elements().Where(x => x.Name.LocalName.ToUpper() == "DOI".ToUpper()).ToList();

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
					#endregion

					#region DataSet
					IEnumerable<XElement> xDS = lnitem.Elements().Where(x => x.Name.LocalName.ToUpper() == "DataSet".ToUpper()).ToList();

					foreach (var dsitem in xDS)
					{
						string nameDS = dsitem.Attribute("name").Value;

						IEnumerable<XElement> xDSElements = dsitem.Elements().Where(x => x.Name.LocalName.ToUpper() == "FCD".ToUpper()
																						 || x.Name.LocalName.ToUpper() == "FCDA").ToList();

						foreach (var dselement in xDSElements)
						{
							string prefix = dselement.Attribute("prefix") != null ? dselement.Attribute("prefix").Value : "";
							string lnClass = dselement.Attribute("lnClass") != null ? dselement.Attribute("lnClass").Value : "";
							string lnInst = dselement.Attribute("lnInst") != null ? dselement.Attribute("lnInst").Value : "";
							string fullName = String.Concat(prefix, lnClass, lnInst);

							//string ldInst = dselement.Attribute("ldInst") != null ? dselement.Attribute("ldInst").Value : "";
							string doName = dselement.Attribute("doName") != null ? dselement.Attribute("doName").Value : "";
							string daName = dselement.Attribute("daName") != null ? @"$" + dselement.Attribute("daName").Value : "";
							string fc = dselement.Attribute("fc") != null ? dselement.Attribute("fc").Value : "";

							string ldname = lditem.Attribute("inst")?.Value;

							string memberName = fullName + @"$" + fc + @"$" + doName + daName;
							//ied + ldname + "/" + 

							string prefix2 = lnitem.Attribute("prefix") != null ? lnitem.Attribute("prefix").Value : "";
							string lnClass2 = lnitem.Attribute("lnClass") != null ? lnitem.Attribute("lnClass").Value : "";
							string lnInst2 = lnitem.Attribute("inst") != null ? lnitem.Attribute("inst").Value : "";
							string lnname = String.Concat(prefix2, lnClass2, lnInst2);

							if (ServerModel.Model.ListLD.First(x => x.NameLD == ldname).ListLN
									.First(y => y.NameLN == fullName).ListDO
									.First(z => z.NameDO == doName) != null)
							{
								if (ServerModel.Model.ListLD.First(x => x.NameLD == ldname).ListLN
										.First(y => y.NameLN == lnname).ListDS
										.Find(z => z.DSName == nameDS) != null)
								{
									ServerModel.Model.ListLD.First(x => x.NameLD == ldname).ListLN
										.First(y => y.NameLN == lnname).ListDS
										.First(z => z.DSName == nameDS).DSMemberRef.Add(memberName);
								}
								else
								{
									ServerModel.Model.ListLD.First(x => x.NameLD == ldname).ListLN
										.First(y => y.NameLN == lnname).ListDS.Add(new ServerModel.DataSet(nameDS, null));

									ServerModel.Model.ListLD.First(x => x.NameLD == ldname).ListLN
										.First(y => y.NameLN == lnname).ListDS
										.First(z => z.DSName == nameDS).DSMemberRef.Add(memberName);
								}
							}
						}
					}
					#endregion

					#region ReportControl
					IEnumerable<XElement> xRCB = lnitem.Elements().Where(x => x.Name.LocalName.ToUpper() == "ReportControl".ToUpper()).ToList();

					foreach (XElement itemrcb in xRCB)
					{
						string nameRCB = itemrcb.Attribute("name") != null ? itemrcb.Attribute("name") .Value: "";
						string refRCB = itemrcb.Attribute("ref") != null ? itemrcb.Attribute("ref").Value : nameRCB;
						string bufferedRCB = itemrcb.Attribute("buffered") != null && itemrcb.Attribute("buffered").Value.ToLower() == "true" ? "BR" : "RP";

						bool indexed = itemrcb.Attribute("indexed") == null || itemrcb.Attribute("indexed").Value.ToLower() == "true";

						var rptEnabled =itemrcb.Elements().First(x => x.Name.LocalName.ToUpper() == "RptEnabled".ToUpper()).Attribute("max") != null
							? Convert.ToInt32(itemrcb.Elements().First(x => x.Name.LocalName.ToUpper() == "RptEnabled".ToUpper()).Attribute("max").Value): 1;

						var rptId = itemrcb.Attribute("rptID") != null ? itemrcb.Attribute("rptID").Value : "";
						var datSet = itemrcb.Attribute("datSet") != null ? itemrcb.Attribute("datSet").Value : null; // null accepted
						var confRev = itemrcb.Attribute("confRev") != null ? uint.Parse(itemrcb.Attribute("confRev").Value) : 1;
						var bufTime = itemrcb.Attribute("bufTime") != null ? uint.Parse(itemrcb.Attribute("bufTime").Value): 0;
						var intgPd = itemrcb.Attribute("intgPd") != null ? uint.Parse(itemrcb.Attribute("intgPd").Value) : 0;

						// <TrgOps dchg="true" qchg="false" dupd="false" period="true" />
						IEC61850.Common.TriggerOptions trgOptions = IEC61850.Common.TriggerOptions.NONE;
						XElement xTrgOps = itemrcb.Elements().First(x => x.Name.LocalName.ToUpper() == "TrgOps".ToUpper());
						if (xTrgOps != null)
						{
							if ((xTrgOps.Attribute("dchg")!= null ? xTrgOps.Attribute("dchg").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.DATA_CHANGED;
							if ((xTrgOps.Attribute("qchg") != null ? xTrgOps.Attribute("qchg").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.QUALITY_CHANGED;
							if ((xTrgOps.Attribute("dupd") != null ? xTrgOps.Attribute("dupd").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.DATA_UPDATE;
							if ((xTrgOps.Attribute("period") != null ? xTrgOps.Attribute("period").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.INTEGRITY;
							if ((xTrgOps.Attribute("gi") != null ? xTrgOps.Attribute("gi").Value : "true").ToLower() == "true") // default true
								trgOptions |= IEC61850.Common.TriggerOptions.GI;
						}

						// <OptFields seqNum="true" timeStamp="true" dataSet="true" reasonCode="true" dataRef="false" entryID="true" configRef="true" bufOvfl="true" />
						IEC61850.Common.ReportOptions rptOptions = IEC61850.Common.ReportOptions.NONE;
						XElement xOptFields = itemrcb.Elements().First(x => x.Name.LocalName.ToUpper() == "OptFields".ToUpper());
						if (xOptFields != null)
						{
							if ((xOptFields.Attribute("seqNum") != null ? xOptFields.Attribute("seqNum").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.SEQ_NUM;
							if ((xOptFields.Attribute("timeStamp") != null ? xOptFields.Attribute("timeStamp").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.TIME_STAMP;
							if ((xOptFields.Attribute("dataSet") != null ? xOptFields.Attribute("dataSet").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.DATA_SET;
							if ((xOptFields.Attribute("reasonCode") != null ? xOptFields.Attribute("reasonCode").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.REASON_FOR_INCLUSION;
							if ((xOptFields.Attribute("dataRef") != null ? xOptFields.Attribute("dataRef").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.DATA_REFERENCE;
							if ((xOptFields.Attribute("entryID") != null ? xOptFields.Attribute("entryID").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.ENTRY_ID;
							if ((xOptFields.Attribute("configRef") != null ? xOptFields.Attribute("configRef").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.CONF_REV;
							if ((xOptFields.Attribute("bufOvfl") != null ? xOptFields.Attribute("bufOvfl").Value : "false").ToLower() == "true")
								rptOptions |= IEC61850.Common.ReportOptions.BUFFER_OVERFLOW;
						}
						 
						string ldname = lditem.Attribute("inst")?.Value;

						string prefix = lnitem.Attribute("prefix") != null ? lnitem.Attribute("prefix").Value : "";
						string lnClass = lnitem.Attribute("lnClass") != null ? lnitem.Attribute("lnClass").Value : "";
						string lnInst = lnitem.Attribute("lnInst") != null ? lnitem.Attribute("lnInst").Value : "";
						string fullName = String.Concat(prefix, lnClass, lnInst);

						for (int i = 0; i < rptEnabled; i++)
						{
							try
							{
								if (ServerModel.Model.ListLD.First(x => x.NameLD == ldname).ListLN
									    .First(y => y.NameLN == fullName) != null)
								{
									ServerModel.Model.ListLD.First(x => x.NameLD == ldname).ListLN
										.First(y => y.NameLN == fullName).ListRCB.Add(new ServerModel.RCB(nameRCB + (indexed ? (i + 1).ToString("D2") : ""), refRCB, rptOptions, trgOptions, bufferedRCB, rptId, datSet, confRev, bufTime, intgPd));
								}
							}
							catch 
							{
								Log.Log.Write("FileParseToAttribute: ReportControl parse error", "Error ");
							}
						}
					}
					#endregion

					#region LogControlBlock
					IEnumerable<XElement> xLCB = lnitem.Elements().Where(x => x.Name.LocalName.ToUpper() == "LogControl".ToUpper()).ToList();

					foreach (var itemlcb in xLCB)
					{
						var nameLCB = itemlcb.Attribute("name") != null ? itemlcb.Attribute("name") .Value: "";
						var datSetLCB = itemlcb.Attribute("datSet") != null ? itemlcb.Attribute("datSet").Value : null; // null accepted
						var refLCB = itemlcb.Attribute("ref") != null ? itemlcb.Attribute("ref").Value : $"{ld}/{ln}.{nameLCB}";
						var logEnaLCB = itemlcb.Attribute("logEna") != null && (itemlcb.Attribute("logEna").Value.ToLower() == "true");

						IEC61850.Common.TriggerOptions trgOptions = IEC61850.Common.TriggerOptions.NONE;
						XElement xTrgOps = itemlcb.Elements().First(x => x.Name.LocalName.ToUpper() == "TrgOps".ToUpper());
						if (xTrgOps != null)
						{
							if ((xTrgOps.Attribute("dchg") != null ? xTrgOps.Attribute("dchg").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.DATA_CHANGED;
							if ((xTrgOps.Attribute("qchg") != null ? xTrgOps.Attribute("qchg").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.QUALITY_CHANGED;
							if ((xTrgOps.Attribute("dupd") != null ? xTrgOps.Attribute("dupd").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.DATA_UPDATE;
							if ((xTrgOps.Attribute("period") != null ? xTrgOps.Attribute("period").Value : "false").ToLower() == "true")
								trgOptions |= IEC61850.Common.TriggerOptions.INTEGRITY;
							if ((xTrgOps.Attribute("gi") != null ? xTrgOps.Attribute("gi").Value : "true").ToLower() == "true") // default true
								trgOptions |= IEC61850.Common.TriggerOptions.GI;
						}

						var intgPdLCB =  itemlcb.Attribute("intPeriod") != null ?  Convert.ToUInt32(itemlcb.Attribute("intPeriod") .Value): 0;
						var reasonCodeLCB = itemlcb.Attribute("reasonCode") != null  && (itemlcb.Attribute("reasonCode").Value.ToLower() == "true");

						try
						{
							if (ServerModel.Model.ListLD.First(x => x.NameLD == ld).ListLN.First(y => y.NameLN == $"{ln}") != null)
							{
								ServerModel.Model.ListLD.First(x => x.NameLD == ld).ListLN
									.First(y => y.NameLN == $"{ln}").ListLCB.Add(new ServerModel.LCB(nameLCB, refLCB, logEnaLCB, datSetLCB, trgOptions, intgPdLCB, reasonCodeLCB));
							}
						}
						catch 
						{
							Log.Log.Write("FileParseToAttribute: LogControlBlock parse error", "Error ");
						}
					}
					#endregion

					#region Log

					#endregion

					#region SettingGroupControlBlock

					#endregion
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

				if (da.BTypeDA.ToUpper() == "Unicode255".ToUpper() || da.BTypeDA.ToUpper() == "VisString255".ToUpper())
				{
					btype = "string";

					try
					{
						da.Value = value;
					}
					catch
					{
						Log.Log.Write("FileParseToAttribute.ParseFillModelBDA: DA.BTypeDA.INT32", "Warning ");
						return;
					}
				}
				else if (da.BTypeDA.ToUpper() == "INT32" || da.BTypeDA.ToUpper() == "INT" || da.BTypeDA.ToUpper() == "INT32U" || da.BTypeDA.ToUpper() == "UINT")
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