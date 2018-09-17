using System.Collections.Generic;

namespace ServerLib.Update
{
	public static partial class UpdateDataObj
	{
		public static class ModHead
		{
			//Beh
			internal static bool IsOn { get; private set; }
			internal static bool IsBlock { get; private set; }
			internal static bool IsTest { get; private set; }
			internal static bool IsTestBlock { get; private set; }
			internal static bool IsOff { get; private set; }

			internal static DestinationDataObject BaseModDataObject { private get; set; }
			internal static DestinationDataObject BaseBehDataObject { private get; set; }

			internal static readonly List<ModDependences> DependencesGroupes = new List<ModDependences>();

			public static void OnWriteValue(dynamic val)
			{
				switch (val.Value)
				{
					case 1:
						ResetStatus();
						IsOn = true;
						BaseModDataObject.IsOn = true;
						BaseBehDataObject.IsOn = true;
						break;
					case 2:
						ResetStatus();
						IsBlock = true;
						BaseModDataObject.IsBlock = true;
						break;
					case 3:
						ResetStatus();
						IsTest = true;
						BaseModDataObject.IsTest = true;
						break;
					case 4:
						ResetStatus();
						IsTestBlock = true;
						BaseModDataObject.IsTestBlock = true;
						break;
					case 5:
						ResetStatus();
						IsOff = true;
						BaseModDataObject.IsOff = true;
						break;
					default:
						ResetStatus();
						IsOn = true;
						BaseModDataObject.IsOn = true;
						val = new { Value = 1, val.Key };
						break;
				}

				var tempValue = new { val.Value, val.Key };
				if (BaseModDataObject != null)
				{
					BaseModDataObject.BaseClass.UpdateClass(tempValue);
					BaseModDataObject.BaseClass.UpdateServer(BaseModDataObject.NameDataObj, _iedServer, _iedModel);
				}

				if (BaseBehDataObject != null)
				{
					BaseBehDataObject.BaseClass.UpdateClass(tempValue);
					BaseBehDataObject.BaseClass.UpdateServer(BaseBehDataObject.NameDataObj, _iedServer, _iedModel);
				}

				UpdateModDependences();

				void ResetStatus()
				{
					IsOn = false;
					IsBlock = false;
					IsTest = false;
					IsTestBlock = false;
					IsOff = false;

					BaseModDataObject.IsOn = false;
					BaseModDataObject.IsBlock = false;
					BaseModDataObject.IsTest = false;
					BaseModDataObject.IsTestBlock = false;
					BaseModDataObject.IsOff = false;

					BaseBehDataObject.IsOn = false;
					BaseBehDataObject.IsBlock = false;
					BaseBehDataObject.IsTest = false;
					BaseBehDataObject.IsTestBlock = false;
					BaseBehDataObject.IsOff = false;
				}

				void UpdateModDependences()
				{
					foreach (var dependences in DependencesGroupes)
					{
						dependences.UpdateWriteValue();
					}
				}
			}
		}

		public class ModDependences
		{
			//Beh
			private bool IsOn { get; set; }
			private bool IsBlock { get; set; }
			private bool IsTest { get; set; }
			private bool IsTestBlock { get; set; }
			private bool IsOff { get; set; }
			private int ValBeh { get; set; } = 1;
			private int ValMod { get; set; } = 1;

			internal DestinationDataObject BaseModDataObject { private get; set; }
			internal DestinationDataObject BaseBehDataObject { private get; set; }

			internal List<DestinationDataObject> DependencesDataObjects { private get; set; } = new List<DestinationDataObject>();

			internal void UpdateWriteValue()
			{
				OnWriteValue(new {Value = ValMod, Key = "stVal"});
			}

			internal void OnWriteValue(dynamic val)
			{
				ResetStatus();
				
				switch (val.Value)
				{
					case 1:
						if (ModHead.IsOn)
						{
							IsOn = true;
							ValBeh = 1;
						}
						else if (ModHead.IsBlock)
						{
							IsBlock = true;
							ValBeh = 2;
						}
						else if (ModHead.IsTest)
						{
							IsTest = true;
							ValBeh = 3;
						}
						else if (ModHead.IsTestBlock)
						{
							IsTestBlock = true;
							ValBeh = 4;
						}
						else if (ModHead.IsOff)
						{
							IsOff = true;
							ValBeh = 5;
						}
						break;
					case 2:
						if (ModHead.IsOn || ModHead.IsBlock)
						{
							IsBlock = true;
							ValBeh = 2;
						}
						else if (ModHead.IsTest || ModHead.IsTestBlock)
						{
							IsTestBlock = true;
							ValBeh = 4;
						}
						else if (ModHead.IsOff)
						{
							IsOff = true;
							ValBeh = 5;
						}
						break;
					case 3:
						if (ModHead.IsOn || ModHead.IsTest)
						{
							IsTest = true;
							ValBeh = 3;
						}
						else if (ModHead.IsBlock || ModHead.IsTestBlock)
						{
							IsTestBlock = true;
							ValBeh = 4;
						}
						else if (ModHead.IsOff)
						{
							IsOff = true;
							ValBeh = 5;
						}
						break;
					case 4:
						if (ModHead.IsOff)
						{
							IsOff = true;
							ValBeh = 5;
						}
						else if (ModHead.IsOn || ModHead.IsBlock || ModHead.IsTest || ModHead.IsTestBlock)
						{
							IsTestBlock = true;
							ValBeh = 4;
						}
						break;
					case 5:
						IsOff = true;
						ValBeh = 5;
						break;
					default:
						if (ModHead.IsOn)
						{
							IsOn = true;
							ValBeh = 1;
						}
						else if (ModHead.IsBlock)
						{
							IsBlock = true;
							ValBeh = 2;
						}
						else if (ModHead.IsTest)
						{
							IsTest = true;
							ValBeh = 3;
						}
						else if (ModHead.IsTestBlock)
						{
							IsTestBlock = true;
							ValBeh = 4;
						}
						else if (ModHead.IsOff)
						{
							IsOff = true;
							ValBeh = 5;
						}
						val = new {Value = 1, val.Key };
						break;
				}

				ValMod = val.Value;
				ChangeStatus();

				var tempModValue = new { val.Value, val.Key };
				if (BaseModDataObject != null)
				{
					BaseModDataObject.BaseClass.UpdateClass(tempModValue);
					BaseModDataObject.BaseClass.UpdateServer(BaseModDataObject.NameDataObj, _iedServer, _iedModel);
				}

				var tempBehValue = new { Value = ValBeh, val.Key };
				if (BaseBehDataObject != null)
				{
					BaseBehDataObject.BaseClass.UpdateClass(tempBehValue);
					BaseBehDataObject.BaseClass.UpdateServer(BaseBehDataObject.NameDataObj, _iedServer, _iedModel);
				}


				void ResetStatus()
				{
					IsOn = false;
					IsBlock = false;
					IsTest = false;
					IsTestBlock = false;
					IsOff = false;
				}

				void ChangeStatus()
				{
					foreach (var destination in DependencesDataObjects)
					{
						destination.IsBusy = true;
						destination.IsOn = IsOn;
						destination.IsBlock = IsBlock;
						destination.IsTest = IsTest;
						destination.IsTestBlock = IsTestBlock;
						destination.IsOff = IsOff;
						destination.IsBusy = false;
					}
				}
			}
		}

		public static class HealthHead
		{
			internal static DestinationDataObject BaseHealthDataObject { private get; set; }
			internal static int ValHealth { get; private set; } = 1;

			// ReSharper disable once CollectionNeverQueried.Global
			internal static readonly List<HealthDependences> DependencesHealth = new List<HealthDependences>();
			
			public static void OnReadValue(dynamic val)
			{
				if (val.Value != ValHealth)
				{
					switch (val.Value)
					{
						case 1:
							ValHealth = 1;
							break;
						case 2:
							ValHealth = 2;
							break;
						case 3:
							ValHealth = 3;
							break;
						default:
							ValHealth = 1;
							break;
					}

					var tempValue = new { Value = ValHealth, val.Key };
					if (BaseHealthDataObject != null)
					{
						BaseHealthDataObject.BaseClass.UpdateClass(tempValue);
						BaseHealthDataObject.BaseClass.UpdateServer(BaseHealthDataObject.NameDataObj, _iedServer, _iedModel);
					}
				}
			}
		}

		public class HealthDependences
		{
			internal DestinationDataObject BaseHealthDataObject { private get; set; }
			private int ValHealth { get; set; } = 1;

			internal void OnReadValue(dynamic val)
			{
				if(val.Value != ValHealth)
				{
					switch (val.Value)
					{
						case 1:
							ValHealth = 1;
							break;
						case 2:
							ValHealth = 2;
							break;
						case 3:
							ValHealth = 3;
							break;
						default:
							ValHealth = 1;
							break;
					}

					var tempValue = new { Value = ValHealth, val.Key };
					if (BaseHealthDataObject != null)
					{
						BaseHealthDataObject.BaseClass.UpdateClass(tempValue);
						BaseHealthDataObject.BaseClass.UpdateServer(BaseHealthDataObject.NameDataObj, _iedServer, _iedModel);
					}

					if (ValHealth > HealthHead.ValHealth)
					{
						HealthHead.OnReadValue(tempValue);
					}
				}
			}
		}
	}
}
