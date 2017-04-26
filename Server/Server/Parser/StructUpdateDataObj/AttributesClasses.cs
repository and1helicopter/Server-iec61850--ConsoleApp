using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Parser
{
    public class Quality
    {
        public ushort Validity;
        private DetailQuality DetailQual;
        private string Source;
        private bool Test;
        private bool OperatorBlocked; 

        public Quality()
        {
            Validity = 0;
            DetailQual = new DetailQuality();
            Source = "process";
            Test = false;
            OperatorBlocked = false;
        }

        private class DetailQuality
        {
            public bool Overflow;
            public bool OutOfRange;
            public bool BadReference;
            public bool Oscillatory;
            public bool Failure;
            public bool OldData; 
            public bool Inconsistent; 
            public bool Inaccurate;

            public DetailQuality()
            {
                Overflow = false;
                OutOfRange = false;
                BadReference = false;
                Oscillatory = false;
                Failure = false;
                OldData = false;
                Inconsistent = false;
                Inaccurate = false;
            }

            public void UpdateOldData(bool status)
            {
                OldData = status;
            }

            public void UpdateOverflow(bool status)
            {
                OldData = status;
            }
        }

        public void UpdateOldData(bool status)
        {
            DetailQual.UpdateOldData(status);
            UpdateQuality();
        }

        public void UpdateOverflow(bool status)
        {
            DetailQual.UpdateOverflow(status);
            UpdateQuality();
        }

        private void UpdateQuality()
        {
            //Обновляем статус качества
            if (DetailQual.Overflow || DetailQual.BadReference || DetailQual.Oscillatory || DetailQual.Failure)
            {
                Validity = MapQuality("INVALID");
            }
            else if (DetailQual.OutOfRange || DetailQual.OldData || DetailQual.Inconsistent || DetailQual.Inaccurate)
            {
                Validity = MapQuality("QUESTIONABLE");
            }
            else
            {
                Validity = MapQuality("GOOD");
            }
        }

        public ushort MapQuality(string quality)
        {
            ushort qual = 0;
            switch (quality.ToUpper())
            {
                case "GOOD":
                    qual = (ushort)ValidityStatus.GOOD;
                    break;
                case "INVALID":
                    qual = (ushort)ValidityStatus.INVALID;
                    break;
                case "RESERVED":
                    qual = (ushort)ValidityStatus.RESERVED;
                    break;
                case "QUESTIONABLE":
                    qual = (ushort)ValidityStatus.QUESTIONABLE;
                    break;
            }
            return qual;
        }

        enum ValidityStatus
        {
            GOOD = 0,
            RESERVED = 1,
            INVALID = 2,
            QUESTIONABLE = 3
        }
    }

    public class AnalogueValueClass
    {
        public float f;

        public AnalogueValueClass()
        {
            f = 0;
        }
    }

    public class UnitClass
    {
        public int SlUnit;
        public int Multiplier;

        public UnitClass()
        {
            SlUnit = 0;
            Multiplier = 0;
        }
    }

    public class ScaledValueConfigClass
    {
        public float ScaleFactor;
        public float Offset;

        public ScaledValueConfigClass()
        {
            ScaleFactor = 1;
            Offset = 0;
        }
    }
}
