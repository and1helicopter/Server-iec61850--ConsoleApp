using System;
using System.Linq;

namespace Server.Parser
{
    public partial class Parser 
    {
        #region Создание обновляймых классов 
        private void CreateClassFromAttribute()
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

                            string[] typeTempDo1 = typeDo.Split(';');
                            string[] typeTempDo2 = typeTempDo1[0].Split(':');

                            DO.Type = typeTempDo1[2];
                            DO.Format = typeTempDo2[0];
                            DO.Mask = Convert.ToUInt16(typeTempDo2[1]);
                            DO.Addr = Convert.ToUInt16(typeTempDo1[1]);

                            Do(DO, pathNameLD, pathNameLN);
                        }
                    }
                }
            }
        }

        private void Do(ServerModel.NodeDO DO, string pathNameLD, string pathNameLN)
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


        private void GetDo(ServerModel.NodeDO itemDo, string path)
        {
            //Проверка MV класса
            if (itemDo.TypeDO == "MV")
            {
                string pathNameDo = path + "." + itemDo.NameDO;
                var mv = new MvClass();

                try
                {
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

                    UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.TypeDO, mv);
                    UpdateDataObj.DataClassGet.Add(dataObj);
                    return;
                }
                catch
                {
                    Logging.Log.Write("CreateClassFromAttribute.GetDo: MV finish whith status false", "Error   ");
                    return;
                }
            }

            if (itemDo.TypeDO == "SPS")
            {
                try
                {
                    string pathNameDo = path + "." + itemDo.NameDO;
                    var sps = new SpsClass();

                    UpdateDataObj.DataObject dataObj = new UpdateDataObj.DataObject(pathNameDo, itemDo.Format, itemDo.Mask, itemDo.Addr, itemDo.TypeDO, sps);
                    UpdateDataObj.DataClassGet.Add(dataObj);
                    //return;
                }
                catch
                {
                    Logging.Log.Write("CreateClassFromAttribute.GetDo: SPS finish whith status false", "Error   ");
                    //return;
                }
            }
        }
        #endregion
    }
}