using System;
using System.Collections;
using System.ComponentModel;
using IEC61850.Common;
using IEC61850.Server;

namespace ServerLib.DataClasses
{
	public abstract class BaseClass
	{
		public DateTime t;
		public Quality q;

		public abstract void UpdateClass(dynamic value);
		public abstract void UpdateServer(string path, IedServer iedServer, IedModel iedModel);
		public abstract void InitServer(string path, IedServer iedServer, IedModel iedModel);
		public abstract void QualityCheckClass();

		internal void GetBoolean(dynamic value, ref bool? obj)
		{
			var dataValue = new BitArray(BitConverter.GetBytes(value.Value)); ;
			var indexValue = value.Index;

			var tempValue = dataValue[indexValue];

			obj = tempValue;
		}

		internal void GetDoublePoint(dynamic value, ref DoublePoint? obj)
		{
			var dataValue = new BitArray(BitConverter.GetBytes(value.Value));

			var indexValue = value.Index;

			var tempValue0 = dataValue[indexValue] == true ? 0b01 : 0b00;
			var tempValue1 = dataValue[indexValue + 1] == true ? 0b10 : 0b00;

			var tempValue = tempValue0 + tempValue1;

			obj = (DoublePoint)tempValue;
		}

		internal void GetInt32(dynamic value, ref Int32? obj)
		{
			ushort[] xxxx = value.Value;
			Int32 tempValue = 0;

			for (int i = 0; i < xxxx.Length; i++)
			{
				var temp = (Int32)xxxx[i];
				tempValue += (temp) << (16 * (xxxx.Length - 1 - i));
			}

			obj = tempValue;
		}

		internal void GetDirectionalProtection(dynamic value, ref DirectionalProtection? obj)
		{

			var dataValue = new BitArray(BitConverter.GetBytes(value.Value));

			var indexValue = value.Index;

			var tempValue0 = dataValue[indexValue] == true ? 0b01 : 0b00;
			var tempValue1 = dataValue[indexValue + 1] == true ? 0b10 : 0b00;

			var tempValue = tempValue0 + tempValue1;

			obj = (DirectionalProtection)tempValue;
		}

		internal void GetSecurityViolation(dynamic value, ref SecurityViolation? obj)
		{
			var dataValue = new BitArray(BitConverter.GetBytes(value.Value[0]));

			var tempValue0 = dataValue[0] ? 0b001 : 0b000;
			var tempValue1 = dataValue[1] ? 0b010 : 0b000;
			var tempValue2 = dataValue[2] ? 0b100 : 0b000;

			var tempValue = tempValue0 + tempValue1 + tempValue2;

			tempValue = tempValue > 4 ? 0 : tempValue;

			obj = (SecurityViolation)tempValue;
		}

		internal void GetUInt32(dynamic value, ref UInt32? obj)
		{
			ushort[] xxxx = value.Value;
			UInt32 tempValue = 0;

			for (int i = 0; i < xxxx.Length; i++)
			{
				var temp = (UInt32)xxxx[i];
				tempValue += (temp) << (16 * (xxxx.Length - 1 - i));
			}

			obj = tempValue;
		}

		internal void GetInt64(dynamic value, ref Int64? obj)
		{
			if (value != null)
			{
				ushort[] xxxx = value.Value;
				Int64 tempValue = 0;

				for (int i = 0; i < xxxx.Length; i++)
				{
					var temp = (Int64) xxxx[i];
					tempValue += (temp) << (16 * (xxxx.Length - 1 - i));
				}

				obj = tempValue;
			}
		}

		internal void SetBooleanValue(string name, Boolean? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToBoolean(value);
				iedServer.UpdateBooleanAttributeValue(namePath, val);
			}
		}

		internal void SetDataTimeValue(string name, DateTime? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToDateTime(value);
				iedServer.UpdateUTCTimeAttributeValue(namePath, val);
			}
		}

		internal void SetQualityValue(string name, UInt16? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToUInt16(value);
				iedServer.UpdateQuality(namePath, val);
			}
		}

		internal void SetStringValue(string name, String value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = value;
				iedServer.UpdateVisibleStringAttributeValue(namePath, val);
			}
		}

		internal void SetDoublePointValue(string name, DoublePoint? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToInt32(value);
				iedServer.UpdateInt32AttributeValue(namePath, val);
			}
		}

		internal void SetInt32Value(string name, Int32? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToInt32(value);
				iedServer.UpdateInt32AttributeValue(namePath, val);
			}
		}

		internal void SetUInt32Value(string name, UInt32? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToUInt32(value);
				iedServer.UpdateAttributeValue(namePath, new MmsValue(val));
			}
		}

		internal void SetDoublePointValue(string name, DirectionalProtection? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToInt32(value);
				iedServer.UpdateAttributeValue(namePath, new MmsValue(val));
			}
		}

		internal void SetSecurityviolationValue(string name, SecurityViolation ? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToInt32(value);
				iedServer.UpdateInt32AttributeValue(namePath, val);
			}
		}

		internal void SetInt64Value(string name, Int64? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToInt64(value);
				iedServer.UpdateAttributeValue(namePath, new MmsValue(val));
			}
		}

		internal void SetSingleValue(string name, Single? value, IedModel iedModel, IedServer iedServer)
		{
			if (value != null)
			{
				var namePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(name);
				var val = Convert.ToSingle(value);
				iedServer.UpdateAttributeValue(namePath, new MmsValue(val));
			}
		}

		protected BaseClass()
		{
			t = DateTime.Now;
			q = new Quality();
		}
	}

	#region Классы общих данных для информации о состоянии
	//Двоичное состояние SPS
	public class SpsClass : BaseClass
	{
		public Boolean? stVal;
		public String d = null;

		public override void UpdateClass(dynamic value)
		{
			try
			{
				switch (value.Key)
				{
					case "stVal":
						GetBoolean(value, ref stVal);
						break;
				}

				t = DateTime.Now;
				q.UpdateQuality(t);
			}
			catch
			{
				Log.Log.Write("SPS UpdateClass", "Error");
			}
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			SetBooleanValue(path + @".stVal", stVal, iedModel, iedServer);
			SetDataTimeValue(path + @".t", t, iedModel, iedServer);
			SetQualityValue(path + @".q", q.Validity, iedModel, iedServer);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			SetStringValue(path + @".d", d, iedModel, iedServer);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}

	//Дублированное состояние DPS
	public class DpsClass : BaseClass
	{
		public DoublePoint? stVal;
		public String d = null;
		
		public override void UpdateClass(dynamic value)
		{
			try
			{
				switch (value.Key)
				{
					case "stVal":
						GetDoublePoint(value, ref stVal);
						break;
				}

				t = DateTime.Now;
				q.UpdateQuality(t);
			}
			catch
			{
				Log.Log.Write("DPS UpdateClass", "Error");
			}
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			SetDoublePointValue(path + @".stVal", stVal, iedModel, iedServer);
			SetDataTimeValue(path + @".t", t, iedModel, iedServer);
			SetQualityValue(path + @".q", q.Validity, iedModel, iedServer);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			SetStringValue(path + @".d", d, iedModel, iedServer);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}

	//Целочисленное состояние
	public class InsClass : BaseClass
	{
		public Int32? stVal;
		public String d = null;

		public override void UpdateClass(dynamic value)
		{
			try
			{
				switch (value.Key)
				{
					case "stVal":
						GetInt32(value, ref stVal);
						break;
				}

				t = DateTime.Now;
				q.UpdateQuality(t);
			}
			catch
			{
				Log.Log.Write("INS UpdateClass", "Error");
			}
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			SetInt32Value(path + @".stVal", stVal, iedModel, iedServer);
			SetDataTimeValue(path + @".t", t, iedModel, iedServer);
			SetQualityValue(path + @".q", q.Validity, iedModel, iedServer);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			SetStringValue(path + @".d", d, iedModel, iedServer);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}

	//Сведения об активации защиты
	public class ActClass : BaseClass
	{
		public Boolean? general;
		public String d = null;

		//Опциональные классы
		public Boolean? phsA;
		public Boolean? phsB;
		public Boolean? phsC;
		public Boolean? neut;

		public override void UpdateClass(dynamic value)
		{
			try
			{
				switch (value.Key)
				{
					case "general":
						GetBoolean(value, ref general);
						break;
					case "phsA":
						GetBoolean(value, ref phsA);
						break;
					case "phsB":
						GetBoolean(value, ref phsB);
						break;
					case "phsC":
						GetBoolean(value, ref phsC);
						break;
					case "neut":
						GetBoolean(value, ref neut);
						break;
				}

				t = DateTime.Now;
				q.UpdateQuality(t);
			}
			catch 
			{
				Log.Log.Write("ACT UpdateClass", "Error");
			}
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			SetBooleanValue(path + @".general", general, iedModel, iedServer);
			SetBooleanValue(path + @".phsA", phsA, iedModel, iedServer);
			SetBooleanValue(path + @".phsB", phsB, iedModel, iedServer);
			SetBooleanValue(path + @".phsC", phsC, iedModel, iedServer);
			SetBooleanValue(path + @".neut", neut, iedModel, iedServer);

			SetDataTimeValue(path + @".t", t, iedModel, iedServer);
			SetQualityValue(path + @".q", q.Validity, iedModel, iedServer);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			SetStringValue(path + @".d", d, iedModel, iedServer);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}

	////Сведения об активации направленной защиты
	public class AcdClass : BaseClass
	{
		public Boolean? general;
		public DirectionalProtection? dirGeneral;
		public String d = null;

		//Опциональные классы
		public Boolean? phsA = null;
		public Boolean? phsB = null;
		public Boolean? phsC = null;
		public Boolean? neut = null;

		public DirectionalProtection? dirPhsA = null;
		public DirectionalProtection? dirPhsB = null;
		public DirectionalProtection? dirPhsC = null;
		public DirectionalProtection? dirNeut = null;

		public override void UpdateClass(dynamic value)
		{
			try
			{
				switch (value.Key)
				{
					case "general":
						GetBoolean(value, ref general);
						break;
					case "dirGeneral":
						GetDirectionalProtection(value, ref dirGeneral);
						break;
					case "phsA":
						GetBoolean(value, ref general);
						break;
					case "dirPhsA":
						GetDirectionalProtection(value, ref dirGeneral);
						break;
					case "phsB":
						GetBoolean(value, ref general);
						break;
					case "dirPhsB":
						GetDirectionalProtection(value, ref dirGeneral);
						break;
					case "phsC":
						GetBoolean(value, ref general);
						break;
					case "dirPhsC":
						GetDirectionalProtection(value, ref dirGeneral);
						break;
					case "neut":
						GetBoolean(value, ref general);
						break;
					case "dirNeut":
						GetDirectionalProtection(value, ref dirGeneral);
						break;
				}

				t = DateTime.Now;
				q.UpdateQuality(t);
			}
			catch
			{
				Log.Log.Write("ACD UpdateClass", "Error");
			}
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			SetBooleanValue(path + @".general", general, iedModel, iedServer);
			SetBooleanValue(path + @".phsA", phsA, iedModel, iedServer);
			SetBooleanValue(path + @".phsB", phsB, iedModel, iedServer);
			SetBooleanValue(path + @".phsC", phsC, iedModel, iedServer);
			SetBooleanValue(path + @".neut", neut, iedModel, iedServer);

			SetDoublePointValue(path + @".dirGeneral", dirGeneral, iedModel, iedServer);
			SetDoublePointValue(path + @".dirPhsA", dirPhsA, iedModel, iedServer);
			SetDoublePointValue(path + @".dirPhsB", dirPhsB, iedModel, iedServer);
			SetDoublePointValue(path + @".dirPhsC", dirPhsC, iedModel, iedServer);
			SetDoublePointValue(path + @".dirNeut", dirNeut, iedModel, iedServer);

			SetDataTimeValue(path + @".t", t, iedModel, iedServer);
			SetQualityValue(path + @".q", q.Validity, iedModel, iedServer);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			SetStringValue(path + @".d", d, iedModel, iedServer);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}

	public class SecClass : BaseClass
	{
		public UInt32? cnt;
		public SecurityViolation? sev;
		public String addr = null;
		public String addInfo = null;
		public String d = null;

		public override void UpdateClass(dynamic value)
		{
			switch (value.Key)
			{
				case "cnt":
					GetUInt32(value, ref cnt);
					break;
				case "sev":
					GetSecurityViolation(value, ref sev);
					break;
			}

			t = DateTime.Now;
			q.UpdateQuality(t);
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			SetUInt32Value(path + @".cnt", cnt, iedModel, iedServer);
			SetSecurityviolationValue(path + @".sev", sev, iedModel, iedServer);

			SetDataTimeValue(path + @".t", t, iedModel, iedServer);
			SetQualityValue(path + @".q", q.Validity, iedModel, iedServer);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			SetStringValue(path + @".addr", d, iedModel, iedServer);
			SetStringValue(path + @".addInfo", d, iedModel, iedServer);
			SetStringValue(path + @".d", d, iedModel, iedServer);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}

	//Считывание показаний двоичного счетчика
	public class BcrClass : BaseClass
	{
		public Int64? actVal;
		public Single? pulsQty;
		public Single? Value;

		public String d = null;

		public override void UpdateClass(dynamic value)
		{
			try
			{
				switch (value.Key)
				{
					case "actVal":
						GetInt64(value, ref actVal);
						break;
				}

				pulsQty = Value / actVal;

				t = DateTime.Now;
				q.UpdateQuality(t);
			}
			catch
			{
				Log.Log.Write("BCR UpdateClass", "Error");
			}
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			SetInt64Value(path + @".actVal", actVal, iedModel, iedServer);
			SetSingleValue(path + @".pulsQty", pulsQty, iedModel, iedServer);

			SetDataTimeValue(path + @".t", t, iedModel, iedServer);
			SetQualityValue(path + @".q", q.Validity, iedModel, iedServer);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			SetStringValue(path + @".d", d, iedModel, iedServer);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}
	#endregion

	#region  Классы общих данных для информации об измеряемой величине
	//измеряемые значения
	public class MvClass : BaseClass
	{
		public MagClass Mag;
		public UnitClass Unit;
		public ScaledValueClass sVC;
		public String d;

		public class MagClass
		{
			public AnalogueValueClass AnalogueValue;

			public MagClass()
			{
				AnalogueValue = new AnalogueValueClass();
			}
		}

		public MvClass() : base()
		{
			Mag = new MagClass();
			Unit = new UnitClass();
			sVC = new ScaledValueClass();
			d = "";
		}

		public override void UpdateClass(dynamic value)
		{
			try
			{
				//Может не верно работать
				ushort[] xxxx = value.Value;
				byte[] tempArrayByte = new byte[4];
				for (int i = 0; i < xxxx.Length;)
				{
					var ffff = BitConverter.GetBytes(xxxx[i / 2]);
					tempArrayByte[i++] = ffff[0];
					tempArrayByte[i++] = ffff[1];
				}

				if (tempArrayByte.Length != 0)
				{
					var tempValue = BitConverter.ToInt32(tempArrayByte, 0);
					Mag.AnalogueValue.f = Convert.ToSingle(tempValue * sVC.ScaleFactor + sVC.Offset);
				}

				t = DateTime.Now;
				q.UpdateQuality(t);
			}
			catch
			{
				Log.Log.Write("MV UpdateClass", "Error");
			}
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			var magPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".mag.f");
			var magVal = Convert.ToSingle(Mag.AnalogueValue.f);
			iedServer.UpdateFloatAttributeValue(magPath, magVal);

			var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".t");
			var tVal = Convert.ToDateTime(t);
			iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

			var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".q");
			var qVal = Convert.ToUInt16(q.Validity);
			iedServer.UpdateQuality(qPath, qVal);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			var siunitPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".units.SIUnit");
			var siunitVal = Unit.SIUnit;
			iedServer.UpdateInt32AttributeValue(siunitPath, siunitVal);

			var multiplierPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".units.multiplier");
			var multiplierVal = Unit.Multiplier;
			iedServer.UpdateInt32AttributeValue(multiplierPath, multiplierVal);

			var scalePath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".sVC.scaleFactor");
			var scaleVal = sVC.ScaleFactor;
			iedServer.UpdateFloatAttributeValue(scalePath, scaleVal);

			var ofssetPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".sVC.offset");
			var ofssetVal = sVC.Offset;
			iedServer.UpdateFloatAttributeValue(ofssetPath, ofssetVal);

			var dPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".d");
			var dVal = d;
			iedServer.UpdateVisibleStringAttributeValue(dPath, dVal);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}
	}

	//комплексные измеряемые значения
	public class CmvClass : BaseClass
	{
		public VectorClass cVal;
		public UnitClass Unit;
		public ScaledValueClass magSVC;
		public ScaledValueClass angSVC;
		public String d;

		public void ClassFill(int siUnit, int multiplier, float scaleFactorMag, float offsetMag, float scaleFactorAng, float offsetAng, string str)
		{
			Unit.SIUnit = siUnit;
			Unit.Multiplier = multiplier;
			magSVC.ScaleFactor = scaleFactorMag;
			magSVC.Offset = offsetMag;
			angSVC.ScaleFactor = scaleFactorAng;
			angSVC.Offset = offsetAng;
			d = str;
		}

		public override void UpdateClass(object obj)
		{
			//   var item = ()obj;

			//cVal.mag.f = Convert.ToSingle(item.valueMag * magSVC.ScaleFactor + magSVC.Offset);
			//   cVal.ang.f = Convert.ToSingle(item.valueAng * angSVC.ScaleFactor + angSVC.Offset);
			t = DateTime.Now;
			q.UpdateQuality(t);
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			throw new NotImplementedException();
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			throw new NotImplementedException();
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}

		public CmvClass() : base()
		{
			cVal = new VectorClass();
			Unit = new UnitClass();
			magSVC = new ScaledValueClass();
			angSVC = new ScaledValueClass();
			d = "";
		}
	}
	#endregion

	#region Спецификации класса общих данных для управления состоянием и информации о состоянии
	//Класс SPC (недублированное управление и состояние)
	public class SpcClass : BaseClass
	{
		public Boolean ctlVal;
		public Boolean stVal;
		public CtlModelsClass ctlModel;
		public String d;

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".stVal");
			var stValVal = Convert.ToBoolean(stVal);
			iedServer.UpdateBooleanAttributeValue(stValPath, stValVal);

			var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".t");
			var tVal = Convert.ToDateTime(t);
			iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

			var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".q");
			var qVal = Convert.ToUInt16(q.Validity);
			iedServer.UpdateQuality(qPath, qVal);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			var dPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".d");
			var dVal = d;
			iedServer.UpdateVisibleStringAttributeValue(dPath, dVal);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}

		public override void UpdateClass(dynamic value)
		{
			var dataValue = new BitArray(BitConverter.GetBytes(value.Value));
			var indexValue = value.Index;

			var tempValue = dataValue[indexValue];

			stVal = tempValue;
			t = DateTime.Now;
			q.UpdateQuality(t);
		}
	}

	//Класс INC (целочисленное управление и состояние)
	public class IncClass : BaseClass
	{
		public Int32 ctlVal;
		public Int32 stVal;
		public CtlModelsClass ctlModel;
		public String d;

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			QualityCheckClass();

			var stValPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".stVal");
			var stValVal = Convert.ToInt32(stVal);
			iedServer.UpdateInt32AttributeValue(stValPath, stValVal);

			var tPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".t");
			var tVal = Convert.ToDateTime(t);
			iedServer.UpdateUTCTimeAttributeValue(tPath, tVal);

			var qPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".q");
			var qVal = Convert.ToUInt16(q.Validity);
			iedServer.UpdateQuality(qPath, qVal);
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);

			var dPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".d");
			var dVal = d;
			iedServer.UpdateVisibleStringAttributeValue(dPath, dVal);
		}

		public override void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}

		public override void UpdateClass(dynamic value)
		{
			byte[] tempArrayByte = BitConverter.GetBytes(value.Value);

			var tempValue = BitConverter.ToInt32(tempArrayByte, 0);

			stVal = tempValue;
			t = DateTime.Now;
			q.UpdateQuality(t);
		}
	}
	#endregion

	#region Спецификации класса общих данных для описательной информации
	// Класс DPL (паспортная табличка устройства)
	public class DplClass : BaseClass
	{
		public string vendor { get; set; }
		public string hwRev { get; set; }
		public string swRev { get; set; }
		public string serNum { get; set; }
		public string model { get; set; }
		public string location { get; set; }

		public override void UpdateClass(dynamic value)
		{
			throw new NotImplementedException();
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			try
			{
				var vendorPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".vendor");
				var vendorVal = vendor;
				iedServer.UpdateVisibleStringAttributeValue(vendorPath, vendorVal);

				var serNumPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".serNum");
				var serNumVal = serNum;
				iedServer.UpdateVisibleStringAttributeValue(serNumPath, serNumVal);

				var modelPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".model");
				var modelVal = model;
				iedServer.UpdateVisibleStringAttributeValue(modelPath, modelVal);

				var locationPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".location");
				var locationVal = location;
				iedServer.UpdateVisibleStringAttributeValue(locationPath, locationVal);

				var hwRevPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".hwRev");
				var hwRevVal = hwRev;
				iedServer.UpdateVisibleStringAttributeValue(hwRevPath, hwRevVal);

				var swRevPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".swRev");
				var swRevVal = vendor;
				iedServer.UpdateVisibleStringAttributeValue(swRevPath, swRevVal);
			}
			catch
			{
				// ignored
			}
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);
		}

		public override void QualityCheckClass()
		{
			throw new NotImplementedException();
		}
	}

	//Класс LPL (паспортная табличка логического узла)
	public class LplClass : BaseClass
	{
		public string vendor;
		public string swRev;
		public string d;
		public string configRev;


		public override void UpdateClass(dynamic value)
		{
			throw new NotImplementedException();
		}

		public override void UpdateServer(string path, IedServer iedServer, IedModel iedModel)
		{
			try
			{
				var vendorPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".vendor");
				var vendorVal = vendor;
				iedServer.UpdateVisibleStringAttributeValue(vendorPath, vendorVal);

				var swRevPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".swRev");
				var swRevVal = swRev;
				iedServer.UpdateVisibleStringAttributeValue(swRevPath, swRevVal);

				var configRevPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".configRev");
				var configRevVal = configRev;
				iedServer.UpdateVisibleStringAttributeValue(configRevPath, configRevVal);

				var dPath = (DataAttribute)iedModel.GetModelNodeByShortObjectReference(path + @".d");
				var dVal = d;
				iedServer.UpdateVisibleStringAttributeValue(dPath, dVal);
			}
			catch
			{
				// ignored
			}
		}

		public override void InitServer(string path, IedServer iedServer, IedModel iedModel)
		{
			UpdateServer(path, iedServer, iedModel);
		}

		public override void QualityCheckClass()
		{
			throw new NotImplementedException();
		}
	}
	#endregion


}
