using System;
using System.Data;

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
    
    public class MvClass: BaseClass
    {
        public MagClass Mag;
        public UnitClass Unit;
        public ScaledValueConfigClass sVC;
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
            sVC = new ScaledValueConfigClass();
            d = "";
        }

        public void MvClassFill(int siUnit, int multiplier, float scaleFactor, float offset, string str)
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
}
