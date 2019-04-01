using System.Collections.Generic;
using System.Linq;
using IEC61850.Server;

namespace ServerLib.DataClasses
{
	public static partial class DataObj
	{
		internal static readonly List<DestinationDataObject> UpdateListDestination = new List<DestinationDataObject>();
		internal static readonly List<SourceClass> SourceList = new List<SourceClass>();
		//Связанные данные Mod, Beh, Health обновляются связано
		
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

			public delegate void ClassResponseHandler(dynamic value, dynamic param, bool status);

			public abstract void GetValueRequest(ClassResponseHandler responseHandler);
			public abstract void GetValueResponse(dynamic value, bool status);

			public abstract void SetValue(dynamic value);

			public delegate void ClassStateHandlerRead(dynamic value, bool status);
			public abstract event ClassStateHandlerRead ReadValueHandler;
			public abstract bool IsReady { get; set; }
		}

		//Digital
		public class SourceClassDigital : SourceClass, ISourceDigital
		{
			public override ushort Addr { get; set; }
			public override ushort Count { get; set; }
			public override dynamic Value { get; set; }
			public string NameBitArray { get; set; }

			public override void GetValueRequest(ClassResponseHandler responseHandler)          //Read ModBus
			{
				ModBus.ModBus.GetRequest(Addr, Count, new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = responseHandler });    //Читаю значение по адресу
			}

			public override void GetValueResponse(dynamic value, bool status)
			{
				if(status) Value = value;
				ReadValueHandler?.Invoke(this, status);
			}

			public override void SetValue(dynamic value)
			{
				ModBus.ModBus.SetRequest(Addr, value);
			}

			public override event ClassStateHandlerRead ReadValueHandler;
			public override bool IsReady { get; set; } = true;
		}

		//Analog
		public class SourceClassAnalog : SourceClass
		{
			public override ushort Addr { get; set; }
			public override ushort Count { get; set; }
			public override dynamic Value { get; set; }

			public override void GetValueRequest(ClassResponseHandler responseHandler)          //Read UpdateModBus
			{
				ModBus.ModBus.GetRequest(Addr, Count, new ModBusTaskController.ModBusTaskController.CycleClass.ResponseObj { Item = this, Response = responseHandler });    //Читаю значение по адресу
			}

			public override void GetValueResponse(dynamic value, bool status)
			{
				if (status) Value = value;
				ReadValueHandler?.Invoke(this, status);
			}

			public override void SetValue(dynamic value)
			{
				ModBus.ModBus.SetRequest(Addr, value);
			}

			public override event ClassStateHandlerRead ReadValueHandler;
			public override bool IsReady { get; set; } = true;
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
			public abstract string NameDataObj { get; set; }			//Путь до класса (destination)
			public abstract BaseClass BaseClass { get; set; }			//Ссылка на объект управления
			protected abstract bool IsReady { get; set; }				//Готов к обновлению
			public abstract bool IsBusy { get; set; }					//Занят

			//Статусы контроля
			public abstract bool IsOn { get; set; }
			public abstract bool IsBlock { get; set; }
			public abstract bool IsTest { get; set; }
			public abstract bool IsTestBlock { get; set; }
			public abstract bool IsOff { get; set; }

			protected abstract bool IsErrorReadValue { get; set; }
			protected abstract bool IsErrorWriteValue { get; set; }

			protected abstract Dictionary<string, bool> ReadValue { get; set; }				//
			protected abstract Dictionary<string, SourceClass> Dictionary { get; set; }
			protected abstract List<SourceClass> SourceItem { get; set; }					//Подписка на 
			
			public abstract void AddSource(SourceClass source, string name);

			protected abstract void OnReadValue(dynamic value, bool status);
			internal abstract void WriteValue(dynamic value);
			internal abstract void Qality(bool init = false);

			//Обработчик для изменения статуса Mod, Beh, Health
			public delegate void ClassStateHandlerWrite(dynamic value);
			public abstract event ClassStateHandlerWrite WriteValueHandler;
			public abstract event ClassStateHandlerWrite ReadValueHandler;

			protected abstract void ResetReady();
		}

		//Digital
		public class DestinationObjectDigital : DestinationDataObject, IDestinationDigital
		{
			public override string NameDataObj { get; set; }
			public override BaseClass BaseClass { get; set; }
			protected override bool IsReady { get; set; }
			public override bool IsBusy { get; set; }

			//Статусы контроля
			public override bool IsOn { get; set; }
			public override bool IsBlock { get; set; }
			public override bool IsTest { get; set; }
			public override bool IsTestBlock { get; set; }
			public override bool IsOff { get; set; }

			protected override bool IsErrorReadValue { get; set; }
			protected override bool IsErrorWriteValue { get; set; }

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
					source.ReadValueHandler += OnReadValue;
				}
			}

			protected override void OnReadValue(dynamic val, bool status)
			{
				try
				{
					//ReadValueHandler?.Invoke(val);

					if (IsOn)
					{
						IsReadValue(val);
					}
					else if (IsBlock)
					{

					}
					else if (IsTest)
					{
						IsReadValue(val);
					}
					else if (IsTestBlock)
					{

					}
					else if (IsOff)
					{

					}
					
					void IsReadValue(dynamic value)
					{
                        if (!status)
                        {
                            BaseClass.UpdateQuality(NameDataObj, ref _iedServer, ref _iedModel);
                            return;
                        }

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
								IsReady = true;
						}

						if (IsReady)
						{
							BaseClass.UpdateServer(NameDataObj, ref _iedServer, ref _iedModel, false);
							ResetReady();
						}
					}
				}
				catch
				{
					if (!IsErrorReadValue)
					{
						Log.Log.Write($"Destination ReadValue Error {val.GetType()}", "Warrning");
						IsErrorReadValue = true;
					}
				}
			}

			internal override void WriteValue(dynamic val)
			{
				try
				{
					//WriteValueHandler?.Invoke(val);

					if (IsOn)
					{

						IsWriteValue(val);
					}
					else if (IsBlock)
					{

					}
					else if (IsTest)
					{
						IsWriteValue(val);
					}
					else if (IsTestBlock)
					{

					}
					else if (IsOff)
					{

					}

					void IsWriteValue(dynamic value)
					{
						if (Dictionary.Count != 0)
						{
							//Отправить значение на плату				
							var index = IndexData[value.Key];
							var source = Dictionary[value.Key];
							var oldValue = (SourceClassDigital)source;
							var newValue = new
							{
								Index = index,
								value.Value
							};

							var xxx = BaseClass.SetValue(oldValue.Value, newValue, value.Key);

							source.SetValue(xxx);
						}
					}
				}
				catch
				{
					if (!IsErrorWriteValue)
					{
						Log.Log.Write($"Destination WriteValue Error {val.GetType()}", "Warrning");
						IsErrorWriteValue = true;
					}
				}
			}

			internal override void Qality(bool init = false)
			{
				BaseClass.UpdateServer(NameDataObj, ref _iedServer, ref _iedModel, init);
			}

			public override event ClassStateHandlerWrite WriteValueHandler;
			public override event ClassStateHandlerWrite ReadValueHandler;

			protected override void ResetReady()
			{
				foreach (var item in ReadValue.ToList())
				{
					ReadValue[item.Key] = false;
				}

				IsReady = true;
			}
		}

		public class DestinationObjectAnalog : DestinationDataObject, IDestinationAnalog
		{
			public override string NameDataObj { get; set; }
			public override BaseClass BaseClass { get; set; }
			protected override bool IsReady { get; set; }
			public override bool IsBusy { get; set; }

			//Статусы контроля
			public override bool IsOn { get; set; }
			public override bool IsBlock { get; set; }
			public override bool IsTest { get; set; }
			public override bool IsTestBlock { get; set; }
			public override bool IsOff { get; set; }

			protected override bool IsErrorReadValue { get; set; }
			protected override bool IsErrorWriteValue { get; set; }

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
					source.ReadValueHandler += OnReadValue;
				}
			}

			protected override void OnReadValue(dynamic val, bool status)
			{
				try
				{
					ReadValueHandler?.Invoke(val);

					if (IsOn)
					{
						IsReadValue(val);
					}
					else if (IsBlock)
					{

					}
					else if (IsTest)
					{
						IsReadValue(val);
					}
					else if (IsTestBlock)
					{

					}
					else if (IsOff)
					{

					}

					void IsReadValue(dynamic value)
					{
                        if (!status)
                        {
                            BaseClass.UpdateQuality(NameDataObj, ref _iedServer, ref _iedModel);
                            return;
                        }

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
								IsReady = true;
						}
						if (IsReady)
						{
							BaseClass.UpdateServer(NameDataObj, ref _iedServer, ref _iedModel, false);
							ResetReady();
						}
					}
				}
				catch
				{
					if (!IsErrorReadValue)
					{
						Log.Log.Write($"Destination ReadValue Error {val.GetType()}", "Warrning");
						IsErrorReadValue = true;
					}				
				}
			}

			internal override void WriteValue(dynamic val)
			{
				try
				{
					WriteValueHandler?.Invoke(val);         //Событие WriteValueHandler

					if (IsOn)
					{
						IsWriteValue(val);
					}
					else if (IsBlock)
					{
					}
					else if (IsTest)
					{
						IsWriteValue(val);
					}
					else if (IsTestBlock)
					{
					}
					else if (IsOff)
					{
					}

					void IsWriteValue(dynamic value)
					{

						if (Dictionary.Count != 0)
						{
							var source = (SourceClass)Dictionary?[value.Key];

							var newValue = new
							{
								source.Count,
								value.Value
							};
							var tempValue = BaseClass.SetValue(null, newValue, value.Key);

							source.SetValue(tempValue);
						}
					}
				}
				catch 
				{
					if (!IsErrorWriteValue)
					{
						Log.Log.Write($"Destination WriteValue Error {val.GetType()}", "Warrning");
						IsErrorWriteValue = true;
					}
				}
			}

			internal override void Qality(bool init=false)
			{
				BaseClass.UpdateServer(NameDataObj, ref _iedServer, ref _iedModel, init);
			}

			public override event ClassStateHandlerWrite WriteValueHandler;
			public override event ClassStateHandlerWrite ReadValueHandler;

			protected override void ResetReady()
			{
				foreach (var item in ReadValue.ToList())
				{
					ReadValue[item.Key] = false;
				}

				IsReady = true; 
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
