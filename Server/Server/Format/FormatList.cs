using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Server.Format
{
	public static partial class FormatConverter
	{
		private static readonly object[] _oldFormat =
		{
			"0 - Percent",
			"1 - uint16",
			"2 - int16",
			"3 - Freq standart",
			"4 - 8.8",
			"5 - 0.16",
			"6 - Slide",
			"7 - Digits",
			"8 - RegulMode",
			"9 - AVR type",
			"10 - Int/10",
			"11 - Hex",
			"12 - *0.135 (Uf)",
			"13 - FreqNew",
			"14 - Current trans",
			"15 - trans alarm",
			"16 - int/8",
			"17 - uint/1000",
			"18 - percent/4",
			"19 - FreqNew2",
			"20 - Percent upp",
			"21 - Freq UPTF"
		};
		
		public static object[] ActualFormat;

		private static readonly List<ObjFormat> ListObjFormats = new List<ObjFormat>();

		private class ObjFormat
		{
			public string NameFormat { get; }
			public string VisualNameFormat { get; }
			public int IndexSizeFormat { get; }

			public ObjFormat(string nameFormat, string visualNameFormat , int indexSizeFormat)
			{
				NameFormat = nameFormat;
				IndexSizeFormat = indexSizeFormat;
				VisualNameFormat = visualNameFormat;
			}
		}

		private static object[] _newFormat;

		public static void UpdateVisualFormat()
		{
			ListObjFormats.Clear();

			if (OldFormat)
			{
				ActualFormat = _oldFormat;
				foreach (var itemFormat in _oldFormat.ToList())
				{
					ListObjFormats.Add(new ObjFormat(itemFormat.ToString(), itemFormat.ToString(), 0));
				}
			}
			else
			{
				_newFormat = new object[FormatList.Count];

				foreach (var itemFormat in FormatList)
				{
					ListObjFormats.Add(new ObjFormat(itemFormat.Name, FormatList.IndexOf(itemFormat) + " - " + itemFormat.Name, itemFormat.BitDepth.Bit - 1));
					_newFormat[FormatList.IndexOf(itemFormat)] = FormatList.IndexOf(itemFormat) + " - "+ itemFormat.Name;
				}
				ActualFormat = _newFormat;
			}
		}

		public static int GetIndexSizeFormat(string name)
		{
			//Проверка на новый и старый формат, если новый блокировка форматов с номером несуществующих

			return (from x in ListObjFormats
					where x.VisualNameFormat == name
					select x.IndexSizeFormat).First(); 
		}

		public static int GetIndexListFormat(string name)
		{
			return ListObjFormats.IndexOf((from x in ListObjFormats
										   where x.VisualNameFormat == name
										   select x).First());
		}

		public static string GetCodeFormat(string name)
		{
			var index = GetIndexListFormat(name);
			return (((ListObjFormats[index].IndexSizeFormat + 1) << 8) + index).ToString();
		}

		public static string GetEquationFormat(int index)
		{
			return global::Server.Format.FormatConverter.FormatList[index].A.ToString(CultureInfo.InvariantCulture) +
			                @" * value + " + global::Server.Format.FormatConverter.FormatList[index].B.ToString(CultureInfo.InvariantCulture); ;
		}
	}
}
