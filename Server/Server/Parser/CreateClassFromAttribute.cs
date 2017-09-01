using System;
using System.Linq;
using Server.DataClasses;
using Server.Update;

namespace Server.Parser
{
    public partial class Parser 
    {
        #region Создание обновляймых классов 
        private static void CreateClassFromAttribute()
        {
            foreach (var itemLd in ServerModel.Model.ListLD)
            {
                string pathNameLD = itemLd.NameLD;

                foreach (var itemLn in itemLd.ListLN)
                {
                    string pathNameLN = itemLn.NameLN;

                    //Если переменную класса нужно читать из памяти
                    var itemDO = (from x in itemLn.ListDO
                                   where x.Type != null
                                   select x).ToList();

                    if (itemDO.Count != 0)
                    {
                        foreach (var DO in itemDO)
                        {
                            string typeDo = DO.Type;

                            string[] tempType = typeDo.Split(';');
                            string[] tempFormatMask = tempType[0].Split(':');
                            string[] tempAddrByte = tempType[1].Split(':');

                            DO.Type = tempType[2];
                            DO.Format = tempFormatMask[0];
                            DO.Mask = Convert.ToUInt16(tempFormatMask[1]);
                            DO.Addr = Convert.ToUInt16(tempAddrByte[0]);
                            DO.Byte = Byte(tempAddrByte[1]);

                            Do(DO, pathNameLD, pathNameLN);
                        }
                    }
                }
            }
        }

        private static ushort Byte(string b)
        {
            switch (b.ToLower())
            {
                case "16b":
                    return 2;
                case "32b":
                    return 4;
                case "64b":
                    return 8;
                default:
                    return 2;
            }
        }

        private static void Do(ServerModel.NodeDO DO, string pathNameLD, string pathNameLN)
        {
            //Если переменную класса нужно читать из памяти
            if (DO.Type == "G")
            {
                GetDo(DO, pathNameLD + "/" + pathNameLN);
                return;
            }

            //Если переменную класса нужно записать в память
            if (DO.Type == "S")
            {
                GetDo(DO, pathNameLD + "/" + pathNameLN);
            }
        }


        private static void GetDo(ServerModel.NodeDO itemDo, string path)
        {
            //Проверка MV класса
            switch (itemDo.TypeDO)
            {
				#region Классы общих данных для информации об измеряемой величине
				case "MV":
					try
					{
						var pathNameDo = path + "." + itemDo.NameDO;
						var mv = new MvClass();

						var siUnit = Convert.ToInt32((from y in (from x in itemDo.ListDA
																 where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
																 select x).ToList().Last().ListDA.ToList()
													  where y.NameDA.ToUpper() == "SIUnit".ToUpper()
													  select y).ToList().Last().Value);

						var multiplier = Convert.ToInt32((from y in (from x in itemDo.ListDA
																	 where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
																	 select x).ToList().Last().ListDA.ToList()
														  where y.NameDA.ToUpper() == "Multiplier".ToUpper()
														  select y).ToList().Last().Value);

						var scaleFactor = Convert.ToSingle((from y in (from x in itemDo.ListDA
																	   where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
																	   select x).ToList().Last().ListDA.ToList()
															where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
															select y).ToList().Last().Value.Replace('.', ','));

						var offset = Convert.ToSingle((from y in (from x in itemDo.ListDA
																  where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
																  select x).ToList().Last().ListDA.ToList()
													   where y.NameDA.ToUpper() == "Offset".ToUpper()
													   select y).ToList().Last().Value.Replace('.', ','));

						if ((from y in (from x in itemDo.ListDA
										where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
										select x).ToList().Last().ListDA.ToList()
							 where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
							 select y).ToList().Last().Value == null)
						{
							scaleFactor = 1;
						}

						mv.ClassFill(siUnit, multiplier, scaleFactor, offset, itemDo.DescDO);

						UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.Byte, itemDo.TypeDO, mv);
						UpdateDataObj.DataClassGet.Add(dataObj);
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
						var pathNameDo = path + "." + itemDo.NameDO;
						var stval = itemDo.ListDA.First(x => x.NameDA.ToUpper() == "stVal".ToUpper()).Value.ToUpper() == "TRUE";
						var sps = new SpsClass(stval, itemDo.DescDO);

						UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.Byte, itemDo.TypeDO, sps);
						UpdateDataObj.DataClassGet.Add(dataObj);
						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: SPS finish whith status false", "Error   ");
						return;
					}
				case "INS":
					try
					{
						var pathNameDo = path + "." + itemDo.NameDO;
						var stval = Convert.ToInt32(itemDo.ListDA.First(x => x.NameDA.ToUpper() == "stVal".ToUpper()).Value);
						var ins = new InsClass(stval, itemDo.DescDO);

						UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.Byte, itemDo.TypeDO, ins);
						UpdateDataObj.DataClassGet.Add(dataObj);
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
						var pathNameDo = path + "." + itemDo.NameDO;

						var general = itemDo.ListDA.First(x => x.NameDA.ToUpper() == "general".ToUpper()).Value.ToUpper() == "TRUE";
						var act = new ActClass(general, itemDo.DescDO);

						UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.Byte, itemDo.TypeDO, act);
						UpdateDataObj.DataClassGet.Add(dataObj);
						return;
					}
					catch
					{
						Log.Log.Write("CreateClassFromAttribute.GetDo: ACT finish whith status false", "Error   ");
						return;
					}
				case "BCR":
					try
					{
						var pathNameDo = path + "." + itemDo.NameDO;

						var actVal = Convert.ToInt32(itemDo.ListDA.First(x => x.NameDA.ToUpper() == "actVal".ToUpper()).Value);
						var bcr = new BcrClass(actVal, itemDo.DescDO);

						UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.Byte, itemDo.TypeDO, bcr);
						UpdateDataObj.DataClassGet.Add(dataObj);
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
					{
						try
						{
							var pathNameDo = path + "." + itemDo.NameDO;
							var stval = itemDo.ListDA.First(x => x.NameDA.ToUpper() == "stVal".ToUpper()).Value.ToUpper() == @"TRUE";
							var ctlVal = itemDo.ListDA.First(x => x.NameDA.ToUpper() == "Oper".ToUpper()).ListDA.First(x => x.NameDA.ToUpper() == "ctlVal".ToUpper()).Value?.ToUpper() == @"TRUE";
							var ctlModel = itemDo.ListDA.First(x => x.NameDA.ToUpper() == "ctlModel".ToUpper()).Value;

							var spc = new SpcClass(ctlVal, stval, ctlModel, itemDo.DescDO);

							UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.Byte, itemDo.TypeDO, spc);
							UpdateDataObj.DataClassSet.Add(dataObj);
							return;
						}
						catch
						{
							Log.Log.Write("CreateClassFromAttribute.GetDo: SPC finish whith status false", "Error   ");
							return;
						}
					}
				case "INC":
					{
						try
						{
							var pathNameDo = path + "." + itemDo.NameDO;
							var stval = Convert.ToInt32(itemDo.ListDA.First(x => x.NameDA.ToUpper() == "stVal".ToUpper()).Value);
							var ctlVal = Convert.ToInt32(itemDo.ListDA.First(x => x.NameDA.ToUpper() == "Oper".ToUpper()).ListDA.First(x => x.NameDA.ToUpper() == "ctlVal".ToUpper()).Value);
							var ctlModel = itemDo.ListDA.First(x => x.NameDA.ToUpper() == "ctlModel".ToUpper()).Value;

							var inc = new IncClass(ctlVal, stval, ctlModel, itemDo.DescDO);

							UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.Byte, itemDo.TypeDO, inc);
							UpdateDataObj.DataClassSet.Add(dataObj);
							return;
						}
						catch
						{
							Log.Log.Write("CreateClassFromAttribute.GetDo: INC finish whith status false", "Error   ");
							return;
						}
					}
				#endregion
			}
		}
        #endregion
    }
}