using System;

namespace Server.Parser
{
    public class BaseClass
    {
        public DateTime t;
        public Quality q;

        public BaseClass()
        {
            t = DateTime.Now;
            q = new Quality();
        }
    }
    // Классы общих данных для информации о состоянии
    #region
    //Недублированное состояние
    public class SpsClass : BaseClass
    {
        public Boolean stVal;
        public String d;
        
        public void UpdateClass(DateTime time, bool value)
        {
            stVal = value;
            t = time;
            q.UpdateQuality(time, value);
        }

        public SpsClass()
        {
            stVal = false;
            d = "";
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
            q.UpdateQuality(time, value);
        }

        public InsClass()
        {
            stVal = 0;
            d = "";
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
            q.UpdateQuality(time, value);
        }

        public ActClass()
        {
            general = false;
            d = "";
        }
    }

    //Сведения об активации направленной защиты
    public class AcdClass : BaseClass
    {
        public Boolean general;
        public Int32 dirGeneral;
        public String d;

        public AcdClass()
        {
            general = false;
            dirGeneral = 0;
            d = "";
        }

        public void UpdateClass(DateTime time, bool value, string valueDir)
        {
            general = value;
            if (value) dirGeneral = MapDirGeneral(valueDir);
            t = time;
            q.UpdateQuality(time, value);
        }

        private ushort MapDirGeneral(string quality)
        {
            ushort qual = 0;
            switch (quality.ToUpper())
            {
                case "UNKNOWN":
                    qual = (ushort)ValidityDirGeneral.UNKNOWN;
                    break;
                case "FORWARD":
                    qual = (ushort)ValidityDirGeneral.FORWARD;
                    break;
                case "BACKWARD":
                    qual = (ushort)ValidityDirGeneral.BACKWARD;
                    break;
                case "BOTH":
                    qual = (ushort)ValidityDirGeneral.BOTH;
                    break;
            }
            return qual;
        }

        enum ValidityDirGeneral
        {
            UNKNOWN = 0,
            FORWARD = 1,
            BACKWARD = 2,
            BOTH = 3
        }
    }

    //Считывание показаний двоичного счетчика
    public class BcrClass : BaseClass
    {
        public Int32 actVal;
        public String d;

        public void UpdateClass(DateTime time, int value)
        {
            actVal = value;
            t = time;
            q.UpdateQuality(time, value);
        }

        public BcrClass()
        {
            actVal = 0;
            d = "";
        }
    }
    #endregion

    // Классы общих данных для информации об измеряемой величине
    #region
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

        public void UpdateClass(DateTime time, long value)
        {
            Mag.AnalogueValue.f = Convert.ToSingle(value * sVC.ScaleFactor + sVC.Offset);
            t = time;
            q.UpdateQuality(time, value);
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
            q.UpdateQuality(time, cVal);
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
}
