using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
				{
					str = $"DA({da.NameDA} {0} {MapLibiecType(da.BTypeDA)} {MapLibiecFc(da.FCDA)} {0} {0})";
					if (da.NameDA == "ctlModel")
					{
						try
						{
							var lol = (from x in ServerModel.ListEnumType
									   where x.NameEnumType.ToUpper() == "CtlModels".ToUpper()
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
					str = $"DA({da.NameDA} {0} {MapLibiecType(da.BTypeDA)} {MapLibiecFc(da.FCDA)} {0} {0}){{\n";
					array = System.Text.Encoding.Default.GetBytes(str);
					fs.Write(array, 0, array.Length);

					SaveDa(fs, da.ListDA);

					str = "}\n";
					array = System.Text.Encoding.Default.GetBytes(str);
					fs.Write(array, 0, array.Length);
				}
			}
		}

		private static int MapLibiecType(string dataType)
		{
			int type = 0;
			switch (dataType.ToUpper())
			{
				case "BOOLEAN":
					type = (int)LibIecDataAttributeType.BOOLEAN;
					break;
				case "INT8":
					type = (int)LibIecDataAttributeType.INT8;
					break;
				case "INT16":
					type = (int)LibIecDataAttributeType.INT16;
					break;
				case "INT32":
					type = (int)LibIecDataAttributeType.INT32;
					break;
				case "ENUM":
					type = (int)LibIecDataAttributeType.INT32;
					break;
				case "INT64":
					type = (int)LibIecDataAttributeType.INT64;
					break;
				case "INT128":
					type = (int)LibIecDataAttributeType.INT128;
					break;
				case "INT8U":
					type = (int)LibIecDataAttributeType.INT8U;
					break;
				case "INT16U":
					type = (int)LibIecDataAttributeType.INT16U;
					break;
				case "INT24U":
					type = (int)LibIecDataAttributeType.INT24U;
					break;
				case "INT32U":
					type = (int)LibIecDataAttributeType.INT32U;
					break;
				case "FLOAT32":
					type = (int)LibIecDataAttributeType.FLOAT32;
					break;
				case "FLOAT64":
					type = (int)LibIecDataAttributeType.FLOAT64;
					break;
				case "ENUMERATED":
					type = (int)LibIecDataAttributeType.ENUMERATED;
					break;
				case "OCTET_STRING_64":
					type = (int)LibIecDataAttributeType.OCTET_STRING_64;
					break;
				case "OCTET64":
					type = (int)LibIecDataAttributeType.OCTET_STRING_64;
					break;
				case "OCTET_STRING_6":
					type = (int)LibIecDataAttributeType.OCTET_STRING_6;
					break;
				case "OCTET6":
					type = (int)LibIecDataAttributeType.OCTET_STRING_6;
					break;
				case "OCTET_STRING_8":
					type = (int)LibIecDataAttributeType.OCTET_STRING_8;
					break;
				case "OCTET8":
					type = (int)LibIecDataAttributeType.OCTET_STRING_8;
					break;
				case "VISIBLE_STRING_32":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_32;
					break;
				case "VISSTRING32":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_32;
					break;
				case "VISIBLE_STRING_64":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_64;
					break;
				case "VISSTRING64":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_64;
					break;
				case "VISIBLE_STRING_65":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_65;
					break;
				case "VISSTRING65":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_65;
					break;
				case "VISIBLE_STRING_129":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_129;
					break;
				case "VISSTRING129":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_129;
					break;
				case "VISIBLE_STRING_255":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_255;
					break;
				case "VISSTRING255":
					type = (int)LibIecDataAttributeType.VISIBLE_STRING_255;
					break;
				case "UNICODE_STRING_255":
					type = (int)LibIecDataAttributeType.UNICODE_STRING_255;
					break;
				case "UNICODE255":
					type = (int)LibIecDataAttributeType.UNICODE_STRING_255;
					break;
				case "TIMESTAMP":
					type = (int)LibIecDataAttributeType.TIMESTAMP;
					break;
				case "QUALITY":
					type = (int)LibIecDataAttributeType.QUALITY;
					break;
				case "CHECK":
					type = (int)LibIecDataAttributeType.CHECK;
					break;
				case "CODEDENUM":
					type = (int)LibIecDataAttributeType.CODEDENUM;
					break;
				case "GENERIC_BITSTRING":
					type = (int)LibIecDataAttributeType.GENERIC_BITSTRING;
					break;
				case "CONSTRUCTED":
					type = (int)LibIecDataAttributeType.CONSTRUCTED;
					break;
				case "STRUCT":
					type = (int)LibIecDataAttributeType.CONSTRUCTED;
					break;
				case "ENTRY_TIME":
					type = (int)LibIecDataAttributeType.ENTRY_TIME;
					break;
				case "PHYCOMADDR":
					type = (int)LibIecDataAttributeType.PHYCOMADDR;
					break;
			}
			return type;
		}

		static int MapLibiecFc(string fc)
		{
			int fco;
			if (fc != null)
			{
				switch (fc.ToUpper())
				{
					case "ST":
						fco = (int)LibIecFunctionalConstraint.FC_ST;
						break;
					case "MX":
						fco = (int)LibIecFunctionalConstraint.FC_MX;
						break;
					case "SP":
						fco = (int)LibIecFunctionalConstraint.FC_SP;
						break;
					case "SV":
						fco = (int)LibIecFunctionalConstraint.FC_SV;
						break;
					case "CF":
						fco = (int)LibIecFunctionalConstraint.FC_CF;
						break;
					case "DC":
						fco = (int)LibIecFunctionalConstraint.FC_DC;
						break;
					case "SG":
						fco = (int)LibIecFunctionalConstraint.FC_SG;
						break;
					case "SE":
						fco = (int)LibIecFunctionalConstraint.FC_SE;
						break;
					case "SR":
						fco = (int)LibIecFunctionalConstraint.FC_SR;
						break;
					case "OR":
						fco = (int)LibIecFunctionalConstraint.FC_OR;
						break;
					case "BL":
						fco = (int)LibIecFunctionalConstraint.FC_BL;
						break;
					case "EX":
						fco = (int)LibIecFunctionalConstraint.FC_EX;
						break;
					case "СО":
						fco = (int)LibIecFunctionalConstraint.FC_CO;
						break;
					case "ALL":
						fco = (int)LibIecFunctionalConstraint.FC_ALL;
						break;
					case "NONE":
						fco = (int)LibIecFunctionalConstraint.FC_NONE;
						break;
					default:
						fco = -1;
						break;
				}
			}
			else
			{
				fco = -1;
			}
			return fco;
		}
		#endregion
	}

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	enum LibIecDataAttributeType
	{
		BOOLEAN = 0,/* int */
		INT8 = 1,   /* int8_t */
		INT16 = 2,  /* int16_t */
		INT32 = 3,  /* int32_t */
		INT64 = 4,  /* int64_t */
		INT128 = 5, /* no native mapping! */
		INT8U = 6,  /* uint8_t */
		INT16U = 7, /* uint16_t */
		INT24U = 8, /* uint32_t */
		INT32U = 9, /* uint32_t */
		FLOAT32 = 10, /* float */
		FLOAT64 = 11, /* double */
		ENUMERATED = 12,
		OCTET_STRING_64 = 13,
		OCTET_STRING_6 = 14,
		OCTET_STRING_8 = 15,
		VISIBLE_STRING_32 = 16,
		VISIBLE_STRING_64 = 17,
		VISIBLE_STRING_65 = 18,
		VISIBLE_STRING_129 = 19,
		VISIBLE_STRING_255 = 20,
		UNICODE_STRING_255 = 21,
		TIMESTAMP = 22,
		QUALITY = 23,
		CHECK = 24,
		CODEDENUM = 25,
		GENERIC_BITSTRING = 26,
		CONSTRUCTED = 27,
		ENTRY_TIME = 28,
		PHYCOMADDR = 29
	}

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	enum LibIecFunctionalConstraint
	{
		/** Status information */
		FC_ST = 0,
		/** Measurands - analog values */
		FC_MX = 1,
		/** Setpoint */
		FC_SP = 2,
		/** Substitution */
		FC_SV = 3,
		/** Configuration */
		FC_CF = 4,
		/** Description */
		FC_DC = 5,
		/** Setting group */
		FC_SG = 6,
		/** Setting group editable */
		FC_SE = 7,
		/** Service response / Service tracking */
		FC_SR = 8,
		/** Operate received */
		FC_OR = 9,
		/** Blocking */
		FC_BL = 10,
		/** Extended definition */
		FC_EX = 11,
		/** Control */
		FC_CO = 12,
		FC_ALL = 99,
		FC_NONE = -1
	}
}