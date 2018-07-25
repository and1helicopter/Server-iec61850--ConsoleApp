using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using IEC61850.Server;
using ServerLib.DataClasses;
using ServerLib.ModBus;

namespace ServerLib.Update
{
	public static partial class UpdateDataObj
	{
		//public static readonly  List<ItemObject> ClassGetObjects = new List<ItemObject>();  //Список 
		//internal static readonly List<DestinationDataObject> StaticListDestination = new List<DestinationDataObject>();
		internal static readonly List<DestinationDataObject> UpdateListDestination = new List<DestinationDataObject>();
		internal static readonly List<SourceClass> SourceList = new List<SourceClass>();
		
		private static IedServer _iedServer;
		private static IedModel _iedModel;

		internal static void SetParamsServer(IedServer iedServer, IedModel iedModel)
		{
			_iedServer = iedServer;
			_iedModel = iedModel;
		}

		#region Source
		//Базовый класс
		public abstract class SourceClass
		{
			public abstract ushort Addr { get; set; }
			public abstract ushort Count { get; set; }
			public abstract dynamic Value { get; set; }

			public abstract void GetValueRequest();
			public abstract void GetValueResponse(dynamic value);

			public abstract void SetValue(dynamic value);

			public delegate void ClassStateHandler(dynamic value);

			public abstract event ClassStateHandler ReadValue;
		}

		//Digital
		public class SourceClassDigital : SourceClass, ISourceDigital
		{
			public override ushort Addr { get; set; }
			public override ushort Count { get; set; }
			public override dynamic Value { get; set; }
			public string NameBitArray { get; set; }

			public override void GetValueRequest()          //Read ModBus
			{
				UpdateModBus.GetRequest(Addr, Count, this);    //Читаю значение по адресу
			}

			public override void GetValueResponse(dynamic value)
			{
				Value = value;
				ReadValue?.Invoke(this);
			}

			public override void SetValue(dynamic value)
			{
				ushort mask = (ushort) value.Mask;
				ushort val = (ushort) value.Value;

				ushort[] tempValue = new ushort[1];
				
				tempValue[0] = (ushort)((Value[0] & mask) | val);

				UpdateModBus.SetRequest(Addr, tempValue);
			}

			public override event ClassStateHandler ReadValue;
		}

		//Analog
		public class SourceClassAnalog : SourceClass, ISourceAnalog
		{
			public override ushort Addr { get; set; }
			public override ushort Count { get; set; }
			public override dynamic Value { get; set; }

			public override void GetValueRequest()          //Read UpdateModBus
			{
				UpdateModBus.GetRequest(Addr, Count, this);    //Читаю значение по адресу
			}

			public override void GetValueResponse(dynamic value)
			{
				Value = value;
				ReadValue?.Invoke(this);
			}

			public override void SetValue(dynamic value)
			{
				UpdateModBus.SetRequest(Addr, Value);
			}

			public override event ClassStateHandler ReadValue;
		}

		interface ISourceDigital
		{
			string NameBitArray { get; set; }
		}

		interface ISourceAnalog
		{
			
		}
		#endregion

		public abstract class DestinationDataObject
		{
			public abstract string NameDataObj { get; set; }            //Путь до класса (destination)
			public abstract BaseClass BaseClass { get; set; }                  //Ссылка на объект управления
			protected abstract bool ReadyForUpdate { get; set; }

			protected abstract Dictionary<string, bool> ReadValue { get; set; }
			protected abstract Dictionary<string, SourceClass> Dictionary { get; set; }
			protected abstract List<SourceClass> SourceItem { get; set; }			//Подписка на 
			
			public abstract void AddSource(SourceClass source, string name);

			protected abstract void OnReadValue(dynamic value);
			public abstract void WriteValue(dynamic value);

			protected abstract void Reset();
		}

		//Digital
		public class DestinationObjectDigital : DestinationDataObject, IDestinationDigital
		{
			public override string NameDataObj { get; set; }
			public override BaseClass BaseClass { get; set; }
			protected override bool ReadyForUpdate { get; set; }

			protected override Dictionary<string, bool> ReadValue { get; set; } = new Dictionary<string, bool>();
			protected override Dictionary<string, SourceClass> Dictionary { get; set; } = new Dictionary<string, SourceClass>();
			protected override List<SourceClass> SourceItem { get; set; } = new List<SourceClass>();

			public Dictionary<string, int> IndexData { get; set; } = new Dictionary<string, int>();

			public override void AddSource(SourceClass source, string name)
			{
				ReadValue.Add(name, false);
				Dictionary.Add(name, source);

				if (!SourceItem.Contains(source))
				{
					SourceItem.Add(source);
					source.ReadValue += OnReadValue;
				}
			}

			protected override void OnReadValue(dynamic value)
			{
				try
				{
					var source = value;
					if (Dictionary.ContainsValue(source))
					{
						var tempValNameList = Dictionary.Where(x => x.Value == source);
						foreach (var tempValName in tempValNameList)
						{
							var tempIndex = IndexData[tempValName.Key];

							var tempValue = new { Value = source.Value[0], Index = tempIndex, tempValName.Key };
							BaseClass.UpdateClass(tempValue);
							ReadValue[tempValName.Key] = true;
						}

						if (!ReadValue.ContainsValue(false))
							ReadyForUpdate = true;
					}

					if (ReadyForUpdate)
					{
						BaseClass.UpdateServer(NameDataObj, _iedServer, _iedModel);
						Reset();
					}
				}
				catch
				{
					Log.Log.Write($"DestinationObjectAnalog OnReadValue {value.GetType()}", "Warrning");
				}
			}

			public override void WriteValue(dynamic value)
			{
				//Отправить значение на плату
				var key = value.Key;
				var val = value.Value;
				ushort tempMask = 0xffff;
				ushort tempVal = 0x0000;
				
				var index = IndexData[key];
				var source = Dictionary[key];

				if (BaseClass.GetType() == typeof(SpcClass))
				{
					tempMask = (ushort)(tempMask - (1 << index));
					tempVal = (ushort)(Convert.ToInt32(val) << index);
				}
				else if (BaseClass.GetType() == typeof(DpsClass))
				{

				}

				var tempValue = new
				{
					Mask = tempMask,
					Value = tempVal
				};
			
				source.SetValue(tempValue);
			}

			protected override void Reset()
			{
				foreach (var item in ReadValue.ToList())
				{
					ReadValue[item.Key] = false;
				}

				ReadyForUpdate = true;
			}
		}

		public class DestinationObjectAnalog : DestinationDataObject, IDestinationAnalog
		{
			public override string NameDataObj { get; set; }
			public override BaseClass BaseClass { get; set; }

			protected override bool ReadyForUpdate { get; set; }
			protected override Dictionary<string, bool> ReadValue { get; set; } = new Dictionary<string, bool>();
			protected override Dictionary<string, SourceClass> Dictionary { get; set; } = new Dictionary<string, SourceClass>();
			protected override List<SourceClass> SourceItem { get; set; } = new List<SourceClass>();

			public override void AddSource(SourceClass source, string name)
			{
				ReadValue.Add(name, false);
				Dictionary.Add(name, source);

				if (!SourceList.Contains(source))
				{
					SourceList.Add(source);
				}

				if (!SourceItem.Contains(source))
				{
					SourceItem.Add(source);
					source.ReadValue += OnReadValue;
				}
			}

			protected override void OnReadValue(dynamic value)
			{
				try
				{
					dynamic source = value;

					if (Dictionary.ContainsValue(source))
					{
						var tempValNameList = Dictionary.Where(x => x.Value == source);

						foreach (var tempValName in tempValNameList)
						{
							var tempValue = new { source.Value, tempValName.Key };
							BaseClass.UpdateClass(tempValue);
							ReadValue[tempValName.Key] = true;
						}

						if (!ReadValue.ContainsValue(false))
							ReadyForUpdate = true;
					}

					if (ReadyForUpdate)
					{
						BaseClass.UpdateServer(NameDataObj, _iedServer, _iedModel);
						Reset();
					}
				}
				catch
				{
					Log.Log.Write($"DestinationObjectAnalog OnReadValue {value.GetType()}", "Warrning");
				}
			}

			public override void WriteValue(dynamic value)
			{
				throw new NotImplementedException();
			}

			protected override void Reset()
			{
				foreach (var item in ReadValue.ToList())
				{
					ReadValue[item.Key] = false;
				}

				ReadyForUpdate = true; 
			}
		}

		interface IDestinationDigital
		{
			Dictionary <string, int> IndexData { get; set; } 
			//int IndexData { get; set; }             //Индекс для дискретного канала
		}

		interface IDestinationAnalog
		{

		}
	}
}
