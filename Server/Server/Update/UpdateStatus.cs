using System.Collections.Generic;

namespace Server.Update
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
					GoodStatus = goodStatus;
					StatusChange(GoodStatus ? 1 : 5);
				}
			}
		}

		private static void StatusChange(int status)
		{
			switch (status)
			{
				case 1:
					ModStatusChange(1);
					BehStatusChange(1);
					HealthStatusChange(1);
					break;
				case 2:
					break;
				case 3:
					break;
				case 4:
					break;
				case 5:
					ModStatusChange(5);
					BehStatusChange(5);
					HealthStatusChange(3);
					break;
				default:
					ModStatusChange(5);
					BehStatusChange(5);
					HealthStatusChange(3);
					break;
			}
		}

		private static void ModStatusChange(int status)
		{
			ModClass.ValMod = status;

			foreach (var itemMod in ModClass.PathModList)
			{
				Server.Server.INT_UpdateStatus(itemMod, ModClass.ValMod);
			}
		}


		private static void BehStatusChange(int status)
		{
			BehClass.ValBeh = status;

			foreach (var itemBeh in BehClass.PathBehList)
			{
				Server.Server.INT_UpdateStatus(itemBeh, BehClass.ValBeh);
			}
		}

		private static void HealthStatusChange(int status)
		{
			HealthClass.ValHealth = status;

			foreach (var itemHealth in HealthClass.PathHealthList)
			{
				Server.Server.INT_UpdateStatus(itemHealth, HealthClass.ValHealth);
			}

			foreach (var itemHealth in HealthClass.PathPhyHealthList)
			{
				Server.Server.INT_UpdateStatus(itemHealth, HealthClass.ValHealth);
			}
		}
	}
}
