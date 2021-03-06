﻿using System;
using System.Globalization;
using System.Linq;
using ServerLib.DataClasses;
using ServerLib.Update;

namespace ServerLib.Parser
{
	public partial class Parser
	{
		#region Заполнение  модели параметрами по умолчанию

		private static bool ModelFillDefultParam()
		{
			var xxx = UpdateDataObj.SourceList;
			var ddd = UpdateDataObj.UpdateListDestination;

			foreach (var itemLd in ServerModel.Model.ListLD)
			{
				string nameItemLd = itemLd.NameLD;

				foreach (var itemLn in itemLd.ListLN)
				{
					string nameItemLn = itemLn.NameLN;

					foreach (var itemDo in itemLn.ListDO)
					{
						string nameItemDo = itemDo.NameDO;
						//Заполняем модель параметрами по-умолчанию

						switch (itemDo.TypeDO)
						{
							#region Классы общих данных для информации о состоянии
							//case "SPS":
							//	{
							//		try
							//		{
							//			var stVal = (from x in itemDo.ListDA
							//						 where x.NameDA.ToUpper() == "stVal".ToUpper()
							//						 select x).ToList().First();

							//			stVal.Value = "false";

							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "bool", stVal.Value));

							//			var q = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "q".ToUpper()
							//					 select x).ToList().First();

							//			q.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

							//			var t = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "t".ToUpper()
							//					 select x).ToList().First();

							//			t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
							//		}
							//		catch
							//		{
							//			Log.Log.Write("ModelFillDefultParam.GetDo: SPS not parse", "Error   ");
							//		}
							//		continue;
							//	}
							//case "INS":
							//	{
							//		try
							//		{
							//			var stVal = (from x in itemDo.ListDA
							//						 where x.NameDA.ToUpper() == "stVal".ToUpper()
							//						 select x).ToList().First();

							//			stVal.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "int", stVal.Value));

							//			var q = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "q".ToUpper()
							//					 select x).ToList().First();

							//			q.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

							//			var t = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "t".ToUpper()
							//					 select x).ToList().First();

							//			t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
							//		}
							//		catch
							//		{
							//			Log.Log.Write("ModelFillDefultParam.GetDo: INS not parse", "Error   ");
							//		}
							//		continue;
							//	}
							//case "ACT":
							//	{
							//		try
							//		{
							//			var general = (from x in itemDo.ListDA
							//						   where x.NameDA.ToUpper() == "general".ToUpper()
							//						   select x).ToList().First();

							//			general.Value = "false";
							//			DataObj.StructDataObj.Add(
							//				new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".general", "bool", general.Value));

							//			var q = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "q".ToUpper()
							//					 select x).ToList()
							//				.First();

							//			q.Value = "0";
							//			DataObj.StructDataObj.Add(
							//				new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q",
							//					"ushort", q.Value));

							//			var t = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "t".ToUpper()
							//					 select x).ToList()
							//				.First();

							//			t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;

							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t",
							//				"DateTime", t.Value));
							//		}
							//		catch
							//		{
							//			Log.Log.Write("ModelFillDefultParam.GetDo: ACT not parse", "Error   ");
							//		}
							//		continue;
							//	}
							//case "BCR":
							//	{
							//		try
							//		{
							//			var actVa = (from x in itemDo.ListDA
							//						 where x.NameDA.ToUpper() == "actVal".ToUpper()
							//						 select x).ToList().First();

							//			actVa.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".actVal", "int", actVa.Value));

							//			var q = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "q".ToUpper()
							//					 select x).ToList().First();

							//			q.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

							//			var t = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "t".ToUpper()
							//					 select x).ToList().First();

							//			t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
							//		}
							//		catch
							//		{
							//			Log.Log.Write("ModelFillDefultParam.GetDo: BCR not parse", "Error   ");
							//		}
							//		continue;
							//	}
							#endregion

							#region Классы общих данных для информации об измеряемой величине
							//case "MV":
							//	{
							//		var f = (from y in (from x in itemDo.ListDA
							//							where x.TypeDA != null && x.TypeDA.ToUpper() == "AnalogueValue".ToUpper()
							//							select x).ToList().Last().ListDA.ToList()
							//				 where y.NameDA.ToUpper() == "f".ToUpper()
							//				 select y).ToList().First();

							//		f.Value = "0";
							//		DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".mag.f", "float", f.Value));

							//		var siUnit = (from y in (from x in itemDo.ListDA
							//								 where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
							//								 select x).ToList().Last().ListDA.ToList()
							//					  where y.NameDA.ToUpper() == "SIUnit".ToUpper()
							//					  select y).ToList().First();

							//		siUnit.Value = "0";
							//		DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".units.SIUnit", "int", siUnit.Value));

							//		var multiplier = (from y in (from x in itemDo.ListDA
							//									 where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
							//									 select x).ToList().Last().ListDA.ToList()
							//						  where y.NameDA.ToUpper() == "Multiplier".ToUpper()
							//						  select y).ToList().First();

							//		multiplier.Value = "0";
							//		DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".units.multiplier", "int", multiplier.Value));

							//		var scaleFactor = (from y in (from x in itemDo.ListDA
							//									  where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
							//									  select x).ToList().Last().ListDA.ToList()
							//						   where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
							//						   select y).ToList().First();

							//		scaleFactor.Value = "1";
							//		DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".sVC.scaleFactor", "float", scaleFactor.Value));

							//		var offset = (from y in (from x in itemDo.ListDA
							//								 where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
							//								 select x).ToList().Last().ListDA.ToList()
							//					  where y.NameDA.ToUpper() == "Offset".ToUpper()
							//					  select y).ToList().First();

							//		offset.Value = "0";
							//		DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".sVC.offset", "float", offset.Value));

							//		var q = (from x in itemDo.ListDA
							//				 where x.NameDA.ToUpper() == "q".ToUpper()
							//				 select x).ToList().First();

							//		q.Value = "0";
							//		DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

							//		var t = (from x in itemDo.ListDA
							//				 where x.NameDA.ToUpper() == "t".ToUpper()
							//				 select x).ToList().First();

							//		t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
							//		DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
							//		continue;
							//	}
							#endregion

							#region Спецификации класса общих данных для управления состоянием и информации о состоянии
							//Класс SPC (недублированное управление и состояние)
							//case "SPC":
							//	{
							//		try
							//		{
							//			var stVal = (from x in itemDo.ListDA
							//						 where x.NameDA.ToUpper() == "stVal".ToUpper()
							//						 select x).ToList().First();

							//			stVal.Value = "false";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "bool", stVal.Value));

							//			var ctlModel = (from x in itemDo.ListDA
							//							where x.NameDA.ToUpper() == "ctlModel".ToUpper()
							//							select x).ToList().First();

							//			ctlModel.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlModel", "int", ctlModel.Value));

							//			var q = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "q".ToUpper()
							//					 select x).ToList().First();

							//			q.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

							//			var t = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "t".ToUpper()
							//					 select x).ToList().First();

							//			t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
							//		}
							//		catch
							//		{
							//			Log.Log.Write("ModelFillDefultParam.GetDo: SPC not parse", "Error   ");
							//		}
							//		continue;
							//	}
							//Класс INC (целочисленное управление и состояние)
							//case "INC":
							//	{
							//		try
							//		{
							//			var stVal = (from x in itemDo.ListDA
							//						 where x.NameDA.ToUpper() == "stVal".ToUpper()
							//						 select x).ToList().First();

							//			stVal.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "int", stVal.Value));

							//			var ctlModel = (from x in itemDo.ListDA
							//							where x.NameDA.ToUpper() == "ctlModel".ToUpper()
							//							select x).ToList().First();

							//			ctlModel.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlModel", "int", ctlModel.Value));

							//			var q = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "q".ToUpper()
							//					 select x).ToList().First();

							//			q.Value = "0";
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

							//			var t = (from x in itemDo.ListDA
							//					 where x.NameDA.ToUpper() == "t".ToUpper()
							//					 select x).ToList().First();

							//			t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
							//			DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
							//		}
							//		catch
							//		{
							//			Log.Log.Write("ModelFillDefultParam.GetDo: INC not parse", "Error   ");
							//		}
							//		continue;
							//	}


								#endregion
						}

						//Целочисленное состояние



						#region Классы общих данных для информации об измеряемой величине

						//измеряемые значения

						//комплексные измеряемые значения

						#endregion



						#region Спецификации класса общих данных для описательной информации

						//if (itemDo.TypeDO == "DPL")
						//{
						//	var vendor = (from x in itemDo.ListDA
						//				  where x.NameDA.ToUpper() == "vendor".ToUpper()
						//				  select x).ToList();

						//	if (vendor.Count != 0)
						//	{
						//		var vendorTemp = vendor.First();
						//		DataObj.StructDataObj.Add(vendorTemp.Value != null
						//		 ? new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string",
						//		  vendorTemp.Value)
						//		 : new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string", ""));
						//	}

						//	var hwRev = (from x in itemDo.ListDA
						//				 where x.NameDA.ToUpper() == "hwRev".ToUpper()
						//				 select x).ToList();

						//	if (hwRev.Count != 0)
						//	{
						//		var hwRevTemp = hwRev.ToList().First();
						//		DataObj.StructDataObj.Add(hwRevTemp.Value != null
						//		 ? new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".hwRev", "string",
						//		  hwRevTemp.Value)
						//		 : new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".hwRev", "string", ""));
						//	}

						//	var swRev = (from x in itemDo.ListDA
						//				 where x.NameDA.ToUpper() == "swRev".ToUpper()
						//				 select x).ToList();

						//	if (swRev.Count != 0)
						//	{
						//		var swRevTemp = swRev.First();
						//		DataObj.StructDataObj.Add(swRevTemp.Value != null
						//		 ? new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string",
						//		  swRevTemp.Value)
						//		 : new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string", ""));
						//	}

						//	var serNum = (from x in itemDo.ListDA
						//				  where x.NameDA.ToUpper() == "serNum".ToUpper()
						//				  select x).ToList();

						//	if (serNum.Count != 0)
						//	{
						//		var serNumTemp = serNum.First();
						//		DataObj.StructDataObj.Add(serNumTemp.Value != null
						//		 ? new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".serNum", "string",
						//		  serNumTemp.Value)
						//		 : new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".serNum", "string", ""));
						//	}

						//	var model = (from x in itemDo.ListDA
						//				 where x.NameDA.ToUpper() == "model".ToUpper()
						//				 select x).ToList();

						//	if (model.Count != 0)
						//	{
						//		var modelTemp = model.First();
						//		DataObj.StructDataObj.Add(modelTemp.Value != null
						//		 ? new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".model", "string",
						//		  modelTemp.Value)
						//		 : new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".model", "string", ""));
						//	}

						//	var location = (from x in itemDo.ListDA
						//					where x.NameDA.ToUpper() == "location".ToUpper()
						//					select x).ToList();

						//	if (location.Count != 0)
						//	{
						//		var locationTemp = location.First();
						//		DataObj.StructDataObj.Add(locationTemp.Value != null
						//		 ? new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".location", "string",
						//		  locationTemp.Value)
						//		 : new DataObj.DefultDataObj(
						//		  nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".location", "string", ""));
						//	}
						//	continue;
						//}

						//if (itemDo.TypeDO == "LPL")
						//{
						//	//public string vendor;
						//	//public string swRev;
						//	//public string d;
						//	//public string configRev;

						//	var vendor = (from x in itemDo.ListDA
						//				  where x.NameDA.ToUpper() == "vendor".ToUpper()
						//				  select x).ToList();

						//	if (vendor.Count != 0)
						//	{
						//		var vendorTemp = vendor.First();
						//		DataObj.StructDataObj.Add(vendorTemp.Value != null
						//		 ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string", vendorTemp.Value)
						//		 : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string", ""));
						//	}

						//	var swRev = (from x in itemDo.ListDA
						//				 where x.NameDA.ToUpper() == "swRev".ToUpper()
						//				 select x).ToList();

						//	if (swRev.Count != 0)
						//	{
						//		var swRevTemp = swRev.First();
						//		DataObj.StructDataObj.Add(swRevTemp.Value != null
						//		 ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string", swRevTemp.Value)
						//		 : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string", ""));
						//	}

						//	var d = (from x in itemDo.ListDA
						//			 where x.NameDA.ToUpper() == "d".ToUpper()
						//			 select x).ToList();

						//	if (d.Count != 0)
						//	{
						//		var dTemp = d.First();
						//		DataObj.StructDataObj.Add(dTemp.Value != null
						//		 ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".d", "string", dTemp.Value)
						//		 : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".d", "string", ""));
						//	}

						//	var configRev = (from x in itemDo.ListDA
						//					 where x.NameDA.ToUpper() == "configRev".ToUpper()
						//					 select x).ToList();

						//	if (configRev.Count != 0)
						//	{
						//		var configRevTemp = configRev.First();
						//		DataObj.StructDataObj.Add(configRevTemp.Value != null
						//		 ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".configRevTemp", "string", configRevTemp.Value)
						//		 : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".configRevTemp", "string", ""));
						//	}
						//	//continue;
						//}
						#endregion
					}
				}
			}

			Log.Log.Write("ModelFillDefultParam: File parse success", "Success ");
			return true;
		}
		#endregion
	}
}