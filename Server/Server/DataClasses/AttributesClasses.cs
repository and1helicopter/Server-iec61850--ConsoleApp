using System;

namespace Server.DataClasses
{
    public class Quality
    {
        public ushort Validity { get; private set; }
        private DetailQuality DetailQual { get; }
        private string Source { get; }
        private bool Test { get; }
        private bool OperatorBlocked { get;  }
        private readonly int _allowedAge;

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

            public void UpdateBadReference(bool status)
            {
                BadReference = status;
            }
        }

        private DateTime _dateValueOldUpdateDataObj; 

        public void UpdateQuality(DateTime time, object value)
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
            UpdateBadReference(ModBus.ModBus.ErrorPort);

            //Проверки
            UpdateQualityClass();
        }

        private void UpdateBadReference(bool status)
        {
            DetailQual.UpdateBadReference(status);
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

            //badReference
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

            Validity = DetailQual.Overflow ? Convert.ToUInt16(Validity | 0x0004) : Convert.ToUInt16(Validity + 0x0004 ^ 0x0004);
            Validity = DetailQual.OutOfRange ? Convert.ToUInt16(Validity | 0x0008) : Convert.ToUInt16(Validity + 0x0008 ^ 0x0008);
            Validity = DetailQual.BadReference ? Convert.ToUInt16(Validity | 0x0010) : Convert.ToUInt16(Validity + 0x0010 ^ 0x0010);
            Validity = DetailQual.Oscillatory ? Convert.ToUInt16(Validity | 0x0020) : Convert.ToUInt16(Validity + 0x0020 ^ 0x0020);
            Validity = DetailQual.Failure ? Convert.ToUInt16(Validity | 0x0040) : Convert.ToUInt16(Validity + 0x0040 ^ 0x0040);
            Validity = DetailQual.OldData ? Convert.ToUInt16(Validity | 0x0080) : Convert.ToUInt16(Validity + 0x0080 ^ 0x0080);
            Validity = DetailQual.Inconsistent ? Convert.ToUInt16(Validity | 0x0100) : Convert.ToUInt16(Validity + 0x0100 ^ 0x0100);
            Validity = DetailQual.Inaccurate ? Convert.ToUInt16(Validity | 0x0200) : Convert.ToUInt16(Validity + 0x0200 ^ 0x0200);
            Validity = Source != "process" ? Convert.ToUInt16(Validity | 0x0400) : Convert.ToUInt16(Validity + 0x0400 ^ 0x0400);
            Validity = Test ? Convert.ToUInt16(Validity | 0x0800) : Convert.ToUInt16(Validity + 0x0800 ^ 0x0800);
            Validity = OperatorBlocked ? Convert.ToUInt16(Validity | 0x1000) : Convert.ToUInt16(Validity + 0x1000 ^ 0x1000);
        }

        private ushort MapQuality(string quality)
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

    public class ScaledValueClass
    {
        public float ScaleFactor;
        public float Offset;

        public ScaledValueClass()
        {
            ScaleFactor = 1;
            Offset = 0;
        }
    }

    public class VectorClass
    {
        public AnalogueValueClass mag;
        public AnalogueValueClass ang;

        public VectorClass()
        {
            mag = new AnalogueValueClass();
            ang = new AnalogueValueClass();
        }
    }
}
