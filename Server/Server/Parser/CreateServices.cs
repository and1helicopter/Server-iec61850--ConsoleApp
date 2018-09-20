﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ServerLib.DataClasses;

namespace ServerLib.Parser
{
	public partial class Parser
	{
		private static bool CreateServices(XDocument doc)
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
				

					string ln = lnitem.Attribute("prefix")?.Value + lnitem.Attribute("lnClass")?.Value + lnitem.Attribute("inst")?.Value;

					#region DataSet
					IEnumerable<XElement> xDS = lnitem.Elements().Where(x => x.Name.LocalName.ToUpperInvariant() == "DataSet".ToUpperInvariant()).ToList();

					foreach (var dsitem in xDS)
					{
						string nameDS = dsitem.Attribute("name").Value;

						IEnumerable<XElement> xDSElements = dsitem.Elements().Where(x => x.Name.LocalName.ToUpperInvariant() == "FCD".ToUpperInvariant()
																						 || x.Name.LocalName.ToUpperInvariant() == "FCDA").ToList();

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
					IEnumerable<XElement> xRCB = lnitem.Elements().Where(x => x.Name.LocalName.ToUpperInvariant() == "ReportControl".ToUpperInvariant()).ToList();

					foreach (XElement itemrcb in xRCB)
					{
						try
						{
							string nameRCB = itemrcb.Attribute("name") != null ? itemrcb.Attribute("name").Value : "";
							string refRCB = itemrcb.Attribute("ref") != null ? itemrcb.Attribute("ref").Value : nameRCB;
							string bufferedRCB = itemrcb.Attribute("buffered") != null && itemrcb.Attribute("buffered").Value.ToLower() == "true" ? "BR" : "RP";

							bool indexed = itemrcb.Attribute("indexed") == null || itemrcb.Attribute("indexed").Value.ToLower() == "true";

							var rptEnabled = itemrcb.Elements().First(x => x.Name.LocalName.ToUpperInvariant() == "RptEnabled".ToUpperInvariant()).Attribute("max") != null
								? Convert.ToInt32(itemrcb.Elements().First(x => x.Name.LocalName.ToUpperInvariant() == "RptEnabled".ToUpperInvariant()).Attribute("max").Value) : 1;

							var rptId = itemrcb.Attribute("rptID") != null ? itemrcb.Attribute("rptID").Value : "";
							var datSet = itemrcb.Attribute("datSet") != null ? itemrcb.Attribute("datSet").Value : null; // null accepted
							var confRev = itemrcb.Attribute("confRev") != null ? uint.Parse(itemrcb.Attribute("confRev").Value) : 1;
							var bufTime = itemrcb.Attribute("bufTime") != null ? uint.Parse(itemrcb.Attribute("bufTime").Value) : 0;
							var intgPd = itemrcb.Attribute("intgPd") != null ? uint.Parse(itemrcb.Attribute("intgPd").Value) : 0;

							// <TrgOps dchg="true" qchg="false" dupd="false" period="true" />
							IEC61850.Common.TriggerOptions trgOptions = IEC61850.Common.TriggerOptions.NONE;

							XElement xTrgOps = itemrcb.Elements()
								.First(x => x.Name.LocalName.ToUpperInvariant() == "TrgOps".ToUpperInvariant());
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
								if ((xTrgOps.Attribute("gi") != null ? xTrgOps.Attribute("gi").Value : "true").ToLower() == "true"
								) // default true
									trgOptions |= IEC61850.Common.TriggerOptions.GI;
							}

							// <OptFields seqNum="true" timeStamp="true" dataSet="true" reasonCode="true" dataRef="false" entryID="true" configRef="true" bufOvfl="true" />
							IEC61850.Common.ReportOptions rptOptions = IEC61850.Common.ReportOptions.NONE;
							XElement xOptFields = itemrcb.Elements().First(x => x.Name.LocalName.ToUpperInvariant() == "OptFields".ToUpperInvariant());
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
								if ((xOptFields.Attribute("segment") != null ? xOptFields.Attribute("segment").Value : "false").ToLower() == "true")
									rptOptions |= IEC61850.Common.ReportOptions.SEGMENTATION;
							}

							string ldname = lditem.Attribute("inst")?.Value;

							string prefix = lnitem.Attribute("prefix") != null ? lnitem.Attribute("prefix").Value : "";
							string lnClass = lnitem.Attribute("lnClass") != null ? lnitem.Attribute("lnClass").Value : "";
							string lnInst = lnitem.Attribute("Inst".ToLowerInvariant()) != null ? lnitem.Attribute("Inst".ToLowerInvariant()).Value : "";
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
						catch
						{
							Log.Log.Write("FileParseToAttribute: ReportControl parse error", "Error ");
						}
					}
					#endregion

					#region LogControlBlock
					IEnumerable<XElement> xLCB = lnitem.Elements().Where(x => x.Name.LocalName.ToUpperInvariant() == "LogControl".ToUpperInvariant()).ToList();

					foreach (var itemlcb in xLCB)
					{
						var nameLCB = itemlcb.Attribute("name") != null ? itemlcb.Attribute("name").Value : "";
						var datSetLCB = itemlcb.Attribute("datSet") != null ? itemlcb.Attribute("datSet").Value : null; // null accepted
						var refLCB = itemlcb.Attribute("ref") != null ? itemlcb.Attribute("ref").Value : $"{ld}/{ln}.{nameLCB}";
						var logEnaLCB = itemlcb.Attribute("logEna") != null && (itemlcb.Attribute("logEna").Value.ToLower() == "true");

						IEC61850.Common.TriggerOptions trgOptions = IEC61850.Common.TriggerOptions.NONE;
						XElement xTrgOps = itemlcb.Elements().First(x => x.Name.LocalName.ToUpperInvariant() == "TrgOps".ToUpperInvariant());
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

						var intgPdLCB = itemlcb.Attribute("intPeriod") != null ? Convert.ToUInt32(itemlcb.Attribute("intPeriod").Value) : 0;
						var reasonCodeLCB = itemlcb.Attribute("reasonCode") != null && (itemlcb.Attribute("reasonCode").Value.ToLower() == "true");

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
	}
}
