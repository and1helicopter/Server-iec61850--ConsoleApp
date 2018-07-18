using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
				#region Классы общих данных для информации об измеряемой величине
				case "MV":
					try
					{
						var siUnit = 0;
						var multiplier = 0;
						var scaleFactor = 1;
						var offset = 0;
						var d = itemDo.DescDO;
						string str = null;

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Descendants().Count(x => x.Attribute("name")?.Value == "SIUnit") != 0)
									siUnit = Convert.ToInt32(xElement.Elements().Descendants().First(x => x.Attribute("name")?.Value == "SIUnit").Value);

								if (xElement.Elements().Descendants().Count(x => x.Attribute("name")?.Value == "multiplier") != 0)
									multiplier = Convert.ToInt32(xElement.Elements().Descendants().First(x => x.Attribute("name")?.Value == "multiplier").Value);

								if (xElement.Elements().Descendants().Count(x => x.Attribute("name")?.Value == "scaleFactor") != 0)
									scaleFactor = Convert.ToInt32(xElement.Elements().Descendants().First(x => x.Attribute("name")?.Value == "scaleFactor").Value);

								if (xElement.Elements().Descendants().Count(x => x.Attribute("name")?.Value == "offset") != 0)
									offset = Convert.ToInt32(xElement.Elements().Descendants().First(x => x.Attribute("name")?.Value == "offset").Value);

								if (xElement.Elements().Descendants().Count(x => x.Attribute("name")?.Value == "d") != 0)
									d = xElement.Elements().Descendants().First(x => x.Attribute("name")?.Value == "d").Value;
							}

							if (xElement.Elements().Count(x => x.Name.LocalName == "private") != 0)
							{
								str = (from x in xElement.Descendants()
									where x.Name.LocalName == "private"
									select x).First().Value;
							}
						}

						ushort count = 1;
						ushort addr = 0;

						if (str != null)
						{
							var splitStr = str.Split(';');

							count = Count(splitStr[1].Split(':')[1]);
							addr = Convert.ToUInt16(splitStr[1].Split(':')[0]);
						}

						var pathNameDo = path + "." + itemDo.NameDO;

						var mv = new MvClass
						{
							q = new Quality(),
							t = DateTime.Now,
							sVC = new ScaledValueClass
							{
								Offset = offset,
								ScaleFactor = scaleFactor
							},
							Unit = new UnitClass
							{
								Multiplier = multiplier,
								SIUnit = siUnit
							},
							d = d
						};

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = mv,
							NameDataObj = pathNameDo
						};

						if (str != null)
						{
							var source = new UpdateDataObj.SourceClassAnalog
							{
								Addr = addr,
								Count = count
							};

							UpdateDataObj.SourceList.Add(source);

							destination.AddSource(source, "mag.f");
							UpdateDataObj.UpdateListDestination.Add(destination);
						}
						
						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: MV finish whith status false", "Error   ");
						return;
					}
				#endregion

				#region Классы общих данных для информации о состоянии
				case "SPS":
					try
					{
						var stval = false;
						string str = null;
						string d = itemDo.DescDO;

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "stVal") != 0)
									stval = Convert.ToBoolean(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "stVal").Value);

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

						var sps = new SpsClass
						{
							stVal = stval,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};
						
						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = sps,
							NameDataObj = pathNameDo
						};

						var sourceList = (from x in UpdateDataObj.SourceList
								  where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
								  select x).ToList();

						//stVal
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
						Log.Log.Write("CreateClassFromAttribute.GetDo: SPS finish whith status false", "Error   ");
						return;
					}
				case "DPS":
					try
					{
						var stval = (DoublePoint)0;
						var d = itemDo.DescDO;

						string str = null;

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "stVal") != 0)
									stval = (DoublePoint) Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "stVal").Value);

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

						var dps = new DpsClass
						{
							stVal = (DoublePoint)stval,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = dps,
							NameDataObj = pathNameDo
						};

						var sourceList = (from x in UpdateDataObj.SourceList
							where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
							select x).ToList();

						//stVal
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
						Log.Log.Write("CreateClassFromAttribute.GetDo: DPS finish whith status false", "Error");
						return;
					}
				case "INS":
					try
					{
						var stval = 0;
						var d = itemDo.DescDO;

						string str = null;

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "stVal") != 0)
									stval = Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "stVal").Value);

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

						var ins = new InsClass
						{
							stVal = stval,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = ins,
							NameDataObj = pathNameDo
						};

						if (str != null)
						{
							GetAddrA(str, out ushort count, out ushort addr);

							var source = new UpdateDataObj.SourceClassAnalog
							{
								Addr = addr,
								Count = count
							};

							UpdateDataObj.SourceList.Add(source);

							destination.AddSource(source, "stVal");
							UpdateDataObj.UpdateListDestination.Add(destination);
						}

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
						string str = null;
						string phsA = null;
						string phsB = null;
						string phsC = null;
						string neut = null;

						string d = itemDo.DescDO;

						var act = new ActClass
						{
							general = false,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "general") != 0)
								{
									act.general = Convert.ToBoolean(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "general").Value);
								}

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "d") != 0)
								{
									act.d = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "d").Value;
								}
							}

							var list = xElement.Elements().Where(x => x.Name.LocalName == "private").ToList();

							if (list.Count() != 0)
							{
								str = (from x in xElement.Descendants()
									   where x.Name.LocalName == "private"
									   select x).First(x => x.Attribute("value")?.Value == "general").Value;

								if (itemDo.ListDA.Count(x => x.NameDA == "phsA") != 0)
								{
									act.phsA = false;
									if (list.Count(x => x.Attribute("value")?.Value == "phsA") != 0)
										phsA = (from x in xElement.Descendants()
											where x.Name.LocalName == "private"
											select x).First(x => x.Attribute("value")?.Value == "phsA").Value;
								}

								if (itemDo.ListDA.Count(x => x.NameDA == "phsB") != 0)
								{
									act.phsB = false;
									if (list.Count(x => x.Attribute("value")?.Value == "phsB") != 0)
										phsB = (from x in xElement.Descendants()
												where x.Name.LocalName == "private"
												select x).First(x => x.Attribute("value")?.Value == "phsB").Value;
								}

								if (itemDo.ListDA.Count(x => x.NameDA == "phsC") != 0)
								{
									act.phsC = false;
									if (list.Count(x => x.Attribute("value")?.Value == "phsC") != 0)
										phsC = (from x in xElement.Descendants()
											where x.Name.LocalName == "private"
											select x).First(x => x.Attribute("value")?.Value == "phsC").Value;
								}

								if (itemDo.ListDA.Count(x => x.NameDA == "neut") != 0)
								{
									act.neut = false;
									if (list.Count(x => x.Attribute("value")?.Value == "neut") != 0)
										neut = (from x in xElement.Descendants()
											where x.Name.LocalName == "private"
											select x).First(x => x.Attribute("value")?.Value == "neut").Value;
								}
							}
						}

						var pathNameDo = path + "." + itemDo.NameDO;
												


						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = act,
							NameDataObj = pathNameDo,
						};

						var sourceList = (from x in UpdateDataObj.SourceList
							where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
							select x).ToList();

						//phsA
						if (phsA != null)
						{
							GetAddrD(phsA, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "phsA");
						}

						//phsB
						if (phsB != null)
						{
							GetAddrD(phsB, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "phsB");
						}

						//phsC
						if (phsC != null)
						{
							GetAddrD(phsC, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "phsC");
						}

						//neut
						if (neut != null)
						{
							GetAddrD(neut, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "neut");
						}

						//general
						if (str != null)
						{
							GetAddrD(str, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "general");
							UpdateDataObj.UpdateListDestination.Add(destination);
						}

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
						string str = null;
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

						var act = new ActClass
						{
							general = false,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "general") != 0)
								{
									act.general = Convert.ToBoolean(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "general").Value);
								}

								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "d") != 0)
								{
									act.d = xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "d").Value;
								}
							}

							var list = xElement.Elements().Where(x => x.Name.LocalName == "private").ToList();

							if (list.Count() != 0)
							{
								str = (from x in xElement.Descendants()
									   where x.Name.LocalName == "private"
									   select x).First(x => x.Attribute("value")?.Value == "general").Value;

								if (itemDo.ListDA.Count(x => x.NameDA == "phsA") != 0)
								{
									act.phsA = false;
									if (list.Count(x => x.Attribute("value")?.Value == "phsA") != 0)
										phsA = (from x in xElement.Descendants()
												where x.Name.LocalName == "private"
												select x).First(x => x.Attribute("value")?.Value == "phsA").Value;
								}

								if (itemDo.ListDA.Count(x => x.NameDA == "phsB") != 0)
								{
									act.phsB = false;
									if (list.Count(x => x.Attribute("value")?.Value == "phsB") != 0)
										phsB = (from x in xElement.Descendants()
												where x.Name.LocalName == "private"
												select x).First(x => x.Attribute("value")?.Value == "phsB").Value;
								}

								if (itemDo.ListDA.Count(x => x.NameDA == "phsC") != 0)
								{
									act.phsC = false;
									if (list.Count(x => x.Attribute("value")?.Value == "phsC") != 0)
										phsC = (from x in xElement.Descendants()
												where x.Name.LocalName == "private"
												select x).First(x => x.Attribute("value")?.Value == "phsC").Value;
								}

								if (itemDo.ListDA.Count(x => x.NameDA == "neut") != 0)
								{
									act.neut = false;
									if (list.Count(x => x.Attribute("value")?.Value == "neut") != 0)
										neut = (from x in xElement.Descendants()
												where x.Name.LocalName == "private"
												select x).First(x => x.Attribute("value")?.Value == "neut").Value;
								}
							}
						}

						var pathNameDo = path + "." + itemDo.NameDO;



						var destination = new UpdateDataObj.DestinationObjectDigital
						{
							BaseClass = act,
							NameDataObj = pathNameDo,
						};

						var sourceList = (from x in UpdateDataObj.SourceList
										  where x.GetType() == typeof(UpdateDataObj.SourceClassDigital)
										  select x).ToList();

						//phsA
						if (phsA != null)
						{
							GetAddrD(phsA, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "phsA");
						}

						//phsB
						if (phsB != null)
						{
							GetAddrD(phsB, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "phsB");
						}

						//phsC
						if (phsC != null)
						{
							GetAddrD(phsC, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "phsC");
						}

						//neut
						if (neut != null)
						{
							GetAddrD(neut, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "neut");
						}

						//general
						if (str != null)
						{
							GetAddrD(str, out int index, out string addr);
							SetDestinationD(destination, sourceList, index, addr, "general");
							UpdateDataObj.UpdateListDestination.Add(destination);
						}

						UpdateDataObj.StaticListDestination.Add(destination);
						return;

					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: ACD finish whith status false", "Error");
						return;
					}
				case "BCR":
					try
					{
						var actVal = 0;
						var d = itemDo.DescDO;

						string str = null;

						if (xElement != null)
						{
							if (xElement.Attribute("name") != null)
							{
								if (xElement.Elements().Count(x => x.Attribute("name")?.Value == "actVal") != 0)
									actVal = Convert.ToInt32(xElement.Elements().ToList().First(x => x.Attribute("name")?.Value == "actVal").Value);

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

						ushort count = 1;
						ushort addr = 0;

						if (str != null)
						{
							var splitStr = str.Split(';');

							count = Count(splitStr[1].Split(':')[1]);
							addr = Convert.ToUInt16(splitStr[1].Split(':')[0]);
						}

						var pathNameDo = path + "." + itemDo.NameDO;

						var bcr = new BcrClass
						{
							actVal = actVal,
							q = new Quality(),
							t = DateTime.Now,
							d = d
						};

						var destination = new UpdateDataObj.DestinationObjectAnalog
						{
							BaseClass = bcr,
							NameDataObj = pathNameDo
						};

						if (str != null)
						{
							var source = new UpdateDataObj.SourceClassAnalog
							{
								Addr = addr,
								Count = count
							};

							UpdateDataObj.SourceList.Add(source);

							destination.AddSource(source, "actVal");
							UpdateDataObj.UpdateListDestination.Add(destination);
						}

						UpdateDataObj.StaticListDestination.Add(destination);

						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: BCR finish whith status false", "Error   ");
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