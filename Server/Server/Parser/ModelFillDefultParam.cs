using System;
using System.Globalization;
using System.Linq;
using Server.DataClasses;

namespace Server.Parser
{
    public partial class Parser
    {
        #region Заполнение  модели параметрами по умолчанию

        private static bool ModelFillDefultParam()
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
                                         select x).ToList()
                                .First();

                            stVal.Value = "false";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal",
                                    "bool", stVal.Value));

                            var q = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "q".ToUpper()
                                     select x).ToList()
                                .First();

                            q.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q",
                                    "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "t".ToUpper()
                                     select x).ToList()
                                .First();

                            t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t",
                                    "DateTime", t.Value));
                            continue;
                        }

                        //Целочисленное состояние
                        if (itemDo.TypeDO == "INS")
                        {
                            var stVal = (from x in itemDo.ListDA
                                         where x.NameDA.ToUpper() == "stVal".ToUpper()
                                         select x).ToList()
                                .First();

                            stVal.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal",
                                    "int", stVal.Value));

                            var q = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "q".ToUpper()
                                     select x).ToList()
                                .First();

                            q.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q",
                                    "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "t".ToUpper()
                                     select x).ToList()
                                .First();

                            t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t",
                                    "DateTime", t.Value));
                            continue;
                        }

                        #endregion

                        #region Классы общих данных для информации об измеряемой величине

                        //измеряемые значения
                        if (itemDo.TypeDO == "MV")
                        {
                            var f = (from y in (from x in itemDo.ListDA
                                                where x.TypeDA != null && x.TypeDA.ToUpper() == "AnalogueValue".ToUpper()
                                                select x).ToList()
                                    .Last()
                                    .ListDA.ToList()
                                     where y.NameDA.ToUpper() == "f".ToUpper()
                                     select y).ToList()
                                .First();

                            f.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".mag.f",
                                    "float", f.Value));

                            var siUnit = (from y in (from x in itemDo.ListDA
                                                     where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
                                                     select x).ToList()
                                    .Last()
                                    .ListDA.ToList()
                                          where y.NameDA.ToUpper() == "SIUnit".ToUpper()
                                          select y).ToList()
                                .First();

                            siUnit.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(
                                    nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".units.SIUnit", "int",
                                    siUnit.Value));

                            var multiplier = (from y in (from x in itemDo.ListDA
                                                         where x.TypeDA != null && x.TypeDA.ToUpper() == "Unit".ToUpper()
                                                         select x).ToList()
                                    .Last()
                                    .ListDA.ToList()
                                              where y.NameDA.ToUpper() == "Multiplier".ToUpper()
                                              select y).ToList()
                                .First();

                            multiplier.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(
                                    nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".units.multiplier", "int",
                                    multiplier.Value));

                            var scaleFactor = (from y in (from x in itemDo.ListDA
                                                          where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
                                                          select x).ToList()
                                    .Last()
                                    .ListDA.ToList()
                                               where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
                                               select y).ToList()
                                .First();

                            scaleFactor.Value = "1";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(
                                    nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".sVC.scaleFactor", "float",
                                    scaleFactor.Value));

                            var offset = (from y in (from x in itemDo.ListDA
                                                     where x.TypeDA != null && x.TypeDA.ToUpper() == "MagSVC".ToUpper()
                                                     select x).ToList()
                                    .Last()
                                    .ListDA.ToList()
                                          where y.NameDA.ToUpper() == "Offset".ToUpper()
                                          select y).ToList()
                                .First();

                            offset.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(
                                    nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".sVC.offset", "float",
                                    offset.Value));

                            var q = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "q".ToUpper()
                                     select x).ToList()
                                .First();

                            q.Value = "0";
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q",
                                    "ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "t".ToUpper()
                                     select x).ToList()
                                .First();

                            t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
                            DataObj.StructDataObj.Add(
                                new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t",
                                    "DateTime", t.Value));
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
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".AnalogueValue.f", "float", f.Value));

                            //var siUnit = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "Unit".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "SIUnit".ToUpper()
                            //    select y).ToList().First();

                            //siUnit.Value = "0";
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".Unit.SIUnit", "int", siUnit.Value));

                            //var multiplier = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "Unit".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "Multiplier".ToUpper()
                            //    select y).ToList().First();

                            //multiplier.Value = "0";
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".Unit.Multiplier", "int", multiplier.Value));

                            //var scaleFactor = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "MagSVC".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "ScaleFactor".ToUpper()
                            //    select y).ToList().First();

                            //scaleFactor.Value = "1";
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".MagSVC.ScaleFactor", "float", scaleFactor.Value));

                            //var offset = (from y in (from x in itemDo.ListDA
                            //        where x.TempDA != null && x.TempDA.ToUpper() == "MagSVC".ToUpper()
                            //        select x).ToList().Last().ListDA.ToList()
                            //    where y.NameDA.ToUpper() == "Offset".ToUpper()
                            //    select y).ToList().First();

                            //offset.Value = "0";
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".MagSVC.Offset", "float", offset.Value));

                            //var q = (from x in itemDo.ListDA
                            //    where x.NameDA.ToUpper() == "q".ToUpper()
                            //    select x).ToList().First();

                            //q.Value = "0";
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q", "ushort", q.Value));

                            //var t = (from x in itemDo.ListDA
                            //    where x.NameDA.ToUpper() == "t".ToUpper()
                            //    select x).ToList().First();

                            //t.Value = DateTime.Now.ToString() + DateTime.Now.Millisecond;
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(ServerModel.Model.NameModel + nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t", "DateTime", t.Value));
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
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlVal", "bool", ctlVal.Value));

                            //var stVal = (from x in itemDo.ListDA
                            //             where x.NameDA.ToUpper() == "stVal".ToUpper()
                            //             select x).ToList().Last();

                            //stVal.Value = "false";
                            //DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal","bool", stVal.Value));

                            //// ReSharper disable once UnusedVariable
                            //var ctlModel = (from x in itemDo.ListDA
                            //                where x.NameDA.ToUpper() == "ctlModel".ToUpper()
                            //                select x).ToList().Last();

                            //stVal.Value = "0";
                            //DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlModel", "int",stVal.Value));

                            //var q = (from x in itemDo.ListDA
                            //         where x.NameDA.ToUpper() == "q".ToUpper()
                            //         select x).ToList().First();

                            //q.Value = "0";
                            //DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q","ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "t".ToUpper()
                                     select x).ToList().First();

                            t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
                            DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t","DateTime", t.Value));
                            continue;
                        }

                        //Класс INC (целочисленное управление и состояние)
                        if (itemDo.TypeDO == "INC")
                        {
                            //var ctlVal = (from x in itemDo.ListDA
                            //             where x.NameDA.ToUpper() == "ctlVal".ToUpper()
                            //             select x).ToList().Last();

                            //ctlVal.Value = "false";
                            //DataObj.structDefultDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlVal", "bool", ctlVal.Value));

                            //var stVal = (from x in itemDo.ListDA
                            //             where x.NameDA.ToUpper() == "stVal".ToUpper()
                            //             select x).ToList().Last();

                            //stVal.Value = "false";
                            //DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".stVal","bool", stVal.Value));

                            //// ReSharper disable once UnusedVariable
                            //var ctlModel = (from x in itemDo.ListDA
                            //                where x.NameDA.ToUpper() == "ctlModel".ToUpper()
                            //                select x).ToList()
                            //    .Last();

                            //stVal.Value = "0";
                            //DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".ctlModel", "int",stVal.Value));

                            //var q = (from x in itemDo.ListDA
                            //         where x.NameDA.ToUpper() == "q".ToUpper()
                            //         select x).ToList().First();

                            //q.Value = "0";
                            //DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".q","ushort", q.Value));

                            var t = (from x in itemDo.ListDA
                                     where x.NameDA.ToUpper() == "t".ToUpper()
                                     select x).ToList().First();

                            t.Value = DateTime.Now.ToString(CultureInfo.CurrentCulture); // + DateTime.Now.Millisecond;
                            DataObj.StructDataObj.Add(new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".t","DateTime", t.Value));
                            continue;
                        }

                        #endregion

                        #region Спецификации класса общих данных для описательной информации

                        if (itemDo.TypeDO == "DPL")
                        {
                            var vendor = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "vendor".ToUpper()
                                select x).ToList();

                            if (vendor.Count != 0)
                            {
                                var vendorTemp = vendor.First();
                                DataObj.StructDataObj.Add(vendorTemp.Value != null
                                    ? new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string",
                                        vendorTemp.Value)
                                    : new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string", ""));
                            }

                            var hwRev = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "hwRev".ToUpper()
                                select x).ToList();

                            if (hwRev.Count != 0)
                            {
                                var hwRevTemp = hwRev.ToList().First();
                                DataObj.StructDataObj.Add(hwRevTemp.Value != null
                                    ? new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".hwRev", "string",
                                        hwRevTemp.Value)
                                    : new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".hwRev", "string", ""));
                            }

                            var swRev = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "swRev".ToUpper()
                                select x).ToList();

                            if (swRev.Count != 0)
                            {
                                var swRevTemp = swRev.First();
                                DataObj.StructDataObj.Add(swRevTemp.Value != null
                                    ? new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string",
                                        swRevTemp.Value)
                                    : new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string", ""));
                            }

                            var serNum = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "serNum".ToUpper()
                                select x).ToList();

                            if (serNum.Count != 0)
                            {
                                var serNumTemp = serNum.First();
                                DataObj.StructDataObj.Add(serNumTemp.Value != null
                                    ? new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".serNum", "string",
                                        serNumTemp.Value)
                                    : new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".serNum", "string", ""));
                            }

                            var model = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "model".ToUpper()
                                select x).ToList();

                            if (model.Count != 0)
                            {
                                var modelTemp = model.First();
                                DataObj.StructDataObj.Add(modelTemp.Value != null
                                    ? new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".model", "string",
                                        modelTemp.Value)
                                    : new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".model", "string", ""));
                            }

                            var location = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "location".ToUpper()
                                select x).ToList();

                            if (location.Count != 0)
                            {
                                var locationTemp = location.First();
                                DataObj.StructDataObj.Add(locationTemp.Value != null
                                    ? new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".location", "string",
                                        locationTemp.Value)
                                    : new DataObj.DefultDataObj(
                                        nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".location", "string", ""));
                            }
                            continue;
                        }

                        if (itemDo.TypeDO == "LPL")
                        {
                            //public string vendor;
                            //public string swRev;
                            //public string d;
                            //public string configRev;

                            var vendor = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "vendor".ToUpper()
                                select x).ToList();

                            if (vendor.Count != 0)
                            {
                                var vendorTemp = vendor.First();
                                DataObj.StructDataObj.Add(vendorTemp.Value != null
                                    ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string",vendorTemp.Value)
                                    : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".vendor", "string", ""));
                            }

                            var swRev = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "swRev".ToUpper()
                                select x).ToList();

                            if (swRev.Count != 0)
                            {
                                var swRevTemp = swRev.First();
                                DataObj.StructDataObj.Add(swRevTemp.Value != null
                                    ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string",swRevTemp.Value)
                                    : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".swRev", "string", ""));
                            }

                            var d = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "d".ToUpper()
                                select x).ToList();

                            if (d.Count != 0)
                            {
                                var dTemp = d.First();
                                DataObj.StructDataObj.Add(dTemp.Value != null
                                    ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".d", "string",dTemp.Value)
                                    : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".d", "string", ""));
                            }

                            var configRev = (from x in itemDo.ListDA
                                where x.NameDA.ToUpper() == "configRev".ToUpper()
                                select x).ToList();

                            if (configRev.Count != 0)
                            {
                                var configRevTemp = configRev.First();
                                DataObj.StructDataObj.Add(configRevTemp.Value != null
                                    ? new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".configRevTemp", "string",configRevTemp.Value)
                                    : new DataObj.DefultDataObj(nameItemLd + "/" + nameItemLn + "." + nameItemDo + ".configRevTemp", "string", ""));
                            }
                            //continue;
                        }
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