using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Server.Format
{
	public static partial class FormatConverter
	{
		public static bool OldFormat;

		public static readonly List<Format> FormatList = new List<Format>()
		{
			new Format("HexToPercent",  "int16", "1/40.96", "0", 2),
			new Format("HexToUint16", "uint16", "1", "0", 4),
			new Format("HexToInt16", "int16", "1", "0", 4),
			new Format("BLOCKED", "uint16", "1", "0", 0),
			new Format("HexTo8_8", "int16", "1/256.0", "0", 4),
			new Format("HexTo0_16", "uint16", "1/65536.0", "0", 4),
			new Format("HexToSlide", "uint16", "1/327.68", "0", 4),
			new Format("BLOCKED", "uint16", "1", "0", 0),
			new Format("BLOCKED", "uint16", "1", "0", 0),
			new Format("BLOCKED", "uint16", "1", "0", 0),
			new Format("HexToInt10", "int16", "1/10.0", "0", 4),
			new Format("HexToHex", "uint16", "1", "0", 0),
			new Format("HexToUf", "int16", "0.135", "0", 4),
			new Format("HexToFreqNew", "uint16", "1/500.0", "0", 4),
			new Format("BLOCKED", "uint16", "1", "0", 0),
			new Format("HexToTransAlarm", "int16", "0.00172633491500621954199424893092", "0", 7 ),
			new Format("HexToInt8", "int16", "1/8.0", "0", 2),
			new Format("HexToUint1000", "uint16", "1/1000.0", "0", 2),
			new Format("HexToPercent4", "int16", "1/10.24", "0", 2),
			new Format("BLOCKED", "uint16", "1", "0", 0),
			new Format("HexToPercentUpp", "int16", "1/20.48", "0", 2),
			new Format("HexToFreqUPTF", "uint16", "16000", "0", 2)
		};

		/*  ____Discription____ 
		(A*value + B)
		A*value + B  // нужно получить такой вид						
		Bit depth:		 
						
		uint16  int16	16bit	1
		uint32  int32	32bit	2
		uint64  int64	64bit	3	
		false	true

		OutFormat:

		binary
		hex
		double
		*/
	
		public static string GetValue(ulong value, byte indexFormat)
		{
			return OldFormat ? GetOldFormat(value, indexFormat) : GetNewFormat(value, indexFormat);
		}

		private static string GetNewFormat(ulong value, byte indexFormat)
		{
			Format temp;
			string str = "";

			try
			{
				temp = FormatList[indexFormat];

			}
			catch
			{
				str = "UNKOWN";
				return str;
			}

			switch (temp.BitDepth.Sign)
			{
				case true:
					switch (temp.BitDepth.Bit)
					{
						case 1:
							str = (temp.A * (short)value + temp.B).ToString($"F{temp.Smaller}");
							break;
						case 2:
							str = (temp.A * (int)value + temp.B).ToString($"F{temp.Smaller}");
							break;
						case 3:
							str = (temp.A * (long)value + temp.B).ToString($"F{temp.Smaller}");
							break;
					}
					break;
				case false:
					switch (temp.BitDepth.Bit)
					{
						case 1:
							str = (temp.A * (ushort)value + temp.B).ToString($"F{temp.Smaller}");
							break;
						case 2:
							str = (temp.A * (uint)value + temp.B).ToString($"F{temp.Smaller}");
							break;
						case 3:
							str = (temp.A * value + temp.B).ToString($"F{temp.Smaller}");
							break;
					}
					break;
			}

			return str;
		}

		public class Format
		{
			public string Name { get; set; }
			public BitDepth BitDepth { get; set; }
			public string AStr { get; private set; }
			public double A { get; private set; }
			public string BStr { get; private set; }
			public double B { get; private set; }
			public uint Smaller { get; set; }

			public void AChange(string a)
			{
				AStr = a;
				A = ConvertToDouble(AStr);
			}

			public void BChange(string b)
			{
				BStr = b;
				B = ConvertToDouble(BStr);
			}

			public Format(string name, string bitDepth, string a, string b, uint smaller)
			{
				Name = name;
				BitDepth = new BitDepth(bitDepth);
				AStr = a;
				A = ConvertToDouble(AStr);
				BStr = b;
				B = ConvertToDouble(BStr);
				Smaller = smaller;
			}

			private double ConvertToDouble(string val)
			{
				double valOut;

				if (val.Split('/').Length == 2)
				{
					var valStr = val.Split('/');
					valOut = Convert.ToDouble(valStr[0].Length != 0 ? valStr[0].Replace('.', ','):"0") 
						/ Convert.ToDouble(valStr[1].Length != 0 ? valStr[1].Replace('.',','):"1");
				}
				else
				{
					valOut = Convert.ToDouble(val.Replace('.', ','));
				}

				return valOut;
			}
		}

		public class BitDepth
		{
			public string Name { get; }
			public ushort Bit { get; }		// 16 - 1, 32 - 2, 64 - 3
			public bool Sign { get; }		// Signed - true, Unsigned - false

			public BitDepth(string bitDepth)
			{
				switch (bitDepth)
				{
					case "uint16":
						Bit = 1;
						Sign = false;
						Name = "uint16";
						break;
					case "uint32":
						Bit = 2;
						Sign = false;
						Name = "uint32";
						break;
					case "uint64":
						Bit = 3;
						Sign = false;
						Name = "uint64";
						break;
					case "int16":
						Bit = 1;
						Sign = true;
						Name = "int16";
						break;
					case "int32":
						Bit = 2;
						Sign = true;
						Name = "int32";
						break;
					case "int64":
						Bit = 3;
						Sign = true;
						Name = "int64";
						break;
				}
			}
		}

		public static string GetRangeMax(int index)
		{
			return (FormatList[index].A * GetMax(index) + FormatList[index].B).ToString("0.#####");
		}

		private static double GetMax(int index)
		{
			switch (FormatList[index].BitDepth.Name)
			{
				case "int64":
					return 9223372036854775807;
				case "int32":
					return 2147483647;
				case "int16":
					return 32767;
				case "uint64":
					return 18446744073709551615;
				case "uint32":
					return 4294967295;
				case "uint16":
					return 65535;
				default: return 0;
			}
		}

		public static string GetRangeMin(int index)
		{
			return (FormatList[index].A * GetMin(index) + FormatList[index].B).ToString("0.#####");
		}

		private static double GetMin(int index)
		{
			switch (FormatList[index].BitDepth.Name)
			{
				case "int64":
					return -9223372036854775808;
				case "int32":
					return -2147483648;
				case "int16":
					return -32768;
				case "uint64":
					return 0;
				case "uint32":
					return 0;
				case "uint16":
					return 0;
				default: return 0;
			}
		}

		public static void ReadFormats(string filePath)
		{
			//задаем путь к нашему рабочему файлу XML
			if(filePath == null) filePath = @"Formats.xml";

			XDocument doc;
			try
			{
				doc = XDocument.Load(filePath);
			}
			catch
			{
				OldFormat = true;
				return;
			}

			//читаем данные из файла
			var xElement = doc.Element("Formats");
			if (xElement != null)
			{
				var formatList = xElement.Elements("Format").ToList();

				foreach (var itemFormat in formatList)
				{
					if (itemFormat.Attribute("name") != null)
					{
						if (!(from x in FormatList
							  where x.Name == itemFormat.Attribute("name")?.Value
							  select x).ToList().Any())
						{
							FormatList.Add(new Format(
								itemFormat.Attribute("name")?.Value,
								itemFormat.Attribute("bitDepth")?.Value,
								itemFormat.Attribute("A")?.Value,
								itemFormat.Attribute("B")?.Value,
								Convert.ToUInt32(itemFormat.Attribute("Smaller")?.Value)));
						}
						else
						{
							var item = (from x in FormatList
										where x.Name == itemFormat.Attribute("name")?.Value
										select x).ToList().First();

							item.AChange(itemFormat.Attribute("A")?.Value);
							item.BChange(itemFormat.Attribute("B")?.Value);
							item.Smaller = Convert.ToUInt32(itemFormat.Attribute("Smaller")?.Value);
							item.BitDepth = new BitDepth(itemFormat.Attribute("bitDepth")?.Value);
						}
					}
				}
			}

			OldFormat = false;
		}

		public static void SaveFormats(string savePath)
		{
			if(savePath == null) savePath = "Formats.xml";

			FileStream fs = new FileStream(savePath, FileMode.Create);

			XDocument xDocument =new XDocument(new XElement("Formats"));

			foreach (var itemFormat in FormatList)
			{
				xDocument.Element("Formats")?.Add(new XElement("Format",
					new XAttribute("name", Convert.ToString(itemFormat.Name)),
					new XAttribute("bitDepth", BitDepthToString(itemFormat.BitDepth)),
					new XAttribute("Smaller", Convert.ToString(itemFormat.Smaller)),
					new XAttribute("A", itemFormat.AStr),
					new XAttribute("B", itemFormat.BStr)));
			}

			xDocument.Save(fs);
			fs.Close();
		}

		private static string BitDepthToString(BitDepth item)
		{
			//public ushort Bit { get; }      // 16 - 1, 32 - 2, 64 - 3
			//public bool Sign { get; }		// Signed - true, Unsigned - false

			string val;

			if (item.Sign)
			{
				switch (item.Bit)
				{
					case 3:
						val = "int64";
						break;
					case 2:
						val = "int32";
						break;
					default:
						val = "int16";
						break;
				}
			}
			else
			{
				switch (item.Bit)
				{
					case 3:
						val = "uint64";
						break;
					case 2:
						val = "uint32";
						break;
					default:
						val = "uint16";
						break;
				}
			}

			return val;
		}

		private static string GetOldFormat(ulong value, byte indexFormat)
		{
			string str;
			switch (indexFormat)
			{
				case 0:
					str = HexToPercent(value);
					break;
				case 1:
					str = HexToUint16(value);
					break;
				case 2:
					str = HexToInt16(value);
					break;
				case 3:
					str = HexToFreq(value);
					break;
				case 4:
					str = HexTo8_8(value);
					break;
				case 5:
					str = HexTo0_16(value);
					break;
				case 6:
					str = HexToSlide(value);
					break;
				case 7:
					str = HexToDigits(value);
					break;
				case 8:
					str = HexRegulMode(value);
					break;
				case 9:
					str = HexToAVRType(value);
					break;
				case 10:
					str = HexToInt10(value);
					break;
				case 11:
					str = HexToHex(value);
					break;
				case 12:
					str = HexToUf(value);
					break;
				case 13:
					str = HexToFreqNew(value);
					break;
				case 14:
					str = HexToTT(value);
					break;
				case 15:
					str = HexToTransAlarm(value);
					break;
				case 16:
					str = HexToInt8(value);
					break;
				case 17:
					str = HexToUint1000(value);
					break;
				case 18:
					str = HexToPercent4(value);
					break;
				case 19:
					str = HexToFreqNew2(value);
					break;
				case 20:
					str = HexToPercentUpp(value);
					break;
				case 21:
					str = HexToFreqUPTF(value);
					break;
				default:
					str = "UNKOWN";
					break;
			}
			return (str);
		}

		private static string HexToPercent(ulong value)
		{
			double f = (short)value / 40.96;
			return (f.ToString("F"));
		}

		private static string HexToUint16(ulong value)
		{
			return ((ushort)value).ToString();
		}

		private static string HexToInt16(ulong value)
		{
			return ((short)value).ToString();
		}

		private static string HexToFreq(ulong value)
		{
			string str;
			switch (value)
			{
				case 0:
					str = "Н/Д";
					break;
				case 0x4000:
					str = "Н/Д";
					break;
				default:
					double f = 8000.0 / value;
					str = f.ToString("F2");
					break;
			}
			return str;
		}

		private static string HexTo8_8(ulong value)
		{
			double f = (short)value / 256.0;
			return (f.ToString("F2"));
		}

		private static string HexTo0_16(ulong value)
		{
			double f = (ushort)(value) / 65536.0;
			return (f.ToString("F3"));
		}

		private static string HexToSlide(ulong value)
		{
			double f = (ushort)value / 327.68;
			if (value == 320) { f = 0; }
			return (f.ToString("F2"));
		}

		private static string HexToDigits(ulong value)
		{
			string str = Convert.ToString((long)value, 2);
			return str;
		}

		private static string HexRegulMode(ulong value)
		{
			string str;
			switch (value)
			{
				case 0: { str = "Авто"; } break;
				case 1: { str = "Ручн."; } break;
				default: { str = "Тест"; } break;
			}
			return str;
		}

		private static string HexToAVRType(ulong value)
		{
			string str;
			switch (value)
			{
				case 1: { str = "cosФ"; } break;
				case 2: { str = "Ire"; } break;
				case 3: { str = "If"; } break;
				default: { str = "U"; } break;
			}
			return str;
		}

		private static string HexToInt10(ulong value)
		{
			double f = (short)value / 10.0;
			return f.ToString("F1");
		}

		private static string HexToHex(ulong value)
		{
			return ("0x" + ((ushort)value).ToString("X4"));
		}

		private static string HexToUf(ulong value)
		{
			double f = (short)value * 0.135;
			return (f.ToString("F1"));
		}

		private static string HexToFreqNew(ulong value)
		{
			double f = (ushort)value / 500.0;
			return (f.ToString("F1"));
		}

		private static string HexToTT(ulong value)
		{
			if (value < 0x0010) { return "Ошибка"; }
			double f = 2560.0 / (short)(value);
			return (f.ToString("F2"));
		}

		private static string HexToTransAlarm(ulong value)
		{
			double f = (short)value * 0.00172633491500621954199424893092;
			return (f.ToString("F1"));
		}

		private static string HexToInt8(ulong value)
		{
			double f = (short)value / 8.0;
			return (f.ToString("F0"));
		}

		private static string HexToUint1000(ulong value)
		{
			double f = value / 1000.0;
			return (f.ToString("F3"));
		}

		private static string HexToPercent4(ulong value)
		{
			double f = (short)value / 10.24;
			return (f.ToString("F2"));
		}

		private static string HexToFreqNew2(ulong value)
		{
			string str;
			switch (value)
			{
				case 0:
					str = "Нет данных";
					break;
				case 3600:
					str = "Нет данных";
					break;
				default:
					double f = 90000.0 / value;
					str = f.ToString("F2");
					break;
			}
			return str;
		}

		private static string HexToPercentUpp(ulong value)
		{
			double f = (short)value / 20.48;
			return (f.ToString("F"));
		}

		private static string HexToFreqUPTF(ulong value)
		{
			string str;
			if (value == 0) { str = "Нет данных"; }
			else if (value == 0x4000) { str = "Нет данных"; }
			else
			{
				double f = 16000.0 / value;
				str = f.ToString("F3");
			}
			return str;
		}

	}
}