using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IEC61850.Common;
using ServerLib.DataClasses;

namespace ServerLib.Parser
{
	public partial class Parser 
	{
		#region Сохранение объектной модели в конфигурациионную модель для сервера
		private static void SaveFileConfig()
		{
			string savePath = "model.cfg";
			FileStream fs = new FileStream(savePath, FileMode.Create);

			string str = $"MODEL({ServerModel.Model.NameModel}){{\n";
			var array = System.Text.Encoding.Default.GetBytes(str);
			fs.Write(array, 0, array.Length);

			SaveLd(fs, ServerModel.Model.ListLD);

			str = "}\n";
			array = System.Text.Encoding.Default.GetBytes(str);
			fs.Write(array, 0, array.Length);

			fs.Close();
		}

		private static void SaveLd(FileStream fs, List<ServerModel.NodeLD> listLd)
		{
			foreach (var ld in listLd)
			{
				// Syntax: LD(<logical device name>){…}
				string str = $"LD({ld.NameLD}){{\n";
				var array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);

				SaveLn(fs, ld.ListLN);

				str = "}\n";
				array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);
			}
		}

		private static void SaveLn(FileStream fs, List<ServerModel.NodeLN> listLn)
		{
			foreach (var ln in listLn)
			{
				// Syntax: LN(<logical node name>){…}
				string str = $"LN({ln.NameLN}){{\n";
				var array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);

				SaveDs(fs, ln.ListDS);
				SaveRCB(fs, ln.ListRCB);
				SaveLCB(fs, ln.ListLCB);
				
				SaveDo(fs, ln.ListDO);

				str = "}\n";
				array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);
			}
		}

		private static void SaveDs(FileStream fs, List<ServerModel.DataSet> listDs)
		{
			// DATASET Syntax: DS(<DataSet name>) { DE(<DataSet item>); ...}

			foreach (var ds in listDs)
			{
				string str = $"DS({ds.DSName}){{\n";
				var array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);

				foreach (var itemds in ds.DSMemberRef)
				{
					str = $"DE({itemds});\n";
					array = System.Text.Encoding.Default.GetBytes(str);
					fs.Write(array, 0, array.Length);
				}

				str = "}\n";
				array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);
			}
		}
		private static void SaveRCB(FileStream fs, List<ServerModel.RCB> lnListRCB)
		{
			// Report Control Block Syntax: RC(<RCB name> <RCB DataSet> <> <> <> <> <>);

			foreach (var rcb in lnListRCB)
			{
				var rcbOptions = (ushort)rcb.RCBrptOptions;
				var trgOptions = (ushort)rcb.RCBtrgOptions;
				var rcbbuffered = rcb.RCBbuffered == "BR" ? "1" : "0";

				string str = $"RC({rcb.RCBName} {rcb.RCBRef} {rcbbuffered} {rcb.RCBdatSet} {rcb.RCBconfRev} {trgOptions} {rcbOptions}  {rcb.RCBbufTime} {rcb.RCBintgPd});\n";
				var array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);
			}
		}

		private static void SaveLCB(FileStream fs, List<ServerModel.LCB> lnListLCB)
		{
			//LCB Syntax: LC(<LCB name> <LCB DataSet> <LCB Ref> <LCB trgOptions> <> <> <>);

			foreach (var lcb in lnListLCB)
			{
				//LC(EventLog Events GenericIO/LLN0$EventLog 19 0 0 1);
				var trgOptions = (ushort)lcb.LCBtrgOptions;
				var logEna = lcb.LCBLogEna ? 1 : 0;
				var reasonCode = lcb.LCBreasonCode ? 1 : 0;


				string str = $"LC({lcb.LCBName} {lcb.LCBDatSet} {lcb.LCBRef} {trgOptions} {lcb.LCBintgPd} {reasonCode} {logEna});\n";
				var array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);
			}
		}

		private static void SaveDo(FileStream fs, List<ServerModel.NodeDO> listDo)
		{
			foreach (var DO in listDo)
			{
				// Syntax: DO(<data object name> <nb of array elements>){…}
				string str = $"DO({DO.NameDO} {0}){{\n";
				var array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);

				SaveDa(fs, DO.ListDA);
				SaveDo(fs, DO.ListDO);

				str = "}\n";
				array = System.Text.Encoding.Default.GetBytes(str);
				fs.Write(array, 0, array.Length);
			}
		}

		private static void SaveDa(FileStream fs, List<ServerModel.NodeDA> listDa)
		{
			// DA(<data attribute name> <nb of array elements> <type> <FC> <trigger options> <sAddr>)[=value];
			// Constructed>
			// DA(<data attribute name> <nb of array elements> 27 <FC> <trigger options> <sAddr>){…}
			foreach (var da in listDa)
			{
				string str;
				byte[] array;
				if (da.ListDA.Count == 0)
				{   //		Name				Type Data					FC		
					//DA (<NameDA><nrElem><MapLibiecType(DataType)><Functional Constrant)><MapTrgOps()><sAddr>)

					str = $"DA({da.NameDA} {0} {(int)da.BTypeDA} {(int)da.FCDA} {(int)da.TrgOpsDA} {0})";
					if (da.NameDA == "ctlModel")
					{
						try
						{
							var lol = (from x in ServerModel.ListEnumType
									   where x.NameEnumType.ToUpperInvariant() == "CtlModels".ToUpperInvariant()
									   select x).ToList().First();

							var locsld = (from x in lol.ListEnumVal
										  where x.OrdEnumVal == Convert.ToInt32(da.Value)
										  select x).ToList().First();

							str += " =" + locsld.OrdEnumVal + "\n";
						}
						catch
						{
							Log.Log.Write("SaveFileConfig: Enum \"CtlModels\" not found", "Warning ");
							str += " = 0\n";
						}
					}
					else
					{
						str += "\n";
					}
					array = System.Text.Encoding.Default.GetBytes(str);
					fs.Write(array, 0, array.Length);
				}
				else
				{
					str = $"DA({da.NameDA} {0} {(int)da.BTypeDA} {(int)da.FCDA} {(int)da.TrgOpsDA} {0}){{\n";
					array = System.Text.Encoding.Default.GetBytes(str);
					fs.Write(array, 0, array.Length);

					SaveDa(fs, da.ListDA);

					str = "}\n";
					array = System.Text.Encoding.Default.GetBytes(str);
					fs.Write(array, 0, array.Length);
				}
			}
		}

		#endregion
	}
}