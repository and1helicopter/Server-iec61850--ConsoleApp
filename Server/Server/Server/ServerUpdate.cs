using System;
using System.Collections;
using IEC61850.Common;
using IEC61850.Server;
using ServerLib.DataClasses;
using ServerLib.Update;

namespace ServerLib.Server
{
	public static partial class ServerIEC61850
	{
		private static void InitHandlersClass(IedServer iedServer, IedModel iedModel)
		{
			foreach (var itemDataObject in UpdateDataObj.UpdateListDestination)
			{
				var temp = (DataObject)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj);

				if (itemDataObject.BaseClass.GetType() == typeof(SpcClass))
				{
					iedServer.SetWaitForExecutionHandler(temp, (controlObject, parameter, val, test, check) =>
						test ? ControlHandlerResult.FAILED : ControlHandlerResult.OK, null);

					iedServer.SetControlHandler(temp, (controlObject, parameter, ctlVal, test) =>
					{
						if (ctlVal.GetType() != MmsType.MMS_BOOLEAN)
							return ControlHandlerResult.FAILED;

						itemDataObject.WriteValue(ctlVal.GetBoolean());

						return ControlHandlerResult.OK;
					}, null);
				}
				else if (itemDataObject.BaseClass.GetType() == typeof(IncClass))
				{
					iedServer.SetWaitForExecutionHandler(temp, (controlObject, parameter, val, test, check) =>
						test ? ControlHandlerResult.FAILED : ControlHandlerResult.OK, null);

					iedServer.SetControlHandler(temp, (controlObject, parameter, ctlVal, test) =>
					{
						if (ctlVal.GetType() != MmsType.MMS_INTEGER)
							return ControlHandlerResult.FAILED;

						itemDataObject.WriteValue(ctlVal.ToInt32());

						return ControlHandlerResult.OK;
					}, null);
				}
			}

			//foreach (var itemGetObjects in UpdateDataObj.ClassGetObjects)
			//{
			//	foreach (var itemDataClass in itemGetObjects.DataClass)
			//	{
			//		var temp = (DataObject)iedModel.GetModelNodeByShortObjectReference(itemDataClass.NameDataObj);

			//		if (itemDataClass.DataObj.GetType() == typeof(SpcClass))
			//		{
			//			//switch (((SpcClass) itemDataClass.DataObj).ctlModel.CtlModels)

			//			iedServer.SetWaitForExecutionHandler(temp, (controlObject, parameter, val, test, check) => 
			//				test ? ControlHandlerResult.FAILED : ControlHandlerResult.OK, null);

			//			iedServer.SetControlHandler(temp, (controlObject, parameter, ctlVal, test) =>
			//			{
			//				if (ctlVal.GetType() != MmsType.MMS_BOOLEAN )
			//					return ControlHandlerResult.FAILED;

			//				UpdateSPC(UpdateDataObj.ClassGetObjects.IndexOf(itemGetObjects), itemGetObjects.DataClass.IndexOf(itemDataClass), ctlVal.GetBoolean());

			//				return ControlHandlerResult.OK;
			//			}, null);
			//		}

			//		else if (itemDataClass.DataObj.GetType() == typeof(IncClass))
			//		{
			//			iedServer.SetWaitForExecutionHandler(temp, (controlObject, parameter, val, test, check) => 
			//				test ? ControlHandlerResult.FAILED : ControlHandlerResult.OK, null);

			//			iedServer.SetControlHandler(temp, (controlObject, parameter, ctlVal, test) =>
			//			{
			//				if (ctlVal.GetType() != MmsType.MMS_INTEGER)
			//					return ControlHandlerResult.FAILED;

			//				UpdateINC(UpdateDataObj.ClassGetObjects.IndexOf(itemGetObjects), ctlVal.ToInt32());
			//				return ControlHandlerResult.OK;
			//			}, null);
			//		}
			//	}
			//}

			foreach (var itemMod in UpdateDataObj.ModClass.PathModList)
			{
				var temp = (DataObject)iedModel.GetModelNodeByShortObjectReference(itemMod);

				iedServer.SetWaitForExecutionHandler(temp, (controlObject, parameter, val, test, check) =>
					test ? ControlHandlerResult.FAILED : ControlHandlerResult.OK, null);

				iedServer.SetControlHandler(temp, (controlObject, parameter, ctlVal, test) =>
				{
					if (ctlVal.GetType() != MmsType.MMS_INTEGER)
						return ControlHandlerResult.FAILED;

					UpdateMod(ctlVal.ToInt32());
					return ControlHandlerResult.OK;
				}, null);
			}
		}

		private static void UpdateMod(int value)
		{
			switch (value)
			{
				case 1:
					UpdateDataObj.StatusChange(1);
					ModBus.UpdateModBus.StopUpdate = false;//Запуск сервисов обмена
					break;
				case 5:
					UpdateDataObj.StatusChange(5);
					ModBus.UpdateModBus.StopUpdate = true;//Остановка сервисов обмен
					break;
			}
		}

		// private static void UpdateSPC(int indexClassGetObjects, int indexDataClass, bool value)
		//{
		//	BitArray bitArray = new BitArray(UpdateDataObj.ClassGetObjects[indexClassGetObjects].BitArray.BitArray);
		//	var index = UpdateDataObj.ClassGetObjects[indexClassGetObjects].DataClass[indexDataClass].IndexDataOBj;
		//	bitArray.Set(index, value);

		//	//Записать значение на плату

		//	ushort temp = 0;
		//	for (int i = 0; i < bitArray.Count; i++)
		//	{
		//		if (bitArray[i])
		//		{
		//			temp |= (ushort)(1 << i);
		//		}
		//	}
		//	ushort[] val = { temp };

		//	UpdateModBus.UpdateModBus.DataSetRequest(indexClassGetObjects, val);
		//}

		//private static void UpdateINC(int indexClassGetObjects, int value)
		//{
		//	//Записать значение на плату
		//	ushort[] val = { (ushort)value, (ushort)(value >> 16) };
		//	UpdateModBus.UpdateModBus.DataSetRequest(indexClassGetObjects, val);
		//}

		//private static void StaticUpdateData(IedServer iedServer, IedModel iedModel)
		//{
		//	foreach (var itemDefultDataObj in DataObj.StructDataObj)
		//	{
		//		InitStaticUpdateData(itemDefultDataObj.Type, itemDefultDataObj.Value, itemDefultDataObj.Path, iedServer, iedModel);
		//	}
		//}

		private static void InitStaticUpdateData(string format, string value, string path, IedServer iedServer, IedModel iedModel)
		{
			if (format.ToUpper() == @"bool".ToUpper())
			{
				UpdateBool(path, value, iedServer, iedModel);
				return;
			}
			if (format.ToUpper() == @"int".ToUpper())
			{
				UpdateInt(path, value, iedServer, iedModel);
				return;
			}
			if (format.ToUpper() == @"float".ToUpper())
			{
				UpdateFloat(path, value, iedServer, iedModel);
				return;
			}
			if (format.ToUpper() == @"string".ToUpper())
			{
				UpdateString(path, value, iedServer, iedModel);
				return;
			}
			if (format.ToUpper() == @"datetime".ToUpper())
			{
				UpdateDateTime(path, value, iedServer, iedModel);
				return;
			}
			if (format.ToUpper() == @"ushort".ToUpper())
			{
				UpdateUshort(path, value, iedServer, iedModel);
			}
		}

		private static void UpdateBool(string path, string value, IedServer iedServer, IedModel iedModel)
		{
			iedServer.LockDataModel();

			try
			{
				bool str = Convert.ToBoolean(value);
				iedServer.UpdateBooleanAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
			}
			catch
			{
				// ignored
			}

			iedServer.UnlockDataModel();
		}

		private static void UpdateInt(string path, string value, IedServer iedServer, IedModel iedModel)
		{
			iedServer.LockDataModel();

			try
			{
				int str = Convert.ToInt32(value);
				iedServer.UpdateInt32AttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
			}
			catch
			{
				// ignored
			}

			iedServer.UnlockDataModel();
		}

		private static void UpdateFloat(string path, string value, IedServer iedServer, IedModel iedModel)
		{
			iedServer.LockDataModel();

			try
			{
				float str = Convert.ToSingle(value);
				iedServer.UpdateFloatAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
			}
			catch
			{
				// ignored
			}

			iedServer.UnlockDataModel();
		}

		private static void UpdateString(string path, string value, IedServer iedServer, IedModel iedModel)
		{
			iedServer.LockDataModel();

			try
			{
				string str = Convert.ToString(value);
				iedServer.UpdateVisibleStringAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
			}
			catch
			{
				// ignored
			}

			iedServer.UnlockDataModel();
		}

		private static void UpdateDateTime(string path, string value, IedServer iedServer, IedModel iedModel)
		{
			iedServer.LockDataModel();

			try
			{
				DateTime str = Convert.ToDateTime(value);
				iedServer.UpdateUTCTimeAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
			}
			catch
			{
				// ignored
			}

			iedServer.UnlockDataModel();
		}

		private static void UpdateUshort(string path, string value, IedServer iedServer, IedModel iedModel)
		{
			iedServer.LockDataModel();

			try
			{
				ushort str = Convert.ToUInt16(value);
				iedServer.UpdateQuality((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
			}
			catch
			{
				// ignored
			}

			iedServer.UnlockDataModel();
		}

		//public static void UpdateDataServer(UpdateDataObj.DataObject itemDataObject)
		//{
		//	if (itemDataObject.DataObj.GetType() == typeof(SpsClass))
		//	{
		//		SPS_ClassUpdate(itemDataObject, _iedServer, _iedModel);
		//	}
		//	else if (itemDataObject.DataObj.GetType() == typeof(InsClass))
		//	{
		//		INS_ClassUpdate(itemDataObject, _iedServer, _iedModel);
		//	}
		//	else if (itemDataObject.DataObj.GetType() == typeof(ActClass))
		//	{
		//		ACT_ClassUpdate(itemDataObject, _iedServer, _iedModel);
		//	}
		//	else if (itemDataObject.DataObj.GetType() == typeof(BcrClass))
		//	{
		//		BCR_ClassUpdate(itemDataObject, _iedServer, _iedModel);
		//	}
		//	else if (itemDataObject.DataObj.GetType() == typeof(MvClass))
		//	{
		//		MV_ClassUpdate(itemDataObject, _iedServer, _iedModel);
		//	}
		//	else if (itemDataObject.DataObj.GetType() == typeof(SpcClass))
		//	{
		//		SPC_ClassUpdate(itemDataObject, _iedServer, _iedModel);
		//	}
		//	else if (itemDataObject.DataObj.GetType() == typeof(IncClass))
		//	{
		//		INC_ClassUpdate(itemDataObject, _iedServer, _iedModel);
		//	}
		//}

		//#region Классы общих данных для информации о состоянии
		//private static void SPS_ClassUpdate(UpdateDataObj.DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		//{
		//	((SpsClass)itemDataObject.DataObj).QualityCheckClass();

		//	var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".stVal");
		//	var stValVal = Convert.ToBoolean(((SpsClass)itemDataObject.DataObj).stVal);
		//	iedServer.UpdateBooleanAttributeValue(stValPath, stValVal);

		//	var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		//	var tVal = Convert.ToDateTime(((SpsClass)itemDataObject.DataObj).t);
		//	iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		//	var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		//	var qVal = Convert.ToUInt16(((SpsClass)itemDataObject.DataObj).q.Validity);
		//	iedServer.UpdateQuality(qPath, qVal);
		//}

		//private static void INS_ClassUpdate(UpdateDataObj.DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		//{
		//	((InsClass)itemDataObject.DataObj).QualityCheckClass();

		//	var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".stVal");
		//	var stValVal = Convert.ToInt32(((InsClass)itemDataObject.DataObj).stVal);
		//	iedServer.UpdateInt32AttributeValue(stValPath, stValVal);

		//	var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		//	var tVal = Convert.ToDateTime(((InsClass)itemDataObject.DataObj).t);
		//	iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		//	var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		//	var qVal = Convert.ToUInt16(((InsClass)itemDataObject.DataObj).q.Validity);
		//	iedServer.UpdateQuality(qPath, qVal);
		//}

		//private static void ACT_ClassUpdate(UpdateDataObj.DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		//{
		//	((ActClass)itemDataObject.DataObj).QualityCheckClass();

		//	var generalPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".general");
		//	var generalVal = Convert.ToBoolean(((ActClass)itemDataObject.DataObj).general);
		//	iedServer.UpdateBooleanAttributeValue(generalPath, generalVal);

		//	var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		//	var tVal = Convert.ToDateTime(((ActClass)itemDataObject.DataObj).t);
		//	iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		//	var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		//	var qVal = Convert.ToUInt16(((ActClass)itemDataObject.DataObj).q.Validity);
		//	iedServer.UpdateQuality(qPath, qVal);
		//}

		//private static void BCR_ClassUpdate(UpdateDataObj.DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		//{
		//	((BcrClass)itemDataObject.DataObj).QualityCheckClass();

		//	var actValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".actVal");
		//	var actValVal = Convert.ToInt32(((BcrClass)itemDataObject.DataObj).actVal);
		//	iedServer.UpdateInt32AttributeValue(actValPath, actValVal);

		//	var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		//	var tVal = Convert.ToDateTime(((BcrClass)itemDataObject.DataObj).t);
		//	iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		//	var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		//	var qVal = Convert.ToUInt16(((BcrClass)itemDataObject.DataObj).q.Validity);
		//	iedServer.UpdateQuality(qPath, qVal);
		//}
		//#endregion

		//#region  Классы общих данных для информации об измеряемой величине
		//private static void MV_ClassUpdate(UpdateDataObj.DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		//{
		//	((MvClass)itemDataObject.DataObj).QualityCheckClass();

		//	var magPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".mag.f");
		//	var magVal = Convert.ToSingle(((MvClass)itemDataObject.DataObj).Mag.AnalogueValue.f);
		//	iedServer.UpdateFloatAttributeValue(magPath, magVal);

		//	var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		//	var tVal = Convert.ToDateTime(((MvClass)itemDataObject.DataObj).t);
		//	iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		//	var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		//	var qVal = Convert.ToUInt16(((MvClass)itemDataObject.DataObj).q.Validity);
		//	iedServer.UpdateQuality(qPath, qVal);
		//}
		//#endregion

		//#region  Спецификации класса общих данных для управления состоянием и информации о состоянии
		//private static void SPC_ClassUpdate(UpdateDataObj.DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		//{
		//	((SpcClass)itemDataObject.DataObj).QualityCheckClass();

		//	var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".stVal");
		//	var stValVal = Convert.ToBoolean(((SpcClass)itemDataObject.DataObj).stVal);
		//	iedServer.UpdateBooleanAttributeValue(stValPath, stValVal);

		//	var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		//	var tVal = Convert.ToDateTime(((SpcClass)itemDataObject.DataObj).t);
		//	iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		//	var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		//	var qVal = Convert.ToUInt16(((SpcClass)itemDataObject.DataObj).q.Validity);
		//	iedServer.UpdateQuality(qPath, qVal);
		//}

		//private static void INC_ClassUpdate(UpdateDataObj.DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		//{
		//	((IncClass)itemDataObject.DataObj).QualityCheckClass();

		//	var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".stVal");
		//	var stValVal = Convert.ToInt32(((IncClass)itemDataObject.DataObj).stVal);
		//	iedServer.UpdateInt32AttributeValue(stValPath, stValVal);

		//	var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		//	var tVal = Convert.ToDateTime(((IncClass)itemDataObject.DataObj).t);
		//	iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		//	var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		//	var qVal = Convert.ToUInt16(((IncClass)itemDataObject.DataObj).q.Validity);
		//	iedServer.UpdateQuality(qPath, qVal);
		//}
		//#endregion

		public static void INT_UpdateStatus(string stValPath, int stVal)
		{
			var stPath = (DataAttribute)_iedModel.GetModelNodeByShortObjectReference(stValPath + @".stVal");
			_iedServer.UpdateInt32AttributeValue(stPath, stVal);

			var tPath = (DataAttribute)_iedModel.GetModelNodeByShortObjectReference(stValPath + @".t");
			_iedServer.UpdateUTCTimeAttributeValue(tPath, DateTime.Now);
		}
	}
}
