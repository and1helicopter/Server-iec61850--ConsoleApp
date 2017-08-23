using System;

namespace Server.DataClasses
{
    public class BaseClass
    {
        public DateTime t;
        public Quality q;

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
        public Boolean stVal;
        public String d;
        
        public void UpdateClass(DateTime time, bool value)
        {
            stVal = value;
            t = time;
            q.UpdateQuality(time);
        }

        public void QualityCheckClass()
        {
            q.QualityCheckClass(t);
        }

        public SpsClass(bool stval,string strd)
        {
            stVal = stval;
            d = strd;
        }
    }

    //Целочисленное состояние
    public class InsClass : BaseClass
    {
        public Int32 stVal;
        public String d;

        public void UpdateClass(DateTime time, int value)
        {
            stVal = value;
            t = time;
            q.UpdateQuality(time);
        }

        public void QualityCheckClass()
        {
            q.QualityCheckClass(t);
        }

        public InsClass(Int32 stval, string strd)
        {
            stVal = stval;
            d = strd;
        }
    }

	//Сведения об активации защиты
	public class ActClass : BaseClass
	{
		public Boolean general;
		public String d;

		public void UpdateClass(DateTime time, bool value)
		{
			general = value;
			t = time;
			q.UpdateQuality(time);
		}

		public void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}

		public ActClass(bool gen , string strd)
		{
			general = gen;
			d = strd;
		}
	}

	////Сведения об активации направленной защиты
	//public class AcdClass : BaseClass
	//{
	//    public Boolean general;
	//    public Int32 dirGeneral;
	//    public String d;

	//    public AcdClass()
	//    {
	//        general = false;
	//        dirGeneral = 0;
	//        d = "";
	//    }

	//    public void UpdateClass(DateTime time, bool value, string valueDir)
	//    {
	//        general = value;
	//        if (value) dirGeneral = MapDirGeneral(valueDir);
	//        t = time;
	//        q.UpdateQuality(time, value);
	//    }

	//    private ushort MapDirGeneral(string quality)
	//    {
	//        ushort qual = 0;
	//        switch (quality.ToUpper())
	//        {
	//            case "UNKNOWN":
	//                qual = (ushort)ValidityDirGeneral.UNKNOWN;
	//                break;
	//            case "FORWARD":
	//                qual = (ushort)ValidityDirGeneral.FORWARD;
	//                break;
	//            case "BACKWARD":
	//                qual = (ushort)ValidityDirGeneral.BACKWARD;
	//                break;
	//            case "BOTH":
	//                qual = (ushort)ValidityDirGeneral.BOTH;
	//                break;
	//        }
	//        return qual;
	//    }

	//    enum ValidityDirGeneral
	//    {
	//        UNKNOWN = 0,
	//        FORWARD = 1,
	//        BACKWARD = 2,
	//        BOTH = 3
	//    }
	//}

	//Считывание показаний двоичного счетчика
	public class BcrClass : BaseClass
	{
		public Int32 actVal;
		public String d;

		public void UpdateClass(DateTime time, int value)
		{
			actVal = value;
			t = time;
			q.UpdateQuality(time);
		}

		public void QualityCheckClass()
		{
			q.QualityCheckClass(t);
		}

		public BcrClass(int act, string strd)
		{
			actVal = act;
			d = strd;
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

        public void ClassFill(int siUnit, int multiplier, float scaleFactor, float offset, string str)
        {
            Unit.SIUnit = siUnit;
            Unit.Multiplier = multiplier;
            sVC.ScaleFactor = scaleFactor;
            sVC.Offset = offset;
            d = str;
        }

        public void UpdateClass(DateTime time, ulong value)
        {
            Mag.AnalogueValue.f = Convert.ToSingle(value * sVC.ScaleFactor + sVC.Offset);
            t = time;
            q.UpdateQuality(time);
        }

        public void QualityCheckClass()
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

        public void UpdateClass(DateTime time, long valueMag, long valueAng)
        {
            cVal.mag.f = Convert.ToSingle(valueMag * magSVC.ScaleFactor + magSVC.Offset);
            cVal.ang.f = Convert.ToSingle(valueAng * angSVC.ScaleFactor + angSVC.Offset);
            t = time;
            q.UpdateQuality(time);
        }

        public void QualityCheckClass()
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
        public Int32 ctlModel;
        public String d;

        public SpcClass(bool ctlval, bool stval, string ctlmodel, string strd)
        {
            ctlVal = ctlval;
            stVal = stval;
            ctlModel = MapCtlModel(ctlmodel);
            d = strd;
        }

	    public void QualityCheckClass()
	    {
		    q.QualityCheckClass(t);
	    }

		public void UpdateClass(DateTime time, bool ctlValue, bool stValue, string ctrMod)
        {
            ctlVal = ctlValue;
            stVal = stValue;
            ctlModel = MapCtlModel(ctrMod);
            t = time;
            q.UpdateQuality(time);
        }

        private ushort MapCtlModel(string quality)
        {
            ushort qual = 0;
            switch (quality.ToUpper().Replace('-', '_').Replace(" ", ""))
            {
                case "STATUS_ONLY":
                    qual = (ushort)ValidityCtlModel.STATUS_ONLY;
                    break;
                case "DIRECT_WITH_NORMAL_SECURITY":
                    qual = (ushort)ValidityCtlModel.DIRECT_WITH_NORMAL_SECURITY;
                    break;
                case "SBO_WITH_NORMAL_SECURITY":
                    qual = (ushort)ValidityCtlModel.SBO_WITH_NORMAL_SECURITY;
                    break;
                case "DIRECR_WITH_ENHANCED_SECURITY":
                    qual = (ushort)ValidityCtlModel.DIRECR_WITH_ENHANCED_SECURITY;
                    break;
                case "SBO_WITH_ENHANCED_SECURITY":
                    qual = (ushort)ValidityCtlModel.SBO_WITH_ENHANCED_SECURITY;
                    break;
            }
            return qual;
        }

        enum ValidityCtlModel
        {
            STATUS_ONLY = 0,
            DIRECT_WITH_NORMAL_SECURITY = 1,
            SBO_WITH_NORMAL_SECURITY = 2,
            DIRECR_WITH_ENHANCED_SECURITY = 3,
            SBO_WITH_ENHANCED_SECURITY = 4
        }
    }

    //Класс INC (целочисленное управление и состояние)
    public class IncClass : BaseClass
    {
        public Int32 ctlVal;
        public Boolean stVal;
        public Int32 ctlModel;
        public String d;

        public IncClass()
        {
            ctlVal = 0;
            stVal = false;
            ctlModel = 1;
            d = "";
        }

        public void UpdateClass(DateTime time, int ctlValue, bool stValue, string ctrMod)
        {
            ctlVal = ctlValue;
            stVal = stValue;
            ctlModel = MapCtlModel(ctrMod);
            t = time;
            q.UpdateQuality(time);
        }

        private ushort MapCtlModel(string quality)
        {
            ushort qual = 0;
            switch (quality.ToUpper().Replace('-', '_').Replace(" ", ""))
            {
                case "STATUS_ONLY":
                    qual = (ushort)ValidityCtlModel.STATUS_ONLY;
                    break;
                case "DIRECT_WITH_NORMAL_SECURITY":
                    qual = (ushort)ValidityCtlModel.DIRECT_WITH_NORMAL_SECURITY;
                    break;
                case "SBO_WITH_NORMAL_SECURITY":
                    qual = (ushort)ValidityCtlModel.SBO_WITH_NORMAL_SECURITY;
                    break;
                case "DIRECR_WITH_ENHANCED_SECURITY":
                    qual = (ushort)ValidityCtlModel.DIRECR_WITH_ENHANCED_SECURITY;
                    break;
                case "SBO_WITH_ENHANCED_SECURITY":
                    qual = (ushort)ValidityCtlModel.SBO_WITH_ENHANCED_SECURITY;
                    break;
            }
            return qual;
        }

        enum ValidityCtlModel
        {
            STATUS_ONLY = 0,
            DIRECT_WITH_NORMAL_SECURITY = 1,
            SBO_WITH_NORMAL_SECURITY = 2,
            DIRECR_WITH_ENHANCED_SECURITY = 3,
            SBO_WITH_ENHANCED_SECURITY = 4
        }
    }
    #endregion

    #region Спецификации класса общих данных для описательной информации
    // Класс DPL (паспортная табличка устройства)
    public class DplClass
    {
        public string vendor;
        public string hwRev;
        public string swRev;
        public string serNum;
        public string model;
        public string location;

        public void UpdateClass(string vendorStr, string hwRevStr, string swRevStr, string serNumStr, string modelStr, string locationStr)
        {
            vendor = vendorStr;
            hwRev = hwRevStr;
            swRev = swRevStr;
            serNum = serNumStr;
            model = modelStr;
            location = locationStr;
        }

        public DplClass()
        {
            vendor = "Energocomplekt";
            hwRev = "";
            swRev = "";
            serNum = "";
            model = "";
            location = "";
        }
    }

    //Класс LPL (паспортная табличка логического узла)
    public class LplClass
    {
        public string vendor;
        public string swRev;
        public string d;
        public string configRev;

        public void UpdateClass(string vendorStr, string swRevStr, string dStr, string configRevStr)
        {
            vendor = vendorStr;
            swRev = swRevStr;
            d = dStr;
            configRev = configRevStr;
        }

        public LplClass()
        {
            vendor = "Energocomplekt";
            swRev = "";
            d = "";
            configRev = "";
        }
    }
    #endregion
}
