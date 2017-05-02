using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Parser
{
    public class Quality
    {
        public ushort Validity { get; private set; }
        private DetailQuality DetailQual { get;  set; }
        private string Source { get;  set; }
        private bool Test { get;  set; }
        private bool OperatorBlocked { get;  set; }
        private int _allowedAge;

        public Quality()
        {
            Validity = 0;
            DetailQual = new DetailQuality();
            Source = "process";
            Test = false;
            OperatorBlocked = false;
            _dateValueOldUpdateDataObj = DateTime.Now;
            _allowedAge = 1000;
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
                Overflow = status;
            }
        }

        private DateTime _dateValueOldUpdateDataObj; 

        public void UpdateQuality(DateTime time, long value)
        {
            int allowedAge = 1000; //допустимый возраст в мск

            UpdateOldData(_dateValueOldUpdateDataObj.AddMilliseconds(allowedAge) > time);
            _dateValueOldUpdateDataObj = time;

            //Проверки
            UpdateQualityClass();
        }

        public void QualityCheckClass(DateTime time)
        {
             //допустимый возраст в мск

            UpdateOldData(time.AddMilliseconds(_allowedAge) < DateTime.Now);

            //Проверки
            UpdateQualityClass();
        }
        

        public void UpdateOldData(bool status)
        {
            DetailQual.UpdateOldData(status);
        }

        public void UpdateOverflow(bool status)
        {
            DetailQual.UpdateOverflow(status);
        }

        private void UpdateQualityClass()
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
        public int SIUnit;
        public int Multiplier;

        public UnitClass()
        {
            SIUnit = 0;
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
