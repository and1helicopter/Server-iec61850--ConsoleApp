using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerLib.Update
{
	public static partial class UpdateDataObj
	{
		public static class ModClass
		{
			public static readonly List<string> PathModList = new List<string>();

			public static int ValMod { get; set; }
		}

		public static class BehClass
		{
			public static readonly List<string> PathBehList = new List<string>();

			public static int ValBeh { get; set; }
		}

		public static class HealthClass
		{
			public static readonly List<string> PathHealthList = new List<string>();
			public static readonly List<string> PathPhyHealthList = new List<string>();

			public static int ValHealth { get; set; }
		}

		public static class ChackChangeStatus
		{
			private static bool GoodStatus { get; set; }

			public static void Chack(bool goodStatus)
			{
				if (goodStatus != GoodStatus)
				{
					StatusChange(goodStatus ? 1 : 5);
					GoodStatus = goodStatus;
				}
			}
		}

		public static async void StatusChange(int status)
		{
			try
			{
				switch (status)
				{
					case 1:
						await ModStatusChange(1);
						await BehStatusChange(1);
						await HealthStatusChange(1);
						break;
					case 2:
						break;
					case 3:
						break;
					case 4:
						break;
					case 5:
						await ModStatusChange(5);
						await BehStatusChange(5);
						await HealthStatusChange(3);
						break;
					default:
						await ModStatusChange(5);
						await BehStatusChange(5);
						await HealthStatusChange(3);
						break;
				}
			}
			catch
			{
				// ignored
			}
		}

		private static async Task ModStatusChange(int status)
		{
			ModClass.ValMod = status;

			foreach (var itemMod in ModClass.PathModList)
			{
				//Server.ServerIEC61850.INT_UpdateStatus(itemMod, ModClass.ValMod);
			}
		}


		private static async Task BehStatusChange(int status)
		{
			BehClass.ValBeh = status;

			foreach (var itemBeh in BehClass.PathBehList)
			{
				//Server.ServerIEC61850.INT_UpdateStatus(itemBeh, BehClass.ValBeh);
			}
		}

		private static async Task HealthStatusChange(int status)
		{
			HealthClass.ValHealth = status;

			foreach (var itemHealth in HealthClass.PathHealthList)
			{
				//Server.ServerIEC61850.INT_UpdateStatus(itemHealth, HealthClass.ValHealth);
			}

			foreach (var itemHealth in HealthClass.PathPhyHealthList)
			{
				//Server.ServerIEC61850.INT_UpdateStatus(itemHealth, HealthClass.ValHealth);
			}
		}
	}
}
