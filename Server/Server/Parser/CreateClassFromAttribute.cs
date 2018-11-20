using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using IEC61850.Common;
using ServerLib.DataClasses;
using Quality = ServerLib.DataClasses.Quality;

namespace ServerLib.Parser
{
	public partial class Parser
	{
		#region Создание обновляймых классов 
		private static bool CreateClassFromAttribute(XDocument doc)
		{
			try
			{
				if (doc.Root == null)
				{
					Log.Log.Write("FileParseToAttribute: doc.Root == null", "Warning");
					return false;
				}

				IEnumerable<XElement> xLd = (from x in doc.Descendants()
					where String.Equals(x.Name.LocalName, "LDevice", StringComparison.InvariantCultureIgnoreCase)
					select x).ToList();

				if (!xLd.Any())
				{
					Log.Log.Write("FileParseToAttribute: LDevice == null", "Warning");
					return false;
				}

				foreach (var itemLd in ServerModel.Model.ListLD)
				{
					string pathNameLD = itemLd.NameLD;

					var lditem = from x in xLd
						where String.Equals(x.Attribute("inst")?.Value, pathNameLD, StringComparison.InvariantCultureIgnoreCase)
						select x;

					var xLn = lditem.Elements().ToList();

					foreach (var itemLn in itemLd.ListLN)
					{
						string pathNameLN = itemLn.NameLN;

						if (!xLn.Any())
						{
							Log.Log.Write("FileParseToAttribute: LN == null", "Warning ");
							return false;
						}
					
						var lnitem = from x in xLn
							where x.Attribute("prefix")?.Value + x.Attribute("lnClass")?.Value + x.Attribute("inst")?.Value == pathNameLN
							select x;

						IEnumerable<XElement> xDoi = lnitem.Elements().Where(x => String.Equals(x.Name.LocalName, "DOI", StringComparison.InvariantCultureIgnoreCase)).ToList();

						foreach (var DO in itemLn.ListDO)
						{
							var nameDO = DO.NameDO;

							//Проверяю на собственный формат 

							XElement xElement = null;
							if (xDoi.Count(x => String.Equals(x.Attribute("name")?.Value, nameDO, StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								xElement = (from x in xDoi
									where String.Equals(x.Attribute("name")?.Value, nameDO, StringComparison.InvariantCultureIgnoreCase)
									select x).First();
							}

							GetDo(DO, pathNameLD + "/" + pathNameLN, xElement);
						}
					}
				}
				
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static void GetDo(ServerModel.NodeDO itemDo, string path, XElement xElement)
		{
			//Проверка классов
			switch (itemDo.TypeDO)
			{
				#region Классы общих данных для информации о состоянии
				case "SPS":
					try
					{
						string stVal = null;

						string d = itemDo.DescDO;

						var sps = new SpsClass
						{
							stVal = false,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						SetAttributeBoolean(xElement, @"stVal", ref sps.stVal);

						SetAttributeString(xElement, @"d", ref sps.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stVal);

						var pathNameDo = path + "." + itemDo.NameDO;
						
						var destination = new DataObj.DestinationObjectDigital
						{
							BaseClass = sps,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						var sourceList = (from x in DataObj.SourceList
								  where x.GetType() == typeof(DataObj.SourceClassDigital)
								  select x).ToList();

						SetAddressD(destination, sourceList, @"stVal", stVal);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SPS finish whith status false", "Error");
						return;
					}
				case "DPS":
					try
					{
						string stVal = null;

						var d = itemDo.DescDO;

						var dps = new DpsClass
						{
							stVal = 0,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						SetAttributeDoublePoint(xElement, @"stVal", ref dps.stVal);

						SetAttributeString(xElement, @"d", ref dps.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stVal);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectDigital
						{
							BaseClass = dps,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						var sourceList = (from x in DataObj.SourceList
							where x.GetType() == typeof(DataObj.SourceClassDigital)
							select x).ToList();

						SetAddressD(destination, sourceList, @"stVal", stVal);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: DPS finish whith status false", "Error");
						return;
					}
				case "INS":
					try
					{
						var d = itemDo.DescDO;

						string stVal = null;
						
						var ins = new InsClass
						{
							stVal = 0,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						SetAttributeInt32(xElement, @"stVal", ref ins.stVal);

						SetAttributeString(xElement, @"d", ref ins.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stVal);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = ins,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						SetAddressA(destination, @"stVal", stVal);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: INS finish whith status false", "Error");
						return;
					}
				case "ACT":
					try
					{
						string d = itemDo.DescDO;

						string general = null;
						string phsA = null;
						string phsB = null;
						string phsC = null;
						string neut = null;

						var act = new ActClass
						{
							general = false,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						SetAttributeBoolean(xElement, @"general", ref act.general);
						SetAttributeBoolean(xElement, @"phsA", ref act.phsA);
						SetAttributeBoolean(xElement, @"phsB", ref act.phsB);
						SetAttributeBoolean(xElement, @"phsC", ref act.phsC);
						SetAttributeBoolean(xElement, @"neut", ref act.neut);

						SetAttributeString(xElement, @"d", ref act.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"general", ref general);
						SetAddres(list, xElement, itemDo, @"phsA", ref phsA);
						SetAddres(list, xElement, itemDo, @"phsB", ref phsB);
						SetAddres(list, xElement, itemDo, @"phsC", ref phsC);
						SetAddres(list, xElement, itemDo, @"neut", ref neut);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectDigital
						{
							BaseClass = act,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						var sourceList = (from x in DataObj.SourceList
										  where x.GetType() == typeof(DataObj.SourceClassDigital)
										  select x).ToList();

						SetAddressD(destination, sourceList, @"general", general);
						SetAddressD(destination, sourceList, @"phsA", phsA);
						SetAddressD(destination, sourceList, @"phsB", phsB);
						SetAddressD(destination, sourceList, @"phsC", phsC);
						SetAddressD(destination, sourceList, @"neut", neut);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: ACT finish whith status false", "Error");
						return;
					}
				case "ACD":
					try
					{
						string general = null;
						string phsA = null;
						string phsB = null;
						string phsC = null;
						string neut = null;

						string dirGeneral = null;
						string dirPhsA = null;
						string dirPhsB = null;
						string dirPhsC = null;
						string dirNeut = null;

						string d = itemDo.DescDO;

						var acd = new AcdClass
						{
							general = false,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						SetAttributeBoolean(xElement, @"general", ref acd.general);
						SetAttributeBoolean(xElement, @"phsA", ref acd.phsA);
						SetAttributeBoolean(xElement, @"phsB", ref acd.phsB);
						SetAttributeBoolean(xElement, @"phsC", ref acd.phsC);
						SetAttributeBoolean(xElement, @"neut", ref acd.neut);

						SetAttributDirectionalProtection(xElement, @"dirGeneral", ref acd.dirGeneral);
						SetAttributDirectionalProtection(xElement, @"dirPhsA", ref acd.dirPhsA);
						SetAttributDirectionalProtection(xElement, @"dirPhsB", ref acd.dirPhsB);
						SetAttributDirectionalProtection(xElement, @"dirPhsC", ref acd.dirPhsC);
						SetAttributDirectionalProtection(xElement, @"dirNeut", ref acd.dirNeut);

						SetAttributeString(xElement, @"d", ref acd.d);

						var list = xElement.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"general", ref general);
						SetAddres(list, xElement, itemDo, @"phsA", ref phsA);
						SetAddres(list, xElement, itemDo, @"phsB", ref phsB);
						SetAddres(list, xElement, itemDo, @"phsC", ref phsC);
						SetAddres(list, xElement, itemDo, @"neut", ref neut);

						SetAddres(list, xElement, itemDo, @"dirGeneral", ref dirGeneral);
						SetAddres(list, xElement, itemDo, @"dirPhsA", ref dirPhsA);
						SetAddres(list, xElement, itemDo, @"dirPhsB", ref dirPhsB);
						SetAddres(list, xElement, itemDo, @"dirPhsC", ref dirPhsC);
						SetAddres(list, xElement, itemDo, @"dirNeut", ref dirNeut);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectDigital
						{
							BaseClass = acd,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						var sourceList = (from x in DataObj.SourceList
										  where x.GetType() == typeof(DataObj.SourceClassDigital)
										  select x).ToList();

						SetAddressD(destination, sourceList, @"general", general);
						SetAddressD(destination, sourceList, @"phsA", phsA);
						SetAddressD(destination, sourceList, @"phsB", phsB);
						SetAddressD(destination, sourceList, @"phsC", phsC);
						SetAddressD(destination, sourceList, @"neut", neut);

						SetAddressD(destination, sourceList, @"dirGeneral", dirGeneral);
						SetAddressD(destination, sourceList, @"dirPhsA", dirPhsA);
						SetAddressD(destination, sourceList, @"dirPhsB", dirPhsB);
						SetAddressD(destination, sourceList, @"dirPhsC", dirPhsC);
						SetAddressD(destination, sourceList, @"dirNeut", dirNeut);

						DataObj.UpdateListDestination.Add(destination);
						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: ACD finish whith status false", "Error");
						return;
					}
				case "SEC":
					try
					{
						var d = itemDo.DescDO;

						string cnt = null;
						string sev = null;

						var sec = new SecClass
						{
							cnt = 0,
							sev = 0,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						SetAttributeUInt32(xElement, @"cnt", ref sec.cnt);
						SetAttributeSecurityViolation(xElement, @"sev", ref sec.sev);

						SetAttributeString(xElement, @"d", ref sec.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"cnt", ref cnt);
						SetAddres(list, xElement, itemDo, @"sev", ref sev);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = sec,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						SetAddressA(destination, @"cnt", cnt);
						SetAddressA(destination, @"sev", sev);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SEC finish whith status false", "Error");
						return;
					}
				case "BCR":
					try
					{
						string actVal = null;

						var d = itemDo.DescDO;

						var bcr = new BcrClass
						{
							actVal = 0,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						SetAttributeInt64(xElement, @"actVal", ref bcr.actVal);
						SetAttributeSingle(xElement, @"pulsQty", ref bcr.pulsQty);

						SetAttributeString(xElement, @"d", ref bcr.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"actVal", ref actVal);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = bcr,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						SetAddressA(destination, @"actVal", actVal);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: BCR finish whith status false", "Error");
						return;
					}
				#endregion

				#region Классы общих данных для информации об измеряемой величине
				case "MV":
					try
					{
						var d = itemDo.DescDO;
						string mag = null;

						var mv = new MvClass
						{
							Mag = new AnalogueValueClass
							{
								f = 0
							},
							q = new Quality(),
							t = DateTime.Now,
							sVC = new ScaledValueClass
							{
								Offset = 0,
								ScaleFactor = 1
							},
							Unit = new UnitClass
							{
								Multiplier = 0,
								SIUnit = 0
							},
							d = d
						};

						if (xElement?.Attribute("name") != null)
						{
							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "units", StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								var temp = xElement.Elements().First(x => String.Equals(x.Attribute("name")?.Value, "units", StringComparison.InvariantCultureIgnoreCase));

								SetAttributeInt32(temp, @"SIUnit", ref mv.Unit.SIUnit);
								SetAttributeInt32(temp, @"multiplier", ref mv.Unit.Multiplier);
							}

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "sVC", StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								var temp = xElement.Elements().First(x => String.Equals(x.Attribute("name")?.Value, "sVC", StringComparison.InvariantCultureIgnoreCase));

								SetAttributeSingle(temp, @"scaleFactor", ref mv.sVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref mv.sVC.Offset);
							}
						}

						SetAttributeString(xElement, @"d", ref mv.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"mag", ref mag);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = mv,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						SetAddressA(destination, @"mag", mag);

						DataObj.UpdateListDestination.Add(destination);				

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: MV finish whith status false", "Error");
						return;
					}
				case "CMV":
					try
					{
						var d = itemDo.DescDO;
						string mag = null;
						string ang = null;

						var cmv = new CmvClass
						{
							cVal = new VectorClass
							{
								mag = new AnalogueValueClass
								{
									f = 0
								},
								ang = new AnalogueValueClass
								{
									f = 0
								}
							},
							magSVC = new ScaledValueClass
								{
								Offset = 0,
								ScaleFactor = 1
							},
							angSVC = new ScaledValueClass
							{
								Offset = 0,
								ScaleFactor = 1
							},
							Unit = new UnitClass
							{
								Multiplier = 0,
								SIUnit = 0
							},
							d = d
						};

						if (xElement?.Attribute("name") != null)
						{
							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "units", StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								var temp = xElement.Elements().First(x => String.Equals(x.Attribute("name")?.Value, "units", StringComparison.InvariantCultureIgnoreCase));

								SetAttributeInt32(temp, @"SIUnit", ref cmv.Unit.SIUnit);
								SetAttributeInt32(temp, @"multiplier", ref cmv.Unit.Multiplier);
							}

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "magSVC", StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								var temp = xElement.Elements().First(x => String.Equals(x.Attribute("name")?.Value, "magSVC", StringComparison.InvariantCultureIgnoreCase));

								SetAttributeSingle(temp, @"scaleFactor", ref cmv.magSVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref cmv.magSVC.Offset);
							}

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "angSVC", StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								var temp = xElement.Elements().First(x => String.Equals(x.Attribute("name")?.Value, "angSVC", StringComparison.InvariantCultureIgnoreCase));

								SetAttributeSingle(temp, @"scaleFactor", ref cmv.angSVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref cmv.angSVC.Offset);
							}
						}

						SetAttributeString(xElement, @"d", ref cmv.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();
						
						SetAddres(list, xElement, itemDo, @"mag", @"cVal", ref mag);
						SetAddres(list, xElement, itemDo, @"ang", @"cVal", ref ang);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = cmv,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						SetAddressA(destination, @"mag", mag);
						SetAddressA(destination, @"ang", ang);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: CMV finish whith status false", "Error");
						return;
					}
				case "SAV":
					try
					{
						var d = itemDo.DescDO;
						string instMag = null;

						var sav = new SavClass
						{
							instMag = new AnalogueValueClass
							{
								f = 0
							},
							q = new Quality(),
							t = DateTime.Now,
							sVC = new ScaledValueClass
							{
								Offset = 0,
								ScaleFactor = 1
							},
							Unit = new UnitClass
							{
								Multiplier = 0,
								SIUnit = 0
							},
							d = d
						};

						if (xElement?.Attribute("name") != null)
						{
							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "units", StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								var temp = xElement.Elements().First(x => String.Equals(x.Attribute("name")?.Value, "units", StringComparison.InvariantCultureIgnoreCase));

								SetAttributeInt32(temp, @"SIUnit", ref sav.Unit.SIUnit);
								SetAttributeInt32(temp, @"multiplier", ref sav.Unit.Multiplier);
							}

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "sVC", StringComparison.InvariantCultureIgnoreCase)) != 0)
							{
								var temp = xElement.Elements().First(x => String.Equals(x.Attribute("name")?.Value, "sVC", StringComparison.InvariantCultureIgnoreCase));

								SetAttributeSingle(temp, @"scaleFactor", ref sav.sVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref sav.sVC.Offset);
							}
						}

						SetAttributeString(xElement, @"d", ref sav.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"instMag", ref instMag);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = sav,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						SetAddressA(destination, @"instMag", instMag);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SAV finish whith status false", "Error");
						return;
					}
				case "WYE":
					try
					{
						var d = itemDo.DescDO;

						foreach (var item in itemDo.ListDO)
						{
							var ddd = path;

							IEnumerable<XElement> xDoi = xElement.Elements().Where(x => String.Equals(x.Name.LocalName, "DOI", StringComparison.InvariantCultureIgnoreCase)).ToList();

							var fff = xDoi.First(x=>String.Equals(x.Attribute("name")?.Value, item.NameDO, StringComparison.InvariantCultureIgnoreCase));

							GetDo(item, $"{ddd}.{itemDo.NameDO}", fff);
						}

						var wye = new WyeClass
						{
							d = d
						};
						
						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = wye,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: WYE finish whith status false", "Error");
						return;
					}
				case "DEL":
					try
					{
						var d = itemDo.DescDO;

						foreach (var item in itemDo.ListDO)
						{
							var ddd = path;

							IEnumerable<XElement> xDoi = xElement.Elements().Where(x => String.Equals(x.Name.LocalName, "DOI", StringComparison.InvariantCultureIgnoreCase)).ToList();

							var fff = xDoi.First(x => String.Equals(x.Attribute("name")?.Value, item.NameDO, StringComparison.InvariantCultureIgnoreCase));

							GetDo(item, $"{ddd}.{itemDo.NameDO}", fff);
						}

						var del = new DelClass()
						{
							d = d
						};

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = del,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: DEL finish whith status false", "Error");
						return;
					}
				case "SEQ":
					try
					{
						var d = itemDo.DescDO;

						foreach (var item in itemDo.ListDO)
						{
							var ddd = path;

							IEnumerable<XElement> xDoi = xElement.Elements().Where(x => String.Equals(x.Name.LocalName, "DOI", StringComparison.InvariantCultureIgnoreCase)).ToList();

							var fff = xDoi.First(x => String.Equals(x.Attribute("name")?.Value, item.NameDO, StringComparison.InvariantCultureIgnoreCase));

							GetDo(item, $"{ddd}.{itemDo.NameDO}", fff);
						}

						var del = new DelClass
						{
							d = d
						};

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = del,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SEQ finish whith status false", "Error");
						return;
					}
				//case "HMV":
				//	try
				//	{
				//		return;
				//	}
				//	catch
				//	{
				//		Log.Log.Write("CreateClassFromAttribute.GetDo: HMV finish whith status false", "Error");
				//		return;
				//	}
				#endregion
				
				#region Спецификации класса общих данных для управления состоянием и информации о состоянии
				case "SPC":
					try
					{
						string stval = null;
						var d = itemDo.DescDO;

						var spc = new SpcClass
						{
							stVal = false,
							ctlModel = ControlModel.STATUS_ONLY,
							ctlVal = false,
							d = d
						};

						SetAttributeBoolean(xElement, @"stVal", ref spc.stVal);
						SetAttributeCtlModel(xElement, @"CtlModels", ref spc.ctlModel);
						
						itemDo.ListDA.First(x => x.TypeDA == @"CtlModels").Value = Convert.ToInt32(spc.ctlModel).ToString();

						SetAttributeString(xElement, @"d", ref spc.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stval);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectDigital
						{
							BaseClass = spc,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						var sourceList = (from x in DataObj.SourceList
							where x.GetType() == typeof(DataObj.SourceClassDigital)
							select x).ToList();

						SetAddressD(destination, sourceList, @"stVal", stval);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SPC finish whith status false", "Error   ");
						return;
					}
				case "DPC":
					try
					{
						string stval = null;
						var d = itemDo.DescDO;

						var dpc = new DpcClass
						{
							stVal = DoublePoint.OFF,
							ctlModel = ControlModel.STATUS_ONLY,
							ctlVal = DoublePoint.OFF,
							d = d
						};

						SetAttributeDoublePoint(xElement, @"stVal", ref dpc.stVal);
						SetAttributeCtlModel(xElement, @"CtlModels", ref dpc.ctlModel);
						
						itemDo.ListDA.First(x => x.TypeDA == @"CtlModels").Value = Convert.ToInt32(dpc.ctlModel).ToString();

						SetAttributeString(xElement, @"d", ref dpc.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stval);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectDigital
						{
							BaseClass = dpc,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						var sourceList = (from x in DataObj.SourceList
							where x.GetType() == typeof(DataObj.SourceClassDigital)
							select x).ToList();

						SetAddressD(destination, sourceList, @"stVal", stval);

						DataObj.UpdateListDestination.Add(destination);
						
						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: DPC finish whith status false", "Error   ");
						return;
					}
				case "INC":
					try
					{
						string stval = null;
						var d = itemDo.DescDO;

						var inc = new IncClass
						{
							stVal = 0,
							ctlModel = ControlModel.STATUS_ONLY,
							d = d
						};
						
						SetAttributeInt32(xElement, @"stVal", ref inc.stVal);
						SetAttributeCtlModel(xElement, @"CtlModels", ref inc.ctlModel);
						
						itemDo.ListDA.First(x => x.TypeDA == @"CtlModels").Value = Convert.ToInt32(inc.ctlModel).ToString();

						SetAttributeString(xElement, @"d", ref inc.d);

						var list = xElement?.Elements().Where(x => String.Equals(x.Name.LocalName, "private", StringComparison.InvariantCultureIgnoreCase)).ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stval);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = inc,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						SetAddressA(destination, @"stVal", stval);

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: INC finish whith status false", "Error   ");
						return;
					}
				#endregion

				#region Спецификации класса общих данных для описательной информации
				case "DPL":
					try
					{
						var vendor = "";
						var hwRev = "";
						var swRev = "";
						var location = "";
						var model = "";
						var serNum = "";

						if (xElement?.Attribute("name") != null)
						{
							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "vendor", StringComparison.InvariantCultureIgnoreCase)) != 0)
								vendor = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "vendor", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "hwRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								hwRev = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "hwRev", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "swRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								swRev = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "swRev", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "location", StringComparison.InvariantCultureIgnoreCase)) != 0)
								location = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "location", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "model", StringComparison.InvariantCultureIgnoreCase)) != 0)
								model = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "model", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "serNum", StringComparison.InvariantCultureIgnoreCase)) != 0)
								serNum = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "serNum", StringComparison.InvariantCultureIgnoreCase)).Value;
						}
						else
						{
							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "vendor", StringComparison.InvariantCultureIgnoreCase)) != 0)
								vendor = itemDo.ListDA.First(x => String.Equals(x.NameDA, "vendor", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "hwRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								hwRev = itemDo.ListDA.First(x => String.Equals(x.NameDA, "hwRev", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "swRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								swRev = itemDo.ListDA.First(x => String.Equals(x.NameDA, "swRev", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "location", StringComparison.InvariantCultureIgnoreCase)) != 0)
								location = itemDo.ListDA.First(x => String.Equals(x.NameDA, "location", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "model", StringComparison.InvariantCultureIgnoreCase)) != 0)
								model = itemDo.ListDA.First(x => String.Equals(x.NameDA, "model", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "serNum", StringComparison.InvariantCultureIgnoreCase)) != 0)
								serNum = itemDo.ListDA.First(x => String.Equals(x.NameDA, "serNum", StringComparison.InvariantCultureIgnoreCase))?.Value;
						}

						var pathNameDo = path + "." + itemDo.NameDO;
						var dpl = new DplClass
						{
							vendor = vendor,
							hwRev = hwRev,
							swRev = swRev,
							location = location,
							model = model,
							serNum = serNum
						};

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = dpl,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: DPL finish whith status false", "Error   ");
						return;
					}
				case "LPL":
					try
					{
						var configRev = "";
						var swRev = "";
						var vendor = "";
						var d = "";

						if (xElement?.Attribute("name") != null)
						{
							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "configRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								configRev = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "configRev", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "swRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								swRev = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "swRev", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "vendor", StringComparison.InvariantCultureIgnoreCase)) != 0)
								vendor = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "vendor", StringComparison.InvariantCultureIgnoreCase)).Value;

							if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, "d", StringComparison.InvariantCultureIgnoreCase)) != 0)
								d = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, "d", StringComparison.InvariantCultureIgnoreCase)).Value;
						}
						else
						{
							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "configRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								configRev = itemDo.ListDA.First(x => String.Equals(x.NameDA, "configRev", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "swRev", StringComparison.InvariantCultureIgnoreCase)) != 0)
								swRev = itemDo.ListDA.First(x => String.Equals(x.NameDA, "swRev", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "vendor", StringComparison.InvariantCultureIgnoreCase)) != 0)
								vendor = itemDo.ListDA.First(x => String.Equals(x.NameDA, "vendor", StringComparison.InvariantCultureIgnoreCase))?.Value;

							if (itemDo.ListDA.Count(x => String.Equals(x.NameDA, "d", StringComparison.InvariantCultureIgnoreCase)) != 0)
								d = itemDo.ListDA.First(x => String.Equals(x.NameDA, "d", StringComparison.InvariantCultureIgnoreCase))?.Value;
						}

						var pathNameDo = path + "." + itemDo.NameDO;
						var lpl = new LplClass
						{
							configRev = configRev,
							swRev = swRev,
							vendor = vendor,
							d = d
						};

						var destination = new DataObj.DestinationObjectAnalog
						{
							BaseClass = lpl,
							NameDataObj = pathNameDo,
							IsOn = true
						};

						DataObj.UpdateListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: LPL finish whith status false", "Error");
						return;
					}
				#endregion
			}
		}

		#region SetAttribute
		private static void SetAttributeBoolean(XElement xElement, string name, ref bool? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			obj = Convert.ToBoolean(
				xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value);
		}

		private static void SetAttributeDoublePoint(XElement xElement, string name, ref DoublePoint? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			obj = (DoublePoint)Convert.ToInt32(xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value);
		}

		private static void SetAttributeInt32(XElement xElement, string name, ref int? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x =>
				    String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			obj = Convert.ToInt32(xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value);
		}

		private static void SetAttributeUInt32(XElement xElement, string name, ref uint? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x =>
				    String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			obj = Convert.ToUInt32(xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value);
		}

		private static void SetAttributDirectionalProtection(XElement xElement, string name, ref DirectionalProtection? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x =>
				    String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			obj = (DirectionalProtection)Convert.ToInt32(xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value);
		}

		private static void SetAttributeString(XElement xElement, string name, ref string obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x =>
				    String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			obj = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value;
		}

		private static void SetAttributeSecurityViolation(XElement xElement, string name, ref SecurityViolation? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			var tempValue = Convert.ToInt32(xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value);
			tempValue = tempValue > 4 ? 0 : tempValue;
			obj = (SecurityViolation)tempValue;
		}

		private static void SetAttributeInt64(XElement xElement, string name, ref Int64? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			var tempValue = Convert.ToInt64(xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value);
			obj = tempValue;
		}

		private static void SetAttributeSingle(XElement xElement, string name, ref Single? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			var val = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value.Replace('.', ',');
			var tempValue = Convert.ToSingle(val);
			obj = tempValue;
		}

		private static void SetAttributeCtlModel(XElement xElement, string name, ref ControlModel? obj)
		{
			if (xElement?.Attribute("name") == null) return;
			if (xElement.Elements().Count(x =>
				    String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
			var val = xElement.Elements().ToList().First(x => String.Equals(x.Attribute("name")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value.ToLower();
			var tempValue = new CtlModelsClass(val);
			obj = (ControlModel?)tempValue.CtlModels;
		}
		#endregion

		#region SetAddress
		private static void SetAddres(List<XElement> list, XElement xElement, ServerModel.NodeDO itemDo, string name,ref string str)
		{
			try
			{
				if (list.Count == 0) return;
				if (itemDo.ListDA.Count(x => x.NameDA == name) == 0) return;
				if (list.Count(x => String.Equals(x.Attribute("value")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
				str = (from x in xElement.Descendants()
					where x.Name.LocalName == "private"
					select x).First(x => String.Equals(x.Attribute("value")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value;
			}
			catch
			{
				//ignored
			}
		}

		private static void SetAddres(List<XElement> list, XElement xElement, ServerModel.NodeDO itemDo, string name, string nameBase, ref string str)
		{
			try
			{
				if (list.Count == 0) return;
				if (itemDo.ListDA.Count(x => x.NameDA == nameBase) == 0) return;
				if (list.Count(x => String.Equals(x.Attribute("value")?.Value, name, StringComparison.InvariantCultureIgnoreCase)) == 0) return;
				str = (from x in xElement.Descendants()
					where x.Name.LocalName == "private"
					select x).First(x => String.Equals(x.Attribute("value")?.Value, name, StringComparison.InvariantCultureIgnoreCase)).Value;
			}
			catch
			{
				//ignored
			}
		}
		#endregion

		#region SetAddressD
		private static void SetAddressD(DataObj.DestinationObjectDigital destination, List<DataObj.SourceClass> sourceList, string name, string str)
		{
			if (str == null) return;
			GetAddrD(str, out int index, out string addr);
			SetDestinationD(destination, sourceList, index, addr, name);
		}


		private static void SetAddressA(DataObj.DestinationObjectAnalog destination, string name, string str)
		{
			if (str == null) return;
			GetAddrA(str, out ushort count, out ushort addr);

			var source = new DataObj.SourceClassAnalog
			{
				Addr = addr,
				Count = count
			};

			destination.AddSource(source, name);
		}
		#endregion

		private static void SetDestinationD(DataObj.DestinationObjectDigital destination, List<DataObj.SourceClass> sourceList, int index, string addr, string value)
		{
			if (sourceList.Count(x => ((DataObj.SourceClassDigital) x).NameBitArray == addr) == 0) return;

			var source = (DataObj.SourceClassDigital)(from y in sourceList
				where ((DataObj.SourceClassDigital)y).NameBitArray == addr
				select y).First();

			destination.IndexData.Add(value, index);
			destination.AddSource(source, value);
		}

		private static void GetAddrD(string str, out int index, out string addr)
		{
			index = 0;
			addr = "";

			if (str == null) return;

			var splitStr = str.Split(';');

			index = Convert.ToInt32(splitStr[0].Split(':')[1]);
			addr = splitStr[1].Split(':')[0];
		}

		private static void GetAddrA(string str, out ushort count, out ushort addr)
		{
			count = 1;
			addr = 0;

			if (str == null) return;

			var splitStr = str.Split(';');

			count = Count(splitStr[1].Split(':')[1]);
			addr = Convert.ToUInt16(splitStr[1].Split(':')[0]);
		}

		private static ushort Count(string b)
		{
			switch (b.ToLower())
			{
				case "16b":
					return 1;
				case "32b":
					return 2;
				case "64b":
					return 4;
				default:
					return 1;
			}
		}
		#endregion
	}
}