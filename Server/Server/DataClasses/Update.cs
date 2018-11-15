using IEC61850.Common;
using IEC61850.Server;

namespace ServerLib.DataClasses
{
	public static class UpdateServer
	{
		/// <summary>
		/// Initialize update (Set default data)
		/// </summary>
		/// <param name="iedServer">Server object</param>
		/// <param name="iedModel">Model object</param>
		public static void InitUpdate(IedServer iedServer, IedModel iedModel)
		{
			foreach (var item in UpdateDataObj.UpdateListDestination)
			{
				item.BaseClass.InitServer(item.NameDataObj, iedServer, iedModel);
			}
		}

		/// <summary>
		/// Set server and model object
		/// </summary>
		/// <param name="iedServer">Server object</param>
		/// <param name="iedModel">Model object</param>
		public static void SetParams(IedServer iedServer, IedModel iedModel)
		{
			UpdateDataObj.SetParamsServer(iedServer, iedModel);
		}

		/// <summary>
		/// Clear data server
		/// </summary>
		public static void Clear()
		{
			UpdateDataObj.SourceList?.Clear();
			UpdateDataObj.UpdateListDestination?.Clear();
			UpdateDataObj.ModHead.DependencesGroupes.Clear();
			UpdateDataObj.SetParamsServer(null, null);
		}

		/// <summary>
		/// Initialize handlers for write data server
		/// </summary>
		/// <param name="iedServer">Server object</param>
		/// <param name="iedModel">Model object</param>
		public static void InitHandlers(IedServer iedServer, IedModel iedModel)
		{			
			foreach (var itemDataObject in UpdateDataObj.UpdateListDestination)
			{
				var temp = (DataObject)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj);

				if (itemDataObject.BaseClass.GetType() == typeof(SpcClass))
				{
					object tempParam = ((SpcClass)itemDataObject.BaseClass).ctlModel;
						
					iedServer.SetCheckHandler(temp,
						(controlObject, parameter, ctlVal, test, interlockCheck, connection) =>
						{
							var result = CheckHandlerResult.ACCEPTED;
							return result;
						},
						tempParam);

					iedServer.SetWaitForExecutionHandler(temp,
						(controlObject, parameter, ctlVal, test, synchroCheck) =>
						{
							var result = ControlHandlerResult.OK;
							return result;
						}, 
						tempParam);

					iedServer.SetControlHandler(temp, 
						(controlObject, parameter, ctlVal, test) =>
						{
							if (ctlVal.GetType() != MmsType.MMS_BOOLEAN)
								return ControlHandlerResult.FAILED;

							if (!test)
							{
								var tempValue = new
								{
									Key = "stVal",
									Value = ctlVal.GetBoolean()
								};
								itemDataObject.WriteValue(tempValue);
							}

							return ControlHandlerResult.OK;
						},
						tempParam);
				}
				else if (itemDataObject.BaseClass.GetType() == typeof(DpcClass))
				{
					object tempParam = ((DpcClass)itemDataObject.BaseClass).ctlModel;
						
					iedServer.SetCheckHandler(temp,
						(controlObject, parameter, ctlVal, test, interlockCheck, connection) =>
						{
							var result = CheckHandlerResult.ACCEPTED;
							return result;
						},
						tempParam);

					iedServer.SetWaitForExecutionHandler(temp,
						(controlObject, parameter, ctlVal, test, synchroCheck) =>
						{
							var result = ControlHandlerResult.OK;
							return result;
						}, 
						tempParam);

					iedServer.SetControlHandler(temp, 
						(controlObject, parameter, ctlVal, test) =>
						{
							if (ctlVal.GetType() != MmsType.MMS_BOOLEAN)
								return ControlHandlerResult.FAILED;

							if (!test)
							{
								var tempValue = new
								{
									Key = "stVal",
									Value = ctlVal.GetBoolean()
								};
								itemDataObject.WriteValue(tempValue);
							}

							return ControlHandlerResult.OK;
						},
						tempParam);
				}
				else if (itemDataObject.BaseClass.GetType() == typeof(IncClass))
				{
					object tempParam = ((IncClass)itemDataObject.BaseClass).ctlModel;
						
					iedServer.SetCheckHandler(temp,
						(controlObject, parameter, ctlVal, test, interlockCheck, connection) =>
						{
							var result = CheckHandlerResult.ACCEPTED;
							return result;
						},
						tempParam);

					iedServer.SetWaitForExecutionHandler(temp,
						(controlObject, parameter, ctlVal, test, synchroCheck) =>
						{
							var result = ControlHandlerResult.OK;
							return result;
						}, 
						tempParam);

					iedServer.SetControlHandler(temp, 
						(controlObject, parameter, ctlVal, test) =>
						{
							if (ctlVal.GetType() != MmsType.MMS_INTEGER)
								return ControlHandlerResult.FAILED;

							if (!test)
							{
								var tempValue = new
								{
									Key = "stVal",
									Value = ctlVal.ToInt32()
								};
								itemDataObject.WriteValue(tempValue);
							}

							return ControlHandlerResult.OK;
						},
						tempParam);
				}
			}
		}

		/// <summary>
		/// Initialize quality and time for server data
		/// </summary>
		public static void InitQualityAndTime()
		{
			foreach (var item in UpdateDataObj.UpdateListDestination)
			{
				item.Qality(true);
			}
		}

		private class ReadDataObjMethodWork: ModBusTaskController.ModBusTaskController.CycleClass.MethodWork
		{
			private static readonly ReadDataObjMethodWork Instance = new ReadDataObjMethodWork();

			internal static ReadDataObjMethodWork GetInstance()
			{
				return Instance;
			}

			internal override void Request(dynamic status)
			{
				var ready = (bool) status;
				UpdateDataObj.SourceList.ForEach(source =>
				{
					if (source.IsReady || ready)
					{
						source.GetValueRequest(Response);
						source.IsReady = false;
					}
				});
			}

			internal override void Response(dynamic value, dynamic source, bool status)
			{
				source.GetValueResponse(value, status);
				source.IsReady = true;
			}
		}

		/// <summary>
		/// Initialize method update data
		/// </summary>
		public static void InitMethodWork()
		{
			ModBusTaskController.ModBusTaskController.CycleClass.AddMethodWork(ReadDataObjMethodWork.GetInstance());
		}
	}
}
