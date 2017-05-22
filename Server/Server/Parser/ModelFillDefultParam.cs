using System;
using System.Linq;

namespace Server.Parser
{
    public partial class Parser
    {
        #region Заполнение  модели параметрами по умолчанию
        private void ModelFillDefultParam()
        {
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

                        #region Проходим по списку поддерживаемых классов
                        //Классы общих данных для информации о состоянии
                        if (itemDo.TypeDO == "SPS")
                        {
                            var stVal = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "stVal".ToUpper()
                                select x).ToList().First();

                            stVal.Value = "false";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "bool", stVal.Value));

                            var q = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "q".ToUpper()
                                select x).ToList().First();

                            q.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "t".ToUpper()
                                select x).ToList().First();

                            t.Value = DateTime.Now.ToString();// + DateTime.Now.Millisecond;
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
                            continue;
                        }

                        //Целочисленное состояние
                        if (itemDo.TypeDO == "INS")
                        {
                            var stVal = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "stVal".ToUpper()
                                select x).ToList().First();

                            stVal.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "int", stVal.Value));

                            var q = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "q".ToUpper()
                                select x).ToList().First();

                            q.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "t".ToUpper()
                                select x).ToList().First();

                            t.Value = DateTime.Now.ToString();// + DateTime.Now.Millisecond;
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
                            continue;
                        }
                        #endregion

                        #region Классы общих данных для информации об измеряемой величине
                        //измеряемые значения
                        if (itemDo.TypeDO == "MV")
                        {
                            var f = (from y in (from x in itemDo.ListDA
                                    where x.TypeDA != null && x.TypeDA.ToUpper() == "AnalogueValue".ToUpper()
                                    select x).ToList().Last().ListDA.ToList()
                                where y.NameDA.ToUpper() == "f".ToUpper()
                                select y).ToList().First();

                            f.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".mag.f", "float", f.Value));

                            var siUnit = (from y in (from x in itemDo.ListDA
                                    where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
                                    select x).ToList().Last().ListDA.ToList()
                                where y.NameDA.ToUpper() == "SIUnit".ToUpper()
                                select y).ToList().First();

                            siUnit.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".units.SIUnit", "int", siUnit.Value));

                            var multiplier = (from y in (from x in itemDo.ListDA
                                    where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
                                    select x).ToList().Last().ListDA.ToList()
                                where y.NameDA.ToUpper() == "Multiplier".ToUpper()
                                select y).ToList().First();

                            multiplier.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".units.multiplier", "int", multiplier.Value));

                            var scaleFactor = (from y in (from x in itemDo.ListDA
                                    where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
                                    select x).ToList().Last().ListDA.ToList()
                                where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
                                select y).ToList().First();

                            scaleFactor.Value = "1";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".sVC.scaleFactor", "float", scaleFactor.Value));

                            var offset = (from y in (from x in itemDo.ListDA
                                    where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
                                    select x).ToList().Last().ListDA.ToList()
                                where y.NameDA.ToUpper() == "Offset".ToUpper()
                                select y).ToList().First();

                            offset.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".sVC.offset", "float", offset.Value));

                            var q = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "q".ToUpper()
                                select x).ToList().First();

                            q.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "t".ToUpper()
                                select x).ToList().First();

                            t.Value = DateTime.Now.ToString();// + DateTime.Now.Millisecond;
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
                            continue;
                        }

                        //комплексные измеряемые значения
                        if (itemDo.TypeDO == "CMV")
                        {
                            //var f = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "AnalogueValue".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "f".ToUpper()
                            //    select y).ToList().First();

                            //f.Value = "0";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".AnalogueValue.f", "float", f.Value));

                            //var siUnit = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "Unit".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "SIUnit".ToUpper()
                            //    select y).ToList().First();

                            //siUnit.Value = "0";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".Unit.SIUnit", "int", siUnit.Value));

                            //var multiplier = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "Unit".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "Multiplier".ToUpper()
                            //    select y).ToList().First();

                            //multiplier.Value = "0";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".Unit.Multiplier", "int", multiplier.Value));

                            //var scaleFactor = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "MagSVC".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
                            //    select y).ToList().First();

                            //scaleFactor.Value = "1";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".MagSVC.ScaleFactor", "float", scaleFactor.Value));

                            //var offset = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "MagSVC".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "Offset".ToUpper()
                            //    select y).ToList().First();

                            //offset.Value = "0";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".MagSVC.Offset", "float", offset.Value));

                            //var q = (from x in itemDo.ListDA
                            //    where x.NameDA.ToUpper() == "q".ToUpper()
                            //    select x).ToList().First();

                            //q.Value = "0";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

                            //var t = (from x in itemDo.ListDA
                            //    where x.NameDA.ToUpper() == "t".ToUpper()
                            //    select x).ToList().First();

                            //t.Value = DateTime.Now.ToString() + DateTime.Now.Millisecond;
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
                            //continue;
                        }
                        #endregion

                        #region Спецификации класса общих данных для управления состоянием и информации о состоянии
                        //Класс SPC (недублированное управление и состояние)
                        if (itemDo.TypeDO == "SPC")
                        {
                            //var ctlVal = (from x in itemDo.ListDA
                            //             where x.NameDA.ToUpper() == "ctlVal".ToUpper()
                            //             select x).ToList().Last();

                            //ctlVal.Value = "false";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlVal", "bool", ctlVal.Value));

                            var stVal = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "stVal".ToUpper()
                                select x).ToList().Last();

                            stVal.Value = "false";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "bool", stVal.Value));

                            var ctlModel = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "ctlModel".ToUpper()
                                select x).ToList().Last();

                            stVal.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlModel", "int", stVal.Value));

                            var q = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "q".ToUpper()
                                select x).ToList().First();

                            q.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "t".ToUpper()
                                select x).ToList().First();

                            t.Value = DateTime.Now.ToString();// + DateTime.Now.Millisecond;
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
                            continue;
                        }

                        //Класс INC (целочисленное управление и состояние)
                        if (itemDo.TypeDO == "INC")
                        {
                            //public Int32 ctlVal;
                            //public Boolean stVal;
                            //public Int32 ctlModel;
                            //public String d;

                            //var ctlVal = (from x in itemDo.ListDA
                            //              where x.NameDA.ToUpper() == "ctlVal".ToUpper()
                            //              select x).ToList().Last();

                            //ctlVal.Value = "0";
                            //StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlVal", "int", ctlVal.Value));

                            var stVal = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "stVal".ToUpper()
                                select x).ToList().Last();

                            stVal.Value = "false";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal", "bool", stVal.Value));

                            var ctlModel = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "ctlModel".ToUpper()
                                select x).ToList().Last();

                            stVal.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlModel", "int", stVal.Value));

                            var q = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "q".ToUpper()
                                select x).ToList().First();

                            q.Value = "0";
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "t".ToUpper()
                                select x).ToList().First();

                            t.Value = DateTime.Now.ToString();// + DateTime.Now.Millisecond;
                            StructDefultDataObj.structDefultDataObj.Add(new StructDefultDataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
                            continue;
                        }
                        #endregion
                    }
                }
            }
        }
        #endregion
    }
}