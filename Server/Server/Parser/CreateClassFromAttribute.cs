using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ServerLib.DataClasses;
using ServerLib.Update;

namespace ServerLib.Parser
{
	public partial class Parser
	{
		#region Создание обновляймых классов 
		private static bool CreateClassFromAttribute(XDocument doc)
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

			foreach (var itemLd in ServerModel.Model.ListLD)
			{
				string pathNameLD = itemLd.NameLD;

				var lditem = from x in xLd
							 where x.Attribute("inst")?.Value == pathNameLD
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

					IEnumerable<XElement> xDoi = lnitem.Elements().Where(x => x.Name.LocalName.ToUpper() == "DOI".ToUpper()).ToList();

					foreach (var DO in itemLn.ListDO)
					{
						var nameDO = DO.NameDO;

						//Проверяю на собственный формат 

						XElement xElement = null;
						if (xDoi.Count(x => x.Attribute("name")?.Value == nameDO) != 0)
						{
							xElement = (from x in xDoi
										where x.Attribute("name")?.Value == nameDO
										select x).First();
						}

						GetDo(DO, pathNameLD + "/" + pathNameLN, xElement);
					}
				}
			}

			return true;
		}



		private static void GetDo(ServerModel.NodeDO itemDo, string path, XElement xElement)
		{
			//Проверка MV класса
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

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stVal);

						var pathNameDo = path + "." + itemDo.NameDO;
						
						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = sps,
							NameDataObj = pathNameDo
						};

						var sourceList = (from x in UpdateDataObj.SourceList
								  where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
								  select x).ToList();

						SetAddressD(destination, sourceList, @"stVal", stVal);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SPS finish whith status false", "Error   ");
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

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stVal);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = dps,
							NameDataObj = pathNameDo
						};

						var sourceList = (from x in UpdateDataObj.SourceList
							where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
							select x).ToList();

						SetAddressD(destination, sourceList, @"stVal", stVal);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

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

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"stVal", ref stVal);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = ins,
							NameDataObj = pathNameDo
						};

						SetAddressA(destination, @"stVal", stVal);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: INS finish whith status false", "Error   ");
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

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"general", ref general);
						SetAddres(list, xElement, itemDo, @"phsA", ref phsA);
						SetAddres(list, xElement, itemDo, @"phsB", ref phsB);
						SetAddres(list, xElement, itemDo, @"phsC", ref phsC);
						SetAddres(list, xElement, itemDo, @"neut", ref neut);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = act,
							NameDataObj = pathNameDo,
						};

						var sourceList = (from x in UpdateDataObj.SourceList
										  where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
										  select x).ToList();

						SetAddressD(destination, sourceList, @"general", general);
						SetAddressD(destination, sourceList, @"phsA", phsA);
						SetAddressD(destination, sourceList, @"phsB", phsB);
						SetAddressD(destination, sourceList, @"phsC", phsC);
						SetAddressD(destination, sourceList, @"neut", neut);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

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

						var list = xElement.Elements().Where(x => x.Name.LocalName == "private").ToList();

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

						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = acd,
							NameDataObj = pathNameDo,
						};

						var sourceList = (from x in UpdateDataObj.SourceList
										  where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
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

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);
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

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"cnt", ref cnt);
						SetAddres(list, xElement, itemDo, @"sev", ref sev);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = sec,
							NameDataObj = pathNameDo
						};

						SetAddressA(destination, @"cnt", cnt);
						SetAddressA(destination, @"sev", sev);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

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

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"actVal", ref actVal);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = bcr,
							NameDataObj = pathNameDo
						};

						SetAddressA(destination, @"actVal", actVal);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: BCR finish whith status false", "Error   ");
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
							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "units") != 0)
							{
								var temp = xElement.Elements().First(x => x.Attribute("name")?.Value == "units");

								SetAttributeInt32(temp, @"SIUnit", ref mv.Unit.SIUnit);
								SetAttributeInt32(temp, @"multiplier", ref mv.Unit.Multiplier);
							}

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "sVC") != 0)
							{
								var temp = xElement.Elements().First(x => x.Attribute("name")?.Value == "sVC");

								SetAttributeSingle(temp, @"scaleFactor", ref mv.sVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref mv.sVC.Offset);
							}
						}

						SetAttributeString(xElement, @"d", ref mv.d);

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"mag", ref mag);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = mv,
							NameDataObj = pathNameDo
						};

						SetAddressA(destination, @"mag", mag);

						UpdateDataObj.UpdateListDestination.Add(destination);				
						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: MV finish whith status false", "Error   ");
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
							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "units") != 0)
							{
								var temp = xElement.Elements().First(x => x.Attribute("name")?.Value == "units");

								SetAttributeInt32(temp, @"SIUnit", ref cmv.Unit.SIUnit);
								SetAttributeInt32(temp, @"multiplier", ref cmv.Unit.Multiplier);
							}

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "magSVC") != 0)
							{
								var temp = xElement.Elements().First(x => x.Attribute("name")?.Value == "magSVC");

								SetAttributeSingle(temp, @"scaleFactor", ref cmv.magSVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref cmv.magSVC.Offset);
							}

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "angSVC") != 0)
							{
								var temp = xElement.Elements().First(x => x.Attribute("name")?.Value == "angSVC");

								SetAttributeSingle(temp, @"scaleFactor", ref cmv.angSVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref cmv.angSVC.Offset);
							}
						}

						SetAttributeString(xElement, @"d", ref cmv.d);

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();
						
						SetAddres(list, xElement, itemDo, @"mag", @"cVal", ref mag);
						SetAddres(list, xElement, itemDo, @"ang", @"cVal", ref ang);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = cmv,
							NameDataObj = pathNameDo
						};

						SetAddressA(destination, @"mag", mag);
						SetAddressA(destination, @"ang", ang);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: CMV finish whith status false", "Error   ");
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
							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "units") != 0)
							{
								var temp = xElement.Elements().First(x => x.Attribute("name")?.Value == "units");

								SetAttributeInt32(temp, @"SIUnit", ref sav.Unit.SIUnit);
								SetAttributeInt32(temp, @"multiplier", ref sav.Unit.Multiplier);
							}

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "sVC") != 0)
							{
								var temp = xElement.Elements().First(x => x.Attribute("name")?.Value == "sVC");

								SetAttributeSingle(temp, @"scaleFactor", ref sav.sVC.ScaleFactor);
								SetAttributeSingle(temp, @"offset", ref sav.sVC.Offset);
							}
						}

						SetAttributeString(xElement, @"d", ref sav.d);

						var list = xElement?.Elements().Where(x => x.Name.LocalName == "private").ToList();

						SetAddres(list, xElement, itemDo, @"instMag", ref instMag);

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = sav,
							NameDataObj = pathNameDo
						};

						SetAddressA(destination, @"instMag", instMag);

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

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
							var xxx = itemDo;
							var ddd = path;

							IEnumerable<XElement> xDoi = xElement.Elements().Where(x => x.Name.LocalName.ToUpper() == "DOI".ToUpper()).ToList();

							var fff = xDoi.First(x=>x.Attribute("name")?.Value == item.NameDO);

							GetDo(item, $"{ddd}.{itemDo.NameDO}", fff);
						}

						var wye = new WyeClass
						{
							d = d
						};
						
						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = wye,
							NameDataObj = pathNameDo
						};

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

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
							var xxx = itemDo;
							var ddd = path;

							IEnumerable<XElement> xDoi = xElement.Elements().Where(x => x.Name.LocalName.ToUpper() == "DOI".ToUpper()).ToList();

							var fff = xDoi.First(x => x.Attribute("name")?.Value == item.NameDO);

							GetDo(item, $"{ddd}.{itemDo.NameDO}", fff);
						}

						var del = new DelClass()
						{
							d = d
						};

						var pathNameDo = path + "." + itemDo.NameDO;

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = del,
							NameDataObj = pathNameDo
						};

						UpdateDataObj.UpdateListDestination.Add(destination);
						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: DEL finish whith status false", "Error");
						return;
					}


				#endregion


				#region Спецификации класса общих данных для управления состоянием и информации о состоянии
				case "SPC":
					try
					{
						var stval = false;
						var ctlval = false;
						var ctlmodel = @"status-only";
						var d = itemDo.DescDO;

						string str = null;

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "stVal") != 0)
									stval = Convert.ToBoolean(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "stVal").Value);

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "ctlVal") != 0)
									ctlval = Convert.ToBoolean(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "ctlVal").Value);

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "ctlModel") != 0)
									ctlmodel = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "ctlModel").Value;

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "d") != 0)
									d = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "d").Value;

							}

							if (xElement.Elements().Count(x => x.Name.LocalName == "private") != 0)
							{
								str = (from x in xElement.Descendants()
									where x.Name.LocalName == "private"
									select x).First().Value;
							}
						}

						var pathNameDo = path + "." + itemDo.NameDO;

						var spc = new SpcClass
						{
							stVal = stval,
							ctlModel = new CtlModelsClass(ctlmodel),
							ctlVal = ctlval,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = spc,
							NameDataObj = pathNameDo,
						};

						var sourceList = (from x in UpdateDataObj.SourceList
							where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
							select x).ToList();

						if (str != null)
						{
							GetAddrD(str, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "stVal");
							UpdateDataObj.UpdateListDestination.Add(destination);
						}

						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SPC finish whith status false", "Error   ");
						return;
					}
				case "INC":
					try
					{
						var stval = 0;
						var ctlval = 0;
						var ctlmodel = @"status-only";
						var d = itemDo.DescDO;

						string str = null;

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "stVal") != 0)
									stval = Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "stVal").Value);

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "ctlVal") != 0)
									ctlval = Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "ctlVal").Value);

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "ctlModel") != 0)
									ctlmodel = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "ctlModel").Value;

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "d") != 0)
									d = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "d").Value;
							}

							if (xElement.Elements().Count(x => x.Name.LocalName == "private") != 0)
							{
								str = (from x in xElement.Descendants()
									where x.Name.LocalName == "private"
									select x).First().Value;
							}
						}

						ushort count = 0;
						ushort addr = 0;

						if (str != null)
						{
							var splitStr = str.Split(';');

							count = Count(splitStr[1].Split(':')[1]);
							addr = Convert.ToUInt16(splitStr[1].Split(':')[0]);
						}

						var pathNameDo = path + "." + itemDo.NameDO;

						var inc = new IncClass
						{
							stVal = stval,
							ctlVal = ctlval,
							ctlModel = new CtlModelsClass(ctlmodel),
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = inc,
							NameDataObj = pathNameDo
						};

						//Если указан адрес куда записывать обновленное значение
						if (str != null)
						{
							//Создаю объект храняший информацию о состояние на устройстве
							var source = new UpdateDataObj.SourceClassAnalog
							{
								Addr = addr,
								Count = count
							};

							UpdateDataObj.SourceList.Add(source);

							//Добавляю обработчик на чтение
							destination.AddSource(source, "stVal");
							UpdateDataObj.UpdateListDestination.Add(destination);
						}

						UpdateDataObj.StaticListDestination.Add(destination);

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
							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "vendor") != 0)
								vendor = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "vendor").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "hwRev") != 0)
								hwRev = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "hwRev").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "swRev") != 0)
								swRev = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "swRev").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "location") != 0)
								location = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "location").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "model") != 0)
								model = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "model").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "serNum") != 0)
								serNum = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "serNum").Value;
						}
						else
						{
							if (itemDo.ListDA.Count(x => x.NameDA == "vendor") != 0)
								vendor = itemDo.ListDA.First(x => x.NameDA == "vendor")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "hwRev") != 0)
								hwRev = itemDo.ListDA.First(x => x.NameDA == "hwRev")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "swRev") != 0)
								swRev = itemDo.ListDA.First(x => x.NameDA == "swRev")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "location") != 0)
								location = itemDo.ListDA.First(x => x.NameDA == "location")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "model") != 0)
								model = itemDo.ListDA.First(x => x.NameDA == "model")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "serNum") != 0)
								serNum = itemDo.ListDA.First(x => x.NameDA == "serNum")?.Value;
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

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = dpl,
							NameDataObj = pathNameDo
						};

						UpdateDataObj.StaticListDestination.Add(destination);

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
							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "configRev") != 0)
								configRev = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "configRev").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "swRev") != 0)
								swRev = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "swRev").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "vendor") != 0)
								vendor = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "vendor").Value;

							if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "d") != 0)
								d = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "d").Value;
						}
						else
						{
							if (itemDo.ListDA.Count(x => x.NameDA == "configRev") != 0)
								configRev = itemDo.ListDA.First(x => x.NameDA == "configRev")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "swRev") != 0)
								swRev = itemDo.ListDA.First(x => x.NameDA == "swRev")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "vendor") != 0)
								vendor = itemDo.ListDA.First(x => x.NameDA == "vendor")?.Value;

							if (itemDo.ListDA.Count(x => x.NameDA == "d") != 0)
								d = itemDo.ListDA.First(x => x.NameDA == "d")?.Value;
						}

						var pathNameDo = path + "." + itemDo.NameDO;
						var lpl = new LplClass
						{
							configRev = configRev,
							swRev = swRev,
							vendor = vendor,
							d = d
						};

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = lpl,
							NameDataObj = pathNameDo
						};

						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: LPL finish whith status false", "Error   ");
						return;
					}
				#endregion
			}
		}




		#region SetAttribute
		private static void SetAttributeBoolean(XElement xElement, string name, ref bool? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					obj = Convert.ToBoolean(
						xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value);
				}
			}
		}

		private static void SetAttributeDoublePoint(XElement xElement, string name, ref DoublePoint? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					obj = (DoublePoint)Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value);
				}
			}
		}

		private static void SetAttributeInt32(XElement xElement, string name, ref int? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					obj = Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value);
				}
			}
		}

		private static void SetAttributeUInt32(XElement xElement, string name, ref uint? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					obj = Convert.ToUInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value);
				}
			}
		}

		private static void SetAttributDirectionalProtection(XElement xElement, string name, ref DirectionalProtection? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					obj = (DirectionalProtection) Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value);
				}
			}
		}

		private static void SetAttributeString(XElement xElement, string name, ref string obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					obj = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value;
				}
			}
		}

		private static void SetAttributeSecurityViolation(XElement xElement, string name, ref SecurityViolation? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					var tempValue = Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value);
					tempValue = tempValue > 4 ? 0 : tempValue;
					obj = (SecurityViolation) tempValue;
				}
			}
		}

		private static void SetAttributeInt64(XElement xElement, string name, ref Int64? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					var tempValue = Convert.ToInt64(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value);
					obj = tempValue;
				}
			}
		}

		private static void SetAttributeSingle(XElement xElement, string name, ref Single? obj)
		{
			if (xElement?.Attribute("name") != null)
			{
				if (xElement.Elements().Count(x => x.Attribute("name")?.Value == name) != 0)
				{
					var val = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == name).Value.Replace('.', ',');
					var tempValue = Convert.ToSingle(val);
					obj = tempValue;
				}
			}
		}
		#endregion

		#region SetAddress
		private static void SetAddres(List<XElement> list, XElement xElement, ServerModel.NodeDO itemDo, string name,ref string str)
		{
			try
			{
				if (list.Count != 0)
				{
					if (itemDo.ListDA.Count(x => x.NameDA == name) != 0)
					{
						if (list.Count(x => x.Attribute("value")?.Value == name) != 0)
							str = (from x in xElement.Descendants()
								where x.Name.LocalName == "private"
								select x).First(x => x.Attribute("value")?.Value == name).Value;
					}
				}
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
				if (list.Count != 0)
				{
					if (itemDo.ListDA.Count(x => x.NameDA == nameBase) != 0)
					{
						if (list.Count(x => x.Attribute("value")?.Value == name) != 0)
							str = (from x in xElement.Descendants()
								where x.Name.LocalName == "private"
								select x).First(x => x.Attribute("value")?.Value == name).Value;
					}
				}
			}
			catch
			{
				//ignored
			}
		}
		#endregion

		#region SetAddressD
		private static void SetAddressD(UpdateDataObj.DestinationObjectDigital destination, List<UpdateDataObj.SourceClass> sourceList, string name, string str)
		{
			if (str != null)
			{
				GetAddrD(str, out int index, out string addr);
				SetDestinationD(destination, sourceList, index, addr, name);
			}
		}


		private static void SetAddressA(UpdateDataObj.DestinationObjectAnalog destination, string name, string str)
		{
			if (str != null)
			{
				GetAddrA(str, out ushort count, out ushort addr);

				var source = new UpdateDataObj.SourceClassAnalog
				{
					Addr = addr,
					Count = count
				};

				destination.AddSource(source, name);
			}
		}
		#endregion

		private static void SetDestinationD(UpdateDataObj.DestinationObjectDigital destination, List<UpdateDataObj.SourceClass> sourceList, int index, string addr, string value)
		{
			if (sourceList.Count(x => ((UpdateDataObj.SourceClassDigital)x).NameBitArray == addr) != 0)
			{
				var source = (UpdateDataObj.SourceClassDigital)(from y in sourceList
					where ((UpdateDataObj.SourceClassDigital)y).NameBitArray == addr
					select y).First();

				destination.IndexData.Add(value, index);
				destination.AddSource(source, value);
			}
		}

		private static void GetAddrD(string str, out int index, out string addr)
		{
			index = 0;
			addr = "";
			if (str != null)
			{
				var splitStr = str.Split(';');

				index = Convert.ToInt32(splitStr[0].Split(':')[1]);
				addr = splitStr[1].Split(':')[0];
			}
		}

		private static void GetAddrA(string str, out ushort count, out ushort addr)
		{
			count = 1;
			addr = 0;

			if (str != null)
			{
				var splitStr = str.Split(';');

				count = Count(splitStr[1].Split(':')[1]);
				addr = Convert.ToUInt16(splitStr[1].Split(':')[0]);
			}
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