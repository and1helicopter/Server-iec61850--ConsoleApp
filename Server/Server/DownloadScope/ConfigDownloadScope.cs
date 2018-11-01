using System;

namespace ServerLib.DownloadScope
{
	/// <summary>
	/// Setting download scope
	/// </summary>
	public static class ConfigDownloadScope
	{
		public static bool Enable { get; private set; }
		public static bool Remove { get; private set; }
		public static string Type { get; private set; }
		public static string ComtradeType { get; private set; }
		public static ushort ConfigurationAddr { get; private set; }
		public static ushort OscilCmndAddr { get; private set; }
		public static string PathScope { get; private set; }
		public static string OscilNominalFrequency { get; private set; }
		public static int TimeWait { get; private set; } = 600000;
		public static bool TimeWaitEnable { get; private set; } = true;

		private static void ChangeEnabale(bool enable)
		{
			Enable = enable;
		}

		private static void ChangeRemove(bool remove)
		{
			Remove = remove;
		}

		private static void ChangeType(string type)
		{
			switch (type.ToLower())
			{
				case "comtrade":
					{
						Type = "comtrade";
					}
					break;
				default:
					{
						Type = "txt";
					}
					break;
			}
		}

		private static void ChangeComtradeType(string comtradeType)
		{
			switch (comtradeType.ToLower())
			{
				case "2013":
					{
						ComtradeType = "2013";
					}
					break;
				default:
					{
						ComtradeType = "1999";
					}
					break;
			}
		}

		private static void ChangeConfigurationAddr(ushort configurationAddr)
		{
			ConfigurationAddr = configurationAddr;
		}

		private static void ChangeOscilCmndAddr(ushort oscilCmndAddr)
		{
			OscilCmndAddr = oscilCmndAddr;
		}

		private static void ChangePathScope(string pathScope)
		{
			//"vmd-filestore" + путь до папки где лежат осциллограммы
			PathScope = pathScope;
		}

		private static void ChangeOscilNominalFrequency(string oscilNominalFrequency)
		{
			OscilNominalFrequency = oscilNominalFrequency;
		}
		
		internal static void InitConfigDownloadScope(string enabele, string remove, string type, string comtradeType, string configurationAddr, string oscilCmndAddr, string pathScope, string oscilNominalFrequency)
		{
			ChangeType(type);
			ChangeComtradeType(comtradeType);
			ChangePathScope(pathScope);
			ChangeOscilNominalFrequency(oscilNominalFrequency);

			try
			{
				ChangeEnabale(Convert.ToBoolean(enabele));
				ChangeRemove(Convert.ToBoolean(remove));
				ChangeConfigurationAddr(Convert.ToUInt16(configurationAddr));
				ChangeOscilCmndAddr(Convert.ToUInt16(oscilCmndAddr));
			}
			catch
			{
				ChangeEnabale(false);
				Log.Log.Write("UpdateModBus: DownloadScope.InitConfigDownloadScope finish with error - Scope = disable ", "Warning ");
			}
		}
	}

}
