using System;
using IEC61850.Common;
using IEC61850.Server;
using Server.DataClasses;

namespace Server.Update
{
    public static partial class UpdateDataObj
    {
		public static void InitControlClass(IedServer iedServer, IedModel iedModel)
	    {
			foreach (var item in DataClassSet)
			{
				var temp = (IEC61850.Server.DataObject)iedModel.GetModelNodeByShortObjectReference(item.NameDataObj);

				switch (item.ClassDataObj)
				{
					case "SPC":
						iedServer.SetControlHandler(temp, delegate (IEC61850.Server.DataObject controlObject, object parameter, MmsValue ctlVal, bool test)
						{
							UpdateSPC(ctlVal.GetBoolean());
							return ControlHandlerResult.OK;
						}, null);
						break;
					case "INC":
						break;
					case "APC":
						break;
				}
			}
	    }

	    private static void UpdateSPC(bool value)
		{
			//Записать значение на плату

		}
		
		public static void StaticUpdateData(IedServer iedServer, IedModel iedModel)
        {
            iedServer.LockDataModel();

            foreach (var itemDefultDataObj in DataObj.StructDataObj)
            {
                InitStaticUpdateData(itemDefultDataObj.Type, itemDefultDataObj.Value, itemDefultDataObj.Path, iedServer, iedModel);
            }

            iedServer.UnlockDataModel();
        }

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
            bool str = Convert.ToBoolean(value);
			iedServer.UpdateBooleanAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
        }

        private static void UpdateInt(string path, string value, IedServer iedServer, IedModel iedModel)
        {
            int str = Convert.ToInt32(value);
            iedServer.UpdateInt32AttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
        }

        private static void UpdateFloat(string path, string value, IedServer iedServer, IedModel iedModel)
        {
            float str = Convert.ToSingle(value);
            iedServer.UpdateFloatAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
        }

        private static void UpdateString(string path, string value, IedServer iedServer, IedModel iedModel)
        {
            string str = Convert.ToString(value);
            iedServer.UpdateVisibleStringAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
        }

        private static void UpdateDateTime(string path, string value, IedServer iedServer, IedModel iedModel)
        {
            DateTime str = Convert.ToDateTime(value);
            iedServer.UpdateUTCTimeAttributeValue((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
        }

        private static void UpdateUshort(string path, string value, IedServer iedServer, IedModel iedModel)
        {
            ushort str = Convert.ToUInt16(value);
            iedServer.UpdateQuality((DataAttribute)iedModel.GetModelNodeByShortObjectReference(path), str);
        }

        public static void UpdateData(IedServer iedServer, IedModel iedModel)
        {
            foreach (var itemDataObject in DataClassGet)
            {
                switch (itemDataObject.ClassDataObj)
                {
	                case @"SPS":
		                SPS_ClassUpdate(itemDataObject, iedServer, iedModel);
		                continue;
	                case @"INS":
		                INS_ClassUpdate(itemDataObject, iedServer, iedModel);
		                continue;
					case @"ACT":
						ACT_ClassUpdate(itemDataObject, iedServer, iedModel);
						continue;
					case @"BCR":
						BCR_ClassUpdate(itemDataObject, iedServer, iedModel);
						continue;
	                case @"MV":
		                MV_ClassUpdate(itemDataObject, iedServer, iedModel);
		                continue;
	                case @"SPC":
		                SPC_ClassUpdate(itemDataObject, iedServer, iedModel);
		                continue;
				}
            }
        }

		#region Классы общих данных для информации о состоянии
		private static void SPS_ClassUpdate(DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
	    {
		    ((SpsClass)itemDataObject.DataObj).QualityCheckClass();

		    var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".stVal");
		    var stValVal = Convert.ToBoolean(((SpsClass)itemDataObject.DataObj).stVal);
		    iedServer.UpdateBooleanAttributeValue(stValPath, stValVal);

		    var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		    var tVal = Convert.ToDateTime(((SpsClass)itemDataObject.DataObj).t);
		    iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		    var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		    var qVal = Convert.ToUInt16(((SpsClass)itemDataObject.DataObj).q.Validity);
		    iedServer.UpdateQuality(qPath, qVal);
	    }

	    private static void INS_ClassUpdate(DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
	    {
		    ((InsClass)itemDataObject.DataObj).QualityCheckClass();

		    var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".stVal");
		    var stValVal = Convert.ToInt32(((InsClass)itemDataObject.DataObj).stVal);
		    iedServer.UpdateInt32AttributeValue(stValPath, stValVal);

		    var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		    var tVal = Convert.ToDateTime(((InsClass)itemDataObject.DataObj).t);
		    iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		    var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		    var qVal = Convert.ToUInt16(((InsClass)itemDataObject.DataObj).q.Validity);
		    iedServer.UpdateQuality(qPath, qVal);
	    }

	    private static void ACT_ClassUpdate(DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
	    {
		    ((ActClass)itemDataObject.DataObj).QualityCheckClass();

		    var generalPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".general");
		    var generalVal = Convert.ToBoolean(((ActClass)itemDataObject.DataObj).general);
		    iedServer.UpdateBooleanAttributeValue(generalPath, generalVal);

		    var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		    var tVal = Convert.ToDateTime(((ActClass)itemDataObject.DataObj).t);
		    iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		    var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		    var qVal = Convert.ToUInt16(((ActClass)itemDataObject.DataObj).q.Validity);
		    iedServer.UpdateQuality(qPath, qVal);
	    }

	    private static void BCR_ClassUpdate(DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
	    {
		    ((BcrClass)itemDataObject.DataObj).QualityCheckClass();

		    var actValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".actVal");
		    var actValVal = Convert.ToInt32(((BcrClass)itemDataObject.DataObj).actVal);
		    iedServer.UpdateInt32AttributeValue(actValPath, actValVal);

		    var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		    var tVal = Convert.ToDateTime(((BcrClass)itemDataObject.DataObj).t);
		    iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		    var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		    var qVal = Convert.ToUInt16(((BcrClass)itemDataObject.DataObj).q.Validity);
		    iedServer.UpdateQuality(qPath, qVal);
	    }
		#endregion

		#region  Классы общих данных для информации об измеряемой величине
		private static void MV_ClassUpdate(DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
		{
			((MvClass)itemDataObject.DataObj).QualityCheckClass();

			var magPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".mag.f");
			var magVal = Convert.ToSingle(((MvClass)itemDataObject.DataObj).Mag.AnalogueValue.f);
			iedServer.UpdateFloatAttributeValue(magPath, magVal);

			var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
			var tVal = Convert.ToDateTime(((MvClass)itemDataObject.DataObj).t);
			iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

			var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
			var qVal = Convert.ToUInt16(((MvClass)itemDataObject.DataObj).q.Validity);
			iedServer.UpdateQuality(qPath, qVal);
		}
		#endregion

		#region  Спецификации класса общих данных для управления состоянием и информации о состоянии
		private static void SPC_ClassUpdate(DataObject itemDataObject, IedServer iedServer, IedModel iedModel)
	    {
		    ((SpcClass)itemDataObject.DataObj).QualityCheckClass();

		    var magPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".stVal");
		    var magVal = Convert.ToSingle(((SpcClass)itemDataObject.DataObj).stVal);
		    iedServer.UpdateFloatAttributeValue(magPath, magVal);

		    var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".t");
		    var tVal = Convert.ToDateTime(((SpcClass)itemDataObject.DataObj).t);
		    iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

		    var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(itemDataObject.NameDataObj + @".q");
		    var qVal = Convert.ToUInt16(((SpcClass)itemDataObject.DataObj).q.Validity);
		    iedServer.UpdateQuality(qPath, qVal);
	    }
	    #endregion
	}
}
