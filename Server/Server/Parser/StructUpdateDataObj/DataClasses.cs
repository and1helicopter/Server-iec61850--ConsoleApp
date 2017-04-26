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
    
    public class MvClass: BaseClass
    {
        public MagClass Mag;
        public UnitClass Unit;
        public ScaledValueConfigClass sVC;
        public String d;

        public string Addr;

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
    }
}
